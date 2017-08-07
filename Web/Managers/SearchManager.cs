#region Licensed : arachnode.net

// Copyright (c) 2009 http://arachnode.net, arachnode.net, LLC
// 
// Permission is hereby granted, upon purchase, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, merge and modify copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following
// conditions:
// 
// 1.) The right to publish, distribute, sublicense, and/or sell is prohibited.
// 2.) Each machine that arachnode.net is installed upon requires a separate license.
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
// 

#endregion

#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Caching;
using Arachnode.Structures;
using Arachnode.Web.Value;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Highlight;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Document=Lucene.Net.Documents.Document;

#endregion

namespace Arachnode.Web.Managers
{
    public static class SearchManager
    {
        //TODO: The last param, Cache isn't used.  Remove it and update dependant projects.  (Version 1.3)
        public static SearchResults<Document> GetDocuments(QueryParser defaultQueryParser, QueryParser customQueryParser, IndexSearcher indexSearcher, string query, int pageNumber, int pageSize, bool shouldDocumentsBeClustered, string sort, int maximumNumberOfDocumentsToScore, Cache cache)
        {
            Query query2 = customQueryParser.Parse(query);

            Hits hits = null;

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

                hits = indexSearcher.Search(query2, new Sort(sortFields.ToArray()));
            }
            else
            {
                hits = indexSearcher.Search(query2);
            }

            SearchResults<Document> searchResults = new SearchResults<Document>();

            searchResults.Documents = new List<Document>();
            searchResults.Query = query2;

            if (hits.Length() != 0)
            {
                Dictionary<string, string> domains = new Dictionary<string, string>();

                PriorityQueue<Document> priorityQueue = new PriorityQueue<Document>();

                //Get the Hits!!!
                //TODO: Optimize this!!! (Version 1.3)
                for (int j = 0; j < hits.Length() && searchResults.Documents.Count < maximumNumberOfDocumentsToScore && priorityQueue.Count < maximumNumberOfDocumentsToScore; j++)
                {
                    Document document = hits.Doc(j);

                    float score = hits.Score(j);

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
                        priorityQueue.Enqueue(document, score*double.Parse(document.GetField("strength").StringValue()));
                    }
                }

                if (string.IsNullOrEmpty(sort))
                {
                    for (int i = 0; i < hits.Length() && priorityQueue.Count != 0; i++)
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
                    searchResults.TotalNumberOfHits = hits.Length();
                }
            }

            return searchResults;
        }

        public static string Summarize(Query query, bool shouldDocumentsBeClustered, string discoveryPath, Encoding encoding, Cache cache)
        {
            StandardAnalyzer standardAnalyzer = new StandardAnalyzer();

            Highlighter highligher = new Highlighter(new QueryScorer(query));

            highligher.SetTextFragmenter(new SimpleFragmenter(150));

            string text = UserDefinedFunctions.ExtractText(File.ReadAllText(discoveryPath, encoding)).Value;

            TokenStream tokenStream = standardAnalyzer.TokenStream("text", new StringReader(text));

            return (highligher.GetBestFragments(tokenStream, text, 1, "...") + " ...").TrimStart(" ,".ToCharArray());
        }
    }
}