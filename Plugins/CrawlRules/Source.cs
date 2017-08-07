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

using System.Collections.Generic;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.AbstractClasses;
using Arachnode.SiteCrawler.Value.Enums;

#endregion

namespace Arachnode.Plugins.CrawlRules
{
    public class Source<TArachnodeDAO> : ACrawlRule<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        private bool _negateIsDisallowed;

        public Source(ApplicationSettings applicationSettings, WebSettings webSettings)
            : base(applicationSettings, webSettings)
        {
        }

        /// <summary>
        /// 	Assigns the additional parameters.
        /// </summary>
        /// <param name = "settings"></param>
        public override void AssignSettings(Dictionary<string, string> settings)
        {
            _negateIsDisallowed = bool.Parse(settings["NegateIsDisallowed"]);
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
            bool isDisallowed = false;

            if (crawlRequest.DataType.DiscoveryType == DiscoveryType.WebPage)
            {
                crawlRequest.OutputIsDisallowedReason = OutputIsDisallowedReason;
                crawlRequest.IsDisallowedReason = "Disallowed by Source.";

                if (UserDefinedFunctions.IsDisallowedForSource(crawlRequest.DecodedHtml, false))
                {
                    isDisallowed = true;
                }

                if (_negateIsDisallowed)
                {
                    isDisallowed = !isDisallowed;
                }
            }

            return isDisallowed;
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