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
    public static class ImageUtilities<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        #region Delegates

        public delegate void OnImageProcessedEventHandler(ArachnodeDataSet.ImagesRow imaesRow, string message);

        #endregion

        public static event OnImageProcessedEventHandler OnImageProcessed;

        /// <summary>
        /// 	Process a range of ImageID after crawling.  Useful if crawled Images were not processed at crawl time according to desired ApplicationSettings configuration.
        /// 	Calling this method DOES change the 'LastDiscovered' fields where applicable.
        /// 	This method is not when crawling, rather during post-processing.
        /// </summary>
        /// <param name = "imageIDLowerBound"></param>
        /// <param name = "imageIDUpperBound"></param>
        public static void ProcessImages(Crawler<TArachnodeDAO> crawler, long imageIDLowerBound, long imageIDUpperBound)
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

            //load the CrawlActions, CrawlRules and EngineActions...
            ruleManager.ProcessCrawlRules(crawler);
            actionManager.ProcessCrawlActions(crawler);
            actionManager.ProcessEngineActions(crawler);

            //these three methods are called in the Engine.
            UserDefinedFunctions.RefreshAllowedExtensions(true);
            UserDefinedFunctions.RefreshAllowedSchemes(true);
            UserDefinedFunctions.RefreshDisallowed();

            //instantiate a WebClient to access the ResponseHeaders...
            WebClient<TArachnodeDAO> webClient = new WebClient<TArachnodeDAO>(crawler.ApplicationSettings, arachnodeDAO.WebSettings, consoleManager, cookieManager, new ProxyManager<TArachnodeDAO>(crawler.ApplicationSettings, crawler.WebSettings, consoleManager));

            webClient.GetHttpWebResponse("http://google.com", "GET", null, null, null, null);

            ImageManager<TArachnodeDAO> imageManager = new ImageManager<TArachnodeDAO>(crawler.ApplicationSettings, crawler.WebSettings, discoveryManager, arachnodeDAO);

            for (long i = imageIDLowerBound; i <= imageIDUpperBound; i++)
            {
                ArachnodeDataSet.ImagesRow imagesRow = null;

                try
                {
                    //get the Image from the database.  we need the source data as we don't store this in the index.
                    //even though most of the fields are available in the Document, the Image is the authoritative source, so we'll use that for all of the fields.
                    imagesRow = arachnodeDAO.GetImage(i.ToString());

                    if (imagesRow != null)
                    {
                        if (imagesRow.Source == null || imagesRow.Source.Length == 0)
                        {
                            if (File.Exists(discoveryManager.GetDiscoveryPath(crawler.ApplicationSettings.DownloadedImagesDirectory, imagesRow.AbsoluteUri, imagesRow.FullTextIndexType)))
                            {
                                imagesRow.Source = File.ReadAllBytes(discoveryManager.GetDiscoveryPath(crawler.ApplicationSettings.DownloadedImagesDirectory, imagesRow.AbsoluteUri, imagesRow.FullTextIndexType));
                            }
                            else
                            {
                                Console.WriteLine("ImageID: " + i + " was NOT processed successfully.");
                                if (OnImageProcessed != null)
                                {
                                    OnImageProcessed.BeginInvoke(imagesRow, "ImageID: " + i + " was NOT processed successfully.", null, null);
                                }
                            }
                        }

                        ProcessImage(crawler.ApplicationSettings, crawler.WebSettings, crawler, imagesRow, webClient, cache, actionManager, consoleManager, crawlerPeerManager, discoveryManager, imageManager, memoryManager, ruleManager, arachnodeDAO);

                        Console.WriteLine("ImageID: " + i + " was processed successfully.");
                        if (OnImageProcessed != null)
                        {
                            OnImageProcessed.BeginInvoke(imagesRow, "ImageID: " + i + " was processed successfully.", null, null);
                        }
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine("ImageID: " + i + " was NOT processed successfully.");
                    Console.WriteLine(exception.Message);

                    if (OnImageProcessed != null)
                    {
                        OnImageProcessed.BeginInvoke(imagesRow, "ImageID: " + i + " was NOT processed successfully.", null, null);
                        OnImageProcessed.BeginInvoke(imagesRow, exception.Message, null, null);
                    }

                    arachnodeDAO.InsertException(null, null, exception, false);
                }
            }

            //stop the CrawlActions, CrawlRules and EngineActions...
            ruleManager.Stop();
            actionManager.Stop();
        }

        /// <summary>
        /// 	Processes an ImagesRow after crawling.
        /// </summary>
        /// <param name = "imagesRow">The images row.</param>
        /// <param name="webClient"></param>
        /// <param name="actionManager"></param>
        /// <param name="consoleManager"></param>
        /// <param name="discoveryManager"></param>
        /// <param name = "imageManager">The image manager.</param>
        /// <param name = "imageManager">The image manager.</param>
        /// <param name = "imageManager">The image manager.</param>
        /// <param name="memoryManager"></param>
        /// <param name="ruleManager"></param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        public static void ProcessImage(ApplicationSettings applicationSettings, WebSettings webSettings, Crawler<TArachnodeDAO> crawler, ArachnodeDataSet.ImagesRow imagesRow, WebClient<TArachnodeDAO> webClient, Cache<TArachnodeDAO> cache, ActionManager<TArachnodeDAO> actionManager, ConsoleManager<TArachnodeDAO> consoleManager, CrawlerPeerManager<TArachnodeDAO> crawlerPeerManager, DiscoveryManager<TArachnodeDAO> discoveryManager, ImageManager<TArachnodeDAO> imageManager, MemoryManager<TArachnodeDAO> memoryManager, RuleManager<TArachnodeDAO> ruleManager, IArachnodeDAO arachnodeDAO)
        {
            CacheManager<TArachnodeDAO> cacheManager = new CacheManager<TArachnodeDAO>(applicationSettings, webSettings);
            CookieManager cookieManager = new CookieManager();;
            CrawlRequestManager<TArachnodeDAO> crawlRequestManager = new CrawlRequestManager<TArachnodeDAO>(applicationSettings, webSettings, cache, consoleManager, discoveryManager);
            DataTypeManager<TArachnodeDAO> dataTypeManager = new DataTypeManager<TArachnodeDAO>(applicationSettings, webSettings);
            EncodingManager<TArachnodeDAO> encodingManager = new EncodingManager<TArachnodeDAO>(applicationSettings, webSettings);
            PolitenessManager<TArachnodeDAO> politenessManager = new PolitenessManager<TArachnodeDAO>(applicationSettings, webSettings, cache);
            ProxyManager<TArachnodeDAO> proxyManager = new ProxyManager<TArachnodeDAO>(applicationSettings, webSettings, consoleManager);
            HtmlManager<TArachnodeDAO> htmlManager = new HtmlManager<TArachnodeDAO>(applicationSettings, webSettings, discoveryManager);
            Crawl<TArachnodeDAO> crawl = new Crawl<TArachnodeDAO>(applicationSettings, webSettings, crawler, actionManager, consoleManager, cookieManager, crawlRequestManager, dataTypeManager, discoveryManager, encodingManager, htmlManager, politenessManager, proxyManager, ruleManager, true);

            //create a CrawlRequest as this is what the internals of SiteCrawler.dll expect to operate on...
            CrawlRequest<TArachnodeDAO> crawlRequest = new CrawlRequest<TArachnodeDAO>(new Discovery<TArachnodeDAO>(imagesRow.AbsoluteUri), 1, UriClassificationType.Host, UriClassificationType.Host, 0, RenderType.None, RenderType.None);

            crawlRequest.Crawl = crawl;
            crawlRequest.Discovery.DiscoveryType = DiscoveryType.Image;
            crawlRequest.Discovery.ID = imagesRow.ID;
            crawlRequest.Data = imagesRow.Source;
            crawlRequest.ProcessData = true;
            crawlRequest.WebClient = webClient;

            crawlRequest.WebClient.HttpWebResponse.Headers.Clear();

            //parse the ResponseHeaders from the ImagesRow.ResponseHeaders string...
            foreach (string responseHeader in imagesRow.ResponseHeaders.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                string[] responseHeaderSplit = responseHeader.Split(":".ToCharArray());

                string name = responseHeaderSplit[0];
                string value = UserDefinedFunctions.ExtractResponseHeader(imagesRow.ResponseHeaders, name, true).Value;

                crawlRequest.WebClient.HttpWebResponse.Headers.Add(name, value);
            }

            //refresh the DataTypes in the DataTypeManager... (if necessary)...
            if (dataTypeManager.AllowedDataTypes.Count == 0)
            {
                dataTypeManager.RefreshDataTypes();
            }

            crawlRequest.DataType = dataTypeManager.DetermineDataType(crawlRequest);

            if (applicationSettings.InsertImages)
            {
                crawlRequest.Discovery.ID = arachnodeDAO.InsertImage(crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri, crawlRequest.WebClient.HttpWebResponse.Headers.ToString(), applicationSettings.InsertImageSource ? crawlRequest.Data : new byte[] {}, crawlRequest.DataType.FullTextIndexType);
            }

            crawlRequest.ManagedDiscovery = imageManager.ManageImage(crawlRequest, crawlRequest.Discovery.ID.Value, crawlRequest.Discovery.Uri.AbsoluteUri, crawlRequest.Data, crawlRequest.DataType.FullTextIndexType, applicationSettings.ExtractImageMetaData, applicationSettings.InsertImageMetaData, applicationSettings.SaveDiscoveredImagesToDisk);

            actionManager.PerformCrawlActions(crawlRequest, CrawlActionType.PostRequest, arachnodeDAO);

            discoveryManager.CloseAndDisposeManagedDiscovery(crawlRequest, arachnodeDAO);
        }
    }
}