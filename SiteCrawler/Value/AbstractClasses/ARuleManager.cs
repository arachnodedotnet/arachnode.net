using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arachnode.Configuration;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Managers;
using Arachnode.SiteCrawler.Value.Enums;

namespace Arachnode.SiteCrawler.Value.AbstractClasses
{
    public abstract class ARuleManager<TArachnodeDAO> : AManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        protected readonly Dictionary<int, List<ACrawlRule<TArachnodeDAO>>> _postRequestCrawlRules = new Dictionary<int, List<ACrawlRule<TArachnodeDAO>>>();
        protected readonly Dictionary<int, List<ACrawlRule<TArachnodeDAO>>> _preGetCrawlRules = new Dictionary<int, List<ACrawlRule<TArachnodeDAO>>>();
        protected readonly Dictionary<int, List<ACrawlRule<TArachnodeDAO>>> _preRequestCrawlRules = new Dictionary<int, List<ACrawlRule<TArachnodeDAO>>>();
        protected ConsoleManager<TArachnodeDAO> _consoleManager;

        protected ARuleManager(ApplicationSettings applicationSettings, WebSettings webSettings, ConsoleManager<TArachnodeDAO> consoleManager) : base(applicationSettings, webSettings)
        {
            _consoleManager = consoleManager;
        }

        /// <summary>
        /// 	Processes and instantiates the CrawlRules specified by ApplicationSettings.CrawlRulesDotConfigLocation.
        /// </summary>
        public abstract void ProcessCrawlRules(Crawler<TArachnodeDAO> crawler);

        /// <summary>
        /// 	Determines whether the specified crawl request is disallowed.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "crawlRuleType">Type of the rule.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        /// <returns>
        /// 	<c>true</c> if the specified crawl request is disallowed; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool IsDisallowed(CrawlRequest<TArachnodeDAO> crawlRequest, CrawlRuleType crawlRuleType, IArachnodeDAO arachnodeDAO);

        /// <summary>
        /// 	Determines whether the specified crawl request is disallowed.
        /// </summary>
        /// <param name = "discovery">The discovery.</param>
        /// <param name = "crawlRuleType">Type of the rule.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        /// <returns>
        /// 	<c>true</c> if the specified crawl request is disallowed; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool IsDisallowed(Discovery<TArachnodeDAO> discovery, CrawlRuleType crawlRuleType, IArachnodeDAO arachnodeDAO);

        /// <summary>
        /// 	Stops this instance.
        /// </summary>
        public abstract void Stop();
    }
}
