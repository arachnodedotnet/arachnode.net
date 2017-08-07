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
using System.Configuration;
using System.Data.SqlTypes;

#endregion

namespace Arachnode.Configuration
{
    /// <summary>
    /// 	Contains all core application settings.  The ConfigurationManager will correct incorrect Application settings and throw an exception if an invalid configuration is set.
    /// </summary>
    public class ApplicationSettings
    {
        private SqlString _connectionString;

        /// <summary>
        /// Should the Crawler attempt to crawl at a rate just below what a web server will serve Discoveries at?  Use this setting if you experience timeouts in downloading web site content.
        /// </summary>
        public bool AutoThrottleHttpWebRequests;

        /// <summary>
        /// Sets the WebClient's HttpWebRequest's 'Accept' HttpHeader to the specific value.  This value mirrors Chrome's 'Accept' header value.
        /// </summary>
        public string Accept { get; set; }

        /// <summary>
        /// Sets the WebClient's HttpWebRequest's 'Accept-Encoding' HttpHeader to the specific value.  This value mirrors Chrome's 'Accept-Encoding' header value.
        /// </summary>
        public string AcceptEncoding { get; set; }

        /// <summary>
        /// Should the WebClient allow automatic redirection to another AbsoluteUri?
        /// </summary>
        public bool AllowAutoRedirect { get; set; }

        /// <summary>
        /// How long should a Discovery remain in the Cache before being considered for expiration?
        /// </summary>
        public int DiscoverySlidingExpirationInSeconds { get; set; }

        /// <summary>
        /// Should WebExceptions be output to the Console?
        /// </summary>
        public bool OutputWebExceptions { get; set; }

        /// <summary>
        /// The maximum number of automatic redirections allowed by the WebClient before throwing an Exception.
        /// </summary>
        public int MaximumNumberOfAutoRedirects { get; set; }

        /// <summary>
        /// The CrawlRequestManager provides a thread for each Crawl thread which submits Discoveries (Files/HyperLinks/Images/WebPages) to the database asynchronously so that the WebClient associated with the Crawl thread may continue downloading without waiting for the database inserts/updates to complete.  Enabling this typically improves crawling performance.
        /// </summary>
        public bool ProcessDiscoveriesAsynchronously { get; set; }

        /// <summary>
        /// Sets the 'Referer' HttpRequestHeader to the CrawlRequests's Parent's AbsoluteUri.
        /// </summary>
        public bool SetRefererToParentAbsoluteUri { get; set; }

        /// <summary>
        /// Should Cookies be processed when returned from the WebSite?  Custom user Cookies will still be processed.
        /// </summary>
        public bool ProcessCookies { get; set; }

        /// <summary>
        /// 	Gets or sets a value indicating whether [assign crawl request priorities for files].
        /// 	Should File Priorities be set according to values in the Priorities table?  If set to 'true', File priorities will be set according to the 'Host' of the File AbsoluteUri.  If set to 'false' the File will be created as a CrawlRequest with a Priority of '0'.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [assign crawl request priorities for files]; otherwise, <c>false</c>.
        /// </value>
        public bool AssignCrawlRequestPrioritiesForFiles { get; set; }

        /// <summary>
        /// 	Gets or sets a value indicating whether [assign crawl request priorities for hyper links].
        /// 	Should HyperLink Priorities be set according to values in the Priorities table?  If set to 'true', HyperLink priorities will be set according to the 'Host' of the HyperLink AbsoluteUri.  If set to 'false' the HyperLink will be created as a CrawlRequest with a Priority of '0'.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [assign crawl request priorities for hyper links]; otherwise, <c>false</c>.
        /// </value>
        public bool AssignCrawlRequestPrioritiesForHyperLinks { get; set; }

        /// <summary>
        /// 	Gets or sets a value indicating whether [assign crawl request priorities for images].
        /// 	Should Image Priorities be set according to values in the Priorities table?  If set to 'true', Image priorities will be set according to the 'Host' of the Image AbsoluteUri.  If set to 'false' the Image will be created as a CrawlRequest with a Priority of '0'.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [assign crawl request priorities for images]; otherwise, <c>false</c>.
        /// </value>
        public bool AssignCrawlRequestPrioritiesForImages { get; set; }

        /// <summary>
        /// 	Gets or sets a value indicating whether [assign crawl request priorities for web pages].
        /// 	Should WebPage Priorities be set according to values in the Priorities table?  If set to 'true', WebPage priorities will be set according to the 'Host' of the WebPage AbsoluteUri.  If set to 'false' the WebPage will be created as a CrawlRequest with a Priority of '0'.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [assign crawl request priorities for web pages]; otherwise, <c>false</c>.
        /// </value>
        public bool AssignCrawlRequestPrioritiesForWebPages { get; set; }

        /// <summary>
        /// 	Should EmailAddresses be assigned (parsed) from a WebPage Discovery?
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [assign email address discoveries]; otherwise, <c>false</c>.
        /// </value>
        public bool AssignEmailAddressDiscoveries { get; set; }

        /// <summary>
        /// 	Should Files and Images be assigned (parsed) from a WebPage Discovery?
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [assign file and image discoveries]; otherwise, <c>false</c>.
        /// </value>
        public bool AssignFileAndImageDiscoveries { get; set; }

        /// <summary>
        /// 	Should HyperLinks be assigned (parsed) from a WebPage Discovery?
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [assign hyper link discoveries]; otherwise, <c>false</c>.
        /// </value>
        public bool AssignHyperLinkDiscoveries { get; set; }

        /// <summary>
        /// 	Upon submission to the database, will AbsoluteUris be classified according to Scheme, Domain, Host and Extension?
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [classify absolute uris]; otherwise, <c>false</c>.
        /// </value>
        public bool ClassifyAbsoluteUris { get; set; }

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
                    //if you encounter an exception here, add a reference to 'Configuration'...
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
        /// 	The UNC\relative path where console output logs should be written to disk.
        /// </summary>
        /// <value>The console output logs directory.</value>
        public string ConsoleOutputLogsDirectory { get; set; }

        /// <summary>
        /// 	The duration until a CrawlRequest times out.
        /// </summary>
        /// <value>The crawl request timeout in minutes.</value>
        public double CrawlRequestTimeoutInMinutes { get; set; }

        /// <summary>
        /// 	Should CrawlRequests stored in the database be converted to CrawlRequests for crawling?
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [create crawl requests from database crawl requests]; otherwise, <c>false</c>.
        /// </value>
        public bool CreateCrawlRequestsFromDatabaseCrawlRequests { get; set; }

        /// <summary>
        /// 	Should Files stored in the database be converted to CrawlRequests for crawling?
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [create crawl requests from database files]; otherwise, <c>false</c>.
        /// </value>
        public bool CreateCrawlRequestsFromDatabaseFiles { get; set; }

        /// <summary>
        /// 	Should HyperLinks stored in the database be converted to CrawlRequests for crawling?
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [create crawl requests from database hyper links]; otherwise, <c>false</c>.
        /// </value>
        public bool CreateCrawlRequestsFromDatabaseHyperLinks { get; set; }

        /// <summary>
        /// 	Should Images stored in the database be converted to CrawlRequests for crawling?
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [create crawl requests from database images]; otherwise, <c>false</c>.
        /// </value>
        public bool CreateCrawlRequestsFromDatabaseImages { get; set; }

        /// <summary>
        /// 	Should WebPages stored in the database be converted to CrawlRequests for crawling?
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [create crawl requests from database web pages]; otherwise, <c>false</c>.
        /// </value>
        public bool CreateCrawlRequestsFromDatabaseWebPages { get; set; }

        /// <summary>
        /// 	The desired number of megabytes of RAM memory that arachnode.net will use.  Try to set this to at least 1/3 of the available RAM.  Higher values equal increased performance.  If crawling on the database server, leave room for SQL's cache, favoring SQL's cache in the number of megabytes allocated.
        /// </summary>
        /// <value>The desired maximum memory usage in megabytes.</value>
        public int DesiredMaximumMemoryUsageInMegabytes { get; set; }

        /// <summary>
        /// 	The UNC/relative path where downloaded Files should be saved.
        /// </summary>
        /// <value>The downloaded files directory.</value>
        public string DownloadedFilesDirectory { get; set; }

        /// <summary>
        /// 	The UNC/relative path where downloaded Images should be saved.
        /// </summary>
        /// <value>The downloaded images directory.</value>
        public string DownloadedImagesDirectory { get; set; }

        /// <summary>
        /// 	The UNC/relative path where downloaded WebPages should be saved.
        /// </summary>
        /// <value>The downloaded web pages directory.</value>
        public string DownloadedWebPagesDirectory { get; set; }

        /// <summary>
        /// 	Should program output be directed to the console?  Logging to the Console is an expensive operation and should be disabled in production systems.
        /// </summary>
        /// <value><c>true</c> if [enable console output]; otherwise, <c>false</c>.</value>
        public bool EnableConsoleOutput { get; set; }

        /// <summary>
        /// 	Should the FileManager extract File MetaData?
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [extract file meta data]; otherwise, <c>false</c>.
        /// </value>
        public bool ExtractFileMetaData { get; set; }

        /// <summary>
        /// 	Should the ImageManager extract Image MetaData?
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [extract image meta data]; otherwise, <c>false</c>.
        /// </value>
        public bool ExtractImageMetaData { get; set; }

        /// <summary>
        /// 	Should the WebPageManager extract WebPage MetaData?  The HtmlAgilityPack is extremely RAM hungry.  Setting this property to 'true' will have an adverse effect on performance.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [extract web page meta data]; otherwise, <c>false</c>.
        /// </value>
        public bool ExtractWebPageMetaData { get; set; }

        /// <summary>
        /// 	The maximum number of retries that a Crawl will attempt should the WebPage not respond as a result of remote WebServer throttling.
        /// </summary>
        public int HttpWebRequestRetries { get; set; }

        /// <summary>
        /// Should discovered CrawlRequests be submitted to the database?  Setting this to 'false' is an advanced technique and in most installations should be set to 'true'.
        /// </summary>
        public bool InsertCrawlRequests { get; set; }

        /// <summary>
        /// 	Should Crawls submit DisallowedAbsoluteUri Discoveries to the database?
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [insert disallowed absolute URI discoveries]; otherwise, <c>false</c>.
        /// </value>
        public bool InsertDisallowedAbsoluteUriDiscoveries { get; set; }

        /// <summary>
        /// 	Should Crawls submit DisallowedAbsoluteUris to the database?
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [insert disallowed absolute uris]; otherwise, <c>false</c>.
        /// </value>
        public bool InsertDisallowedAbsoluteUris { get; set; }

        /// <summary>
        /// Should discovered Discovery Discoveries be submitted to the database?  Set this to 'true' if it is important to know where a disallowed Discovery originated from.  The default value is 'false'.
        /// </summary>
        public bool InsertDisallowedDiscoveries { get; set; }

        /// <summary>
        /// Should discovered Discoveries be submitted to the database?  Setting this to 'false' is an advanced technique and in most installations should be set to 'true'.
        /// </summary>
        public bool InsertDiscoveries { get; set; }

        /// <summary>
        /// Should File 'ResponseHeaders' be inserted into the database?
        /// </summary>
        public bool InsertFileResponseHeaders { get; set; }

        /// <summary>
        /// Should Image 'ResponseHeaders' be inserted into the database?
        /// </summary>
        public bool InsertImageResponseHeaders { get; set; }

        /// <summary>
        /// 	Should Crawls submit EmailAddress Discoveries to the database?
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [insert email address discoveries]; otherwise, <c>false</c>.
        /// </value>
        public bool InsertEmailAddressDiscoveries { get; set; }

        /// <summary>
        /// 	Should Crawls submit EmailAddresses to the database?
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [insert email addresses]; otherwise, <c>false</c>.
        /// </value>
        public bool InsertEmailAddresses { get; set; }

        /// <summary>
        /// 	Should Crawls, et. al. submit Exceptions to the database?
        /// </summary>
        /// <value><c>true</c> if [insert exceptions]; otherwise, <c>false</c>.</value>
        public bool InsertExceptions { get; set; }

        /// <summary>
        /// 	Should Crawls submit File Discoveries to the database?
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [insert file discoveries]; otherwise, <c>false</c>.
        /// </value>
        public bool InsertFileDiscoveries { get; set; }

        /// <summary>
        /// 	Should the FileManager submit File MetaData to the database?
        /// </summary>
        /// <value><c>true</c> if [insert file meta data]; otherwise, <c>false</c>.</value>
        public bool InsertFileMetaData { get; set; }

        /// <summary>
        /// 	Should the FileManager submit Files to the database?
        /// </summary>
        /// <value><c>true</c> if [insert files]; otherwise, <c>false</c>.</value>
        public bool InsertFiles { get; set; }

        /// <summary>
        /// 	Should the FileManager submit File Sources to the database?
        /// </summary>
        /// <value><c>true</c> if [insert file source]; otherwise, <c>false</c>.</value>
        public bool InsertFileSource { get; set; }

        /// <summary>
        /// 	Should Crawls submit HyperLinks to the database?
        /// </summary>
        /// <value><c>true</c> if [insert hyper links]; otherwise, <c>false</c>.</value>
        public bool InsertHyperLinks { get; set; }

        /// <summary>
        /// 	Should Crawls submit HyperLink Discoveries to the database?
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [insert hyper link discoveries]; otherwise, <c>false</c>.
        /// </value>
        public bool InsertHyperLinkDiscoveries { get; set; }

        /// <summary>
        /// 	Should Crawls submit Image Discoveries to the database?
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [insert image discoveries]; otherwise, <c>false</c>.
        /// </value>
        public bool InsertImageDiscoveries { get; set; }

        /// <summary>
        /// 	Should the ImageManager submit Image MetaData to the database?
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [insert image meta data]; otherwise, <c>false</c>.
        /// </value>
        public bool InsertImageMetaData { get; set; }

        /// <summary>
        /// 	Should the ImageManager submit Images to the database?
        /// </summary>
        /// <value><c>true</c> if [insert images]; otherwise, <c>false</c>.</value>
        public bool InsertImages { get; set; }

        /// <summary>
        /// 	Should the ImageManager submit Image Sources to the database?
        /// </summary>
        /// <value><c>true</c> if [insert image source]; otherwise, <c>false</c>.</value>
        public bool InsertImageSource { get; set; }

        /// <summary>
        /// 	Should the WebPageManager submit WebPage MetaData to the database?
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [insert web page meta data]; otherwise, <c>false</c>.
        /// </value>
        public bool InsertWebPageMetaData { get; set; }

        /// <summary>
        /// 	Should the WebPageManager submit WebPages to the database?
        /// </summary>
        /// <value><c>true</c> if [insert web pages]; otherwise, <c>false</c>.</value>
        public bool InsertWebPages { get; set; }

        /// <summary>
        /// Should WebPage 'ResponseHeaders' be inserted into the database?
        /// </summary>
        public bool InsertWebPageResponseHeaders { get; set; }

        /// <summary>
        /// 	Should the WebPageManager submit WebPage Sources to the database?
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [insert web page source]; otherwise, <c>false</c>.
        /// </value>
        public bool InsertWebPageSource { get; set; }

        /// <summary>
        /// 	Gets or sets the maximum number of crawl requests to create per batch.
        /// 	The maximum number of CrawlRequests created from the CrawlRequests, Files, HyperLinks, Images and/or WebPages tables when the Engine requests CrawlRequests to crawl.
        /// </summary>
        /// <value>The maximum number of crawl requests to create per batch.</value>
        public int MaximumNumberOfCrawlRequestsToCreatePerBatch { get; set; }

        /// <summary>
        /// 	The maximum number of crawl threads spawned by the Engine.
        ///     Many ISP's limit the number of concurrent outbound connections to 10.  If you find CrawRequests are being cancelled or throttled frequently reduce this number.
        /// </summary>
        /// <value>The maximum number of crawl threads.</value>
        public int MaximumNumberOfCrawlThreads { get; set; }

        /// <summary>
        /// 	The maximum number of Hosts and Priorities to select for use by the PriorityQueue and the ReportingManager.
        /// </summary>
        public int MaximumNumberOfHostsAndPrioritiesToSelect { get; set; }

        /// <summary>
        /// 	Should console output be redirected to logs on disk?
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [output console to logs]; otherwise, <c>false</c>.
        /// </value>
        public bool OutputConsoleToLogs { get; set; }

        /// <summary>
        /// 	Should Crawl statistics be output to the console?
        /// </summary>
        /// <value><c>true</c> if [output statistics]; otherwise, <c>false</c>.</value>
        [Obsolete]
        public bool OutputStatistics { get; set; }

        /// <summary>
        /// //ANODET: Currently in development...
        /// </summary>
        public bool OverrideDatabaseConfigurationWithXmlConfiguration { get; set; }

        /// <summary>
        /// 	Should discovered Files be saved to disk?
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [save discovered files to disk]; otherwise, <c>false</c>.
        /// </value>
        public bool SaveDiscoveredFilesToDisk { get; set; }

        /// <summary>
        /// 	Should discovered Images be saved to disk?
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [save discovered images to disk]; otherwise, <c>false</c>.
        /// </value>
        public bool SaveDiscoveredImagesToDisk { get; set; }

        /// <summary>
        /// 	Should discovered WebPages be saved to disk?
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [save discovered web pages to disk]; otherwise, <c>false</c>.
        /// </value>
        public bool SaveDiscoveredWebPagesToDisk { get; set; }

        /// <summary>
        /// Adds a suffix to the CacheKey used by the cache.  If multiple instances of the Crawler are used simultaneously on the same machine in the same process, each Crawler instance should use a distinct UniqueIdentifier as the instances use a common in-memory cache for state verification.
        /// </summary>
        public string UniqueIdentifier { get; set; }

        /// <summary>
        /// 	The User-Agent HttpRequest header used to identify Crawls.
        /// </summary>
        /// <value>The user agent.</value>
        public string UserAgent { get; set; }

        /// <summary>
        /// 	Should console output be verbose?  Verbose output sends the AbsoluteUri of eacu Discovery to the console.
        /// </summary>
        /// <value><c>true</c> if [verbose output]; otherwise, <c>false</c>.</value>
        public bool VerboseOutput { get; set; }
    }
}