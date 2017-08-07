using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Arachnode.Configuration;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Core;
using Arachnode.SiteCrawler.Managers;
using Arachnode.SiteCrawler.Value.Enums;

namespace Arachnode.SiteCrawler.Value.AbstractClasses
{
    public abstract class ADiscoveryManager<TArachnodeDAO> : AManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        protected readonly Regex _emailAddressRegex = new Regex(@"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        protected readonly Regex _fileOrImageRegex = new Regex("<\\s*(?<Tag>(applet|embed|frame|iframe|img|link|script|xml))\\s*.*?(?<AttributeName>(src|href|xhref))\\s*=\\s*([\\\"\\'])(?<FileOrImage>.*?)\\3", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        protected readonly Regex _hyperLinkRegex = new Regex("<\\s*(?<Tag>(a|base|form|frame))\\s*.*?(?<AttributeName>(action|href|src))\\s*=\\s*([\\\"\\'])(?<HyperLink>.*?)\\3", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        protected Cache<TArachnodeDAO> _cache;
        protected ActionManager<TArachnodeDAO> _actionManager;
        protected CacheManager<TArachnodeDAO> _cacheManager;
        protected MemoryManager<TArachnodeDAO> _memoryManager;
        protected RuleManager<TArachnodeDAO> _ruleManager;

        protected ADiscoveryManager(ApplicationSettings applicationSettings, WebSettings webSettings, Cache<TArachnodeDAO> cache, ActionManager<TArachnodeDAO> actionManager, CacheManager<TArachnodeDAO> cacheManager, MemoryManager<TArachnodeDAO> memoryManager, RuleManager<TArachnodeDAO> ruleManager) : base(applicationSettings, webSettings)
        {
            _cache = cache;

            _actionManager = actionManager;
            _cacheManager = cacheManager;
            _memoryManager = memoryManager;
            _ruleManager = ruleManager;
        }

        public abstract Regex EmailAddressRegex { get; }
        public abstract Regex FileOrImageRegex { get; }
        public abstract Regex HyperLinkRegex { get; }

        /// <summary>
        /// 	Assigns the email address discoveries.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        public abstract void AssignEmailAddressDiscoveries(CrawlRequest<TArachnodeDAO> crawlRequest, IArachnodeDAO arachnodeDAO);

        /// <summary>
        /// 	Assigns the hyper link discoveries.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        public abstract void AssignHyperLinkDiscoveries(CrawlRequest<TArachnodeDAO> crawlRequest, IArachnodeDAO arachnodeDAO);

        /// <summary>
        /// 	Assigns the file and image discoveries.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        public abstract void AssignFileAndImageDiscoveries(CrawlRequest<TArachnodeDAO> crawlRequest, IArachnodeDAO arachnodeDAO);

        /// <summary>
        /// Decodes a match value such that html encoded references will match the base href (if used).
        /// </summary>
        /// <param name="match"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public abstract string GetGroupValue(Match match, string groupName);

        /// <summary>
        /// The regular expression balances speed and accuracy and is correct the vast majority of the time.
        /// The grouping of the tags and the attribute names allows mixing and matching, as many browsers will account for invalid HTML and present the user with "This is what the HTML developer really meant".
        /// HTML such as "script source='\something'" is allowed when 'src' is the syntactically correct attribute name.
        /// There are cases where due to backreferencing (\3) which allows " AND ' to designate match boundaries that may occasionally result in matches contanining extraneous characters which may result in false positives for File and/or Image matches.
        /// If an AbsoluteUri is dicovered as a File or Image, it will not be allowed to be discovered as a WebPage.
        /// As the Tag and Attribute name are known after regular expression evaluation, it is beneficial to evaluate for proper tag and attribute name combinations.
        /// Evalating after regular expression evaluation is much faster than providing multiple regular expressions which specify concrete sets thereby requiring parsing the HTML multiple times.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public abstract bool IsFileAndImageMatchValid(Match match);

        /// <summary>
        /// 	Determines whether the specified crawl request is restricted.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "absoluteUri">The absolute URI.</param>
        /// <returns>
        /// 	<c>true</c> if the specified crawl request is restricted; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool IsCrawlRestricted(CrawlRequest<TArachnodeDAO> crawlRequest, string absoluteUri);

        /// <summary>
        /// 	Determines whether [is discovery restricted] [the specified crawl request].
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "absoluteUri">The absolute URI.</param>
        /// <returns>
        /// 	<c>true</c> if [is discovery restricted] [the specified crawl request]; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool IsDiscoveryRestricted(CrawlRequest<TArachnodeDAO> crawlRequest, string absoluteUri);

        /// <summary>
        /// 	Determines whether the specified crawl request is restricted.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "absoluteUri">The absolute URI.</param>
        /// <param name = "uriClassificationType">Type of the URI classification.</param>
        /// <returns>
        /// 	<c>true</c> if the specified crawl request is restricted; otherwise, <c>false</c>.
        /// </returns>
        protected abstract bool IsRestricted(CrawlRequest<TArachnodeDAO> crawlRequest, string absoluteUri, short uriClassificationType);

        /// <summary>
        /// 	Manages the discovery.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "discoveryState">State of the discovery.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        public abstract void ManageDiscovery(CrawlRequest<TArachnodeDAO> crawlRequest, DiscoveryState discoveryState, IArachnodeDAO arachnodeDAO);

        public abstract bool WasCrawlRequestRedirected(CrawlRequest<TArachnodeDAO> crawlRequest);
        public abstract void CloseAndDisposeManagedDiscovery(CrawlRequest<TArachnodeDAO> crawlRequest, IArachnodeDAO arachnodeDAO);

        /// <summary>
        /// 	Gets the discovery path.
        /// </summary>
        /// <param name = "downloadedDiscoveryDirectory">The downloaded discovery directory.</param>
        /// <param name = "absoluteUri">The absolute URI.</param>
        /// <returns></returns>
        public abstract string GetDiscoveryPath(string downloadedDiscoveryDirectory, string absoluteUri, string fullTextIndexType);

        public abstract string GetDiscoveryPath(string downloadedDiscoveryVirtualDirectory, string absoluteUri);
        public abstract bool DoesDiscoveryExist(string discoveryPath);
        public abstract string GetDiscoveryExtension(string discoveryPath);
        public abstract byte[] GetFileSource(string fileAbsoluteUriOrID, IArachnodeDAO arachnodeDAO);
        public abstract byte[] GetImageSource(string imageAbsoluteUriOrID, IArachnodeDAO arachnodeDAO);
        public abstract string GetWebPageSource(string webPageAbsoluteUriOrID, IArachnodeDAO arachnodeDAO);
    }
}
