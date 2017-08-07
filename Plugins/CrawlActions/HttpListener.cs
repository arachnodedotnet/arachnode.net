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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.DataSource;
using Arachnode.DataSource.ArachnodeDataSetTableAdapters;
using Arachnode.Proxy.Clients;
using Arachnode.Proxy.Value.AbstractClasses;
using Arachnode.Renderer.Value.Enums;
using Arachnode.SiteCrawler;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.AbstractClasses;
using Arachnode.SiteCrawler.Value.Enums;

#endregion

namespace Arachnode.Plugins.CrawlActions
{
    public class HttpListener<TArachnodeDAO> : ACrawlAction<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        private readonly TArachnodeDAO _arachnodeDAO;
        private readonly object _arachnodeDAOLock = new object();

        private Proxy.Listeners.HttpListener _httpListener;
        private Crawler<TArachnodeDAO> _crawler;

        public HttpListener(ApplicationSettings applicationSettings, WebSettings webSettings)
            : base(applicationSettings, webSettings)
        {
            _arachnodeDAO = (TArachnodeDAO)Activator.CreateInstance(typeof(TArachnodeDAO), applicationSettings.ConnectionString);
            _arachnodeDAO.ApplicationSettings = applicationSettings;
        }

        public override void AssignSettings(Dictionary<string, string> settings)
        {
            //set the default proxy for all in-process WebRequests...
            //if using the Renderers, set the proxy address in IE as well...
            WebProxy webProxy = new WebProxy(settings["ProxyServerScheme"] + "://" + settings["ProxyServerIPAddress"] + ":" + settings["ProxyServerPort"]);
            WebRequest.DefaultWebProxy = webProxy;

            Thread thread = new Thread(() =>
                                           {
                                               _httpListener = new Proxy.Listeners.HttpListener(IPAddress.Parse(settings["ProxyServerIPAddress"]), int.Parse(settings["ProxyServerPort"]));

                                               _httpListener.Start();

                                               _httpListener.OnClientAdded += httpListener_OnClientAdded;
                                               _httpListener.OnClientRemoved += _httpListener_OnClientRemoved;

                                               while (_httpListener.Listening)
                                               {
                                                   Thread.Sleep(100);
                                               }
                                           });

            thread.Start();

            TArachnodeDAO arachnodeDAO = (TArachnodeDAO)Activator.CreateInstance(typeof(TArachnodeDAO), _applicationSettings, _webSettings);
            arachnodeDAO.ExecuteSql("Truncate Table HttpListener");

            ////to test from a user's browser...
            //while (true)
            //{
            //    Application.DoEvents();

            //    Thread.Sleep(100);
            //}
        }

        private void httpListener_OnClientAdded(HttpClient httpClient)
        {
            httpClient.OnClientSentEnd += client_OnClientSentEnd;
            httpClient.OnRemoteReceivedEnd += client_OnRemoteReceivedEnd;
            httpClient.OnRemoteSentEnd += client_OnRemoteSentEnd;
            httpClient.OnClientReceiveEnd += client_OnClientReceiveEnd;

            httpClient.OnConnectedEnd += httpClient_OnConnectedEnd;
            httpClient.OnStartHandshakeEnd += httpClient_OnStartHandshakeEnd;
            httpClient.OnErrorSentEnd += httpClient_OnErrorSentEnd;
            httpClient.OnProcessQueryEnd += httpClient_OnProcessQueryEnd;
            httpClient.OnQuerySentEnd += httpClient_OnQuerySentEnd;
            httpClient.OnStartHandshakeEnd += httpClient_OnStartHandshakeEnd;
            httpClient.OnOkSentEnd += httpClient_OnOkSentEnd;
            httpClient.OnReceiveQueryEnd += httpClient_OnReceiveQueryEnd;
        }

        private void _httpListener_OnClientRemoved(Client client)
        {
        }

        /**/

        Arachnode.DataSource.ArachnodeDataSetTableAdapters.HttpListenerTableAdapter _httpListenerTableAdapter = new HttpListenerTableAdapter();
        private HashSet<string> _extraCrawlRequests = new HashSet<string>();

        protected void Log(HttpClient httpClient, string headers, List<byte> buffer, bool isComplete)
        {
            try
            {
                if (isComplete || true)
                {
                    string referer = null;
                    bool isGZip = false;
                    string httpHeadersString = null;
                    string bufferString = null;
                    int bufferLength = 0;

                    if (headers != null)
                    {
                        WebHeaderCollection httpHeaders = httpClient.ParseHeaders(headers, true);

                        if (httpClient.DoHeadersContainKey(httpHeaders, "Requesting-Process"))
                        {
                            if (httpHeaders["Requesting-Process"] == Process.GetCurrentProcess().ProcessName)
                            {
                                //return;
                            }
                        }
                        
                        if (httpClient.DoHeadersContainKey(httpHeaders, "referer"))
                        {
                            referer = Regex.Replace(httpHeaders["referer"], "://www.", "://", RegexOptions.IgnoreCase);
                        }
                        else
                        {
                            referer = httpClient.RequestedAbsoluteUri;
                            referer = httpClient.OriginallyRequestedAbsoluteUri;
                        }
                        if (httpClient.DoHeadersContainKey(httpHeaders, "content-encoding"))
                        {
                            string contentType = httpHeaders["content-encoding"];
                            isGZip = !string.IsNullOrEmpty(contentType) && contentType.ToLowerInvariant().Contains("gzip");
                        }
                        httpHeadersString = httpHeaders.ToString();

                        //if (_crawler != null)
                        //{
                        //    bool ok = _crawler.Crawl(new CrawlRequest(new Discovery(referer), new Discovery(httpClient.RequestedAbsoluteUri), 1, 2, (short)UriClassificationType.Domain, (short)UriClassificationType.Domain, 1, RenderType.None, RenderType.None));

                        //    if (ok)
                        //    {
                        //        _extraCrawlRequests.Add(httpClient.RequestedAbsoluteUri);
                        //    }
                        //}
                    }

                    if (buffer != null)
                    {
                        if (!isGZip)
                        {
                            bufferString = Encoding.Default.GetString(buffer.ToArray()).Replace("\0", string.Empty);
                        }
                        else
                        {
                            byte[] data = null;

                            int numberOfBytesRead = -1;
                            byte[] buffer2 = new byte[4096];

                            using (MemoryStream memoryStream = new MemoryStream(buffer.ToArray()))
                            {
                                using (MemoryStream memoryStream2 = new MemoryStream())
                                {
                                    using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                                    {
                                        while (numberOfBytesRead != 0)
                                        {
                                            numberOfBytesRead = gZipStream.Read(buffer2, 0, 4096);

                                            memoryStream2.Write(buffer2, 0, numberOfBytesRead);
                                        }

                                        data = memoryStream2.ToArray();

                                        bufferString = Encoding.Default.GetString(data);
                                    }
                                }
                            }
                        }
                        bufferLength = buffer.Count;
                        if (bufferString.Length == 0)
                        {
                            bufferString = null;
                        }
                    }

                    lock (_arachnodeDAOLock)
                    {
                        _httpListenerTableAdapter.Insert(httpClient.InstanceID, DateTime.Now, new StackTrace().GetFrame(2).GetMethod().Name + "->" + new StackTrace().GetFrame(1).GetMethod().Name, httpClient.ClientSocket == null ? null : httpClient.ClientSocket.LocalEndPoint.ToString(), httpClient.ClientSocket == null ? null : httpClient.ClientSocket.RemoteEndPoint.ToString(), httpClient.DestinationSocket == null ? null : httpClient.DestinationSocket.LocalEndPoint.ToString(), httpClient.DestinationSocket == null ? null : httpClient.DestinationSocket.RemoteEndPoint.ToString(), httpClient.ToString(true), isComplete, referer, httpClient.RequestedAbsoluteUri, httpHeadersString, bufferString, bufferLength);
                    }
                }
            }
            catch (Exception exception)
            {
                lock (_arachnodeDAOLock)
                {
                    _arachnodeDAO.InsertException(null, null, exception, false);
                }
            }
        }

        /**/

        //these events are presented in the general order in which they are first called...
        protected virtual void httpClient_OnReceiveQueryEnd(Socket destination, HttpClient httpClient)
        {
            Log(httpClient, httpClient.CurrentHttpQuery, null, false);
        }

        protected virtual void httpClient_OnProcessQueryEnd(Socket destination, HttpClient httpClient)
        {
            Log(httpClient, httpClient.CurrentHttpQuery, null, false);
        }

        protected virtual void httpClient_OnConnectedEnd(Socket destination, HttpClient httpClient)
        {
            Log(httpClient, httpClient.CurrentHttpQuery, null, false);
        }

        protected virtual void httpClient_OnQuerySentEnd(Socket destination, HttpClient httpClient)
        {
            Log(httpClient, httpClient.CurrentHttpQuery, null, false);
        }

        protected virtual void client_OnRemoteReceivedEnd(Socket destinationSocket, Client client, string headers, List<byte> buffer, bool isComplete)
        {
            if (client is HttpClient)
            {
                Log((HttpClient)client, headers, buffer, isComplete);
            }
        }

        protected virtual void client_OnClientSentEnd(Socket clientSocket, Client client, string headers, List<byte> buffer, bool isComplete)
        {
            if (client is HttpClient)
            {
                Log((HttpClient)client, headers, buffer, isComplete);
            }
        }

        protected virtual void client_OnRemoteSentEnd(Socket clientSocket, Client client, string headers, List<byte> buffer, bool isComplete)
        {
            if (client is HttpClient)
            {
                Log((HttpClient)client, headers, buffer, isComplete);
            }
        }

        protected virtual void client_OnClientReceiveEnd(Socket clientSocket, Client client, string headers, List<byte> buffer, bool isComplete)
        {
            if (client is HttpClient)
            {
                Log((HttpClient)client, headers, buffer, isComplete);
            }
        }

        protected virtual void httpClient_OnOkSentEnd(Socket client, HttpClient httpClient)
        {
            Log(httpClient, httpClient.CurrentHttpQuery, null, false);
        }

        protected virtual void httpClient_OnStartHandshakeEnd(Socket destination, HttpClient httpClient)
        {
            Log(httpClient, httpClient.CurrentHttpQuery, null, false);
        }

        protected virtual void httpClient_OnErrorSentEnd(Socket destination, HttpClient httpClient)
        {
            Log(httpClient, httpClient.CurrentHttpQuery, null, false);
        }

        /**/

        public override void PerformAction(CrawlRequest<TArachnodeDAO> crawlRequest, IArachnodeDAO arachnodeDAO)
        {
            //here you would insert/update your data storage...
            if(_crawler == null)
            {
                _crawler = crawlRequest.Crawl.Crawler;
            }
        }

        public override void Stop()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (stopwatch.Elapsed.TotalSeconds < 600)
            {
                Application.DoEvents();

                Thread.Sleep(100);
            }

            _httpListener.Stop();

            Console.WriteLine("EXTRA: " + _extraCrawlRequests.Count);
        }
    }
}