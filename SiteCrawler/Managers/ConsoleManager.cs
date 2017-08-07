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
using System.Text;
using Arachnode.Configuration;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Components;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.AbstractClasses;
using Arachnode.SiteCrawler.Value.Enums;

#endregion

namespace Arachnode.SiteCrawler.Managers
{
    /// <summary>
    /// 	Provides console output and logging functionality.
    /// </summary>
    public class ConsoleManager<TArachnodeDAO> : AConsoleManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        public ConsoleManager(ApplicationSettings applicationSettings, WebSettings webSettings) : base(applicationSettings, webSettings)
        {
            
        }

        /// <summary>
        /// 	Builds the output string.
        /// </summary>
        /// <param name = "strings">The strings.</param>
        /// <returns></returns>
        public override string BuildOutputString(params object[] strings)
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < strings.Length; i++)
            {
                if (i%2 == 0)
                {
                    stringBuilder.Append(strings[i] + ":");
                }
                else
                {
                    if (i + 1 < strings.Length)
                    {
                        stringBuilder.Append(strings[i] + "|");
                    }
                    else
                    {
                        stringBuilder.Append(strings[i]);
                    }
                }
            }

            _writeLineCount++;

            return DateTime.Now.ToShortTimeString() + " | " + stringBuilder.ToString();
        }

        /// <summary>
        /// 	Outputs the behavior.
        /// </summary>
        /// <param name = "aBehavior">The behavior.</param>
        public override void OutputBehavior(ABehavior<TArachnodeDAO> aBehavior)
        {
            if (ApplicationSettings.EnableConsoleOutput)
            {
                Console.ForegroundColor = ConsoleColor.White;

                if (aBehavior is ACrawlAction<TArachnodeDAO>)
                {
                    Console.Write(BuildOutputString("CrawlAction: "));
                }

                if (aBehavior is ACrawlRule<TArachnodeDAO>)
                {
                    Console.Write(BuildOutputString("CrawlRule: "));
                }

                if (aBehavior is AEngineAction<TArachnodeDAO>)
                {
                    Console.Write(BuildOutputString("EngineAction: "));
                }

                Console.WriteLine(aBehavior.TypeName);
                Console.Write(BuildOutputString(" -> IsEnabled: "));
                if (aBehavior.IsEnabled)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                Console.Write(aBehavior.IsEnabled);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" Order: " + aBehavior.Order);

                if (aBehavior is ACrawlAction<TArachnodeDAO>)
                {
                    Console.WriteLine(BuildOutputString(" CrawlActionType: " + ((ACrawlAction<TArachnodeDAO>)aBehavior).CrawlActionType));
                }

                if (aBehavior is ACrawlRule<TArachnodeDAO>)
                {
                    Console.WriteLine(BuildOutputString(" CrawlRuleType: " + ((ACrawlRule<TArachnodeDAO>)aBehavior).CrawlRuleType));
                }

                if (aBehavior is AEngineAction<TArachnodeDAO>)
                {
                    Console.WriteLine(BuildOutputString(" EngineActionType: " + ((AEngineAction<TArachnodeDAO>)aBehavior).EngineActionType));
                }
            }
        }

        /// <summary>
        /// 	Outputs the cache hit.
        /// </summary>
        /// <param name = "crawlInfo">The crawl info.</param>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "discovery">The discovery.</param>
        public override void OutputCacheHit(CrawlInfo<TArachnodeDAO> crawlInfo, CrawlRequest<TArachnodeDAO> crawlRequest, Discovery<TArachnodeDAO> discovery)
        {
            if (ApplicationSettings.EnableConsoleOutput && ApplicationSettings.VerboseOutput)
            {
                Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine(BuildOutputString("ot", OutputType.CacheHit, "tn", crawlInfo.ThreadNumber, "ecr", crawlInfo.EnqueuedCrawlRequests, "crcd", crawlRequest.CurrentDepth, "crmd", crawlRequest.MaximumDepth, "AbsoluteUri", " " + discovery.Uri.AbsoluteUri + " "));

                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        /// <summary>
        /// 	Outputs the cache miss.
        /// </summary>
        /// <param name = "threadNumber">The thread number.</param>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "discovery">The discovery.</param>
        public override void OutputCacheMiss(int threadNumber, CrawlRequest<TArachnodeDAO> crawlRequest, Discovery<TArachnodeDAO> discovery)
        {
            if (ApplicationSettings.EnableConsoleOutput && ApplicationSettings.VerboseOutput)
            {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine(BuildOutputString("ot", OutputType.CacheMiss, "tn", threadNumber, "crcd", crawlRequest.CurrentDepth, "crmd", crawlRequest.MaximumDepth, "AbsoluteUri", " " + discovery.Uri.AbsoluteUri + " "));

                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        /// <summary>
        /// 	Outputs the email address discovered.
        /// </summary>
        /// <param name = "threadNumber">The thread number.</param>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "discovery">The discovery.</param>
        public override void OutputEmailAddressDiscovered(int threadNumber, CrawlRequest<TArachnodeDAO> crawlRequest, Discovery<TArachnodeDAO> discovery)
        {
            if (ApplicationSettings.EnableConsoleOutput && ApplicationSettings.VerboseOutput)
            {
                Console.WriteLine(BuildOutputString(" -> EmailAddress Discovered"));
                Console.WriteLine(BuildOutputString("ot", OutputType.EmailAddressDiscovered, "tn", threadNumber, "crcd", crawlRequest.CurrentDepth, "crmd", crawlRequest.MaximumDepth, "AbsoluteUri", " " + discovery.Uri.AbsoluteUri + " ", "sc", crawlRequest.WebClient.HttpWebResponse.StatusCode));
            }
        }

        /// <summary>
        /// 	Outputs the state of the engine.
        /// </summary>
        /// <param name = "crawl">The crawl.</param>
        public override void OutputEngineState(Crawl<TArachnodeDAO> crawl)
        {
            if (ApplicationSettings.EnableConsoleOutput)
            {
                Console.WriteLine(BuildOutputString("ot", OutputType.EngineState, "tn", crawl.CrawlInfo.ThreadNumber, "cr", crawl.UncrawledCrawlRequests.Count, "State", crawl.Crawler.Engine.State));
            }
        }

        /// <summary>
        /// 	Outputs the exception.
        /// </summary>
        /// <param name = "threadNumber">The thread number.</param>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "exceptionID">The exception ID.</param>
        /// <param name = "message">The message.</param>
        public override void OutputException(int threadNumber, CrawlRequest<TArachnodeDAO> crawlRequest, long? exceptionID, string message)
        {
            if (ApplicationSettings.EnableConsoleOutput && (ApplicationSettings.OutputWebExceptions || ApplicationSettings.VerboseOutput))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;

                if (crawlRequest.WebClient != null && crawlRequest.WebClient.HttpWebResponse != null)
                {
                    Console.WriteLine(BuildOutputString("ot", OutputType.Exception, "tn", threadNumber, "crcd", crawlRequest.CurrentDepth, "crmd", crawlRequest.MaximumDepth, "AbsoluteUri", " " + crawlRequest.Discovery.Uri.AbsoluteUri + " ", "ExceptionID", exceptionID, "Message", message, "sc", crawlRequest.WebClient.HttpWebResponse.StatusCode + " " + (int)crawlRequest.WebClient.HttpWebResponse.StatusCode));
                }
                else
                {
                    Console.WriteLine(BuildOutputString("ot", OutputType.Exception, "tn", threadNumber, "crcd", crawlRequest.CurrentDepth, "crmd", crawlRequest.MaximumDepth, "AbsoluteUri", " " + crawlRequest.Discovery.Uri.AbsoluteUri + " ", "ExceptionID", exceptionID, "Message", message));
                }

                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        /// <summary>
        /// 	Outputs the file discovered.
        /// </summary>
        /// <param name = "threadNumber">The thread number.</param>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "discovery">The discovery.</param>
        public override void OutputFileDiscovered(int threadNumber, CrawlRequest<TArachnodeDAO> crawlRequest, Discovery<TArachnodeDAO> discovery)
        {
            if (ApplicationSettings.EnableConsoleOutput && ApplicationSettings.VerboseOutput)
            {
                Console.WriteLine(BuildOutputString(" -> File Discovered"));
                Console.WriteLine(BuildOutputString("ot", OutputType.FileDiscovered, "tn", threadNumber, "crcd", crawlRequest.CurrentDepth, "crmd", crawlRequest.MaximumDepth, "AbsoluteUri", " " + discovery.Uri.AbsoluteUri + " ", "sc", crawlRequest.WebClient.HttpWebResponse.StatusCode));
            }
        }

        /// <summary>
        /// 	Outputs the hyper link discovered.
        /// </summary>
        /// <param name = "threadNumber">The thread number.</param>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "discovery">The discovery.</param>
        public override void OutputHyperLinkDiscovered(int threadNumber, CrawlRequest<TArachnodeDAO> crawlRequest, Discovery<TArachnodeDAO> discovery)
        {
            if (ApplicationSettings.EnableConsoleOutput && ApplicationSettings.VerboseOutput)
            {
                Console.WriteLine(BuildOutputString("ot", OutputType.HyperLinkDiscovered, "tn", threadNumber, "crcd", crawlRequest.CurrentDepth, "crmd", crawlRequest.MaximumDepth, "AbsoluteUri", " " + discovery.Uri.AbsoluteUri + " "));
            }
        }

        /// <summary>
        /// 	Outputs the image discovered.
        /// </summary>
        /// <param name = "threadNumber">The thread number.</param>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "discovery">The discovery.</param>
        public override void OutputImageDiscovered(int threadNumber, CrawlRequest<TArachnodeDAO> crawlRequest, Discovery<TArachnodeDAO> discovery)
        {
            if (ApplicationSettings.EnableConsoleOutput && ApplicationSettings.VerboseOutput)
            {
                Console.WriteLine(BuildOutputString(" -> Image Discovered"));
                Console.WriteLine(BuildOutputString("ot", OutputType.ImageDiscovered, "tn", threadNumber, "crcd", crawlRequest.CurrentDepth, "crmd", crawlRequest.MaximumDepth, "AbsoluteUri", " " + discovery.Uri.AbsoluteUri + " ", "sc", crawlRequest.WebClient.HttpWebResponse.StatusCode));
            }
        }

        /// <summary>
        /// 	Outputs the is disallowed reason.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        public override void OutputIsDisallowedReason(CrawlRequest<TArachnodeDAO> crawlRequest)
        {
            if (ApplicationSettings.EnableConsoleOutput && crawlRequest.OutputIsDisallowedReason)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;

                Console.WriteLine(BuildOutputString("ot", OutputType.IsDisallowedReason, "AbsoluteUri", " " + crawlRequest.Discovery.Uri.AbsoluteUri + " ", "IsDisallowedReason", crawlRequest.IsDisallowedReason));

                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        /// <summary>
        /// 	Outputs the is disallowed reason.
        /// </summary>
        /// <param name = "crawlInfo">The crawl info.</param>
        /// <param name = "crawlRequest">The crawl request.</param>
        public override void OutputIsDisallowedReason(CrawlInfo<TArachnodeDAO> crawlInfo, CrawlRequest<TArachnodeDAO> crawlRequest)
        {
            if (ApplicationSettings.EnableConsoleOutput && crawlRequest.OutputIsDisallowedReason)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;

                Console.WriteLine(BuildOutputString("ot", OutputType.IsDisallowedReason, "tn", crawlInfo.ThreadNumber, "crcd", crawlRequest.CurrentDepth, "crmd", crawlRequest.MaximumDepth, "ecr", crawlInfo.EnqueuedCrawlRequests, "AbsoluteUri", " " + crawlRequest.Discovery.Uri.AbsoluteUri + " ", "IsDisallowedReason", crawlRequest.IsDisallowedReason));

                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        /// <summary>
        /// 	Outputs the is disallowed reason.
        /// </summary>
        /// <param name = "crawlInfo">The crawl info.</param>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "discovery">The discovery.</param>
        public override void OutputIsDisallowedReason(CrawlInfo<TArachnodeDAO> crawlInfo, CrawlRequest<TArachnodeDAO> crawlRequest, Discovery<TArachnodeDAO> discovery)
        {
            if (ApplicationSettings.EnableConsoleOutput && discovery.OutputIsDisallowedReason)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;

                Console.WriteLine(BuildOutputString("ot", OutputType.IsDisallowedReason, "tn", crawlInfo.ThreadNumber, "crcd", crawlRequest.CurrentDepth, "crmd", crawlRequest.MaximumDepth, "ecr", crawlInfo.EnqueuedCrawlRequests, "AbsoluteUri", " " + discovery.Uri.AbsoluteUri + " ", "IsDisallowedReason", discovery.IsDisallowedReason));

                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        /// <summary>
        /// 	Outputs the process crawl requests end.  Called when the Crawl has finished processing all CrawlRequests assigned to the Crawl.
        /// </summary>
        /// <param name = "threadNumber">The thread number.</param>
        public override void OutputProcessCrawlRequestsEnd(int threadNumber)
        {
            if (ApplicationSettings.EnableConsoleOutput)
            {
                Console.WriteLine(BuildOutputString("ot", OutputType.ProcessCrawlRequestsEnd, "tn", threadNumber));
            }
        }

        /// <summary>
        /// 	Outputs the process crawl request.
        /// </summary>
        /// <param name = "threadNumber">The thread number.</param>
        /// <param name = "crawlRequest">The crawl request.</param>
        public override void OutputProcessCrawlRequest(int threadNumber, CrawlRequest<TArachnodeDAO> crawlRequest)
        {
            try
            {
                if (ApplicationSettings.EnableConsoleOutput)
                {
                    if (crawlRequest.WebClient == null)
                    {
                        Console.WriteLine(BuildOutputString("ot", OutputType.ProcessCrawlRequest, "tn", threadNumber, "crcd", crawlRequest.CurrentDepth, "crmd", crawlRequest.MaximumDepth, "AbsoluteUri", " " + crawlRequest.Discovery.Uri.AbsoluteUri + " ", "sc", "Processing"));
                    }
                    else if (crawlRequest.WebClient != null && crawlRequest.WebClient.HttpWebResponse != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;

                        Console.WriteLine(BuildOutputString("ot", OutputType.ProcessCrawlRequest, "tn", threadNumber, "crcd", crawlRequest.CurrentDepth, "crmd", crawlRequest.MaximumDepth, "AbsoluteUri", " " + crawlRequest.Discovery.Uri.AbsoluteUri + " ", "sc", crawlRequest.WebClient.HttpWebResponse != null ? crawlRequest.WebClient.HttpWebResponse.StatusCode.ToString() : "NULL"));

                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 	Outputs the process crawl requests start.
        /// </summary>
        /// <param name = "threadNumber">The thread number.</param>
        public override void OutputProcessCrawlRequestsStart(int threadNumber)
        {
            if (ApplicationSettings.EnableConsoleOutput)
            {
                Console.WriteLine(BuildOutputString("ot", OutputType.ProcessCrawlRequestsStart, "tn", threadNumber));
            }
        }

        /// <summary>
        ///     Uses the current colors as to the 'output' and 'afterOutput' colors.
        /// </summary>
        /// <param name="string"></param>
        public override void OutputString(string @string)
        {
            OutputString(@string, Console.ForegroundColor, Console.ForegroundColor);
        }

        public override void OutputString(string @string, ConsoleColor output, ConsoleColor afterOutput)
        {
            if (ApplicationSettings.EnableConsoleOutput)
            {
                Console.ForegroundColor = output;

                Console.WriteLine(DateTime.Now.ToShortTimeString() + " | " + @string);

                Console.ForegroundColor = afterOutput;
            }
        }

        /// <summary>
        /// 	Outputs the web page discovered.
        /// </summary>
        /// <param name = "threadNumber">The thread number.</param>
        /// <param name = "crawlRequest">The crawl request.</param>
        public override void OutputWebPageDiscovered(int threadNumber, CrawlRequest<TArachnodeDAO> crawlRequest)
        {
            if (ApplicationSettings.EnableConsoleOutput && ApplicationSettings.VerboseOutput)
            {
                Console.WriteLine(BuildOutputString(" -> WebPage Discovered"));
                Console.WriteLine(BuildOutputString("ot", OutputType.WebPageDiscovered, "tn", threadNumber, "crcd", crawlRequest.CurrentDepth, "crmd", crawlRequest.MaximumDepth, "AbsoluteUri", " " + crawlRequest.Discovery.Uri.AbsoluteUri + " ", "sc", crawlRequest.WebClient.HttpWebResponse.StatusCode));
            }
        }

        /// <summary>
        /// 	Refreshes the console output log.
        /// </summary>
        public override void RefreshConsoleOutputLog()
        {
            if (ApplicationSettings.EnableConsoleOutput && ApplicationSettings.OutputConsoleToLogs && (_writeLineCount == 0 || _writeLineCount >= 10000))
            {
                Console.SetOut(new StreamWriter(new FileStream(ApplicationSettings.ConsoleOutputLogsDirectory + @"\ConsoleOutputLog_" + _tickCount + "_" + _section + ".txt", FileMode.Create)));
            }
            _writeLineCount = 0;
            _section++;
        }
    }
}