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
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Value;

#endregion

namespace Arachnode.SiteCrawler.Core
{
    /// <summary>
    /// 	Currently a placeholder for parsing CrawlRequests with specific addresses formats/syntax into multiple CrawlRequests.  (reserved for future use)
    /// </summary>
    public class QueryProcessor<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        #region Delegates

        /// <summary>
        /// </summary>
        public delegate void EventHandler(object sender, EventArgs e);

        #endregion

        /// <summary>
        /// 	The QueryProcessor.
        /// </summary>
        internal QueryProcessor()
        {
        }

        /// <summary>
        /// 	Occurs when [on query successfully processed].
        /// </summary>
        public event EventHandler OnQuerySuccessfullyProcessed;

        /// <summary>
        /// 	Process a CrawlRequest before crawling.
        /// </summary>
        /// <param name = "request">The CrawlRequest to be processed.</param>
        internal void ProcessQuery(CrawlRequest<TArachnodeDAO> request)
        {
            if (OnQuerySuccessfullyProcessed != null)
            {
                OnQuerySuccessfullyProcessed(request, new EventArgs());
            }
        }
    }
}