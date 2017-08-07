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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.DataSource;
using Arachnode.Plugins.CrawlActions.Managers;
using Arachnode.SiteCrawler.Core;
using Arachnode.SiteCrawler.Managers;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.AbstractClasses;
using Arachnode.SiteCrawler.Value.Enums;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Directory = System.IO.Directory;

#endregion

namespace Arachnode.Plugins.CrawlActions
{
    public class ManageLuceneDotNetIndexes<TArachnodeDAO> : ACrawlAction<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
#pragma warning disable 612,618
        protected static readonly Regex _title = new Regex(@"<(|\s*)title(|\s*)>(?<Title>.*?)<(|\s*)/(|\s*)title(|\s*)>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        protected readonly DOCManager _docManager = new DOCManager();
        protected readonly PDFManager _pdfManager = new PDFManager();

        private bool _autoCommit;
        private object _autoCommitLock;
        private bool _checkIndexes;
        private FSDirectory _currentCrawlDirectory;
        protected bool _indexFiles;
        protected bool _indexImages;
        protected IndexSearcher _indexSearcher;
        protected FSDirectory _indexTempDirectory;
        protected bool _indexWebPages;
        protected IndexWriter _indexWriter;
        private DateTime _lastCommitDateTime;
        private FSDirectory _luceneDotNetIndexDirectory;
        protected StandardAnalyzer _standardAnalyzer;

        public ManageLuceneDotNetIndexes(ApplicationSettings applicationSettings, WebSettings webSettings)
            : base(applicationSettings, webSettings)
        {
        }

        /// <summary>
        /// 	Assigns the additional parameters.
        /// </summary>
        /// <param name = "settings"></param>
        public override void AssignSettings(Dictionary<string, string> settings)
        {
            _checkIndexes = bool.Parse(settings["CheckIndexes"]);

            bool autoCommit = bool.Parse(settings["AutoCommit"]);

            string luceneDotNetIndexDirectory = settings["LuceneDotNetIndexDirectory"];
            string currentCrawlDirectory = Path.Combine(luceneDotNetIndexDirectory, "CurrentCrawl");

            //create required directories...
            if (!Directory.Exists(luceneDotNetIndexDirectory))
            {
                Directory.CreateDirectory(luceneDotNetIndexDirectory);
            }

            if (!Directory.Exists(currentCrawlDirectory))
            {
                Directory.CreateDirectory(currentCrawlDirectory);
            }

            //create lucene.net directories...
            _luceneDotNetIndexDirectory = FSDirectory.Open(new DirectoryInfo(luceneDotNetIndexDirectory));
            _currentCrawlDirectory = FSDirectory.Open(new DirectoryInfo(currentCrawlDirectory));

            _standardAnalyzer = new StandardAnalyzer();

            //delete the lock - a crawl may have been prematurely terminated, likely by the user's election.  write.lock prevents us from writing to the index on subsequent crawls.
            if (File.Exists(Path.Combine(luceneDotNetIndexDirectory, "write.lock")))
            {
                File.Delete(Path.Combine(luceneDotNetIndexDirectory, "write.lock"));
            }

            //delete the lock - a crawl may have been prematurely terminated, likely by the user's election.  write.lock prevents us from writing to the index on subsequent crawls.
            if (File.Exists(Path.Combine(currentCrawlDirectory, "write.lock")))
            {
                File.Delete(Path.Combine(currentCrawlDirectory, "write.lock"));
            }

            File.Delete(Path.Combine(currentCrawlDirectory, "write.lock"));

            ManageIndexes();

            TearDownIndexWriter();

            _indexFiles = bool.Parse(settings["IndexFiles"]);
            _indexImages = bool.Parse(settings["IndexImages"]);
            _indexWebPages = bool.Parse(settings["IndexWebPages"]);

            //check to see if we have requested to rebuild the index.
            if (bool.Parse(settings["RebuildIndexOnLoad"]))
            {
                string tempDirectory = Path.Combine(luceneDotNetIndexDirectory, "Temp");
                int fileIDLowerBound = int.Parse(settings["FileIDLowerBound"]);
                int fileIDUpperBound = int.Parse(settings["FileIDUpperBound"]);
                int imageIDLowerBound = int.Parse(settings["ImageIDLowerBound"]);
                int imageIDUpperBound = int.Parse(settings["ImageIDUpperBound"]);
                int webPageIDLowerBound = int.Parse(settings["WebPageIDLowerBound"]);
                int webPageIDUpperBound = int.Parse(settings["WebPageIDUpperBound"]);

                RebuildIndexes(tempDirectory, fileIDLowerBound, fileIDUpperBound, imageIDLowerBound, imageIDUpperBound, webPageIDLowerBound, webPageIDUpperBound);

                TearDownIndexWriter();
            }

            //switch back to the _current
            if (autoCommit)
            {
                //NOTE: autoCommit was disabled in Lucene.net 2.4.  The threads now check when to Commit();
                _autoCommit = true;
                _autoCommitLock = new object();
                _lastCommitDateTime = DateTime.Now;
                //_indexWriter = new IndexWriter(_luceneDotNetIndexDirectory, true, _standardAnalyzer, false);
                _indexWriter = new IndexWriter(_luceneDotNetIndexDirectory, _standardAnalyzer, false, IndexWriter.MaxFieldLength.UNLIMITED);
            }
            else
            {
                _indexWriter = new IndexWriter(_currentCrawlDirectory, _standardAnalyzer, true, IndexWriter.MaxFieldLength.UNLIMITED);
            }

            SetIndexWriterDefaults();

            _indexSearcher = new IndexSearcher(_luceneDotNetIndexDirectory, true);
        }

        /// <summary>
        /// 	Sets the index writer defaults.
        /// </summary>
        protected void SetIndexWriterDefaults()
        {
            _indexWriter.SetUseCompoundFile(false);
            _indexWriter.SetMergeFactor(1000);
            _indexWriter.SetRAMBufferSizeMB(48);
        }

        /// <summary>
        /// 	Manages the indexes.
        /// </summary>
        private void ManageIndexes()
        {
            //try to create an indexWriter over an existing index in the main directory.

            try
            {
                _indexWriter = new IndexWriter(_luceneDotNetIndexDirectory, _standardAnalyzer, false, IndexWriter.MaxFieldLength.UNLIMITED);
            }
            catch
            {
                //and index doesn't exist - create a new one...
                _indexWriter = new IndexWriter(_luceneDotNetIndexDirectory, _standardAnalyzer, true, IndexWriter.MaxFieldLength.UNLIMITED);
            }

            if (_checkIndexes && !CheckIndex.Check(_luceneDotNetIndexDirectory, false))
            {
                throw new Exception("CheckIndex.Check reported errors checking the LuceneDotNetIndex at " + _luceneDotNetIndexDirectory + ".");
            }

            if (_checkIndexes && !CheckIndex.Check(_currentCrawlDirectory, false))
            {
                throw new Exception("CheckIndex.Check reported errors checking the LuceneDotNetIndex at " + _currentCrawlDirectory + ".");
            }

            SetIndexWriterDefaults();

            ConsoleManager<TArachnodeDAO> consoleManager = new ConsoleManager<TArachnodeDAO>(_applicationSettings, _webSettings);

            try
            {
                consoleManager.OutputString("\tMerging CurrentCrawl.", ConsoleColor.White, ConsoleColor.Gray);

                _indexWriter.AddIndexesNoOptimize(new Lucene.Net.Store.Directory[] {_currentCrawlDirectory});
            }
            catch
            {
            }

            consoleManager.OutputString("\tOptimizing LuceneDotNetIndex.", ConsoleColor.White, ConsoleColor.Gray);

            _indexWriter.Optimize();
        }

        /// <summary>
        /// 	Performs the action.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        public override void PerformAction(CrawlRequest<TArachnodeDAO> crawlRequest, IArachnodeDAO arachnodeDAO)
        {
            if (!crawlRequest.IsStorable || crawlRequest.IsDisallowed || !crawlRequest.ProcessData || crawlRequest.WebClient.WebException != null || !crawlRequest.Discovery.ID.HasValue || crawlRequest.ManagedDiscovery == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(crawlRequest.Discovery.ID.ToString()))
            {
                if (crawlRequest.Data != null)
                {
                    //check to see if we need to commit new documents to the searchable index...
                    if (_autoCommit)
                    {
                        if (DateTime.Now.Subtract(_lastCommitDateTime).Duration().TotalMinutes >= _webSettings.CacheTimeoutInMinutes)
                        {
                            if (Monitor.TryEnter(_autoCommitLock, 0))
                            {
                                _indexWriter.Commit();

                                _lastCommitDateTime = DateTime.Now;

                                Monitor.Exit(_autoCommitLock);
                            }
                        }
                    }

                    //ANODET: Using an ID limits...
                    QueryParser queryParser = new QueryParser("indexkey", _standardAnalyzer);

                    Query query = queryParser.Parse("\"" + crawlRequest.Discovery.DiscoveryType.ToString().ToLower().Substring(0, 1) + crawlRequest.Discovery.ID + "\"");

                    Hits hits = _indexSearcher.Search(query);

                    /**/

                    float strength = (float)crawlRequest.Crawl.Crawler.ReportingManager.GetStrengthForHost(crawlRequest.Discovery.Uri.AbsoluteUri);

                    if (strength == 0)
                    {
                        strength = (float)crawlRequest.Crawl.Crawler.ReportingManager.GetPriorityForHost(crawlRequest.Discovery.Uri.AbsoluteUri);
                    }

                    //Files
                    if (_indexFiles)
                    {
                        if (crawlRequest.DataType.DiscoveryType == DiscoveryType.File)
                        {
                            Document document = new Document();

                            //check for a File document.
                            bool isAFileDocumentPresent = false;
                            int iF;

                            for (iF = 0; iF < hits.Length(); iF++)
                            {
                                if (hits.Doc(iF).GetField("discoverytype").StringValue() == "file")
                                {
                                    isAFileDocumentPresent = true;

                                    break;
                                }
                            }

                            if (hits.Length() == 0 || !isAFileDocumentPresent)
                            {
                                document.Add(new Field("created", DateTime.Now.Date.ToString("yyyyMMdd"), Field.Store.YES, Field.Index.NOT_ANALYZED));
                            }
                            else
                            {
                                document.Add(new Field("created", hits.Doc(iF).GetField("created").StringValue(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                                document.Add(new Field("updated", DateTime.Now.Date.ToString("yyyyMMdd"), Field.Store.YES, Field.Index.NOT_ANALYZED));

                                Term[] terms = new Term[1];

                                terms[0] = new Term("indexkey", hits.Doc(iF).GetField("indexkey").StringValue());

                                //we have to delete the existing document and then re-add it.  We can't "Update" documents in Lucene.net...
                                _indexWriter.DeleteDocuments(terms);
                            }

                            switch (crawlRequest.DataType.ContentType)
                            {
                                case "application/msword":
                                case "application/vnd.ms-excel":
                                case "application/vnd.ms-powerpoint":
                                    CreateDocument(document, crawlRequest.Discovery.ID.Value, crawlRequest.DataType.DiscoveryType, crawlRequest.Discovery.Uri.AbsoluteUri, _docManager.GetText(crawlRequest.ManagedDiscovery.DiscoveryPath) + " " + crawlRequest.Discovery.Uri.AbsoluteUri, Encoding.UTF8.CodePage, crawlRequest.DataType.FullTextIndexType, strength, crawlRequest.ManagedDiscovery.DiscoveryPath, crawlRequest.Crawl.CrawlInfo.ThreadNumber);
                                    break;
                                case "application/pdf":
                                    StringBuilder stringBuilder = new StringBuilder();

                                    PdfReader pdfReader = new PdfReader(crawlRequest.Data);

                                    for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                                    {
                                        stringBuilder.Append(PdfTextExtractor.GetTextFromPage(pdfReader, page) + " ");
                                    }

                                    CreateDocument(document, crawlRequest.Discovery.ID.Value, crawlRequest.DataType.DiscoveryType, crawlRequest.Discovery.Uri.AbsoluteUri, stringBuilder + " " + crawlRequest.Discovery.Uri.AbsoluteUri, Encoding.UTF8.CodePage, crawlRequest.DataType.FullTextIndexType, strength, crawlRequest.ManagedDiscovery.DiscoveryPath, crawlRequest.Crawl.CrawlInfo.ThreadNumber);
                                    break;
                                default:
                                    CreateDocument(document, crawlRequest.Discovery.ID.Value, crawlRequest.DataType.DiscoveryType, crawlRequest.Discovery.Uri.AbsoluteUri, Encoding.UTF8.GetString(crawlRequest.Data) + " " + crawlRequest.Discovery.Uri.AbsoluteUri, Encoding.UTF8.CodePage, crawlRequest.DataType.FullTextIndexType, strength, crawlRequest.ManagedDiscovery.DiscoveryPath, crawlRequest.Crawl.CrawlInfo.ThreadNumber);
                                    break;
                            }

                            return;
                        }
                    }

                    //Images
                    if (_indexImages)
                    {
                        if (crawlRequest.DataType.DiscoveryType == DiscoveryType.Image)
                        {
                            Document document = new Document();

                            //check for an Image document.
                            bool isAnImageDocumentPresent = false;
                            int iI;

                            for (iI = 0; iI < hits.Length(); iI++)
                            {
                                if (hits.Doc(iI).GetField("discoverytype").StringValue() == "image")
                                {
                                    isAnImageDocumentPresent = true;

                                    break;
                                }
                            }

                            if (hits.Length() == 0 || !isAnImageDocumentPresent)
                            {
                                document.Add(new Field("created", DateTime.Now.Date.ToString("yyyyMMdd"), Field.Store.YES, Field.Index.NOT_ANALYZED));
                            }
                            else
                            {
                                document.Add(new Field("created", hits.Doc(iI).GetField("created").StringValue(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                                document.Add(new Field("updated", DateTime.Now.Date.ToString("yyyyMMdd"), Field.Store.YES, Field.Index.NOT_ANALYZED));

                                Term[] terms = new Term[1];

                                terms[0] = new Term("indexkey", hits.Doc(iI).GetField("indexkey").StringValue());

                                //we have to delete the existing document and then re-add it.  We can't "Update" documents in Lucene.net...
                                _indexWriter.DeleteDocuments(terms);
                            }

                            if (_applicationSettings.ExtractImageMetaData)
                            {
                                CreateDocument(document, crawlRequest.Discovery.ID.Value, crawlRequest.DataType.DiscoveryType, crawlRequest.Discovery.Uri.AbsoluteUri, ((ManagedImage)crawlRequest.ManagedDiscovery).EXIFData.InnerXml + " " + crawlRequest.Discovery.Uri.AbsoluteUri, Encoding.UTF8.CodePage, crawlRequest.DataType.FullTextIndexType, strength, crawlRequest.ManagedDiscovery.DiscoveryPath, crawlRequest.Crawl.CrawlInfo.ThreadNumber);
                            }
                            else
                            {
                                CreateDocument(document, crawlRequest.Discovery.ID.Value, crawlRequest.DataType.DiscoveryType, crawlRequest.Discovery.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri, Encoding.UTF8.CodePage, crawlRequest.DataType.FullTextIndexType, strength, crawlRequest.ManagedDiscovery.DiscoveryPath, crawlRequest.Crawl.CrawlInfo.ThreadNumber);
                            }

                            return;
                        }
                    }

                    //WebPages
                    if (_indexWebPages)
                    {
                        if (crawlRequest.DataType.DiscoveryType == DiscoveryType.WebPage)
                        {
                            Document document = new Document();

                            //check for a WebPage document.
                            bool isAWebPageDocumentPresent = false;
                            int iW;

                            for (iW = 0; iW < hits.Length(); iW++)
                            {
                                if (hits.Doc(iW).GetField("discoverytype").StringValue() == "webpage")
                                {
                                    isAWebPageDocumentPresent = true;

                                    break;
                                }
                            }

                            if (hits.Length() == 0 || !isAWebPageDocumentPresent)
                            {
                                document.Add(new Field("created", DateTime.Now.Date.ToString("yyyyMMdd"), Field.Store.YES, Field.Index.NOT_ANALYZED));
                            }
                            else
                            {
                                document.Add(new Field("created", hits.Doc(iW).GetField("created").StringValue(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                                document.Add(new Field("updated", DateTime.Now.Date.ToString("yyyyMMdd"), Field.Store.YES, Field.Index.NOT_ANALYZED));

                                Term[] terms = new Term[1];

                                terms[0] = new Term("indexkey", hits.Doc(iW).GetField("indexkey").StringValue());

                                //we have to delete the existing document and then re-add it.  We can't "Update" documents in Lucene.net...
                                _indexWriter.DeleteDocuments(terms);
                            }

                            CreateDocument(document, crawlRequest.Discovery.ID.Value, crawlRequest.DataType.DiscoveryType, crawlRequest.Discovery.Uri.AbsoluteUri, crawlRequest.DecodedHtml, crawlRequest.Encoding.CodePage, crawlRequest.DataType.FullTextIndexType, strength, crawlRequest.ManagedDiscovery.DiscoveryPath, crawlRequest.Crawl.CrawlInfo.ThreadNumber);

                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 	Creates the document.
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <param name = "discoveryID">The discovery ID.</param>
        /// <param name = "discoveryType">The discovery type.</param>
        /// <param name = "absoluteUri">The absolute URI.</param>
        /// <param name = "contentToIndex">The content to index.</param>
        /// <param name = "codePage">The code page.</param>
        /// <param name = "discoveryPath">The discovery path.</param>
        /// <param name = "threadNumber">The thread number.</param>
        protected virtual void CreateDocument(Document document, long discoveryID, DiscoveryType discoveryType, string absoluteUri, string contentToIndex, int codePage, string fullTextIndexType, float strength, string discoveryPath, int threadNumber)
        {
            document.Add(new Field("indexkey", discoveryType.ToString().ToLower().Substring(0, 1) + discoveryID, Field.Store.YES, Field.Index.NOT_ANALYZED));

            document.Add(new Field("discoveryid", discoveryID.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field("discoverytype", discoveryType.ToString().ToLower(), Field.Store.YES, Field.Index.NOT_ANALYZED));

            //Discovery
            document.Add(new Field("absoluteuri", absoluteUri, Field.Store.YES, Field.Index.ANALYZED));

            //core fields
            document.Add(new Field("text", contentToIndex, Field.Store.NO, Field.Index.ANALYZED));
            document.Add(new Field("codepage", codePage.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field("title", _title.Match(contentToIndex).Groups["Title"].Value.Trim(), Field.Store.YES, Field.Index.ANALYZED));

            //DiscoveryPath
            document.Add(new Field("discoverypath", discoveryPath, Field.Store.YES, Field.Index.NO));

            //AbsoluteUri Classification
            document.Add(new Field("domain", UserDefinedFunctions.ExtractDomain(absoluteUri).Value, Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field("extension", UserDefinedFunctions.ExtractExtension(absoluteUri, false).Value, Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field("host", UserDefinedFunctions.ExtractHost(absoluteUri).Value, Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field("scheme", UserDefinedFunctions.ExtractScheme(absoluteUri, false).Value, Field.Store.YES, Field.Index.NOT_ANALYZED));

            //FullTextIndexType - used to store the extension that can be used with the default IIS MIME types configuration... (.pl images cannot be served without MIME type modification...)
            document.Add(new Field("fulltextindextype", fullTextIndexType, Field.Store.YES, Field.Index.NOT_ANALYZED));

            AddDocument(document, absoluteUri, strength);
        }

        /// <summary>
        /// 	Adds the document.
        /// </summary>
        /// <param name = "document">The document.</param>
        /// <param name = "absoluteUri">The absolute URI.</param>
        protected void AddDocument(Document document, string absoluteUri, float strength)
        {
            document.Add(new Field("strength", strength.ToString(), Field.Store.YES, Field.Index.NO));

            strength /= ((float) Math.Log((absoluteUri.Length - 2) - absoluteUri.Replace("/", string.Empty).Length) + 1);

            if (strength < 1)
            {
                strength = 1;
            }

            if (strength > 1000000)
            {
                strength = 1000000;
            }

            //Set Fields Boosts.
            if (document.GetField("absoluteuri") != null)
            {
                document.GetField("absoluteuri").SetBoost(4*strength);
            }
            if (document.GetField("text") != null)
            {
                document.GetField("text").SetBoost(1*strength);
            }
            if (document.GetField("host") != null)
            {
                document.GetField("host").SetBoost(2*strength);
            }
            if (document.GetField("title") != null)
            {
                document.GetField("title").SetBoost(3*strength);
            }

            document.SetBoost(strength + 1);

            _indexWriter.AddDocument(document);
        }

        private void TearDownIndexWriter()
        {
            _indexWriter.Commit();

            ConsoleManager<TArachnodeDAO> consoleManager = new ConsoleManager<TArachnodeDAO>(_applicationSettings, _webSettings);

            consoleManager.OutputString("\tCommitting LuceneDotNetIndex.", ConsoleColor.White, ConsoleColor.Gray);

            _indexWriter.Optimize();

            consoleManager.OutputString("\tOptimizing LuceneDotNetIndex.", ConsoleColor.White, ConsoleColor.Gray);

            _indexWriter.Close();

            consoleManager.OutputString("\tClosing LuceneDotNetIndex.", ConsoleColor.White, ConsoleColor.Gray);
        }

        /// <summary>
        /// 	Stops this instance.
        /// </summary>
        public override void Stop()
        {
            ConsoleManager<TArachnodeDAO> consoleManager = new ConsoleManager<TArachnodeDAO>(_applicationSettings, _webSettings);

            consoleManager.OutputString("Saving CurrentCrawl.", ConsoleColor.Gray, ConsoleColor.Gray);

            TearDownIndexWriter();

            ManageIndexes();

            TearDownIndexWriter();
        }

        private void RebuildIndexes(string tempDirectory, long fileIDLowerBound, long fileIDUpperBound, long imageIDLowerBound, long imageIDUpperBound, long webPageIDLowerBound, long webPageIDUpperBound)
        {
            if (!Directory.Exists(tempDirectory))
            {
                Directory.CreateDirectory(tempDirectory);
            }

            _indexTempDirectory = FSDirectory.Open(new DirectoryInfo(tempDirectory));

            _indexWriter = new IndexWriter(_indexTempDirectory, _standardAnalyzer, true, IndexWriter.MaxFieldLength.UNLIMITED);

            SetIndexWriterDefaults();

            TArachnodeDAO arachnodeDAO = (TArachnodeDAO)Activator.CreateInstance(typeof(TArachnodeDAO), _applicationSettings.ConnectionString);
            arachnodeDAO.ApplicationSettings = _applicationSettings;
            arachnodeDAO.WebSettings = _webSettings;

            ConsoleManager<TArachnodeDAO> consoleManager = new ConsoleManager<TArachnodeDAO>(_applicationSettings, _webSettings);
            ActionManager<TArachnodeDAO> actionManager = new ActionManager<TArachnodeDAO>(_applicationSettings, _webSettings, consoleManager);
            MemoryManager<TArachnodeDAO> memoryManager = new MemoryManager<TArachnodeDAO>(_applicationSettings, _webSettings);
            ReportingManager<TArachnodeDAO> reportingManager = new ReportingManager<TArachnodeDAO>(_applicationSettings, _webSettings, consoleManager);
            RuleManager<TArachnodeDAO> ruleManager = new RuleManager<TArachnodeDAO>(_applicationSettings, _webSettings, consoleManager);
            CacheManager<TArachnodeDAO> cacheManager = new CacheManager<TArachnodeDAO>(_applicationSettings, _webSettings);
            Cache<TArachnodeDAO> cache = new Cache<TArachnodeDAO>(_applicationSettings, _webSettings, null, actionManager, cacheManager, null, memoryManager, ruleManager);
            DiscoveryManager<TArachnodeDAO> discoveryManager = new DiscoveryManager<TArachnodeDAO>(_applicationSettings, _webSettings, cache, actionManager, cacheManager, memoryManager, ruleManager);
            HtmlManager<TArachnodeDAO> htmlManager = new HtmlManager<TArachnodeDAO>(_applicationSettings, _webSettings, discoveryManager);

            //Files
            if (_indexFiles)
            {
                FileManager<TArachnodeDAO> fileManager = new FileManager<TArachnodeDAO>(_applicationSettings, _webSettings, discoveryManager, arachnodeDAO);

                for (long i = fileIDLowerBound; i <= fileIDUpperBound; i++)
                {
                    try
                    {
                        //get the File from the database.  we need the source data as we don't store this in the index.
                        //even though most of the fields are available in the Document, the File is the authoritative source, so we'll use that for all of the fields.
                        ArachnodeDataSet.FilesRow filesRow = arachnodeDAO.GetFile(i.ToString());

                        if (filesRow != null)
                        {
                            if (filesRow.Source == null || filesRow.Source.Length == 0)
                            {
                                filesRow.Source = File.ReadAllBytes(discoveryManager.GetDiscoveryPath(_applicationSettings.DownloadedFilesDirectory, filesRow.AbsoluteUri, filesRow.FullTextIndexType));
                            }

                            /**/

                            float strength = (float)reportingManager.GetStrengthForHost(filesRow.AbsoluteUri);

                            if (strength == 0)
                            {
                                strength = (float)reportingManager.GetPriorityForHost(filesRow.AbsoluteUri);
                            }

                            /**/

                            //manage the File to update the discovery path if needed and to ensure the File is saved to disk.
                            ManagedFile managedFile = fileManager.ManageFile(null, filesRow.ID, filesRow.AbsoluteUri, filesRow.Source, filesRow.FullTextIndexType, _applicationSettings.ExtractFileMetaData, _applicationSettings.InsertFileMetaData, _applicationSettings.SaveDiscoveredFilesToDisk);

                            Document document = new Document();

                            document.Add(new Field("created", DateTime.Now.Date.ToString("yyyyMMdd"), Field.Store.YES, Field.Index.NOT_ANALYZED));
                            document.Add(new Field("updated", DateTime.Now.Date.ToString("yyyyMMdd"), Field.Store.YES, Field.Index.NOT_ANALYZED));

                            switch (UserDefinedFunctions.ExtractResponseHeader(filesRow.ResponseHeaders, "Content-Type:", false).Value)
                            {
                                case "application/msword":
                                case "application/vnd.ms-excel":
                                case "application/vnd.ms-powerpoint":
                                    CreateDocument(document, filesRow.ID, DiscoveryType.File, filesRow.AbsoluteUri, _docManager.GetText(managedFile.DiscoveryPath) + " " + filesRow.AbsoluteUri, Encoding.UTF8.CodePage, filesRow.FullTextIndexType, strength, managedFile.DiscoveryPath, 1);
                                    break;
                                case "application/pdf":
                                    StringBuilder stringBuilder = new StringBuilder();

                                    PdfReader pdfReader = new PdfReader(filesRow.Source);

                                    for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                                    {
                                        stringBuilder.Append(_pdfManager.ExtractText(pdfReader.GetPageContent(page)) + " ");
                                    }

                                    CreateDocument(document, filesRow.ID, DiscoveryType.File, filesRow.AbsoluteUri, stringBuilder + " " + filesRow.AbsoluteUri, Encoding.UTF8.CodePage, filesRow.FullTextIndexType, strength, managedFile.DiscoveryPath, 1);
                                    break;
                                default:
                                    CreateDocument(document, filesRow.ID, DiscoveryType.File, filesRow.AbsoluteUri, Encoding.UTF8.GetString(filesRow.Source) + " " + filesRow.AbsoluteUri, Encoding.UTF8.CodePage, filesRow.FullTextIndexType, strength, managedFile.DiscoveryPath, 1);
                                    break;
                            }

                            Console.WriteLine("File: '" + filesRow.AbsoluteUri + "' indexed. (" + i + " of " + (fileIDUpperBound - fileIDLowerBound + 1) + ")");
                        }
                        else
                        {
                            Console.WriteLine("File: " + i + " was not found. (" + i + " of " + (fileIDUpperBound - fileIDLowerBound + 1) + ")");
                        }
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine("File: " + i + " was not indexed. (" + i + " of " + (fileIDUpperBound - fileIDLowerBound + 1) + ")");

                        arachnodeDAO.InsertException(null, null, exception, false);
                    }
                }
            }

            //Images
            if (_indexImages)
            {
                ImageManager<TArachnodeDAO> imageManager = new ImageManager<TArachnodeDAO>(_applicationSettings, _webSettings, discoveryManager, arachnodeDAO);

                for (long i = imageIDLowerBound; i <= imageIDUpperBound; i++)
                {
                    try
                    {
                        //get the Image from the database.  we need the source data as we don't store this in the index.
                        //even though most of the fields are available in the Document, the Image is the authoritative source, so we'll use that for all of the fields.
                        ArachnodeDataSet.ImagesRow imagesRow = arachnodeDAO.GetImage(i.ToString());

                        if (imagesRow != null)
                        {
                            if (imagesRow.Source == null || imagesRow.Source.Length == 0)
                            {
                                imagesRow.Source = File.ReadAllBytes(discoveryManager.GetDiscoveryPath(_applicationSettings.DownloadedImagesDirectory, imagesRow.AbsoluteUri, imagesRow.FullTextIndexType));
                            }

                            /**/

                            float strength = (float)reportingManager.GetStrengthForHost(imagesRow.AbsoluteUri);

                            if (strength == 0)
                            {
                                strength = (float)reportingManager.GetPriorityForHost(imagesRow.AbsoluteUri);
                            }

                            /**/

                            //manage the Image to update the discovery path if needed and to ensure the Image is saved to disk.
                            ManagedImage managedImage = imageManager.ManageImage(null, imagesRow.ID, imagesRow.AbsoluteUri, imagesRow.Source, imagesRow.FullTextIndexType, _applicationSettings.ExtractImageMetaData, _applicationSettings.InsertImageMetaData, _applicationSettings.SaveDiscoveredImagesToDisk);

                            ArachnodeDataSet.ImagesMetaDataRow imagesMetaDataRow = arachnodeDAO.GetImageMetaData(imagesRow.ID);

                            Document document = new Document();
                            //ANODET: Remove the UriClassification for Files, Images and WebPages... add the alt tags!

                            document.Add(new Field("created", DateTime.Now.Date.ToString("yyyyMMdd"), Field.Store.YES, Field.Index.NOT_ANALYZED));
                            document.Add(new Field("updated", DateTime.Now.Date.ToString("yyyyMMdd"), Field.Store.YES, Field.Index.NOT_ANALYZED));

                            if (imagesMetaDataRow != null)
                            {
                                CreateDocument(document, imagesRow.ID, DiscoveryType.Image, imagesRow.AbsoluteUri, imagesMetaDataRow.EXIFData + " " + imagesRow.AbsoluteUri, Encoding.UTF8.CodePage, imagesRow.FullTextIndexType, strength, managedImage.DiscoveryPath, 1);
                            }
                            else
                            {
                                CreateDocument(document, imagesRow.ID, DiscoveryType.Image, imagesRow.AbsoluteUri, imagesRow.AbsoluteUri, Encoding.UTF8.CodePage, imagesRow.FullTextIndexType, strength, managedImage.DiscoveryPath, 1);
                            }

                            Console.WriteLine("Image: '" + imagesRow.AbsoluteUri + "' indexed. (" + i + " of " + (imageIDUpperBound - imageIDLowerBound + 1) + ")");
                        }
                        else
                        {
                            Console.WriteLine("Image: " + i + " was not found. (" + i + " of " + (imageIDUpperBound - imageIDLowerBound + 1) + ")");
                        }
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine("Image: " + i + " was not indexed. (" + i + " of " + (imageIDUpperBound - imageIDLowerBound + 1) + ")");

                        arachnodeDAO.InsertException(null, null, exception, false);
                    }
                }
            }

            //WebPages
            if (_indexWebPages)
            {
                WebPageManager<TArachnodeDAO> webPageManager = new WebPageManager<TArachnodeDAO>(_applicationSettings, _webSettings, discoveryManager, htmlManager, arachnodeDAO);

                for (long i = webPageIDLowerBound; i <= webPageIDUpperBound; i++)
                {
                    try
                    {
                        //get the WebPage from the database.  we need the source data as we don't store this in the index.
                        //even though most of the fields are available in the Document, the WebPage is the authoritative source, so we'll use that for all of the fields.
                        ArachnodeDataSet.WebPagesRow webPagesRow = arachnodeDAO.GetWebPage(i.ToString());

                        if (webPagesRow != null)
                        {
                            Encoding encoding = Encoding.GetEncoding(webPagesRow.CodePage);

                            if (webPagesRow.Source == null || webPagesRow.Source.Length == 0)
                            {
                                using (StreamReader streamReader = File.OpenText(discoveryManager.GetDiscoveryPath(_applicationSettings.DownloadedWebPagesDirectory, webPagesRow.AbsoluteUri, webPagesRow.FullTextIndexType)))
                                {
                                    webPagesRow.Source = encoding.GetBytes(streamReader.ReadToEnd());
                                }
                            }

                            /**/

                            float strength = (float)reportingManager.GetStrengthForHost(webPagesRow.AbsoluteUri);

                            if (strength == 0)
                            {
                                strength = (float)reportingManager.GetPriorityForHost(webPagesRow.AbsoluteUri);
                            }

                            /**/

                            //manage the WebPage to update the discovery path if needed and to ensure the WebPage is saved to disk.
                            ManagedWebPage managedWebPage = webPageManager.ManageWebPage(webPagesRow.ID, webPagesRow.AbsoluteUri, webPagesRow.Source, encoding, webPagesRow.FullTextIndexType, _applicationSettings.ExtractWebPageMetaData, _applicationSettings.InsertWebPageMetaData, _applicationSettings.SaveDiscoveredWebPagesToDisk);

                            Document document = new Document();

                            document.Add(new Field("created", webPagesRow.InitiallyDiscovered.Date.ToString("yyyyMMdd"), Field.Store.YES, Field.Index.NOT_ANALYZED));

                            if (!webPagesRow.IsLastModifiedNull())
                            {
                                document.Add(new Field("updated", webPagesRow.LastModified.Date.ToString("yyyyMMdd"), Field.Store.YES, Field.Index.NOT_ANALYZED));
                            }

                            CreateDocument(document, webPagesRow.ID, DiscoveryType.WebPage, webPagesRow.AbsoluteUri, encoding.GetString(webPagesRow.Source), encoding.CodePage, webPagesRow.FullTextIndexType, strength, managedWebPage.DiscoveryPath, 1);

                            Console.WriteLine("WebPage: '" + webPagesRow.AbsoluteUri + "' indexed. (" + i + " of " + (webPageIDUpperBound - webPageIDLowerBound + 1) + ")");
                        }
                        else
                        {
                            Console.WriteLine("WebPageID: " + i + " was not found. (" + i + " of " + (webPageIDUpperBound - webPageIDLowerBound + 1) + ")");
                        }
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine("WebPageID: " + i + " was not indexed. (" + i + " of " + (webPageIDUpperBound - webPageIDLowerBound + 1) + ")");

                        arachnodeDAO.InsertException(null, null, exception, false);
                    }
                }
            }
        }
    }
#pragma warning restore 612,618
}