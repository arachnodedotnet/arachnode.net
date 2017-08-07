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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Managers;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.AbstractClasses;
using Arachnode.SiteCrawler.Value.Enums;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

#endregion

namespace Arachnode.Plugins.CrawlActions
{
    public class Templater<TArachnodeDAO> : ACrawlAction<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        private static readonly HashSet<string> _allowedTagNames = new HashSet<string>(new[] {"div", "p", "td", "span"});
        private static readonly HashSet<string> _headerTagNames = new HashSet<string>(new[] {"h1", "h2", "h3", "h4", "h5", "h6"});

        public Templater(ApplicationSettings applicationSettings, WebSettings webSettings)
            : base(applicationSettings, webSettings)
        {
        }

        /// <summary>
        /// 	Assigns the additional parameters.
        /// </summary>
        /// <param name = "settings"></param>
        public override void AssignSettings(Dictionary<string, string> settings)
        {
        }

        /// <summary>
        /// 	Performs the action.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        public override void PerformAction(CrawlRequest<TArachnodeDAO> crawlRequest, IArachnodeDAO arachnodeDAO)
        {
            //use this instead: http://code.google.com/p/boilerpipe/

            /*if (!crawlRequest.ProcessData)
            {
                return;
            }*/

            if (crawlRequest.DataType.DiscoveryType == DiscoveryType.WebPage)
            {
                if (crawlRequest.Data != null)
                {
                    ManagedWebPage managedWebPage = ((ManagedWebPage) crawlRequest.ManagedDiscovery);

                    if (managedWebPage.HtmlDocument == null)
                    {
                        managedWebPage.HtmlDocument = crawlRequest.Crawl.Crawler.HtmlManager.CreateHtmlDocument(crawlRequest.Html, Encoding.Unicode);
                    }

                    IDictionary<string, XPathInfo> xPathInfos = new Dictionary<string, XPathInfo>();

                    xPathInfos = GenerateXPaths(managedWebPage.HtmlDocument.DocumentNode, string.Empty, xPathInfos);

                    //string dateXPath = ExtractDateXPath(htmlDocument1, xpathInfos);

                    //List<string> dates = htmlDocument1.DocumentNode.SelectNodes(dateXPath).OfType<HtmlNode>().Select(h => h.InnerText).ToList();

                    ProcessXPaths(xPathInfos);

                    List<XPathInfo> xPathInfos2 = xPathInfos.Values.OrderByDescending(x => x.LevenstheinDistance).ToList();

                    int numberOfSlashes = 0;

                    IDictionary<string, XPathInfo> xPathInfos3 = new Dictionary<string, XPathInfo>();

                    int xPaths = 0;
                    int minimumNumberOfXPaths = 5;

                    foreach (XPathInfo xPathInfo in xPathInfos2)
                    {
                        int numberOfSlashes2 = xPathInfo.XPath.Length - xPathInfo.XPath.Replace("/", string.Empty).Length;

                        if (numberOfSlashes2 > numberOfSlashes)
                        {
                            numberOfSlashes = numberOfSlashes2;

                            xPathInfos3.Add(xPathInfo.XPath, xPathInfo);
                        }
                        else
                        {
                            if (xPaths++ > minimumNumberOfXPaths)
                            {
                                break;
                            }
                        }
                    }

                    StringBuilder stringBuilder = new StringBuilder();

                    Dictionary<string, XPathInfo> dictionary = new Dictionary<string, XPathInfo>();

                    foreach (XPathInfo xPathInfo in xPathInfos3.Values)
                    {
                        //stringBuilder.Remove(0, stringBuilder.Length);

                        foreach (HtmlNode htmlNode in managedWebPage.HtmlDocument.DocumentNode.SelectNodes(xPathInfo.XPath))
                        {
                            string text = UserDefinedFunctions.ExtractText(htmlNode.InnerHtml).Value;

                            if (!dictionary.ContainsKey(text))
                            {
                                XPathInfo xPathInfo2 = new XPathInfo();

                                xPathInfo2.XPath = xPathInfo.XPath;

                                dictionary.Add(text, xPathInfo2);
                            }

                            dictionary[text].Count++;
                        }
                    }

                    Dictionary<string, XPathInfo> dictionary2 = new Dictionary<string, XPathInfo>();

                    foreach (KeyValuePair<string, XPathInfo> keyValuePair in dictionary)
                    {
                        if (!string.IsNullOrEmpty(keyValuePair.Key.Trim()))
                        {
                            dictionary2.Add(keyValuePair.Key, keyValuePair.Value);
                        }
                    }

                    foreach (string key in dictionary.Keys)
                    {
                        foreach (string key2 in dictionary.Keys)
                        {
                            if (!string.IsNullOrEmpty(key.Trim()) && !string.IsNullOrEmpty(key2.Trim()))
                            {
                                if (key.Contains(key2) || key2.Contains(key))
                                {
                                    dictionary2[key].Count++;
                                    dictionary2[key2].Count++;
                                }
                            }
                        }
                    }

                    int dictionary2Max = dictionary2.Max(d => d.Value.Count);

                    foreach (KeyValuePair<string, XPathInfo> keyValuePair in dictionary2)
                    {
                        if (keyValuePair.Value.Count == dictionary2Max)
                        {
                            foreach (HtmlNode htmlNode in managedWebPage.HtmlDocument.DocumentNode.SelectNodes(keyValuePair.Value.XPath))
                            {
                                stringBuilder.Append(UserDefinedFunctions.ExtractText(htmlNode.InnerHtml).Value);
                            }
                        }
                    }

                    MessageBox.Show(stringBuilder.ToString());

                    //return stringBuilder.ToString();
                }
            }
        }

        private static void ProcessXPaths(IDictionary<string, XPathInfo> xpathInfos)
        {
            foreach (XPathInfo xPathInfo1 in xpathInfos.Values)
            {
                foreach (XPathInfo xPathInfo2 in xpathInfos.Values)
                {
                    try
                    {
                        if (xPathInfo1.InnerText != xPathInfo2.InnerText)
                        {
                            int maximumNumberOfCharatersToEvaluate = xPathInfo1.InnerText.Length > xPathInfo2.InnerText.Length ? xPathInfo2.InnerText.Length : xPathInfo1.InnerText.Length;

                            maximumNumberOfCharatersToEvaluate = maximumNumberOfCharatersToEvaluate > 1000 ? 1000 : maximumNumberOfCharatersToEvaluate;

                            int levenstheinDistance = UserDefinedFunctions.ComputeLevenstheinDistance(xPathInfo1.InnerText.Substring(0, maximumNumberOfCharatersToEvaluate), xPathInfo2.InnerText.Substring(0, maximumNumberOfCharatersToEvaluate)).Value;

                            xPathInfo1.LevenstheinDistance += levenstheinDistance;
                            xPathInfo2.LevenstheinDistance += levenstheinDistance;
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        private static string ExtractDateXPath(HtmlDocument htmlDocument1, IDictionary<string, XPathInfo> xpathInfos)
        {
            //start with the most likely candiates for date (headers)...
            List<string> headersXPaths = xpathInfos.Where(x => _headerTagNames.Contains(x.Key.Split('/').Last())).OrderByDescending(x => x.Value.Count).Select(x => x.Key).ToList();

            foreach (string headerXPath in headersXPaths)
            {
                foreach (HtmlNode headerNode in htmlDocument1.DocumentNode.SelectNodes(headerXPath))
                {
                    DateTime dateTime;

                    if (DateTime.TryParse(headerNode.InnerText, out dateTime))
                    {
                        return headerXPath;
                    }
                }
            }

            return null;
        }

        private static IDictionary<string, XPathInfo> GenerateXPaths(HtmlNode htmlNode, string xpath, IDictionary<string, XPathInfo> xpathInfos)
        {
            if (htmlNode.NodeType == HtmlNodeType.Element)
            {
                xpath += "/" + htmlNode.Name;

                if (_allowedTagNames.Contains(htmlNode.Name) && !xpathInfos.ContainsKey(xpath))
                {
                    string innerText = UserDefinedFunctions.ExtractText(htmlNode.InnerHtml).Value;

                    if (!string.IsNullOrEmpty(innerText.Trim()))
                    {
                        XPathInfo xPathInfo = new XPathInfo();

                        xPathInfo.InnerText = innerText;
                        xPathInfo.Tag = htmlNode.Name;
                        xPathInfo.XPath = xpath;

                        xpathInfos.Add(xpath, xPathInfo);
                    }
                }

                if (xpathInfos.ContainsKey(xpath))
                {
                    xpathInfos[xpath].Count++;
                }

                Debug.Print(xpath);
            }

            foreach (HtmlNode childNode in htmlNode.ChildNodes)
            {
                if (childNode.NodeType == HtmlNodeType.Element)
                {
                    GenerateXPaths(childNode, xpath, xpathInfos);
                }
            }

            return xpathInfos;
        }

        /// <summary>
        /// 	Stops this instance.
        /// </summary>
        public override void Stop()
        {
        }
    }

    internal class XPathInfo
    {
        internal int Count { get; set; }
        internal string InnerText { get; set; }
        internal int LevenstheinDistance { get; set; }
        internal string Tag { get; set; }
        internal string XPath { get; set; }

        public new string ToString()
        {
            return XPath;
        }
    }
}