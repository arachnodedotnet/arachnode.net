#region License : arachnode.net

// // Copyright (c) 2015 http://arachnode.net, arachnode.net, LLC
// //  
// // Permission is hereby granted, upon purchase, to any person
// // obtaining a copy of this software and associated documentation
// // files (the "Software"), to deal in the Software without
// // restriction, including without limitation the rights to use,
// // copy, merge and modify copies of the Software, and to permit persons
// // to whom the Software is furnished to do so, subject to the following
// // conditions:
// // 
// // LICENSE (ALL VERSIONS/EDITIONS): http://arachnode.net/r.ashx?3
// // 
// // The above copyright notice and this permission notice shall be
// // included in all copies or substantial portions of the Software.
// // 
// // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// // EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// // OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// // NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// // HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// // WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// // FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// // OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Web.Services;
using System.Web.UI;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.Renderer.Value.Enums;
using Arachnode.Security;
using Arachnode.SiteCrawler;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.AbstractClasses;
using Arachnode.SiteCrawler.Value.Enums;
using Crawl2 = Arachnode.SiteCrawler.Components.Crawl<Arachnode.DataAccess.ArachnodeDAO>;

#endregion

namespace Application
{
    public partial class Crawl : Page
    {
        private static ApplicationSettings _applicationSettings = new ApplicationSettings();
        private static WebSettings _webSettings = new WebSettings();

        private static readonly object _fileDiscoveriesLock = new object();
        private static readonly object _hyperLinkDiscoveriesLock = new object();
        private static readonly object _imageDiscoveriesLock = new object();

        private static readonly Dictionary<string, List<Discovery<ArachnodeDAO>>> FileDiscoveries = new Dictionary<string, List<Discovery<ArachnodeDAO>>>();
        private static readonly Dictionary<string, List<Discovery<ArachnodeDAO>>> HyperLinkDiscoveries = new Dictionary<string, List<Discovery<ArachnodeDAO>>>();
        private static readonly Dictionary<string, List<Discovery<ArachnodeDAO>>> ImageDiscoveries = new Dictionary<string, List<Discovery<ArachnodeDAO>>>();

        private static readonly CacheItemRemovedCallback _cacheItemRemovedCallback = CacheItemRemoved;

        private static Crawler<ArachnodeDAO> _crawler;

        private static Crawl2 _crawl2;

        private static string UserId
        {
            get
            {
                //if (Session["UserID"] == null)
                //{
                //    Session["UserID"] = Guid.NewGuid();
                //}

                //return (Guid)Session["UserID"];

                return Guid.Empty.ToString();
            }
        }

        private Crawler<ArachnodeDAO> Crawler
        {
            get
            {
                if (_crawler == null)
                {
                    //ApplicationSettings can be set from code, overriding Database settings found in cfg.Configuration.
                    _applicationSettings.AssignCrawlRequestPrioritiesForFiles = false;
                    _applicationSettings.AssignCrawlRequestPrioritiesForHyperLinks = false;
                    _applicationSettings.AssignCrawlRequestPrioritiesForImages = false;
                    _applicationSettings.AssignCrawlRequestPrioritiesForWebPages = false;
                    _applicationSettings.AssignEmailAddressDiscoveries = true;
                    _applicationSettings.AssignFileAndImageDiscoveries = true;
                    _applicationSettings.AssignHyperLinkDiscoveries = true;
                    _applicationSettings.ClassifyAbsoluteUris = false;
                    //_applicationSettings.ConnectionString = "";
                    //_applicationSettings.ConsoleOutputLogsDirectory = "";
                    _applicationSettings.CrawlRequestTimeoutInMinutes = 1;
                    _applicationSettings.CreateCrawlRequestsFromDatabaseCrawlRequests = false;
                    _applicationSettings.CreateCrawlRequestsFromDatabaseFiles = false;
                    _applicationSettings.CreateCrawlRequestsFromDatabaseHyperLinks = false;
                    _applicationSettings.CreateCrawlRequestsFromDatabaseImages = false;
                    _applicationSettings.CreateCrawlRequestsFromDatabaseWebPages = false;
                    _applicationSettings.DesiredMaximumMemoryUsageInMegabytes = 128;
                    //_applicationSettings.DownloadedFilesDirectory = "";
                    //_applicationSettings.DownloadedImagesDirectory = "";
                    //_applicationSettings.DownloadedWebPagesDirectory = "";
                    _applicationSettings.EnableConsoleOutput = false;
                    _applicationSettings.ExtractFileMetaData = false;
                    _applicationSettings.ExtractImageMetaData = false;
                    _applicationSettings.ExtractWebPageMetaData = false;
                    _applicationSettings.HttpWebRequestRetries = 5;
                    _applicationSettings.InsertDisallowedAbsoluteUriDiscoveries = false;
                    _applicationSettings.InsertDisallowedAbsoluteUris = false;
                    _applicationSettings.InsertEmailAddressDiscoveries = false;
                    _applicationSettings.InsertEmailAddresses = false;
                    _applicationSettings.InsertExceptions = true;
                    _applicationSettings.InsertFileDiscoveries = false;
                    _applicationSettings.InsertFileMetaData = false;
                    _applicationSettings.InsertFiles = false;
                    _applicationSettings.InsertFileSource = false;
                    _applicationSettings.InsertHyperLinkDiscoveries = false;
                    _applicationSettings.InsertHyperLinks = false;
                    _applicationSettings.InsertImageDiscoveries = false;
                    _applicationSettings.InsertImageMetaData = false;
                    _applicationSettings.InsertImages = false;
                    _applicationSettings.InsertImageSource = false;
                    _applicationSettings.InsertWebPageMetaData = false;
                    _applicationSettings.InsertWebPages = false;
                    _applicationSettings.InsertWebPageSource = false;
                    _applicationSettings.MaximumNumberOfCrawlRequestsToCreatePerBatch = 1000;
                    _applicationSettings.MaximumNumberOfCrawlThreads = 10;
                    _applicationSettings.MaximumNumberOfHostsAndPrioritiesToSelect = 1000;
                    _applicationSettings.OutputConsoleToLogs = false;
                    _applicationSettings.SaveDiscoveredFilesToDisk = false;
                    _applicationSettings.SaveDiscoveredImagesToDisk = false;
                    _applicationSettings.SaveDiscoveredWebPagesToDisk = false;
                    _applicationSettings.UserAgent = "Your unique UserAgent string.";
                    _applicationSettings.VerboseOutput = false;

                    _crawler = new Crawler<ArachnodeDAO>(_applicationSettings, _webSettings, CrawlMode.BreadthFirstByPriority, false);

                    //Application["Crawler"] = crawler;

                    _crawler.Engine.CrawlRequestCompleted += Engine_CrawlRequestCompleted;

                    //set desired ApplicationSettings...
                    //this example disables all actions and rules, does not store Discoveries, and is configured to co-exist with a running Console or Service implementation.
                    //EmailAddresses, Files, HyperLinks and Images are assigned and returned for display.
                    //<IMPORTANT>as arachnode.net uses the ASP.Net object cache, this project should run in a seperate AppPool.</IMPORTANT>

                    //cfg.CrawlActions
                    foreach (ACrawlAction<ArachnodeDAO> crawlAction in _crawler.CrawlActions.Values)
                    {
                        crawlAction.IsEnabled = false;
                    }

                    //cfg.CrawlRules
                    foreach (ACrawlRule<ArachnodeDAO> crawlRule in _crawler.CrawlRules.Values)
                    {
                        crawlRule.IsEnabled = false;
                    }

                    //cfg.EngineActions
                    foreach (AEngineAction<ArachnodeDAO> engineAction in _crawler.Engine.EngineActions.Values)
                    {
                        engineAction.IsEnabled = false;
                    }

                    //start to instantiate the Managers.
                    _crawler.Engine.Start();

                    //stop to stop the threads.  (explicitly requesting Crawls from this application...)
                    _crawler.Engine.Stop();
                }

                return _crawler;
            }
        }

        private Crawl2 Crawl2
        {
            get
            {
                if (_crawl2 == null)
                {
                    _crawl2 = new Crawl2(Crawler.ApplicationSettings, Crawler.WebSettings, Crawler, Crawler.ActionManager, Crawler.ConsoleManager, Crawler.CookieManager, Crawler.CrawlRequestManager, Crawler.DataTypeManager, Crawler.DiscoveryManager, Crawler.EncodingManager, Crawler.HtmlManager, Crawler.PolitenessManager, Crawler.ProxyManager, Crawler.RuleManager, true);
                }

                return _crawl2;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void UxBtnCrawl_Click(object sender, EventArgs e)
        {
            lock (_fileDiscoveriesLock)
            {
                FileDiscoveries.Remove(UserId);
            }

            lock (_imageDiscoveriesLock)
            {
                ImageDiscoveries.Remove(UserId);
            }

            lock (_hyperLinkDiscoveriesLock)
            {
                HyperLinkDiscoveries.Remove(UserId);
            }

            var thread = new Thread(() => Crawl2.BeginCrawl(new CrawlRequest<ArachnodeDAO>(new Discovery<ArachnodeDAO>(uxTbAbsoluteUri.Text), 2, UriClassificationType.None, UriClassificationType.None, 2, RenderType.None, RenderType.None), false, false, false));

            thread.Start();
        }

        private static void CacheItemRemoved(string cacheKey, object o, CacheItemRemovedReason cacheItemRemovedReason)
        {
            DiscoveryType discoveryType = (DiscoveryType) o;

            switch (discoveryType)
            {
                case DiscoveryType.File:
                    lock (_fileDiscoveriesLock)
                    {
                        FileDiscoveries.Remove(UserId);
                    }
                    break;
                case DiscoveryType.Image:
                    lock (_imageDiscoveriesLock)
                    {
                        ImageDiscoveries.Remove(UserId);
                    }
                    break;
                case DiscoveryType.WebPage:
                    lock (_hyperLinkDiscoveriesLock)
                    {
                        HyperLinkDiscoveries.Remove(UserId);
                    }
                    break;
            }
        }

        private void Engine_CrawlRequestCompleted(CrawlRequest<ArachnodeDAO> crawlRequest)
        {
            switch (crawlRequest.Discovery.DiscoveryType)
            {
                case DiscoveryType.File:
                    HttpRuntime.Cache.Add(UserId, DiscoveryType.File, null, DateTime.MaxValue, TimeSpan.FromMinutes(_crawler.ApplicationSettings.CrawlRequestTimeoutInMinutes), CacheItemPriority.NotRemovable, _cacheItemRemovedCallback);

                    if (!FileDiscoveries.ContainsKey(UserId))
                    {
                        lock (_fileDiscoveriesLock)
                        {
                            FileDiscoveries.Add(UserId, new List<Discovery<ArachnodeDAO>>());
                        }
                    }

                    lock (FileDiscoveries[UserId])
                    {
                        FileDiscoveries[UserId].Add(crawlRequest.Discovery);
                    }
                    break;
                case DiscoveryType.Image:
                    HttpRuntime.Cache.Add(UserId, DiscoveryType.Image, null, DateTime.MaxValue, TimeSpan.FromMinutes(_crawler.ApplicationSettings.CrawlRequestTimeoutInMinutes), CacheItemPriority.NotRemovable, _cacheItemRemovedCallback);

                    if (!ImageDiscoveries.ContainsKey(UserId))
                    {
                        lock (_imageDiscoveriesLock)
                        {
                            ImageDiscoveries.Add(UserId, new List<Discovery<ArachnodeDAO>>());
                        }
                    }

                    lock (ImageDiscoveries[UserId])
                    {
                        ImageDiscoveries[UserId].Add(crawlRequest.Discovery);
                    }
                    break;
                case DiscoveryType.WebPage:
                    HttpRuntime.Cache.Add(UserId, DiscoveryType.HyperLink, null, DateTime.MaxValue, TimeSpan.FromMinutes(_crawler.ApplicationSettings.CrawlRequestTimeoutInMinutes), CacheItemPriority.NotRemovable, _cacheItemRemovedCallback);

                    if (!HyperLinkDiscoveries.ContainsKey(UserId))
                    {
                        lock (_imageDiscoveriesLock)
                        {
                            HyperLinkDiscoveries.Add(UserId, new List<Discovery<ArachnodeDAO>>());
                        }
                    }

                    lock (HyperLinkDiscoveries[UserId])
                    {
                        foreach (Discovery<ArachnodeDAO> hyperLinkDiscovery in crawlRequest.Discoveries.HyperLinks.Values)
                        {
                            HyperLinkDiscoveries[UserId].Add(hyperLinkDiscovery);
                        }
                    }

                    break;
            }
        }

        [WebMethod]
        public static string[] UpdateFiles()
        {
            List<string> fileDiscoveries = new List<string>();

            if (FileDiscoveries.ContainsKey(UserId))
            {
                lock (FileDiscoveries[UserId])
                {
                    foreach (Discovery<ArachnodeDAO> fileDiscovery in FileDiscoveries[UserId])
                    {
                        fileDiscoveries.Add(new Hash(fileDiscovery.Uri.AbsoluteUri) + "|" + fileDiscovery.Uri.AbsoluteUri);

                        if (fileDiscoveries.Count == 20)
                        {
                            break;
                        }
                    }

                    FileDiscoveries[UserId].Clear();
                }
            }

            return fileDiscoveries.ToArray();
        }

        [WebMethod]
        public static string[] UpdateHyperLinks()
        {
            List<string> hyperLinkDiscoveries = new List<string>();

            if (HyperLinkDiscoveries.ContainsKey(UserId))
            {
                lock (HyperLinkDiscoveries[UserId])
                {
                    foreach (Discovery<ArachnodeDAO> hyperLinkDiscovery in HyperLinkDiscoveries[UserId])
                    {
                        hyperLinkDiscoveries.Add(new Hash(hyperLinkDiscovery.Uri.AbsoluteUri) + "|" + hyperLinkDiscovery.Uri.AbsoluteUri);

                        if (hyperLinkDiscoveries.Count == 20)
                        {
                            break;
                        }
                    }

                    HyperLinkDiscoveries[UserId].Clear();
                }
            }

            return hyperLinkDiscoveries.ToArray();
        }

        [WebMethod]
        public static string[] UpdateImages()
        {
            List<string> imageDiscoveries = new List<string>();

            if (ImageDiscoveries.ContainsKey(UserId))
            {
                lock (ImageDiscoveries[UserId])
                {
                    foreach (Discovery<ArachnodeDAO> imageDiscovery in ImageDiscoveries[UserId])
                    {
                        imageDiscoveries.Add(new Hash(imageDiscovery.Uri.AbsoluteUri) + "|" + imageDiscovery.Uri.AbsoluteUri);
                    }

                    ImageDiscoveries[UserId].Clear();
                }
            }

            return imageDiscoveries.ToArray();
        }
    }
}