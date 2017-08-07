using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Services;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.Renderer.Value.Enums;
using Arachnode.SiteCrawler;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.AbstractClasses;
using Arachnode.SiteCrawler.Value.Enums;
using Arachnode.Web.Value.AbstractClasses;

namespace Arachnode.Web.Services
{
    /// <summary>
    /// Summary description for CrawlerService
    /// </summary>
    [WebService(Namespace = "http://arachnode.net/CrawlerService")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
        // [System.Web.Script.Services.ScriptService]
    public class CrawlerService : AWebService
    {
        private Crawler<ArachnodeDAO> Crawler
        {
            get { return (Crawler<ArachnodeDAO>) Session["_crawler"]; }
            set { Session["_crawler"] = value; }
        }

        [WebMethod(EnableSession = true)]
        public void Constructor1(CrawlMode crawlMode, bool enableRenderers)
        {
            Crawler = new Crawler<ArachnodeDAO>(ApplicationSettings, WebSettings, crawlMode, enableRenderers);
        }

        [WebMethod(EnableSession = true)]
        public void Constructor2(ApplicationSettings applicationSettings, WebSettings webSettings, CrawlMode crawlMode, bool enableRenderers)
        {
            Crawler = new Crawler<ArachnodeDAO>(applicationSettings, webSettings, crawlMode, enableRenderers);
        }

        [WebMethod(EnableSession = true)]
        public void Destructor()
        {
            Crawler = null;
        }

        [WebMethod(EnableSession = true)]
        public new void SetApplicationSettings(ApplicationSettings applicationSettings)
        {
            ApplicationSettings = applicationSettings;
            Crawler.ApplicationSettings = ApplicationSettings;
        }

        [WebMethod(EnableSession = true)]
        public new void SetWebSettings(WebSettings webSettings)
        {
            WebSettings = webSettings;
            Crawler.WebSettings = WebSettings;
        }

        [WebMethod(EnableSession = true)]
        public bool Crawl(string absoluteUri, int depth, UriClassificationType restrictCrawlTo, UriClassificationType restrictDiscoveriesTo, double priority, RenderType renderType, RenderType renderTypeForChildren)
        {
            var discovery = new Discovery<ArachnodeDAO>(absoluteUri);
            var crawlRequest = new CrawlRequest<ArachnodeDAO>(discovery, depth, restrictCrawlTo, restrictDiscoveriesTo, priority, renderType, renderTypeForChildren);

            return Crawler.Crawl(crawlRequest);
        }

        [WebMethod(EnableSession = true)]
        public List<ACrawlAction<ArachnodeDAO>> GetCrawlActions()
        {
            try
            {
                return Crawler.CrawlActions.Values.ToList();
            }
            catch (Exception exception)
            {
                ArachnodeDAO.InsertException(null, null, exception, false);

                throw;
            }
        }

        [WebMethod(EnableSession = true)]
        public List<ACrawlRule<ArachnodeDAO>> GetCrawlRules()
        {
            try
            {
                return Crawler.CrawlRules.Values.ToList();
            }
            catch (Exception exception)
            {
                ArachnodeDAO.InsertException(null, null, exception, false);

                throw;
            }
        }

        [WebMethod(EnableSession = true)]
        public List<AEngineAction<ArachnodeDAO>> GetEngineActions()
        {
            try
            {
                return Crawler.Engine.EngineActions.Values.ToList();
            }
            catch (Exception exception)
            {
                ArachnodeDAO.InsertException(null, null, exception, false);

                throw;
            }
        }

        [WebMethod(EnableSession = true)]
        public void Reset()
        {
            Crawler.Reset();
        }

        [WebMethod(EnableSession = true)]
        public void StartEngine()
        {
            Crawler.Engine.Start();
        }

        [WebMethod(EnableSession = true)]
        public void StopEngine()
        {
            Crawler.Engine.Stop();
        }

        [WebMethod(EnableSession = true)]
        public void PauseEngine()
        {
            Crawler.Engine.Pause();
        }

        [WebMethod(EnableSession = true)]
        public void ResumeEngine()
        {
            Crawler.Engine.Resume();
        }
    }
}