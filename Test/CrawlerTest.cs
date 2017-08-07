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
using System.Configuration;
using System.Threading;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.Renderer.Value.Enums;
using Arachnode.SiteCrawler;
using Arachnode.SiteCrawler.Core;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.AbstractClasses;
using Arachnode.SiteCrawler.Value.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace Test
{
    /// <summary>
    /// 	Summary description for CrawlerTest
    /// </summary>
    [TestClass]
    public class CrawlerTest
    {
        private bool _isCrawl1Completed = false;
        private bool _isCrawl2Completed = false;

        ///<summary>
        ///	Gets or sets the test context which provides
        ///	information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion

        [TestMethod]
        public void EnsureAllTestWebPagesAreCrawler()
        {
            ConfigurationManager.ConnectionStrings.Add(new ConnectionStringSettings("arachnode_net_ConnectionString", "Data Source=.;Initial Catalog=arachnode.net;Integrated Security=True;Connection Timeout=3600;"));

            ApplicationSettings applicationSettings = new ApplicationSettings();
            WebSettings webSettings = new WebSettings();

            var crawler = new Crawler<ArachnodeDAO>(applicationSettings, webSettings, CrawlMode.DepthFirstByPriority, false);

            crawler.ApplicationSettings.AssignCrawlRequestPrioritiesForFiles = false;
            crawler.ApplicationSettings.AssignCrawlRequestPrioritiesForHyperLinks = false;
            crawler.ApplicationSettings.AssignCrawlRequestPrioritiesForImages = false;

            crawler.ApplicationSettings.ClassifyAbsoluteUris = false;
            crawler.ApplicationSettings.InsertHyperLinks = false;
            crawler.ApplicationSettings.InsertHyperLinkDiscoveries = false;

            crawler.ApplicationSettings.SaveDiscoveredWebPagesToDisk = false;

            foreach (ACrawlAction<ArachnodeDAO> crawlAction in crawler.CrawlActions.Values)
            {
                crawlAction.IsEnabled = false;
            }

            foreach (ACrawlRule<ArachnodeDAO> crawlRule in crawler.CrawlRules.Values)
            {
                crawlRule.IsEnabled = false;
            }

            foreach (AEngineAction<ArachnodeDAO> engineAction in crawler.Engine.EngineActions.Values)
            {
                engineAction.IsEnabled = false;
            }

            crawler.ApplicationSettings.InsertHyperLinks = false;
            crawler.ApplicationSettings.InsertHyperLinkDiscoveries = false;
            crawler.ApplicationSettings.ClassifyAbsoluteUris = false;
            crawler.ApplicationSettings.InsertImageSource = false;
            crawler.ApplicationSettings.AssignEmailAddressDiscoveries = false;
            crawler.ApplicationSettings.MaximumNumberOfCrawlThreads = 5;

            //A depth of 1 means 'crawl this page only'.
            //Setting the Depth to int.Max means to crawl the first page, and then int.MaxValue - 1 hops away from the initial CrawlRequest AbsoluteUri.
            //The higher the value for 'Priority', the higher the Priority.
            bool wasTheCrawlRequestAddedForCrawling = crawler.Crawl(new CrawlRequest<ArachnodeDAO>(new Discovery<ArachnodeDAO>("http://localhost:56830/Test.aspx"), int.MaxValue, UriClassificationType.Host, UriClassificationType.Host, 1, RenderType.None, RenderType.None));

            crawler.Engine.Start();

            while (crawler.Engine.State == EngineState.Start)
            {
                Thread.Sleep(5000);
            }
        }

        [TestMethod]
        public void TestCrawlerConcurrency()
        {
            var thread1 = new Thread(delegate()
                                         {
                                             try
                                             {
                                                 ApplicationSettings applicationSettings = new ApplicationSettings();
                                                 WebSettings webSettings = new WebSettings();

                                                 var crawler1 = new Crawler<ArachnodeDAO>(applicationSettings, webSettings, CrawlMode.BreadthFirstByPriority, false);

                                                 crawler1.ApplicationSettings.CreateCrawlRequestsFromDatabaseCrawlRequests = false;

                                                 crawler1.ClearUncrawledCrawlRequests();
                                                 crawler1.ClearDiscoveries();

                                                 foreach (ACrawlAction<ArachnodeDAO> crawlAction in crawler1.CrawlActions.Values)
                                                 {
                                                     crawlAction.IsEnabled = false;
                                                 }

                                                 foreach (ACrawlRule<ArachnodeDAO> crawlRule in crawler1.CrawlRules.Values)
                                                 {
                                                     crawlRule.IsEnabled = false;
                                                 }

                                                 foreach (AEngineAction<ArachnodeDAO> engineAction in crawler1.Engine.EngineActions.Values)
                                                 {
                                                     engineAction.IsEnabled = false;
                                                 }

                                                 crawler1.Engine.CrawlRequestCompleted += Engine_CrawlRequestCompleted1;
                                                 crawler1.Engine.CrawlCompleted += Engine_CrawlCompleted1;

                                                 crawler1.Crawl(new CrawlRequest<ArachnodeDAO>(new Discovery<ArachnodeDAO>("http://cbs.com"), 2, UriClassificationType.Domain, UriClassificationType.Domain, 1, RenderType.None, RenderType.None));

                                                 crawler1.Engine.Start();

                                                 while (!_isCrawl2Completed)
                                                 {
                                                     Thread.Sleep(1000);
                                                 }
                                             }
                                             catch (Exception exception)
                                             {
                                                 Console.WriteLine(exception.Message);
                                             }
                                         });

            var thread2 = new Thread(delegate()
                                         {
                                             try
                                             {
                                                ApplicationSettings applicationSettings = new ApplicationSettings();
                                                WebSettings webSettings = new WebSettings();

                                                var crawler2 = new Crawler<ArachnodeDAO>(applicationSettings, webSettings, CrawlMode.BreadthFirstByPriority, false);

                                                 crawler2.ApplicationSettings.CreateCrawlRequestsFromDatabaseCrawlRequests = false;

                                                 crawler2.ClearDiscoveries();

                                                 foreach (ACrawlAction<ArachnodeDAO> crawlAction in crawler2.CrawlActions.Values)
                                                 {
                                                     crawlAction.IsEnabled = false;
                                                 }

                                                 foreach (ACrawlRule<ArachnodeDAO> crawlRule in crawler2.CrawlRules.Values)
                                                 {
                                                     crawlRule.IsEnabled = false;
                                                 }

                                                 foreach (AEngineAction<ArachnodeDAO> engineAction in crawler2.Engine.EngineActions.Values)
                                                 {
                                                     engineAction.IsEnabled = false;
                                                 }

                                                 crawler2.Engine.CrawlRequestCompleted += Engine_CrawlRequestCompleted2;
                                                 crawler2.Engine.CrawlCompleted += Engine_CrawlCompleted2;

                                                 crawler2.Crawl(new CrawlRequest<ArachnodeDAO>(new Discovery<ArachnodeDAO>("http://nbc.com"), 1, UriClassificationType.Domain, UriClassificationType.Domain, 1, RenderType.None, RenderType.None));

                                                 crawler2.Engine.Start();

                                                 while (!_isCrawl2Completed)
                                                 {
                                                     Thread.Sleep(1000);
                                                 }
                                             }
                                             catch (Exception exception)
                                             {
                                                 Console.WriteLine(exception.Message);
                                             }
                                         });

            thread1.Start();
            thread2.Start();

            while (!_isCrawl1Completed || !_isCrawl2Completed)
            {
                Thread.Sleep(1000);
            }
        }

        private void Engine_CrawlRequestCompleted1(CrawlRequest<ArachnodeDAO> sender)
        {
            if (UserDefinedFunctions.ExtractDomain(sender.Discovery.Uri.AbsoluteUri).Value != "cbs.com")
            {
            }
        }

        private void Engine_CrawlCompleted1(Engine<ArachnodeDAO> engine)
        {
            _isCrawl1Completed = true;
        }

        private void Engine_CrawlRequestCompleted2(CrawlRequest<ArachnodeDAO> sender)
        {
            if (UserDefinedFunctions.ExtractDomain(sender.Discovery.Uri.AbsoluteUri).Value != "nbc.com")
            {
            }
        }

        private void Engine_CrawlCompleted2(Engine<ArachnodeDAO> engine)
        {
            _isCrawl1Completed = true;
        }
    }
}