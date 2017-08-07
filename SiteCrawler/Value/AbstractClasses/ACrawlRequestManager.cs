using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arachnode.Configuration;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Core;
using Arachnode.SiteCrawler.Managers;

namespace Arachnode.SiteCrawler.Value.AbstractClasses
{
    public abstract class ACrawlRequestManager<TArachnodeDAO> : AManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        protected Cache<TArachnodeDAO> _cache;
        protected ConsoleManager<TArachnodeDAO> _consoleManager;
        protected ADiscoveryManager<TArachnodeDAO> _discoveryManager;

        protected ACrawlRequestManager(ApplicationSettings applicationSettings, WebSettings webSettings, Cache<TArachnodeDAO> cache, ConsoleManager<TArachnodeDAO> consoleManager, ADiscoveryManager<TArachnodeDAO> discoveryManager) : base(applicationSettings, webSettings)
        {
            _cache = cache;

            _consoleManager = consoleManager;
            _discoveryManager = discoveryManager;
        }

        /// <summary>
        /// 	Processes the crawl request.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "fileManager">The file manager.</param>
        /// <param name = "imageManager">The image manager.</param>
        /// <param name = "webPageManager">The web page manager.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        public abstract void ProcessCrawlRequest(CrawlRequest<TArachnodeDAO> crawlRequest, FileManager<TArachnodeDAO> fileManager, ImageManager<TArachnodeDAO> imageManager, WebPageManager<TArachnodeDAO> webPageManager, IArachnodeDAO arachnodeDAO);

        /// <summary>
        /// 	Processes the file or image discovery.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "fileOrImageDiscovery">The file or image discovery.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        public abstract void ProcessFileOrImageDiscovery(CrawlRequest<TArachnodeDAO> crawlRequest, Discovery<TArachnodeDAO> fileOrImageDiscovery, IArachnodeDAO arachnodeDAO);

        /// <summary>
        /// 	Processes the file.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "fileManager">The file manager.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        protected abstract void ProcessFile(CrawlRequest<TArachnodeDAO> crawlRequest, FileManager<TArachnodeDAO> fileManager, IArachnodeDAO arachnodeDAO);

        /// <summary>
        /// 	Processes the image.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "imageManager">The image manager.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        protected abstract void ProcessImage(CrawlRequest<TArachnodeDAO> crawlRequest, ImageManager<TArachnodeDAO> imageManager, IArachnodeDAO arachnodeDAO);

        /// <summary>
        /// 	Processes the web page.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "webPageManager">The web page manager.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        protected abstract void ProcessWebPage(CrawlRequest<TArachnodeDAO> crawlRequest, WebPageManager<TArachnodeDAO> webPageManager, IArachnodeDAO arachnodeDAO);

        public abstract void ProcessDiscoveries(CrawlRequest<TArachnodeDAO> crawlRequest, IArachnodeDAO arachnodeDAO);

        /// <summary>
        /// 	Processes the email addresses.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        public abstract void ProcessEmailAddresses(CrawlRequest<TArachnodeDAO> crawlRequest, IArachnodeDAO arachnodeDAO);

        /// <summary>
        /// 	Processes the files and images.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        protected abstract void ProcessFilesAndImages(CrawlRequest<TArachnodeDAO> crawlRequest, IArachnodeDAO arachnodeDAO);

        /// <summary>
        /// 	Processes the hyper links.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        public abstract void ProcessHyperLinks(CrawlRequest<TArachnodeDAO> crawlRequest, IArachnodeDAO arachnodeDAO);
    }
}
