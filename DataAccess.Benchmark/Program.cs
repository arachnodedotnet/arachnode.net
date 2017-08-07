using System;
using System.Diagnostics;
using System.Threading;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.Performance;

namespace Arachnode.DataAccess.Benchmark
{
    internal class Program
    {
        private static readonly Arachnode.Configuration.ApplicationSettings _applicationSettings = new Arachnode.Configuration.ApplicationSettings();
        private static readonly Arachnode.Configuration.WebSettings _webSettings = new Arachnode.Configuration.WebSettings();

        private static Stopwatch _stopwatch = new Stopwatch();

        private static int _numberOfEntitiesToCreate = 100000;
        private static int _numberOfThreads = 10;

        private static void Main(string[] args)
        {
            Console.WriteLine("Press any key (three times) to reset the arachnode.net database and run the benchmark...");

            Console.ReadLine();
            Console.ReadLine();
            Console.ReadLine();

            IArachnodeDAO arachnodeDAO = new ArachnodeDAO(_applicationSettings.ConnectionString, _applicationSettings, _webSettings, true, false);
            //running with '1' as the input parameter removes all user data.
            //running this stored procedure alleviates many installation issues.
            arachnodeDAO.ExecuteSql("EXEC [dbo].[arachnode_usp_arachnode.net_RESET_DATABASE]");

            BenchmarkCrawlRequests();

            BenchmarkDisallowedAbsoluteUris();

            BenchmarkDiscoveries();

            BenchmarkEmailAddresss();

            BenchmarkExceptions();

            BenchmarkFiles();            

            BenchmarkHyperLinks();

            BenchmarkImages();

            BenchmarkWebPages();

            Console.WriteLine("DataAccess.Benchmark: " + _stopwatch.Elapsed);
            Console.ReadLine();
        }

        private static void BenchmarkCrawlRequests()
        {
            _stopwatch.Start();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            int crawlRequestNumber = 0;
            int totalNumberOfCrawlRequests = _numberOfEntitiesToCreate;

            for (int t = 1; t <= _numberOfThreads; t++)
            {
                var thread = new Thread(delegate(object o)
                                            {
                                                IArachnodeDAO arachnodeDAO2 = new ArachnodeDAO(_applicationSettings.ConnectionString, _applicationSettings, _webSettings, false, false);
                                                arachnodeDAO2.OpenCommandConnections();

                                                while (crawlRequestNumber != totalNumberOfCrawlRequests)
                                                {
                                                    int threadCrawlRequestNumber = Interlocked.Increment(ref crawlRequestNumber);

                                                    arachnodeDAO2.InsertCrawlRequest(DateTime.Now, "http://arachnode.net/" + threadCrawlRequestNumber + ".htm", "http://arachnode.net/" + threadCrawlRequestNumber + ".htm", "http://arachnode.net/" + threadCrawlRequestNumber + ".htm", 1, 0, 0, 1, 0, 0);

                                                    Counters.GetInstance().CrawlRequestInserted();

                                                    if (threadCrawlRequestNumber%1000 == 0)
                                                    {
                                                        Console.WriteLine("BenchmarkCrawlRequests: " + threadCrawlRequestNumber);
                                                    }
                                                }

                                                arachnodeDAO2.CloseCommandConnections();
                                            });
                thread.Start(t);
            }

            while (crawlRequestNumber != totalNumberOfCrawlRequests)
            {
                Thread.Sleep(1000);
            }

            _stopwatch.Stop();
            stopwatch.Stop();

            Console.WriteLine(stopwatch.Elapsed);
            Console.ReadLine();
        }

        private static void BenchmarkDisallowedAbsoluteUris()
        {
            _stopwatch.Start();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            int disallowedAbsoluteUriNumber = 0;
            int totalNumberOfDisallowedAbsoluteUris = _numberOfEntitiesToCreate;

            for (int t = 1; t <= _numberOfThreads; t++)
            {
                var thread = new Thread(delegate(object o)
                {
                    IArachnodeDAO arachnodeDAO2 = new ArachnodeDAO(_applicationSettings.ConnectionString, _applicationSettings, _webSettings, false, false);

                    arachnodeDAO2.OpenCommandConnections();

                    while (disallowedAbsoluteUriNumber != totalNumberOfDisallowedAbsoluteUris)
                    {
                        int threadDisallowedAbsoluteUriNumber = Interlocked.Increment(ref disallowedAbsoluteUriNumber);

                        arachnodeDAO2.InsertDisallowedAbsoluteUri(1, 1, "http://arachnode.net/" + (threadDisallowedAbsoluteUriNumber) + ".htm", "http://arachnode.net/" + (threadDisallowedAbsoluteUriNumber) + ".htm", "DataAccess.Benchmark", false);

                        Counters.GetInstance().DisallowedAbsoluteUriInserted();

                        if (threadDisallowedAbsoluteUriNumber % 1000 == 0)
                        {
                            Console.WriteLine("BenchmarkDisallowedAbsoluteUris: " + threadDisallowedAbsoluteUriNumber);
                        }
                    }

                    arachnodeDAO2.CloseCommandConnections();
                });
                thread.Start(t);
            }

            while (disallowedAbsoluteUriNumber != totalNumberOfDisallowedAbsoluteUris)
            {
                Thread.Sleep(1000);
            }

            _stopwatch.Stop();
            stopwatch.Stop();

            Console.WriteLine(stopwatch.Elapsed);
            Console.ReadLine();
        }

        private static void BenchmarkDiscoveries()
        {
            _stopwatch.Start();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            int discoveryNumber = 0;
            int totalNumberOfDiscoveries = _numberOfEntitiesToCreate;

            for (int t = 1; t <= _numberOfThreads; t++)
            {
                var thread = new Thread(delegate(object o)
                                            {
                                                IArachnodeDAO arachnodeDAO2 = new ArachnodeDAO(_applicationSettings.ConnectionString, _applicationSettings, _webSettings, false, false);

                                                arachnodeDAO2.OpenCommandConnections();

                                                while (discoveryNumber != totalNumberOfDiscoveries)
                                                {
                                                    int threadDiscoveryNumber = Interlocked.Increment(ref discoveryNumber);

                                                    arachnodeDAO2.InsertDiscovery(threadDiscoveryNumber, "http://arachnode.net/" + threadDiscoveryNumber + ".htm", 1, 1, false, 1);

                                                    Counters.GetInstance().DiscoveryInserted();

                                                    if (threadDiscoveryNumber%1000 == 0)
                                                    {
                                                        Console.WriteLine("BenchmarkDiscoveries: " + threadDiscoveryNumber);
                                                    }
                                                }

                                                arachnodeDAO2.CloseCommandConnections();
                                            });
                thread.Start(t);
            }

            while (discoveryNumber != totalNumberOfDiscoveries)
            {
                Thread.Sleep(1000);
            }

            _stopwatch.Stop();
            stopwatch.Stop();

            Console.WriteLine(stopwatch.Elapsed);
            Console.ReadLine();
        }

        private static void BenchmarkEmailAddresss()
        {
            _stopwatch.Start();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            int emailAddressNumber = 0;
            int totalNumberOfEmailAddresss = _numberOfEntitiesToCreate;

            for (int t = 1; t <= _numberOfThreads; t++)
            {
                var thread = new Thread(delegate(object o)
                {
                    IArachnodeDAO arachnodeDAO2 = new ArachnodeDAO(_applicationSettings.ConnectionString, _applicationSettings, _webSettings, false, false);

                    arachnodeDAO2.OpenCommandConnections();

                    while (emailAddressNumber != totalNumberOfEmailAddresss)
                    {
                        int threadEmailAddressNumber = Interlocked.Increment(ref emailAddressNumber);

                        arachnodeDAO2.InsertEmailAddress("http://arachnode.net/" + (threadEmailAddressNumber) + ".htm", (threadEmailAddressNumber) + "@arachnode.net", false);

                        Counters.GetInstance().EmailAddressInserted();

                        if (threadEmailAddressNumber % 1000 == 0)
                        {
                            Console.WriteLine("BenchmarkEmailAddresss: " + threadEmailAddressNumber);
                        }
                    }

                    arachnodeDAO2.CloseCommandConnections();
                });
                thread.Start(t);
            }

            while (emailAddressNumber != totalNumberOfEmailAddresss)
            {
                Thread.Sleep(1000);
            }

            _stopwatch.Stop();
            stopwatch.Stop();

            Console.WriteLine(stopwatch.Elapsed);
            Console.ReadLine();
        }

        private static void BenchmarkExceptions()
        {
            _stopwatch.Start();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            int exceptionNumber = 0;
            int totalNumberOfExceptions = _numberOfEntitiesToCreate;

            for (int t = 1; t <= _numberOfThreads; t++)
            {
                var thread = new Thread(delegate(object o)
                {
                    IArachnodeDAO arachnodeDAO2 = new ArachnodeDAO(_applicationSettings.ConnectionString, _applicationSettings, _webSettings, false, false);
                    arachnodeDAO2.ApplicationSettings.ClassifyAbsoluteUris = false;

                    arachnodeDAO2.OpenCommandConnections();

                    while (exceptionNumber != totalNumberOfExceptions)
                    {
                        int threadExceptionNumber = Interlocked.Increment(ref exceptionNumber);

                        arachnodeDAO2.InsertException("http://arachnode.net/" + (threadExceptionNumber) + ".htm", "http://arachnode.net/" + (threadExceptionNumber) + ".htm", null, "Message", "Source", "StackTrace", false);

                        Counters.GetInstance().ExceptionInserted();

                        if (threadExceptionNumber % 1000 == 0)
                        {
                            Console.WriteLine("BenchmarkExceptions: " + threadExceptionNumber);
                        }
                    }

                    arachnodeDAO2.CloseCommandConnections();
                });
                thread.Start(t);
            }

            while (exceptionNumber != totalNumberOfExceptions)
            {
                Thread.Sleep(1000);
            }

            _stopwatch.Stop();
            stopwatch.Stop();

            Console.WriteLine(stopwatch.Elapsed);
            Console.ReadLine();
        }

        private static void BenchmarkFiles()
        {
            _stopwatch.Start();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            int fileNumber = 0;
            int totalNumberOfFiles = _numberOfEntitiesToCreate;

            for (int t = 1; t <= _numberOfThreads; t++)
            {
                var thread = new Thread(delegate(object o)
                {
                    IArachnodeDAO arachnodeDAO2 = new ArachnodeDAO(_applicationSettings.ConnectionString, _applicationSettings, _webSettings, false, false);

                    arachnodeDAO2.OpenCommandConnections();

                    while (fileNumber != totalNumberOfFiles)
                    {
                        int threadFileNumber = Interlocked.Increment(ref fileNumber);

                        arachnodeDAO2.InsertFile("http://arachnode.net/" + (threadFileNumber) + ".htm", "http://arachnode.net/" + (threadFileNumber) + ".htm", null, new byte[0], ".txt", false);

                        Counters.GetInstance().FileInserted();

                        if (threadFileNumber % 1000 == 0)
                        {
                            Console.WriteLine("BenchmarkFiles: " + threadFileNumber);
                        }
                    }

                    arachnodeDAO2.CloseCommandConnections();
                });
                thread.Start(t);
            }

            while (fileNumber != totalNumberOfFiles)
            {
                Thread.Sleep(1000);
            }

            _stopwatch.Stop();
            stopwatch.Stop();

            Console.WriteLine(stopwatch.Elapsed);
            Console.ReadLine();
        }

        private static void BenchmarkHyperLinks()
        {
            _stopwatch.Start();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            int hyperLinkNumber = 0;
            int totalNumberOfHyperLinks = _numberOfEntitiesToCreate;

            for (int t = 1; t <= _numberOfThreads; t++)
            {
                var thread = new Thread(delegate(object o)
                {
                    IArachnodeDAO arachnodeDAO2 = new ArachnodeDAO(_applicationSettings.ConnectionString, _applicationSettings, _webSettings, false, false);

                    arachnodeDAO2.OpenCommandConnections();

                    while (hyperLinkNumber != totalNumberOfHyperLinks)
                    {
                        int threadHyperLinkNumber = Interlocked.Increment(ref hyperLinkNumber);

                        arachnodeDAO2.InsertHyperLink("http://arachnode.net/" + (threadHyperLinkNumber) + ".htm", "http://arachnode.net/" + (threadHyperLinkNumber) + ".htm", false);

                        Counters.GetInstance().HyperLinkInserted();

                        if (threadHyperLinkNumber % 1000 == 0)
                        {
                            Console.WriteLine("BenchmarkHyperLinks: " + threadHyperLinkNumber);
                        }
                    }

                    arachnodeDAO2.CloseCommandConnections();
                });
                thread.Start(t);
            }

            while (hyperLinkNumber != totalNumberOfHyperLinks)
            {
                Thread.Sleep(1000);
            }

            _stopwatch.Stop();
            stopwatch.Stop();

            Console.WriteLine(stopwatch.Elapsed);
            Console.ReadLine();
        }

        private static void BenchmarkImages()
        {
            _stopwatch.Start();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            int imageNumber = 0;
            int totalNumberOfImages = _numberOfEntitiesToCreate;

            for (int t = 1; t <= _numberOfThreads; t++)
            {
                var thread = new Thread(delegate(object o)
                {
                    IArachnodeDAO arachnodeDAO2 = new ArachnodeDAO(_applicationSettings.ConnectionString, _applicationSettings, _webSettings, false, false);
                    arachnodeDAO2.ApplicationSettings.ClassifyAbsoluteUris = false;

                    arachnodeDAO2.OpenCommandConnections();

                    while (imageNumber != totalNumberOfImages)
                    {
                        int threadImageNumber = Interlocked.Increment(ref imageNumber);

                        arachnodeDAO2.InsertImage("http://arachnode.net/" + (threadImageNumber) + ".htm", "http://arachnode.net/" + (threadImageNumber) + ".htm", null, new byte[0], ".jpg");

                        Counters.GetInstance().ImageInserted();

                        if (threadImageNumber % 1000 == 0)
                        {
                            Console.WriteLine("BenchmarkImages: " + threadImageNumber);
                        }
                    }

                    arachnodeDAO2.CloseCommandConnections();
                });
                thread.Start(t);
            }

            while (imageNumber != totalNumberOfImages)
            {
                Thread.Sleep(1000);
            }

            _stopwatch.Stop();
            stopwatch.Stop();

            Console.WriteLine(stopwatch.Elapsed);
            Console.ReadLine();
        }

        private static void BenchmarkWebPages()
        {
            _stopwatch.Start();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            int webPageNumber = 0;
            int totalNumberOfWebPages = _numberOfEntitiesToCreate;

            for (int t = 1; t <= _numberOfThreads; t++)
            {
                var thread = new Thread(delegate(object o)
                {
                    IArachnodeDAO arachnodeDAO2 = new ArachnodeDAO(_applicationSettings.ConnectionString, _applicationSettings, _webSettings, false, false);

                    arachnodeDAO2.OpenCommandConnections();

                    while (webPageNumber != totalNumberOfWebPages)
                    {
                        int threadWebPageNumber = Interlocked.Increment(ref webPageNumber);

                        arachnodeDAO2.InsertWebPage("http://arachnode.net/" + threadWebPageNumber + ".htm", null, new byte[0], 1, ".htm", 1, false);

                        Counters.GetInstance().WebPageInserted();

                        if (threadWebPageNumber % 1000 == 0)
                        {
                            Console.WriteLine("BenchmarkWebPages: " + threadWebPageNumber);
                        }
                    }

                    arachnodeDAO2.CloseCommandConnections();
                });
                thread.Start(t);
            }

            while (webPageNumber != totalNumberOfWebPages)
            {
                Thread.Sleep(1000);
            }

            _stopwatch.Stop();
            stopwatch.Stop();

            Console.WriteLine(stopwatch.Elapsed);
            Console.ReadLine();
        }
    }
}