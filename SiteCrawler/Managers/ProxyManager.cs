using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Arachnode.Configuration;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Value.AbstractClasses;

namespace Arachnode.SiteCrawler.Managers
{
    public class ProxyManager<TArachnodeDAO> : AProxyManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        public override event Action<IWebProxy, TimeSpan> OnProxyServerPassed;
        public override event Action<IWebProxy, TimeSpan, Exception> OnProxyServerFailed;

        public ProxyManager(ApplicationSettings applicationSettings, WebSettings webSettings, ConsoleManager<TArachnodeDAO> consoleManager) : base(applicationSettings, webSettings, consoleManager)
        {
            
        }

        public override Queue<IWebProxy> Proxies
        {
            get
            {
                if(_proxies == null)
                {
                    _proxies = new Queue<IWebProxy>();
                }
                
                return _proxies;
            }
        }

        public override Dictionary<IWebProxy, HttpStatusCode> ProxiesAndHttpStatusCodes
        {
            get
            {
                if (_proxiesAndHttpStatusCodes == null)
                {
                    _proxiesAndHttpStatusCodes = new Dictionary<IWebProxy, HttpStatusCode>();
                }

                return _proxiesAndHttpStatusCodes;
            }
        }

        public override List<IWebProxy> LoadFastestProxyServers(List<Uri> uris, int timeoutInMilliseconds, int numberOfWorkingProxyServersToLoad, string absoluteUriToVerify, string stringToVerify, bool resetProxyServers)
        {
            lock (_proxiesLock)
            {
                _proxiesAndTimeSpans.Clear();

                LoadProxyServers(uris, timeoutInMilliseconds, int.MaxValue, absoluteUriToVerify, stringToVerify, resetProxyServers);

                var orderedProxyServers = _proxiesAndTimeSpans.OrderBy(pats => pats.Value).Take(numberOfWorkingProxyServersToLoad);

                return orderedProxyServers.Select(pats => pats.Key).ToList();
            }
        }

        public override List<IWebProxy> LoadProxyServers(List<Uri> uris, int timeoutInMilliseconds, int numberOfWorkingProxyServersToLoad, string absoluteUriToVerify, string stringToVerify, bool resetProxyServers)
        {
            List<IWebProxy> proxies = new List<IWebProxy>();

            lock (_proxiesLock)
            {
                if (uris == null)
                {
                    return null;
                }

                if (resetProxyServers)
                {
                    _proxies = new Queue<IWebProxy>();
                }

                double passed = 0;
                double failed = 0;

                foreach (Uri uri in uris)
                {
                    WebProxy webProxy = null;
                    if (uri.Host != "127.0.0.1")
                    {
                        webProxy = new WebProxy(uri.AbsoluteUri, true);
                    }
                    else
                    {
                        webProxy = new WebProxy();

                        Proxies.Enqueue(webProxy);
                        _proxiesAndTimeSpans.Add(webProxy, TimeSpan.Zero);

                        continue;
                    }

                    _stopwatch.Reset();
                    _stopwatch.Start();

                    HttpWebResponse httpWebResponse = null;

                    try
                    {
                        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(absoluteUriToVerify);
                        httpWebRequest.Proxy = webProxy;
                        httpWebRequest.ReadWriteTimeout = timeoutInMilliseconds;
                        httpWebRequest.Timeout = timeoutInMilliseconds;
                        httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/534.24 (KHTML, like Gecko) Chrome/11.0.696.71 Safari/534.24";

                        httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                        if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                        {
                            using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                            {
                                string toEnd = streamReader.ReadToEnd();

                                if (toEnd.Contains(stringToVerify))
                                {
                                    passed++;

                                    Proxies.Enqueue(webProxy);
                                    _proxiesAndTimeSpans.Add(webProxy, _stopwatch.Elapsed);

                                    proxies.Add(webProxy);

                                    if (OnProxyServerPassed != null)
                                    {
                                        _stopwatch.Stop();
                                        OnProxyServerPassed.Invoke(webProxy, _stopwatch.Elapsed);
                                    }

                                    if (passed >= numberOfWorkingProxyServersToLoad)
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    failed++;
                                }
                            }
                        }
                        else
                        {
                            failed++;
                        }
                    }
                    catch (Exception exception)
                    {
                        failed++;

                        if (OnProxyServerFailed != null)
                        {
                            OnProxyServerFailed.Invoke(webProxy, _stopwatch.Elapsed, exception);
                        }
                    }
                    finally
                    {
                        if (httpWebResponse != null)
                        {
                            _proxiesAndHttpStatusCodes.Add(webProxy, httpWebResponse.StatusCode);
                        }
#if !RELEASE
                        if (uri.Host != "127.0.0.1")
                        {
                            _consoleManager.OutputString("LoadProxyServers: " + webProxy.Address.AbsoluteUri + " :: " + (Math.Round(passed / (passed + failed), 2) * 100) + "% (" + passed + " of " + (passed + failed) + ")");
                        }
                        else
                        {
                            _consoleManager.OutputString("LoadProxyServers: 127.0.0.1 :: " + (Math.Round(passed / (passed + failed), 2) * 100) + "% (" + passed + " of " + (passed + failed) + ")");
                        }
#endif
                    }
                }
#if !RELEASE
                Console.WriteLine("LoadProxyServers (Total): " + (Math.Round(passed / (passed + failed), 2) * 100) + "% (" + passed + " of " + (passed + failed) + ")");
#endif
                if (Proxies.Count == 0)
                {
                    _proxies = null;
                    proxies = null;
                }
            }

            return proxies;
        }

        public override void AddProxy(IWebProxy webProxy)
        {
            lock (_proxiesLock)
            {
                if (webProxy == null)
                {
                }

                if(Proxies == null)
                {
                    _proxies = new Queue<IWebProxy>();
                }

                Proxies.Enqueue(webProxy);
            }
        }

        public override void RemoveProxy(IWebProxy webProxy)
        {
            lock (_proxiesLock)
            {
                if (webProxy == null || Proxies == null)
                {
                    return;
                }

                Queue<IWebProxy> proxies = new Queue<IWebProxy>();

                int proxiesCount = Proxies.Count;

                while (Proxies.Count != 0)
                {
                    IWebProxy webProxy2 = Proxies.Dequeue();

                    if (webProxy != webProxy2)
                    {
                        proxies.Enqueue(webProxy2);
                    }
                }

                _proxies = proxies;
            }
        }

        public override IWebProxy GetNextProxy()
        {
            lock (_proxiesLock)
            {
                if (Proxies == null)
                {
                    return null;
                }

                int proxiesCount = Proxies.Count;

                if (Proxies.Count != 0)
                {
                    IWebProxy webProxy = Proxies.Dequeue();

                    Proxies.Enqueue(webProxy);

                    return webProxy;
                }

                return null;
            }
        }
    }
}

