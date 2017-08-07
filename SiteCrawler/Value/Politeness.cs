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

#endregion

namespace Arachnode.SiteCrawler.Value
{
    /// <summary>
    /// 	Politeness class used by Rules and the RulesEngine.
    /// </summary>
    public class Politeness
    {
        /// <summary>
        /// 	Initializes a new instance of the <see cref = "Politeness" /> class.
        /// </summary>
        /// <param name = "host">The host.</param>
        public Politeness(string host)
        {
            Host = host;

            LastFileHttpWebRequestCompleted = DateTime.MinValue;
            LastImageHttpWebRequestCompleted = DateTime.MinValue;
            LastWebPageHttpWebRequestCompleted = DateTime.MinValue;
            LastHttpWebRequestCompleted = DateTime.MinValue;

            MaximumActiveHttpWebRequests = short.MaxValue;
        }

        public int ActiveHttpWebRequests { get; set; }

        /// <summary>
        /// 	Gets or sets the crawl delay.
        /// </summary>
        /// <value>The crawl delay.</value>
        public double AutoThrottleCrawlDelayInMilliseconds { get; set; }

        public bool AutoThrottleHttpWebRequests { get; set; }

        /// <summary>
        /// 	Gets or sets the crawl delay.
        /// </summary>
        /// <value>The crawl delay.</value>
        public int CrawlDelayInMilliseconds { get; set; }

        /// <summary>
        /// 	Gets or sets the disallowed paths.
        /// </summary>
        /// <value>The disallowed paths.</value>
        public List<string> DisallowedPaths { get; set; }

        /// <summary>
        /// 	Gets or sets the disallowed paths since.
        /// </summary>
        /// <value>The disallowed paths since.</value>
        public DateTime DisallowedPathsSince { get; set; }

        public DateTime FirstHttpWebRequest { get; set; }
        public DateTime LastHttpWebRequestRequested { get; set; }

        /// <summary>
        /// 	Gets or sets the host.
        /// </summary>
        /// <value>The host.</value>
        public string Host { get; set; }

        public DateTime LastFileHttpWebRequestCompleted { get; set; }
        public DateTime LastImageHttpWebRequestCompleted { get; set; }
        public DateTime LastWebPageHttpWebRequestCompleted { get; set; }
        public DateTime LastHttpWebRequestCompleted { get; set; }

        public DateTime LastFileHttpWebRequestCanceled { get; set; }
        public DateTime LastImageHttpWebRequestCanceled { get; set; }
        public DateTime LastWebPageHttpWebRequestCanceled { get; set; }
        public DateTime LastHttpWebRequestCanceled { get; set; }

        public short MaximumActiveHttpWebRequests { get; set; }

        public long TotalDownloadedBytes { get; set; }
        public long TotalFileDownloadedBytes { get; set; }
        public long TotalImageDownloadedBytes { get; set; }
        public long TotalWebPageDownloadedBytes { get; set; }

        public int TotalFileHttpWebRequestsCompleted { get; set; }
        public int TotalHttpWebRequestsCompleted { get; set; }
        public int TotalImageHttpWebRequestsCompleted { get; set; }
        public int TotalWebPageHttpWebRequestsCompleted { get; set; }

        public int TotalFileHttpWebRequestsCanceled { get; set; }
        public int TotalHttpWebRequestsCanceled { get; set; }
        public int TotalImageHttpWebRequestsCanceled { get; set; }
        public int TotalWebPageHttpWebRequestsCanceled { get; set; }

        public TimeSpan TotalFileHttpWebResponseTime { get; set; }
        public TimeSpan TotalHttpWebResponseTime { get; set; }
        public TimeSpan TotalImageHttpWebResponseTime { get; set; }
        public TimeSpan TotalWebPageHttpWebResponseTime { get; set; }

        public override string ToString()
        {
            return Host;
        }
    }
}