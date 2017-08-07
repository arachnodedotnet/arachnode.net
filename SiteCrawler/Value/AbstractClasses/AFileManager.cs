using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arachnode.Configuration;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Managers;

namespace Arachnode.SiteCrawler.Value.AbstractClasses
{
    public abstract class AFileManager<TArachnodeDAO> : AManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        protected DiscoveryManager<TArachnodeDAO> _discoveryManager;
        protected IArachnodeDAO _arachnodeDAO;

        /// <summary>
        /// 	Initializes a new instance of the <see cref = "FileManager{TArachnodeDAO}" /> class.
        /// </summary>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        protected AFileManager(ApplicationSettings applicationSettings, WebSettings webSettings, DiscoveryManager<TArachnodeDAO> discoveryManager, IArachnodeDAO arachnodeDAO) : base(applicationSettings, webSettings)
        {
            _discoveryManager = discoveryManager;
            _arachnodeDAO = arachnodeDAO;
        }

        /// <summary>
        /// 	Manages the file.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        public abstract void ManageFile(CrawlRequest<TArachnodeDAO> crawlRequest);

        /// <summary>
        /// 	Manages the file.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "fileID">The file ID.</param>
        /// <param name = "absoluteUri">The absolute URI.</param>
        /// <param name = "source">The source.</param>
        /// <param name = "fullTextIndexType">Full type of the text index.</param>
        /// <param name = "extractFileMetaData">if set to <c>true</c> [extract file meta data].</param>
        /// <param name = "insertFileMetaData">if set to <c>true</c> [insert file meta data].</param>
        /// <param name = "saveFileToDisk">if set to <c>true</c> [save file to disk].</param>
        /// <returns></returns>
        public abstract ManagedFile ManageFile(CrawlRequest<TArachnodeDAO> crawlRequest, long fileID, string absoluteUri, byte[] source, string fullTextIndexType, bool extractFileMetaData, bool insertFileMetaData, bool saveFileToDisk);
    }
}
