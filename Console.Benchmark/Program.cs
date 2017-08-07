#region License : arachnode.net

// Copyright (c) 2015 http://arachnode.net, arachnode.net, LLC
//   
// Permission is hereby granted, upon purchase, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, merge and modify copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following
// conditions:
//  
// LICENSE (ALL VERSIONS/EDITIONS): http://arachnode.net/r.ashx?3
//  
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using System.Web;
using Arachnode.Performance;

#endregion

namespace Arachnode.Console.Benchmark
{
    internal class Program
    {
        private static Stopwatch _stopwatch = new Stopwatch();
        private static bool? _generateTestSite = true;
        private static bool? _crawlTheTestSite = true;
        private static bool? _crawlTheCrawlRequests = false;

        private static object _queueLock = new object();
        private static Queue<string> _absoluteUris = new Queue<string>();
        private static double _numberOfAbsoluteUrisDownloaded;

        private static int _numberOfThreads = 10;

        private static bool _assignHyperLinkDiscoveries;
        private static bool _cacheToFileSystem;
        private static bool _saveWebPagesToDisk;

        private static void Main(string[] args)
        {
            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine("arachnode.net AN.Benchmark " + Assembly.GetExecutingAssembly().GetName().Version + "\n");
            System.Console.WriteLine("[START]: Performing basic iteration test...\n");
            System.Console.ForegroundColor = ConsoleColor.White;

            _stopwatch.Start();

            for (int i = 0; i < int.MaxValue; i++)
            {
            }

            System.Console.WriteLine("[FINISH]: Performing basic integer iteration test...\n");
            System.Console.WriteLine("Elapsed: " + _stopwatch.Elapsed + "\n");
            _stopwatch.Stop();
            _stopwatch.Reset();
            System.Console.WriteLine("Press any key to continue.");
            System.Console.ReadLine();

            System.Console.WriteLine("Generate the TestSite?  (http://localhost:56830/Test/)  (y/n)");

            if ((_generateTestSite.HasValue && _generateTestSite.Value) || (!_generateTestSite.HasValue && System.Console.ReadLine().ToLower() == "y"))
            {
                _generateTestSite = true;

                System.Console.WriteLine("Generating " + 14606 + " WebPages...");

                ProcessStartInfo processStartInfo = new ProcessStartInfo("TestSite.exe");
                processStartInfo.WorkingDirectory = "..\\..\\..\\Web\\Test";

                Process process = Process.Start(processStartInfo);

                process.WaitForExit();
            }

            //remove limits from service point manager
            ServicePointManager.MaxServicePoints = 10000;
            ServicePointManager.DefaultConnectionLimit = 10000;
            ServicePointManager.CheckCertificateRevocationList = true;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.MaxServicePointIdleTime = 1000*30;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            ServicePointManager.UseNagleAlgorithm = false;

            System.Timers.Timer timer = new System.Timers.Timer();

            timer.Interval = 1000;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Enabled = true;
            timer.Stop();

            _assignHyperLinkDiscoveries = false;
            _cacheToFileSystem = false;
            _saveWebPagesToDisk = false;
            System.Console.WriteLine("\n[START]: Crawling without HyperLink parsing or WebPage storage...");
            EnqueueCrawlRequests();
            _stopwatch.Start();
            timer.Start();
            Crawl();

            while (_absoluteUris.Count != 0)
            {
                Thread.Sleep(1000);
            }
            timer.Stop();

            System.Console.WriteLine("\n[FINISH]: Crawling without HyperLink parsing or WebPage storage...");
            TimeSpan timeSpan1 = _stopwatch.Elapsed;
            System.Console.WriteLine("Elapsed: " + timeSpan1);
            System.Console.WriteLine("Press any key to continue.");
            System.Console.ReadLine();

            /**/

            _assignHyperLinkDiscoveries = true;
            _cacheToFileSystem = false;
            _saveWebPagesToDisk = false;
            System.Console.WriteLine("\n[START]: Crawling with HyperLink parsing but without WebPage storage...");
            _stopwatch.Reset();
            EnqueueCrawlRequests();
            _stopwatch.Start();
            timer.Start();
            Crawl();

            while (_absoluteUris.Count != 0)
            {
                Thread.Sleep(1000);
            }
            timer.Stop();

            System.Console.WriteLine("\n[FINISH]: Crawling with HyperLink parsing but without WebPage storage...");
            TimeSpan timeSpan2 = _stopwatch.Elapsed;
            System.Console.WriteLine("Elapsed: " + timeSpan1);
            System.Console.WriteLine("Elapsed: " + timeSpan2);
            System.Console.WriteLine("Press any key to continue.");
            System.Console.ReadLine();

            /**/

            _assignHyperLinkDiscoveries = true;
            _cacheToFileSystem = false;
            _saveWebPagesToDisk = true;
            System.Console.WriteLine("\n[START]: Crawling with HyperLink parsing and WebPage storage...");
            _stopwatch.Reset();
            EnqueueCrawlRequests();
            _stopwatch.Start();
            timer.Start();
            Crawl();

            while (_absoluteUris.Count != 0)
            {
                Thread.Sleep(1000);
            }
            timer.Stop();

            System.Console.WriteLine("\n[FINISH]: Crawling with HyperLink parsing and WebPage storage...");
            TimeSpan timeSpan3 = _stopwatch.Elapsed;
            System.Console.WriteLine("Elapsed: " + timeSpan1);
            System.Console.WriteLine("Elapsed: " + timeSpan2);
            System.Console.WriteLine("Elapsed: " + timeSpan3);
            System.Console.WriteLine("Press any key to continue.");
            System.Console.ReadLine();

            /**/

            System.Console.WriteLine("Here is where most free crawlers stop: Non-RAM based caching...\n");
            System.Console.WriteLine("As the AbsoluteUris were explicitly specified and each AbsoluteUri is checked for on disk (multiple times as each WebPage's HyperLinks intersect other WebPages' HyperLinks), this case represents a less than 'Best Case Scenario' for disk-backed RAM-based caching for this small crawl set, but approaches a 'Best Case Scenario' for a large crawl where the number of discovered AbsoluteUris greatly outnumbers the amount of available RAM needed to store those AbsoluteUris.\n");
            System.Console.WriteLine("Press any key to continue.");
            System.Console.ReadLine();

            _assignHyperLinkDiscoveries = true;
            _cacheToFileSystem = true;
            _saveWebPagesToDisk = true;
            System.Console.WriteLine("\n[START]: Crawling with HyperLink parsing and WebPage storage and filesystem caching...");
            _stopwatch.Reset();
            EnqueueCrawlRequests();
            _stopwatch.Start();
            timer.Start();
            Crawl();

            while (_absoluteUris.Count != 0)
            {
                Thread.Sleep(1000);
            }
            timer.Stop();

            System.Console.WriteLine("\n[FINISH]: Crawling with HyperLink parsing and WebPage storage and filesystem caching...");
            TimeSpan timeSpan4 = _stopwatch.Elapsed;
            System.Console.WriteLine("Elapsed: " + timeSpan1);
            System.Console.WriteLine("Elapsed: " + timeSpan2);
            System.Console.WriteLine("Elapsed: " + timeSpan3);
            System.Console.WriteLine("Elapsed: " + timeSpan4);
            System.Console.WriteLine("Press any key to continue.");
            System.Console.ReadLine();

            /**/

            System.Console.WriteLine("When comparing crawlers 'Crawl Rate' is not an adequate measure of viability.  Many crawlers omit ");
        }

        private static void EnqueueCrawlRequests()
        {
            System.Console.WriteLine("\nCrawl the TestSite? (y/n)");
            System.Console.WriteLine("  -> http://localhost:56830/Test/1.htm (14606 WebPages)");

            if (_crawlTheTestSite.HasValue)
            {
                System.Console.WriteLine("_crawlTheTestSite: " + _crawlTheTestSite.Value);
            }
            if ((_crawlTheTestSite.HasValue && _crawlTheTestSite.Value) || (!_crawlTheTestSite.HasValue && System.Console.ReadLine().ToLower() == "y"))
            {
                for (int i = 1; i <= 14606; i++)
                {
                    _absoluteUris.Enqueue("http://localhost:56830/Test/" + i + ".htm");

                    Counters.GetInstance().CrawlRequestAdded();
                }
            }

            /**/

            System.Console.WriteLine("\nCrawl the CrawlRequests? (y/n)");
            System.Console.WriteLine("  -> http://CrawlRequests.txt (" + File.ReadAllLines("CrawlRequests.txt").Length  + " WebPages)");

            if (_crawlTheCrawlRequests.HasValue)
            {
                System.Console.WriteLine("_crawlTheCrawlRequests: " + _crawlTheCrawlRequests.Value);
            }
            if ((_crawlTheTestSite.HasValue && _crawlTheCrawlRequests.Value) || (!_crawlTheCrawlRequests.HasValue && System.Console.ReadLine().ToLower() == "y"))
            {
                foreach (string absoluteUri in File.ReadAllLines("CrawlRequests.txt"))
                {
                    string absoluteUri2 = absoluteUri.Trim();

                    try
                    {
                        if (!absoluteUri2.StartsWith("http://"))
                        {
                            _absoluteUris.Enqueue("http://" + absoluteUri2);
                        }
                        else
                        {
                            _absoluteUris.Enqueue(absoluteUri2);
                        }

                        Counters.GetInstance().CrawlRequestAdded();
                    }
                    catch (Exception exception)
                    {
                        System.Console.WriteLine(exception.Message + ":" + absoluteUri2);
                    }
                }
            }
        }

        private static void Crawl()
        {
            Regex hyperLinkAbsoluteUriRegex = new Regex("<\\s*(?<Tag>(a|base|form|frame))\\s*.*?(?<AttributeName>(action|href|src))\\s*=\\s*([\\\"\\'])(?<HyperLink>.*?)\\3", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

            for (int i = 0; i < _numberOfThreads; i++)
            {
                Thread thread = new Thread((o) =>
                                               {
                                                   int threadNumber = (int) o;

                                                   WebClient webClient = new WebClient();

                                                   while (_absoluteUris.Count != 0)
                                                   {
                                                       string absoluteUri = null;

                                                       lock (_queueLock)
                                                       {
                                                           if (_absoluteUris.Count != 0)
                                                           {
                                                               absoluteUri = _absoluteUris.Dequeue();

                                                               Counters.GetInstance().CrawlRequestRemoved();
                                                           }
                                                       }

                                                       try
                                                       {
                                                           System.Console.WriteLine(absoluteUri + " :: " + threadNumber);

                                                           byte[] data = webClient.DownloadData(absoluteUri);

                                                           if (_assignHyperLinkDiscoveries)
                                                           {
                                                               //this is appropriate for our "English only" test site, on an "English installation" of Windows, but incorrect if a WebPage contains non-English characters... (Kanji/Cyrillic/etc...)
                                                               string decodedHtml = HttpUtility.HtmlDecode(Encoding.Default.GetString(data));

                                                               MatchCollection matchCollection = hyperLinkAbsoluteUriRegex.Matches(decodedHtml);

                                                               //when assigning HyperLinks, we'll need to be able to reference something other than RAM to determine whether a HyperLink is accounted for...
                                                               if (_cacheToFileSystem)
                                                               {
                                                                   foreach (Match match in matchCollection)
                                                                   {
                                                                       string tempFileName = Path.Combine(Path.GetTempPath(), Path.GetFileName(match.Groups["HyperLink"].Value));

                                                                       if (File.Exists(tempFileName))
                                                                       {
                                                                       }
                                                                   }
                                                               }
                                                           }

                                                           if (_saveWebPagesToDisk)
                                                           {
                                                               string tempFileName = Path.Combine(Path.GetTempPath(), Path.GetFileName(absoluteUri));

                                                               File.WriteAllBytes(tempFileName, data);
                                                           }

                                                           Counters.GetInstance().CrawlRequestProcessed();
                                                       }
                                                       catch (Exception exception)
                                                       {
                                                           System.Console.ForegroundColor = ConsoleColor.Red;
                                                           System.Console.WriteLine(exception.Message);
                                                           System.Console.ForegroundColor = ConsoleColor.White;
                                                       }

                                                       lock (_queueLock)
                                                       {
                                                           _numberOfAbsoluteUrisDownloaded++;
                                                       }
                                                   }
                                               });

                thread.Start(i);
            }
        }

        private static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (_queueLock)
            {
                System.Console.ForegroundColor = ConsoleColor.Green;
                System.Console.WriteLine((_numberOfAbsoluteUrisDownloaded) + " AbsoluteUris/s :: " + _absoluteUris.Count + " remaining.");
                System.Console.ForegroundColor = ConsoleColor.White;

                _numberOfAbsoluteUrisDownloaded = 0;
            }
        }
    }
}