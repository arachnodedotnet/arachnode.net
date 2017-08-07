using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arachnode.Configuration;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Managers;
using Arachnode.SiteCrawler.Value.Enums;
using HtmlAgilityPack;

namespace Arachnode.SiteCrawler.Value.AbstractClasses
{
    public abstract class AHtmlManager<TArachnodeDAO> : AManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        protected DiscoveryManager<TArachnodeDAO> _discoveryManager;

        protected AHtmlManager(ApplicationSettings applicationSettings, WebSettings webSettings, DiscoveryManager<TArachnodeDAO> discoveryManager) : base(applicationSettings, webSettings)
        {
            _discoveryManager = discoveryManager;
        }

        /// <summary>
        /// 	Creates the HTML document.
        /// </summary>
        /// <param name = "source">The source.</param>
        /// <param name = "encoding">The encoding.</param>
        /// <returns></returns>
        public abstract HtmlDocument CreateHtmlDocument(string source, Encoding encoding);

        /// <summary>
        /// 	Creates a HtmlDocument (WebPage) that references downloaded content.  If the Discovery isn't available locally, a remote (hotlinked) request is made.
        /// </summary>
        /// <param name = "webPageAbsoluteUri">The web page absolute URI.</param>
        /// <param name = "source">The source.</param>
        /// <param name = "uriQualificationType">Type of the URI qualification.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        /// <returns></returns>
        public abstract HtmlDocument CreateHtmlDocument(string webPageAbsoluteUri, string fullTextIndexType, string source, UriQualificationType uriQualificationType, IArachnodeDAO arachnodeDAO, bool prepareForLocalBrowsing);

        /// <summary>
        /// 	Creates a WebPages that references downloaded content.  If the Discovery isn't available locally, a remote (hotlinked) request is made.v
        /// </summary>
        /// <param name = "absoluteUri">The absolute URI.</param>
        /// <param name = "htmlNode">The HTML node.</param>
        /// <param name = "uriQualificationType">Type of the URI qualification.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        public abstract void QualifyNode(string absoluteUri, string fullTextIndexType, HtmlNode htmlNode, UriQualificationType uriQualificationType, IArachnodeDAO arachnodeDAO, bool prepareForLocalBrowsing);

        /// <summary>
        /// 	Get the AbsoluteUri for an Image.  The image Image be on disk, or we may need to hotlink to the Image.  The method is used by the search display facilities.
        /// </summary>
        /// <param name = "absoluteUri"></param>
        /// <param name = "arachnodeDAO"></param>
        /// <returns></returns>
        public abstract string GetImageUrl(string absoluteUri, string fullTextIndexType, IArachnodeDAO arachnodeDAO);

        /// <summary>
        /// 	Get the AbsoluteUri for a File.  The File could be on disk, or we may need to hotlink to the File.  The method is used by the search display facilities.
        /// </summary>
        /// <param name = "absoluteUri"></param>
        /// <param name = "arachnodeDAO"></param>
        /// <returns></returns>
        public abstract string GetFileUrl(string absoluteUri, string fullTextIndexType, IArachnodeDAO arachnodeDAO);
    }
}
