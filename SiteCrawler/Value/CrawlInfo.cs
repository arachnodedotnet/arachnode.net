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

#endregion

#region

using System;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Value.Enums;

#endregion

namespace Arachnode.SiteCrawler.Value
{
    public class CrawlInfo<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        public CrawlInfo()
        {
            Guid = Guid.NewGuid();
        }

        public CrawlState CrawlState { get; internal set; }

        /// <summary>
        /// 	Gets or sets the current crawl request.
        /// </summary>
        /// <value>The current crawl request.</value>
        public CrawlRequest<TArachnodeDAO> CurrentCrawlRequest { get; internal set; }

        /// <summary>
        /// 	Gets or sets the enqueued crawl requests.
        /// </summary>
        /// <value>The enqueued crawl requests.</value>
        public int EnqueuedCrawlRequests { get; internal set; }

        public Guid Guid { get; private set; }

        /// <summary>
        /// 	Used by the Engine to control Breadth-wise crawling.
        /// </summary>
        internal int MaximumCrawlDepth { get; set; }

        public int TotalCrawlFedCount { get; internal set; }
        public int TotalCrawlRequestsAssigned { get; internal set; }
        public int TotalCrawlStarvedCount { get; internal set; }
        public int TotalCrawlRequestsProcessed { get; internal set; }
        public TimeSpan TotalHttpWebResponseTime { get; internal set; }

        /// <summary>
        /// 	Gets or sets the thread number.
        /// </summary>
        /// <value>The thread number.</value>
        public int ThreadNumber { get; internal set; }
    }
}