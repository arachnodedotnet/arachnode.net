using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arachnode.Configuration;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Managers;

namespace Arachnode.SiteCrawler.Value.AbstractClasses
{
    public abstract class AWebPageManager<TArachnodeDAO> : AManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        protected HtmlManager<TArachnodeDAO> _htmlManager;
        protected DiscoveryManager<TArachnodeDAO> _discoveryManager;
        protected IArachnodeDAO _arachnodeDAO;

        /// <summary>
        /// 	The WebPageManager.
        /// </summary>
        /// <param name = "arachnodeDAO">Must be thread-safe.</param>
        protected AWebPageManager(ApplicationSettings applicationSettings, WebSettings webSettings, DiscoveryManager<TArachnodeDAO> discoveryManager, HtmlManager<TArachnodeDAO> htmlManager, IArachnodeDAO arachnodeDAO) : base(applicationSettings, webSettings)
        {
            _discoveryManager = discoveryManager;
            _htmlManager = htmlManager;
            _arachnodeDAO = arachnodeDAO;
        }

        /// <summary>
        /// 	Manages the web page.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        public abstract void ManageWebPage(CrawlRequest<TArachnodeDAO> crawlRequest);

        /// <summary>
        /// 	Manages the web page.
        /// </summary>
        /// <param name = "webPageID">The web page ID.</param>
        /// <param name = "absoluteUri">The absolute URI.</param>
        /// <param name = "source">The source.</param>
        /// <param name = "encoding">The encoding.</param>
        /// <param name = "fullTextIndexType">Full type of the text index.</param>
        /// <param name = "extractWebPageMetaData">if set to <c>true</c> [extract web page meta data].</param>
        /// <param name = "insertWebPageMetaData">if set to <c>true</c> [insert web page meta data].</param>
        /// <param name = "saveWebPageToDisk">if set to <c>true</c> [save web page to disk].</param>
        /// <returns></returns>
        public abstract ManagedWebPage ManageWebPage(long webPageID, string absoluteUri, byte[] source, Encoding encoding, string fullTextIndexType, bool extractWebPageMetaData, bool insertWebPageMetaData, bool saveWebPageToDisk);
    }
}
