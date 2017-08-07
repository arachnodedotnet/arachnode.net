using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using Arachnode.Configuration;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Managers;

namespace Arachnode.SiteCrawler.Value.AbstractClasses
{
    public abstract class AProxyManager<TArachnodeDAO> : AManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        protected AProxyManager(ApplicationSettings applicationSettings, WebSettings webSettings, ConsoleManager<TArachnodeDAO> consoleManager) : base(applicationSettings, webSettings)
        {
            _consoleManager = consoleManager;
        }

        protected ConsoleManager<TArachnodeDAO> _consoleManager;

        protected Queue<IWebProxy> _proxies = new Queue<IWebProxy>();
        protected Dictionary<IWebProxy, TimeSpan> _proxiesAndTimeSpans = new Dictionary<IWebProxy, TimeSpan>();
        protected Dictionary<IWebProxy, HttpStatusCode> _proxiesAndHttpStatusCodes = new Dictionary<IWebProxy, HttpStatusCode>();
        protected object _proxiesLock = new object();
        protected Stopwatch _stopwatch = new Stopwatch();
        public abstract Queue<IWebProxy> Proxies { get; }
        public abstract Dictionary<IWebProxy, HttpStatusCode> ProxiesAndHttpStatusCodes { get; }

        public abstract event Action<IWebProxy, TimeSpan> OnProxyServerPassed;
        public abstract event Action<IWebProxy, TimeSpan, Exception> OnProxyServerFailed;
        public abstract List<IWebProxy> LoadFastestProxyServers(List<Uri> uris, int timeoutInMilliseconds, int numberOfWorkingProxyServersToLoad, string absoluteUriToVerify, string stringToVerify, bool resetProxyServers);
        public abstract List<IWebProxy> LoadProxyServers(List<Uri> uris, int timeoutInMilliseconds, int numberOfWorkingProxyServersToLoad, string absoluteUriToVerify, string stringToVerify, bool resetProxyServers);
        public abstract void AddProxy(IWebProxy webProxy);
        public abstract void RemoveProxy(IWebProxy webProxy);
        public abstract IWebProxy GetNextProxy();
    }
}