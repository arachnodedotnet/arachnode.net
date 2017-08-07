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

using System.Configuration;
using System.Data.SqlTypes;

#endregion

namespace Arachnode.Configuration
{
    /// <summary>
    /// 	Contains all core web settings.
    /// </summary>
    public class WebSettings
    {
        private SqlString _connectionString;

        /// <summary>
        /// 	The maximum time in minutes that a cached Query or Hits object will remain in the Cache.  This setting also controls how often the IndexSearcher will refresh, allowing new search results.
        /// </summary>
        /// <value>The cache timeout in minutes.</value>
        public int CacheTimeoutInMinutes { get; set; }

        /// <summary>
        /// 	The ConnectionString used for connecting to the arachnode.net Database.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString
        {
            get
            {
                if (_connectionString.IsNull)
                {
                    if (ConfigurationManager.ConnectionStrings["arachnode_net_ConnectionString"] != null)
                    {
                        _connectionString = ConfigurationManager.ConnectionStrings["arachnode_net_ConnectionString"].ConnectionString;
                    }
                    else
                    {
                        _connectionString = "Data Source=.;Initial Catalog=arachnode.net;Integrated Security=True;Connection Timeout=3600;Max Pool Size=100000";
                    }
                }
                return _connectionString.Value;
            }
            set { _connectionString = value; }
        }

        /// <summary>
        /// 	Gets or sets a value indicating whether [create crawl requests for missing files and images].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [create crawl requests for missing files and images]; otherwise, <c>false</c>.
        /// </value>
        public bool CreateCrawlRequestsForMissingFilesAndImages { get; set; }

        /// <summary>
        /// 	The IIS virtual directory for the downloaded Files.  The leading backslash is necessary.
        /// </summary>
        public string DownloadedFilesVirtualDirectory { get; set; }

        /// <summary>
        /// 	The IIS virtual directory for the downloaded Images.  The leading backslash is necessary.
        /// </summary>
        public string DownloadedImagesVirtualDirectory { get; set; }

        /// <summary>
        /// 	The UNC/relative path where the lucene.net index is located.
        /// </summary>
        /// <value>The lucene dot net index directory.</value>
        public string LuceneDotNetIndexDirectory { get; set; }

        /// <summary>
        /// 	The maximum number of characters that will be displayed on Search.aspx before the title is truncated and followed by an ellipses.
        /// </summary>
        /// <value>The maximum length of the page title.</value>
        public int MaximumPageTitleLength { get; set; }

        /// <summary>
        /// 	Gets or sets the maximum number of documents to return per search.
        /// </summary>
        /// <value>The maximum number of Documents to return per search.</value>
        public int MaximumNumberOfDocumentsToReturnPerSearch { get; set; }

        /// <summary>
        /// 	The number of Documents shown per search result page.
        /// </summary>
        /// <value>The size of the page.</value>
        public int PageSize { get; set; }
    }
}