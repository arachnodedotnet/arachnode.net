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
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.Renderer.Value.Enums;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.AbstractClasses;
using Arachnode.SiteCrawler.Value.Enums;

#endregion

namespace Arachnode.SiteCrawler.Managers
{
    public class EncodingManager<TArachnodeDAO> : AEncodingManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        public EncodingManager(ApplicationSettings applicationSettings, WebSettings webSettings) : base(applicationSettings, webSettings)
        {
            
        }

        public override void ProcessCrawlRequest(CrawlRequest<TArachnodeDAO> crawlRequest, IArachnodeDAO arachnodeDAO)
        {
            //Rendering determines the Encoding...
            if (crawlRequest.RenderType == RenderType.None)
            {
                if (crawlRequest.DataType.DiscoveryType == DiscoveryType.WebPage)
                {
                    string contentType = null;
                    if (crawlRequest.WebClient.HttpWebResponse.Headers["Content-Type"] != null)
                    {
                        string[] contentTypeHeader = crawlRequest.WebClient.HttpWebResponse.Headers["Content-Type"].Split('=');

                        if (contentTypeHeader.Length == 2)
                        {
                            contentType = contentTypeHeader[1].Replace("utf8", "utf-8");
                        }
                    }

                    Encoding encoding = null;
                    string decodedHtml = null;

                    try
                    {
                        //first, try and get the Encoding from the 'Content-Type'...
                        if (!string.IsNullOrEmpty(contentType))
                        {
                            encoding = Encoding.GetEncoding(contentType);
                        }
                        else
                        {
                            decodedHtml = DetermineEncoding(crawlRequest, out encoding);
                        }
                    }
                    catch (Exception exception)
                    {
                        try
                        {
                            //if there is an error, try and get the Encoding from the 'Charset'...
                            decodedHtml = DetermineEncoding(crawlRequest, out encoding);
                        }
                        catch (Exception exception2)
                        {
                            //if there is an error, default to UTF8.
                            arachnodeDAO.InsertException(crawlRequest.Discovery.Uri.AbsoluteUri, null, exception, false);
                            arachnodeDAO.InsertException(crawlRequest.Discovery.Uri.AbsoluteUri, null, exception2, false);

                            encoding = Encoding.UTF8;
                        }
                    }

                    crawlRequest.Encoding = encoding;

                    if (encoding == Encoding.UTF8 && decodedHtml != null)
                    {
                        crawlRequest.DecodedHtml = HttpUtility.HtmlDecode(decodedHtml);
                        crawlRequest.Html = decodedHtml;
                    }
                    else
                    {
                        crawlRequest.DecodedHtml = HttpUtility.HtmlDecode(encoding.GetString(crawlRequest.Data));
                        crawlRequest.Html = encoding.GetString(crawlRequest.Data);
                    }
                }
            }
        }

        private string DetermineEncoding(CrawlRequest<TArachnodeDAO> crawlRequest, out Encoding encoding)
        {
            string decodedHtml = Encoding.UTF8.GetString(crawlRequest.Data);

            Match match = _charsetRegex.Match(decodedHtml);

            if (match.Success)
            {
                string value = match.Groups["Charset"].Value;

                if (string.IsNullOrEmpty(value) || value.ToLowerInvariant().StartsWith("utf-8") || value.ToLowerInvariant().StartsWith("utf8"))
                {
                    encoding = Encoding.UTF8;
                }
                else
                {
                    encoding = Encoding.GetEncoding(value);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(crawlRequest.WebClient.HttpWebResponse.CharacterSet))
                {
                    encoding = Encoding.GetEncoding(crawlRequest.WebClient.HttpWebResponse.CharacterSet);
                }
                else
                {
                    encoding = Encoding.UTF8;
                }
            }

            return decodedHtml;
        }
    }
}