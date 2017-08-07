#region License : arachnode.net

// // Copyright (c) 2015 http://arachnode.net, arachnode.net, LLC
// //  
// // Permission is hereby granted, upon purchase, to any person
// // obtaining a copy of this software and associated documentation
// // files (the "Software"), to deal in the Software without
// // restriction, including without limitation the rights to use,
// // copy, merge and modify copies of the Software, and to permit persons
// // to whom the Software is furnished to do so, subject to the following
// // conditions:
// // 
// // LICENSE (ALL VERSIONS/EDITIONS): http://arachnode.net/r.ashx?3
// // 
// // The above copyright notice and this permission notice shall be
// // included in all copies or substantial portions of the Software.
// // 
// // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// // EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// // OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// // NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// // HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// // WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// // FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// // OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Cache;
using System.Threading;
using Arachnode.Configuration;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Managers;
using Arachnode.Utilities;

#endregion

namespace Arachnode.SiteCrawler.Components
{
    public class WebClient<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        //private Stopwatch _stopwatch1 = new Stopwatch();
        private bool _didTheWebRequestTimeOut;
        private bool _didTheWebRequestThrowAnException;
        private readonly ManualResetEvent _httpWebResponseCallbackResetEvent = new ManualResetEvent(false);

        private ApplicationSettings _applicationSettings;
        private WebSettings _webSettings;

        private ConsoleManager<TArachnodeDAO> _consoleManager;
        private CookieManager _cookieManager;
        private ProxyManager<TArachnodeDAO> _proxyManager;

        public WebClient(ApplicationSettings applicationSettings, WebSettings webSettings, ConsoleManager<TArachnodeDAO> consoleManager, CookieManager cookieManager, ProxyManager<TArachnodeDAO> proxyManager)
        {
            _applicationSettings = applicationSettings;
            _webSettings = webSettings;

            _cookieManager = cookieManager;
            _consoleManager = consoleManager;
            _proxyManager = proxyManager;
        }

        /// <summary>
        /// 	Gets the web exception.
        /// </summary>
        /// <value>The web exception.</value>
        public WebException WebException { get; private set; }

        /// <summary>
        /// 	Gets the web request.
        /// </summary>
        /// <value>The web request.</value>
        public FtpWebRequest FtpWebRequest { get; private set; }

        /// <summary>
        /// 	Gets the web response.
        /// </summary>
        /// <value>The web response.</value>
        public FtpWebResponse FtpWebResponse { get; private set; }

        /// <summary>
        /// 	Gets the web request.
        /// </summary>
        /// <value>The web request.</value>
        public HttpWebRequest HttpWebRequest { get; private set; }

        /// <summary>
        /// 	Gets the web response.
        /// </summary>
        /// <value>The web response.</value>
        public HttpWebResponse HttpWebResponse { get; private set; }

        public HttpWebResponse GetHttpWebResponse(string absoluteUri, string method, string referer, CredentialCache credentialCache, CookieContainer cookieContainer, IWebProxy webProxy)
        {
            return GetHttpWebResponse(absoluteUri, method, referer, true, credentialCache, cookieContainer, webProxy);
        }

        /// <summary>
        /// 	Gets the web response.
        /// </summary>
        /// <param name = "absoluteUri">The absolute URI.</param>
        /// <param name = "method">The method.</param>
        /// <param name = "referer">The referer.</param>
        /// <returns></returns>
        private HttpWebResponse GetHttpWebResponse(string absoluteUri, string method, string referer, bool retryAfterHeadException, CredentialCache credentialCache, CookieContainer cookieContainer, IWebProxy webProxy)
        {
            if (HttpWebRequest != null)
            {
                HttpWebRequest.Abort();
                HttpWebRequest = null;
            }

            HttpWebResponse = null;

            HttpWebRequest = (HttpWebRequest) WebRequest.Create(absoluteUri);

            //HttpWebRequest.Headers.Add("Proxy-Requesting-Process", Process.GetCurrentProcess().ProcessName);
            if (!string.IsNullOrEmpty(_applicationSettings.Accept))
            {
                HttpWebRequest.Accept = _applicationSettings.Accept;
            }
            if (!string.IsNullOrEmpty(_applicationSettings.AcceptEncoding))
            {
                HttpWebRequest.Headers.Add("Accept-Encoding", _applicationSettings.AcceptEncoding);
            }
            HttpWebRequest.AllowAutoRedirect = _applicationSettings.AllowAutoRedirect;
            HttpWebRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);

            if (_applicationSettings.ProcessCookies)
            {
                _cookieManager.GetCookies(absoluteUri, cookieContainer);
            }

            HttpWebRequest.CookieContainer = cookieContainer;

            if (credentialCache != null)
            {
                HttpWebRequest.Credentials = credentialCache;
            }
            else
            {
                //HttpWebRequest.UseDefaultCredentials = true;
            }

            HttpWebRequest.KeepAlive = false;
            HttpWebRequest.MaximumAutomaticRedirections = _applicationSettings.MaximumNumberOfAutoRedirects;
            HttpWebRequest.Method = method;
            //set as not setting reduces the crawl rate by 90%...
            HttpWebRequest.Proxy = _proxyManager.GetNextProxy();
            HttpWebRequest.ReadWriteTimeout = (int)(_applicationSettings.CrawlRequestTimeoutInMinutes * 60000);
            if (!string.IsNullOrEmpty(referer) && absoluteUri != referer)
            {
                HttpWebRequest.Referer = referer;
            }
            HttpWebRequest.Timeout = (int)(_applicationSettings.CrawlRequestTimeoutInMinutes * 60000);
            HttpWebRequest.UserAgent = _applicationSettings.UserAgent;

            try
            {
                //_stopwatch.Start();

                WebException = null;
                //old, serialized, which may wait longer than anticipated on DNS lookups...
                //HttpWebResponse = (HttpWebResponse) HttpWebRequest.GetResponse();

                _didTheWebRequestTimeOut = false;
                _didTheWebRequestThrowAnException = false;

                _httpWebResponseCallbackResetEvent.Reset();

                //new, semi-serialized, which will not wait longer than anticipated on DNS lookups...
                IAsyncResult result = HttpWebRequest.BeginGetResponse(GetHttpWebResponseCallback, HttpWebRequest);

                result.AsyncWaitHandle.WaitOne((int)(_applicationSettings.CrawlRequestTimeoutInMinutes * 60000), false);

                if (!result.IsCompleted)
                {
                    _didTheWebRequestTimeOut = true;

                    throw new WebException("The operation timed out.", WebExceptionStatus.Timeout);
                }

                if (_didTheWebRequestThrowAnException)
                {
                    if (WebException != null)
                    {
                        throw WebException;
                    }
                }

                _httpWebResponseCallbackResetEvent.WaitOne();
            }
            catch (WebException webException)
            {
                if (webException.Response != null)
                {
                    HttpWebResponse = (HttpWebResponse) webException.Response;

                    //some webservers don't allow the HEAD method...
                    if (HttpWebResponse.StatusCode == HttpStatusCode.MethodNotAllowed ||
                        HttpWebResponse.StatusCode == HttpStatusCode.NotAcceptable)
                    {
                        if (method == "HEAD" && retryAfterHeadException)
                        {
                            GetHttpWebResponse(absoluteUri, "GET", referer, false, credentialCache, cookieContainer, webProxy);

                            return HttpWebResponse;
                        }
                    }
                }

                //If you encounter exception after exception here, see here: http://arachnode.net/forums/p/321/10290.aspx
                //Also, look up turning off 'Just My Code' - this is a Visual Studio option.

                WebException = webException;

                if (webException.Response != null)
                {
                    HttpWebResponse = (HttpWebResponse) webException.Response;
                }

                throw new WebException(webException.Message, webException);
            }
            finally
            {
                //_stopwatch.Reset();
            }

            if (HttpWebResponse == null)
            {
                if (WebException != null)
                {
                    if (WebException.Response != null)
                    {
                        HttpWebResponse = (HttpWebResponse) WebException.Response;

                        //some webservers don't allow the HEAD method...
                        if (HttpWebResponse.StatusCode == HttpStatusCode.MethodNotAllowed ||
                            HttpWebResponse.StatusCode == HttpStatusCode.NotAcceptable)
                        {
                            if (method == "HEAD" && retryAfterHeadException)
                            {
                                GetHttpWebResponse(absoluteUri, "GET", referer, false, credentialCache, cookieContainer, webProxy);

                                return HttpWebResponse;
                            }
                        }
                    }
                }
            }

            return HttpWebResponse;
        }

        private void GetHttpWebResponseCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                if (!_didTheWebRequestTimeOut)
                {
                    HttpWebResponse = (HttpWebResponse)((HttpWebRequest)asynchronousResult.AsyncState).EndGetResponse(asynchronousResult);
                }
            }
            catch(WebException webException)
            {
                _didTheWebRequestThrowAnException = true;

                WebException = webException;
            }
            finally
            {
                _httpWebResponseCallbackResetEvent.Set();
            }
        }

        /// <summary>
        /// 	Downloads the data.
        /// </summary>
        /// <param name = "absoluteUri">The absolute URI.</param>
        /// <returns></returns>
        public byte[] DownloadHttpData(string absoluteUri, bool isGZipStream, bool isDeflateStream, CookieContainer cookieContainer)
        {
            byte[] data = null;

            try
            {
                int numberOfBytesRead = -1;
                byte[] buffer = new byte[4096];

                if (HttpWebResponse != null)
                {
                    DateTime startTimeOriginal = DateTime.Now;
                    DateTime startTime = DateTime.Now;

                    using (Stream stream = HttpWebResponse.GetResponseStream())
                    {
                        _cookieManager.UpdateCookies(absoluteUri, cookieContainer, HttpWebResponse.Cookies);

                        //memory/gzip stream used here as StreamReader needs an Encoding, and the Encoding is properly determined after downloading the Stream.
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            while (numberOfBytesRead != 0)
                            {
                                numberOfBytesRead = stream.Read(buffer, 0, buffer.Length);

                                memoryStream.Write(buffer, 0, numberOfBytesRead);

                                if (DateTime.Now.Subtract(startTime).TotalSeconds >= _applicationSettings.CrawlRequestTimeoutInMinutes * 60)
                                {
                                    _consoleManager.OutputString(_consoleManager.BuildOutputString(" -> DownloadHttpData", absoluteUri, startTimeOriginal, DateTime.Now.Subtract(startTimeOriginal)), ConsoleColor.DarkGreen, ConsoleColor.Gray);

                                    startTime = DateTime.Now;
                                }
                            }

                            data = memoryStream.ToArray();
                        }

                        if (isGZipStream)
                        {
                            numberOfBytesRead = -1;

                            using (MemoryStream memoryStream = new MemoryStream(data))
                            {
                                using (MemoryStream memoryStream2 = new MemoryStream())
                                {
                                    using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                                    {
                                        while (numberOfBytesRead != 0)
                                        {
                                            numberOfBytesRead = gZipStream.Read(buffer, 0, buffer.Length);

                                            memoryStream2.Write(buffer, 0, numberOfBytesRead);

                                            if (DateTime.Now.Subtract(startTime).TotalSeconds >= _applicationSettings.CrawlRequestTimeoutInMinutes * 60)
                                            {
                                                _consoleManager.OutputString(_consoleManager.BuildOutputString(" -> DownloadHttpData", absoluteUri, startTimeOriginal, DateTime.Now.Subtract(startTimeOriginal)), ConsoleColor.DarkGreen, ConsoleColor.Gray);

                                                startTime = DateTime.Now;
                                            }
                                        }

                                        data = memoryStream2.ToArray();
                                    }
                                }
                            }
                        }

                        if (isDeflateStream)
                        {
                            numberOfBytesRead = -1;

                            using (MemoryStream memoryStream = new MemoryStream(data))
                            {
                                using (MemoryStream memoryStream2 = new MemoryStream())
                                {
                                    using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress))
                                    {
                                        while (numberOfBytesRead != 0)
                                        {
                                            numberOfBytesRead = deflateStream.Read(buffer, 0, buffer.Length);

                                            memoryStream2.Write(buffer, 0, numberOfBytesRead);

                                            if (DateTime.Now.Subtract(startTime).TotalSeconds >= _applicationSettings.CrawlRequestTimeoutInMinutes * 60)
                                            {
                                                _consoleManager.OutputString(_consoleManager.BuildOutputString(" -> DownloadHttpData", absoluteUri, startTimeOriginal, DateTime.Now.Subtract(startTimeOriginal)), ConsoleColor.DarkGreen, ConsoleColor.Gray);

                                                startTime = DateTime.Now;
                                            }
                                        }

                                        data = memoryStream2.ToArray();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message, exception);
            }
            finally
            {
                if (HttpWebResponse != null)
                {
                    HttpWebResponse.Close();
                    HttpWebResponse.GetResponseStream().Close();
                    HttpWebResponse.GetResponseStream().Dispose();
                }
            }

            if (data == null)
            {
                data = new byte[0];
            }

            return data;
        }

        /// <summary>
        /// 	Gets the web response.
        /// </summary>
        /// <param name = "absoluteUri">The absolute URI.</param>
        /// <param name = "method">The method.</param>
        /// <returns></returns>
        public FtpWebResponse GetFtpWebResponse(string absoluteUri, string method, CredentialCache credentialCache, IWebProxy webProxy)
        {
            if (FtpWebResponse != null)
            {
                FtpWebResponse.Close();
                FtpWebResponse.GetResponseStream().Close();
                FtpWebResponse.GetResponseStream().Dispose();
            }

            FtpWebRequest = (FtpWebRequest) WebRequest.Create(absoluteUri);
            FtpWebRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
            if (credentialCache != null)
            {
                FtpWebRequest.Credentials = credentialCache;
            }
            else
            {
                //FtpWebRequest.UseDefaultCredentials = true;
            }
            FtpWebRequest.KeepAlive = false;
            FtpWebRequest.Method = method;
            FtpWebRequest.Proxy = webProxy;
            FtpWebRequest.ReadWriteTimeout = (int) (_applicationSettings.CrawlRequestTimeoutInMinutes*60000);
            FtpWebRequest.Timeout = (int) (_applicationSettings.CrawlRequestTimeoutInMinutes*60000);

            try
            {
                WebException = null;
                FtpWebResponse = (FtpWebResponse) FtpWebRequest.GetResponse();
            }
            catch (WebException webException)
            {
                //If you encounter exception after exception here, see here: http://arachnode.net/forums/p/321/10290.aspx
                //Also, look up turning off 'Just My Code' - this is a Visual Studio option.

                WebException = webException;

                if (webException.Response != null)
                {
                    FtpWebResponse = (FtpWebResponse) webException.Response;
                }

                throw new WebException(webException.Message, webException);
            }

            return FtpWebResponse;
        }

        /// <summary>
        /// 	Downloads the data.
        /// </summary>
        /// <param name = "absoluteUri">The absolute URI.</param>
        /// <returns></returns>
        public byte[] DownloadFtpData(string absoluteUri)
        {
            byte[] data = null;

            try
            {
                int numberOfBytesRead = -1;
                byte[] buffer = new byte[4096];

                if (FtpWebResponse != null)
                {
                    using (Stream stream = FtpWebResponse.GetResponseStream())
                    {
                        //memory stream used here as StreamReader needs an Encoding, and the Encoding is properly determined after downloading the Stream.
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            while (numberOfBytesRead != 0)
                            {
                                numberOfBytesRead = stream.Read(buffer, 0, 4096);

                                memoryStream.Write(buffer, 0, numberOfBytesRead);
                            }

                            data = memoryStream.ToArray();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message, exception);
            }
            finally
            {
                if (FtpWebResponse != null)
                {
                    FtpWebResponse.Close();
                    FtpWebResponse.GetResponseStream().Close();
                    FtpWebResponse.GetResponseStream().Dispose();
                }
            }

            if (data == null)
            {
                data = new byte[0];
            }

            return data;
        }
    }
}