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
using System.Web.UI;
using System.Web.UI.WebControls;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.DataSource;
using Arachnode.Plugins.CrawlActions.Managers;
using Arachnode.SiteCrawler.Managers;
using Arachnode.SiteCrawler.Value;
using Lucene.Net.Documents;

#endregion

namespace Arachnode.Web.UserControls
{
    public partial class SearchResults : UserControlBase
    {
        /// <summary>
        /// 	Gets or sets the documents.
        /// </summary>
        /// <value>The documents.</value>
        public Plugins.CrawlActions.Value.SearchResults<Document> Results { get; set; }

        /// <summary>
        /// 	Handles the Load event of the Page control.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.EventArgs" /> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString.Count == 5 && Request.QueryString.AllKeys[0] == "query" && Request.QueryString.AllKeys[1] == "discoveryType" && Request.QueryString.AllKeys[2] == "pageNumber" && Request.QueryString.AllKeys[3] == "pageSize" && Request.QueryString.AllKeys[4] == "shouldDocumentsBeClustered")
            {
                string query = Request.QueryString["query"];
                int pageNumber = int.Parse(Request.QueryString["pageNumber"]);
                int pageSize = int.Parse(Request.QueryString["pageSize"]);
                bool shouldDocumentsBeClustered = Request.QueryString["shouldDocumentsBeClustered"] == "1" ? true : false;

                if (Results != null)
                {
                    for (int i = 0; i < Results.Documents.Count; i++)
                    {
                        SearchResult searchResult = (SearchResult) LoadControl("SearchResult.ascx");

                        searchResult.Document = Results.Documents[i];
                        searchResult.InitializeAsUserControl(Page);
                        searchResult.ID = "uxUcSearchResult_" + i;

                        //AN will no longer populate the filesystem from database sources...
                        //string discoveryPath = Results.Documents[i].GetField("discoverypath").StringValue();

                        //if (!File.Exists(discoveryPath))
                        //{
                        //    switch (Results.Documents[i].GetField("discoverypath").StringValue())
                        //    {
                        //        case "image":
                        //            //ArachnodeDataSet.ImagesRow imagesRow = null;

                        //            //try
                        //            //{
                        //            //    imagesRow = ArachnodeDAO.GetImage(Results.Documents[i].GetField("discoveryid").StringValue());

                        //            //    WebPageManager webPageManager = new WebPageManager(ArachnodeDAO);

                        //            //    ManagedWebPage managedWebPage = webPageManager.ManageWebPage(webPagesRow.ID, webPagesRow.AbsoluteUri, webPagesRow.Source, Encoding.GetEncoding(webPagesRow.CodePage), webPagesRow.FullTextIndexType, false, false, true);

                        //            //    managedWebPage.StreamWriter.Close();
                        //            //    managedWebPage.StreamWriter.Dispose();

                        //            //    discoveryPath = managedWebPage.DiscoveryPath;
                        //            //}
                        //            //catch (Exception exception)
                        //            //{
                        //            //    if (webPagesRow != null)
                        //            //    {
                        //            //        ArachnodeDAO.InsertException(webPagesRow.AbsoluteUri, null, exception, false);
                        //            //    }
                        //            //    else
                        //            //    {
                        //            //        ArachnodeDAO.InsertException(null, null, exception, false);
                        //            //    }

                        //            //    Results.TotalNumberOfHits--;

                        //            //    continue;
                        //            //}
                        //            break;

                        //        case "webpage":
                        //            ArachnodeDataSet.WebPagesRow webPagesRow = null;

                        //            try
                        //            {
                        //                webPagesRow = ArachnodeDAO.GetWebPage(Results.Documents[i].GetField("discoveryid").StringValue());

                        //                WebPageManager webPageManager = new WebPageManager(ArachnodeDAO);

                        //                ManagedWebPage managedWebPage = webPageManager.ManageWebPage(webPagesRow.ID, webPagesRow.AbsoluteUri, webPagesRow.Source, Encoding.GetEncoding(webPagesRow.CodePage), webPagesRow.FullTextIndexType, false, false, true);

                        //                managedWebPage.StreamWriter.Close();
                        //                managedWebPage.StreamWriter.Dispose();

                        //                discoveryPath = managedWebPage.DiscoveryPath;
                        //            }
                        //            catch (Exception exception)
                        //            {
                        //                if (webPagesRow != null)
                        //                {
                        //                    ArachnodeDAO.InsertException(webPagesRow.AbsoluteUri, null, exception, false);
                        //                }
                        //                else
                        //                {
                        //                    ArachnodeDAO.InsertException(null, null, exception, false);
                        //                }

                        //                Results.TotalNumberOfHits--;

                        //                continue;
                        //            }
                        //            break;
                        //    }
                        //}

                        string text = null;

                        if (File.Exists(Results.Documents[i].GetField("discoverypath").StringValue()))
                        {
                            text = File.ReadAllText(Results.Documents[i].GetField("discoverypath").StringValue(), Encoding.GetEncoding(int.Parse(Results.Documents[i].GetField("codepage").StringValue())));
                        }
                        else
                        {
                            ArachnodeDataSet.WebPagesRow webPagesRow = ArachnodeDAO.GetWebPage(Results.Documents[i].GetField("discoveryid").StringValue());

                            if (webPagesRow != null && webPagesRow.Source.Length != 0)
                            {
                                text = Encoding.GetEncoding(webPagesRow.CodePage).GetString(webPagesRow.Source);
                            }
                        }

                        if (text != null)
                        {
                            searchResult.Summary = SearchManager.Summarize(Results.Query, Results.WildcardSafeQuery, shouldDocumentsBeClustered, text);
                        }
                        else
                        {
                            searchResult.Summary = "The WebPage source for " + Results.Documents[i].GetField("absoluteuri").StringValue() + " was not found in the database or on disk.";

                            try
                            {
                                throw new Exception("The WebPage source for " + Results.Documents[i].GetField("absoluteuri").StringValue() + " was not found in the database or on disk.");
                            }
                            catch (Exception exception)
                            {
                                ArachnodeDAO.InsertException(null, null, exception, false);
                            }

                            Results.TotalNumberOfHits--;
                        }

                        uxPhSearchResults.Controls.Add(searchResult);
                    }

                    if (shouldDocumentsBeClustered)
                    {
                        uxHlShouldDocumentsBeClustered.NavigateUrl = Request.Url.AbsoluteUri.Replace("shouldDocumentsBeClustered=1", "shouldDocumentsBeClustered=0");
                    }
                    else
                    {
                        uxHlShouldDocumentsBeClustered.NavigateUrl = Request.Url.AbsoluteUri.Replace("shouldDocumentsBeClustered=0", "shouldDocumentsBeClustered=1");
                    }

                    uxHlShouldDocumentsBeClustered.Visible = true;
                    
                    //create the page links.
                    for (int i = 1; i*pageSize <= Results.TotalNumberOfHits + pageSize && i*pageSize <= WebSettings.MaximumNumberOfDocumentsToReturnPerSearch; i++)
                    {
                        HyperLink hyperLink = new HyperLink();

                        if (pageNumber != i)
                        {
                            hyperLink.CssClass = "pageNumber";

                            hyperLink.NavigateUrl = Request.Url.LocalPath + "?query=" + query + "&discoveryType=" + Request.QueryString["discoveryType"] + "&pageNumber=" + i + "&pageSize=" + pageSize;

                            if (shouldDocumentsBeClustered)
                            {
                                hyperLink.NavigateUrl += "&shouldDocumentsBeClustered=1";
                            }
                            else
                            {
                                hyperLink.NavigateUrl += "&shouldDocumentsBeClustered=0";
                            }
                        }
                        else
                        {
                            hyperLink.CssClass = "currentPageNumber";
                        }

                        hyperLink.Text = i.ToString();

                        uxPhPages.Controls.Add(hyperLink);
                    }

                    uxLblPage.Visible = true;
                    uxHlPrevious.Visible = pageNumber > 1;
                    uxHlPrevious.NavigateUrl = "~/Search.aspx?query=" + HttpUtility.UrlEncode(query) + "&discoveryType=" + Request.QueryString["discoveryType"] + "&pageNumber=" + (pageNumber - 1) + "&pageSize=" + pageSize;
                    if (shouldDocumentsBeClustered)
                    {
                        uxHlPrevious.NavigateUrl += "&shouldDocumentsBeClustered=0";
                    }
                    else
                    {
                        uxHlPrevious.NavigateUrl += "&shouldDocumentsBeClustered=1";
                    }
                    uxHlNext.Visible = pageNumber < Results.TotalNumberOfHits/pageSize && pageNumber < WebSettings.MaximumNumberOfDocumentsToReturnPerSearch/pageSize;
                    uxHlNext.NavigateUrl = "~/Search.aspx?query=" + HttpUtility.UrlEncode(query) + "&discoveryType=" + Request.QueryString["discoveryType"] + "&pageNumber=" + (pageNumber + 1) + "&pageSize=" + pageSize;
                    if (shouldDocumentsBeClustered)
                    {
                        uxHlNext.NavigateUrl += "&shouldDocumentsBeClustered=1";
                    }
                    else
                    {
                        uxHlNext.NavigateUrl += "&shouldDocumentsBeClustered=0";
                    }
                }
            }
        }
    }
}