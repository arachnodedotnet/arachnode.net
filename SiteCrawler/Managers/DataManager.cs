#region License : arachnode.net

// Copyright (c) 2015 http://arachnode.net, arachnode.net, LLC
//  
// Permission is hereby granted, upon purchase, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, merge and modify copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following
// conditions:
// 
// LICENSE (ALL VERSIONS/EDITIONS): http://arachnode.net/r.ashx?3
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using System;
using System.Data.SqlTypes;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.DataSource;
using Arachnode.Renderer.Value;
using Arachnode.Renderer.Value.Enums;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.AbstractClasses;
using Arachnode.SiteCrawler.Value.Enums;
using mshtml;

#endregion

namespace Arachnode.SiteCrawler.Managers
{
    public class DataManager<TArachnodeDAO> : ADataManager<TArachnodeDAO> where TArachnodeDAO: IArachnodeDAO
    {
        public DataManager(ApplicationSettings applicationSettings, WebSettings webSettings, ActionManager<TArachnodeDAO> actionManager, DataTypeManager<TArachnodeDAO> dataTypeManager, DiscoveryManager<TArachnodeDAO> discoveryManager, RuleManager<TArachnodeDAO> ruleManager, IArachnodeDAO arachnodeDAO) : base(applicationSettings, webSettings, actionManager, dataTypeManager, discoveryManager, ruleManager, arachnodeDAO)
        {
        }

        public override void ProcessCrawlRequest(CrawlRequest<TArachnodeDAO> crawlRequest, bool obeyCrawlRules, bool executeCrawlActions)
        {
            IssueWebRequest(crawlRequest, "GET");

            crawlRequest.DataType = _dataTypeManager.DetermineDataType(crawlRequest);

            if (obeyCrawlRules)
            {
                _ruleManager.IsDisallowed(crawlRequest, CrawlRuleType.PreGet, _arachnodeDAO);
            }

            if (executeCrawlActions)
            {
                _actionManager.PerformCrawlActions(crawlRequest, CrawlActionType.PreGet, _arachnodeDAO);
            }

            if (!crawlRequest.IsDisallowed)
            {
                try
                {
                    if (crawlRequest.WebClient.HttpWebResponse != null)
                    {
                        crawlRequest.ProcessData = true;

                        bool isLastModifiedOutdated = true;

                        try
                        {
                            isLastModifiedOutdated = crawlRequest.WebClient.HttpWebResponse.LastModified != DateTime.Now;
                        }
                        catch(Exception exception)
                        {
                            _arachnodeDAO.InsertException(crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri, exception, false);
                        }

                        if (isLastModifiedOutdated)
                        {
                            switch (crawlRequest.DataType.DiscoveryType)
                            {
                                case DiscoveryType.File:
                                    if (ApplicationSettings.AssignFileAndImageDiscoveries) //ANODET: robots.txt
                                    {
                                        ArachnodeDataSet.FilesRow filesRow = _arachnodeDAO.GetFile(crawlRequest.Discovery.Uri.AbsoluteUri);

                                        if (filesRow == null)
                                        {
                                            crawlRequest.ProcessData = true;
                                        }
                                        else
                                        {
                                            if (!filesRow.IsResponseHeadersNull())
                                            {
                                                DateTime lastModified;

                                                SqlString lastModifiedValue = UserDefinedFunctions.ExtractResponseHeader(filesRow.ResponseHeaders, "Last-Modified: ", false);

                                                if (!lastModifiedValue.IsNull && DateTime.TryParse(lastModifiedValue.Value, out lastModified))
                                                {
                                                    //crawlRequest.WebClient.HttpWebResponse.LastModified will equal DateTime.Now (or close to it) if the 'Last-Modified' ResponseHeader is not present...
                                                    if ((crawlRequest.WebClient.HttpWebResponse).LastModified > lastModified)
                                                    {
                                                        crawlRequest.ProcessData = true;
                                                    }
                                                    else
                                                    {
                                                        crawlRequest.ProcessData = false;
                                                    }
                                                }
                                                else
                                                {
                                                    crawlRequest.ProcessData = false;
                                                }
                                            }
                                            else
                                            {
                                                crawlRequest.ProcessData = true;
                                            }

                                            if (!crawlRequest.ProcessData)
                                            {
                                                if (filesRow.Source.Length != 0)
                                                {
                                                    crawlRequest.Data = filesRow.Source;
                                                }
                                                else
                                                {
                                                    string discoveryPath = _discoveryManager.GetDiscoveryPath(ApplicationSettings.DownloadedFilesDirectory, crawlRequest.Discovery.Uri.AbsoluteUri, crawlRequest.DataType.FullTextIndexType);

                                                    if (File.Exists(discoveryPath))
                                                    {
                                                        crawlRequest.Data = File.ReadAllBytes(discoveryPath);
                                                    }
                                                    else
                                                    {
                                                        try
                                                        {
                                                            throw new Exception("The 'LastModified' HttpResponse Header indicated that the Data was not stale, but the Data (Source) could not be found in the Files database table or at _applicationSettings.DownloadedFilesDirectory.  Therefore, the data was re-downloaded from the server.  The File file may have been deleted from disk or the 'Source' column in the 'Files' table may have been cleared or a previous crawl may have crawled with both _applicationSettings.InsertFileSource = false and _applicationSettings.SaveDiscoveredFilesToDisk = false.");
                                                        }
                                                        catch (Exception exception)
                                                        {
                                                            _arachnodeDAO.InsertException(crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri, exception, false);
                                                        }

                                                        crawlRequest.ProcessData = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        crawlRequest.ProcessData = false;
                                    }
                                    break;
                                case DiscoveryType.Image:
                                    if (ApplicationSettings.AssignFileAndImageDiscoveries)
                                    {
                                        ArachnodeDataSet.ImagesRow imagesRow = _arachnodeDAO.GetImage(crawlRequest.Discovery.Uri.AbsoluteUri);

                                        if (imagesRow == null)
                                        {
                                            crawlRequest.ProcessData = true;
                                        }
                                        else
                                        {
                                            if (!imagesRow.IsResponseHeadersNull())
                                            {
                                                DateTime lastModified;

                                                SqlString lastModifiedValue = UserDefinedFunctions.ExtractResponseHeader(imagesRow.ResponseHeaders, "Last-Modified: ", false);

                                                if (!lastModifiedValue.IsNull && DateTime.TryParse(lastModifiedValue.Value, out lastModified))
                                                {
                                                    //crawlRequest.WebClient.HttpWebResponse.LastModified will equal DateTime.Now (or close to it) if the 'Last-Modified' ResponseHeader is not present...
                                                    if (crawlRequest.WebClient.HttpWebResponse.LastModified > lastModified)
                                                    {
                                                        crawlRequest.ProcessData = true;
                                                    }
                                                    else
                                                    {
                                                        crawlRequest.ProcessData = false;
                                                    }
                                                }
                                                else
                                                {
                                                    crawlRequest.ProcessData = false;
                                                }

                                                if (!crawlRequest.ProcessData)
                                                {
                                                    if (imagesRow.Source.Length != 0)
                                                    {
                                                        crawlRequest.Data = imagesRow.Source;
                                                    }
                                                    else
                                                    {
                                                        string discoveryPath = _discoveryManager.GetDiscoveryPath(ApplicationSettings.DownloadedImagesDirectory, crawlRequest.Discovery.Uri.AbsoluteUri, crawlRequest.DataType.FullTextIndexType);

                                                        if (File.Exists(discoveryPath))
                                                        {
                                                            crawlRequest.Data = File.ReadAllBytes(discoveryPath);
                                                        }
                                                        else
                                                        {
                                                            try
                                                            {
                                                                throw new Exception("The 'LastModified' HttpResponse Header indicated that the Data was not stale, but the Data (Source) could not be found in the Images database table or at _applicationSettings.DownloadedImagesDirectory.  Therefore, the data was downloaded from the server.  The Image file may have been deleted from disk or the 'Source' column in the 'Images' table may have been cleared.  A previous crawl may have crawled with both _applicationSettings.InsertImageSource = false and _applicationSettings.SaveDiscoveredImagesToDisk = false.");
                                                            }
                                                            catch (Exception exception)
                                                            {
                                                                _arachnodeDAO.InsertException(crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri, exception, false);
                                                            }

                                                            crawlRequest.ProcessData = true;
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                crawlRequest.ProcessData = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        crawlRequest.ProcessData = false;
                                    }
                                    break;
                                case DiscoveryType.WebPage:
                                    ArachnodeDataSet.WebPagesRow webPagesRow = _arachnodeDAO.GetWebPage(crawlRequest.Discovery.Uri.AbsoluteUri);

                                    if (webPagesRow == null)
                                    {
                                        crawlRequest.ProcessData = true;
                                    }
                                    else
                                    {
                                        if ((crawlRequest.WebClient.HttpWebResponse).LastModified > webPagesRow.LastDiscovered)
                                        {
                                            crawlRequest.ProcessData = true;
                                        }
                                        else
                                        {
                                            crawlRequest.ProcessData = false;
                                        }

                                        if (!crawlRequest.ProcessData)
                                        {
                                            if (webPagesRow.Source.Length != 0)
                                            {
                                                crawlRequest.Data = webPagesRow.Source;
                                            }
                                            else
                                            {
                                                string discoveryPath = _discoveryManager.GetDiscoveryPath(ApplicationSettings.DownloadedWebPagesDirectory, crawlRequest.Discovery.Uri.AbsoluteUri, crawlRequest.DataType.FullTextIndexType);

                                                if (File.Exists(discoveryPath))
                                                {
                                                    crawlRequest.Data = File.ReadAllBytes(discoveryPath);
                                                }
                                                else
                                                {
                                                    try
                                                    {
                                                        throw new Exception("The 'LastModified' HttpResponse Header indicated that the Data was not stale, but the Data (Source) could not be found in the WebPages database table or at _applicationSettings.DownloadedWebPagesDirectory.  Therefore, the data was re-downloaded from the server.  The WebPage file may have been deleted from disk or the 'Source' column in the 'WebPages' table may have been cleared or a previous crawl may have crawled with both _applicationSettings.InsertWebPageSource = false and _applicationSettings.SaveDiscoveredWebPagesToDisk = false.");
                                                    }
                                                    catch (Exception exception)
                                                    {
                                                        _arachnodeDAO.InsertException(crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri, exception, false);
                                                    }

                                                    crawlRequest.ProcessData = true;
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case DiscoveryType.None:
                                    crawlRequest.ProcessData = true;
                                    break;
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    _arachnodeDAO.InsertException(crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri, exception, false);
                }

                if (crawlRequest.ProcessData)
                {
                    if(crawlRequest.Data != null)
                    {
                        
                    }

                    if (crawlRequest.RenderType == RenderType.None)
                    {
                        if (crawlRequest.Discovery.Uri.Scheme.ToLowerInvariant() != "ftp")
                        {
                            if (crawlRequest.WebClient.HttpWebResponse != null && crawlRequest.WebClient.HttpWebResponse.Method == "HEAD")
                            {
                                IssueWebRequest(crawlRequest, "GET");
                            }

                            if (crawlRequest.WebClient.HttpWebResponse != null)
                            {
                                crawlRequest.Data = crawlRequest.WebClient.DownloadHttpData(crawlRequest.Discovery.Uri.AbsoluteUri, crawlRequest.WebClient.HttpWebResponse.ContentEncoding.ToLowerInvariant() == "gzip", crawlRequest.WebClient.HttpWebResponse.ContentEncoding.ToLowerInvariant() == "deflate", crawlRequest.Crawl.Crawler.CookieContainer);
                            }
                        }
                        else
                        {
                            crawlRequest.Data = crawlRequest.WebClient.DownloadFtpData(crawlRequest.Discovery.Uri.AbsoluteUri);
                        }
                    }
                    else
                    {
                        RendererResponse rendererResponse = crawlRequest.Crawl.Crawler.Engine.Render(crawlRequest, RenderAction.Render, crawlRequest.RenderType);

                        if (rendererResponse != null)
                        {
                            if (rendererResponse.HTMLDocumentClass != null)
                            {
                                crawlRequest.Encoding = Encoding.GetEncoding(rendererResponse.HTMLDocumentClass.charset);

                                string outerHTML = rendererResponse.HTMLDocumentClass.documentElement.outerHTML;

                                crawlRequest.Data = crawlRequest.Encoding.GetBytes(outerHTML);
                                crawlRequest.DecodedHtml = HttpUtility.HtmlDecode(outerHTML);
                                crawlRequest.Html = outerHTML;
                                crawlRequest.HtmlDocument = rendererResponse.HTMLDocumentClass;
                            }

                            crawlRequest.RendererMessage = rendererResponse.RendererMessage;
                        }
                    }
                }
            }
            else
            {
                if (crawlRequest.Data == null)
                {

                }
            }

            if (crawlRequest.Data == null)
            {
                crawlRequest.Data = new byte[0];
            }
        }

        protected override void IssueWebRequest(CrawlRequest<TArachnodeDAO> crawlRequest, string method)
        {
            try
            {
                if (crawlRequest.Discovery.Uri.Scheme.ToLowerInvariant() != "ftp")
                {
                    if (ApplicationSettings.SetRefererToParentAbsoluteUri)
                    {
                        crawlRequest.WebClient.GetHttpWebResponse(crawlRequest.Discovery.Uri.AbsoluteUri, method, crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Crawl.Crawler.CredentialCache, crawlRequest.Crawl.Crawler.CookieContainer, crawlRequest.Crawl.Crawler.Proxy);
                    }
                    else
                    {
                        crawlRequest.WebClient.GetHttpWebResponse(crawlRequest.Discovery.Uri.AbsoluteUri, method, null, crawlRequest.Crawl.Crawler.CredentialCache, crawlRequest.Crawl.Crawler.CookieContainer, crawlRequest.Crawl.Crawler.Proxy);
                    }
                }
                else
                {
                    crawlRequest.WebClient.GetFtpWebResponse(crawlRequest.Discovery.Uri.AbsoluteUri, "GET", crawlRequest.Crawl.Crawler.CredentialCache, crawlRequest.Crawl.Crawler.Proxy);
                }
            }
            catch (WebException webException)
            {
                throw new WebException(webException.Message, crawlRequest.WebClient.WebException);
            }
        }
    }
}