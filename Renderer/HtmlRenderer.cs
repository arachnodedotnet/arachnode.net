#region License : arachnode.net

// Copyright (c) 2015 http://arachnode.net, arachnode.net, LLC
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
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Windows.Forms;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.Renderer.Value;
using Arachnode.Renderer.Value.Enums;
using Arachnode.Renderer.Value.EventArgs;
using mshtml;
using SHDocVw;

#endregion

namespace Arachnode.Renderer
{
    //README: http://stackoverflow.com/questions/790542/replacing-net-webbrowser-control-with-a-better-browser-like-chrome
    //README: http://msdn.microsoft.com/en-us/library/ee330720(v=vs.85)

    [ComVisible(true)]
    public class HtmlRenderer : IOleClientSite, IDocHostShowUI, IHostDialogHelper
    {
        private readonly ManualResetEvent _parse = new ManualResetEvent(false);
        private string _absoluteUri;
        private DateTime _parseStartTime;
        public IHTMLDocument4 MSHTML;

        public HtmlRenderer(IArachnodeDAO arachnodeDAO)
        {
            Type htmldoctype = Type.GetTypeFromCLSID(Iid_Clsids.CLSID_HTMLDocument, true);
            //Using Activator inplace of CoCreateInstance, returns IUnknown
            //which we cast to a IHtmlDocument2 interface
            MSHTML = (IHTMLDocument4)Activator.CreateInstance(htmldoctype);

            IPersistStreamInit ips = (IPersistStreamInit) MSHTML;
            ips.InitNew();

            IOleObject oleObject = (IOleObject) MSHTML;
            //Set client site
            int iret = oleObject.SetClientSite(this);

            CrawlRequestTimeoutInMinutes = 1;

            //Getting exceptions when trying to get change notification through IPropertyNotifySink and connectionpointcontainer.
            //So, using this technique.
            Thread t = new Thread(() =>
                                      {
                                          string previousReadyState = "";
                                          while (true)
                                          {
                                              try
                                              {
                                                  if (_parse.WaitOne())
                                                  {
                                                      if (DateTime.Now.Subtract(_parseStartTime).TotalMinutes < CrawlRequestTimeoutInMinutes)
                                                      {
                                                          string currentReadyState = ((IHTMLDocument2)MSHTML).readyState;

                                                          if (string.Compare(currentReadyState, previousReadyState, true) != 0)
                                                          {
                                                              previousReadyState = currentReadyState;

                                                              ReadyState = ReadyState.Uninitialized;
                                                              switch (currentReadyState.ToLower())
                                                              {
                                                                  case "loading":
                                                                      ReadyState = ReadyState.Loading;
                                                                      break;
                                                                  case "loaded":
                                                                      ReadyState = ReadyState.Loaded;
                                                                      break;
                                                                  case "interactive":
                                                                      ReadyState = ReadyState.Interactive;
                                                                      ModifyDOM(((IHTMLDocument2)MSHTML), true);
                                                                      break;
                                                                  case "complete":
                                                                      ReadyState = ReadyState.Complete;
                                                                      break;
                                                                  default:
                                                                      ReadyState = ReadyState.Uninitialized;
                                                                      break;
                                                              }

                                                              if (ReadyStateChange != null)
                                                              {
                                                                  ReadyStateChange(this, new ReadyStateChangeEventArgs(ReadyState, ((IHTMLDocument2)MSHTML)));
                                                              }
                                                          }

                                                          if (ReadyState == ReadyState.Complete)
                                                          {
                                                              EndRendering();

                                                              previousReadyState = "";
                                                          }
                                                      }
                                                      else
                                                      {
                                                          throw new Exception("The AbsoluteUri timed out while rendering.");
                                                      }

                                                      Thread.Sleep(100);
                                                  }
                                              }
                                              catch (Exception exception)
                                              {
                                                  EndRendering();

                                                  previousReadyState = "";

                                                  arachnodeDAO.InsertException(AbsoluteUri, AbsoluteUri, exception, false);
                                              }
                                          }
                                      });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        internal double CrawlRequestTimeoutInMinutes { get; set; }

        public ReadyState ReadyState { get; private set; }

        public string AbsoluteUri
        {
            get { return _absoluteUri; }
        }

        public void EndRendering()
        {
            _parse.Reset();

            ((IHTMLDocument2)MSHTML).execCommand("Stop", false, true);
            ((IHTMLDocument2)MSHTML).execCommand("UnloadDocument", false, true);

            if (DocumentComplete != null)
            {
                DocumentComplete(this, new DocumentCompleteEventArgs(((IHTMLDocument2)MSHTML), false));
            }            
        }

        public event ReadyStateChangeHandler ReadyStateChange;
        public event DocumentCompleteHandler DocumentComplete;
        public event HTMLWindowEvents2_onerrorEventHandler onerror;

        public void Render(string absoluteUri)
        {
            /**/

            ReadyState = ReadyState.Uninitialized;
            _absoluteUri = absoluteUri;

            IMoniker moniker = null;
            WinApis.CreateURLMonikerEx(null, absoluteUri, out moniker, 1);
            if (moniker == null)
            {
                //MessageBox.Show("Moniker is NULL.");

                return;
            }
            IBindCtx bindContext = null;
            WinApis.CreateBindCtx(0, out bindContext);
            if (bindContext == null)
            {
                //MessageBox.Show("Binding context is NULL.");

                return;
            }

            IPersistMoniker persistMoniker = (IPersistMoniker)MSHTML;
            persistMoniker.Load(0, moniker, bindContext, 0);

            _parseStartTime = DateTime.Now;
            _parse.Set();
        }

        public virtual void ModifyDOM(IHTMLDocument2 document, bool addEventHandler)
        {
            ((IHTMLDocument3) document).documentElement.insertAdjacentHTML("afterBegin", "<script language='javascript' type='text/css'>window.onerror = function(e){return true;}</script>");
            ((IHTMLDocument3) document).documentElement.insertAdjacentHTML("afterBegin", "<script language='javascript'>window.alert = function () { }</script>");

            IHTMLDocument3 doc = (IHTMLDocument3) document;

            IHTMLElementCollection linkElements = doc.getElementsByTagName("LINK");

            foreach (IHTMLLinkElement linkElement in linkElements)
            {
                linkElement.disabled = true;
            }

            //Also disable style elements, so that @import inside styles won't trigger download.
            IHTMLElementCollection styleElements = doc.getElementsByTagName("STYLE");

            foreach (IHTMLStyleElement styleElement in styleElements)
            {
                styleElement.disabled = true;
            }

            //BrowserOptions.NoFrameDownload seems to work only for FRAME elements and not the IFRAME elements.
            IHTMLElementCollection iframeElements = doc.getElementsByTagName("IFRAME");

            foreach (IHTMLElement iframeElement in iframeElements)
            {
                //iframeElement.setAttribute("src", "about:blank");
            }

            if (addEventHandler)
            {
                HTMLWindowEvents2_Event onErrorEvent = (HTMLWindowEvents2_Event) document.parentWindow;

                onErrorEvent.onerror += myHTMLWindowEvents2_onerror;
            }
        }

        private void myHTMLWindowEvents2_onerror(string description, string url, int line)
        {
            if (onerror != null)
            {
                onerror(description, url, line);
            }
        }

        #region IOleClientSite

        int IOleClientSite.SaveObject()
        {
            return 0;
        }

        int IOleClientSite.GetMoniker(uint dwAssign, uint dwWhichMoniker, out object ppmk)
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
            return Hresults.S_OK;
        }

        int IOleClientSite.OnShowWindow(int fShow)
        {
            return Hresults.S_OK;
        }

        int IOleClientSite.RequestNewObjectLayout()
        {
            return Hresults.S_OK;
        }

        [DispId(-5512)]
        public virtual int IDispatch_Invoke_Handler()
        {
            return (int) (BrowserOptions.DownloadOnly | BrowserOptions.Silent |
                          BrowserOptions.NoActiveXDownload | BrowserOptions.DontRunActiveX | BrowserOptions.NoJava |
                          BrowserOptions.NoClientPull | BrowserOptions.NoFrameDownload | 0);
        }

        #endregion

        #region IDocHostShowUI

        int IDocHostShowUI.ShowMessage(IntPtr hwnd, string lpstrText, string lpstrCaption, uint dwType, string lpstrHelpFile, uint dwHelpContext, ref int lpResult)
        {
            return Hresults.S_OK;
        }

        int IDocHostShowUI.ShowHelp(IntPtr hwnd, string pszHelpFile, uint uCommand, uint dwData, tagPOINT ptMouse, object pDispatchObjectHit)
        {
            return Hresults.S_OK;
        }

        #endregion

        #region IHostDialogHelper

        int IHostDialogHelper.ShowHTMLDialog(IntPtr hwndParent, IMoniker pMk, ref object pvarArgIn, string pchOptions, ref object pvarArgOut, object punkHost)
        {
            return Hresults.S_OK;
        }

        #endregion
    }
}