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

using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Value.Enums;

#endregion

namespace Arachnode.SiteCrawler.Value.AbstractClasses
{
    public abstract class ACrawlAction<TArachnodeDAO> : ABehavior<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        protected ACrawlAction(ApplicationSettings applicationSettings, WebSettings webSettings)
        {
            _applicationSettings = applicationSettings;
            _webSettings = webSettings;
        }

        protected ApplicationSettings _applicationSettings;
        protected WebSettings _webSettings;

        /// <summary>
        /// 	Gets or sets the type of the crawl action.
        /// </summary>
        /// <value>The type of the crawl action.</value>
        public CrawlActionType CrawlActionType { get; set; }

        public string Settings { get; set; }

        /// <summary>
        /// 	Performs the action.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        public abstract void PerformAction(CrawlRequest<TArachnodeDAO> crawlRequest, IArachnodeDAO arachnodeDAO);
    }
}