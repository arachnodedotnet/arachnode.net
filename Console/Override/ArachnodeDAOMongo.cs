using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Arachnode.Configuration;
using Arachnode.Configuration.Value.Enums;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Managers;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.DataSource;
using Arachnode.DataSource.ArachnodeDataSetTableAdapters;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Arachnode.Console.Override
{
    public class ArachnodeDAOMongo : ArachnodeDAO, IArachnodeDAO
    {
        protected new ApplicationSettings _applicationSettings;
        protected new WebSettings _webSettings;

        protected new readonly AllowedDataTypesTableAdapter _allowedDataTypesTableAdapter = new AllowedDataTypesTableAdapter();
        protected new readonly AllowedExtensionsTableAdapter _allowedExtensionsTableAdapter = new AllowedExtensionsTableAdapter();
        protected new readonly AllowedSchemesTableAdapter _allowedSchemesTableAdapter = new AllowedSchemesTableAdapter();
        protected new readonly BusinessInformationTableAdapter _businessInformationTableAdapter = new BusinessInformationTableAdapter();
        protected new readonly ConfigurationTableAdapter _configurationTableAdapter = new ConfigurationTableAdapter();
        protected new readonly ContentTypesTableAdapter _contentTypesTableAdapter = new ContentTypesTableAdapter();
        protected new readonly CrawlActionsTableAdapter _crawlActionsTableAdapter = new CrawlActionsTableAdapter();
        protected new readonly CrawlRequestsTableAdapter _crawlRequestsTableAdapter = new CrawlRequestsTableAdapter();
        protected new readonly CrawlRulesTableAdapter _crawlRulesTableAdapter = new CrawlRulesTableAdapter();
        protected new readonly ArachnodeDataSet.DisallowedAbsoluteUrisDataTable _disallowedAbsoluteUrisDataTable = new ArachnodeDataSet.DisallowedAbsoluteUrisDataTable();
        protected new readonly DisallowedAbsoluteUrisTableAdapter _disallowedAbsoluteUrisTableAdapter = new DisallowedAbsoluteUrisTableAdapter();
        protected new readonly ArachnodeDataSet.DisallowedAbsoluteUrisDiscoveriesDataTable _disallowedAbsoluteUrisDiscoveriesDataTable = new ArachnodeDataSet.DisallowedAbsoluteUrisDiscoveriesDataTable();
        protected new readonly DisallowedAbsoluteUrisDiscoveriesTableAdapter _disallowedAbsoluteUrisDiscoveriesTableAdapter = new DisallowedAbsoluteUrisDiscoveriesTableAdapter();
        protected new readonly ArachnodeDataSet.DiscoveriesDataTable _discoveriesDataTable = new ArachnodeDataSet.DiscoveriesDataTable();
        protected new readonly DisallowedDomainsTableAdapter _disallowedDomainsTableAdapter = new DisallowedDomainsTableAdapter();
        protected new readonly DisallowedExtensionsTableAdapter _disallowedExtensionsTableAdapter = new DisallowedExtensionsTableAdapter();
        protected new readonly DisallowedFileExtensionsTableAdapter _disallowedFileExtensionsTableAdapter = new DisallowedFileExtensionsTableAdapter();
        protected new readonly DisallowedHostsTableAdapter _disallowedHostsTableAdapter = new DisallowedHostsTableAdapter();
        protected new readonly DisallowedSchemesTableAdapter _disallowedSchemesTableAdapter = new DisallowedSchemesTableAdapter();
        protected new readonly DisallowedWordsTableAdapter _disallowedWordsTableAdapter = new DisallowedWordsTableAdapter();
        protected new readonly DiscoveriesTableAdapter _discoveriesTableAdapter = new DiscoveriesTableAdapter();
        protected new readonly DomainsTableAdapter _domainsTableAdapter = new DomainsTableAdapter();
        protected new readonly ArachnodeDataSet.EmailAddressesDataTable _emailAddressesDataTable = new ArachnodeDataSet.EmailAddressesDataTable();
        protected new readonly EmailAddressesTableAdapter _emailAddressesTableAdapter = new EmailAddressesTableAdapter();
        protected new readonly ArachnodeDataSet.EmailAddressesDiscoveriesDataTable _emailAddressesDiscoveriesDataTable = new ArachnodeDataSet.EmailAddressesDiscoveriesDataTable();
        protected new readonly EmailAddressesDiscoveriesTableAdapter _emailAddressesDiscoveriesTableAdapter = new EmailAddressesDiscoveriesTableAdapter();
        protected new readonly EngineActionsTableAdapter _engineActionsTableAdapter = new EngineActionsTableAdapter();
        protected new readonly ExtensionsTableAdapter _extensionsTableAdapter = new ExtensionsTableAdapter();
        protected new readonly ArachnodeDataSet.FilesDataTable _filesDataTable = new ArachnodeDataSet.FilesDataTable();
        protected new readonly FilesTableAdapter _filesTableAdapter = new FilesTableAdapter();
        protected new readonly ArachnodeDataSet.FilesDiscoveriesDataTable _filesDiscoveriesDataTable = new ArachnodeDataSet.FilesDiscoveriesDataTable();
        protected new readonly FilesDiscoveriesTableAdapter _filesDiscoveriesTableAdapter = new FilesDiscoveriesTableAdapter();
        protected new readonly ArachnodeDataSet.FilesMetaDataDataTable _filesMetaDataDataTable = new ArachnodeDataSet.FilesMetaDataDataTable();
        protected new readonly FilesMetaDataTableAdapter _filesMetaDataTableAdapter = new FilesMetaDataTableAdapter();
        protected new readonly HostsTableAdapter _hostsTableAdapter = new HostsTableAdapter();
        protected new readonly ArachnodeDataSet.HyperLinksDataTable _hyperLinksDataTable = new ArachnodeDataSet.HyperLinksDataTable();
        protected new readonly HyperLinksTableAdapter _hyperLinksTableAdapter = new HyperLinksTableAdapter();
        protected new readonly ArachnodeDataSet.HyperLinksDiscoveriesDataTable _hyperLinksDiscoveriesDataTable = new ArachnodeDataSet.HyperLinksDiscoveriesDataTable();
        protected new readonly HyperLinksDiscoveriesTableAdapter _hyperLinksDiscoveriesTableAdapter = new HyperLinksDiscoveriesTableAdapter();
        protected new readonly ArachnodeDataSet.ImagesDataTable _imagesDataTable = new ArachnodeDataSet.ImagesDataTable();
        protected new readonly ImagesTableAdapter _imagesTableAdapter = new ImagesTableAdapter();
        protected new readonly ArachnodeDataSet.ImagesDiscoveriesDataTable _imagesDiscoveriesDataTable = new ArachnodeDataSet.ImagesDiscoveriesDataTable();
        protected new readonly ImagesDiscoveriesTableAdapter _imagesDiscoveriesTableAdapter = new ImagesDiscoveriesTableAdapter();
        protected new readonly ArachnodeDataSet.ImagesMetaDataDataTable _imagesMetaDataDataTable = new ArachnodeDataSet.ImagesMetaDataDataTable();
        protected new readonly ImagesMetaDataTableAdapter _imagesMetaDataTableAdapter = new ImagesMetaDataTableAdapter();
        protected new readonly PrioritiesTableAdapter _prioritiesTableAdapter = new PrioritiesTableAdapter();
        protected new readonly QueriesTableAdapter _queriesTableAdapter = new QueriesTableAdapter();
        protected new readonly SchemesTableAdapter _schemesTableAdapter = new SchemesTableAdapter();
        protected new readonly VersionTableAdapter _versionTableAdapter = new VersionTableAdapter();
        protected new readonly ArachnodeDataSet.WebPagesDataTable _webPagesDataTable = new ArachnodeDataSet.WebPagesDataTable();
        protected new readonly WebPagesTableAdapter _webPagesTableAdapter = new WebPagesTableAdapter();

        //private long? _lastCrawlRequestID;
        private long? _lastDisallowedAbsoluteUriID;
        private long? _lastEmailAddressID;
        private long? _lastExceptionID;
        private string _lastExceptionMessage;
        private long? _lastFileID;
        private long? _lastHyperLinkID;
        private long? _lastImageID;
        private long? _lastWebPageID;

        private MongoServer _mongoServer;
        private MongoDatabase _mongoDatabase;

        private MongoCollection<BsonDocument> _crawlRequestsCollection;
        private MongoCollection<BsonDocument> _discoveriesCollection;
        private MongoCollection<BsonDocument> _exceptionsCollection;
        private MongoCollection<BsonDocument> _versionCollection;

        public ArachnodeDAOMongo(string connectionString) : this(connectionString, null, null, false, false)
        {
            InitializeMongoDB();
        }

        //don't call the base implementation as it references SQL Server specific commands...
        public ArachnodeDAOMongo(string connectionString, ApplicationSettings applicationSettings, WebSettings webSettings, bool initializeApplicationConfiguration, bool initializeWebConfiguration) : base(connectionString, applicationSettings, webSettings, initializeApplicationConfiguration, initializeWebConfiguration)
        {
            ConnectionString.Value = connectionString;

            _applicationSettings = applicationSettings;
            if (_applicationSettings != null)
            {
                _applicationSettings.ConnectionString = connectionString;
            }

            _webSettings = webSettings;
            if (_webSettings != null)
            {
                _webSettings.ConnectionString = connectionString;
            }

            InitializeMongoDB();

            CheckVersion();

            if (initializeApplicationConfiguration)
            {
                ConfigurationManager.InitializeConfiguration(ref _applicationSettings, ref _webSettings, ConfigurationType.Application, this);
            }

            if (initializeWebConfiguration)
            {
                ConfigurationManager.InitializeConfiguration(ref _applicationSettings, ref _webSettings, ConfigurationType.Web, this);
            }
        }

        private void InitializeMongoDB()
        {
            MongoServerSettings mongoServerSettings = new MongoServerSettings();

            mongoServerSettings.MinConnectionPoolSize = ApplicationSettings.MaximumNumberOfCrawlThreads;
            mongoServerSettings.Server = new MongoServerAddress("127.0.0.1");

            _mongoServer = new MongoServer(mongoServerSettings);

            _mongoServer.Connect(TimeSpan.FromSeconds(60));

            _mongoDatabase = _mongoServer.GetDatabase("arachnodedotnet");

            //pre-load the collections...

            //CrawlRequests...
            if (!_mongoDatabase.CollectionExists("CrawlRequests"))
            {
                CommandResult commandResult = _mongoDatabase.CreateCollection("CrawlRequests");

                if (!commandResult.Ok)
                {
                    throw new Exception(commandResult.ErrorMessage);
                }
            }

            _crawlRequestsCollection = _mongoDatabase.GetCollection("CrawlRequests");

            //Discoveries...
            if (!_mongoDatabase.CollectionExists("Discoveries"))
            {
                CommandResult commandResult = _mongoDatabase.CreateCollection("Discoveries");

                if (!commandResult.Ok)
                {
                    throw new Exception(commandResult.ErrorMessage);
                }
            }

            _discoveriesCollection = _mongoDatabase.GetCollection("Discoveries");

            //Exceptions...
            if (!_mongoDatabase.CollectionExists("Exceptions"))
            {
                CommandResult commandResult = _mongoDatabase.CreateCollection("Exceptions");

                if (!commandResult.Ok)
                {
                    throw new Exception(commandResult.ErrorMessage);
                }
            }

            _exceptionsCollection = _mongoDatabase.GetCollection("Exceptions");

            //cfg.Version...
            if (!_mongoDatabase.CollectionExists("cfg.Version"))
            {
                CommandResult commandResult = _mongoDatabase.CreateCollection("cfg.Version");

                if (!commandResult.Ok)
                {
                    throw new Exception(commandResult.ErrorMessage);
                }
            }

            _versionCollection = _mongoDatabase.GetCollection("cfg.Version");
        }

        /// <summary>
        /// 	The last ExceptionID submitted to table 'DisallowedAbsoluteUris'.
        /// </summary>
        /// <value>The last disallowed absolute URI ID.</value>
        public new long? LastDisallowedAbsoluteUriID
        {
            get { return _lastDisallowedAbsoluteUriID; }
            private set { _lastDisallowedAbsoluteUriID = value; }
        }

        /// <summary>
        /// 	The last EmailAddressID submitted to table 'EmailAddresses'.
        /// </summary>
        /// <value>The last email address ID.</value>
        public new long? LastEmailAddressID
        {
            get { return _lastEmailAddressID; }
            private set { _lastEmailAddressID = value; }
        }

        /// <summary>
        /// 	The last ExceptionID submitted to table 'Exceptions'.
        /// </summary>
        /// <value>The last exception ID.</value>
        public new long? LastExceptionID
        {
            get { return _lastExceptionID; }
            private set { _lastExceptionID = value; }
        }

        /// <summary>
        /// 	The last Message submitted to table 'Exceptions'.
        /// </summary>
        /// <value>The last exception message.</value>
        public new string LastExceptionMessage
        {
            get { return _lastExceptionMessage; }
            private set { _lastExceptionMessage = value; }
        }

        /// <summary>
        /// 	The last FileID submitted to table 'Files'.
        /// </summary>
        /// <value>The last file ID.</value>
        public new long? LastFileID
        {
            get { return _lastFileID; }
            private set { _lastFileID = value; }
        }

        /// <summary>
        /// 	The last HyperLinksID submitted to table 'HyperLinks'.
        /// </summary>
        /// <value>The last hyper link ID.</value>
        public new long? LastHyperLinkID
        {
            get { return _lastHyperLinkID; }
            private set { _lastHyperLinkID = value; }
        }

        /// <summary>
        /// 	The last ImageID submitted to table 'Images'.
        /// </summary>
        /// <value>The last image ID.</value>
        public new long? LastImageID
        {
            get { return _lastImageID; }
            private set { _lastImageID = value; }
        }

        /// <summary>
        /// 	The last WebPageID submitted to table 'WebPages'.
        /// </summary>
        public new long? LastWebPageID
        {
            get { return _lastWebPageID; }
            private set { _lastWebPageID = value; }
        }

        public new ApplicationSettings ApplicationSettings
        {
            get { return _applicationSettings; }
            set { _applicationSettings = value; }
        }

        public new WebSettings WebSettings
        {
            get { return _webSettings; }
            set { _webSettings = value; }
        }

        public new void OpenCommandConnections()
        {
            //not applicable for non-SQL Server applications...
        }

        public new void CloseCommandConnections()
        {
            //not applicable for non-SQL Server applications...
        }

        public new void CloseCommandConnections(IEnumerable<IDbCommand> dbCommands)
        {
            //not applicable for non-SQL Server applications...
        }

        public new void CheckVersion()
        {
            ArachnodeDataSet.VersionDataTable versionDataTable = GetVersion();

            CheckVersion(versionDataTable);
        }

        public new void DeleteCrawlRequest(string absoluteUri1, string absoluteUri2)
        {
        }

        public new void DeleteDiscoveries()
        {
        }

        public new void DeleteWebPage(string webPageAbsoluteUriOrID)
        {
        }

        public new void ExecuteSql(string query)
        {
        }

        public new DataTable ExecuteSql2(string query)
        {
            return new DataTable();
        }

        public new ArachnodeDataSet.AllowedDataTypesDataTable GetAllowedDataTypes()
        {
            ArachnodeDataSet.AllowedDataTypesDataTable allowedDataTypesDataTable = new ArachnodeDataSet.AllowedDataTypesDataTable();

            try
            {
                if (File.Exists("cfg.AllowedDataTypes.xml"))
                {
                    allowedDataTypesDataTable.ReadXml("cfg.AllowedDataTypes.xml");
                }
            }
            catch (Exception exception)
            {
                InsertException(null, null, exception, true);
            }

            return allowedDataTypesDataTable;
        }

        public new ArachnodeDataSet.AllowedExtensionsDataTable GetAllowedExtensions()
        {
            ArachnodeDataSet.AllowedExtensionsDataTable allowedExtensionsDataTable = new ArachnodeDataSet.AllowedExtensionsDataTable();

            try
            {
                if (File.Exists("cfg.AllowedExtensions.xml"))
                {
                    allowedExtensionsDataTable.ReadXml("cfg.AllowedExtensions.xml");
                }
            }
            catch (Exception exception)
            {
                InsertException(null, null, exception, true);
            }

            return allowedExtensionsDataTable;
        }

        public new ArachnodeDataSet.AllowedSchemesDataTable GetAllowedSchemes()
        {
            ArachnodeDataSet.AllowedSchemesDataTable allowedSchemesDataTable = new ArachnodeDataSet.AllowedSchemesDataTable();

            try
            {
                if (File.Exists("cfg.AllowedSchemes.xml"))
                {
                    allowedSchemesDataTable.ReadXml("cfg.AllowedSchemes.xml");
                }
            }
            catch (Exception exception)
            {
                InsertException(null, null, exception, true);
            }

            return allowedSchemesDataTable;
        }

        public new ArachnodeDataSet.ConfigurationDataTable GetConfiguration()
        {
            ArachnodeDataSet.ConfigurationDataTable configurationDataTable = new ArachnodeDataSet.ConfigurationDataTable();

            try
            {
                if (File.Exists("cfg.Configuration.xml"))
                {
                    configurationDataTable.ReadXml("cfg.Configuration.xml");
                }
            }
            catch (Exception exception)
            {
                InsertException(null, null, exception, true);
            }

            return configurationDataTable;
        }

        public new ArachnodeDataSet.CrawlActionsDataTable GetCrawlActions()
        {
            ArachnodeDataSet.CrawlActionsDataTable crawlActionsDataTable = new ArachnodeDataSet.CrawlActionsDataTable();

            try
            {
                if (File.Exists("cfg.CrawlActions.xml"))
                {
                    crawlActionsDataTable.ReadXml("cfg.CrawlActions.xml");
                }
            }
            catch (Exception exception)
            {
                InsertException(null, null, exception, true);
            }

            return crawlActionsDataTable;
        }

        public new ArachnodeDataSet.CrawlRulesDataTable GetCrawlRules()
        {
            ArachnodeDataSet.CrawlRulesDataTable crawlRulesDataTable = new ArachnodeDataSet.CrawlRulesDataTable();

            try
            {
                if (File.Exists("cfg.CrawlRules.xml"))
                {
                    crawlRulesDataTable.ReadXml("cfg.CrawlRules.xml");
                }
            }
            catch (Exception exception)
            {
                InsertException(null, null, exception, true);
            }

            return crawlRulesDataTable;
        }

        public new ArachnodeDataSet.ContentTypesDataTable GetContentTypes()
        {
            ArachnodeDataSet.ContentTypesDataTable contentTypesDataTable = new ArachnodeDataSet.ContentTypesDataTable();

            try
            {
                if (File.Exists("cfg.ContentTypes.xml"))
                {
                    contentTypesDataTable.ReadXml("cfg.ContentTypes.xml");
                }
            }
            catch (Exception exception)
            {
                InsertException(null, null, exception, true);
            }

            return contentTypesDataTable;
        }

        public new ArachnodeDataSet.CrawlRequestsDataTable GetCrawlRequests(int maximumNumberOfCrawlRequestsToCreatePerBatch, bool createCrawlRequestsFromDatabaseCrawlRequests, bool createCrawlRequestsFromDatabaseFiles, bool assignCrawlRequestPrioritiesForFiles, bool createCrawlRequestsFromDatabaseHyperLinks, bool assignCrawlRequestPrioritiesForHyperLinks, bool createCrawlRequestsFromDatabaseImages, bool assignCrawlRequestPrioritiesForImages, bool createCrawlRequestsFromDatabaseWebPages, bool assignCrawlRequestPrioritiesForWebPages)
        {
            return new ArachnodeDataSet.CrawlRequestsDataTable();
        }

        public new ArachnodeDataSet.DiscoveriesRow GetDiscovery(string absoluteUri)
        {
            try
            {
                BsonDocument bsonDocument = new BsonDocument(false);

                bsonDocument.Add("absoluteUri", BsonValue.Create(absoluteUri));

                /**/

                IMongoQuery mongoQuery = Query.EQ("absoluteUri", absoluteUri);

                BsonDocument mongoResponse = _mongoDatabase.GetCollection("Discoveries").FindOne(mongoQuery);

                if(mongoResponse != null)
                {
                    ArachnodeDataSet.DiscoveriesRow discoveriesRow = _discoveriesDataTable.NewDiscoveriesRow();

                    if (mongoResponse["id"].AsNullableInt64 != BsonNull.Value)
                    {
                        discoveriesRow.ID = mongoResponse["id"].AsInt64;
                    }
                    discoveriesRow.AbsoluteUri = mongoResponse["absoluteUri"].AsString;
                    discoveriesRow.DiscoveryStateID = (byte)mongoResponse["discoveryStateID"].AsInt32;
                    discoveriesRow.DiscoveryTypeID = (byte)mongoResponse["discoveryTypeID"].AsInt32;
                    discoveriesRow.ExpectFileOrImage = mongoResponse["expectFileOrImage"].AsBoolean;
                    discoveriesRow.NumberOfTimesDiscovered = mongoResponse["numberOfTimesDiscovered"].AsInt32;
                }
            }
            catch (Exception exception)
            {
                InsertException(absoluteUri, null, exception, false);
            }

            return null;
        }

        public new ArachnodeDataSet.DiscoveriesDataTable GetDiscoveries(int numberOfDiscoveriesToReturn)
        {
            return new ArachnodeDataSet.DiscoveriesDataTable();
        }

        public new ArachnodeDataSet.DisallowedAbsoluteUrisRow GetDisallowedAbsoluteUri(string disallowedAbsoluteUriAbsoluteUriOrID)
        {
            return null;
        }

        public new ArachnodeDataSet.DisallowedAbsoluteUrisDataTable GetDisallowedAbsoluteUris(int numberOfDisallowedAbsoluteUrisToReturn)
        {
            return null;
        }

        public new ArachnodeDataSet.DisallowedAbsoluteUrisDiscoveriesDataTable GetDisallowedAbsoluteUriDiscoveries(long disallowedAbsoluteUriID)
        {
            return null;
        }

        public new ArachnodeDataSet.DisallowedDomainsDataTable GetDisallowedDomains()
        {
            return null;
        }

        public new ArachnodeDataSet.DisallowedExtensionsDataTable GetDisallowedExtensions()
        {
            return null;
        }

        public new ArachnodeDataSet.DisallowedFileExtensionsDataTable GetDisallowedFileExtensions()
        {
            return null;
        }

        public new ArachnodeDataSet.DisallowedHostsDataTable GetDisallowedHosts()
        {
            return null;
        }

        public new ArachnodeDataSet.DisallowedSchemesDataTable GetDisallowedSchemes()
        {
            return null;
        }

        public new ArachnodeDataSet.DisallowedWordsDataTable GetDisallowedWords()
        {
            return null;
        }

        public new ArachnodeDataSet.DomainsDataTable GetDomains()
        {
            return null;
        }

        public new ArachnodeDataSet.EmailAddressesRow GetEmailAddress(string emailAddressAbsoluteUriOrID)
        {
            return null;
        }

        public new ArachnodeDataSet.EmailAddressesDiscoveriesDataTable GetEmailAddressDiscoveries(long emailAddressID)
        {
            return null;
        }

        public new ArachnodeDataSet.EngineActionsDataTable GetEngineActions()
        {
            ArachnodeDataSet.EngineActionsDataTable engineActionsDataTable = new ArachnodeDataSet.EngineActionsDataTable();

            try
            {
                if (File.Exists("cfg.EngineActions.xml"))
                {
                    engineActionsDataTable.ReadXml("cfg.EngineActions.xml");
                }
            }
            catch (Exception exception)
            {
                InsertException(null, null, exception, true);
            }

            return engineActionsDataTable;
        }

        public new ArachnodeDataSet.ExtensionsDataTable GetExtensions()
        {
            return null;
        }

        public new ArachnodeDataSet.FilesRow GetFile(string fileAbsoluteUriOrID)
        {
            return null;
        }

        public new ArachnodeDataSet.FilesDiscoveriesDataTable GetFileDiscoveries(long fileID)
        {
            return null;
        }

        public new ArachnodeDataSet.HostsDataTable GetHosts()
        {
            throw new NotImplementedException();
        }

        public new ArachnodeDataSet.VersionDataTable GetVersion()
        {
            try
            {
                IMongoQuery mongoQuery = Query.EQ("value", VERSION);

                BsonDocument mongoResponse = _versionCollection.FindOne(mongoQuery);

                if (mongoResponse != null)
                {
                    ArachnodeDataSet.VersionRow versionRow = _versionDataTable.NewVersionRow();

                    if (mongoResponse["value"].AsString != BsonNull.Value)
                    {
                        versionRow.Value = mongoResponse["value"].AsString;
                    }

                    _versionDataTable.AddVersionRow(versionRow);
                }
                else
                {
                    BsonDocument bsonDocument = new BsonDocument(false);

                    bsonDocument.Add("value", BsonValue.Create(VERSION, true));

                    _versionCollection.Insert(bsonDocument);

                    _versionDataTable.AddVersionRow(VERSION);
                }                
            }
            catch (Exception exception)
            {
                InsertException(null, null, exception, false);

                throw new Exception(exception.Message, exception);
            }

            return _versionDataTable;
        }

        public new ArachnodeDataSet.FilesMetaDataRow GetFileMetaData(long fileID)
        {
            return null;
        }

        public new ArachnodeDataSet.HyperLinksRow GetHyperLink(string hyperLinkAbsoluteUriOrID)
        {
            return null;
        }

        public new ArachnodeDataSet.HyperLinksDiscoveriesDataTable GetHyperLinkDiscoveries(long hyperLinkID)
        {
            return null;
        }

        public new ArachnodeDataSet.ImagesRow GetImage(string imageAbsoluteUriOrID)
        {
            return null;
        }

        public new ArachnodeDataSet.ImagesDiscoveriesDataTable GetImageDiscoveries(long imageID)
        {
            return null;
        }

        public new ArachnodeDataSet.ImagesMetaDataRow GetImageMetaData(long imageID)
        {
            return null;
        }

        public new ArachnodeDataSet.PrioritiesDataTable GetPriorities(int maximumNumberOfPriorities)
        {   //TODO: Selects the Top 10000 (all) from the cfg.Priorities table...  (the input param should actually work...)
            ArachnodeDataSet.PrioritiesDataTable prioritiesDataTable = new ArachnodeDataSet.PrioritiesDataTable();

            try
            {
                if (File.Exists("cfg.Priorities.xml"))
                {
                    prioritiesDataTable.ReadXml("cfg.Priorities.xml");
                }
            }
            catch (Exception exception)
            {
                InsertException(null, null, exception, true);
            }

            return prioritiesDataTable;
        }

        public new ArachnodeDataSet.SchemesDataTable GetSchemes()
        {
            throw new NotImplementedException();
        }

        public new ArachnodeDataSet.WebPagesRow GetWebPage(string webPageAbsoluteUriOrID)
        {
            return null;
        }

        public new long? InsertBusinessInformation(string parentWebPageAbsoluteUri, string webPageAbsoluteUri, long webPageID, string name, string address1, string address2, string city, string state, string zip, string phoneNumber, string category, string latitude, string longitude)
        {
            return null;
        }

        public new long? InsertCrawlRequest(DateTime created, string absoluteUri0, string absoluteUri1, string absoluteUri2, int depth, short restrictCrawlTo, short restrictDiscoveriesTo, double priority, byte renderType, byte renderTypeForChildren)
        {
            return null;
        }

        public new long? InsertCrawlRequest(DateTime created, string absoluteUri0, string absoluteUri1, string absoluteUri2, int currentDepth, int maximumDepth, short restrictCrawlTo, short restrictDiscoveriesTo, double priority, byte renderType, byte renderTypeForChildren)
        {
            return null;
        }

        public new void InsertDiscovery(long? discoveryID, string absoluteUri, int discoveryStateID, int discoveryTypeID, bool expectFileOrImage, int numberOfTimesDiscovered)
        {
            try
            {
                BsonDocument bsonDocument = new BsonDocument(false);

                bsonDocument.Add("id", BsonValue.Create(discoveryID, true));
                bsonDocument.Add("absoluteUri", BsonValue.Create(absoluteUri, true));
                bsonDocument.Add("discoveryStateID", BsonValue.Create(discoveryStateID, true));
                bsonDocument.Add("discoveryTypeID", BsonValue.Create(discoveryTypeID, true));
                bsonDocument.Add("expectFileOrImage", BsonValue.Create(expectFileOrImage, true));
                bsonDocument.Add("numberOfTimesDiscovered", BsonValue.Create(numberOfTimesDiscovered, true));

                IMongoQuery mongoQuery = Query.EQ("absoluteUri", absoluteUri);

                BsonDocument mongoResponse = _discoveriesCollection.FindOne(mongoQuery);

                if (mongoResponse != null)
                {
                    mongoResponse["id"] = bsonDocument["id"];
                    mongoResponse["discoveryStateID"] = bsonDocument["discoveryStateID"];
                    mongoResponse["discoveryTypeID"] = bsonDocument["discoveryTypeID"];
                    mongoResponse["expectFileOrImage"] = bsonDocument["expectFileOrImage"];
                    mongoResponse["numberOfTimesDiscovered"] = bsonDocument["numberOfTimesDiscovered"];

                    UpdateDocument updateDocument = new UpdateDocument();

                    updateDocument.AddRange(mongoResponse);

                    _discoveriesCollection.Update(mongoQuery, updateDocument);
                }
                else
                {
                    _discoveriesCollection.Insert(bsonDocument);
                }
            }
            catch (Exception exception)
            {
                InsertException(absoluteUri, null, exception, false);
            }
        }

        public new void InsertDisallowedAbsoluteUri(int contentTypeID, int discoveryTypeID, string webPageAbsoluteUri, string disallowedAbsoluteUriAbsoluteUri, string reason, bool classifyAbsoluteUris)
        {
        }

        public new void InsertDisallowedAbsoluteUriDiscovery(string webPageAbsoluteUri, string disallowedAbsoluteUriAbsoluteUri)
        {
        }

        public new long? InsertEmailAddress(string webPageAbsoluteUri, string emailAddressAbsoluteUri, bool classifyAbsoluteUris)
        {
            return null;
        }

        public new void InsertEmailAddressDiscovery(string webPageAbsoluteUri, string emailAddressAbsoluteUri)
        {
        }

        public new long? InsertException(string absoluteUri1, string absoluteUri2, Exception exception, bool insertExceptionInWindowsApplicationLog)
        {
            return InsertException(absoluteUri1, absoluteUri2, exception.HelpLink, exception.Message, exception.Source, exception.StackTrace, insertExceptionInWindowsApplicationLog);
        }

        public new long? InsertException(string absoluteUri1, string absoluteUri2, string helpLink, string message, string source, string stackTrace, bool insertExceptionInWindowsApplicationLog)
        {
            try
            {
                BsonDocument bsonDocument = new BsonDocument(false);

                //Interlocked.Increment(ref _lastExceptionID.Value);

                bsonDocument.Add("id", BsonValue.Create(_lastExceptionID, true));
                bsonDocument.Add("absoluteUri", BsonValue.Create(absoluteUri1, true));
                bsonDocument.Add("discoveryStateID", BsonValue.Create(absoluteUri2, true));
                bsonDocument.Add("discoveryTypeID", BsonValue.Create(helpLink, true));
                bsonDocument.Add("message", BsonValue.Create(message, true));
                bsonDocument.Add("source", BsonValue.Create(source, true));
                bsonDocument.Add("stackTrace", BsonValue.Create(stackTrace, true));

                _exceptionsCollection.Insert(bsonDocument);

                return _lastExceptionID;
            }
            catch (Exception exception2)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;

                if (ApplicationSettings != null && ApplicationSettings.EnableConsoleOutput)
                {
                    System.Console.WriteLine("InsertException: " + message);
                }

                System.Console.ForegroundColor = ConsoleColor.Gray;

                System.Console.Beep();
                System.Console.WriteLine(exception2);

                InsertExceptionIntoWindowsApplicationLog(exception2.Message, exception2.StackTrace);
            }

            return null;
        }

        public new long? InsertFile(string webPageAbsoluteUri, string fileAbsoluteUri, string responseHeaders, byte[] source, string fullTextIndexType, bool classifyAbsoluteUris)
        {
            return null;
        }

        public new void InsertFileDiscovery(string webPageAbsoluteUri, string fileAbsoluteUri)
        {
        }

        public new void InsertFileMetaData(string fileAbsoluteUri, long fileID)
        {
        }

        public new long? InsertHyperLink(string webPageAbsoluteUri, string hyperLinkAbsoluteUri, bool classifyAbsoluteUris)
        {
            return null;
        }

        public new void InsertHyperLinkDiscovery(string webPageAbsoluteUri, string hyperLinkAbsoluteUri)
        {
        }

        public new long? InsertImage(string webPageAbsoluteUri, string imageAbsoluteUri, string responseHeaders, byte[] source, string fullTextIndexType)
        {
            return null;
        }

        public new void InsertImageDiscovery(string webPageAbsoluteUri, string imageAbsoluteUri)
        {
        }

        public new void InsertImageMetaData(string imageAbsoluteUri, long imageID, string exifData, int flags, int height, double horizontalResolution, double verticalResolution, int width)
        {
        }

        public new long? InsertWebPage(string webPageAbsoluteUri, string responseHeaders, byte[] source, int codePage, string fullTextIndexType, int crawlDepth, bool classifyAbsoluteUris)
        {
            return null;
        }

        public new void InsertWebPageMetaData(long webPageID, string webPageAbsoluteUri, byte[] text, string xml)
        {
        }
    }
}