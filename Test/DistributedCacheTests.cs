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
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Web;
using System.Web.Caching;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.SiteCrawler.Value;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace Arachnode.Test
{
    /// <summary>
    /// 	Summary description for DistributedCacheTests
    /// </summary>
    [TestClass]
    public class DistributedCacheTests
    {
        private static readonly CacheItemRemovedCallback _cacheItemRemovedCallback = CacheItemRemoved;

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
        public void TestCompression()
        {
            long memoryUsageBefore1 = Environment.WorkingSet/1024/1024;

            Stopwatch stopwatch1 = new Stopwatch();
            stopwatch1.Start();

            BinaryFormatter formatter = new BinaryFormatter();

            List<GZipStream> gZipStreams = new List<GZipStream>();

            for (int i = 0; i < 250000; i++)
            {
                Discovery<ArachnodeDAO> discovery = new Discovery<ArachnodeDAO>("http://a.com/");

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (GZipStream compressionStream = new GZipStream(memoryStream, CompressionMode.Compress))
                    {
                        formatter.Serialize(compressionStream, discovery);
                        compressionStream.Flush();

                        gZipStreams.Add(compressionStream);
                    }
                }
            }

            stopwatch1.Start();

            long memoryUsageAfter1 = Environment.WorkingSet/1024/1024;

            gZipStreams = null;

            GC.Collect();

            /**/

            long memoryUsageBefore2 = Environment.WorkingSet/1024/1024;

            Stopwatch stopwatch2 = new Stopwatch();
            stopwatch2.Start();

            List<Discovery<ArachnodeDAO>> discoveries = new List<Discovery<ArachnodeDAO>>();

            for (int i = 0; i < 100000; i++)
            {
                Discovery<ArachnodeDAO> discovery = new Discovery<ArachnodeDAO>("http://a.com/");

                discoveries.Add(discovery);
            }

            stopwatch2.Stop();

            long memoryUsageAfter2 = Environment.WorkingSet/1024/1024;

            gZipStreams = null;

            GC.Collect();

            Debug.Print(stopwatch1.Elapsed.TotalSeconds.ToString());
            Debug.Print((memoryUsageAfter1 - memoryUsageBefore1).ToString());

            Debug.Print(stopwatch2.Elapsed.TotalSeconds.ToString());
            Debug.Print((memoryUsageAfter2 - memoryUsageBefore2).ToString());
        }

        [TestMethod]
        public void TestDiscoveriesInsertAndSelectRate()
        {
            int iterations = 100000;

            ApplicationSettings applicationSettings = new ApplicationSettings();

            ArachnodeDAO arachnodeDAO = new ArachnodeDAO(applicationSettings.ConnectionString);

            /**/

            for (int i = 0; i < 10; i++)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                for (int j = 0; j < iterations; j++)
                {
                    arachnodeDAO.InsertDiscovery(null, "http://" + j + ".com/", 0, 0, false, 1);
                }

                stopwatch.Stop();

                Debug.Print("Discoveries Write:" + stopwatch.Elapsed.TotalSeconds);
                Debug.Print("Discoveries Write:" + (iterations/stopwatch.Elapsed.TotalSeconds));
            }

            /**/

            for (int i = 0; i < 10; i++)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                for (int j = 0; j < iterations; j++)
                {
                    arachnodeDAO.GetDiscovery("http://" + j + ".com/");
                }

                stopwatch.Stop();

                Debug.Print("Discoveries Read:" + stopwatch.Elapsed.TotalSeconds);
                Debug.Print("Discoveries Read:" + (iterations/stopwatch.Elapsed.TotalSeconds));
            }

            /**/
        }

        [TestMethod]
        public void TestCrawlRequestsInsertAndSelectRate()
        {
            int iterations = 100000;

            ApplicationSettings applicationSettings = new ApplicationSettings();

            ArachnodeDAO arachnodeDAO = new ArachnodeDAO(applicationSettings.ConnectionString);

            /**/

            for (int i = 0; i < 10; i++)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                for (int j = 0; j < iterations; j++)
                {
                    arachnodeDAO.InsertCrawlRequest(DateTime.Now, "http://" + j + ".com/", "http://" + j + ".com/", "http://" + j + ".com/", 1, 0, 0, 0, 0, 0);
                }

                stopwatch.Stop();

                Debug.Print("CrawlRequests Write:" + stopwatch.Elapsed.TotalSeconds);
                Debug.Print("CrawlRequests Write:" + (iterations/stopwatch.Elapsed.TotalSeconds));
            }

            /**/

            for (int i = 0; i < 10; i++)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                for (int j = 0; j < iterations; j++)
                {
                    arachnodeDAO.GetCrawlRequests(1000, false, false, false, false, false, false, false, false, false);
                }

                stopwatch.Stop();

                Debug.Print("CrawlRequests Read:" + stopwatch.Elapsed.TotalSeconds);
                Debug.Print("CrawlRequests Read:" + (iterations/stopwatch.Elapsed.TotalSeconds));
            }

            /**/
        }

        [TestMethod]
        public void TestHttpRuntimeCacheSlidingWindow()
        {
            HttpRuntime.Cache.Add("test", "test", null, DateTime.MaxValue, TimeSpan.FromSeconds(10), CacheItemPriority.Normal, _cacheItemRemovedCallback);

            //ensuring that a Cache Get extends the sliding window expiration.
            while (true)
            {
                Thread.Sleep(1000);

                //HttpRuntime.Cache.Get("test");
            }
        }

        private static void CacheItemRemoved(string cacheKey, object o, CacheItemRemovedReason cacheItemRemovedReason)
        {
        }
    }
}