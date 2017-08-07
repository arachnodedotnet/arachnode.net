#region License : arachnode.net

// Copyright (c) 2014 http://arachnode.net, arachnode.net, LLC
//  
// Permission is hereby granted, upon purchase, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, merge and modify copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following
// conditions:
// 
// LICENSE (ALL VERSIONS/EDITIONS): http://arachnode.net/r.ashx?3
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization.Formatters;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.Renderer.Value;
using Arachnode.Renderer.Value.Enums;
using Arachnode.Renderer.Value.EventArgs;
using AxSHDocVw;
using mshtml;
using SHDocVw;
using Message=System.Messaging.Message;
using Timer=System.Windows.Forms.Timer;
using Arachnode.Proxy;
using System.Runtime.Remoting.Messaging;
using Arachnode.Renderer.Value.AbstractClasses;
using System.Collections.Generic;
using Arachnode.Renderer.RendererActions;
using System.Net.Security;
using System.Net;

#endregion

namespace Arachnode.Renderer
{
    //README: http://www.west-wind.com/weblog/posts/2011/May/21/Web-Browser-Control-Specifying-the-IE-Version

    [ComVisible(true)]
    public partial class Renderer : Form, IOleClientSite, IDocHostShowUI
    {
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetSetCookie(string url, string cookieName, string cookieData);

        private readonly Queue _absoluteUris = new Queue();
        private readonly IArachnodeDAO _arachnodeDAO;
        private readonly HtmlRenderer _htmlRenderer;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly Stopwatch _stopwatchTotal = new Stopwatch();
        private readonly Thread _thread;
        private bool _abortThread;
        private object _absoluteUri = "https://arachnode.net";

        //determines what (if anything) happens when you click 'Test'...
        //if either of these are set to 'true' the main crawling process will not be able to communicate with the Renderers... (set both to 'false')...
        private bool _debugMultipleAbsoluteUris = false;
        private bool _debugSingleAbsoluteUri = false;

        private MessageQueue _engineMessageQueue;
        private IHTMLDocument2 _htmlDocument2;
        private bool _modifyDOM = false;
        private int _numberOfRenderedAbsoluteUris;
        private RendererMessage _rendererMessage;
        private bool _showDebugHtml = false;
        private bool _useAxWebBrowser = false;
        private List<ARendererAction> _rendererActions = new List<ARendererAction>();

        public Renderer()
        {
            try
            {
                InitializeComponent();

                /**/

                //remove limits from service point manager
                ServicePointManager.MaxServicePoints = 10000;
                ServicePointManager.DefaultConnectionLimit = 10000;
                ServicePointManager.CheckCertificateRevocationList = true;
                ServicePointManager.Expect100Continue = false;
                ServicePointManager.MaxServicePointIdleTime = 1000 * 30;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                ServicePointManager.UseNagleAlgorithm = false;

                //Use if you encounter certificate errors...
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

                /**/

                ApplicationSettings applicationSettings = new ApplicationSettings();

                _arachnodeDAO = new ArachnodeDAO(applicationSettings.ConnectionString);

                _htmlRenderer = new HtmlRenderer(_arachnodeDAO);

                Closed += Renderer_Closed;

                if (_useAxWebBrowser && !DesignMode)
                {
                    object o = axWebBrowser1.GetOcx();

                    IOleObject oleObject = o as IOleObject;

                    oleObject.SetClientSite(this);
                }

                axWebBrowser1.Silent = true;

                if (_useAxWebBrowser)
                {
                    Thread thread = new Thread(() =>
                                                   {
                                                       while (true)
                                                       {
                                                           Thread.Sleep(1000 * 60 * 1);

                                                           if (_stopwatch.Elapsed.TotalMinutes > 1)
                                                           {
                                                               _stopwatch.Reset();
                                                               _stopwatch.Start();

                                                               axWebBrowser1.Stop();

                                                               axWebBrowser1_DocumentComplete(this, null);
                                                           }
                                                       }
                                                   });

                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                }

                /**/

                //uncomment these to use...
                //_rendererActions.Add(new IFrames());
                //_rendererActions.Add(new Hrefs());
                //_rendererActions.Add(new Inputs());

                _htmlRenderer.DocumentComplete += _htmlParser_DocumentComplete;

                /**/

                if (_debugSingleAbsoluteUri || _debugMultipleAbsoluteUris)
                {
                    return;
                }

                /**/

                #region Default Crawling Thread
                //both should be set to 'false' for default crawling execution...
                if (!_debugSingleAbsoluteUri && !_debugMultipleAbsoluteUris)
                {
                    _stopwatchTotal.Reset();
                    _stopwatchTotal.Start();

                    _thread = new Thread(delegate()
                                             {
                                                 try
                                                 {
                                                     MessageQueue rendererMessageQueue = new MessageQueue(".\\private$\\Renderer_Renderers:" + 0);
                                                     rendererMessageQueue.Formatter = new XmlMessageFormatter(new[] { typeof(RendererMessage) });

                                                     while (rendererMessageQueue.Peek() == null)
                                                     {
                                                         Thread.Sleep(10);
                                                     }

                                                     Message message = rendererMessageQueue.Receive();

                                                     _rendererMessage = (RendererMessage)message.Body;

                                                     /**/

                                                     rendererMessageQueue = new MessageQueue(".\\private$\\Renderer_Renderers:" + _rendererMessage.ThreadNumber);
                                                     rendererMessageQueue.Formatter = new XmlMessageFormatter(new[] { typeof(RendererMessage) });

                                                     _engineMessageQueue = new MessageQueue(".\\private$\\Renderer_Engine:" + _rendererMessage.ThreadNumber);

                                                     /**/

                                                     //remoting code for Marshalling the HTMLDocumentClass...
                                                     BinaryClientFormatterSinkProvider clientProvider = null;
                                                     BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider();
                                                     serverProvider.TypeFilterLevel = TypeFilterLevel.Full;

                                                     Hashtable props = new Hashtable();
                                                     props["name"] = "Renderer" + _rendererMessage.ThreadNumber;
                                                     props["portName"] = "Renderer" + _rendererMessage.ThreadNumber;
                                                     props["authorizedGroup"] = WindowsIdentity.GetCurrent().Name;
                                                     //props["typeFilterLevel"] = TypeFilterLevel.Full;

                                                     IpcChannel channel = new IpcChannel(props, clientProvider, serverProvider);

                                                     ChannelServices.RegisterChannel(channel, false);

                                                     RemotingConfiguration.RegisterWellKnownServiceType(typeof(Renderer), "Renderer" + _rendererMessage.ThreadNumber, WellKnownObjectMode.SingleCall);
                                                     RemotingServices.Marshal(this, "Renderer" + _rendererMessage.ThreadNumber);

                                                     /**/

                                                     tsslStatus.Text = ".\\private$\\Renderer_Engine:" + _rendererMessage.ThreadNumber + " Awaiting CrawlRequests...";

                                                     while (true && !_abortThread)
                                                     {
                                                         try
                                                         {
                                                             message = rendererMessageQueue.Receive();

                                                             _stopwatch.Reset();
                                                             _stopwatch.Start();

                                                             _rendererMessage = (RendererMessage)message.Body;
                                                             _htmlRenderer.CrawlRequestTimeoutInMinutes = _rendererMessage.CrawlRequestTimeoutInMinutes;

                                                             tsslStatus.Text = DateTime.Now.ToLongTimeString() + " .\\private$\\Renderer_Engine:" + _rendererMessage.ThreadNumber + " " + _rendererMessage.AbsoluteUri + " TimeTakenToReceiveMessage: " + _stopwatch.Elapsed.TotalSeconds;

                                                             if (!_rendererMessage.Kill)
                                                             {
                                                                 switch (_rendererMessage.RenderAction)
                                                                 {
                                                                     case RenderAction.Render:
                                                                         if (!string.IsNullOrEmpty(_rendererMessage.ProxyServer))
                                                                         {
                                                                             ConnectionProxy.SetConnectionProxy(_rendererMessage.ProxyServer.TrimEnd('/'));
                                                                         }
                                                                         else
                                                                         {
                                                                             ConnectionProxy.RestoreSystemProxy();
                                                                         }

                                                                         if (!string.IsNullOrEmpty(_rendererMessage.Cookie))
                                                                         {
                                                                             //key1=value1;key2=value2;

                                                                             if (!string.IsNullOrEmpty(_rendererMessage.Cookie))
                                                                             {
                                                                                 string[] cookieSplit = _rendererMessage.Cookie.Split(";".ToCharArray());

                                                                                 foreach (string cookieSplit2 in cookieSplit)
                                                                                 {
                                                                                     string[] cookieSplit3 = cookieSplit2.Split("=".ToCharArray());

                                                                                     if (cookieSplit3.Length >= 2)
                                                                                     {
                                                                                         StringBuilder stringBuilder = new StringBuilder();

                                                                                         for (int i = 1; i < cookieSplit3.Length; i++)
                                                                                         {
                                                                                             stringBuilder.Append(cookieSplit3[i] + "=");
                                                                                         }
                                                                                         string value = stringBuilder.ToString().TrimEnd("=".ToCharArray());

                                                                                         InternetSetCookie(_rendererMessage.AbsoluteUri, cookieSplit3[0], cookieSplit3[1]);
                                                                                     }
                                                                                 }
                                                                             }
                                                                         }

                                                                         if (_useAxWebBrowser)
                                                                         {
                                                                             object userAgent = "User-Agent: " + _rendererMessage.UserAgent;
                                                                             object o1 = null;
                                                                             object o2 = null;
                                                                             object o3 = null;
                                                                             DateTime startTime = DateTime.Now;

                                                                             axWebBrowser1.Navigate(_rendererMessage.AbsoluteUri, ref o1, ref o2, ref o3, ref userAgent);

                                                                             if (_modifyDOM)
                                                                             {
                                                                                 bool wasDOMModified = false;

                                                                                 while (axWebBrowser1.ReadyState != tagREADYSTATE.READYSTATE_COMPLETE && DateTime.Now.Subtract(startTime).Duration().TotalMinutes < _rendererMessage.CrawlRequestTimeoutInMinutes)
                                                                                 {
                                                                                     Thread.Sleep(100);

                                                                                     if (axWebBrowser1.ReadyState == tagREADYSTATE.READYSTATE_INTERACTIVE)
                                                                                     {
                                                                                         if (!wasDOMModified)
                                                                                         {
                                                                                             _htmlRenderer.ModifyDOM((IHTMLDocument2)axWebBrowser1.Document, false);

                                                                                             wasDOMModified = true;
                                                                                         }
                                                                                     }
                                                                                 }
                                                                             }
                                                                         }
                                                                         else
                                                                         {
                                                                             _htmlRenderer.Render(_rendererMessage.AbsoluteUri);
                                                                         }
                                                                         break;
                                                                     case RenderAction.Back:
                                                                         axWebBrowser1.GoBack();
                                                                         break;
                                                                     case RenderAction.Forward:
                                                                         axWebBrowser1.GoForward();
                                                                         break;
                                                                 }

                                                                 try
                                                                 {
                                                                     foreach (Process process in Process.GetProcesses())
                                                                     {
                                                                         if (process.ProcessName.ToLowerInvariant() == "iexplore" ||
                                                                             process.ProcessName.ToLowerInvariant() == "chrome" ||
                                                                             process.ProcessName.ToLowerInvariant() == "vsjitdebugger" ||
                                                                             process.MainWindowTitle.ToLowerInvariant() == "web browser" ||
                                                                             process.MainWindowTitle.ToLowerInvariant() == "renderer" ||
                                                                             process.MainWindowTitle.ToLowerInvariant() == "visual studio just-in-time debugger")
                                                                         {
                                                                             //if (MessageBox.Show("Close? 1", "Arachnode.Renderer", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                                                             //{
                                                                             //    process.Kill();
                                                                             //}
                                                                         }
                                                                     }

                                                                     IntPtr window = WinApis.FindWindowByCaption(IntPtr.Zero, "Web Browser");

                                                                     if (window != IntPtr.Zero)
                                                                     {
                                                                         WinApis.CloseWindow(window);
                                                                     }

                                                                     window = WinApis.FindWindowByCaption(IntPtr.Zero, "Message from webpage");

                                                                     if (window != IntPtr.Zero)
                                                                     {
                                                                         WinApis.CloseWindow(window);
                                                                     }
                                                                 }
                                                                 catch (Exception exception)
                                                                 {
                                                                     //MessageBox.Show(exception.Message);
                                                                     //MessageBox.Show(exception.StackTrace);

                                                                     _arachnodeDAO.InsertException(null, null, exception, false);
                                                                 }
                                                             }
                                                             else
                                                             {
                                                                 //if (MessageBox.Show("Close? 2", "Arachnode.Renderer", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                                                 //{
                                                                 //    Process.GetCurrentProcess().Kill();
                                                                 //}
                                                             }
                                                         }
                                                         catch (Exception exception)
                                                         {
                                                             //MessageBox.Show(exception.Message);
                                                             //MessageBox.Show(exception.StackTrace);

                                                             _arachnodeDAO.InsertException(null, null, exception, false);
                                                         }
                                                     }
                                                 }
                                                 catch (Exception exception)
                                                 {
                                                     //MessageBox.Show(exception.Message);
                                                     //MessageBox.Show(exception.StackTrace);

                                                     _arachnodeDAO.InsertException(null, null, exception, false);
                                                 }
                                             });

                    _thread.Start();
                }

                #endregion
            }
            catch(Exception exception)
            {
                if(_arachnodeDAO != null)
                {
                    _arachnodeDAO.InsertException(null, null, exception, false);
                }
                else
                {
                    MessageBox.Show(exception.Message + " :: " + exception.StackTrace.ToString());

                    Application.Exit();
                    Process.GetCurrentProcess().Kill();
                }
            }
        }

        private void Renderer_Closed(object sender, EventArgs e)
        {
            _abortThread = true;

            ManageRendererProcesses();
        }

        private void Renderer_FormClosed(object sender, FormClosedEventArgs e)
        {
            _abortThread = true;

            ManageRendererProcesses();
        }

        private static void ManageRendererProcesses()
        {
            Process currentProcess = Process.GetCurrentProcess();

            foreach (Process process in Process.GetProcesses().Where(_ => _.Id != currentProcess.Id))
            {
                if (process.MainWindowTitle.ToLowerInvariant().Contains("arachnode.net | renderer") || process.ProcessName.Contains("arachnode.renderer"))
                {
                    process.Kill();
                }
            }

            Process.GetCurrentProcess().Kill();
        }

        public HTMLDocumentClass HtmlDocumentClass { get; set; }

        #region IOleClientSite Members

        int IOleClientSite.SaveObject()
        {
            return 0;
        }

        int IOleClientSite.GetMoniker(uint dwAssign, uint dwWhichMoniker, out Object ppmk)
        {
            ppmk = null;
            return 0;
        }

        int IOleClientSite.GetContainer(out IOleContainer ppContainer)
        {
            ppContainer = null;
            return 0;
        }

        int IOleClientSite.ShowObject()
        {
            return 0;
        }

        int IOleClientSite.OnShowWindow(int fShow)
        {
            return 0;
        }

        int IOleClientSite.RequestNewObjectLayout()
        {
            return 0;
        }

        #endregion

        #region Implementation of IDocHostShowUI

        public int ShowMessage(IntPtr hwnd, string lpstrText, string lpstrCaption, uint dwType, string lpstrHelpFile, uint dwHelpContext, ref int lpResult)
        {
            return 0;
        }

        public int ShowHelp(IntPtr hwnd, string pszHelpFile, uint uCommand, uint dwData, tagPOINT ptMouse, object pDispatchObjectHit)
        {
            return 0;
        }

        #endregion

        private void btnTest_Click(object sender, EventArgs e)
        {
            btnTest.Enabled = false;

            _stopwatchTotal.Reset();
            _stopwatchTotal.Start();

            _numberOfRenderedAbsoluteUris = 0;

            if (_debugMultipleAbsoluteUris)
            {
                foreach (string absoluteUri2 in File.ReadAllLines("CrawlRequests.txt"))
                {
                    _absoluteUris.Enqueue(absoluteUri2);
                }

                _absoluteUri = _absoluteUris.Dequeue().ToString();
            }

            _stopwatch.Reset();
            _stopwatch.Start();

            if (_useAxWebBrowser)
            {
                BeginInvoke(new MethodInvoker(delegate
                                                  {
                                                      axWebBrowser1.Navigate2(ref _absoluteUri);

                                                      if (_modifyDOM)
                                                      {
                                                          bool wasDOMModified = false;

                                                          while (axWebBrowser1.ReadyState != tagREADYSTATE.READYSTATE_COMPLETE)
                                                          {
                                                              Thread.Sleep(100);

                                                              if (axWebBrowser1.ReadyState == tagREADYSTATE.READYSTATE_INTERACTIVE)
                                                              {
                                                                  if (!wasDOMModified)
                                                                  {
                                                                      _htmlRenderer.ModifyDOM((IHTMLDocument2)axWebBrowser1.Document, false);

                                                                      wasDOMModified = true;
                                                                  }
                                                              }
                                                          }
                                                      }
                                                  }));
            }
            else
            {
                _htmlRenderer.Render(_absoluteUri.ToString());
            }

            tsslStatus.Text = "arachnode.net | renderer";
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        [DispId(-5512)]
        public virtual int IDispatch_Invoke_Handler()
        {
            return (int) (BrowserOptions.DownloadOnly | BrowserOptions.Silent | BrowserOptions.NoActiveXDownload | BrowserOptions.DontRunActiveX | BrowserOptions.NoClientPull | BrowserOptions.NoFrameDownload | BrowserOptions.NoJava | 0);
        }

        private void axWebBrowser1_NavigateError(object sender, DWebBrowserEvents2_NavigateErrorEvent e)
        {
            e.cancel = true;
        }

        private void axWebBrowser1_FileDownload(object sender, DWebBrowserEvents2_FileDownloadEvent e)
        {
            e.cancel = true;
        }

        private void axWebBrowser1_NewWindow2(object sender, DWebBrowserEvents2_NewWindow2Event e)
        {
            e.cancel = true;
        }

        private void axWebBrowser1_NewWindow3(object sender, DWebBrowserEvents2_NewWindow3Event e)
        {
            e.cancel = true;
        }

        private void axWebBrowser1_NavigateComplete2(object sender, DWebBrowserEvents2_NavigateComplete2Event e)
        {
            _htmlDocument2 = (IHTMLDocument2) axWebBrowser1.Document;

            HTMLWindowEvents2_Event onErrorEvent = (HTMLWindowEvents2_Event) _htmlDocument2.parentWindow;

            onErrorEvent.onerror += myHTMLWindowEvents2_onerror;
        }

        private void axWebBrowser1_DocumentComplete(object sender, DWebBrowserEvents2_DocumentCompleteEvent e)
        {
            _htmlDocument2 = (IHTMLDocument2) axWebBrowser1.Document;

            HTMLWindowEvents2_Event onErrorEvent = (HTMLWindowEvents2_Event) _htmlDocument2.parentWindow;

            onErrorEvent.onerror += myHTMLWindowEvents2_onerror;

            /**/

            try
            {
                //this shouldn't be null, but if it is (using the Renderer as the start up project)... set it to something to be able to debug the RendererActions...
                if (_rendererMessage == null)
                {
                    _rendererMessage = new RendererMessage();
                }

                if (_rendererMessage != null)
                {
                    HtmlDocumentClass = (HTMLDocumentClass) axWebBrowser1.Document;
                    //turn off 'Disable Script Debugging (Other)' in 'Advanced > Settings > Browsing' in IE if you encounter script errors...

                    ProcessRendererActions();

                    if (_engineMessageQueue != null)
                    {
                        _engineMessageQueue.Send(_rendererMessage);
                    }
                }

                BeginInvoke(new MethodInvoker(delegate
                {
                    tsslStatus.Text = "arachnode.net | renderer :: " + axWebBrowser1.LocationURL + " : " + _stopwatch.Elapsed;
                    tsslWorkingSet.Text = "WorkingSet: " + (Environment.WorkingSet/1024).ToString("###,###,###,###") + " KB";
                    tsslNumberOfRenderedAbsoluteUris.Text = "NumberOfRenderedAbsoluteUris: " + (++_numberOfRenderedAbsoluteUris).ToString();
                    tsslElapsedTime.Text = "ElapsedTime: " + _stopwatchTotal.Elapsed.ToString();
                    tsslWebPagePerSecond.Text = "WebPages/sec.: " + (_numberOfRenderedAbsoluteUris/_stopwatchTotal.Elapsed.TotalSeconds);

                    _stopwatch.Reset();
                    _stopwatch.Start();

                    if (_showDebugHtml && axWebBrowser1.Document != null && ((IHTMLDocument2) axWebBrowser1.Document).body != null)
                    {
                        rtbDebug.Text = ((IHTMLDocument2) axWebBrowser1.Document).body.innerHTML;
                    }
                    else
                    {
                        rtbDebug.Text = "Set _showDebugHtml = true; to see the HTML source.";
                    }

                    if (_absoluteUris.Count != 0)
                    {
                        _absoluteUri = _absoluteUris.Dequeue().ToString();

                        axWebBrowser1.Navigate2(ref _absoluteUri);
                    }
                    else
                    {
                        tsslStatus.Text += " : Done";

                        btnTest.Enabled = true;
                    }
                }));
            }
            catch (Exception exception)
            {
                _arachnodeDAO.InsertException(null, null, exception, false);
            }
        }

        private void _htmlParser_DocumentComplete(object sender, DocumentCompleteEventArgs e)
        {
            try
            {
                //this shouldn't be null, but if it is (using the Renderer as the start up project)... set it to something to be able to debug the RendererActions...
                if (_rendererMessage == null)
                {
                    _rendererMessage = new RendererMessage();
                }

                if (_rendererMessage != null)
                {
                    HtmlDocumentClass = (HTMLDocumentClass) e.HtmlDocument;

                    ProcessRendererActions();

                    if (_engineMessageQueue != null)
                    {
                        _engineMessageQueue.Send(_rendererMessage);
                    }
                }

                BeginInvoke(new MethodInvoker(delegate
                {
                    tsslStatus.Text = _htmlRenderer.AbsoluteUri + " : " + _stopwatch.Elapsed;
                    tsslWorkingSet.Text = "WorkingSet: " + (Environment.WorkingSet/1024).ToString("###,###,###,###") + " KB";
                    tsslNumberOfRenderedAbsoluteUris.Text = "NumberOfRenderedAbsoluteUris: " + (++_numberOfRenderedAbsoluteUris).ToString();
                    tsslElapsedTime.Text = "ElapsedTime: " + _stopwatchTotal.Elapsed.ToString();
                    tsslWebPagePerSecond.Text = "WebPages/sec.: " + (_numberOfRenderedAbsoluteUris/_stopwatchTotal.Elapsed.TotalSeconds);

                    _stopwatch.Reset();
                    _stopwatch.Start();

                    if (_showDebugHtml && e.HtmlDocument != null && e.HtmlDocument.body != null)
                    {
                        rtbDebug.Text = e.HtmlDocument.body.innerHTML;
                    }
                    else
                    {
                        rtbDebug.Text = "Set _showDebugHtml = true; to see the HTML source.";
                    }

                    if (_absoluteUris.Count != 0)
                    {
                        string absoluteUri = _absoluteUris.Dequeue().ToString();

                        _htmlRenderer.Render(absoluteUri);
                    }
                    else
                    {
                        tsslStatus.Text += " : Done";

                        btnTest.Enabled = true;
                    }
                }));
            }
            catch (Exception exception)
            {
                _arachnodeDAO.InsertException(null, null, exception, false);
            }
        }

        private void ProcessRendererActions()
        {
            _rendererMessage.PropertiesKeys = new System.Collections.Generic.List<object>();
            _rendererMessage.PropertiesValues = new System.Collections.Generic.List<object>();

            foreach (ARendererAction rendererAction in _rendererActions)
            {
                bool hasCompleted = false;

                BeginInvoke(new MethodInvoker(delegate
                {
                    try
                    {
                        rendererAction.PerformAction(_rendererMessage, HtmlDocumentClass);
                    }
                    catch (Exception exception)
                    {
                        _arachnodeDAO.InsertException(_rendererMessage.AbsoluteUri, null, exception, false);
                    }
                    finally
                    {
                        hasCompleted = true;
                    }
                }));

                while (!hasCompleted)
                {
                    Thread.Sleep(100);
                }
            }

            _rendererMessage.RenderTime = _stopwatch.Elapsed;
        }

        private void myHTMLWindowEvents2_onerror(string description, string url, int line)
        {
            IHTMLEventObj eventObj = _htmlDocument2.parentWindow.@event;

            if (eventObj != null)
            {
                eventObj.returnValue = true;
            }
        }

        private void axWebBrowser1_ThirdPartyUrlBlocked(object sender, DWebBrowserEvents2_ThirdPartyUrlBlockedEvent e)
        {
            MessageBox.Show("axWebBrowser1_ThirdPartyUrlBlocked");
            MessageBox.Show(e.uRL.ToString());
        }

        private void axWebBrowser1_RedirectXDomainBlocked(object sender, DWebBrowserEvents2_RedirectXDomainBlockedEvent e)
        {
            MessageBox.Show("axWebBrowser1_RedirectXDomainBlocked");
            MessageBox.Show(e.startURL.ToString());
        }
    }
}
