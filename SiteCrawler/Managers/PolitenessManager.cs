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
using System.Threading;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.Performance;
using Arachnode.SiteCrawler.Core;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.AbstractClasses;
using Arachnode.SiteCrawler.Value.Enums;

#endregion

namespace Arachnode.SiteCrawler.Managers
{
    public class PolitenessManager<TArachnodeDAO> : APolitenessManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        public PolitenessManager(ApplicationSettings applicationSettings, WebSettings webSettings, Cache<TArachnodeDAO> cache) : base(applicationSettings, webSettings, cache)
        {
        }

        public override bool ManagePoliteness(CrawlRequest<TArachnodeDAO> crawlRequest, PolitenessState politenessState, IArachnodeDAO arachnodeDAO)
        {
            if (crawlRequest != null && crawlRequest.Politeness == null)
            {
                string domain = UserDefinedFunctions.ExtractDomain(crawlRequest.Discovery.Uri.AbsoluteUri).Value;

                //politeness/throttling can operate per host (cars.msn.com) or per domain (msn.com)...
                //string host = UserDefinedFunctions.ExtractHost(crawlRequest.Discovery.Uri.AbsoluteUri).Value;
                //domain = host;

                crawlRequest.Politeness = _cache.GetPoliteness(domain);

                if (crawlRequest.Politeness == null)
                {
                    crawlRequest.Politeness = new Politeness(domain);
                    crawlRequest.Politeness.FirstHttpWebRequest = DateTime.Now;

                    _cache.AddPoliteness(crawlRequest.Politeness);
                }
            }

            if (crawlRequest != null && crawlRequest.Politeness != null)
            {
                if (politenessState == PolitenessState.HttpWebRequestRequested)
                {
                    if ((crawlRequest.Politeness.CrawlDelayInMilliseconds != 0 && DateTime.Now.Subtract(crawlRequest.Politeness.LastHttpWebRequestCompleted).TotalMilliseconds < crawlRequest.Politeness.CrawlDelayInMilliseconds))
                    {
                        ResubmitCrawlRequest(crawlRequest, true, arachnodeDAO);

                        return false;
                    }

                    if (crawlRequest.Politeness.ActiveHttpWebRequests >= crawlRequest.Politeness.MaximumActiveHttpWebRequests)
                    {
                        ResubmitCrawlRequest(crawlRequest, true, arachnodeDAO);

                        return false;
                    }

                    if (ApplicationSettings.AutoThrottleHttpWebRequests)
                    {
                        if (crawlRequest.Politeness.AutoThrottleHttpWebRequests)
                        {
                            if (crawlRequest.Politeness.CrawlDelayInMilliseconds == 0)
                            {
                                if (crawlRequest.Politeness.LastHttpWebRequestCompleted == DateTime.MinValue)
                                {
                                    crawlRequest.Politeness.LastHttpWebRequestCompleted = crawlRequest.Politeness.LastHttpWebRequestRequested;
                                }

                                double millisecondsBetweenLastCanceledAndLastCompleted = crawlRequest.Politeness.LastHttpWebRequestCanceled.Subtract(crawlRequest.Politeness.LastHttpWebRequestCompleted).TotalMilliseconds;
                                double millisecondsBetweenNowAndLastRequested = DateTime.Now.Subtract(crawlRequest.Politeness.LastHttpWebRequestRequested).TotalMilliseconds;

                                if (crawlRequest.Politeness.AutoThrottleCrawlDelayInMilliseconds == 0 && millisecondsBetweenLastCanceledAndLastCompleted > 0)
                                {
                                    crawlRequest.Politeness.AutoThrottleCrawlDelayInMilliseconds = millisecondsBetweenLastCanceledAndLastCompleted;
                                }

                                if (crawlRequest.Politeness.AutoThrottleCrawlDelayInMilliseconds > millisecondsBetweenNowAndLastRequested)
                                {
                                    if (millisecondsBetweenLastCanceledAndLastCompleted > 0)
                                    {
                                        crawlRequest.Politeness.AutoThrottleCrawlDelayInMilliseconds = millisecondsBetweenLastCanceledAndLastCompleted;
                                    }

                                    ResubmitCrawlRequest(crawlRequest, true, arachnodeDAO);

                                    return false;
                                }
                                else
                                {
                                    crawlRequest.Politeness.AutoThrottleCrawlDelayInMilliseconds *= 0.9;
                                }
                            }
                        }
                    }

                    lock (_lock)
                    {
                        crawlRequest.Politeness.ActiveHttpWebRequests++;
                    }

                    crawlRequest.Politeness.LastHttpWebRequestRequested = DateTime.Now;

                    return true;
                }

                lock (_lock)
                {
                    crawlRequest.Politeness.ActiveHttpWebRequests--;

                    if(crawlRequest.Politeness.ActiveHttpWebRequests < 0)
                    {
                        //shouldn't occur...
                        crawlRequest.Politeness.ActiveHttpWebRequests = 0;
                    }
                }

                switch (crawlRequest.DataType.DiscoveryType)
                {
                    case DiscoveryType.File:
                        switch (politenessState)
                        {
                            case PolitenessState.HttpWebRequestCompleted:
                                crawlRequest.Politeness.LastFileHttpWebRequestCompleted = DateTime.Now;
                                crawlRequest.Politeness.TotalFileHttpWebRequestsCompleted++;
                                break;
                            case PolitenessState.HttpWebRequestCanceled:
                                crawlRequest.Politeness.LastFileHttpWebRequestCanceled = DateTime.Now;
                                crawlRequest.Politeness.TotalFileHttpWebRequestsCanceled++;
                                break;
                        }
                        if (crawlRequest.Data != null)
                        {
                            crawlRequest.Politeness.TotalFileDownloadedBytes += crawlRequest.Data.LongLength;
                        }
                        crawlRequest.Politeness.TotalFileHttpWebResponseTime += crawlRequest.HttpWebResponseTime;
                        break;
                    case DiscoveryType.Image:
                        switch (politenessState)
                        {
                            case PolitenessState.HttpWebRequestCompleted:
                                crawlRequest.Politeness.LastImageHttpWebRequestCompleted = DateTime.Now;
                                crawlRequest.Politeness.TotalImageHttpWebRequestsCompleted++;
                                break;
                            case PolitenessState.HttpWebRequestCanceled:
                                crawlRequest.Politeness.LastImageHttpWebRequestCanceled = DateTime.Now;
                                crawlRequest.Politeness.TotalImageHttpWebRequestsCanceled++;
                                break;
                        }
                        if (crawlRequest.Data != null)
                        {
                            crawlRequest.Politeness.TotalImageDownloadedBytes += crawlRequest.Data.LongLength;
                        }
                        crawlRequest.Politeness.TotalImageHttpWebResponseTime += crawlRequest.HttpWebResponseTime;
                        break;
                    case DiscoveryType.WebPage:
                        switch (politenessState)
                        {
                            case PolitenessState.HttpWebRequestCompleted:
                                crawlRequest.Politeness.LastWebPageHttpWebRequestCompleted = DateTime.Now;
                                crawlRequest.Politeness.TotalWebPageHttpWebRequestsCompleted++;
                                break;
                            case PolitenessState.HttpWebRequestCanceled:
                                crawlRequest.Politeness.LastWebPageHttpWebRequestCanceled = DateTime.Now;
                                crawlRequest.Politeness.TotalWebPageHttpWebRequestsCanceled++;
                                break;
                        }
                        if (crawlRequest.Data != null)
                        {
                            crawlRequest.Politeness.TotalWebPageDownloadedBytes += crawlRequest.Data.LongLength;
                        }
                        crawlRequest.Politeness.TotalWebPageHttpWebResponseTime += crawlRequest.HttpWebResponseTime;
                        break;
                }

                switch (politenessState)
                {
                    case PolitenessState.HttpWebRequestCompleted:
                        crawlRequest.Politeness.LastHttpWebRequestCompleted = DateTime.Now;
                        crawlRequest.Politeness.TotalHttpWebRequestsCompleted++;
                        break;
                    case PolitenessState.HttpWebRequestCanceled:
                        crawlRequest.Politeness.LastHttpWebRequestCanceled = DateTime.Now;
                        crawlRequest.Politeness.TotalHttpWebRequestsCanceled++;

                        crawlRequest.Politeness.AutoThrottleHttpWebRequests = true;
                        break;
                }

                if (crawlRequest.Data != null)
                {
                    crawlRequest.Politeness.TotalDownloadedBytes += crawlRequest.Data.LongLength;
                }
                crawlRequest.Politeness.TotalHttpWebResponseTime += crawlRequest.HttpWebResponseTime;
            }

            return true;
        }

        public override void ResubmitCrawlRequest(CrawlRequest<TArachnodeDAO> crawlRequest, bool retryIndefinitely, IArachnodeDAO arachnodeDAO)
        {
            Thread.Sleep(10);

            if (crawlRequest.Discovery.HttpWebRequestRetriesRemaining != 0 || retryIndefinitely)
            {
                //resetting the DiscoveryState to allow the CrawlRequest to (attempt to) be re-crawled...
                crawlRequest.Discovery.DiscoveryState = DiscoveryState.Undiscovered;

                //removed because it will be re-added...
                Counters.GetInstance().CrawlRequestRemoved();

                if (crawlRequest.Priority > 0)
                {
                    crawlRequest.Priority = double.MinValue + crawlRequest.Priority;
                }
                _cache.UncrawledCrawlRequests.Enqueue(crawlRequest, crawlRequest.Priority);

                if (!retryIndefinitely)
                {
                    crawlRequest.Discovery.HttpWebRequestRetriesRemaining--;
                }
            }
            else
            {
                crawlRequest.Crawl.Crawler.Engine.OnCrawlRequestCanceled(crawlRequest);

                if (crawlRequest.IsFromDatabase)
                {
                    arachnodeDAO.DeleteCrawlRequest(crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri);
                }

                Counters.GetInstance().ReportCurrentDepth(crawlRequest.CurrentDepth);

                Counters.GetInstance().CrawlRequestRemoved();

                Counters.GetInstance().CrawlRequestProcessed();
            }
        }
    }
}