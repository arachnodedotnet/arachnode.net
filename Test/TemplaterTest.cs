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

using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.Plugins.CrawlActions;
using Arachnode.Renderer.Value.Enums;
using Arachnode.SiteCrawler;
using Arachnode.SiteCrawler.Components;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace Test
{
    ///<summary>
    ///	This is a test class for TemplaterTest and is intended
    ///	to contain all TemplaterTest Unit Tests
    ///</summary>
    [TestClass]
    public class TemplaterTest
    {
        ///<summary>
        ///	Gets or sets the test context which provides
        ///	information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

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

        ///<summary>
        ///	A test for PerformAction
        ///</summary>
        [TestMethod]
        public void PerformActionTest()
        {
            ApplicationSettings applicationSettings = new ApplicationSettings();
            WebSettings webSettings = new WebSettings();

            ArachnodeDAO arachnodeDAO = new ArachnodeDAO(applicationSettings.ConnectionString, applicationSettings, webSettings, true, true);

            Crawler<ArachnodeDAO> crawler = new Crawler<ArachnodeDAO>(applicationSettings, webSettings, CrawlMode.BreadthFirstByPriority, false);

            CrawlRequest<ArachnodeDAO> crawlRequest = new CrawlRequest<ArachnodeDAO>(new Discovery<ArachnodeDAO>("http://trycatchfail.com/blog/post/2008/11/12/Deep-web-crawling-with-NET-Getting-Started.aspx"), 1, UriClassificationType.Host, UriClassificationType.Host, 1, RenderType.None, RenderType.None);

            Crawl<ArachnodeDAO> crawl = new Crawl<ArachnodeDAO>(applicationSettings, webSettings, crawler, crawler.ActionManager, crawler.ConsoleManager, crawler.CookieManager, crawler.CrawlRequestManager, crawler.DataTypeManager, crawler.DiscoveryManager, crawler.EncodingManager, crawler.HtmlManager, crawler.PolitenessManager, crawler.ProxyManager, crawler.RuleManager, true);

            applicationSettings.MaximumNumberOfCrawlThreads = 0;

            UserDefinedFunctions.ConnectionString = "Data Source=.;Initial Catalog=arachnode.net;Integrated Security=True;Connection Timeout=3600;";
            crawler.Engine.Start();

            crawl.BeginCrawl(crawlRequest, false, false, false);

            Templater<ArachnodeDAO> target = new Templater<ArachnodeDAO>(applicationSettings, webSettings);

            target.PerformAction(crawlRequest, arachnodeDAO);
        }

        ///<summary>
        ///	A test for Templater Constructor
        ///</summary>
        [TestMethod]
        public void TemplaterConstructorTest()
        {
            ApplicationSettings applicationSettings = new ApplicationSettings();
            WebSettings webSettings = new WebSettings();

            Templater<ArachnodeDAO> target = new Templater<ArachnodeDAO>(applicationSettings, webSettings);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}