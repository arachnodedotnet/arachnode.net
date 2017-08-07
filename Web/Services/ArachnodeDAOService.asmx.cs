using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web.Services;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.DataSource;
using Arachnode.Web.Value.AbstractClasses;

namespace Arachnode.Web.Services
{
    /// <summary>
    /// Summary description for ArachnodeDAOService
    /// </summary>
    [WebService(Namespace = "http://arachnode.net/ArachnodeDAOService")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
    // [System.Web.Script.Services.ScriptService]
    public class ArachnodeDAOService : AWebService, IArachnodeDAO
    {
        #region IArachnodeDAO Members

        public long? LastDisallowedAbsoluteUriID { get; private set; }
        public long? LastEmailAddressID { get; private set; }
        public long? LastExceptionID { get; private set; }
        public string LastExceptionMessage { get; private set; }
        public long? LastFileID { get; private set; }
        public long? LastHyperLinkID { get; private set; }
        public long? LastImageID { get; private set; }
        public long? LastWebPageID { get; private set; }

        public new ApplicationSettings ApplicationSettings
        {
            get { return base.ApplicationSettings; }
            set { base.ApplicationSettings = value; }
        }

        public new WebSettings WebSettings
        {
            get { return base.WebSettings; }
            set { base.WebSettings = value; }
        }

        [WebMethod(Description = "Opens all database connections.  Doing so before crawling allows CRUD operations to operate faster than opening and closing the connection each time an operation is requested.  Extremely advanced...  ask if you have performance problems...", EnableSession = true)]
        public void OpenCommandConnections()
        {
            ArachnodeDAO.OpenCommandConnections();
        }

        [WebMethod(Description = "Closes all database connections.  Extremely advanced...  ask if you have performance problems...", EnableSession = true)]
        public void CloseCommandConnections()
        {
            ArachnodeDAO.CloseCommandConnections();
        }

        //[WebMethod]
        public void CloseCommandConnections(IEnumerable<IDbCommand> dbCommands)
        {
            throw new NotImplementedException("This method is meant for internal use only.");
        }

        [WebMethod(Description = "Checks the Database Version.  Used by the Crawler to ensure the client is communicating with the proper Database Version.", EnableSession = true)]
        public void CheckVersion()
        {
            ArachnodeDAO.CheckVersion();
        }

        [WebMethod(Description = "Deletes a row from table 'CrawlRequests'.  Calling this method with absoluteUri1 set to 'null' and absoluteUri2 set to 'null' deletes all CrawlRequests from the 'CrawlRequests' table.", EnableSession = true)]
        public void DeleteCrawlRequest(string absoluteUri1, string absoluteUri2)
        {
            ArachnodeDAO.DeleteCrawlRequest(absoluteUri1, absoluteUri2);
        }

        [WebMethod(Description = "Deletes all rows from table 'Discoveries'.", EnableSession = true)]
        public void DeleteDiscoveries()
        {
            ArachnodeDAO.DeleteDiscoveries();
        }

        [WebMethod(Description = "Deletes a row from table 'WebPages'.", EnableSession = true)]
        public void DeleteWebPage(string webPageAbsoluteUriOrID)
        {
            ArachnodeDAO.DeleteWebPage(webPageAbsoluteUriOrID);
        }

        [WebMethod(Description = "Executes dynamic SQL.", EnableSession = true)]
        public void ExecuteSql(string query)
        {
            ArachnodeDAO.ExecuteSql(query);
        }

        [WebMethod(Description = "Executes dynamic SQL.", EnableSession = true)]
        public DataTable ExecuteSql2(string query)
        {
            return ArachnodeDAO.ExecuteSql2(query);
        }

        [WebMethod(Description = "DataTypes represent mappings between the ResponseHeader 'Content-type:' and the type to be used by SQL for Full-text indexing.  If you find that content you wish to crawl isn't being crawled, check the table 'DisallowedAbsoluteUris'.  If you see 'Disallowed by unassigned DataType', add the DataType to table 'AllowedDataTypes'.", EnableSession = true)]
        public ArachnodeDataSet.AllowedDataTypesDataTable GetAllowedDataTypes()
        {
            return ArachnodeDAO.GetAllowedDataTypes();
        }

        public ArachnodeDataSet.AllowedExtensionsDataTable GetAllowedExtensions()
        {
            return ArachnodeDAO.GetAllowedExtensions();
        }

        public ArachnodeDataSet.AllowedSchemesDataTable GetAllowedSchemes()
        {
            return ArachnodeDAO.GetAllowedSchemes();
        }

        [WebMethod(Description = "Gets the configuration.", EnableSession = true)]
        public ArachnodeDataSet.ConfigurationDataTable GetConfiguration()
        {
            return ArachnodeDAO.GetConfiguration();
        }

        [WebMethod(Description = "Gets the crawl actions.  Used by the ActionManager.", EnableSession = true)]
        public ArachnodeDataSet.CrawlActionsDataTable GetCrawlActions()
        {
            return ArachnodeDAO.GetCrawlActions();
        }

        [WebMethod(Description = "Gets the crawl rules.  Used by the RuleManager.", EnableSession = true)]
        public ArachnodeDataSet.CrawlRulesDataTable GetCrawlRules()
        {
            return ArachnodeDAO.GetCrawlRules();
        }

        [WebMethod(Description = "Gets all ContentTypes.  A ContentType would be text/html, image/jpeg, etc.", EnableSession = true)]
        public ArachnodeDataSet.ContentTypesDataTable GetContentTypes()
        {
            return ArachnodeDAO.GetContentTypes();
        }

        [WebMethod(Description = "Returns a DataTable containing the database representation of a CrawlRequest.  This procedure creates CrawlRequests for the Engine to process from rows in table 'CrawlRequests'.", EnableSession = true)]
        public ArachnodeDataSet.CrawlRequestsDataTable GetCrawlRequests(int maximumNumberOfCrawlRequestsToCreatePerBatch, bool createCrawlRequestsFromDatabaseCrawlRequests, bool createCrawlRequestsFromDatabaseFiles, bool assignCrawlRequestPrioritiesForFiles, bool createCrawlRequestsFromDatabaseHyperLinks, bool assignCrawlRequestPrioritiesForHyperLinks, bool createCrawlRequestsFromDatabaseImages, bool assignCrawlRequestPrioritiesForImages, bool createCrawlRequestsFromDatabaseWebPages, bool assignCrawlRequestPrioritiesForWebPages)
        {
            return ArachnodeDAO.GetCrawlRequests(maximumNumberOfCrawlRequestsToCreatePerBatch, createCrawlRequestsFromDatabaseCrawlRequests, createCrawlRequestsFromDatabaseFiles, assignCrawlRequestPrioritiesForFiles, createCrawlRequestsFromDatabaseHyperLinks, assignCrawlRequestPrioritiesForHyperLinks, createCrawlRequestsFromDatabaseImages, assignCrawlRequestPrioritiesForImages, createCrawlRequestsFromDatabaseWebPages, assignCrawlRequestPrioritiesForWebPages);
        }

        //[WebMethod]
        public ArachnodeDataSet.DiscoveriesRow GetDiscovery(string absoluteUri)
        {
            return ArachnodeDAO.GetDiscovery(absoluteUri);
        }

        [WebMethod(Description = "Gets the Discoveries.", EnableSession = true)]
        public ArachnodeDataSet.DiscoveriesDataTable GetDiscoveries(int numberOfDiscoveriesToReturn)
        {
            return ArachnodeDAO.GetDiscoveries(numberOfDiscoveriesToReturn);
        }

        //[WebMethod]
        public ArachnodeDataSet.DisallowedAbsoluteUrisRow GetDisallowedAbsoluteUri(string disallowedAbsoluteUriAbsoluteUriOrID)
        {
            return ArachnodeDAO.GetDisallowedAbsoluteUri(disallowedAbsoluteUriAbsoluteUriOrID);
        }

        [WebMethod(Description = "Gets the DisallowedAbsoluteUris.", EnableSession = true)]
        public ArachnodeDataSet.DisallowedAbsoluteUrisDataTable GetDisallowedAbsoluteUris(int numberOfDisallowedAbsoluteUrisToReturn)
        {
            return ArachnodeDAO.GetDisallowedAbsoluteUris(numberOfDisallowedAbsoluteUrisToReturn);
        }

        [WebMethod(Description = "Selects a row from table 'DisallowedAbsoluteUris_Discoveries' by DisallowedAbsoluteUriID.", EnableSession = true)]
        public ArachnodeDataSet.DisallowedAbsoluteUrisDiscoveriesDataTable GetDisallowedAbsoluteUriDiscoveries(long disallowedAbsoluteUriID)
        {
            return ArachnodeDAO.GetDisallowedAbsoluteUriDiscoveries(disallowedAbsoluteUriID);
        }

        public ArachnodeDataSet.DisallowedDomainsDataTable GetDisallowedDomains()
        {
            return ArachnodeDAO.GetDisallowedDomains();
        }

        public ArachnodeDataSet.DisallowedExtensionsDataTable GetDisallowedExtensions()
        {
            return ArachnodeDAO.GetDisallowedExtensions();
        }

        public ArachnodeDataSet.DisallowedFileExtensionsDataTable GetDisallowedFileExtensions()
        {
            return ArachnodeDAO.GetDisallowedFileExtensions();
        }

        public ArachnodeDataSet.DisallowedHostsDataTable GetDisallowedHosts()
        {
            return ArachnodeDAO.GetDisallowedHosts();
        }

        public ArachnodeDataSet.DisallowedSchemesDataTable GetDisallowedSchemes()
        {
            return ArachnodeDAO.GetDisallowedSchemes();
        }

        public ArachnodeDataSet.DisallowedWordsDataTable GetDisallowedWords()
        {
            return ArachnodeDAO.GetDisallowedWords();
        }

        [WebMethod(Description = "Gets all Domains.", EnableSession = true)]
        public ArachnodeDataSet.DomainsDataTable GetDomains()
        {
            return ArachnodeDAO.GetDomains();
        }

        //[WebMethod]
        public ArachnodeDataSet.EmailAddressesRow GetEmailAddress(string emailAddressAbsoluteUriOrID)
        {
            return ArachnodeDAO.GetEmailAddress(emailAddressAbsoluteUriOrID);
        }

        [WebMethod(Description = "Selects a row from table 'EmailAddresses_Discoveries' by EmailAddressID.", EnableSession = true)]
        public ArachnodeDataSet.EmailAddressesDiscoveriesDataTable GetEmailAddressDiscoveries(long emailAddressID)
        {
            return ArachnodeDAO.GetEmailAddressDiscoveries(emailAddressID);
        }

        [WebMethod(Description = "Gets the engine actions.  Used by the ActionManager.", EnableSession = true)]
        public ArachnodeDataSet.EngineActionsDataTable GetEngineActions()
        {
            return ArachnodeDAO.GetEngineActions();
        }

        [WebMethod(Description = "Gets all Extensions.", EnableSession = true)]
        public ArachnodeDataSet.ExtensionsDataTable GetExtensions()
        {
            return ArachnodeDAO.GetExtensions();
        }

        //[WebMethod]
        public ArachnodeDataSet.FilesRow GetFile(string fileAbsoluteUriOrID)
        {
            return ArachnodeDAO.GetFile(fileAbsoluteUriOrID);
        }

        [WebMethod(Description = "Selects a row from table 'Files_Discoveries' by FileID.", EnableSession = true)]
        public ArachnodeDataSet.FilesDiscoveriesDataTable GetFileDiscoveries(long fileID)
        {
            return ArachnodeDAO.GetFileDiscoveries(fileID);
        }

        [WebMethod(Description = "Gets all Hosts.", EnableSession = true)]
        public ArachnodeDataSet.HostsDataTable GetHosts()
        {
            return ArachnodeDAO.GetHosts();
        }

        [WebMethod(Description = "Gets the Database Version.  Used by the Crawler to ensure the client is communicating with the proper Database Version.", EnableSession = true)]
        public ArachnodeDataSet.VersionDataTable GetVersion()
        {
            return ArachnodeDAO.GetVersion();
        }

        //[WebMethod]
        public ArachnodeDataSet.FilesMetaDataRow GetFileMetaData(long fileID)
        {
            return ArachnodeDAO.GetFileMetaData(fileID);
        }

        //[WebMethod]
        public ArachnodeDataSet.HyperLinksRow GetHyperLink(string hyperLinkAbsoluteUriOrID)
        {
            return ArachnodeDAO.GetHyperLink(hyperLinkAbsoluteUriOrID);
        }

        [WebMethod(Description = "Selects a row from table 'HyperLinks_Discoveries' by HyperLinkID.", EnableSession = true)]
        public ArachnodeDataSet.HyperLinksDiscoveriesDataTable GetHyperLinkDiscoveries(long hyperLinkID)
        {
            return ArachnodeDAO.GetHyperLinkDiscoveries(hyperLinkID);
        }

        //[WebMethod]
        public ArachnodeDataSet.ImagesRow GetImage(string imageAbsoluteUriOrID)
        {
            return ArachnodeDAO.GetImage(imageAbsoluteUriOrID);
        }

        [WebMethod(Description = "Selects a row from table 'Images_Discoveries' by ImageID.", EnableSession = true)]
        public ArachnodeDataSet.ImagesDiscoveriesDataTable GetImageDiscoveries(long imageID)
        {
            return ArachnodeDAO.GetImageDiscoveries(imageID);
        }

        //[WebMethod]
        public ArachnodeDataSet.ImagesMetaDataRow GetImageMetaData(long imageID)
        {
            return ArachnodeDAO.GetImageMetaData(imageID);
        }

        [WebMethod(Description = "Gets all Priorities.  Priorities are used by CrawlRequests, the ReportingManager and by the lucene.net indexing functionality to determine crawl priority, and to enhance the boosts of lucene.net documents.  A document for \"C#\" from domain \"XYZ\" may score a higher relevancy score than a document for \"C#\" from microsoft.com, but actual popularity/priority dictates that the result from microsoft.com should receive a higher boost and thereby a higher overall relevancy score than the document from domain \"XYZ\".", EnableSession = true)]
        public ArachnodeDataSet.PrioritiesDataTable GetPriorities(int maximumNumberOfPriorities)
        {
            return ArachnodeDAO.GetPriorities(maximumNumberOfPriorities);
        }

        [WebMethod(Description = "Gets all Schemes.", EnableSession = true)]
        public ArachnodeDataSet.SchemesDataTable GetSchemes()
        {
            return ArachnodeDAO.GetSchemes();
        }

        //[WebMethod]
        public ArachnodeDataSet.WebPagesRow GetWebPage(string webPageAbsoluteUriOrID)
        {
            return ArachnodeDAO.GetWebPage(webPageAbsoluteUriOrID);
        }

        [WebMethod(Description = "Inserts a row into table 'BusinessInformation'.", EnableSession = true)]
        public long? InsertBusinessInformation(string parentWebPageAbsoluteUri, string webPageAbsoluteUri, long webPageID, string name, string address1, string address2, string city, string state, string zip, string phoneNumber, string category, string latitude, string longitude)
        {
            return ArachnodeDAO.InsertBusinessInformation(parentWebPageAbsoluteUri, webPageAbsoluteUri, webPageID, name, address1, address2, city, state, zip, phoneNumber, category, latitude, longitude);
        }

        [WebMethod(Description = "Inserts the crawl request.  See: http://arachnode.net/forums/p/564/10762.aspx#10762 if you experience difficulty with this function.  Use this function if you want to insert CrawlRequests into the database.  The overload with the split 'currentDepth' and 'maximumDepth' parameters is intended for use by the Crawler itself.", EnableSession = true)]
        public long? InsertCrawlRequest(DateTime created, string absoluteUri0, string absoluteUri1, string absoluteUri2, int depth, short restrictCrawlTo, short restrictDiscoveriesTo, double priority, byte renderType, byte renderTypeForChildren)
        {
            return ArachnodeDAO.InsertCrawlRequest(created, new Uri(absoluteUri0).AbsoluteUri, new Uri(absoluteUri1).AbsoluteUri, new Uri(absoluteUri2).AbsoluteUri, 1, depth, restrictCrawlTo, restrictDiscoveriesTo, priority, renderType, renderTypeForChildren);
        }

        //[WebMethod]
        public long? InsertCrawlRequest(DateTime created, string absoluteUri0, string absoluteUri1, string absoluteUri2, int currentDepth, int maximumDepth, short restrictCrawlTo, short restrictDiscoveriesTo, double priority, byte renderType, byte renderTypeForChildren)
        {
            throw new NotImplementedException("This method is meant for internal use only.");
        }

        [WebMethod(Description = "Inserts an AbsoluteUri into the Discoveries table.", EnableSession = true)]
        public void InsertDiscovery(long? discoveryID, string absoluteUri, int discoveryStateID, int discoveryTypeID, bool expectFileOrImage, int numberOfTimesDiscovered)
        {
            ArachnodeDAO.InsertDiscovery(discoveryID, absoluteUri, discoveryStateID, discoveryTypeID, expectFileOrImage, numberOfTimesDiscovered);
        }

        [WebMethod(Description = "Inserts a row into table 'DisallowedAbsoluteUris'.", EnableSession = true)]
        public long? InsertDisallowedAbsoluteUri(int contentTypeID, int discoveryTypeID, string webPageAbsoluteUri, string disallowedAbsoluteUriAbsoluteUri, string reason, bool classifyAbsoluteUris)
        {
            return ArachnodeDAO.InsertDisallowedAbsoluteUri(contentTypeID, discoveryTypeID, webPageAbsoluteUri, disallowedAbsoluteUriAbsoluteUri, reason, classifyAbsoluteUris);
        }

        [WebMethod(Description = "Inserts a row into table 'DisallowedAbsoluteUris_Discoveries'.", EnableSession = true)]
        public void InsertDisallowedAbsoluteUriDiscovery(string webPageAbsoluteUri, string disallowedAbsoluteUriAbsoluteUri)
        {
            ArachnodeDAO.InsertDisallowedAbsoluteUriDiscovery(webPageAbsoluteUri, disallowedAbsoluteUriAbsoluteUri);
        }

        [WebMethod(Description = "Inserts a row into table 'Documents'.", EnableSession = true)]
        public virtual long? InsertDocument(string parentWebPageAbsoluteUri, string webPageAbsoluteUri, byte documentTypeID, long webPageID, double weight, string field01, string field02, string field03, string field04, string field05, string field06, string field07, string field08, string field09, string field10, string field11, string field12, string field13, string field14, string field15, string field16, string field17, string field18, string field19, string field20, string fullTextIndexType)
        {
            return ArachnodeDAO.InsertDocument(parentWebPageAbsoluteUri, webPageAbsoluteUri, documentTypeID, webPageID, weight, field01, field02, field03, field04, field05, field06, field07, field08, field09, field10, field11, field12, field13, field14, field15, field16, field17, field18, field19, field20, fullTextIndexType);
        }

        [WebMethod(Description = "Inserts a row into table 'EmailAddresses'.", EnableSession = true)]
        public long? InsertEmailAddress(string webPageAbsoluteUri, string emailAddressAbsoluteUri, bool classifyAbsoluteUris)
        {
            return ArachnodeDAO.InsertEmailAddress(webPageAbsoluteUri, emailAddressAbsoluteUri, classifyAbsoluteUris);
        }

        [WebMethod(Description = "Inserts a row into table 'EmailAddresses_Discoveries'.", EnableSession = true)]
        public void InsertEmailAddressDiscovery(string webPageAbsoluteUri, string emailAddressAbsoluteUri)
        {
            ArachnodeDAO.InsertEmailAddressDiscovery(webPageAbsoluteUri, emailAddressAbsoluteUri);
        }

        //[WebMethod]
        public long? InsertException(string absoluteUri1, string absoluteUri2, Exception exception, bool insertExceptionInWindowsApplicationLog)
        {
            throw new NotImplementedException("This method is meant for internal use only.");
        }

        [WebMethod(Description = "Inserts a row into table 'Exceptions'.", EnableSession = true)]
        public long? InsertException(string absoluteUri1, string absoluteUri2, string helpLink, string message, string source, string stackTrace, bool insertExceptionInWindowsApplicationLog)
        {
            return ArachnodeDAO.InsertException(absoluteUri1, absoluteUri2, helpLink, message, source, stackTrace, insertExceptionInWindowsApplicationLog);
        }

        [WebMethod(Description = "Inserts a row into table 'Files'.", EnableSession = true)]
        public long? InsertFile(string webPageAbsoluteUri, string fileAbsoluteUri, string responseHeaders, byte[] source, string fullTextIndexType, bool classifyAbsoluteUris)
        {
            return ArachnodeDAO.InsertFile(webPageAbsoluteUri, fileAbsoluteUri, responseHeaders, source, fullTextIndexType, classifyAbsoluteUris);
        }

        [WebMethod(Description = "Inserts a row into table 'Files_Discoveries'.", EnableSession = true)]
        public void InsertFileDiscovery(string webPageAbsoluteUri, string fileAbsoluteUri)
        {
            ArachnodeDAO.InsertFileDiscovery(webPageAbsoluteUri, fileAbsoluteUri);
        }

        [WebMethod(Description = "Inserts a row into table 'Files_MetaData'.", EnableSession = true)]
        public void InsertFileMetaData(string fileAbsoluteUri, long fileID)
        {
            ArachnodeDAO.InsertFileMetaData(fileAbsoluteUri, fileID);
        }

        [WebMethod(Description = "Inserts a row into table 'HyperLinks'.", EnableSession = true)]
        public long? InsertHyperLink(string webPageAbsoluteUri, string hyperLinkAbsoluteUri, bool classifyAbsoluteUris)
        {
            return ArachnodeDAO.InsertHyperLink(webPageAbsoluteUri, hyperLinkAbsoluteUri, classifyAbsoluteUris);
        }

        [WebMethod(Description = "Inserts a row into table 'HyperLinks_Discoveries'.", EnableSession = true)]
        public void InsertHyperLinkDiscovery(string webPageAbsoluteUri, string hyperLinkAbsoluteUri)
        {
            ArachnodeDAO.InsertHyperLinkDiscovery(webPageAbsoluteUri, hyperLinkAbsoluteUri);
        }

        [WebMethod(Description = "Inserts a row into table 'Images'.", EnableSession = true)]
        public long? InsertImage(string webPageAbsoluteUri, string imageAbsoluteUri, string responseHeaders, byte[] source, string fullTextIndexType)
        {
            return ArachnodeDAO.InsertImage(webPageAbsoluteUri, imageAbsoluteUri, responseHeaders, source, fullTextIndexType);
        }

        [WebMethod(Description = "Inserts a row into table 'Images_Discoveries'.", EnableSession = true)]
        public void InsertImageDiscovery(string webPageAbsoluteUri, string imageAbsoluteUri)
        {
            ArachnodeDAO.InsertImageDiscovery(webPageAbsoluteUri, imageAbsoluteUri);
        }

        [WebMethod(Description = "Inserts a row into table 'Images_MetaData'.", EnableSession = true)]
        public void InsertImageMetaData(string imageAbsoluteUri, long imageID, string exifData, int flags, int height, double horizontalResolution, double verticalResolution, int width)
        {
            ArachnodeDAO.InsertImageMetaData(imageAbsoluteUri, imageID, exifData, flags, height, horizontalResolution, verticalResolution, width);
        }

        [WebMethod(Description = "Inserts a row into table 'WebPages'.", EnableSession = true)]
        public long? InsertWebPage(string webPageAbsoluteUri, string responseHeaders, byte[] source, int codePage, string fullTextIndexType, int crawlDepth, bool classifyAbsoluteUris)
        {
            return ArachnodeDAO.InsertWebPage(webPageAbsoluteUri, responseHeaders, source, codePage, fullTextIndexType, crawlDepth, classifyAbsoluteUris);
        }

        [WebMethod(Description = "Inserts a row into table 'WebPages_MetaData'.", EnableSession = true)]
        public void InsertWebPageMetaData(long webPageID, string webPageAbsoluteUri, byte[] text, string xml)
        {
            ArachnodeDAO.InsertWebPageMetaData(webPageID, webPageAbsoluteUri, text, xml);
        }

        #endregion

        [WebMethod(Description = "The last ExceptionID submitted to table 'DisallowedAbsoluteUris'.", EnableSession = true)]
        public long? GetLastDisallowedAbsoluteUriID()
        {
            return LastDisallowedAbsoluteUriID;
        }

        [WebMethod(Description = "The last EmailAddressID submitted to table 'EmailAddresses'.", EnableSession = true)]
        public long? GetLastEmailAddressID()
        {
            return LastEmailAddressID;
        }

        [WebMethod(Description = "The last ExceptionID submitted to table 'Exceptions'.", EnableSession = true)]
        public long? GetLastExceptionID()
        {
            return LastExceptionID;
        }

        [WebMethod(Description = "The last Message submitted to table 'Exceptions'.", EnableSession = true)]
        public string GetLastExceptionMessage()
        {
            return LastExceptionMessage;
        }

        [WebMethod(Description = "The last FileID submitted to table 'Files'.", EnableSession = true)]
        public long? GetLastFileID()
        {
            return LastFileID;
        }

        [WebMethod(Description = "The last HyperLinksID submitted to table 'HyperLinks'.", EnableSession = true)]
        public long? GetLastHyperLinkID()
        {
            return LastHyperLinkID;
        }

        [WebMethod(Description = "The last ImageID submitted to table 'Images'.", EnableSession = true)]
        public long? GetLastImageID()
        {
            return LastImageID;
        }

        [WebMethod(Description = "The last WebPageID submitted to table 'WebPages'.", EnableSession = true)]
        public long? GetLastWebPageID()
        {
            return LastWebPageID;
        }

        [WebMethod(Description = "Creates a new instance of 'ArachnodeDAO', replacing the default ArachnodeDAO instance as intantiated by the service instance.", EnableSession = true)]
        public void Constructor(string connectionString, ApplicationSettings applicationSettings, WebSettings webSettings, bool initializeApplicationConfiguration, bool initializeWebConfiguration)
        {
            ArachnodeDAO = new ArachnodeDAO(connectionString, base.ApplicationSettings, base.WebSettings, initializeApplicationConfiguration, initializeWebConfiguration);
        }

        [WebMethod(Description = "Gets a Discovery from the Discoveries table.  If the Discovery isn't present, null is returned.  This method is used by the Cache to determine if a Discovery has been encountered in a crawl run.  If a Discovery hasn't been 'Discovered', then it is eligible to have a CrawlRequest created from it and to be crawled.", EnableSession = true)]
        public ArachnodeDataSet.DiscoveriesDataTable GetDiscoverySerialized(string absoluteUri)
        {
            return (ArachnodeDataSet.DiscoveriesDataTable)GetDiscovery(absoluteUri).Table;
        }

        [WebMethod(Description = "Selects a row from table 'DisallowedAbsoluteUris' by AbsoluteUri or ID.", EnableSession = true)]
        public ArachnodeDataSet.DisallowedAbsoluteUrisDataTable GetDisallowedAbsoluteUriSerialized(string disallowedAbsoluteUriAbsoluteUriOrID)
        {
            ArachnodeDataSet.DisallowedAbsoluteUrisRow disallowedAbsoluteUrisRow = GetDisallowedAbsoluteUri(disallowedAbsoluteUriAbsoluteUriOrID);

            if (disallowedAbsoluteUrisRow != null)
            {
                return (ArachnodeDataSet.DisallowedAbsoluteUrisDataTable)disallowedAbsoluteUrisRow.Table;
            }

            return new ArachnodeDataSet.DisallowedAbsoluteUrisDataTable();
        }

        [WebMethod(Description = "Selects a row from table 'EmailAddresses' by AbsoluteUri or ID.", EnableSession = true)]
        public ArachnodeDataSet.EmailAddressesDataTable GetEmailAddressSerialized(string emailAddressAbsoluteUriOrID)
        {
            return (ArachnodeDataSet.EmailAddressesDataTable)GetEmailAddress(emailAddressAbsoluteUriOrID).Table;
        }

        [WebMethod(Description = "Selects a row from table 'Files' by AbsoluteUri or ID.", EnableSession = true)]
        public ArachnodeDataSet.FilesDataTable GetFileSerialized(string fileAbsoluteUriOrID)
        {
            return (ArachnodeDataSet.FilesDataTable)GetFile(fileAbsoluteUriOrID).Table;
        }

        [WebMethod(Description = "Selects a row from table 'Files_MetaData' by FileID.", EnableSession = true)]
        public ArachnodeDataSet.FilesMetaDataDataTable GetFileMetaDataSerialized(long fileID)
        {
            return (ArachnodeDataSet.FilesMetaDataDataTable)GetFileMetaData(fileID).Table;
        }

        [WebMethod(Description = "Selects a row from table 'HyperLinks' by AbsoluteUri or ID.", EnableSession = true)]
        public ArachnodeDataSet.HyperLinksDataTable GetHyperLinkSerialized(string hyperLinkAbsoluteUriOrID)
        {
            return (ArachnodeDataSet.HyperLinksDataTable)GetHyperLink(hyperLinkAbsoluteUriOrID).Table;
        }

        [WebMethod(Description = "Selects a row from table 'Images' by AbsoluteUri or ID.", EnableSession = true)]
        public ArachnodeDataSet.ImagesDataTable GetImageSerialized(string imageAbsoluteUriOrID)
        {
            return (ArachnodeDataSet.ImagesDataTable)GetImage(imageAbsoluteUriOrID).Table;
        }

        [WebMethod(Description = "Selects a row from table 'Images_MetaData' by ImageID.", EnableSession = true)]
        public ArachnodeDataSet.ImagesMetaDataDataTable GetImageMetaDataSerialized(long imageID)
        {
            return (ArachnodeDataSet.ImagesMetaDataDataTable)GetImageMetaData(imageID).Table;
        }

        [WebMethod(Description = "Selects a row from table 'WebPages' by AbsoluteUri or ID.", EnableSession = true)]
        public ArachnodeDataSet.WebPagesDataTable GetWebPageSerialized(string absoluteUri)
        {
            return (ArachnodeDataSet.WebPagesDataTable)GetWebPage(absoluteUri).Table;
        }
    }
}