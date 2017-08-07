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
using System.Web;
using System.Windows.Forms;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.Renderer.Value.Enums;
using Arachnode.SiteCrawler.Managers;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.AbstractClasses;
using Arachnode.SiteCrawler.Value.Enums;
using mshtml;

#endregion

namespace Arachnode.Plugins.CrawlActions
{
    /// <summary>
    /// 	The Renderer plugin is used to process DecodedHtml form rendered CrawlRequests.
    /// 	The primary usage of rendering a WebPage is to obtain HyperLinks to Discoveries that are not present in the DecodedHtml 
    /// 	when downloaded with the WebClient, or when viewing the source from a browser.
    /// </summary>
    internal class Renderer<TArachnodeDAO> : ACrawlAction<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        public Renderer(ApplicationSettings applicationSettings, WebSettings webSettings)
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
            if (crawlRequest.RenderType != RenderType.None && crawlRequest.DataType.DiscoveryType == DiscoveryType.WebPage)
            {
                if (crawlRequest.HtmlDocument != null)
                {
                    //domain/host specific code here...
                    PerformDomainAndHostSpecificActions(crawlRequest, arachnodeDAO);
                }
            }
        }

        private void PerformDomainAndHostSpecificActions(CrawlRequest<TArachnodeDAO> crawlRequest, IArachnodeDAO arachnodeDAO)
        {
            if (crawlRequest.Discovery.Uri.AbsoluteUri == "http://www.stradeanas.it/index.php?/appalti/rilevanza_comunitaria/index")
            {
                //perform a basic proof-of-concept search...

                List<string> queryParameters = new List<string>();

                queryParameters.Add("12");
                queryParameters.Add("24");

                foreach (string queryParameter in queryParameters)
                {
                    crawlRequest.HtmlDocument.getElementById("oggetto").innerText = queryParameter;

                    foreach (HtmlElement htmlElement in crawlRequest.HtmlDocument.getElementsByTagName("input"))
                    {
                        if (htmlElement.GetAttribute("value") == "Vai")
                        {
                            string originalHtmlDocumentBodyInnerHtml = crawlRequest.HtmlDocument.body.parentElement.innerHTML;
                            htmlElement.InvokeMember("Click");
                            RenderDecodedHtml(originalHtmlDocumentBodyInnerHtml, crawlRequest.HtmlDocument);
                            break;
                        }
                    }

                    //now, take the htmlDocument.Body, and create a CrawlReqest.
                    //pass that to the DiscoveryManager to parse the results of the search.

                    CrawlRequest<TArachnodeDAO> crawlRequest2 = new CrawlRequest<TArachnodeDAO>(crawlRequest.Discovery, 1, UriClassificationType.None, UriClassificationType.None, 1, RenderType.None, RenderType.None);

                    crawlRequest2.DecodedHtml = HttpUtility.HtmlDecode(crawlRequest.HtmlDocument.documentElement.outerHTML);
                    crawlRequest2.Html = crawlRequest.HtmlDocument.documentElement.outerHTML;

                    crawlRequest.Crawl.Crawler.DiscoveryManager.AssignFileAndImageDiscoveries(crawlRequest2, arachnodeDAO);

                    foreach (Discovery<TArachnodeDAO> fileOrImageDiscovery in crawlRequest2.Discoveries.FilesAndImages.Values)
                    {
                        //CrawlRequestManager.ProcessFileOrImageDiscovery(crawlRequest, fileOrImageDiscovery, arachnodeDAO);
                    }

                    crawlRequest.Crawl.Crawler.DiscoveryManager.AssignHyperLinkDiscoveries(crawlRequest2, arachnodeDAO);

                    foreach (Discovery<TArachnodeDAO> hyperLinkDiscovery in crawlRequest2.Discoveries.HyperLinks.Values)
                    {
                        if (hyperLinkDiscovery.Uri.AbsoluteUri.Contains("bando"))
                        {
                            //in this case we know "bando" is a .pdf file... so, go ahead and tell the CRM to submit it as a file request.
                            //CrawlRequestManager.ProcessFileOrImageDiscovery(crawlRequest, hyperLinkDiscovery, arachnodeDAO);
                        }
                    }

                    //now, go back and perform additional queries...
                    //crawlRequest.Crawl.Crawler.Engine.Render(crawlRequest, RenderAction.Back, crawlRequest.RenderType);
                }
            }
        }

        private void RenderDecodedHtml(string originalHtmlDocumentBodyInnerHtml, HTMLDocumentClass htmlDocument)
        {
            DateTime startTime = DateTime.Now;

            while (originalHtmlDocumentBodyInnerHtml == htmlDocument.documentElement.outerHTML && DateTime.Now.Subtract(startTime).Duration().TotalMinutes < _applicationSettings.CrawlRequestTimeoutInMinutes)
            {
                Thread.Sleep(100);
            }
        }
    }
}