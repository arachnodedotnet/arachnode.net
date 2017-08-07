using System;
using Arachnode.Configuration;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Components;

namespace Arachnode.SiteCrawler.Value.AbstractClasses
{
    public abstract class AConsoleManager<TArachnodeDAO> : AManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        protected readonly double _tickCount = Environment.TickCount;
        protected int _section = 1;
        protected int _writeLineCount;

        protected AConsoleManager(ApplicationSettings applicationSettings, WebSettings webSettings) : base(applicationSettings, webSettings)
        {
            
        }

        /// <summary>
        /// 	Builds the output string.
        /// </summary>
        /// <param name = "strings">The strings.</param>
        /// <returns></returns>
        public abstract string BuildOutputString(params object[] strings);

        /// <summary>
        /// 	Outputs the behavior.
        /// </summary>
        /// <param name = "aBehavior">The behavior.</param>
        public abstract void OutputBehavior(ABehavior<TArachnodeDAO> aBehavior);

        /// <summary>
        /// 	Outputs the cache hit.
        /// </summary>
        /// <param name = "crawlInfo">The crawl info.</param>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "discovery">The discovery.</param>
        public abstract void OutputCacheHit(CrawlInfo<TArachnodeDAO> crawlInfo, CrawlRequest<TArachnodeDAO> crawlRequest, Discovery<TArachnodeDAO> discovery);

        /// <summary>
        /// 	Outputs the cache miss.
        /// </summary>
        /// <param name = "threadNumber">The thread number.</param>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "discovery">The discovery.</param>
        public abstract void OutputCacheMiss(int threadNumber, CrawlRequest<TArachnodeDAO> crawlRequest, Discovery<TArachnodeDAO> discovery);

        /// <summary>
        /// 	Outputs the email address discovered.
        /// </summary>
        /// <param name = "threadNumber">The thread number.</param>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "discovery">The discovery.</param>
        public abstract void OutputEmailAddressDiscovered(int threadNumber, CrawlRequest<TArachnodeDAO> crawlRequest, Discovery<TArachnodeDAO> discovery);

        /// <summary>
        /// 	Outputs the state of the engine.
        /// </summary>
        /// <param name = "crawl">The crawl.</param>
        public abstract void OutputEngineState(Crawl<TArachnodeDAO> crawl);

        /// <summary>
        /// 	Outputs the exception.
        /// </summary>
        /// <param name = "threadNumber">The thread number.</param>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "exceptionID">The exception ID.</param>
        /// <param name = "message">The message.</param>
        public abstract void OutputException(int threadNumber, CrawlRequest<TArachnodeDAO> crawlRequest, long? exceptionID, string message);

        /// <summary>
        /// 	Outputs the file discovered.
        /// </summary>
        /// <param name = "threadNumber">The thread number.</param>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "discovery">The discovery.</param>
        public abstract void OutputFileDiscovered(int threadNumber, CrawlRequest<TArachnodeDAO> crawlRequest, Discovery<TArachnodeDAO> discovery);

        /// <summary>
        /// 	Outputs the hyper link discovered.
        /// </summary>
        /// <param name = "threadNumber">The thread number.</param>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "discovery">The discovery.</param>
        public abstract void OutputHyperLinkDiscovered(int threadNumber, CrawlRequest<TArachnodeDAO> crawlRequest, Discovery<TArachnodeDAO> discovery);

        /// <summary>
        /// 	Outputs the image discovered.
        /// </summary>
        /// <param name = "threadNumber">The thread number.</param>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "discovery">The discovery.</param>
        public abstract void OutputImageDiscovered(int threadNumber, CrawlRequest<TArachnodeDAO> crawlRequest, Discovery<TArachnodeDAO> discovery);

        /// <summary>
        /// 	Outputs the is disallowed reason.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        public abstract void OutputIsDisallowedReason(CrawlRequest<TArachnodeDAO> crawlRequest);

        /// <summary>
        /// 	Outputs the is disallowed reason.
        /// </summary>
        /// <param name = "crawlInfo">The crawl info.</param>
        /// <param name = "crawlRequest">The crawl request.</param>
        public abstract void OutputIsDisallowedReason(CrawlInfo<TArachnodeDAO> crawlInfo, CrawlRequest<TArachnodeDAO> crawlRequest);

        /// <summary>
        /// 	Outputs the is disallowed reason.
        /// </summary>
        /// <param name = "crawlInfo">The crawl info.</param>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "discovery">The discovery.</param>
        public abstract void OutputIsDisallowedReason(CrawlInfo<TArachnodeDAO> crawlInfo, CrawlRequest<TArachnodeDAO> crawlRequest, Discovery<TArachnodeDAO> discovery);

        /// <summary>
        /// 	Called when the Crawl has finished processing all CrawlRequests assigned to the Crawl.
        /// </summary>
        /// <param name = "threadNumber">The thread number.</param>
        public abstract void OutputProcessCrawlRequestsEnd(int threadNumber);

        /// <summary>
        /// 	Outputs the process crawl request.
        /// </summary>
        /// <param name = "threadNumber">The thread number.</param>
        /// <param name = "crawlRequest">The crawl request.</param>
        public abstract void OutputProcessCrawlRequest(int threadNumber, CrawlRequest<TArachnodeDAO> crawlRequest);

        /// <summary>
        /// 	Outputs the process crawl requests start.
        /// </summary>
        /// <param name = "threadNumber">The thread number.</param>
        public abstract void OutputProcessCrawlRequestsStart(int threadNumber);

        /// <summary>
        ///     Uses the current colors as to the 'output' and 'afterOutput' colors.
        /// </summary>
        /// <param name="string"></param>
        public abstract void OutputString(string @string);

        public abstract void OutputString(string @string, ConsoleColor output, ConsoleColor afterOutput);

        /// <summary>
        /// 	Outputs the web page discovered.
        /// </summary>
        /// <param name = "threadNumber">The thread number.</param>
        /// <param name = "crawlRequest">The crawl request.</param>
        public abstract void OutputWebPageDiscovered(int threadNumber, CrawlRequest<TArachnodeDAO> crawlRequest);

        /// <summary>
        /// 	Refreshes the console output log.
        /// </summary>
        public abstract void RefreshConsoleOutputLog();
    }
}