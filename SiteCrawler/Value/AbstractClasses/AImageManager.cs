using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arachnode.Configuration;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Managers;

namespace Arachnode.SiteCrawler.Value.AbstractClasses
{
    public abstract class AImageManager<TArachnodeDAO> : AManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        protected DiscoveryManager<TArachnodeDAO> _discoveryManager;
        protected IArachnodeDAO _arachnodeDAO;

        /// <summary>
        /// 	Initializes a new instance of the <see cref = "ImageManager" /> class.
        /// </summary>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        protected AImageManager(ApplicationSettings applicationSettings, WebSettings webSettings, DiscoveryManager<TArachnodeDAO> discoveryManager, IArachnodeDAO arachnodeDAO) : base(applicationSettings, webSettings)
        {
            _discoveryManager = discoveryManager;
            _arachnodeDAO = arachnodeDAO;
        }

        /// <summary>
        /// 	Manages the image.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        public abstract void ManageImage(CrawlRequest<TArachnodeDAO> crawlRequest);

        /// <summary>
        /// 	Manages the image.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "imageID">The image ID.</param>
        /// <param name = "absoluteUri">The absolute URI.</param>
        /// <param name = "source">The source.</param>
        /// <param name = "fullTextIndexType">Full type of the text index.</param>
        /// <param name = "extractImageMetaData">if set to <c>true</c> [extract image meta data].</param>
        /// <param name = "insertImageMetaData">if set to <c>true</c> [insert image meta data].</param>
        /// <param name = "saveImageToDisk">if set to <c>true</c> [save image to disk].</param>
        /// <returns></returns>
        public abstract ManagedImage ManageImage(CrawlRequest<TArachnodeDAO> crawlRequest, long imageID, string absoluteUri, byte[] source, string fullTextIndexType, bool extractImageMetaData, bool insertImageMetaData, bool saveImageToDisk);
    }
}
