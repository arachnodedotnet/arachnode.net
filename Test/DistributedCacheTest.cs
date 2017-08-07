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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Web.Caching;
using Arachnode.Cache;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace Arachnode.Test
{
    ///<summary>
    ///	This is a test class for DistributedCacheTest and is intended
    ///	to contain all DistributedCacheTest Unit Tests
    ///</summary>
    [TestClass]
    public class DistributedCacheTest
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
        ///	A test for WriteCacheItemsWhenReadFromCachePeers
        ///</summary>
        [TestMethod]
        [DeploymentItem("Arachnode.Cache.dll")]
        public void WriteCacheItemsWhenReadFromCachePeersTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            DistributedCache_Accessor target = new DistributedCache_Accessor(param0); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.WriteCacheItemsWhenReadFromCachePeers = expected;
            actual = target.WriteCacheItemsWhenReadFromCachePeers;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        ///<summary>
        ///	A test for UseSlidingWindowCache
        ///</summary>
        [TestMethod]
        [DeploymentItem("Arachnode.Cache.dll")]
        public void UseSlidingWindowCacheTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            DistributedCache_Accessor target = new DistributedCache_Accessor(param0); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.UseSlidingWindowCache = expected;
            actual = target.UseSlidingWindowCache;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        ///<summary>
        ///	A test for WriteObjectToDisk
        ///</summary>
        [TestMethod]
        [DeploymentItem("Arachnode.Cache.dll")]
        public void WriteObjectToDiskTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            DistributedCache_Accessor target = new DistributedCache_Accessor(param0); // TODO: Initialize to an appropriate value
            string cacheKey = string.Empty; // TODO: Initialize to an appropriate value
            object cacheObject = null; // TODO: Initialize to an appropriate value
            string fileName = string.Empty; // TODO: Initialize to an appropriate value
            target.WriteObjectToDisk(cacheKey, cacheObject, fileName);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        ///<summary>
        ///	A test for Write
        ///</summary>
        [TestMethod]
        [DeploymentItem("Arachnode.Cache.dll")]
        public void WriteTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            DistributedCache_Accessor target = new DistributedCache_Accessor(param0); // TODO: Initialize to an appropriate value
            string cacheKey = string.Empty; // TODO: Initialize to an appropriate value
            object cacheObject = null; // TODO: Initialize to an appropriate value
            SHA1 sha1 = null; // TODO: Initialize to an appropriate value
            target.Write(cacheKey, cacheObject, sha1, true);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        ///<summary>
        ///	A test for ReadObjectFromDisk
        ///</summary>
        [TestMethod]
        [DeploymentItem("Arachnode.Cache.dll")]
        public void ReadObjectFromDiskTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            DistributedCache_Accessor target = new DistributedCache_Accessor(param0); // TODO: Initialize to an appropriate value
            string cacheKey = string.Empty; // TODO: Initialize to an appropriate value
            string fileName = string.Empty; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = target.ReadObjectFromDisk(cacheKey, fileName);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        ///<summary>
        ///	A test for ReadObjectFromCachePeerDisk
        ///</summary>
        [TestMethod]
        [DeploymentItem("Arachnode.Cache.dll")]
        public void ReadObjectFromCachePeerDiskTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            DistributedCache_Accessor target = new DistributedCache_Accessor(param0); // TODO: Initialize to an appropriate value
            string cacheKey = string.Empty; // TODO: Initialize to an appropriate value
            byte[] sha1Hash = null; // TODO: Initialize to an appropriate value
            string fileName = string.Empty; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = target.ReadObjectFromCachePeerDisk(cacheKey, sha1Hash, fileName);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        ///<summary>
        ///	A test for Read
        ///</summary>
        [TestMethod]
        [DeploymentItem("Arachnode.Cache.dll")]
        public void ReadTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            DistributedCache_Accessor target = new DistributedCache_Accessor(param0); // TODO: Initialize to an appropriate value
            string cacheKey = string.Empty; // TODO: Initialize to an appropriate value
            SHA1 sha1 = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = target.Read(cacheKey, sha1);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        ///<summary>
        ///	A test for GetSHA1Hash
        ///</summary>
        [TestMethod]
        [DeploymentItem("Arachnode.Cache.dll")]
        public void GetSHA1HashTest()
        {
            string cacheKey = string.Empty; // TODO: Initialize to an appropriate value
            SHA1 sha1 = null; // TODO: Initialize to an appropriate value
            byte[] expected = null; // TODO: Initialize to an appropriate value
            byte[] actual;
            actual = DistributedCache_Accessor.GetSHA1Hash(cacheKey, sha1);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        ///<summary>
        ///	A test for GetFileName
        ///</summary>
        [TestMethod]
        [DeploymentItem("Arachnode.Cache.dll")]
        public void GetFileNameTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            DistributedCache_Accessor target = new DistributedCache_Accessor(param0); // TODO: Initialize to an appropriate value
            string cacheDirectoryRoot = string.Empty; // TODO: Initialize to an appropriate value
            string cacheKey = string.Empty; // TODO: Initialize to an appropriate value
            byte[] sha1Hash = null; // TODO: Initialize to an appropriate value
            bool createKey = false; // TODO: Initialize to an appropriate value
            bool createDirectory = false; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.GetFileName(cacheDirectoryRoot, cacheKey, sha1Hash, createKey, createDirectory);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        ///<summary>
        ///	A test for DistributedCache Constructor
        ///</summary>
        [TestMethod]
        [DeploymentItem("Arachnode.Cache.dll")]
        public void DistributedCacheConstructorTest1()
        {
            string diskCacheDirectoryRoot = string.Empty; // TODO: Initialize to an appropriate value
            DistributedCache_Accessor target = new DistributedCache_Accessor(diskCacheDirectoryRoot);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        ///<summary>
        ///	A test for DistributedCache Constructor
        ///</summary>
        [TestMethod]
        [DeploymentItem("Arachnode.Cache.dll")]
        public void DistributedCacheConstructorTest()
        {
            string diskCacheDirectoryRoot = string.Empty; // TODO: Initialize to an appropriate value
            string[] diskCachePeerDirectoryRoots = null; // TODO: Initialize to an appropriate value
            DistributedCache_Accessor target = new DistributedCache_Accessor(diskCacheDirectoryRoot, diskCachePeerDirectoryRoots);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        [TestMethod]
        [DeploymentItem("Arachnode.Cache.dll")]
        public void TestFullTextDistributedCacheStorageBasic()
        {
            string diskCacheDirectoryRoot = "C:\\DeleteMe"; // TODO: Initialize to an appropriate value
            DistributedCache_Accessor target = new DistributedCache_Accessor(diskCacheDirectoryRoot);

            SHA1 sha1 = SHA1.Create();

            target.Write("the", "some document:1", sha1, true);
            target.Write("quick", "some document:1", sha1, true);
            target.Write("brown", "some document:1", sha1, true);
            target.Write("fox", "some document:1", sha1, true);
            target.Write("jumped", "some document:1", sha1, true);
            target.Write("over", "some document:1", sha1, true);
            target.Write("the", "some document:1", sha1, true);
            target.Write("lazy", "some document:1", sha1, true);
            target.Write("dog", "some document:1", sha1, true);
        }

        [TestMethod]
        [DeploymentItem("Arachnode.Cache.dll")]
        public void TestFullTextDistributedCacheStorageStress()
        {
            string directoryRoot1 = "c:\\DeleteMe\\WordsPerAbsoluteUri";
            string directoryRoot2 = "c:\\DeleteMe\\InvertedIndexPerWord";

            try
            {
                //Directories.DeleteFilesInDirectory(directoryRoot1 + "\\arachnode.cache");
                //Directories.DeleteFilesInDirectory(directoryRoot2 + "\\arachnode.cache");
            }
            catch (Exception)
            {
                throw;
            }

            DistributedCache_Accessor wordsPerAbsoluteUriDistributedCache = new DistributedCache_Accessor(directoryRoot1);
            wordsPerAbsoluteUriDistributedCache.UseSlidingWindowCache = false;

            DistributedCache_Accessor invertedIndexPerWordDistributedCache = new DistributedCache_Accessor(directoryRoot2);
            invertedIndexPerWordDistributedCache.UseSlidingWindowCache = true;
            invertedIndexPerWordDistributedCache.CacheItemPriority = CacheItemPriority.Normal;
            invertedIndexPerWordDistributedCache.CacheItemSlidingExpiration = TimeSpan.FromSeconds(10);

            Stopwatch perDocumentStopwatch = new Stopwatch();
            Stopwatch corpusStopwatch = new Stopwatch();
            SHA1 sha1 = SHA1.Create();

            int iterations = 10;
            int numberOfWebPages = 10000;

            double totalNumberOfWebPages = 0;

            WebClient webClient = new WebClient();

            corpusStopwatch.Reset();
            corpusStopwatch.Start();

            for (int j = 1; j <= iterations; j++)
            {
                for (int i = 1; i <= numberOfWebPages; i++)
                {
                    totalNumberOfWebPages++;

                    string absoluteUri = "http://localhost:56830/Test/" + i + ".htm";

                    string downloadedString = webClient.DownloadString(absoluteUri);

                    IEnumerator enumerator = UserDefinedFunctions.ExtractWords(downloadedString, true, true).GetEnumerator();

                    perDocumentStopwatch.Reset();
                    perDocumentStopwatch.Start();

                    HashSet<string> wordsPerAbsoluteUri = new HashSet<string>();

                    bool writeWordsPerAbsoluteUri = false;

                    object o2 = wordsPerAbsoluteUriDistributedCache.Read("WORDS_" + absoluteUri + "_" + j, sha1);

                    if (true || o2 == null)
                    {
                        while (enumerator.MoveNext())
                        {
                            wordsPerAbsoluteUri.Add(enumerator.Current.ToString());

                            /**/

                            string directory = invertedIndexPerWordDistributedCache.GenerateFullTextUniqueDirectory(enumerator.Current.ToString(), false, sha1);

                            string directory2 = directory + "\\" + invertedIndexPerWordDistributedCache.GetFileNameWithoutDirectory(absoluteUri + "_" + j, sha1);

                            if (!Directory.Exists(directory2))
                            {
                                Directory.CreateDirectory(directory2);
                            }

                            //HashSet<string> invertedIndexPerWord;

                            //object o = invertedIndexPerWordDistributedCache.Read(enumerator.Current.ToString(), sha1);

                            //if (o == null)
                            //{
                            //    invertedIndexPerWord = new HashSet<string>();

                            //    invertedIndexPerWordDistributedCache.Write(enumerator.Current.ToString(), invertedIndexPerWord, sha1, false);
                            //}
                            //else
                            //{
                            //    invertedIndexPerWord = (HashSet<string>) o;
                            //}

                            //if (!invertedIndexPerWord.Contains(absoluteUri + "_" + j))
                            //{
                            //    invertedIndexPerWord.Add(absoluteUri + "_" + j);

                            //    writeWordsPerAbsoluteUri = true;
                            //}
                            //Debug.Print(invertedIndexPerWord.Count.ToString());
                        }

                        if (true || writeWordsPerAbsoluteUri)
                        {
                            wordsPerAbsoluteUriDistributedCache.Write("WORDS_" + absoluteUri + "_" + j, wordsPerAbsoluteUri, sha1, true);

                            invertedIndexPerWordDistributedCache.CacheItemSlidingExpiration = TimeSpan.FromSeconds(perDocumentStopwatch.Elapsed.TotalSeconds + 2);
                        }
                    }

                    perDocumentStopwatch.Stop();

                    Debug.Print((((j - 1)*numberOfWebPages) + i).ToString());
                    Debug.Print(perDocumentStopwatch.Elapsed.ToString());
                    Debug.Print((corpusStopwatch.Elapsed.TotalSeconds/totalNumberOfWebPages).ToString());
                }

                Debug.Print(j.ToString());
                Debug.Print(corpusStopwatch.Elapsed.ToString());
                Debug.Print((corpusStopwatch.Elapsed.TotalSeconds/totalNumberOfWebPages).ToString());
            }

            corpusStopwatch.Stop();
            Debug.Print(corpusStopwatch.Elapsed.ToString());
            Debug.Print((corpusStopwatch.Elapsed.TotalSeconds/totalNumberOfWebPages).ToString());
        }
    }
}

//using System;
//using System.Diagnostics;
//using System.Security.Cryptography;
//using System.Threading;

//namespace ConsoleApplication1
//{
//    [Serializable]
//    internal class CacheObject
//    {
//        public string Value { get; set; }
//    }

//    internal class Program
//    {
//        private static readonly DiskBackedCache diskBackedCache = new DiskBackedCache("D:\\", new[] {"F:\\", "H:\\"});
//        //private static readonly DiskBackedCache diskBackedCache = new DiskBackedCache("D:\\");
//        private static double iterations = 1000000;
//        private static int threadCount = 1;

//        private static void Main(string[] args)
//        {
//            diskBackedCache.WriteCacheItemsWhenReadFromCachePeers = false;
//            diskBackedCache.UseSlidingWindowCache = false;

//            var thread1 = new Thread(ThreadMethodWrite);
//            var thread2 = new Thread(ThreadMethodRead);
//            var thread3 = new Thread(ThreadMethodWrite);
//            var thread4 = new Thread(ThreadMethodRead);

//            var thread11 = new Thread(ThreadMethodWrite);
//            var thread21 = new Thread(ThreadMethodRead);
//            var thread31 = new Thread(ThreadMethodWrite);
//            var thread41 = new Thread(ThreadMethodRead);

//            var thread111 = new Thread(ThreadMethodWrite);
//            var thread211 = new Thread(ThreadMethodRead);
//            var thread311 = new Thread(ThreadMethodWrite);
//            var thread411 = new Thread(ThreadMethodRead);

//            thread1.Start(1);
//            //thread2.Start(3);
//            //thread3.Start(1);
//            //thread4.Start(3);

//            //thread11.Start(2);
//            //thread21.Start(3);
//            //thread31.Start(2);
//            //thread41.Start(3);

//            //thread111.Start(3);
//            //thread211.Start(3);
//            //thread311.Start(3);
//            //thread411.Start(3);

//            Console.ReadLine();
//        }

//        private static void ThreadMethodRead(object o)
//        {
//            SHA1 sha11 = SHA1.Create();

//            while (true)
//            {
//                var stopwatch1 = new Stopwatch();
//                stopwatch1.Start();

//                for (double i = 0; i < double.Parse(o.ToString()) * iterations; i++)
//                {
//                    var cacheObject = (CacheObject)diskBackedCache.Read(i.ToString(), sha11);

//                    if (cacheObject != null)
//                    {
//                        Debug.Assert(i.ToString() == cacheObject.Value);
//                    }

//                    //Console.WriteLine(i);
//                }

//                stopwatch1.Stop();

//                //Debug.Print("Read:" + stopwatch1.Elapsed.TotalSeconds);
//                Debug.Print("Read:" + (iterations/stopwatch1.Elapsed.TotalSeconds)*threadCount);
//            }
//        }

//        private static void ThreadMethodWrite(object o)
//        {
//            SHA1 sha11 = SHA1.Create();

//            while (true)
//            {
//                var stopwatch1 = new Stopwatch();
//                stopwatch1.Start();

//                for (double i = (double.Parse(o.ToString()) - 1)*iterations; i < double.Parse(o.ToString())*iterations; i++)
//                {
//                    var cacheObject = new CacheObject();

//                    cacheObject.Value = i.ToString();

//                    diskBackedCache.Write(cacheObject.Value, cacheObject, sha11);

//                    //Console.WriteLine(i);
//                }

//                stopwatch1.Stop();

//                //Debug.Print("Write:" + stopwatch1.Elapsed.TotalSeconds);
//                Debug.Print("Write:" + (iterations/stopwatch1.Elapsed.TotalSeconds)*threadCount);
//            }
//        }
//    }
//}