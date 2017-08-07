using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Arachnode.Configuration;
using Arachnode.DataSource;

namespace Arachnode.DataAccess.Value.Interfaces
{
    public interface IArachnodeDAO
    {
        /// <summary>
        /// 	The last ExceptionID submitted to table 'DisallowedAbsoluteUris'.
        /// </summary>
        /// <value>The last disallowed absolute URI ID.</value>
        long? LastDisallowedAbsoluteUriID { get; }

        /// <summary>
        /// 	The last EmailAddressID submitted to table 'EmailAddresses'.
        /// </summary>
        /// <value>The last email address ID.</value>
        long? LastEmailAddressID { get; }

        /// <summary>
        /// 	The last ExceptionID submitted to table 'Exceptions'.
        /// </summary>
        /// <value>The last exception ID.</value>
        long? LastExceptionID { get; }

        /// <summary>
        /// 	The last Message submitted to table 'Exceptions'.
        /// </summary>
        /// <value>The last exception message.</value>
        string LastExceptionMessage { get; }

        /// <summary>
        /// 	The last FileID submitted to table 'Files'.
        /// </summary>
        /// <value>The last file ID.</value>
        long? LastFileID { get; }

        /// <summary>
        /// 	The last HyperLinksID submitted to table 'HyperLinks'.
        /// </summary>
        /// <value>The last hyper link ID.</value>
        long? LastHyperLinkID { get; }

        /// <summary>
        /// 	The last ImageID submitted to table 'Images'.
        /// </summary>
        /// <value>The last image ID.</value>
        long? LastImageID { get; }

        /// <summary>
        /// 	The last WebPageID submitted to table 'WebPages'.
        /// </summary>
        long? LastWebPageID { get; }

        ApplicationSettings ApplicationSettings { get; set; }
        WebSettings WebSettings { get; set; }

        /// <summary>
        /// Opens all database connections.  Doing so before crawling allows CRUD operations to operate faster than opening and closing the connection each time an operation is requested.
        /// Extremely advanced...  ask if you have performance problems...
        /// </summary>
        void OpenCommandConnections();

        /// <summary>
        /// Closes all database connections.
        /// Extremely advanced...  ask if you have performance problems...
        /// </summary>
        void CloseCommandConnections();

        /// <summary>
        /// Closes all dastabase connections.  Called by the ArachnodeDAO destructor.
        /// Extremely advanced...  ask if you have performance problems...
        /// </summary>
        /// <param name="dbCommands"></param>
        void CloseCommandConnections(IEnumerable<IDbCommand> dbCommands);

        /// <summary>
        /// Checks the Database Version.  Used by the Crawler to ensure the client is communicating with the proper Database Version.
        /// </summary>
        void CheckVersion();

        /// <summary>
        /// 	Deletes a row from table 'CrawlRequests'.
        /// 	Calling this method with absoluteUri1 set to 'null' and absoluteUri2 set to 'null' deletes all CrawlRequests from the 'CrawlRequests' table.
        /// </summary>
        /// <param name = "absoluteUri1">The absolute uri1.</param>
        /// <param name = "absoluteUri2">The absolute uri2.</param>
        void DeleteCrawlRequest(string absoluteUri1, string absoluteUri2);

        /// <summary>
        /// 	Deletes all rows from table 'Discoveries'.
        /// </summary>
        void DeleteDiscoveries();

        /// <summary>
        /// 	Deletes a row from table 'WebPages'.
        /// </summary>
        /// <param name = "webPageAbsoluteUriOrID">The WebPage AbsoluteUri or ID.</param>
        void DeleteWebPage(string webPageAbsoluteUriOrID);

        /// <summary>
        /// 	Executes dynamic SQL.
        /// </summary>
        /// <param name = "query">The query.</param>
        void ExecuteSql(string query);

        /// <summary>
        /// 	Executes dynamic SQL.
        /// </summary>
        /// <param name = "query">The query.</param>
        DataTable ExecuteSql2(string query);

        /// <summary>
        /// 	DataTypes represent mappings between the ResponseHeader 'Content-type:' and the type to be used by SQL for Full-text indexing.
        /// 	If you find that content you wish to crawl isn't being crawled, check the table 'DisallowedAbsoluteUris'.  If you see 'Disallowed by unassigned DataType', add the DataType to table 'AllowedDataTypes'.
        /// </summary>
        /// <returns></returns>
        ArachnodeDataSet.AllowedDataTypesDataTable GetAllowedDataTypes();

        ArachnodeDataSet.AllowedExtensionsDataTable GetAllowedExtensions();

        ArachnodeDataSet.AllowedSchemesDataTable GetAllowedSchemes();

        /// <summary>
        /// 	Gets the configuration.
        /// </summary>
        /// <returns></returns>
        ArachnodeDataSet.ConfigurationDataTable GetConfiguration();

        /// <summary>
        /// 	Gets the crawl actions.  Used by the ActionManager.
        /// </summary>
        /// <returns></returns>
        ArachnodeDataSet.CrawlActionsDataTable GetCrawlActions();

        /// <summary>
        /// 	Gets the crawl rules.  Used by the RuleManager.
        /// </summary>
        /// <returns></returns>
        ArachnodeDataSet.CrawlRulesDataTable GetCrawlRules();

        /// <summary>
        /// 	Gets all ContentTypes.
        /// 	A ContentType would be text/html, image/jpeg, etc.
        /// </summary>
        /// <returns></returns>
        ArachnodeDataSet.ContentTypesDataTable GetContentTypes();

        /// <summary>
        /// 	Returns a DataTable containing the database representation of a CrawlRequest.
        /// 	This procedure creates CrawlRequests for the Engine to process from rows in table 'CrawlRequests'.
        /// </summary>
        /// <param name = "maximumNumberOfCrawlRequestsToCreatePerBatch">See: ApplicationSettings.MaximumNumberOfCrawlRequestsToCreatePerBatch</param>
        /// <param name = "createCrawlRequestsFromDatabaseCrawlRequests">See: ApplicationSettings.CreateCrawlRequestsFromDatabaseCrawlRequests</param>
        /// <param name = "createCrawlRequestsFromDatabaseFiles">See: ApplicationSettings.CreateCrawlRequestsFromDatabaseFiles</param>
        /// <param name = "assignCrawlRequestPrioritiesForFiles">if set to <c>true</c> [assign crawl request priorities for files].</param>
        /// <param name = "createCrawlRequestsFromDatabaseHyperLinks">See: ApplicationSettings.CreateCrawlRequestsFromDatabaseHyperLinks</param>
        /// <param name = "assignCrawlRequestPrioritiesForHyperLinks">if set to <c>true</c> [assign crawl request priorities for hyper links].</param>
        /// <param name = "createCrawlRequestsFromDatabaseImages">if set to <c>true</c> [create crawl requests from database images].</param>
        /// <param name = "assignCrawlRequestPrioritiesForImages">if set to <c>true</c> [assign crawl request priorities for images].</param>
        /// <param name = "createCrawlRequestsFromDatabaseWebPages">See: ApplicationSettings.CreateCrawlRequestsFromDatabaseWebPages</param>
        /// <param name = "assignCrawlRequestPrioritiesForWebPages">if set to <c>true</c> [assign crawl request priorities for web pages].</param>
        /// <returns></returns>
        ArachnodeDataSet.CrawlRequestsDataTable GetCrawlRequests(int maximumNumberOfCrawlRequestsToCreatePerBatch, bool createCrawlRequestsFromDatabaseCrawlRequests, bool createCrawlRequestsFromDatabaseFiles, bool assignCrawlRequestPrioritiesForFiles, bool createCrawlRequestsFromDatabaseHyperLinks, bool assignCrawlRequestPrioritiesForHyperLinks, bool createCrawlRequestsFromDatabaseImages, bool assignCrawlRequestPrioritiesForImages, bool createCrawlRequestsFromDatabaseWebPages, bool assignCrawlRequestPrioritiesForWebPages);

        /// <summary>
        /// 	Gets a Discovery from the Discoveries table.  If the Discovery isn't present, null is returned.
        /// 	This method is used by the Cache to determine if a Discovery has been encountered in a crawl run.
        /// 	If a Discovery hasn't been 'Discovered', then it is eligible to have a CrawlRequest created from it and to be crawled.
        /// </summary>
        /// <param name = "absoluteUri">The absolute URI.</param>
        /// <returns></returns>
        ArachnodeDataSet.DiscoveriesRow GetDiscovery(string absoluteUri);

        /// <summary>
        /// 	Gets the Discoveries.
        /// </summary>
        /// <param name = "numberOfDiscoveriesToReturn">The number of discoveries to return.</param>
        /// <returns></returns>
        ArachnodeDataSet.DiscoveriesDataTable GetDiscoveries(int numberOfDiscoveriesToReturn);

        /// <summary>
        /// 	Selects a row from table 'DisallowedAbsoluteUris' by AbsoluteUri or ID.
        /// </summary>
        /// <param name = "disallowedAbsoluteUriAbsoluteUriOrID">The disallowedAbsoluteUri absolute URI or ID.</param>
        /// <returns></returns>
        ArachnodeDataSet.DisallowedAbsoluteUrisRow GetDisallowedAbsoluteUri(string disallowedAbsoluteUriAbsoluteUriOrID);

        /// <summary>
        /// 	Gets the DisallowedAbsoluteUris.
        /// </summary>
        /// <param name = "numberOfDisallowedAbsoluteUrisToReturn">The number of disallowedAbsoluteUris to return.</param>
        /// <returns></returns>
        ArachnodeDataSet.DisallowedAbsoluteUrisDataTable GetDisallowedAbsoluteUris(int numberOfDisallowedAbsoluteUrisToReturn);

        /// <summary>
        /// 	Selects a row from table 'DisallowedAbsoluteUris_Discoveries' by DisallowedAbsoluteUriID.
        /// </summary>
        /// <param name = "disallowedAbsoluteUriID">The disallowedAbsoluteUri ID.</param>
        /// <returns></returns>
        ArachnodeDataSet.DisallowedAbsoluteUrisDiscoveriesDataTable GetDisallowedAbsoluteUriDiscoveries(long disallowedAbsoluteUriID);

        ArachnodeDataSet.DisallowedDomainsDataTable GetDisallowedDomains();
        ArachnodeDataSet.DisallowedExtensionsDataTable GetDisallowedExtensions();
        ArachnodeDataSet.DisallowedFileExtensionsDataTable GetDisallowedFileExtensions();
        ArachnodeDataSet.DisallowedHostsDataTable GetDisallowedHosts();
        ArachnodeDataSet.DisallowedSchemesDataTable GetDisallowedSchemes();
        ArachnodeDataSet.DisallowedWordsDataTable GetDisallowedWords();

        /// <summary>
        /// 	Gets all Domains.
        /// </summary>
        /// <returns></returns>
        ArachnodeDataSet.DomainsDataTable GetDomains();

        /// <summary>
        /// 	Selects a row from table 'EmailAddresses' by AbsoluteUri or ID.
        /// </summary>
        /// <param name = "emailAddressAbsoluteUriOrID">The emailAddress absolute URI or ID.</param>
        /// <returns></returns>
        ArachnodeDataSet.EmailAddressesRow GetEmailAddress(string emailAddressAbsoluteUriOrID);

        /// <summary>
        /// 	Selects a row from table 'EmailAddresses_Discoveries' by EmailAddressID.
        /// </summary>
        /// <param name = "emailAddressID">The emailAddress ID.</param>
        /// <returns></returns>
        ArachnodeDataSet.EmailAddressesDiscoveriesDataTable GetEmailAddressDiscoveries(long emailAddressID);

        /// <summary>
        /// 	Gets the engine actions.  Used by the ActionManager.
        /// </summary>
        /// <returns></returns>
        ArachnodeDataSet.EngineActionsDataTable GetEngineActions();

        /// <summary>
        /// 	Gets all Extensions.
        /// </summary>
        /// <returns></returns>
        ArachnodeDataSet.ExtensionsDataTable GetExtensions();

        /// <summary>
        /// 	Selects a row from table 'Files' by AbsoluteUri or ID.
        /// </summary>
        /// <param name = "fileAbsoluteUriOrID">The file absolute URI or ID.</param>
        /// <returns></returns>
        ArachnodeDataSet.FilesRow GetFile(string fileAbsoluteUriOrID);

        /// <summary>
        /// 	Selects a row from table 'Files_Discoveries' by FileID.
        /// </summary>
        /// <param name = "fileID">The file ID.</param>
        /// <returns></returns>
        ArachnodeDataSet.FilesDiscoveriesDataTable GetFileDiscoveries(long fileID);

        /// <summary>
        /// 	Gets all Hosts.
        /// </summary>
        /// <returns></returns>
        ArachnodeDataSet.HostsDataTable GetHosts();

        /// <summary>
        /// 	Gets the Database Version.  Used by the Crawler to ensure the client is communicating with the proper Database Version.
        /// </summary>
        /// <returns></returns>
        ArachnodeDataSet.VersionDataTable GetVersion();

        /// <summary>
        /// 	Selects a row from table 'Files_MetaData' by FileID.
        /// </summary>
        /// <param name = "fileID">The file ID.</param>
        /// <returns></returns>
        ArachnodeDataSet.FilesMetaDataRow GetFileMetaData(long fileID);

        /// <summary>
        /// 	Selects a row from table 'HyperLinks' by AbsoluteUri or ID.
        /// </summary>
        /// <param name = "hyperLinkAbsoluteUriOrID">The hyperLink absolute URI or ID.</param>
        /// <returns></returns>
        ArachnodeDataSet.HyperLinksRow GetHyperLink(string hyperLinkAbsoluteUriOrID);

        /// <summary>
        /// 	Selects a row from table 'HyperLinks_Discoveries' by HyperLinkID.
        /// </summary>
        /// <param name = "hyperLinkID">The hyperLink ID.</param>
        /// <returns></returns>
        ArachnodeDataSet.HyperLinksDiscoveriesDataTable GetHyperLinkDiscoveries(long hyperLinkID);

        /// <summary>
        /// 	Selects a row from table 'Images' by AbsoluteUri or ID.
        /// </summary>
        /// <param name = "imageAbsoluteUriOrID">The image absolute URI or ID.</param>
        /// <returns></returns>
        ArachnodeDataSet.ImagesRow GetImage(string imageAbsoluteUriOrID);

        /// <summary>
        /// 	Selects a row from table 'Images_Discoveries' by ImageID.
        /// </summary>
        /// <param name = "imageID">The image ID.</param>
        /// <returns></returns>
        ArachnodeDataSet.ImagesDiscoveriesDataTable GetImageDiscoveries(long imageID);

        /// <summary>
        /// 	Selects a row from table 'Images_MetaData' by ImageID.
        /// </summary>
        /// <param name = "imageID">The image ID.</param>
        /// <returns></returns>
        ArachnodeDataSet.ImagesMetaDataRow GetImageMetaData(long imageID);

        /// <summary>
        /// 	Gets all Priorities.  Priorities are used by CrawlRequests, the ReportingManager and by the lucene.net indexing functionality
        /// 	to determine crawl priority, and to enhance the boosts of lucene.net documents.  A document for "C#" from domain "XYZ" may score a higher
        /// 	relevancy score than a document for "C#" from microsoft.com, but actual popularity/priority dictates that the result from microsoft.com should
        /// 	receive a higher boost and thereby a higher overall relevancy score than the document from domain "XYZ".
        /// </summary>
        /// <returns></returns>
        ArachnodeDataSet.PrioritiesDataTable GetPriorities(int maximumNumberOfPriorities);

        /// <summary>
        /// 	Gets all Schemes.
        /// </summary>
        /// <returns></returns>
        ArachnodeDataSet.SchemesDataTable GetSchemes();

        /// <summary>
        /// 	Selects a row from table 'WebPages' by AbsoluteUri or ID.
        /// </summary>
        /// <param name = "webPageAbsoluteUriOrID">The web page absolute URI or ID.</param>
        /// <returns></returns>
        ArachnodeDataSet.WebPagesRow GetWebPage(string webPageAbsoluteUriOrID);

        long? InsertBusinessInformation(string parentWebPageAbsoluteUri, string webPageAbsoluteUri, long webPageID, string name, string address1, string address2, string city, string state, string zip, string phoneNumber, string category, string latitude, string longitude);

        /// <summary>
        /// 	Inserts the crawl request.
        /// 	See: http://arachnode.net/forums/p/564/10762.aspx#10762 if you experience difficulty with this function.
        /// 	Use this function if you want to insert CrawlRequests into the database.  The overload with the split 'currentDepth' and 'maximumDepth' parameters is intended for use by the Crawler itself.
        /// </summary>
        /// <param name = "created">The created.</param>
        /// <param name = "absoluteUri1">The absolute uri1.</param>
        /// <param name = "absoluteUri2">The absolute uri2.</param>
        /// <param name = "depth">The depth.</param>
        /// <param name = "restrictCrawlTo">The restrict crawl to.  Restricting a Crawl to a specific UriQualificationType means that the Crawl will only crawl WebPages that match the initial UriQualificationType.</param>
        /// <param name = "restrictDiscoveriesTo">The restrict discoveries to. Restricting a Crawl's Discoveries to a specific UriQualificationType means that the Crawl will only collect Discoveries that match the initial UriQualificationType.</param>
        /// <param name = "priority">The priority.</param>
        /// <returns></returns>
        long? InsertCrawlRequest(DateTime created, string absoluteUri0, string absoluteUri1, string absoluteUri2, int depth, short restrictCrawlTo, short restrictDiscoveriesTo, double priority, byte renderType, byte renderTypeForChildren);

        /// <summary>
        /// 	Inserts a row into table 'CrawlRequests'.
        /// 	See: http://arachnode.net/forums/p/564/10762.aspx#10762 if you experience difficulty with this function.
        /// 	This is an advanced function, primarily intended for use by the Crawler.
        /// </summary>
        /// <param name = "created">The created.</param>
        /// <param name = "absoluteUri1">The absolute uri1.</param>
        /// <param name = "absoluteUri2">The absolute uri2.</param>
        /// <param name = "currentDepth">The current depth.</param>
        /// <param name = "maximumDepth">The maximum depth.</param>
        /// <param name = "restrictCrawlTo">The restrict crawl to.  Restricting a Crawl to a specific UriQualificationType means that the Crawl will only crawl WebPages that match the initial UriQualificationType.</param>
        /// <param name = "restrictDiscoveriesTo">The restrict discoveries to. Restricting a Crawl's Discoveries to a specific UriQualificationType means that the Crawl will only collect Discoveries that match the initial UriQualificationType.</param>
        /// <param name = "priority">The priority.</param>
        /// <returns></returns>
        long? InsertCrawlRequest(DateTime created, string absoluteUri0, string absoluteUri1, string absoluteUri2, int currentDepth, int maximumDepth, short restrictCrawlTo, short restrictDiscoveriesTo, double priority, byte renderType, byte renderTypeForChildren);

        /// <summary>
        /// 	Inserts an AbsoluteUri into the Discoveries table.
        /// </summary>
        /// <param name = "discoveryID">The discovery ID.</param>
        /// <param name = "absoluteUri">The absolute URI.</param>
        /// <param name = "discoveryStateID">The discovery state ID.</param>
        /// <param name = "discoveryTypeID">The discovery type ID.</param>
        /// <param name = "expectFileOrImage">if set to <c>true</c> [expect file or image].</param>
        /// <param name = "numberOfTimesDiscovered">The number of times discovered.</param>
        void InsertDiscovery(long? discoveryID, string absoluteUri, int discoveryStateID, int discoveryTypeID, bool expectFileOrImage, int numberOfTimesDiscovered);

        /// <summary>
        /// 	Inserts a row into table 'DisallowedAbsoluteUris'.
        /// </summary>
        /// <param name = "contentTypeID">The content type ID.</param>
        /// <param name = "discoveryTypeID">The discovery type ID.</param>
        /// <param name = "webPageAbsoluteUri">The web page absolute URI.</param>
        /// <param name = "disallowedAbsoluteUriAbsoluteUri">The disallowed absolute URI absolute URI.</param>
        /// <param name = "reason">The reason.</param>
        long? InsertDisallowedAbsoluteUri(int contentTypeID, int discoveryTypeID, string webPageAbsoluteUri, string disallowedAbsoluteUriAbsoluteUri, string reason, bool classifyAbsoluteUris);

        /// <summary>
        /// 	Inserts a row into table 'DisallowedAbsoluteUris_Discoveries'.
        /// </summary>
        /// <param name = "webPageAbsoluteUri">The AbsoluteUri of the WebPage where the DisallowedAbsoluteUri was discovered.</param>
        /// <param name = "disallowedAbsoluteUriAbsoluteUri">The AbsoluteUri of the DisallowedAbsoluteUri.</param>
        void InsertDisallowedAbsoluteUriDiscovery(string webPageAbsoluteUri, string disallowedAbsoluteUriAbsoluteUri);

        long? InsertDocument(string parentWebPageAbsoluteUri, string webPageAbsoluteUri, byte documentTypeID, long webPageID, double weight, string field01, string field02, string field03, string field04, string field05, string field06, string field07, string field08, string field09, string field10, string field11, string field12, string field13, string field14, string field15, string field16, string field17, string field18, string field19, string field20, string fullTextIndexType);

        /// <summary>
        /// 	Inserts a row into table 'EmailAddresses'.
        /// </summary>
        /// <param name = "webPageAbsoluteUri">The AbsoluteUri of the WebPage where the EmailAddress was discovered.</param>
        /// <param name = "emailAddressAbsoluteUri">The AbsoluteUri of the EmailAddress.</param>
        /// <returns></returns>
        long? InsertEmailAddress(string webPageAbsoluteUri, string emailAddressAbsoluteUri, bool classifyAbsoluteUris);

        /// <summary>
        /// 	Inserts a row into table 'EmailAddresses_Discoveries'.
        /// </summary>
        /// <param name = "webPageAbsoluteUri">The AbsoluteUri of the WebPage where the EmailAddress was discovered.</param>
        /// <param name = "emailAddressAbsoluteUri">The AbsoluteUri of the EmailAddress.</param>
        void InsertEmailAddressDiscovery(string webPageAbsoluteUri, string emailAddressAbsoluteUri);

        /// <summary>
        /// 	Inserts a row into table 'Exceptions'.
        /// </summary>
        /// <param name = "absoluteUri1">The absolute uri1.</param>
        /// <param name = "absoluteUri2">The absolute uri2.</param>
        /// <param name = "exception">The exception.</param>
        /// <param name = "insertExceptionInWindowsApplicationLog">if set to <c>true</c> [insert exception in windows event log].</param>
        /// <returns>
        /// 	The last ExceptionID submitted to table 'Exceptions'.
        /// </returns>
        long? InsertException(string absoluteUri1, string absoluteUri2, Exception exception, bool insertExceptionInWindowsApplicationLog);

        /// <summary>
        /// 	Inserts a row into table 'Exceptions'.
        /// </summary>
        /// <param name = "absoluteUri1">The absolute uri1.</param>
        /// <param name = "absoluteUri2">The absolute uri2.</param>
        /// <param name = "insertExceptionInWindowsApplicationLog">if set to <c>true</c> [insert exception in windows event log].</param>
        /// <returns>
        /// 	The last ExceptionID submitted to table 'Exceptions'.
        /// </returns>
        long? InsertException(string absoluteUri1, string absoluteUri2, string helpLink, string message, string source, string stackTrace, bool insertExceptionInWindowsApplicationLog);

        /// <summary>
        /// 	Inserts a row into table 'Files'.
        /// </summary>
        /// <param name = "webPageAbsoluteUri">The AbsoluteUri of the WebPage where the File was discovered.</param>
        /// <param name = "fileAbsoluteUri">The AbsoluteUri of the File.</param>
        /// <param name = "responseHeaders">The ResponseHeaders returned from the HttpRequest.</param>
        /// <param name = "source">The binary data of the File.</param>
        /// <param name = "fullTextIndexType">Full type of the text index.</param>
        /// <returns></returns>
        long? InsertFile(string webPageAbsoluteUri, string fileAbsoluteUri, string responseHeaders, byte[] source, string fullTextIndexType, bool classifyAbsoluteUris);

        /// <summary>
        /// 	Inserts a row into table 'Files_Discoveries'.
        /// </summary>
        /// <param name = "webPageAbsoluteUri">The AbsoluteUri of the WebPage where the File was discovered.</param>
        /// <param name = "fileAbsoluteUri">The AbsoluteUri of the File.</param>
        void InsertFileDiscovery(string webPageAbsoluteUri, string fileAbsoluteUri);

        /// <summary>
        /// 	Inserts a row into table 'Files_MetaData'.
        /// </summary>
        /// <param name = "fileAbsoluteUri">The AbsoluteUri of the File.</param>
        /// <param name = "fileID">The FileID of the File.</param>
        void InsertFileMetaData(string fileAbsoluteUri, long fileID);

        /// <summary>
        /// 	Inserts a row into table 'HyperLinks'.
        /// </summary>
        /// <param name = "webPageAbsoluteUri">The AbsoluteUri of the WebPage where the HyperLink was discovered.</param>
        /// <param name = "hyperLinkAbsoluteUri">The AbsoluteUri of the HyperLink.</param>
        /// <returns></returns>
        long? InsertHyperLink(string webPageAbsoluteUri, string hyperLinkAbsoluteUri, bool classifyAbsoluteUris);

        /// <summary>
        /// 	Inserts a row into table 'HyperLinks_Discoveries'.
        /// </summary>
        /// <param name = "webPageAbsoluteUri">The AbsoluteUri of the WebPage where the HyperLink was discovered.</param>
        /// <param name = "hyperLinkAbsoluteUri">The AbsoluteUri of the HyperLink.</param>
        void InsertHyperLinkDiscovery(string webPageAbsoluteUri, string hyperLinkAbsoluteUri);

        /// <summary>
        /// 	Inserts a row into table 'Images'.
        /// </summary>
        /// <param name = "webPageAbsoluteUri">The AbsoluteUri of the WebPage where the Image was discovered.</param>
        /// <param name = "imageAbsoluteUri">The AbsoluteUri of the Image.</param>
        /// <param name = "responseHeaders">The ResponseHeaders returned from the HttpRequest.</param>
        /// <param name = "source">The binary data of the Image.</param>
        /// <param name = "fullTextIndexType">Full type of the text index.</param>
        /// <returns></returns>
        long? InsertImage(string webPageAbsoluteUri, string imageAbsoluteUri, string responseHeaders, byte[] source, string fullTextIndexType);

        /// <summary>
        /// 	Inserts a row into table 'Images_Discoveries'.
        /// </summary>
        /// <param name = "webPageAbsoluteUri">The AbsoluteUri of the WebPage where the Image was discovered.</param>
        /// <param name = "imageAbsoluteUri">The AbsoluteUri of the Image.</param>
        void InsertImageDiscovery(string webPageAbsoluteUri, string imageAbsoluteUri);

        /// <summary>
        /// 	Inserts a row into table 'Images_MetaData'.
        /// </summary>
        /// <param name = "imageAbsoluteUri">The AbsoluteUri of the Image.</param>
        /// <param name = "imageID">The ImageID of the Image.</param>
        /// <param name = "exifData">The ExifData of the Image.</param>
        /// <param name = "flags">The Flags of the Image.</param>
        /// <param name = "height">The Height of the Image.</param>
        /// <param name = "horizontalResolution">The HorizontalResolution of the Image.</param>
        /// <param name = "verticalResolution">The VerticalResoution of the Image.</param>
        /// <param name = "width">The Width of the Image.</param>
        void InsertImageMetaData(string imageAbsoluteUri, long imageID, string exifData, int flags, int height, double horizontalResolution, double verticalResolution, int width);

        /// <summary>
        /// 	Inserts a row into table 'WebPages'.
        /// </summary>
        /// <param name = "webPageAbsoluteUri">The web page absolute URI.</param>
        /// <param name = "responseHeaders">The response headers.</param>
        /// <param name = "source">The source.</param>
        /// <param name = "codePage">The code page.</param>
        /// <param name = "fullTextIndexType">Full type of the text index.</param>
        /// <param name = "crawlDepth">The crawl depth.</param>
        /// <returns></returns>
        long? InsertWebPage(string webPageAbsoluteUri, string responseHeaders, byte[] source, int codePage, string fullTextIndexType, int crawlDepth, bool classifyAbsoluteUris);

        /// <summary>
        /// 	Inserts a row into table 'WebPages_MetaData'.
        /// </summary>
        /// <param name = "webPageID">The web page ID.</param>
        /// <param name = "webPageAbsoluteUri">The web page absolute URI.</param>
        /// <param name = "text">The text.</param>
        /// <param name = "xml">The XML.</param>
        void InsertWebPageMetaData(long webPageID, string webPageAbsoluteUri, byte[] text, string xml);
    }
}
