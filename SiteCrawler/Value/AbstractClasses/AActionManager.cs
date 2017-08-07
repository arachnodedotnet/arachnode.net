using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arachnode.Configuration;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Managers;
using Arachnode.SiteCrawler.Value.Enums;
using Arachnode.Structures;

namespace Arachnode.SiteCrawler.Value.AbstractClasses
{
    public abstract class AActionManager<TArachnodeDAO> : AManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        protected readonly Dictionary<int, List<AEngineAction<TArachnodeDAO>>> _postGetCrawlRequestsActions = new Dictionary<int, List<AEngineAction<TArachnodeDAO>>>();
        protected readonly Dictionary<int, List<ACrawlAction<TArachnodeDAO>>> _postRequestCrawlActions = new Dictionary<int, List<ACrawlAction<TArachnodeDAO>>>();
        protected readonly Dictionary<int, List<ACrawlAction<TArachnodeDAO>>> _preGetCrawlActions = new Dictionary<int, List<ACrawlAction<TArachnodeDAO>>>();
        protected readonly Dictionary<int, List<AEngineAction<TArachnodeDAO>>> _preGetCrawlRequestsActions = new Dictionary<int, List<AEngineAction<TArachnodeDAO>>>();
        protected readonly Dictionary<int, List<ACrawlAction<TArachnodeDAO>>> _preRequestCrawlActions = new Dictionary<int, List<ACrawlAction<TArachnodeDAO>>>();
        protected ConsoleManager<TArachnodeDAO> _consoleManager;

        protected AActionManager(ApplicationSettings applicationSettings, WebSettings webSettings, ConsoleManager<TArachnodeDAO> consoleManager) : base(applicationSettings, webSettings)
        {
            _consoleManager = consoleManager;
        }

        /// <summary>
        /// 	Processes and instantiates the CrawlActions specified by ApplicationSettings.CrawlActionsDotConfigLocation.
        /// </summary>
        public abstract void ProcessCrawlActions(Crawler<TArachnodeDAO> crawler);

        /// <summary>
        /// 	Processes and instantiates the CrawlActions specified by ApplicationSettings.EngineActionsDotConfigLocation.
        /// </summary>
        public abstract void ProcessEngineActions(Crawler<TArachnodeDAO> crawler);

        /// <summary>
        /// 	Performs the crawl actions.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "crawlActionType">Type of the crawl action.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        public abstract void PerformCrawlActions(CrawlRequest<TArachnodeDAO> crawlRequest, CrawlActionType crawlActionType, IArachnodeDAO arachnodeDAO);

        /// <summary>
        /// 	Performs the engine actions.
        /// </summary>
        /// <param name = "crawlRequests">The crawl requests.</param>
        /// <param name = "engineActionType">Type of the engine action.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        public abstract void PerformEngineActions(PriorityQueue<CrawlRequest<TArachnodeDAO>> crawlRequests, EngineActionType engineActionType, IArachnodeDAO arachnodeDAO);

        /// <summary>
        /// 	Stops this instance.
        /// </summary>
        public abstract void Stop();
    }
}
