using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.SiteCrawler.Core;
using Arachnode.SiteCrawler.Managers;

namespace Arachnode.Console.Override
{
    class CustomDiscoveryManager : DiscoveryManager<ArachnodeDAO>
    {
        public CustomDiscoveryManager(ApplicationSettings applicationSettings, WebSettings webSettings, Cache<ArachnodeDAO> cache, ActionManager<ArachnodeDAO> actionManager, CacheManager<ArachnodeDAO> cacheManager, MemoryManager<ArachnodeDAO> memoryManager, RuleManager<ArachnodeDAO> ruleManager) : base(applicationSettings, webSettings, cache, actionManager, cacheManager, memoryManager, ruleManager)
        {
        }

        public override void AssignHyperLinkDiscoveries(Arachnode.SiteCrawler.Value.CrawlRequest<ArachnodeDAO> crawlRequest, Arachnode.DataAccess.Value.Interfaces.IArachnodeDAO arachnodeDAO)
        {
            base.AssignHyperLinkDiscoveries(crawlRequest, arachnodeDAO);

            //if you wanted to change which HyperLinks were assigned, you could perform that action here...
        }
    }
}
