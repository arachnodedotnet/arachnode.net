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

using Arachnode.Configuration;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.Enums;
using Lucene.Net.Documents;

#endregion

namespace Arachnode.Plugins.CrawlActions
{
    internal class CustomManageLuceneDotNetIndexes<TArachnodeDAO> : ManageLuceneDotNetIndexes<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        public CustomManageLuceneDotNetIndexes(ApplicationSettings applicationSettings, WebSettings webSettings)
            : base(applicationSettings, webSettings)
        {
        }

        protected override void CreateDocument(Document document, long discoveryID, DiscoveryType discoveryType, string absoluteUri, string contentToIndex, int codePage, string fullTextIndexType, float strength, string discoveryPath, int threadNumber)
        {
            //a bare bones example of what you could do to add a new field to the index...
            //if (discoveryType == DiscoveryType.WebPage)
            //{
            //    HtmlDocument htmlDocument = new HtmlDocument();

            //    htmlDocument.LoadHtml(contentToIndex);

            //    HtmlNode htmlNode = htmlDocument.DocumentNode.SelectSingleNode("/html/body");

            //    string body = htmlNode.InnerText;

            //    document.Add(new Field("body", "Mike", Field.Store.NO, Field.Index.UN_TOKENIZED));
            //}

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
    }
}