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
using System.IO;
using System.Text;
using System.Web;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.AbstractClasses;

#endregion

namespace Arachnode.SiteCrawler.Managers
{
    /// <summary>
    /// 	Provides core WebPage management functionality.
    /// </summary>
    public class WebPageManager<TArachnodeDAO> : AWebPageManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        /// <summary>
        /// 	The WebPageManager.
        /// </summary>
        /// <param name = "arachnodeDAO">Must be thread-safe.</param>
        public WebPageManager(ApplicationSettings applicationSettings, WebSettings webSettings, DiscoveryManager<TArachnodeDAO> discoveryManager, HtmlManager<TArachnodeDAO> htmlManager, IArachnodeDAO arachnodeDAO) : base(applicationSettings, webSettings, discoveryManager, htmlManager, arachnodeDAO)
        {
        }

        /// <summary>
        /// 	Manages the web page.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        public override void ManageWebPage(CrawlRequest<TArachnodeDAO> crawlRequest)
        {
            if (ApplicationSettings.InsertWebPages && crawlRequest.IsStorable)
            {
                crawlRequest.Discovery.ID = _arachnodeDAO.InsertWebPage(crawlRequest.Discovery.Uri.AbsoluteUri, ApplicationSettings.InsertWebPageResponseHeaders ? crawlRequest.WebClient.HttpWebResponse.Headers.ToString() : null, ApplicationSettings.InsertWebPageSource ? crawlRequest.Data : new byte[] { }, crawlRequest.Encoding.CodePage, crawlRequest.DataType.FullTextIndexType, crawlRequest.CurrentDepth, ApplicationSettings.ClassifyAbsoluteUris);
            }

            if (crawlRequest.Discovery.ID.HasValue)
            {
                ManagedWebPage managedWebPage = ManageWebPage(crawlRequest.Discovery.ID.Value, crawlRequest.Discovery.Uri.AbsoluteUri, crawlRequest.Data, crawlRequest.Encoding, crawlRequest.DataType.FullTextIndexType, ApplicationSettings.ExtractWebPageMetaData, ApplicationSettings.InsertWebPageMetaData, ApplicationSettings.SaveDiscoveredWebPagesToDisk);

                crawlRequest.ManagedDiscovery = managedWebPage;
            }
        }

        /// <summary>
        /// 	Manages the web page.
        /// </summary>
        /// <param name = "webPageID">The web page ID.</param>
        /// <param name = "absoluteUri">The absolute URI.</param>
        /// <param name = "source">The source.</param>
        /// <param name = "encoding">The encoding.</param>
        /// <param name = "fullTextIndexType">Full type of the text index.</param>
        /// <param name = "extractWebPageMetaData">if set to <c>true</c> [extract web page meta data].</param>
        /// <param name = "insertWebPageMetaData">if set to <c>true</c> [insert web page meta data].</param>
        /// <param name = "saveWebPageToDisk">if set to <c>true</c> [save web page to disk].</param>
        /// <returns></returns>
        public override ManagedWebPage ManageWebPage(long webPageID, string absoluteUri, byte[] source, Encoding encoding, string fullTextIndexType, bool extractWebPageMetaData, bool insertWebPageMetaData, bool saveWebPageToDisk)
        {
            try
            {
                ManagedWebPage managedWebPage = new ManagedWebPage();

                string source2 = null;

                if (extractWebPageMetaData || saveWebPageToDisk)
                {
                    source2 = encoding.GetString(source);
                }

                if (extractWebPageMetaData)
                {
                    string source3 = HttpUtility.HtmlDecode(source2);

                    //ANODET: Enable the HtmlAgilityPack to work with bytes.
                    managedWebPage.HtmlDocument = _htmlManager.CreateHtmlDocument(source2, Encoding.Unicode);
                    managedWebPage.Tags = UserDefinedFunctions.ExtractTags(source3).Value;
                    managedWebPage.Text = UserDefinedFunctions.ExtractText(source3).Value;

                    #region Experimental Code comparing character parsing vs. regular expressions...

                    //bool inATag = false;

                    //StringBuilder stringBuilder = new StringBuilder();

                    //for (int i = 0; i < source3.Length; i++)
                    //{
                    //    if(source3[i] == '<')
                    //    {
                    //        inATag = true;
                    //        continue;
                    //    }

                    //    if (source3[i] == '>')
                    //    {
                    //        inATag = false;
                    //        continue;
                    //    }

                    //    if (!inATag && !char.IsControl(source3[i]))
                    //    {
                    //        stringBuilder.Append(source3[i]);
                    //    }
                    //}

                    //managedWebPage.Text = stringBuilder.ToString();

                    #endregion

                    if (insertWebPageMetaData)
                    {
                        _arachnodeDAO.InsertWebPageMetaData(webPageID, absoluteUri, encoding.GetBytes(managedWebPage.Text), managedWebPage.HtmlDocument.DocumentNode.OuterHtml);
                    }
                }

                if (saveWebPageToDisk)
                {
                    managedWebPage.DiscoveryPath = _discoveryManager.GetDiscoveryPath(ApplicationSettings.DownloadedWebPagesDirectory, absoluteUri, fullTextIndexType);

                    managedWebPage.StreamWriter = new StreamWriter(managedWebPage.DiscoveryPath, false, encoding);

                    managedWebPage.StreamWriter.Write(source2);
                }

                return managedWebPage;
            }
            catch (Exception exception)
            {
                //ANODET: Long paths...
#if !DEMO
                _arachnodeDAO.InsertException(absoluteUri, null, exception, false);
#endif
            }

            return null;
        }
    }
}