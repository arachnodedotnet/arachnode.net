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
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Web.Services;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.DataSource;
using Arachnode.Plugins.CrawlActions.Managers;
using Arachnode.Plugins.CrawlActions.Value;
using Arachnode.SiteCrawler.Value.Enums;
using Arachnode.Web.Value.AbstractClasses;
using Lucene.Net.Documents;
using Document = Arachnode.Plugins.CrawlActions.Value.Document;

#endregion

namespace Arachnode.Web.Services
{
    /// <summary>
    /// 	Summary description for SearchService
    /// </summary>
    [WebService(Namespace = "http://arachnode.net/SearchService")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class SearchService : AWebService
    {
        [WebMethod(Description = "Searches the lucene.net index.", EnableSession = true)]
        public SearchResults<Document> Search(string query, string discoveryType, int pageNumber, int pageSize, bool shouldDocumentsBeClustered, string sort)
        {
            try
            {
                Global.RefreshIndexSearcher();

                if (string.IsNullOrEmpty(discoveryType))
                {
                    discoveryType = "WebPage";
                }
                if (pageNumber == 0)
                {
                    pageNumber = 1;
                }
                if (pageSize == 0)
                {
                    pageSize = 20;
                }
                if (string.IsNullOrEmpty(sort))
                {
                }
                
                SearchResults<Lucene.Net.Documents.Document> searchResults = SearchManager.GetDocuments(Global.DefaultQueryParser, Global.CustomQueryParser, Global.IndexSearcher, query, (DiscoveryType) Enum.Parse(typeof (DiscoveryType), discoveryType), pageNumber, pageSize, shouldDocumentsBeClustered, sort, WebSettings.MaximumNumberOfDocumentsToReturnPerSearch);
                
                SearchResults<Document> searchResults2 = new SearchResults<Document>();

                searchResults2.Documents = new List<Document>(searchResults.Documents.Count);
                searchResults2.TotalNumberOfHits = searchResults.TotalNumberOfHits;

                foreach (Lucene.Net.Documents.Document document in searchResults.Documents)
                {
                    try
                    {
                        Document document2 = new Document();

                        document2.AbsoluteUri = document.GetField("absoluteuri").StringValue();
                        document2.Created = DateTools.StringToDate(document.GetField("created").StringValue());
                        document2.DiscoveryID = long.Parse(document.GetField("discoveryid").StringValue());
                        document2.DiscoveryPath = document.GetField("discoverypath").StringValue();
                        document2.Domain = document.GetField("domain").StringValue();
                        document2.Extension = document.GetField("extension").StringValue();
                        document2.Host = document.GetField("host").StringValue();
                        document2.Scheme = document.GetField("scheme").StringValue();
                        document2.Score = float.Parse(document.GetField("relevancyscore").StringValue());
                        document2.Strength = float.Parse(document.GetField("strength").StringValue());

                        string text = null;

                        if (File.Exists(document2.DiscoveryPath))
                        {
                            text = File.ReadAllText(document2.DiscoveryPath, Encoding.GetEncoding(int.Parse(document.GetField("codepage").StringValue())));
                        }
                        else
                        {
                            ArachnodeDataSet.WebPagesRow webPagesRow = ArachnodeDAO.GetWebPage(document2.DiscoveryID.ToString());

                            if (webPagesRow != null && webPagesRow.Source != null)
                            {
                                text = Encoding.GetEncoding(webPagesRow.CodePage).GetString(webPagesRow.Source);
                            }
                        }

                        if (text != null)
                        {
                            document2.Summary = SearchManager.Summarize(searchResults.Query, searchResults.WildcardSafeQuery, shouldDocumentsBeClustered, text);
                        }
                        else
                        {
                            document2.Summary = "The WebPage source was not found in the database or on disk.";

                            try
                            {
                                throw new Exception("The WebPage source for " + document2.AbsoluteUri + " was not found in the database or on disk.");
                            }
                            catch (Exception exception)
                            {
                                ArachnodeDAO.InsertException(null, null, exception, false);
                            }
                        }

                        document2.Title = document.GetField("title").StringValue();
                        if (document.GetField("updated") != null)
                        {
                            document2.Updated = DateTools.StringToDate(document.GetField("updated").StringValue());
                        }

                        searchResults2.Documents.Add(document2);
                    }
                    catch (Exception exception)
                    {
                        ArachnodeDAO.InsertException(null, null, exception, false);
                    }
                }

                return searchResults2;
            }
            catch (Exception exception)
            {
                ArachnodeDAO.InsertException(null, null, exception, false);
            }

            return null;
        }
    }
}