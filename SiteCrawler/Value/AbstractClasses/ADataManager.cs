using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arachnode.Configuration;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Managers;

namespace Arachnode.SiteCrawler.Value.AbstractClasses
{
    public abstract class ADataManager<TArachnodeDAO> : AManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        protected DataTypeManager<TArachnodeDAO> _dataTypeManager;
        protected DiscoveryManager<TArachnodeDAO> _discoveryManager;
        protected RuleManager<TArachnodeDAO> _ruleManager;
        protected ActionManager<TArachnodeDAO> _actionManager;
        protected IArachnodeDAO _arachnodeDAO;

        protected ADataManager(ApplicationSettings applicationSettings, WebSettings webSettings, ActionManager<TArachnodeDAO> actionManager, DataTypeManager<TArachnodeDAO> dataTypeManager, DiscoveryManager<TArachnodeDAO> discoveryManager, RuleManager<TArachnodeDAO> ruleManager, IArachnodeDAO arachnodeDAO) : base(applicationSettings, webSettings)
        {
            _dataTypeManager = dataTypeManager;
            _discoveryManager = discoveryManager;
            _ruleManager = ruleManager;
            _actionManager = actionManager;
            _arachnodeDAO = arachnodeDAO;
        }

        public abstract void ProcessCrawlRequest(CrawlRequest<TArachnodeDAO> crawlRequest, bool obeyCrawlRules, bool executeCrawlActions);
        protected abstract void IssueWebRequest(CrawlRequest<TArachnodeDAO> crawlRequest, string method);
    }
}
