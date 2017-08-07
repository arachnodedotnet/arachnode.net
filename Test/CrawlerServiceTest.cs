using System.Net;
using Arachnode.Test.CrawlerServiceReference;
using Arachnode.Web.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using Arachnode.Configuration;
using Arachnode.SiteCrawler.Value.Enums;
using Arachnode.Renderer.Value.Enums;
using Arachnode.DataAccess;
using Arachnode.SiteCrawler.Value.AbstractClasses;
using System.Collections.Generic;
using ApplicationSettings = Arachnode.Configuration.ApplicationSettings;
using CrawlMode = Arachnode.SiteCrawler.Value.Enums.CrawlMode;
using WebSettings = Arachnode.Configuration.WebSettings;

namespace Arachnode.Test
{
    
    
    /// <summary>
    ///This is a test class for CrawlerServiceTest and is intended
    ///to contain all CrawlerServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CrawlerServiceTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [TestMethod()]
        public void TestCrawlerService()
        {
            CrawlerServiceReference.CrawlerService crawlerService = new CrawlerServiceReference.CrawlerService();

            crawlerService.CookieContainer = new CookieContainer();

            crawlerService.InitializeConfiguration();

            CrawlerServiceReference.ApplicationSettings applicationSettings = crawlerService.GetApplicationSettings();
            CrawlerServiceReference.WebSettings webSettings = crawlerService.GetWebSettings();

            applicationSettings.MaximumNumberOfAutoRedirects = 50;

            crawlerService.Constructor(applicationSettings, webSettings, CrawlerServiceReference.CrawlMode.BreadthFirstByPriority, false);

            crawlerService.Reset();
            crawlerService.Crawl("http://arachnode.net", 1, CrawlerServiceReference.UriClassificationType.Domain, CrawlerServiceReference.UriClassificationType.Domain, 1, CrawlerServiceReference.RenderType.None, CrawlerServiceReference.RenderType.None);

            crawlerService.StartEngineAsync(null);
        }
    }
}
