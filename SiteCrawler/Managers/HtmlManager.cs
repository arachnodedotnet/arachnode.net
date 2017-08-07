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
using System.Data.SqlTypes;
using System.IO;
using System.Text;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.Renderer.Value.Enums;
using Arachnode.SiteCrawler.Value.AbstractClasses;
using Arachnode.SiteCrawler.Value.Enums;
using HtmlAgilityPack;

#endregion

namespace Arachnode.SiteCrawler.Managers
{
    /// <summary>
    /// 	Provides HTML utility functionality.
    /// </summary>
    public class HtmlManager<TArachnodeDAO> : AHtmlManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        private WebSettings _webSettings;

        public HtmlManager(ApplicationSettings applicationSettings, WebSettings webSettings, DiscoveryManager<TArachnodeDAO> discoveryManager) : base(applicationSettings, webSettings, discoveryManager)
        {
            _webSettings = webSettings;
        }

        /// <summary>
        /// 	Creates the HTML document.
        /// </summary>
        /// <param name = "source">The source.</param>
        /// <param name = "encoding">The encoding.</param>
        /// <returns></returns>
        public override HtmlDocument CreateHtmlDocument(string source, Encoding encoding)
        {
            HtmlDocument htmlDocument = new HtmlDocument();

            using (MemoryStream memoryStream = new MemoryStream(encoding.GetBytes(source)))
            {
                htmlDocument.OptionAutoCloseOnEnd = true;
                htmlDocument.OptionCheckSyntax = true;
                htmlDocument.OptionFixNestedTags = true;
                htmlDocument.OptionOutputAsXml = true;
                htmlDocument.OptionOutputOptimizeAttributeValues = true;
                //htmlDocument.OptionOutputUpperCase = true;
                htmlDocument.OptionReadEncoding = false;

                htmlDocument.Load(memoryStream, encoding);

                return htmlDocument;
            }
        }

        /// <summary>
        /// 	Creates a HtmlDocument (WebPage) that references downloaded content.  If the Discovery isn't available locally, a remote (hotlinked) request is made.
        /// </summary>
        /// <param name = "webPageAbsoluteUri">The web page absolute URI.</param>
        /// <param name = "source">The source.</param>
        /// <param name = "uriQualificationType">Type of the URI qualification.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        /// <returns></returns>
        public override HtmlDocument CreateHtmlDocument(string webPageAbsoluteUri, string fullTextIndexType, string source, UriQualificationType uriQualificationType, IArachnodeDAO arachnodeDAO, bool prepareForLocalBrowsing)
        {
            HtmlDocument htmlDocument = new HtmlDocument();

            try
            {
                htmlDocument.LoadHtml(source);

                switch (uriQualificationType)
                {
                    case UriQualificationType.None:
                    case UriQualificationType.Relative:
                        break;
                    case UriQualificationType.Absolute:
                    case UriQualificationType.AbsoluteWhenDownloadedDiscoveryIsUnavailable:
                        Uri uri = new Uri(webPageAbsoluteUri);

                        QualifyNode(uri.Scheme + "://" + uri.Host + "/", fullTextIndexType, htmlDocument.DocumentNode, uriQualificationType, arachnodeDAO, prepareForLocalBrowsing);
                        break;
                }
            }
            catch (Exception exception)
            {
                arachnodeDAO.InsertException(webPageAbsoluteUri, null, exception, false);
            }

            return htmlDocument;
        }

        /// <summary>
        /// 	Creates a WebPages that references downloaded content.  If the Discovery isn't available locally, a remote (hotlinked) request is made.v
        /// </summary>
        /// <param name = "absoluteUri">The absolute URI.</param>
        /// <param name = "htmlNode">The HTML node.</param>
        /// <param name = "uriQualificationType">Type of the URI qualification.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        public override void QualifyNode(string absoluteUri, string fullTextIndexType, HtmlNode htmlNode, UriQualificationType uriQualificationType, IArachnodeDAO arachnodeDAO, bool prepareForLocalBrowsing)
        {
            if (htmlNode.HasAttributes)
            {
                foreach (HtmlAttribute htmlAttribute in htmlNode.Attributes)
                {
                    if (string.Compare(htmlAttribute.Name, "src", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(htmlAttribute.Name, "href", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        Uri uri;
                        if (Uri.TryCreate(htmlAttribute.Value, UriKind.RelativeOrAbsolute, out uri))
                        {
                            if (!uri.IsAbsoluteUri)
                            {
                                Uri.TryCreate(absoluteUri + uri.OriginalString, UriKind.Absolute, out uri);
                            }
                        }

                        //remove double "//"...
                        uri = new Uri(uri.Scheme + "://" + uri.AbsoluteUri.Replace(uri.Scheme + "://", string.Empty).Replace("//", "/").TrimEnd("/".ToCharArray()));

                        if (uri.IsAbsoluteUri)
                        {
                            string downloadedFileDiscoveryPath = _discoveryManager.GetDiscoveryPath(ApplicationSettings.DownloadedFilesDirectory, uri.AbsoluteUri, string.Empty);
                            string downloadedImageDiscoveryPath = _discoveryManager.GetDiscoveryPath(ApplicationSettings.DownloadedImagesDirectory, uri.AbsoluteUri, string.Empty);
                            string downloadedWebPageDiscoveryPath = _discoveryManager.GetDiscoveryPath(ApplicationSettings.DownloadedWebPagesDirectory, uri.AbsoluteUri, string.Empty);

                            bool doesDownloadedFileDiscoveryExist = false;
                            bool doesDownloadedImageDiscoveryExist = false;
                            bool doesDownloadedWebPageDiscoveryExist = false;

                            if (prepareForLocalBrowsing)
                            {
                                if (htmlNode.Name == "applet" || htmlNode.Name == "embed" || htmlNode.Name == "img")
                                {
                                    if (!_discoveryManager.DoesDiscoveryExist(downloadedImageDiscoveryPath))
                                    {
                                        htmlNode.ParentNode.InnerHtml = "<span class=\"undiscoveredDiscovery\">" + htmlNode.OuterHtml + "</span>";
                                    }
                                    else
                                    {
                                        doesDownloadedImageDiscoveryExist = true;

                                        htmlNode.ParentNode.InnerHtml = "<span class=\"discoveredDiscovery\">" + htmlNode.OuterHtml + "</span>";
                                    }
                                }

                                if (htmlNode.Name == "link" || htmlNode.Name == "script")
                                {
                                    //favicons are referenced by <link> but are images... (thus the check in the images path...)
                                    if (!_discoveryManager.DoesDiscoveryExist(downloadedFileDiscoveryPath) && !_discoveryManager.DoesDiscoveryExist(downloadedImageDiscoveryPath))
                                    {
                                        htmlNode.OwnerDocument.DocumentNode.InnerHtml = "<span class=\"undiscoveredDiscovery\">Undiscovered: " + uri.AbsoluteUri + "</span>" + htmlNode.OwnerDocument.DocumentNode.InnerHtml;
                                    }
                                    else
                                    {
                                        doesDownloadedFileDiscoveryExist = true;
                                    }
                                }

                                if (htmlNode.Name == "a")
                                {
                                    if (!_discoveryManager.DoesDiscoveryExist(downloadedWebPageDiscoveryPath))
                                    {
                                        htmlNode.InnerHtml = "<span class=\"undiscoveredDiscovery\">" + htmlNode.InnerHtml + "</span>";
                                    }
                                    else
                                    {
                                        htmlNode.InnerHtml = "<span class=\"discoveredDiscovery\">" + htmlNode.InnerHtml + "</span>";

                                        doesDownloadedWebPageDiscoveryExist = true;
                                    }
                                }
                            }

                            string discoveryExtension = null;

                            if (uriQualificationType == UriQualificationType.AbsoluteWhenDownloadedDiscoveryIsUnavailable)
                            {
                                if (doesDownloadedFileDiscoveryExist || _discoveryManager.DoesDiscoveryExist(downloadedFileDiscoveryPath))
                                {
                                    discoveryExtension = _discoveryManager.GetDiscoveryExtension(downloadedFileDiscoveryPath);

                                    if (!string.IsNullOrEmpty(discoveryExtension))
                                    {
                                        htmlAttribute.Value = _discoveryManager.GetDiscoveryPath(_webSettings.DownloadedFilesVirtualDirectory, downloadedFileDiscoveryPath.Replace(ApplicationSettings.DownloadedFilesDirectory, _webSettings.DownloadedFilesVirtualDirectory) + discoveryExtension);
                                    }
                                    return;
                                }

                                if (doesDownloadedImageDiscoveryExist || _discoveryManager.DoesDiscoveryExist(downloadedImageDiscoveryPath))
                                {
                                    discoveryExtension = _discoveryManager.GetDiscoveryExtension(downloadedImageDiscoveryPath);

                                    if (!string.IsNullOrEmpty(discoveryExtension))
                                    {
                                        htmlAttribute.Value = _discoveryManager.GetDiscoveryPath(_webSettings.DownloadedImagesVirtualDirectory, downloadedImageDiscoveryPath.Replace(ApplicationSettings.DownloadedImagesDirectory, _webSettings.DownloadedImagesVirtualDirectory) + discoveryExtension);
                                    }
                                    return;
                                }

                                if (prepareForLocalBrowsing)
                                {
                                    if (doesDownloadedWebPageDiscoveryExist)
                                    {
                                        htmlAttribute.Value = "/Browse.aspx?absoluteUri=" + uri.AbsoluteUri;
                                        return;
                                    }
                                }

                                if (_webSettings.CreateCrawlRequestsForMissingFilesAndImages)
                                {
                                    if (ApplicationSettings.InsertCrawlRequests)
                                    {
                                        arachnodeDAO.InsertCrawlRequest(SqlDateTime.MinValue.Value, null, absoluteUri, uri.AbsoluteUri, 1, 1, (short)UriClassificationType.Host, (short)UriClassificationType.Host, Double.MaxValue, (byte)RenderType.None, (byte)RenderType.None);
                                    }
                                }
                            }

                            htmlAttribute.Value = uri.AbsoluteUri;
                        }
                    }
                }
            }

            for (int i = 0; i < htmlNode.ChildNodes.Count; i++)
            {
                HtmlNode htmlNode2 = htmlNode.ChildNodes[i];

                QualifyNode(absoluteUri, fullTextIndexType, htmlNode2, uriQualificationType, arachnodeDAO, prepareForLocalBrowsing);
            }
        }

        /// <summary>
        /// 	Get the AbsoluteUri for an Image.  The image Image be on disk, or we may need to hotlink to the Image.  The method is used by the search display facilities.
        /// </summary>
        /// <param name = "absoluteUri"></param>
        /// <param name = "arachnodeDAO"></param>
        /// <returns></returns>
        public override string GetImageUrl(string absoluteUri, string fullTextIndexType, IArachnodeDAO arachnodeDAO)
        {
            //ANODET: 2.5+ Add a property specification for hotlinking.  Also, many hotlinked images may not render.  Add a setting to omit images that can't be previewed.
            string discoveryPath = _discoveryManager.GetDiscoveryPath(ApplicationSettings.DownloadedImagesDirectory, absoluteUri, fullTextIndexType);

            if (!File.Exists(discoveryPath))
            {
                discoveryPath = absoluteUri;

                if (_webSettings.CreateCrawlRequestsForMissingFilesAndImages)
                {
                    //arachnodeDAO.InsertCrawlRequest(SqlDateTime.MinValue.Value, absoluteUri, absoluteUri, 1, 1, (short)UriClassificationType.Host, (short)UriClassificationType.Host, Double.MaxValue);
                }
            }

            return discoveryPath.ToLower().Replace(ApplicationSettings.DownloadedImagesDirectory.ToLower(), _webSettings.DownloadedImagesVirtualDirectory);
        }

        /// <summary>
        /// 	Get the AbsoluteUri for a File.  The File could be on disk, or we may need to hotlink to the File.  The method is used by the search display facilities.
        /// </summary>
        /// <param name = "absoluteUri"></param>
        /// <param name = "arachnodeDAO"></param>
        /// <returns></returns>
        public override string GetFileUrl(string absoluteUri, string fullTextIndexType, IArachnodeDAO arachnodeDAO)
        {
            //ANODET: 2.5+ Add a property specification for hotlinking.  Also, many hotlinked files may not render.  Add a setting to omit files that can't be previewed.
            string discoveryPath = _discoveryManager.GetDiscoveryPath(ApplicationSettings.DownloadedFilesDirectory, absoluteUri, fullTextIndexType);

            if (!File.Exists(discoveryPath))
            {
                discoveryPath = absoluteUri;

                if (_webSettings.CreateCrawlRequestsForMissingFilesAndImages)
                {
                    //arachnodeDAO.InsertCrawlRequest(SqlDateTime.MinValue.Value, null, absoluteUri, absoluteUri, 1, 1, (short)UriClassificationType.Host, (short)UriClassificationType.Host, Double.MaxValue, (byte)RenderType.None, (byte)RenderType.None);
                }
            }

            return discoveryPath.ToLower().Replace(ApplicationSettings.DownloadedFilesDirectory.ToLower(), _webSettings.DownloadedFilesVirtualDirectory);
        }
    }
}