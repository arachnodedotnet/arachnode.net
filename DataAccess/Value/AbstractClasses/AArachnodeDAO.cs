using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Arachnode.Configuration;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.DataSource;
using Arachnode.DataSource.ArachnodeDataSetTableAdapters;

namespace Arachnode.DataAccess.Value.AbstractClasses
{
    public abstract class AArachnodeDAO : IArachnodeDAO
    {
        protected ApplicationSettings _applicationSettings;
        protected WebSettings _webSettings;

        protected readonly AllowedDataTypesTableAdapter _allowedDataTypesTableAdapter = new AllowedDataTypesTableAdapter();
        protected readonly AllowedExtensionsTableAdapter _allowedExtensionsTableAdapter = new AllowedExtensionsTableAdapter();
        protected readonly AllowedSchemesTableAdapter _allowedSchemesTableAdapter = new AllowedSchemesTableAdapter();
        protected readonly BusinessInformationTableAdapter _businessInformationTableAdapter = new BusinessInformationTableAdapter();
        protected readonly ConfigurationTableAdapter _configurationTableAdapter = new ConfigurationTableAdapter();
        protected readonly ContentTypesTableAdapter _contentTypesTableAdapter = new ContentTypesTableAdapter();
        protected readonly CrawlActionsTableAdapter _crawlActionsTableAdapter = new CrawlActionsTableAdapter();
        protected readonly CrawlRequestsTableAdapter _crawlRequestsTableAdapter = new CrawlRequestsTableAdapter();
        protected readonly CrawlRulesTableAdapter _crawlRulesTableAdapter = new CrawlRulesTableAdapter();
        protected readonly ArachnodeDataSet.DisallowedAbsoluteUrisDataTable _disallowedAbsoluteUrisDataTable = new ArachnodeDataSet.DisallowedAbsoluteUrisDataTable();
        protected readonly DisallowedAbsoluteUrisTableAdapter _disallowedAbsoluteUrisTableAdapter = new DisallowedAbsoluteUrisTableAdapter();
        protected readonly ArachnodeDataSet.DisallowedAbsoluteUrisDiscoveriesDataTable _disallowedAbsoluteUrisDiscoveriesDataTable = new ArachnodeDataSet.DisallowedAbsoluteUrisDiscoveriesDataTable();
        protected readonly DisallowedAbsoluteUrisDiscoveriesTableAdapter _disallowedAbsoluteUrisDiscoveriesTableAdapter = new DisallowedAbsoluteUrisDiscoveriesTableAdapter();
        protected readonly ArachnodeDataSet.DiscoveriesDataTable _discoveriesDataTable = new ArachnodeDataSet.DiscoveriesDataTable();
        protected readonly DisallowedDomainsTableAdapter _disallowedDomainsTableAdapter = new DisallowedDomainsTableAdapter();
        protected readonly DisallowedExtensionsTableAdapter _disallowedExtensionsTableAdapter = new DisallowedExtensionsTableAdapter();
        protected readonly DisallowedFileExtensionsTableAdapter _disallowedFileExtensionsTableAdapter = new DisallowedFileExtensionsTableAdapter();
        protected readonly DisallowedHostsTableAdapter _disallowedHostsTableAdapter = new DisallowedHostsTableAdapter();
        protected readonly DisallowedSchemesTableAdapter _disallowedSchemesTableAdapter = new DisallowedSchemesTableAdapter();
        protected readonly DisallowedWordsTableAdapter _disallowedWordsTableAdapter = new DisallowedWordsTableAdapter();
        protected readonly DiscoveriesTableAdapter _discoveriesTableAdapter = new DiscoveriesTableAdapter();
        protected readonly DomainsTableAdapter _domainsTableAdapter = new DomainsTableAdapter();
        protected readonly ArachnodeDataSet.EmailAddressesDataTable _emailAddressesDataTable = new ArachnodeDataSet.EmailAddressesDataTable();
        protected readonly EmailAddressesTableAdapter _emailAddressesTableAdapter = new EmailAddressesTableAdapter();
        protected readonly ArachnodeDataSet.EmailAddressesDiscoveriesDataTable _emailAddressesDiscoveriesDataTable = new ArachnodeDataSet.EmailAddressesDiscoveriesDataTable();
        protected readonly EmailAddressesDiscoveriesTableAdapter _emailAddressesDiscoveriesTableAdapter = new EmailAddressesDiscoveriesTableAdapter();
        protected readonly EngineActionsTableAdapter _engineActionsTableAdapter = new EngineActionsTableAdapter();
        protected readonly ExtensionsTableAdapter _extensionsTableAdapter = new ExtensionsTableAdapter();
        protected readonly ArachnodeDataSet.FilesDataTable _filesDataTable = new ArachnodeDataSet.FilesDataTable();
        protected readonly FilesTableAdapter _filesTableAdapter = new FilesTableAdapter();
        protected readonly ArachnodeDataSet.FilesDiscoveriesDataTable _filesDiscoveriesDataTable = new ArachnodeDataSet.FilesDiscoveriesDataTable();
        protected readonly FilesDiscoveriesTableAdapter _filesDiscoveriesTableAdapter = new FilesDiscoveriesTableAdapter();
        protected readonly ArachnodeDataSet.FilesMetaDataDataTable _filesMetaDataDataTable = new ArachnodeDataSet.FilesMetaDataDataTable();
        protected readonly FilesMetaDataTableAdapter _filesMetaDataTableAdapter = new FilesMetaDataTableAdapter();
        protected readonly HostsTableAdapter _hostsTableAdapter = new HostsTableAdapter();
        protected readonly ArachnodeDataSet.HyperLinksDataTable _hyperLinksDataTable = new ArachnodeDataSet.HyperLinksDataTable();
        protected readonly HyperLinksTableAdapter _hyperLinksTableAdapter = new HyperLinksTableAdapter();
        protected readonly ArachnodeDataSet.HyperLinksDiscoveriesDataTable _hyperLinksDiscoveriesDataTable = new ArachnodeDataSet.HyperLinksDiscoveriesDataTable();
        protected readonly HyperLinksDiscoveriesTableAdapter _hyperLinksDiscoveriesTableAdapter = new HyperLinksDiscoveriesTableAdapter();
        protected readonly ArachnodeDataSet.ImagesDataTable _imagesDataTable = new ArachnodeDataSet.ImagesDataTable();
        protected readonly ImagesTableAdapter _imagesTableAdapter = new ImagesTableAdapter();
        protected readonly ArachnodeDataSet.ImagesDiscoveriesDataTable _imagesDiscoveriesDataTable = new ArachnodeDataSet.ImagesDiscoveriesDataTable();
        protected readonly ImagesDiscoveriesTableAdapter _imagesDiscoveriesTableAdapter = new ImagesDiscoveriesTableAdapter();
        protected readonly ArachnodeDataSet.ImagesMetaDataDataTable _imagesMetaDataDataTable = new ArachnodeDataSet.ImagesMetaDataDataTable();
        protected readonly ImagesMetaDataTableAdapter _imagesMetaDataTableAdapter = new ImagesMetaDataTableAdapter();
        protected readonly PrioritiesTableAdapter _prioritiesTableAdapter = new PrioritiesTableAdapter();
        protected readonly QueriesTableAdapter _queriesTableAdapter = new QueriesTableAdapter();
        protected readonly SchemesTableAdapter _schemesTableAdapter = new SchemesTableAdapter();
        protected readonly VersionTableAdapter _versionTableAdapter = new VersionTableAdapter();
        protected readonly ArachnodeDataSet.WebPagesDataTable _webPagesDataTable = new ArachnodeDataSet.WebPagesDataTable();
        protected readonly WebPagesTableAdapter _webPagesTableAdapter = new WebPagesTableAdapter();

        #region Implementation of IArachnodeDAO

        public abstract long? LastDisallowedAbsoluteUriID { get; }
        public abstract long? LastEmailAddressID { get; }
        public abstract long? LastExceptionID { get; }
        public abstract string LastExceptionMessage { get; }
        public abstract long? LastFileID { get; }
        public abstract long? LastHyperLinkID { get; }
        public abstract long? LastImageID { get; }
        public abstract long? LastWebPageID { get; }
        public abstract ApplicationSettings ApplicationSettings { get; set; }
        public abstract WebSettings WebSettings { get; set; }
        public abstract void OpenCommandConnections();
        public abstract void CloseCommandConnections();
        public abstract void CloseCommandConnections(IEnumerable<IDbCommand> dbCommands);
        public abstract void CheckVersion();
        public abstract void DeleteCrawlRequest(string absoluteUri1, string absoluteUri2);
        public abstract void DeleteDiscoveries();
        public abstract void DeleteWebPage(string webPageAbsoluteUriOrID);
        public abstract void ExecuteSql(string query);
        public abstract DataTable ExecuteSql2(string query);
        public abstract ArachnodeDataSet.AllowedDataTypesDataTable GetAllowedDataTypes();
        public abstract ArachnodeDataSet.AllowedExtensionsDataTable GetAllowedExtensions();
        public abstract ArachnodeDataSet.AllowedSchemesDataTable GetAllowedSchemes();
        public abstract ArachnodeDataSet.ConfigurationDataTable GetConfiguration();
        public abstract ArachnodeDataSet.CrawlActionsDataTable GetCrawlActions();
        public abstract ArachnodeDataSet.CrawlRulesDataTable GetCrawlRules();
        public abstract ArachnodeDataSet.ContentTypesDataTable GetContentTypes();
        public abstract ArachnodeDataSet.CrawlRequestsDataTable GetCrawlRequests(int maximumNumberOfCrawlRequestsToCreatePerBatch, bool createCrawlRequestsFromDatabaseCrawlRequests, bool createCrawlRequestsFromDatabaseFiles, bool assignCrawlRequestPrioritiesForFiles, bool createCrawlRequestsFromDatabaseHyperLinks, bool assignCrawlRequestPrioritiesForHyperLinks, bool createCrawlRequestsFromDatabaseImages, bool assignCrawlRequestPrioritiesForImages, bool createCrawlRequestsFromDatabaseWebPages, bool assignCrawlRequestPrioritiesForWebPages);
        public abstract ArachnodeDataSet.DiscoveriesRow GetDiscovery(string absoluteUri);
        public abstract ArachnodeDataSet.DiscoveriesDataTable GetDiscoveries(int numberOfDiscoveriesToReturn);
        public abstract ArachnodeDataSet.DisallowedAbsoluteUrisRow GetDisallowedAbsoluteUri(string disallowedAbsoluteUriAbsoluteUriOrID);
        public abstract ArachnodeDataSet.DisallowedAbsoluteUrisDataTable GetDisallowedAbsoluteUris(int numberOfDisallowedAbsoluteUrisToReturn);
        public abstract ArachnodeDataSet.DisallowedAbsoluteUrisDiscoveriesDataTable GetDisallowedAbsoluteUriDiscoveries(long disallowedAbsoluteUriID);
        public abstract ArachnodeDataSet.DisallowedDomainsDataTable GetDisallowedDomains();
        public abstract ArachnodeDataSet.DisallowedExtensionsDataTable GetDisallowedExtensions();
        public abstract ArachnodeDataSet.DisallowedFileExtensionsDataTable GetDisallowedFileExtensions();
        public abstract ArachnodeDataSet.DisallowedHostsDataTable GetDisallowedHosts();
        public abstract ArachnodeDataSet.DisallowedSchemesDataTable GetDisallowedSchemes();
        public abstract ArachnodeDataSet.DisallowedWordsDataTable GetDisallowedWords();
        public abstract ArachnodeDataSet.DomainsDataTable GetDomains();
        public abstract ArachnodeDataSet.EmailAddressesRow GetEmailAddress(string emailAddressAbsoluteUriOrID);
        public abstract ArachnodeDataSet.EmailAddressesDiscoveriesDataTable GetEmailAddressDiscoveries(long emailAddressID);
        public abstract ArachnodeDataSet.EngineActionsDataTable GetEngineActions();
        public abstract ArachnodeDataSet.ExtensionsDataTable GetExtensions();
        public abstract ArachnodeDataSet.FilesRow GetFile(string fileAbsoluteUriOrID);
        public abstract ArachnodeDataSet.FilesDiscoveriesDataTable GetFileDiscoveries(long fileID);
        public abstract ArachnodeDataSet.HostsDataTable GetHosts();
        public abstract ArachnodeDataSet.VersionDataTable GetVersion();
        public abstract ArachnodeDataSet.FilesMetaDataRow GetFileMetaData(long fileID);
        public abstract ArachnodeDataSet.HyperLinksRow GetHyperLink(string hyperLinkAbsoluteUriOrID);
        public abstract ArachnodeDataSet.HyperLinksDiscoveriesDataTable GetHyperLinkDiscoveries(long hyperLinkID);
        public abstract ArachnodeDataSet.ImagesRow GetImage(string imageAbsoluteUriOrID);
        public abstract ArachnodeDataSet.ImagesDiscoveriesDataTable GetImageDiscoveries(long imageID);
        public abstract ArachnodeDataSet.ImagesMetaDataRow GetImageMetaData(long imageID);
        public abstract ArachnodeDataSet.PrioritiesDataTable GetPriorities(int maximumNumberOfPriorities);
        public abstract ArachnodeDataSet.SchemesDataTable GetSchemes();
        public abstract ArachnodeDataSet.WebPagesRow GetWebPage(string webPageAbsoluteUriOrID);
        public abstract long? InsertBusinessInformation(string parentWebPageAbsoluteUri, string webPageAbsoluteUri, long webPageID, string name, string address1, string address2, string city, string state, string zip, string phoneNumber, string category, string latitude, string longitude);
        public abstract long? InsertCrawlRequest(DateTime created, string absoluteUri0, string absoluteUri1, string absoluteUri2, int depth, short restrictCrawlTo, short restrictDiscoveriesTo, double priority, byte renderType, byte renderTypeForChildren);
        public abstract long? InsertCrawlRequest(DateTime created, string absoluteUri0, string absoluteUri1, string absoluteUri2, int currentDepth, int maximumDepth, short restrictCrawlTo, short restrictDiscoveriesTo, double priority, byte renderType, byte renderTypeForChildren);
        public abstract void InsertDiscovery(long? discoveryID, string absoluteUri, int discoveryStateID, int discoveryTypeID, bool expectFileOrImage, int numberOfTimesDiscovered);
        public abstract long? InsertDisallowedAbsoluteUri(int contentTypeID, int discoveryTypeID, string webPageAbsoluteUri, string disallowedAbsoluteUriAbsoluteUri, string reason, bool classifyAbsoluteUris);
        public abstract void InsertDisallowedAbsoluteUriDiscovery(string webPageAbsoluteUri, string disallowedAbsoluteUriAbsoluteUri);
        public abstract long? InsertDocument(string parentWebPageAbsoluteUri, string webPageAbsoluteUri, byte documentTypeID, long webPageID, double weight, string field01, string field02, string field03, string field04, string field05, string field06, string field07, string field08, string field09, string field10, string field11, string field12, string field13, string field14, string field15, string field16, string field17, string field18, string field19, string field20, string fullTextIndexType);
        public abstract long? InsertEmailAddress(string webPageAbsoluteUri, string emailAddressAbsoluteUri, bool classifyAbsoluteUris);
        public abstract void InsertEmailAddressDiscovery(string webPageAbsoluteUri, string emailAddressAbsoluteUri);
        public abstract long? InsertException(string absoluteUri1, string absoluteUri2, Exception exception, bool insertExceptionInWindowsApplicationLog);
        public abstract long? InsertException(string absoluteUri1, string absoluteUri2, string helpLink, string message, string source, string stackTrace, bool insertExceptionInWindowsApplicationLog);
        public abstract long? InsertFile(string webPageAbsoluteUri, string fileAbsoluteUri, string responseHeaders, byte[] source, string fullTextIndexType, bool classifyAbsoluteUris);
        public abstract void InsertFileDiscovery(string webPageAbsoluteUri, string fileAbsoluteUri);
        public abstract void InsertFileMetaData(string fileAbsoluteUri, long fileID);
        public abstract long? InsertHyperLink(string webPageAbsoluteUri, string hyperLinkAbsoluteUri, bool classifyAbsoluteUris);
        public abstract void InsertHyperLinkDiscovery(string webPageAbsoluteUri, string hyperLinkAbsoluteUri);
        public abstract long? InsertImage(string webPageAbsoluteUri, string imageAbsoluteUri, string responseHeaders, byte[] source, string fullTextIndexType);
        public abstract void InsertImageDiscovery(string webPageAbsoluteUri, string imageAbsoluteUri);
        public abstract void InsertImageMetaData(string imageAbsoluteUri, long imageID, string exifData, int flags, int height, double horizontalResolution, double verticalResolution, int width);
        public abstract long? InsertWebPage(string webPageAbsoluteUri, string responseHeaders, byte[] source, int codePage, string fullTextIndexType, int crawlDepth, bool classifyAbsoluteUris);
        public abstract void InsertWebPageMetaData(long webPageID, string webPageAbsoluteUri, byte[] text, string xml);

        #endregion
    }
}
