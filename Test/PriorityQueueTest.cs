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

using Arachnode.DataAccess;
using Arachnode.Renderer.Value.Enums;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.Enums;
using Arachnode.Structures;
using Arachnode.Structures.Value;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace Test
{
    ///<summary>
    ///	This is a test class for PriorityQueueTest and is intended
    ///	to contain all PriorityQueueTest Unit Tests
    ///</summary>
    [TestClass]
    public class PriorityQueueTest
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
        ///	A test for Count
        ///</summary>
        public void CountTestHelper<TValue>()
        {
            PriorityQueue<TValue> target = new PriorityQueue<TValue>();
            Assert.AreEqual(0, target.Count);
        }

        [TestMethod]
        public void CountTest()
        {
            CountTestHelper<GenericParameterHelper>();
        }

        ///<summary>
        ///	A test for GetEnumerator
        ///</summary>
        public void GetEnumeratorTestHelper<TValue>()
        {
            PriorityQueue<CrawlRequest<ArachnodeDAO>> target = new PriorityQueue<CrawlRequest<ArachnodeDAO>>();

            int numberOfPriorityQueueItems = 1000;

            for (int i = 1; i <= numberOfPriorityQueueItems; i++)
            {
                target.Enqueue(new CrawlRequest<ArachnodeDAO>(new Discovery<ArachnodeDAO>("http://" + i + ".com"), 1, UriClassificationType.Domain, UriClassificationType.Domain, i, RenderType.None, RenderType.None), i);
            }

            int j = numberOfPriorityQueueItems;

            foreach (PriorityQueueItem<CrawlRequest<ArachnodeDAO>> priorityQueueItem in target)
            {
                Assert.AreEqual(j, priorityQueueItem.Priority);
                Assert.AreEqual(j, priorityQueueItem.Value.Priority);

                Assert.AreEqual(target.Count, numberOfPriorityQueueItems);

                j--;
            }
        }

        [TestMethod]
        public void GetEnumeratorTest()
        {
            GetEnumeratorTestHelper<GenericParameterHelper>();
        }

        ///<summary>
        ///	A test for Enqueue
        ///</summary>
        public void EnqueueTestHelper<TValue>()
        {
            PriorityQueue<CrawlRequest<ArachnodeDAO>> target = new PriorityQueue<CrawlRequest<ArachnodeDAO>>();

            int numberOfPriorityQueueItems = 1000;

            for (int i = 0; i < numberOfPriorityQueueItems; i++)
            {
                target.Enqueue(new CrawlRequest<ArachnodeDAO>(new Discovery<ArachnodeDAO>("http://" + i + ".com"), 1, UriClassificationType.Domain, UriClassificationType.Domain, i, RenderType.None, RenderType.None), i);
            }

            Assert.AreEqual(numberOfPriorityQueueItems, target.Count);
        }

        [TestMethod]
        public void EnqueueTest()
        {
            EnqueueTestHelper<GenericParameterHelper>();
        }

        ///<summary>
        ///	A test for Dequeue
        ///</summary>
        public void DequeueTestHelper<TValue>()
        {
            PriorityQueue<CrawlRequest<ArachnodeDAO>> target = new PriorityQueue<CrawlRequest<ArachnodeDAO>>();

            int numberOfPriorityQueueItems = 1000;

            //test with increasing priorities...
            for (int i = 0; i < numberOfPriorityQueueItems; i++)
            {
                target.Enqueue(new CrawlRequest<ArachnodeDAO>(new Discovery<ArachnodeDAO>("http://" + i + ".com"), 1, UriClassificationType.Domain, UriClassificationType.Domain, i, RenderType.None, RenderType.None), i);
            }

            Assert.AreEqual(numberOfPriorityQueueItems, target.Count);

            int j = numberOfPriorityQueueItems - 1;

            while (target.Count != 0)
            {
                CrawlRequest<ArachnodeDAO> crawlRequest = target.Dequeue();

                Assert.AreEqual(j, crawlRequest.Priority);

                j--;
            }

            Assert.AreEqual(0, target.Count);

            /**/

            //test with fixed priorities...
            for (int i = 0; i < numberOfPriorityQueueItems; i++)
            {
                target.Enqueue(new CrawlRequest<ArachnodeDAO>(new Discovery<ArachnodeDAO>("http://" + i + ".com"), 1, UriClassificationType.Domain, UriClassificationType.Domain, i, RenderType.None, RenderType.None), 1);
            }

            Assert.AreEqual(numberOfPriorityQueueItems, target.Count);

            j = 0;

            while (target.Count != 0)
            {
                CrawlRequest<ArachnodeDAO> crawlRequest = target.Dequeue();

                Assert.AreEqual("http://" + j + ".com/", crawlRequest.Discovery.Uri.AbsoluteUri);

                j++;
            }

            Assert.AreEqual(0, target.Count);
        }

        [TestMethod]
        public void DequeueTest()
        {
            DequeueTestHelper<GenericParameterHelper>();
        }

        ///<summary>
        ///	A test for PriorityQueue`1 Constructor
        ///</summary>
        public void PriorityQueueConstructorTestHelper<TValue>()
        {
            PriorityQueue<TValue> target = new PriorityQueue<TValue>();
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void PriorityQueueConstructorTest()
        {
            PriorityQueueConstructorTestHelper<GenericParameterHelper>();
        }
    }
}