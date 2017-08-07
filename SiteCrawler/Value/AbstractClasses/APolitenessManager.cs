using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arachnode.Configuration;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Core;
using Arachnode.SiteCrawler.Value.Enums;

namespace Arachnode.SiteCrawler.Value.AbstractClasses
{
    public abstract class APolitenessManager<TArachnodeDAO> : AManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        protected readonly object _lock = new object();
        protected Cache<TArachnodeDAO> _cache;

        protected APolitenessManager(ApplicationSettings applicationSettings, WebSettings webSettings, Cache<TArachnodeDAO> cache) : base(applicationSettings, webSettings)
        {
            _cache = cache;
        }

        public abstract bool ManagePoliteness(CrawlRequest<TArachnodeDAO> crawlRequest, PolitenessState politenessState, IArachnodeDAO arachnodeDAO);
        public abstract void ResubmitCrawlRequest(CrawlRequest<TArachnodeDAO> crawlRequest, bool retryIndefinitely, IArachnodeDAO arachnodeDAO);
    }
}
