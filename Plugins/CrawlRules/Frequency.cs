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
using System.Threading;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.AbstractClasses;

#endregion

namespace Arachnode.Plugins.CrawlRules
{
    public class Frequency<TArachnodeDAO> : ACrawlRule<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        private int _maximumNumberOfWebRequestsPerHostPerDay;
        private int _threadSleepTimeInMillisecondsBetweenWebRequests;

        public Frequency(ApplicationSettings applicationSettings, WebSettings webSettings)
            : base(applicationSettings, webSettings)
        {
        }

        /// <summary>
        /// 	Assigns the additional parameters.
        /// </summary>
        /// <param name = "settings"></param>
        public override void AssignSettings(Dictionary<string, string> settings)
        {
            _maximumNumberOfWebRequestsPerHostPerDay = int.Parse(settings["MaximumNumberOfWebRequestsPerHostPerDay"]);
            _threadSleepTimeInMillisecondsBetweenWebRequests = int.Parse(settings["ThreadSleepTimeInMillisecondsBetweenWebRequests"]);
        }

        /// <summary>
        /// 	Determines whether the specified crawl request is disallowed.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        /// <returns>
        /// 	<c>true</c> if the specified crawl request is disallowed; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsDisallowed(CrawlRequest<TArachnodeDAO> crawlRequest, IArachnodeDAO arachnodeDAO)
        {
            //if a CrawlRequest has a Priority of double.MaxValue it is a CrawlRequest needed to satisfy a piece of content belonging to a WebPage.  (e.g. An image, a file.)
            if (crawlRequest.Priority != 1000001)
            {
                while (DateTime.Now.Subtract(crawlRequest.Politeness.LastWebPageHttpWebRequestCompleted).TotalMilliseconds < _threadSleepTimeInMillisecondsBetweenWebRequests)
                {
                    Thread.Sleep(10);
                }
            }

            crawlRequest.OutputIsDisallowedReason = OutputIsDisallowedReason;

            if (DateTime.Now.Subtract(crawlRequest.Politeness.FirstHttpWebRequest) > TimeSpan.FromDays(1))
            {
                crawlRequest.Politeness.FirstHttpWebRequest = DateTime.Now;
                crawlRequest.Politeness.TotalHttpWebRequestsCompleted = 0;
            }

            if (crawlRequest.Politeness.TotalHttpWebRequestsCompleted + crawlRequest.Politeness.TotalHttpWebRequestsCanceled >= _maximumNumberOfWebRequestsPerHostPerDay)
            {
                crawlRequest.IsDisallowedReason = "Too many HttpWebRequests per day.";
                return true;
            }

            return false;
        }

        /// <summary>
        /// 	Determines whether the specified crawl request is disallowed.
        /// </summary>
        /// <param name = "discovery">The discovery.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        /// <returns>
        /// 	<c>true</c> if the specified crawl request is disallowed; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsDisallowed(Discovery<TArachnodeDAO> discovery, IArachnodeDAO arachnodeDAO)
        {
            return false;
        }

        /// <summary>
        /// 	Stops this instance.
        /// </summary>
        public override void Stop()
        {
        }
    }
}