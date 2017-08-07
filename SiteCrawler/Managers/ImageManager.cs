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
using System.Drawing;
using System.IO;
using System.Web.UI;
using System.Xml;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.AbstractClasses;
using Arachnode.Utilities.EXIF;

#endregion

namespace Arachnode.SiteCrawler.Managers
{
    public class ImageManager<TArachnodeDAO> : AImageManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        /// <summary>
        /// 	Initializes a new instance of the <see cref = "ImageManager" /> class.
        /// </summary>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        public ImageManager(ApplicationSettings applicationSettings, WebSettings webSettings, DiscoveryManager<TArachnodeDAO> discoveryManager, IArachnodeDAO arachnodeDAO) : base(applicationSettings, webSettings, discoveryManager, arachnodeDAO)
        {
        }

        /// <summary>
        /// 	Manages the image.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        public override void ManageImage(CrawlRequest<TArachnodeDAO> crawlRequest)
        {
            //we want to prevent files and images from being created by bugs when we haven't explicitly allowed file and image crawling,
            //but want to allow specific requests for file and image AbsoluteUris...
            if (ApplicationSettings.AssignFileAndImageDiscoveries || crawlRequest.Discovery.Uri.AbsoluteUri == crawlRequest.Parent.Uri.AbsoluteUri)
            {
                if (ApplicationSettings.InsertImages && crawlRequest.IsStorable)
                {
                    crawlRequest.Discovery.ID = _arachnodeDAO.InsertImage(crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri, ApplicationSettings.InsertImageResponseHeaders ? crawlRequest.WebClient.HttpWebResponse.Headers.ToString() : null, ApplicationSettings.InsertImageSource ? crawlRequest.Data : new byte[] {}, crawlRequest.DataType.FullTextIndexType);
                }

                if (crawlRequest.Discovery.ID.HasValue)
                {
                    ManagedImage managedImage = ManageImage(crawlRequest, crawlRequest.Discovery.ID.Value, crawlRequest.Discovery.Uri.AbsoluteUri, crawlRequest.Data, Path.GetExtension(crawlRequest.WebClient.HttpWebResponse.ResponseUri.AbsolutePath), ApplicationSettings.ExtractImageMetaData, ApplicationSettings.InsertImageMetaData, ApplicationSettings.SaveDiscoveredImagesToDisk);

                    crawlRequest.ManagedDiscovery = managedImage;
                }
            }
        }

        /// <summary>
        /// 	Manages the image.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "imageID">The image ID.</param>
        /// <param name = "absoluteUri">The absolute URI.</param>
        /// <param name = "source">The source.</param>
        /// <param name = "fullTextIndexType">Full type of the text index.</param>
        /// <param name = "extractImageMetaData">if set to <c>true</c> [extract image meta data].</param>
        /// <param name = "insertImageMetaData">if set to <c>true</c> [insert image meta data].</param>
        /// <param name = "saveImageToDisk">if set to <c>true</c> [save image to disk].</param>
        /// <returns></returns>
        public override ManagedImage ManageImage(CrawlRequest<TArachnodeDAO> crawlRequest, long imageID, string absoluteUri, byte[] source, string fullTextIndexType, bool extractImageMetaData, bool insertImageMetaData, bool saveImageToDisk)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream(source, true))
                {
                    ManagedImage managedImage = new ManagedImage();

                    managedImage.Image = Image.FromStream(memoryStream);

                    if (extractImageMetaData)
                    {
                        XmlDocument xmlDocument = new XmlDocument();
                        XmlElement xmlElement;

                        xmlDocument.AppendChild(xmlDocument.CreateNode(XmlNodeType.XmlDeclaration, "", ""));
                        xmlDocument.AppendChild(xmlDocument.CreateElement("", "EXIFData", ""));

                        Dictionary<string, string> dictionary = new Dictionary<string, string>();

                        foreach (Pair pair in new EXIFExtractor(managedImage.Image, "", ""))
                        {
                            dictionary.Add(pair.First.ToString(), pair.Second.ToString());
                        }

                        foreach (KeyValuePair<string, string> keyValuePair in dictionary)
                        {
                            xmlElement = xmlDocument.CreateElement("", keyValuePair.Key.Replace(" ", "_"), "");

                            string value = UserDefinedFunctions.ExtractAlphaNumericCharacters(keyValuePair.Value).Value ?? string.Empty;

                            xmlElement.AppendChild(xmlDocument.CreateTextNode(value));

                            xmlDocument.ChildNodes.Item(1).AppendChild(xmlElement);
                        }

                        managedImage.EXIFData = xmlDocument;

                        if (insertImageMetaData)
                        {
                            _arachnodeDAO.InsertImageMetaData(absoluteUri, imageID, xmlDocument.InnerXml, managedImage.Image.Flags, managedImage.Image.Height, managedImage.Image.HorizontalResolution, managedImage.Image.VerticalResolution, managedImage.Image.Width);
                        }
                    }

                    if (saveImageToDisk)
                    {
                        managedImage.DiscoveryPath = _discoveryManager.GetDiscoveryPath(ApplicationSettings.DownloadedImagesDirectory, absoluteUri, fullTextIndexType);

                        managedImage.Image.Save(managedImage.DiscoveryPath);
                    }

                    return managedImage;
                } //ANODET: Parameter is not valid in the exception handler...
            }
            catch (Exception exception)
            {
                //ANODET: Images of 7 bytes (Generic GDI Error)...
#if !DEMO
                if (crawlRequest != null)
                {
                    _arachnodeDAO.InsertException(crawlRequest.Parent.Uri.AbsoluteUri, absoluteUri, exception, false);
                }
                else
                {
                    _arachnodeDAO.InsertException(null, absoluteUri, exception, false);
                }
#endif
            }

            return null;
        }
    }
}