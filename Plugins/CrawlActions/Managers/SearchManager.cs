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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Arachnode.SiteCrawler.Value.Enums;
using Arachnode.Structures;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Highlight;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;

#endregion

namespace Arachnode.Plugins.CrawlActions.Managers
{
    public static class SearchManager
    {
#pragma warning disable 612,618
        public static Value.SearchResults<Document> GetDocuments(QueryParser defaultQueryParser, QueryParser customQueryParser, IndexSearcher indexSearcher, string query, DiscoveryType discoveryType, int pageNumber, int pageSize, bool shouldDocumentsBeClustered, string sort, int maximumNumberOfDocumentsToScore)
        {
            Query query2;
            Query wildcardSafeQuery2;

            if (!query.Contains(":"))
            {
                query2 = defaultQueryParser.Parse(query + " AND discoverytype:" + Enum.GetName(typeof (DiscoveryType), discoveryType));
                wildcardSafeQuery2 = defaultQueryParser.Parse(query.Replace("*", " ").Replace("?", " ") + " AND discoverytype:" + Enum.GetName(typeof(DiscoveryType), discoveryType));
            }
            else
            {
                query2 = customQueryParser.Parse(query);
                wildcardSafeQuery2 = customQueryParser.Parse(query.Replace("*", " ").Replace("?", " "));
            }

            TopDocs topDocs = null;

            if (!string.IsNullOrEmpty(sort))
            {
                string[] sorts = sort.ToLower().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                List<SortField> sortFields = new List<SortField>(sorts.Length/2);

                for (int i = 0; i < sorts.Length; i++)
                {
                    if (sorts[i].Split(' ')[1] == "asc")
                    {
                        sortFields.Add(new SortField(sorts[i].Split(' ')[0], false));
                    }
                    else
                    {
                        sortFields.Add(new SortField(sorts[i].Split(' ')[0], true));
                    }
                }

                topDocs = indexSearcher.Search(query2, null, maximumNumberOfDocumentsToScore, new Sort(sortFields.ToArray()));
            }
            else
            {
                topDocs = indexSearcher.Search(query2, null, maximumNumberOfDocumentsToScore);
            }

            Value.SearchResults<Document> searchResults = new Value.SearchResults<Document>();

            searchResults.Documents = new List<Document>();
            searchResults.Query = query2;
            searchResults.WildcardSafeQuery = wildcardSafeQuery2;

            if (topDocs.scoreDocs.Length != 0)
            {
                Dictionary<string, string> domains = new Dictionary<string, string>();

                PriorityQueue<Document> priorityQueue = new PriorityQueue<Document>();

                //Get the Hits!!!
                //ANODET: Optimize this!!! (Version 2.5+)
                for (int j = 0; j < topDocs.scoreDocs.Length && searchResults.Documents.Count < maximumNumberOfDocumentsToScore && priorityQueue.Count < maximumNumberOfDocumentsToScore; j++)
                {
                    Document document = indexSearcher.Doc(topDocs.scoreDocs[j].doc);

                    float score = topDocs.scoreDocs[j].score;

                    document.Add(new Field("documentid", j.ToString(), Field.Store.YES, Field.Index.NO));
                    document.Add(new Field("relevancyscore", score.ToString(), Field.Store.YES, Field.Index.NO));

                    if (!string.IsNullOrEmpty(sort))
                    {
                        if (shouldDocumentsBeClustered)
                        {
                            if (document.GetField("domain") != null)
                            {
                                string domain = document.GetField("domain").StringValue();

                                if (!domains.ContainsKey(domain))
                                {
                                    domains.Add(domain, null);

                                    if (searchResults.Documents.Count < pageSize && j >= (pageNumber*pageSize) - pageSize)
                                    {
                                        searchResults.Documents.Add(document);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (searchResults.Documents.Count < pageSize && j >= (pageNumber*pageSize) - pageSize)
                            {
                                searchResults.Documents.Add(document);
                            }
                        }
                    }
                    else
                    {
                        priorityQueue.Enqueue(document, score*(double.Parse(document.GetField("strength").StringValue()) + 1));
                    }
                }

                if (string.IsNullOrEmpty(sort))
                {
                    for (int i = 0; i < topDocs.scoreDocs.Length && priorityQueue.Count != 0; i++)
                    {
                        Document document = priorityQueue.Dequeue();

                        if (shouldDocumentsBeClustered)
                        {
                            if (document.GetField("domain") != null)
                            {
                                string domain = document.GetField("domain").StringValue();

                                if (!domains.ContainsKey(domain))
                                {
                                    domains.Add(domain, null);

                                    if (searchResults.Documents.Count < pageSize && i >= (pageNumber*pageSize) - pageSize)
                                    {
                                        searchResults.Documents.Add(document);
                                    }
                                }
                                else
                                {
                                    i--;
                                }
                            }
                        }
                        else
                        {
                            if (searchResults.Documents.Count < pageSize && i >= (pageNumber*pageSize) - pageSize)
                            {
                                searchResults.Documents.Add(document);
                            }
                        }
                    }
                }

                if (shouldDocumentsBeClustered)
                {
                    searchResults.TotalNumberOfHits = domains.Count;
                }
                else
                {
                    searchResults.TotalNumberOfHits = topDocs.totalHits;
                }
            }

            return searchResults;
        }

        public static string Summarize(Query query, Query wildcardSafeQuery, bool shouldDocumentsBeClustered, string discoveryPath, Encoding encoding)
        {
            if(discoveryPath != null && encoding != null)
            {
                if(File.Exists(discoveryPath))
                {
                    return Summarize(query, wildcardSafeQuery, shouldDocumentsBeClustered, File.ReadAllText(discoveryPath, encoding));
                }
            }

            return null;
        }

        public static string Summarize(Query query, Query wildcardSafeQuery, bool shouldDocumentsBeClustered, string text)
        {
            int fragmentLength = 150;

            StandardAnalyzer standardAnalyzer = new StandardAnalyzer();

            Highlighter highligher = new Highlighter(new QueryScorer(query));

            highligher.SetTextFragmenter(new SimpleFragmenter(fragmentLength));

            string text2 = UserDefinedFunctions.ExtractText(text).Value;

            TokenStream tokenStream = standardAnalyzer.TokenStream("text", new StringReader(text2));

            string bestFragments = (highligher.GetBestFragments(tokenStream, text2, 1, "...") + " ...").TrimStart(" ,".ToCharArray());

            if (bestFragments == "...")
            {
                text = HttpUtility.HtmlEncode(text);

                tokenStream = standardAnalyzer.TokenStream("text", new StringReader(text));

                bestFragments = (highligher.GetBestFragments(tokenStream, text, 1, "...") + " ...").TrimStart(" ,".ToCharArray());

                if (bestFragments == "...")
                {
                    Hashtable hashTable = new Hashtable();

                    try
                    {
                        query.ExtractTerms(hashTable);
                    }
                    catch
                    {
                        try
                        {
                            wildcardSafeQuery.ExtractTerms(hashTable);
                        }
                        catch
                        {
                        }
                    }

                    if (hashTable.Count != 0)
                    {
                        string firstTerm = null;

                        foreach (Term term in hashTable.Values)
                        {
                            if (term.Field() == "text")
                            {
                                string termText = term.Text();

                                if (termText != null)
                                {
                                    firstTerm = termText.Split(' ')[0];

                                    break;
                                }
                            }
                        }

                        if (firstTerm != null)
                        {
                            int index = text.ToLowerInvariant().IndexOf(firstTerm);

                            if (index != -1)
                            {
                                if (index + fragmentLength > text.Length)
                                {
                                    fragmentLength = text.Length - index;
                                }

                                bestFragments = Regex.Replace(text.Substring(index, fragmentLength), firstTerm, "<b>" + firstTerm + "</b>", RegexOptions.IgnoreCase) + "...";
                            }
                        }
                    }
                }
            }

            return bestFragments;
        }
    }
#pragma warning restore 612,618
}