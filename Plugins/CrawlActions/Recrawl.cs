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
using System.Net;
using Arachnode.Configuration;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.AbstractClasses;

#endregion

namespace Arachnode.Plugins.CrawlActions
{
    internal class Recrawl<TArachnodeDAO> : ACrawlAction<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        public Recrawl(ApplicationSettings applicationSettings, WebSettings webSettings)
            : base(applicationSettings, webSettings)
        {
        }

        public override void AssignSettings(Dictionary<string, string> settings)
        {
        }

        public override void Stop()
        {
        }

        public override void PerformAction(CrawlRequest<TArachnodeDAO> crawlRequest, IArachnodeDAO arachnodeDAO)
        {
            //as an example...
            if (crawlRequest.WebClient != null && crawlRequest.WebClient.HttpWebResponse != null && crawlRequest.WebClient.HttpWebResponse.ResponseUri != null)
            {
                if (crawlRequest.WebClient.HttpWebResponse.ResponseUri.AbsoluteUri.EndsWith("503.html") || crawlRequest.WebClient.HttpWebResponse.StatusCode == HttpStatusCode.ServiceUnavailable)
                {
                    crawlRequest.Crawl.Crawler.PolitenessManager.ResubmitCrawlRequest(crawlRequest, false, arachnodeDAO);
                }
            }
        }
    }
}