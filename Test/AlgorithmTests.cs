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
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace Arachnode.Test
{
    /// <summary>
    /// 	Summary description for AlgorithmTests
    /// </summary>
    [TestClass]
    public class AlgorithmTests
    {
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
        public void TestIntelligentAutoBalancing()
        {
            //how to crawl on-demand...

            //supposing that each int represents a unique domain... 
            //if there are 10 threads and more than 10 domains that need to be crawled... what is the best way to shuffle the domains to be crawled.
            //running 10 crawlers is fine until an 11th is requested.
            //it is much, much smarter to run 10 threads than can cycle through 11 domains...
            //here's how the algorithm should work...

            List<int> domains = new List<int>();

            Random random = new Random();

            for (int i = 0; i < 1000; i++)
            {
                int domain = random.Next(1, 10);

                //create a sequence of domains (CrawlRequests) to be crawled...
                domains.Add(domain);
            }

            domains.Sort();

            Queue<int> domainsQueue = new Queue<int>();

            List<int> domainsSeen = new List<int>();
            List<int> domainsInOrderProcessed = new List<int>();

            for (int i = domains.Count - 1; i >= 0; i--)
            {
                int domain = domains[i];

                if (!domainsSeen.Contains(domain))
                {
                    domainsSeen.Add(domain);
                    domainsQueue.Enqueue(domain);

                    domainsInOrderProcessed.Add(domain);
                    domains.RemoveAt(i);
                }
                else
                {
                    int nextDomain = domainsQueue.Dequeue();
                    domainsQueue.Enqueue(nextDomain);

                    int domainsOffset = 0;

                    while (domain != nextDomain && i - domainsOffset > 0)
                    {
                        domain = domains[i - domainsOffset];
                        domainsOffset++;
                    }

                    domainsInOrderProcessed.Add(domain);
                    domains.RemoveAt(i - domainsOffset);
                }
            }
        }
    }
}