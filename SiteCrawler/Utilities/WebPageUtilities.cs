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
using System.IO;
using System.Text;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.DataSource;
using Arachnode.Renderer.Value.Enums;
using Arachnode.SiteCrawler.Components;
using Arachnode.SiteCrawler.Core;
using Arachnode.SiteCrawler.Managers;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.Enums;

#endregion

namespace Arachnode.SiteCrawler.Utilities
{
    public static class WebPageUtilities<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        #region Delegates

        public delegate void OnWebPageProcessedEventHandler(ArachnodeDataSet.WebPagesRow webPagesRow, string message);

        #endregion

        public static event OnWebPageProcessedEventHandler OnWebPageProcessed;

        /// <summary>
        /// 	Process a range of WebPageID after crawling.  Useful if crawled WebPages were not processed at crawl time according to desired ApplicationSettings configuration.
        /// 	Calling this method DOES change the 'LastDiscovered' fields where applicable.
        /// 	This method is not when crawling, rather during post-processing.
        /// </summary>
        /// <param name = "webPageIDLowerBound"></param>
        /// <param name = "webPageIDUpperBound"></param>
        public static void ProcessWebPages(Crawler<TArachnodeDAO> crawler, long webPageIDLowerBound, long webPageIDUpperBound)
        {
            //do not assign the application settings.  doing so will override the ApplicationSetting you set before calling this method...
            TArachnodeDAO arachnodeDAO = (TArachnodeDAO)Activator.CreateInstance(typeof(TArachnodeDAO), crawler.ApplicationSettings.ConnectionString, crawler.ApplicationSettings, crawler.WebSettings, false, false);

            ConsoleManager<TArachnodeDAO> consoleManager = new ConsoleManager<TArachnodeDAO>(crawler.ApplicationSettings, crawler.WebSettings);
            ActionManager<TArachnodeDAO> actionManager = new ActionManager<TArachnodeDAO>(crawler.ApplicationSettings, crawler.WebSettings, consoleManager);
            CookieManager cookieManager = new CookieManager();;
            MemoryManager<TArachnodeDAO> memoryManager = new MemoryManager<TArachnodeDAO>(crawler.ApplicationSettings, crawler.WebSettings);
            RuleManager<TArachnodeDAO> ruleManager = new RuleManager<TArachnodeDAO>(crawler.ApplicationSettings, crawler.WebSettings, consoleManager);
            CacheManager<TArachnodeDAO> cacheManager = new CacheManager<TArachnodeDAO>(crawler.ApplicationSettings, crawler.WebSettings);
            CrawlerPeerManager<TArachnodeDAO> crawlerPeerManager = new CrawlerPeerManager<TArachnodeDAO>(crawler.ApplicationSettings, crawler.WebSettings, null, arachnodeDAO);
            Cache<TArachnodeDAO> cache = new Cache<TArachnodeDAO>(crawler.ApplicationSettings, crawler.WebSettings, crawler, actionManager, cacheManager, crawlerPeerManager, memoryManager, ruleManager);
            DiscoveryManager<TArachnodeDAO> discoveryManager = new DiscoveryManager<TArachnodeDAO>(crawler.ApplicationSettings, crawler.WebSettings, cache, actionManager, cacheManager, memoryManager, ruleManager);
            HtmlManager<TArachnodeDAO> htmlManager = new HtmlManager<TArachnodeDAO>(crawler.ApplicationSettings, crawler.WebSettings, discoveryManager);

            //load the CrawlActions, CrawlRules and EngineActions...
            ruleManager.ProcessCrawlRules(crawler);
            actionManager.ProcessCrawlActions(crawler);
            actionManager.ProcessEngineActions(crawler);

            //these three methods are called in the Engine.
            UserDefinedFunctions.RefreshAllowedExtensions(true);
            UserDefinedFunctions.RefreshAllowedSchemes(true);
            UserDefinedFunctions.RefreshDisallowed();

            //instantiate a WebClient to access the ResponseHeaders...
            WebClient<TArachnodeDAO> webClient = new WebClient<TArachnodeDAO>(crawler.ApplicationSettings, crawler.WebSettings, consoleManager, cookieManager, new ProxyManager<TArachnodeDAO>(crawler.ApplicationSettings, crawler.WebSettings, consoleManager));

            webClient.GetHttpWebResponse("http://google.com", "GET", null, null, null, null);

            WebPageManager<TArachnodeDAO> webPageManager = new WebPageManager<TArachnodeDAO>(crawler.ApplicationSettings, crawler.WebSettings, discoveryManager, htmlManager, arachnodeDAO);

            for (long i = webPageIDLowerBound; i <= webPageIDUpperBound; i++)
            {
                ArachnodeDataSet.WebPagesRow webPagesRow = null;

                try
                {
                    //get the WebPage from the database.  we need the source data as we don't store this in the index.
                    //even though most of the fields are available in the Document, the WebPage is the authoritative source, so we'll use that for all of the fields.
                    webPagesRow = arachnodeDAO.GetWebPage(i.ToString());

                    if (webPagesRow != null)
                    {
                        if (webPagesRow.Source == null || webPagesRow.Source.Length == 0)
                        {
                            if (File.Exists(discoveryManager.GetDiscoveryPath(crawler.ApplicationSettings.DownloadedWebPagesDirectory, webPagesRow.AbsoluteUri, webPagesRow.FullTextIndexType)))
                            {
                                using (StreamReader streamReader = File.OpenText(discoveryManager.GetDiscoveryPath(crawler.ApplicationSettings.DownloadedWebPagesDirectory, webPagesRow.AbsoluteUri, webPagesRow.FullTextIndexType)))
                                {
                                    webPagesRow.Source = Encoding.UTF8.GetBytes(streamReader.ReadToEnd());
                                }
                            }
                            else
                            {
                                Console.WriteLine("WebPageID: " + i + " was NOT processed successfully.");
                                if (OnWebPageProcessed != null)
                                {
                                    OnWebPageProcessed.BeginInvoke(webPagesRow, "WebPageID: " + i + " was NOT processed successfully.", null, null);
                                }
                            }
                        }

                        ProcessWebPage(crawler.ApplicationSettings, crawler.WebSettings, crawler, webPagesRow, webClient, cache, actionManager, consoleManager, crawlerPeerManager, discoveryManager, memoryManager, ruleManager, webPageManager, arachnodeDAO);

                        Console.WriteLine("WebPageID: " + i + " was processed successfully.");
                        if (OnWebPageProcessed != null)
                        {
                            OnWebPageProcessed.BeginInvoke(webPagesRow, "WebPageID: " + i + " was processed successfully.", null, null);
                        }
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine("WebPageID: " + i + " was NOT processed successfully.");
                    Console.WriteLine(exception.Message);

                    if (OnWebPageProcessed != null)
                    {
                        OnWebPageProcessed.BeginInvoke(webPagesRow, "WebPageID: " + i + " was NOT processed successfully.", null, null);
                        OnWebPageProcessed.BeginInvoke(webPagesRow, exception.Message, null, null);
                    }

                    arachnodeDAO.InsertException(null, null, exception, false);
                }
            }

            //stop the CrawlActions, CrawlRules and EngineActions...
            ruleManager.Stop();
            actionManager.Stop();
        }

        /// <summary>
        /// 	Processes a WebPagesRow after crawling.
        /// </summary>
        /// <param name = "webPagesRow">The web pages row.</param>
        /// <param name="webClient"></param>
        /// <param name="actionManager"></param>
        /// <param name="consoleManager"></param>
        /// <param name="discoveryManager"></param>
        /// <param name="memoryManager"></param>
        /// <param name="ruleManager"></param>
        /// <param name = "webPageManager">The web page manager.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        /// <param name = "fileManager">The file manager.</param>
        /// <param name = "imageManager">The image manager.</param>
        public static void ProcessWebPage(ApplicationSettings applicationSettings, WebSettings webSettings, Crawler<TArachnodeDAO> crawler, ArachnodeDataSet.WebPagesRow webPagesRow, WebClient<TArachnodeDAO> webClient, Cache<TArachnodeDAO> cache, ActionManager<TArachnodeDAO> actionManager, ConsoleManager<TArachnodeDAO> consoleManager, CrawlerPeerManager<TArachnodeDAO> crawlerPeerManager, DiscoveryManager<TArachnodeDAO> discoveryManager, MemoryManager<TArachnodeDAO> memoryManager, RuleManager<TArachnodeDAO> ruleManager, WebPageManager<TArachnodeDAO> webPageManager, IArachnodeDAO arachnodeDAO)
        {
            CacheManager<TArachnodeDAO> cacheManager = new CacheManager<TArachnodeDAO>(applicationSettings, webSettings);
            CookieManager cookieManager = new CookieManager();
            CrawlRequestManager<TArachnodeDAO> crawlRequestManager = new CrawlRequestManager<TArachnodeDAO>(applicationSettings, webSettings, cache, consoleManager, discoveryManager);
            DataTypeManager<TArachnodeDAO> dataTypeManager = new DataTypeManager<TArachnodeDAO>(applicationSettings, webSettings);
            EncodingManager<TArachnodeDAO> encodingManager = new EncodingManager<TArachnodeDAO>(applicationSettings, webSettings);
            PolitenessManager<TArachnodeDAO> politenessManager = new PolitenessManager<TArachnodeDAO>(applicationSettings, webSettings, cache);
            ProxyManager<TArachnodeDAO> proxyManager = new ProxyManager<TArachnodeDAO>(applicationSettings, webSettings, consoleManager);
            HtmlManager<TArachnodeDAO> htmlManager = new HtmlManager<TArachnodeDAO>(applicationSettings, webSettings, discoveryManager);
            Crawl<TArachnodeDAO> crawl = new Crawl<TArachnodeDAO>(applicationSettings, webSettings, crawler, actionManager, consoleManager, cookieManager, crawlRequestManager, dataTypeManager, discoveryManager, encodingManager, htmlManager, politenessManager, proxyManager, ruleManager, true);

            //create a CrawlRequest as this is what the internals of SiteCrawler.dll expect to operate on...
            CrawlRequest<TArachnodeDAO> crawlRequest = new CrawlRequest<TArachnodeDAO>(new Discovery<TArachnodeDAO>(webPagesRow.AbsoluteUri), webPagesRow.CrawlDepth, UriClassificationType.Host, UriClassificationType.Host, 0, RenderType.None, RenderType.None);

            crawlRequest.Crawl = crawl;
            crawlRequest.Discovery.DiscoveryType = DiscoveryType.WebPage;
            crawlRequest.Discovery.ID = webPagesRow.ID;
            crawlRequest.Data = webPagesRow.Source;
            crawlRequest.CurrentDepth = webPagesRow.CrawlDepth;
            crawlRequest.Encoding = Encoding.GetEncoding(webPagesRow.CodePage);
            crawlRequest.ProcessData = true;
            crawlRequest.WebClient = webClient;

            crawlRequest.WebClient.HttpWebResponse.Headers.Clear();

            //parse the ResponseHeaders from the WebPagesRow.ResponseHeaders string...
            foreach (string responseHeader in webPagesRow.ResponseHeaders.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                string[] responseHeaderSplit = responseHeader.Split(":".ToCharArray());

                string name = responseHeaderSplit[0];
                string value = UserDefinedFunctions.ExtractResponseHeader(webPagesRow.ResponseHeaders, name, true).Value;

                crawlRequest.WebClient.HttpWebResponse.Headers.Add(name, value);
            }

            //refresh the DataTypes in the DataTypeManager... (if necessary)...
            if (dataTypeManager.AllowedDataTypes.Count == 0)
            {
                dataTypeManager.RefreshDataTypes();
            }

            crawlRequest.DataType = dataTypeManager.DetermineDataType(crawlRequest);

            //now, process the bytes...
            encodingManager.ProcessCrawlRequest(crawlRequest, arachnodeDAO);

            if (applicationSettings.InsertWebPages)
            {
                crawlRequest.Discovery.ID = arachnodeDAO.InsertWebPage(crawlRequest.Discovery.Uri.AbsoluteUri, crawlRequest.WebClient.HttpWebResponse.Headers.ToString(), applicationSettings.InsertWebPageSource ? crawlRequest.Data : new byte[] { }, crawlRequest.Encoding.CodePage, crawlRequest.DataType.FullTextIndexType, crawlRequest.CurrentDepth, applicationSettings.ClassifyAbsoluteUris);
            }

            crawlRequest.ManagedDiscovery = webPageManager.ManageWebPage(crawlRequest.Discovery.ID.Value, crawlRequest.Discovery.Uri.AbsoluteUri, crawlRequest.Data, crawlRequest.Encoding, crawlRequest.DataType.FullTextIndexType, applicationSettings.ExtractWebPageMetaData, applicationSettings.InsertWebPageMetaData, applicationSettings.SaveDiscoveredWebPagesToDisk);

            //assigning FileAndImageDiscoveries isn't applicable because Files and Images need to be crawled to be properly classified... without classification we don't know whether they belong in dbo.Files or dbo.Images...
            crawlRequestManager.ProcessEmailAddresses(crawlRequest, arachnodeDAO);
            crawlRequestManager.ProcessHyperLinks(crawlRequest, arachnodeDAO);

            actionManager.PerformCrawlActions(crawlRequest, CrawlActionType.PostRequest, arachnodeDAO);

            discoveryManager.CloseAndDisposeManagedDiscovery(crawlRequest, arachnodeDAO);
        }
    }
}