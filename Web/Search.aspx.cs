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
using System.Diagnostics;
using System.Web.UI;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.Plugins.CrawlActions.Managers;
using Arachnode.SiteCrawler.Value.Enums;
using Lucene.Net.Documents;

#endregion

namespace Arachnode.Web
{
    public partial class Search : PageBase
    {
        /// <summary>
        /// 	Handles the Load event of the Page control.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.EventArgs" /> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            EnableViewState = false;

            uxTbQuery.Focus();

            if (!IsPostBack)
            {
                //ANODET: Implement page caching. (Version 2.5+)

                Stopwatch stopwatch = new Stopwatch();

                stopwatch.Start();

                try
                {
                    if (Request.QueryString.Count == 5 && Request.QueryString.AllKeys[0] == "query" && Request.QueryString.AllKeys[1] == "discoveryType" && Request.QueryString.AllKeys[2] == "pageNumber" && Request.QueryString.AllKeys[3] == "pageSize" && Request.QueryString.AllKeys[4] == "shouldDocumentsBeClustered")
                    {
                        string query = Request.QueryString["query"];
                        int pageNumber = int.Parse(Request.QueryString["pageNumber"]);
                        int pageSize = int.Parse(Request.QueryString["pageSize"]);
                        bool shouldDocumentsBeClustered = Request.QueryString["shouldDocumentsBeClustered"] == "1" ? true : false;

                        uxTbQuery.Text = query;
                        if (!query.Contains(":"))
                        {
                            uxRblDiscoveryType.SelectedValue = Request.QueryString["discoveryType"];
                        }
                        else
                        {
                            uxRblDiscoveryType.Visible = false;
                        }

                        /**/

                        Global.RefreshIndexSearcher();

                        Plugins.CrawlActions.Value.SearchResults<Document> searchResults = SearchManager.GetDocuments(Global.DefaultQueryParser, Global.CustomQueryParser, Global.IndexSearcher, query, (DiscoveryType) Enum.Parse(typeof (DiscoveryType), Request.QueryString["discoveryType"]), pageNumber, pageSize, shouldDocumentsBeClustered, null, WebSettings.MaximumNumberOfDocumentsToReturnPerSearch);

                        uxLblTotalNumberOfHits.Text = searchResults == null ? 0.ToString() : searchResults.Documents.Count + " of " + Global.IndexSearcher.GetIndexReader().NumDocs().ToString("###,###,###,###") + " Results";

                        if (searchResults != null)
                        {
                            if (searchResults.TotalNumberOfHits != 0)
                            {
                                uxLblResultsDetails.Text = "Results <b>" + (((pageNumber - 1)*pageSize) + 1) + "</b> - <b>" + ((pageNumber*pageSize) - (pageSize - searchResults.Documents.Count)) + "</b> of about <b>" + searchResults.TotalNumberOfHits.ToString("###,###,###,###") + "</b> for <b>" + query + "</b>. (<b>" + Math.Round((double) stopwatch.ElapsedMilliseconds/1000, 2) + "</b> seconds)";

                                ucUxSearchResults.Results = searchResults;
                            }
                            else
                            {
                                uxLblResultsDetails.Text = "<b>0</b> Results for <b>" + query + "</b>. (<b>" + Math.Round((double) stopwatch.ElapsedMilliseconds/1000, 2) + "</b> seconds)";
                            }
                        }
                        else
                        {
                            uxLblResultsDetails.Text = "<b>0</b> Results for <b>" + query + "</b>. (<b>" + Math.Round((double) stopwatch.ElapsedMilliseconds/1000, 2) + "</b> seconds)";
                        }

                        uxPnlResultsDetails.Visible = true;
                    }
                }
                catch (Exception exception)
                {
                    uxLblException.Text = exception.Message;
                    uxLblException.Visible = true;

                    ArachnodeDAO.InsertException(null, null, exception, false);
                }
            }
        }

        /// <summary>
        /// 	Handles the Click event of the uxBtnSearch control.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.EventArgs" /> instance containing the event data.</param>
        protected void uxBtnSearch_Click(object sender, EventArgs e)
        {
            if (uxTbQuery.Text != string.Empty)
            {
                if (Request.QueryString["shouldDocumentsBeClustered"] == "0")
                {
                    Response.Redirect(Request.Url.LocalPath + "?query=" + uxTbQuery.Text + "&discoveryType=" + uxRblDiscoveryType.SelectedValue + "&pageNumber=1&pageSize=" + WebSettings.PageSize + "&shouldDocumentsBeClustered=0", true);
                }
                else
                {
                    Response.Redirect(Request.Url.LocalPath + "?query=" + uxTbQuery.Text + "&discoveryType=" + uxRblDiscoveryType.SelectedValue + "&pageNumber=1&pageSize=" + WebSettings.PageSize + "&shouldDocumentsBeClustered=1", true);
                }
            }
        }
    }
}
