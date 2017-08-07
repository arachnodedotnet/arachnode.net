using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Arachnode.Configuration;
using Arachnode.Configuration.Value.Enums;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Managers;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.DataSource;
using Arachnode.DataSource.ArachnodeDataSetTableAdapters;
using Arachnode.Performance;
using Arachnode.Security;
using Arachnode.SiteCrawler.Core;
using Arachnode.SiteCrawler.Managers;

namespace Arachnode.Console.Override
{
    public class ArachnodeDAOFileSystem : IArachnodeDAO
    {
        //locks are needed: IOException cannot distinguish between a disk integrity error and a disk access/concurrency error.
        private static ReaderWriterLockSlim _businessInformationLock = new ReaderWriterLockSlim();
        private static ReaderWriterLockSlim _crawlRequestsLock = new ReaderWriterLockSlim();
        private static ReaderWriterLockSlim _discoveriesLock = new ReaderWriterLockSlim();
        private static ReaderWriterLockSlim _disallowedAbsoluteUrisLock = new ReaderWriterLockSlim();
        private static ReaderWriterLockSlim _disallowedAbsoluteUriDiscoveriesLock = new ReaderWriterLockSlim();
        private static ReaderWriterLockSlim _documentsLock = new ReaderWriterLockSlim();
        private static ReaderWriterLockSlim _emailAddressesLock = new ReaderWriterLockSlim();
        private static ReaderWriterLockSlim _emailAddressDiscoveriesLock = new ReaderWriterLockSlim();
        private static ReaderWriterLockSlim _exceptionsLock = new ReaderWriterLockSlim();
        private static ReaderWriterLockSlim _filesLock = new ReaderWriterLockSlim();
        private static ReaderWriterLockSlim _fileDiscoveriesLock = new ReaderWriterLockSlim();
        private static ReaderWriterLockSlim _fileMetaDataLock = new ReaderWriterLockSlim();
        private static ReaderWriterLockSlim _hyperLinksLock = new ReaderWriterLockSlim();
        private static ReaderWriterLockSlim _hyperLinkDiscoveriesLock = new ReaderWriterLockSlim();
        private static ReaderWriterLockSlim _imagesLock = new ReaderWriterLockSlim();
        private static ReaderWriterLockSlim _imageDiscoveriesLock = new ReaderWriterLockSlim();
        private static ReaderWriterLockSlim _imageMetaDataLock = new ReaderWriterLockSlim();
        private static ReaderWriterLockSlim _webPagesLock = new ReaderWriterLockSlim();
        private static ReaderWriterLockSlim _webPageMetaDataLock = new ReaderWriterLockSlim();

        public const string VERSION = "4.0.0.0";

        protected ApplicationSettings _applicationSettings;
        protected WebSettings _webSettings;

        protected readonly AllowedDataTypesTableAdapter _allowedDataTypesTableAdapter = new AllowedDataTypesTableAdapter();
        protected readonly AllowedExtensionsTableAdapter _allowedExtensionsTableAdapter = new AllowedExtensionsTableAdapter();
        protected readonly AllowedSchemesTableAdapter _allowedSchemesTableAdapter = new AllowedSchemesTableAdapter();
        protected readonly ArachnodeDataSet.BusinessInformationDataTable _businessInformationDataTable = new ArachnodeDataSet.BusinessInformationDataTable();
        protected readonly BusinessInformationTableAdapter _businessInformationTableAdapter = new BusinessInformationTableAdapter();
        protected readonly ConfigurationTableAdapter _configurationTableAdapter = new ConfigurationTableAdapter();
        protected readonly ContentTypesTableAdapter _contentTypesTableAdapter = new ContentTypesTableAdapter();
        protected readonly CrawlActionsTableAdapter _crawlActionsTableAdapter = new CrawlActionsTableAdapter();
        protected readonly ArachnodeDataSet.CrawlRequestsDataTable _crawlRequestsDataTable = new ArachnodeDataSet.CrawlRequestsDataTable();
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
        protected readonly ArachnodeDataSet.DocumentsDataTable _documentsDataTable = new ArachnodeDataSet.DocumentsDataTable();
        protected readonly DomainsTableAdapter _domainsTableAdapter = new DomainsTableAdapter();
        protected readonly ArachnodeDataSet.EmailAddressesDataTable _emailAddressesDataTable = new ArachnodeDataSet.EmailAddressesDataTable();
        protected readonly EmailAddressesTableAdapter _emailAddressesTableAdapter = new EmailAddressesTableAdapter();
        protected readonly ArachnodeDataSet.EmailAddressesDiscoveriesDataTable _emailAddressesDiscoveriesDataTable = new ArachnodeDataSet.EmailAddressesDiscoveriesDataTable();
        protected readonly EmailAddressesDiscoveriesTableAdapter _emailAddressesDiscoveriesTableAdapter = new EmailAddressesDiscoveriesTableAdapter();
        protected readonly EngineActionsTableAdapter _engineActionsTableAdapter = new EngineActionsTableAdapter();
        protected readonly ArachnodeDataSet.ExceptionsDataTable _exceptionsDataTable = new ArachnodeDataSet.ExceptionsDataTable();
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
        protected readonly ArachnodeDataSet.WebPagesMetaDataDataTable _webPagesMetaDataDataTable = new ArachnodeDataSet.WebPagesMetaDataDataTable();

        private long? _lastCrawlRequestID;
        private long? _lastDisallowedAbsoluteUriID;
        private long? _lastEmailAddressID;
        private long? _lastExceptionID;
        private string _lastExceptionMessage;
        private long? _lastFileID;
        private long? _lastHyperLinkID;
        private long? _lastImageID;
        private long? _lastWebPageID;

        private Hash _hash;
        private static string _fileSystemStoragePath = "C:\\FileSystemStoragePath";
        private string _fileSystemStoragePath_BusinessInformation = _fileSystemStoragePath + "\\BusinessInformation";
        private string _fileSystemStoragePath_CrawlRequests = _fileSystemStoragePath + "\\CrawlRequests";
        private string _fileSystemStoragePath_DisallowedAbsoluteUris = _fileSystemStoragePath + "\\DisallowedAbsoluteUris";
        private string _fileSystemStoragePath_Discoveries = _fileSystemStoragePath + "\\Discoveries";
        private string _fileSystemStoragePath_Documents = _fileSystemStoragePath + "\\Documents";
        private string _fileSystemStoragePath_EmailAddresses = _fileSystemStoragePath + "\\EmailAddresses";
        private string _fileSystemStoragePath_Exceptions = _fileSystemStoragePath + "\\Exceptions";
        private string _fileSystemStoragePath_EmailAddresses_Discoveries = _fileSystemStoragePath + "\\EmailAddresses_Discoveries";
        private string _fileSystemStoragePath_Files = _fileSystemStoragePath + "\\Files";
        private string _fileSystemStoragePath_Files_Discoveries = _fileSystemStoragePath + "\\Files_Discoveries";
        private string _fileSystemStoragePath_Files_MetaData = _fileSystemStoragePath + "\\Files_MetaData";
        private string _fileSystemStoragePath_HyperLinks = _fileSystemStoragePath + "\\HyperLinks";
        private string _fileSystemStoragePath_HyperLinks_Discoveries = _fileSystemStoragePath + "\\HyperLinks_Discoveries";
        private string _fileSystemStoragePath_Images = _fileSystemStoragePath + "\\Images";
        private string _fileSystemStoragePath_Images_Discoveries = _fileSystemStoragePath + "\\Images_Discoveries";
        private string _fileSystemStoragePath_Images_MetaData = _fileSystemStoragePath + "\\Images_MetaData";
        private string _fileSystemStoragePath_WebPages = _fileSystemStoragePath + "\\WebPages";
        private string _fileSystemStoragePath_WebPages_MetaData = _fileSystemStoragePath + "\\WebPages_MetaData";

        private DiscoveryManager<ArachnodeDAOFileSystem> _discoveryManager;

        public ArachnodeDAOFileSystem(string connectionString) : this(connectionString, null, null, true, true)
        {
        }

        public ArachnodeDAOFileSystem(string connectionString, ApplicationSettings applicationSettings, WebSettings webSettings, bool initializeApplicationConfiguration, bool initializeWebConfiguration)
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

            _hash = new Hash();

            CheckVersion();

            if (initializeApplicationConfiguration)
            {
                ConfigurationManager.InitializeConfiguration(ref _applicationSettings, ref _webSettings, ConfigurationType.Application, this);
            }

            if (initializeWebConfiguration)
            {
                ConfigurationManager.InitializeConfiguration(ref _applicationSettings, ref _webSettings, ConfigurationType.Web, this);
            }

            ConsoleManager<ArachnodeDAOFileSystem> consoleManager = new ConsoleManager<ArachnodeDAOFileSystem>(applicationSettings, webSettings);
            ActionManager<ArachnodeDAOFileSystem> actionManager = new ActionManager<ArachnodeDAOFileSystem>(applicationSettings, webSettings, consoleManager);
            CookieManager cookieManager = new CookieManager();
            MemoryManager<ArachnodeDAOFileSystem> memoryManager = new MemoryManager<ArachnodeDAOFileSystem>(applicationSettings, webSettings);
            RuleManager<ArachnodeDAOFileSystem> ruleManager = new RuleManager<ArachnodeDAOFileSystem>(applicationSettings, webSettings, consoleManager);
            CacheManager<ArachnodeDAOFileSystem> cacheManager = new CacheManager<ArachnodeDAOFileSystem>(applicationSettings, webSettings);
            CrawlerPeerManager<ArachnodeDAOFileSystem> crawlerPeerManager = new CrawlerPeerManager<ArachnodeDAOFileSystem>(applicationSettings, webSettings, null, this);
            Cache<ArachnodeDAOFileSystem> cache = new Cache<ArachnodeDAOFileSystem>(applicationSettings, webSettings, null, actionManager, cacheManager, crawlerPeerManager, memoryManager, ruleManager);

            _discoveryManager = new DiscoveryManager<ArachnodeDAOFileSystem>(applicationSettings, webSettings, cache, actionManager, cacheManager, memoryManager, ruleManager);

            CreateFileSystemStoragePaths();

            SetDataTableCaseSensitivity(true);
        }

        /// <summary>
        /// 	The last ExceptionID submitted to table 'DisallowedAbsoluteUris'.
        /// </summary>
        /// <value>The last disallowed absolute URI ID.</value>
        public long? LastDisallowedAbsoluteUriID
        {
            get { return _lastDisallowedAbsoluteUriID; }
            private set { _lastDisallowedAbsoluteUriID = value; }
        }

        /// <summary>
        /// 	The last EmailAddressID submitted to table 'EmailAddresses'.
        /// </summary>
        /// <value>The last email address ID.</value>
        public long? LastEmailAddressID
        {
            get { return _lastEmailAddressID; }
            private set { _lastEmailAddressID = value; }
        }

        /// <summary>
        /// 	The last ExceptionID submitted to table 'Exceptions'.
        /// </summary>
        /// <value>The last exception ID.</value>
        public long? LastExceptionID
        {
            get { return _lastExceptionID; }
            private set { _lastExceptionID = value; }
        }

        /// <summary>
        /// 	The last Message submitted to table 'Exceptions'.
        /// </summary>
        /// <value>The last exception message.</value>
        public string LastExceptionMessage
        {
            get { return _lastExceptionMessage; }
            private set { _lastExceptionMessage = value; }
        }

        /// <summary>
        /// 	The last FileID submitted to table 'Files'.
        /// </summary>
        /// <value>The last file ID.</value>
        public long? LastFileID
        {
            get { return _lastFileID; }
            private set { _lastFileID = value; }
        }

        /// <summary>
        /// 	The last HyperLinksID submitted to table 'HyperLinks'.
        /// </summary>
        /// <value>The last hyper link ID.</value>
        public long? LastHyperLinkID
        {
            get { return _lastHyperLinkID; }
            private set { _lastHyperLinkID = value; }
        }

        /// <summary>
        /// 	The last ImageID submitted to table 'Images'.
        /// </summary>
        /// <value>The last image ID.</value>
        public long? LastImageID
        {
            get { return _lastImageID; }
            private set { _lastImageID = value; }
        }

        /// <summary>
        /// 	The last WebPageID submitted to table 'WebPages'.
        /// </summary>
        public long? LastWebPageID
        {
            get { return _lastWebPageID; }
            private set { _lastWebPageID = value; }
        }

        public ApplicationSettings ApplicationSettings
        {
            get { return _applicationSettings; }
            set { _applicationSettings = value; }
        }

        public WebSettings WebSettings
        {
            get { return _webSettings; }
            set { _webSettings = value; }
        }

        private void SetDataTableCaseSensitivity(bool caseSensitive)
        {
            _businessInformationDataTable.CaseSensitive = caseSensitive;
            _crawlRequestsDataTable.CaseSensitive = caseSensitive;
            _disallowedAbsoluteUrisDataTable.CaseSensitive = caseSensitive;
            _disallowedAbsoluteUrisDiscoveriesDataTable.CaseSensitive = caseSensitive;
            _discoveriesDataTable.CaseSensitive = caseSensitive;
            _emailAddressesDataTable.CaseSensitive = caseSensitive;
            _emailAddressesDiscoveriesDataTable.CaseSensitive = caseSensitive;
            _exceptionsDataTable.CaseSensitive = caseSensitive;
            _filesDataTable.CaseSensitive = caseSensitive;
            _filesDiscoveriesDataTable.CaseSensitive = caseSensitive;
            _filesMetaDataDataTable.CaseSensitive = caseSensitive;
            _hyperLinksDataTable.CaseSensitive = caseSensitive;
            _hyperLinksDiscoveriesDataTable.CaseSensitive = caseSensitive;
            _imagesDataTable.CaseSensitive = caseSensitive;
            _imagesDiscoveriesDataTable.CaseSensitive = caseSensitive;
            _imagesMetaDataDataTable.CaseSensitive = caseSensitive;
            _webPagesDataTable.CaseSensitive = caseSensitive;
            _webPagesMetaDataDataTable.CaseSensitive = caseSensitive;
        }

        public void OpenCommandConnections()
        {
            //not applicable for non-SQL Server applications...
        }

        public void CloseCommandConnections()
        {
            //not applicable for non-SQL Server applications...
        }

        public void CloseCommandConnections(IEnumerable<IDbCommand> dbCommands)
        {
            //not applicable for non-SQL Server applications...
        }

        public void CheckVersion()
        {
            ArachnodeDataSet.VersionDataTable versionDataTable = GetVersion();

            CheckVersion(versionDataTable);
        }

        protected void CheckVersion(ArachnodeDataSet.VersionDataTable versionDataTable)
        {
            if (versionDataTable.Count == 0)
            {
                throw new Exception("EXCEPTION: The installed database version is not equal to the expected applicaton version.\nEXPECTED: " + VERSION + "\nACTUAL: N/A\n\nPlease contact 'arachnode.net' at http://arachnode.net.");
            }

            //ANODET: Update the version for the release.
            if (versionDataTable.Rows[0]["Value"].ToString() != VERSION)
            {
                throw new Exception("EXCEPTION: The installed database version is not equal to the expected applicaton version.\nEXPECTED: " + VERSION + "\nACTUAL: " + versionDataTable.Rows[0]["Value"] + "\n\nPlease contact arachnode.net.");
            }
        }

        private void CreateFileSystemStoragePaths()
        {
            if (!Directory.Exists(_fileSystemStoragePath))
            {
                Directory.CreateDirectory(_fileSystemStoragePath);

                //TODO: This could be smarter - ...
                while (!Directory.Exists(_fileSystemStoragePath))
                {
                    Thread.Sleep(100);

                    if (!Directory.Exists(_fileSystemStoragePath))
                    {
                        Directory.CreateDirectory(_fileSystemStoragePath);
                    }
                }
            }

            string cfgVersionPath = Path.Combine(_fileSystemStoragePath, "cfg.Version");

            if(!File.Exists(cfgVersionPath))
            {
                //create the 'Version.cfg' file...
                File.WriteAllText(cfgVersionPath, VERSION);
            }

            if (!Directory.Exists(_fileSystemStoragePath))
            {
                Directory.CreateDirectory(_fileSystemStoragePath);
            }
            if (!Directory.Exists(_fileSystemStoragePath_BusinessInformation))
            {
                Directory.CreateDirectory(_fileSystemStoragePath_BusinessInformation);
            }
            if (!Directory.Exists(_fileSystemStoragePath_CrawlRequests))
            {
                Directory.CreateDirectory(_fileSystemStoragePath_CrawlRequests);
            }
            if (!Directory.Exists(_fileSystemStoragePath_DisallowedAbsoluteUris))
            {
                Directory.CreateDirectory(_fileSystemStoragePath_DisallowedAbsoluteUris);
            }
            if (!Directory.Exists(_fileSystemStoragePath_Discoveries))
            {
                Directory.CreateDirectory(_fileSystemStoragePath_Discoveries);
            }
            if (!Directory.Exists(_fileSystemStoragePath_Documents))
            {
                Directory.CreateDirectory(_fileSystemStoragePath_Documents);
            }
            if (!Directory.Exists(_fileSystemStoragePath_EmailAddresses))
            {
                Directory.CreateDirectory(_fileSystemStoragePath_EmailAddresses);
            }
            if (!Directory.Exists(_fileSystemStoragePath_Exceptions))
            {
                Directory.CreateDirectory(_fileSystemStoragePath_Exceptions);
            }
            if (!Directory.Exists(_fileSystemStoragePath_EmailAddresses_Discoveries))
            {
                Directory.CreateDirectory(_fileSystemStoragePath_EmailAddresses_Discoveries);
            }
            if (!Directory.Exists(_fileSystemStoragePath_Files))
            {
                Directory.CreateDirectory(_fileSystemStoragePath_Files);
            }
            if (!Directory.Exists(_fileSystemStoragePath_Files_Discoveries))
            {
                Directory.CreateDirectory(_fileSystemStoragePath_Files_Discoveries);
            }
            if (!Directory.Exists(_fileSystemStoragePath_Files_MetaData))
            {
                Directory.CreateDirectory(_fileSystemStoragePath_Files_MetaData);
            }
            if (!Directory.Exists(_fileSystemStoragePath_HyperLinks))
            {
                Directory.CreateDirectory(_fileSystemStoragePath_HyperLinks);
            }
            if (!Directory.Exists(_fileSystemStoragePath_HyperLinks_Discoveries))
            {
                Directory.CreateDirectory(_fileSystemStoragePath_HyperLinks_Discoveries);
            }
            if (!Directory.Exists(_fileSystemStoragePath_Images))
            {
                Directory.CreateDirectory(_fileSystemStoragePath_Images);
            }
            if (!Directory.Exists(_fileSystemStoragePath_Images_Discoveries))
            {
                Directory.CreateDirectory(_fileSystemStoragePath_Images_Discoveries);
            }
            if (!Directory.Exists(_fileSystemStoragePath_Images_MetaData))
            {
                Directory.CreateDirectory(_fileSystemStoragePath_Images_MetaData);
            }
            if (!Directory.Exists(_fileSystemStoragePath_WebPages))
            {
                Directory.CreateDirectory(_fileSystemStoragePath_WebPages);
            }
            if (!Directory.Exists(_fileSystemStoragePath_WebPages_MetaData))
            {
                Directory.CreateDirectory(_fileSystemStoragePath_WebPages_MetaData);
            }
        }

        public void DeleteCrawlRequest(string absoluteUri1, string absoluteUri2)
        {
            try
            {
                if(string.IsNullOrEmpty(absoluteUri1) || string.IsNullOrEmpty(absoluteUri2))
                {
                    if (Directory.Exists(_fileSystemStoragePath_CrawlRequests))
                    {
                        Directory.Delete(_fileSystemStoragePath_CrawlRequests, true);
                    }

                    Directory.CreateDirectory(_fileSystemStoragePath_CrawlRequests);

                    return;
                }

                string discoveryPath = _discoveryManager.GetDiscoveryPath(_fileSystemStoragePath_CrawlRequests, absoluteUri2, ".cr");

                if(_crawlRequestsLock.TryEnterWriteLock(-1))
                {
                    if (File.Exists(discoveryPath))
                    {
                        File.Delete(discoveryPath);
                    }
                }
            }
            catch (Exception exception)
            {
                InsertException(absoluteUri1, absoluteUri2, exception, false);
            }
            finally
            {
                if(_crawlRequestsLock.IsWriteLockHeld)
                {
                    _crawlRequestsLock.ExitWriteLock();
                }
            }
        }

        public void DeleteDiscoveries()
        {
            try
            {
                if (Directory.Exists(_fileSystemStoragePath_Discoveries))
                {
                    Directory.Delete(_fileSystemStoragePath_Discoveries, true);
                }

                Directory.CreateDirectory(_fileSystemStoragePath_Discoveries);
            }
            catch (Exception exception)
            {
                InsertException(null, null, exception, false);
            }
        }

        public void DeleteWebPage(string webPageAbsoluteUriOrID)
        {
            try
            {
                string discoveryPath = _discoveryManager.GetDiscoveryPath(_fileSystemStoragePath_WebPages, webPageAbsoluteUriOrID, ".wp");

                if (File.Exists(discoveryPath))
                {
                    File.Delete(discoveryPath);
                }
            }
            catch (Exception exception)
            {
                InsertException(null, null, exception, false);
            }
        }

        public void ExecuteSql(string query)
        {
            if (query == "EXEC [dbo].[arachnode_usp_arachnode.net_RESET_DATABASE]")
            {
                if (Directory.Exists(_fileSystemStoragePath))
                {
                    Directory.Delete(_fileSystemStoragePath, true);
                }

                CreateFileSystemStoragePaths();

                return;
            }
            
            if(query.StartsWith("UPDATE"))
            {
                return;
            }

            if (query == "DELETE FROM dbo.DisallowedAbsoluteUris")
            {
                if (Directory.Exists(_fileSystemStoragePath_DisallowedAbsoluteUris))
                {
                    Directory.Delete(_fileSystemStoragePath_DisallowedAbsoluteUris, true);
                }

                Directory.CreateDirectory(_fileSystemStoragePath_DisallowedAbsoluteUris);

                return;
            }

            throw new NotSupportedException("ExecuteSql is not supported with 'ArachnodeDAOFileSystem'.");
        }

        public DataTable ExecuteSql2(string query)
        {
            throw new NotSupportedException("ExecuteSql2 is not supported with 'ArachnodeDAOFileSystem'.");
        }

        public ArachnodeDataSet.AllowedDataTypesDataTable GetAllowedDataTypes()
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

        public ArachnodeDataSet.AllowedExtensionsDataTable GetAllowedExtensions()
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

        public ArachnodeDataSet.AllowedSchemesDataTable GetAllowedSchemes()
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

        public ArachnodeDataSet.ConfigurationDataTable GetConfiguration()
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

        public ArachnodeDataSet.CrawlActionsDataTable GetCrawlActions()
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

        public ArachnodeDataSet.CrawlRulesDataTable GetCrawlRules()
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

        public ArachnodeDataSet.ContentTypesDataTable GetContentTypes()
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

        public ArachnodeDataSet.CrawlRequestsDataTable GetCrawlRequests(int maximumNumberOfCrawlRequestsToCreatePerBatch, bool createCrawlRequestsFromDatabaseCrawlRequests, bool createCrawlRequestsFromDatabaseFiles, bool assignCrawlRequestPrioritiesForFiles, bool createCrawlRequestsFromDatabaseHyperLinks, bool assignCrawlRequestPrioritiesForHyperLinks, bool createCrawlRequestsFromDatabaseImages, bool assignCrawlRequestPrioritiesForImages, bool createCrawlRequestsFromDatabaseWebPages, bool assignCrawlRequestPrioritiesForWebPages)
        {
            ArachnodeDataSet.CrawlRequestsDataTable crawlRequestsDataTable = new ArachnodeDataSet.CrawlRequestsDataTable();

            try
            {
                _crawlRequestsDataTable.Clear();

                if (_crawlRequestsLock.TryEnterReadLock(-1))
                {
                    foreach (string file in Directory.GetFiles(_fileSystemStoragePath_CrawlRequests, "*.cr", SearchOption.AllDirectories).Take(maximumNumberOfCrawlRequestsToCreatePerBatch))
                    {
                        if (File.Exists(file))
                        {
                            crawlRequestsDataTable.ReadXml(file);
                        }

                        if (crawlRequestsDataTable.Rows.Count != 0)
                        {
                            ArachnodeDataSet.CrawlRequestsRow crawlRequestsRow = (ArachnodeDataSet.CrawlRequestsRow) crawlRequestsDataTable.Rows[0];

                            string absoluteUri0 = null;

                            if (!crawlRequestsRow.IsAbsoluteUri0Null())
                            {
                                absoluteUri0 = crawlRequestsRow.AbsoluteUri0;
                            }

                            _crawlRequestsDataTable.AddCrawlRequestsRow(crawlRequestsRow.Created, absoluteUri0, crawlRequestsRow.AbsoluteUri1, crawlRequestsRow.AbsoluteUri2, crawlRequestsRow.CurrentDepth, crawlRequestsRow.MaximumDepth, crawlRequestsRow.RestrictCrawlTo, crawlRequestsRow.RestrictDiscoveriesTo, crawlRequestsRow.Priority, crawlRequestsRow.RenderType, crawlRequestsRow.RenderTypeForChildren, crawlRequestsRow.DiscoveryTypeID);

                            crawlRequestsDataTable.Clear();
                        }
                    }
                }

                return _crawlRequestsDataTable;
            }
            catch (Exception exception)
            {
                InsertException(null, null, exception, false);
            }
            finally
            {
                if(_crawlRequestsLock.IsReadLockHeld)
                {
                    _crawlRequestsLock.ExitReadLock();
                }
            }

            return new ArachnodeDataSet.CrawlRequestsDataTable();
        }

        public ArachnodeDataSet.DiscoveriesRow GetDiscovery(string absoluteUri)
        {
            try
            {
                string discoveryPath = _discoveryManager.GetDiscoveryPath(_fileSystemStoragePath_Discoveries, absoluteUri, ".d");

                _discoveriesDataTable.Clear();

                if (File.Exists(discoveryPath))
                {
                    if (_discoveriesLock.TryEnterReadLock(-1))
                    {
                        _discoveriesDataTable.ReadXml(discoveryPath);
                    }
                }

                if (_discoveriesDataTable.Count != 0)
                {
                    return (ArachnodeDataSet.DiscoveriesRow) _discoveriesDataTable.Rows[0];
                }

                return null;
            }
            catch (Exception exception)
            {
                InsertException(absoluteUri, null, exception, false);
            }
            finally
            {
                if (_discoveriesLock.IsReadLockHeld)
                {
                    _discoveriesLock.ExitReadLock();
                }    
            }

            return null;
        }

        public ArachnodeDataSet.DiscoveriesDataTable GetDiscoveries(int numberOfDiscoveriesToReturn)
        {
            ArachnodeDataSet.DiscoveriesDataTable discoveriesDataTable = new ArachnodeDataSet.DiscoveriesDataTable();

            try
            {
                _discoveriesDataTable.Clear();

                if (_discoveriesLock.TryEnterReadLock(-1))
                {
                    foreach (string file in Directory.GetFiles(_fileSystemStoragePath_Discoveries, "*.d", SearchOption.AllDirectories).Take(numberOfDiscoveriesToReturn))
                    {
                        discoveriesDataTable.ReadXml(file);

                        ArachnodeDataSet.DiscoveriesRow discoveriesRow = (ArachnodeDataSet.DiscoveriesRow) discoveriesDataTable.Rows[0];

                        _discoveriesDataTable.AddDiscoveriesRow(discoveriesRow.ID, discoveriesRow.AbsoluteUri, discoveriesRow.DiscoveryStateID, discoveriesRow.DiscoveryTypeID, discoveriesRow.ExpectFileOrImage, discoveriesRow.NumberOfTimesDiscovered);

                        discoveriesDataTable.Clear();
                    }
                }

                return _discoveriesDataTable;
            }
            catch (Exception exception)
            {
                InsertException(null, null, exception, false);
            }
            finally
            {
                if (_discoveriesLock.IsReadLockHeld)
                {
                    _discoveriesLock.ExitReadLock();
                }
            }

            return new ArachnodeDataSet.DiscoveriesDataTable();
        }

        public ArachnodeDataSet.DisallowedAbsoluteUrisRow GetDisallowedAbsoluteUri(string disallowedAbsoluteUriAbsoluteUriOrID)
        {
            try
            {
                string discoveryPath = _discoveryManager.GetDiscoveryPath(_fileSystemStoragePath_DisallowedAbsoluteUris, disallowedAbsoluteUriAbsoluteUriOrID, ".dau");

                _disallowedAbsoluteUrisDataTable.Clear();

                if (File.Exists(discoveryPath))
                {
                    _disallowedAbsoluteUrisDataTable.ReadXml(discoveryPath);
                }

                if (_disallowedAbsoluteUrisDataTable.Count != 0)
                {
                    return (ArachnodeDataSet.DisallowedAbsoluteUrisRow)_disallowedAbsoluteUrisDataTable.Rows[0];
                }

                return null;

            }
            catch (Exception exception)
            {
                InsertException(null, null, exception, false);
            }

            return null;
        }

        public ArachnodeDataSet.DisallowedAbsoluteUrisDataTable GetDisallowedAbsoluteUris(int numberOfDisallowedAbsoluteUrisToReturn)
        {
            ArachnodeDataSet.DisallowedAbsoluteUrisDataTable disallowedAbsoluteUrisDataTable = new ArachnodeDataSet.DisallowedAbsoluteUrisDataTable();

            try
            {
                _disallowedAbsoluteUrisDataTable.Clear();

                foreach (string file in Directory.GetFiles(_fileSystemStoragePath, "*.dau", SearchOption.AllDirectories).Take(numberOfDisallowedAbsoluteUrisToReturn))
                {
                    disallowedAbsoluteUrisDataTable.ReadXml(file);

                    _disallowedAbsoluteUrisDataTable.AddDisallowedAbsoluteUrisRow((ArachnodeDataSet.DisallowedAbsoluteUrisRow)disallowedAbsoluteUrisDataTable.Rows[0]);

                    disallowedAbsoluteUrisDataTable.Clear();
                }

                return _disallowedAbsoluteUrisDataTable;
            }
            catch (Exception exception)
            {
                InsertException(null, null, exception, false);
            }

            return new ArachnodeDataSet.DisallowedAbsoluteUrisDataTable();
        }

        public ArachnodeDataSet.DisallowedAbsoluteUrisDiscoveriesDataTable GetDisallowedAbsoluteUriDiscoveries(long disallowedAbsoluteUriID)
        {
            return null;
        }

        public ArachnodeDataSet.DisallowedDomainsDataTable GetDisallowedDomains()
        {
            ArachnodeDataSet.DisallowedDomainsDataTable disallowedDomainsDataTable = new ArachnodeDataSet.DisallowedDomainsDataTable();

            try
            {
                if (File.Exists("cfg.DisallowedDomains.xml"))
                {
                    disallowedDomainsDataTable.ReadXml("cfg.DisallowedDomains.xml");
                }
            }
            catch (Exception exception)
            {
                InsertException(null, null, exception, true);
            }

            return disallowedDomainsDataTable;
        }

        public ArachnodeDataSet.DisallowedExtensionsDataTable GetDisallowedExtensions()
        {
            ArachnodeDataSet.DisallowedExtensionsDataTable disallowedExtensionsDataTable = new ArachnodeDataSet.DisallowedExtensionsDataTable();

            try
            {
                if (File.Exists("cfg.DisallowedExtensions.xml"))
                {
                    disallowedExtensionsDataTable.ReadXml("cfg.DisallowedExtensions.xml");
                }
            }
            catch (Exception exception)
            {
                InsertException(null, null, exception, true);
            }

            return disallowedExtensionsDataTable;
        }

        public ArachnodeDataSet.DisallowedFileExtensionsDataTable GetDisallowedFileExtensions()
        {
            ArachnodeDataSet.DisallowedFileExtensionsDataTable disallowedFileExtensionsDataTable = new ArachnodeDataSet.DisallowedFileExtensionsDataTable();

            try
            {
                if (File.Exists("cfg.DisallowedFileExtensions.xml"))
                {
                    disallowedFileExtensionsDataTable.ReadXml("cfg.DisallowedFileExtensions.xml");
                }
            }
            catch (Exception exception)
            {
                InsertException(null, null, exception, true);
            }

            return disallowedFileExtensionsDataTable;
        }

        public ArachnodeDataSet.DisallowedHostsDataTable GetDisallowedHosts()
        {
            ArachnodeDataSet.DisallowedHostsDataTable disallowedHostsDataTable = new ArachnodeDataSet.DisallowedHostsDataTable();

            try
            {
                if (File.Exists("cfg.DisallowedHosts.xml"))
                {
                    disallowedHostsDataTable.ReadXml("cfg.DisallowedHosts.xml");
                }
            }
            catch (Exception exception)
            {
                InsertException(null, null, exception, true);
            }

            return disallowedHostsDataTable;
        }

        public ArachnodeDataSet.DisallowedSchemesDataTable GetDisallowedSchemes()
        {
            ArachnodeDataSet.DisallowedSchemesDataTable disallowedSchemesDataTable = new ArachnodeDataSet.DisallowedSchemesDataTable();

            try
            {
                if (File.Exists("cfg.DisallowedSchemes.xml"))
                {
                    disallowedSchemesDataTable.ReadXml("cfg.DisallowedSchemes.xml");
                }
            }
            catch (Exception exception)
            {
                InsertException(null, null, exception, true);
            }

            return disallowedSchemesDataTable;
        }

        public ArachnodeDataSet.DisallowedWordsDataTable GetDisallowedWords()
        {
            ArachnodeDataSet.DisallowedWordsDataTable disallowedWordsDataTable = new ArachnodeDataSet.DisallowedWordsDataTable();

            try
            {
                if (File.Exists("cfg.DisallowedWords.xml"))
                {
                    disallowedWordsDataTable.ReadXml("cfg.DisallowedWords.xml");
                }
            }
            catch (Exception exception)
            {
                InsertException(null, null, exception, true);
            }

            return disallowedWordsDataTable;
        }

        public ArachnodeDataSet.DomainsDataTable GetDomains()
        {
            //this feature is not implemented in the ArachnodeDAOFileSystem...  (not yet, anyway...)
            return null;
        }

        public ArachnodeDataSet.EmailAddressesRow GetEmailAddress(string emailAddressAbsoluteUriOrID)
        {
            try
            {
                string discoveryPath = _discoveryManager.GetDiscoveryPath(_fileSystemStoragePath_EmailAddresses, emailAddressAbsoluteUriOrID, ".ea");

                _emailAddressesDataTable.Clear();

                if (File.Exists(discoveryPath))
                {
                    _emailAddressesDataTable.ReadXml(discoveryPath);
                }

                if (_emailAddressesDataTable.Count != 0)
                {
                    return (ArachnodeDataSet.EmailAddressesRow)_emailAddressesDataTable.Rows[0];
                }

                return null;
            }
            catch (Exception exception)
            {
                InsertException(null, null, exception, false);
            }

            return null;
        }

        public ArachnodeDataSet.EmailAddressesDiscoveriesDataTable GetEmailAddressDiscoveries(long emailAddressID)
        {
            return null;
        }

        public ArachnodeDataSet.EngineActionsDataTable GetEngineActions()
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

        public ArachnodeDataSet.ExtensionsDataTable GetExtensions()
        {
            //this feature is not implemented in the ArachnodeDAOFileSystem...  (not yet, anyway...)
            return null;
        }

        public ArachnodeDataSet.FilesRow GetFile(string fileAbsoluteUriOrID)
        {
            try
            {
                string discoveryPath = _discoveryManager.GetDiscoveryPath(_fileSystemStoragePath_Files, fileAbsoluteUriOrID, ".f");

                _filesDataTable.Clear();

                if (_filesLock.TryEnterReadLock(-1))
                {
                    if (File.Exists(discoveryPath))
                    {
                        _filesDataTable.ReadXml(discoveryPath);
                    }
                }

                if (_filesDataTable.Count != 0)
                {
                    return (ArachnodeDataSet.FilesRow) _filesDataTable.Rows[0];
                }

                return null;
            }
            catch (Exception exception)
            {
                InsertException(null, null, exception, false);
            }
            finally
            {
                if(_filesLock.IsReadLockHeld)
                {
                    _filesLock.ExitReadLock();
                }
            }

            return null;
        }

        public ArachnodeDataSet.FilesDiscoveriesDataTable GetFileDiscoveries(long fileID)
        {
            return null;
        }

        public ArachnodeDataSet.HostsDataTable GetHosts()
        {
            //this feature is not implemented in the ArachnodeDAOFileSystem...  (not yet, anyway...)
            return null;
        }

        public ArachnodeDataSet.VersionDataTable GetVersion()
        {
            ArachnodeDataSet.VersionDataTable versionDataTable = new ArachnodeDataSet.VersionDataTable();

            string cfgVersionPath = Path.Combine(_fileSystemStoragePath, "cfg.Version");

            if (File.Exists(cfgVersionPath))
            {
                string allText = File.ReadAllText(cfgVersionPath);

                versionDataTable.AddVersionRow(allText);
            }
            else
            {
                versionDataTable.AddVersionRow(VERSION);
            }

            return versionDataTable;
        }

        public ArachnodeDataSet.FilesMetaDataRow GetFileMetaData(long fileID)
        {
            try
            {
                string discoveryPath = _discoveryManager.GetDiscoveryPath(_fileSystemStoragePath_Files_MetaData, fileID.ToString(), ".fmd");

                _filesMetaDataDataTable.Clear();

                if (File.Exists(discoveryPath))
                {
                    _filesMetaDataDataTable.ReadXml(discoveryPath);
                }

                if (_filesMetaDataDataTable.Count != 0)
                {
                    return (ArachnodeDataSet.FilesMetaDataRow)_filesMetaDataDataTable.Rows[0];
                }

                return null;
            }
            catch (Exception exception)
            {
                InsertException(null, null, exception, false);
            }

            return null;
        }

        public ArachnodeDataSet.HyperLinksRow GetHyperLink(string hyperLinkAbsoluteUriOrID)
        {
            try
            {
                string discoveryPath = _discoveryManager.GetDiscoveryPath(_fileSystemStoragePath_HyperLinks, hyperLinkAbsoluteUriOrID, ".hl");

                _hyperLinksDataTable.Clear();

                if (File.Exists(discoveryPath))
                {
                    _hyperLinksDataTable.ReadXml(discoveryPath);
                }

                if (_hyperLinksDataTable.Count != 0)
                {
                    return (ArachnodeDataSet.HyperLinksRow)_hyperLinksDataTable.Rows[0];
                }

                return null;
            }
            catch (Exception exception)
            {
                InsertException(null, null, exception, false);
            }

            return null;
        }

        public ArachnodeDataSet.HyperLinksDiscoveriesDataTable GetHyperLinkDiscoveries(long hyperLinkID)
        {
            return null;
        }

        public ArachnodeDataSet.ImagesRow GetImage(string imageAbsoluteUriOrID)
        {
            try
            {
                string discoveryPath = _discoveryManager.GetDiscoveryPath(_fileSystemStoragePath_Images, imageAbsoluteUriOrID, ".i");

                _imagesDataTable.Clear();

                if (File.Exists(discoveryPath))
                {
                    _imagesDataTable.ReadXml(discoveryPath);
                }

                if (_imagesDataTable.Count != 0)
                {
                    return (ArachnodeDataSet.ImagesRow)_imagesDataTable.Rows[0];
                }

                return null;
            }
            catch (Exception exception)
            {
                InsertException(null, null, exception, false);
            }

            return null;
        }

        public ArachnodeDataSet.ImagesDiscoveriesDataTable GetImageDiscoveries(long imageID)
        {
            return null;
        }

        public ArachnodeDataSet.ImagesMetaDataRow GetImageMetaData(long imageID)
        {
            try
            {
                string discoveryPath = _discoveryManager.GetDiscoveryPath(_fileSystemStoragePath_Images, imageID.ToString(), ".hl");

                _imagesMetaDataDataTable.Clear();

                if (File.Exists(discoveryPath))
                {
                    _imagesMetaDataDataTable.ReadXml(discoveryPath);
                }

                if (_imagesMetaDataDataTable.Count != 0)
                {
                    return (ArachnodeDataSet.ImagesMetaDataRow)_imagesMetaDataDataTable.Rows[0];
                }

                return null;
            }
            catch (Exception exception)
            {
                InsertException(null, null, exception, false);
            }

            return null;
        }

        public ArachnodeDataSet.PrioritiesDataTable GetPriorities(int maximumNumberOfPriorities)
        {//TODO: Selects the Top 10000 (all) from the cfg.Priorities table...  (the input param should actually work...)
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

        public ArachnodeDataSet.SchemesDataTable GetSchemes()
        {
            //this feature is not implemented in the ArachnodeDAOFileSystem...  (not yet, anyway...)
            return null;
        }

        public ArachnodeDataSet.WebPagesRow GetWebPage(string webPageAbsoluteUriOrID)
        {
            try
            {
                string discoveryPath = _discoveryManager.GetDiscoveryPath(_fileSystemStoragePath_WebPages, webPageAbsoluteUriOrID, ".wp");

                _webPagesDataTable.Clear();

                if (_webPagesLock.TryEnterReadLock(-1))
                {
                    if (File.Exists(discoveryPath))
                    {
                        _webPagesDataTable.ReadXml(discoveryPath);
                    }
                }

                if (_webPagesDataTable.Count != 0)
                {
                    return (ArachnodeDataSet.WebPagesRow)_webPagesDataTable.Rows[0];
                }

                return null;
            }
            catch (Exception exception)
            {
                InsertException(null, null, exception, false);
            }
            finally
            {
                if (_webPagesLock.IsReadLockHeld)
                {
                    _webPagesLock.ExitReadLock();
                }
            }

            return null;
        }

        public long? InsertBusinessInformation(string parentWebPageAbsoluteUri, string webPageAbsoluteUri, long webPageID, string name, string address1, string address2, string city, string state, string zip, string phoneNumber, string category, string latitude, string longitude)
        {
            try
            {
                string discoveryPath = _discoveryManager.GetDiscoveryPath(_fileSystemStoragePath_BusinessInformation, webPageAbsoluteUri, ".bi");

                _businessInformationDataTable.Clear();

                _businessInformationDataTable.AddBusinessInformationRow(webPageID, name, address1, address2, city, state, zip, phoneNumber, category, latitude, longitude);

                if(_businessInformationLock.TryEnterWriteLock(-1))
                {
                    _businessInformationDataTable.WriteXml(discoveryPath);
                }

                //Counters.GetInstance().BusinessInformationInserted();
            }
            catch (Exception exception)
            {
                InsertException(parentWebPageAbsoluteUri, webPageAbsoluteUri, exception, false);
            }
            finally
            {
                if(_businessInformationLock.IsWriteLockHeld)
                {
                    _businessInformationLock.ExitWriteLock();
                }
            }

            return 0;
        }

        public long? InsertCrawlRequest(DateTime created, string absoluteUri0, string absoluteUri1, string absoluteUri2, int depth, short restrictCrawlTo, short restrictDiscoveriesTo, double priority, byte renderType, byte renderTypeForChildren)
        {
            return InsertCrawlRequest(created, new Uri(absoluteUri0).AbsoluteUri, new Uri(absoluteUri1).AbsoluteUri, new Uri(absoluteUri2).AbsoluteUri, 1, depth, restrictCrawlTo, restrictDiscoveriesTo, priority, renderType, renderTypeForChildren);
        }

        public long? InsertCrawlRequest(DateTime created, string absoluteUri0, string absoluteUri1, string absoluteUri2, int currentDepth, int maximumDepth, short restrictCrawlTo, short restrictDiscoveriesTo, double priority, byte renderType, byte renderTypeForChildren)
        {
            try
            {
                string discoveryPath = _discoveryManager.GetDiscoveryPath(_fileSystemStoragePath_CrawlRequests, absoluteUri1, ".cr");

                _crawlRequestsDataTable.Clear();

                _crawlRequestsDataTable.AddCrawlRequestsRow(DateTime.Now, absoluteUri0, absoluteUri1, absoluteUri2, currentDepth, maximumDepth, restrictCrawlTo, restrictDiscoveriesTo, priority, renderType, renderTypeForChildren, 0);

                if(_crawlRequestsLock.TryEnterWriteLock(-1))
                {
                    _crawlRequestsDataTable.WriteXml(discoveryPath);
                }

                Counters.GetInstance().CrawlRequestInserted();

                _lastCrawlRequestID = _hash.HashStringToLong(absoluteUri1);

                return _lastCrawlRequestID;    
            }
            catch (Exception exception)
            {
                InsertException(absoluteUri1, absoluteUri2, exception, false);
            }
            finally
            {
                if (_crawlRequestsLock.IsWriteLockHeld)
                {
                    _crawlRequestsLock.ExitWriteLock();
                }
            }

            return null;
        }

        public void InsertDiscovery(long? discoveryID, string absoluteUri, int discoveryStateID, int discoveryTypeID, bool expectFileOrImage, int numberOfTimesDiscovered)
        {
            try
            {
                string discoveryPath = _discoveryManager.GetDiscoveryPath(_fileSystemStoragePath_Discoveries, absoluteUri, ".d");

                _discoveriesDataTable.Clear();

                if (discoveryID.HasValue)
                {
                    _discoveriesDataTable.AddDiscoveriesRow(discoveryID.Value, absoluteUri, (byte) discoveryStateID, (byte) discoveryTypeID, expectFileOrImage, numberOfTimesDiscovered);
                }
                else
                {
                    discoveryID = _hash.HashStringToLong(absoluteUri);

                    _discoveriesDataTable.AddDiscoveriesRow(discoveryID.Value, absoluteUri, (byte) discoveryStateID, (byte) discoveryTypeID, expectFileOrImage, numberOfTimesDiscovered);
                }

                if (_discoveriesLock.TryEnterWriteLock(-1))
                {
                    _discoveriesDataTable.WriteXml(discoveryPath);
                }
            }
            catch (Exception exception)
            {
                InsertException(absoluteUri, absoluteUri, exception, false);
            }
            finally
            {
                if (_discoveriesLock.IsWriteLockHeld)
                {
                    _discoveriesLock.ExitWriteLock();
                }
            }
        }

        public long? InsertDisallowedAbsoluteUri(int contentTypeID, int discoveryTypeID, string webPageAbsoluteUri, string disallowedAbsoluteUriAbsoluteUri, string reason, bool classifyAbsoluteUris)
        {
            try
            {
                string discoveryPath = _discoveryManager.GetDiscoveryPath(_fileSystemStoragePath_DisallowedAbsoluteUris, disallowedAbsoluteUriAbsoluteUri, ".dau");

                _disallowedAbsoluteUrisDataTable.Clear();

                _disallowedAbsoluteUrisDataTable.AddDisallowedAbsoluteUrisRow(disallowedAbsoluteUriAbsoluteUri);

                if(_disallowedAbsoluteUrisLock.TryEnterWriteLock(-1))
                {
                    _disallowedAbsoluteUrisDataTable.WriteXml(discoveryPath);
                }

                Counters.GetInstance().DisallowedAbsoluteUriInserted();

                _lastDisallowedAbsoluteUriID = _hash.HashStringToLong(disallowedAbsoluteUriAbsoluteUri);

                return _lastDisallowedAbsoluteUriID;
            }
            catch (Exception exception)
            {
                InsertException(webPageAbsoluteUri, disallowedAbsoluteUriAbsoluteUri, exception, false);
            }
            finally
            {
                if(_disallowedAbsoluteUrisLock.IsWriteLockHeld)
                {
                    _disallowedAbsoluteUrisLock.ExitWriteLock();
                }
            }

            return null;
        }

        public void InsertDisallowedAbsoluteUriDiscovery(string webPageAbsoluteUri, string disallowedAbsoluteUriAbsoluteUri)
        {
            try
            {
                string discoveryPath = _discoveryManager.GetDiscoveryPath(_fileSystemStoragePath_DisallowedAbsoluteUris, disallowedAbsoluteUriAbsoluteUri, ".daud");

                _disallowedAbsoluteUrisDiscoveriesDataTable.Clear();

                long webPageID = _hash.HashStringToLong(webPageAbsoluteUri);
                long disallowedAbsoluteUriID = _hash.HashStringToLong(disallowedAbsoluteUriAbsoluteUri);
                //TODO: Last discovered isn't exactly correct...  FileInfo?  Not mission critical...
                _disallowedAbsoluteUrisDiscoveriesDataTable.AddDisallowedAbsoluteUrisDiscoveriesRow(DateTime.Now, DateTime.Now, webPageID, disallowedAbsoluteUriID);

                if(_disallowedAbsoluteUriDiscoveriesLock.TryEnterWriteLock(-1))
                {
                    _disallowedAbsoluteUrisDiscoveriesDataTable.WriteXml(discoveryPath);
                }

                Counters.GetInstance().DisallowedAbsoluteUriDiscoveryInserted();
            }
            catch (Exception exception)
            {
                InsertException(webPageAbsoluteUri, disallowedAbsoluteUriAbsoluteUri, exception, false);
            }
            finally
            {
                if(_disallowedAbsoluteUriDiscoveriesLock.IsWriteLockHeld)
                {
                    _disallowedAbsoluteUriDiscoveriesLock.ExitWriteLock();
                }
            }
        }

        public virtual long? InsertDocument(string parentWebPageAbsoluteUri, string webPageAbsoluteUri, byte documentTypeID, long webPageID, double weight, string field01, string field02, string field03, string field04, string field05, string field06, string field07, string field08, string field09, string field10, string field11, string field12, string field13, string field14, string field15, string field16, string field17, string field18, string field19, string field20, string fullTextIndexType)
        {
            try
            {
                string discoveryPath = _discoveryManager.GetDiscoveryPath(_fileSystemStoragePath_Documents, webPageAbsoluteUri, ".docu");

                _documentsDataTable.Clear();

                _documentsDataTable.AddDocumentsRow(documentTypeID, webPageID, weight, field01, field02, field03, field04, field05, field06, field07, field08, field09, field10, field11, field12, field13, field14, field15, field16, field17, field18, field19, field20, fullTextIndexType);

                if (_documentsLock.TryEnterWriteLock(-1))
                {
                    _documentsDataTable.WriteXml(discoveryPath);
                }

                //Counters.GetInstance().BusinessInformationInserted();
            }
            catch (Exception exception)
            {
                InsertException(parentWebPageAbsoluteUri, webPageAbsoluteUri, exception, false);
            }
            finally
            {
                if (_documentsLock.IsWriteLockHeld)
                {
                    _documentsLock.ExitWriteLock();
                }
            }

            return 0;
        }

        public long? InsertEmailAddress(string webPageAbsoluteUri, string emailAddressAbsoluteUri, bool classifyAbsoluteUris)
        {
            try
            {
                string discoveryPath = _discoveryManager.GetDiscoveryPath(_fileSystemStoragePath_EmailAddresses, emailAddressAbsoluteUri, ".ea");

                _emailAddressesDataTable.Clear();

                _emailAddressesDataTable.AddEmailAddressesRow(emailAddressAbsoluteUri);

                if(_emailAddressesLock.TryEnterWriteLock(-1))
                {
                    _emailAddressesDataTable.WriteXml(discoveryPath);
                }

                Counters.GetInstance().EmailAddressInserted();

                _lastEmailAddressID = _hash.HashStringToLong(emailAddressAbsoluteUri);

                return _lastEmailAddressID;
            }
            catch (Exception exception)
            {
                InsertException(webPageAbsoluteUri, emailAddressAbsoluteUri, exception, false);
            }
            finally
            {
                if(_emailAddressesLock.IsWriteLockHeld)
                {
                    _emailAddressesLock.ExitWriteLock();
                }
            }

            return null;
        }

        public void InsertEmailAddressDiscovery(string webPageAbsoluteUri, string emailAddressAbsoluteUri)
        {
            try
            {
                string discoveryPath = _discoveryManager.GetDiscoveryPath(_fileSystemStoragePath_EmailAddresses_Discoveries, emailAddressAbsoluteUri, ".ead");

                long webPageID = _hash.HashStringToLong(webPageAbsoluteUri);
                long emailAddressID = _hash.HashStringToLong(emailAddressAbsoluteUri);

                _emailAddressesDiscoveriesDataTable.Clear();

                _emailAddressesDiscoveriesDataTable.AddEmailAddressesDiscoveriesRow(DateTime.Now, DateTime.Now, webPageID, emailAddressID);

                if(_emailAddressDiscoveriesLock.TryEnterWriteLock(-1))
                {
                    _emailAddressesDiscoveriesDataTable.WriteXml(discoveryPath);
                }

                Counters.GetInstance().EmailAddressDiscoveryInserted();
            }
            catch (Exception exception)
            {
                InsertException(webPageAbsoluteUri, emailAddressAbsoluteUri, exception, false);
            }
            finally
            {
                if(_emailAddressDiscoveriesLock.IsWriteLockHeld)
                {
                    _emailAddressDiscoveriesLock.ExitWriteLock();
                }
            }
        }

        public long? InsertException(string absoluteUri1, string absoluteUri2, Exception exception, bool insertExceptionInWindowsApplicationLog)
        {            
            return InsertException(absoluteUri1, absoluteUri2, exception.HelpLink, exception.Message, exception.Source, exception.StackTrace, insertExceptionInWindowsApplicationLog);
        }

        public long? InsertException(string absoluteUri1, string absoluteUri2, string helpLink, string message, string source, string stackTrace, bool insertExceptionInWindowsApplicationLog)
        {
            try
            {
                if (ApplicationSettings.EnableConsoleOutput && ApplicationSettings.VerboseOutput)
                {
                    System.Console.ForegroundColor = ConsoleColor.Yellow;

                    System.Console.WriteLine("InsertException: " + message);

                    System.Console.ForegroundColor = ConsoleColor.Gray;
                }

                if (ApplicationSettings.InsertExceptions)
                {
                    string discoveryPath = _discoveryManager.GetDiscoveryPath(_fileSystemStoragePath_Exceptions, absoluteUri2, ".e");

                    _exceptionsDataTable.Clear();

                    _exceptionsDataTable.AddExceptionsRow(DateTime.Now, absoluteUri1,absoluteUri2, helpLink, message, source, stackTrace);

                    if(_exceptionsLock.TryEnterWriteLock(-1))
                    {
                        _exceptionsDataTable.WriteXml(discoveryPath);
                    }

                    Counters.GetInstance().ExceptionInserted();
                }

                if (insertExceptionInWindowsApplicationLog)
                {
                    InsertExceptionIntoWindowsApplicationLog(message, stackTrace);
                }

                Counters.GetInstance().ExceptionInserted();

                _lastExceptionMessage = message;

                _lastExceptionID = _hash.HashStringToLong(absoluteUri2 + message + stackTrace);

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
            finally
            {
                if(_exceptionsLock.IsWriteLockHeld)
                {
                    _exceptionsLock.ExitWriteLock();
                }
            }

            return null;
        }

        /// <summary>
        /// 	Inserts the exception into the Windows 'Application' log.
        /// </summary>
        /// <param name = "exception">The exception.</param>
        public void InsertExceptionIntoWindowsApplicationLog(string message, string stackTrace)
        {
            new EventLog("Application", Environment.MachineName, "arachnode.net").WriteEntry(message + "\n\n" + stackTrace, EventLogEntryType.Error);
        }

        public long? InsertFile(string webPageAbsoluteUri, string fileAbsoluteUri, string responseHeaders, byte[] source, string fullTextIndexType, bool classifyAbsoluteUris)
        {
            try
            {
                string discoveryPath = _discoveryManager.GetDiscoveryPath(_fileSystemStoragePath_Files, fileAbsoluteUri, ".f");

                _filesDataTable.Clear();

                _filesDataTable.AddFilesRow(fileAbsoluteUri, responseHeaders, source, fullTextIndexType);

                if(_filesLock.TryEnterWriteLock(-1))
                {
                    _filesDataTable.WriteXml(discoveryPath);
                }

                Counters.GetInstance().FileInserted();

                _lastFileID = _hash.HashStringToLong(fileAbsoluteUri);

                return _lastFileID;
            }
            catch (Exception exception)
            {
                InsertException(webPageAbsoluteUri, fileAbsoluteUri, exception, false);
            }
            finally
            {
                if (_filesLock.IsWriteLockHeld)
                {
                    _filesLock.ExitWriteLock();
                }
            }


            return null;
        }

        public void InsertFileDiscovery(string webPageAbsoluteUri, string fileAbsoluteUri)
        {
            try
            {
                string discoveryPath = _discoveryManager.GetDiscoveryPath(_fileSystemStoragePath_Files_Discoveries, fileAbsoluteUri, ".fd");

                long webPageID = _hash.HashStringToLong(webPageAbsoluteUri);
                long fileID = _hash.HashStringToLong(fileAbsoluteUri);

                _filesDiscoveriesDataTable.Clear();

                _filesDiscoveriesDataTable.AddFilesDiscoveriesRow(DateTime.Now, DateTime.Now, webPageID, fileID);

                if (_fileDiscoveriesLock.TryEnterWriteLock(-1))
                {
                    _filesDiscoveriesDataTable.WriteXml(discoveryPath);
                }

                Counters.GetInstance().FileDiscoveryInserted();
            }
            catch (Exception exception)
            {
                InsertException(webPageAbsoluteUri, fileAbsoluteUri, exception, false);
            }
            finally
            {
                if (_fileDiscoveriesLock.IsWriteLockHeld)
                {
                    _fileDiscoveriesLock.ExitWriteLock();
                }
            }
        }

        public void InsertFileMetaData(string fileAbsoluteUri, long fileID)
        {
            try
            {
                string discoveryPath = _discoveryManager.GetDiscoveryPath(_fileSystemStoragePath_Files_MetaData, fileAbsoluteUri, ".fmd");

                _filesMetaDataDataTable.Clear();

                _filesMetaDataDataTable.AddFilesMetaDataRow(fileID);

                if (_fileMetaDataLock.TryEnterWriteLock(-1))
                {
                    _filesMetaDataDataTable.WriteXml(discoveryPath);
                }

                Counters.GetInstance().WebPageMetaDataInserted();
            }
            catch (Exception exception)
            {
                InsertException(null, fileAbsoluteUri, exception, false);
            }
            finally
            {
                if (_fileMetaDataLock.IsWriteLockHeld)
                {
                    _fileMetaDataLock.ExitWriteLock();
                }
            }
        }

        public long? InsertHyperLink(string webPageAbsoluteUri, string hyperLinkAbsoluteUri, bool classifyAbsoluteUris)
        {
            try
            {
                string discoveryPath = _discoveryManager.GetDiscoveryPath(_fileSystemStoragePath_HyperLinks, hyperLinkAbsoluteUri, ".hl");
               
                _hyperLinksDataTable.Clear();

                _hyperLinksDataTable.AddHyperLinksRow(hyperLinkAbsoluteUri);

                if(_hyperLinksLock.TryEnterWriteLock(-1))
                {
                    _hyperLinksDataTable.WriteXml(discoveryPath);
                }

                Counters.GetInstance().HyperLinkInserted();

                if(ApplicationSettings.InsertHyperLinkDiscoveries)
                {
                    InsertHyperLinkDiscovery(webPageAbsoluteUri, hyperLinkAbsoluteUri);
                }

                _lastHyperLinkID = _hash.HashStringToLong(hyperLinkAbsoluteUri);

                return _lastHyperLinkID;
            }
            catch (Exception exception)
            {
                InsertException(webPageAbsoluteUri, hyperLinkAbsoluteUri, exception, false);
            }
            finally
            {
                if (_hyperLinksLock.IsWriteLockHeld)
                {
                    _hyperLinksLock.ExitWriteLock();
                }
            }

            return null;
        }

        public void InsertHyperLinkDiscovery(string webPageAbsoluteUri, string hyperLinkAbsoluteUri)
        {
            try
            {
                string discoveryPath = _discoveryManager.GetDiscoveryPath(_fileSystemStoragePath_HyperLinks_Discoveries, hyperLinkAbsoluteUri, ".hld");

                long webPageID = _hash.HashStringToLong(webPageAbsoluteUri);
                long hyperLinkID = _hash.HashStringToLong(hyperLinkAbsoluteUri);

                _hyperLinksDiscoveriesDataTable.Clear();

                _hyperLinksDiscoveriesDataTable.AddHyperLinksDiscoveriesRow(DateTime.Now, DateTime.Now, webPageID, hyperLinkID);

                if(_hyperLinkDiscoveriesLock.TryEnterWriteLock(-1))
                {
                    _hyperLinksDiscoveriesDataTable.WriteXml(discoveryPath);
                }

                Counters.GetInstance().HyperLinkDiscoveryInserted();
            }
            catch (Exception exception)
            {
                InsertException(webPageAbsoluteUri, hyperLinkAbsoluteUri, exception, false);
            }
            finally
            {
                if (_hyperLinkDiscoveriesLock.IsWriteLockHeld)
                {
                    _hyperLinkDiscoveriesLock.ExitWriteLock();
                }
            }
        }

        public long? InsertImage(string webPageAbsoluteUri, string imageAbsoluteUri, string responseHeaders, byte[] source, string fullTextIndexType)
        {
            try
            {
                string discoveryPath = _discoveryManager.GetDiscoveryPath(_fileSystemStoragePath_Images, imageAbsoluteUri, ".i");

                _imagesDataTable.Clear();

                _imagesDataTable.AddImagesRow(imageAbsoluteUri, responseHeaders, source, fullTextIndexType);

                if (_imagesLock.TryEnterWriteLock(-1))
                {
                    _imagesDataTable.WriteXml(discoveryPath);
                }

                Counters.GetInstance().ImageInserted();

                _lastImageID = _hash.HashStringToLong(imageAbsoluteUri);

                return _lastImageID;
            }
            catch (Exception exception)
            {
                InsertException(webPageAbsoluteUri, imageAbsoluteUri, exception, false);
            }
            finally
            {
                if (_imagesLock.IsWriteLockHeld)
                {
                    _imagesLock.ExitWriteLock();
                }
            }

            return null;
        }

        public void InsertImageDiscovery(string webPageAbsoluteUri, string imageAbsoluteUri)
        {
            try
            {
                string discoveryPath = _discoveryManager.GetDiscoveryPath(_fileSystemStoragePath_Images_Discoveries, imageAbsoluteUri, ".id");

                long webPageID = _hash.HashStringToLong(webPageAbsoluteUri);
                long imageID = _hash.HashStringToLong(imageAbsoluteUri);

                _imagesDiscoveriesDataTable.Clear();

                _imagesDiscoveriesDataTable.AddImagesDiscoveriesRow(DateTime.Now, DateTime.Now, webPageID, imageID);

                if(_imageDiscoveriesLock.TryEnterWriteLock(-1))
                {
                    _imagesDiscoveriesDataTable.WriteXml(discoveryPath);
                }

                Counters.GetInstance().ImageDiscoveryInserted();
            }
            catch (Exception exception)
            {
                InsertException(webPageAbsoluteUri, imageAbsoluteUri, exception, false);
            }
            finally
            {
                if (_imageDiscoveriesLock.IsWriteLockHeld)
                {
                    _imageDiscoveriesLock.ExitWriteLock();
                }
            }
        }

        public void InsertImageMetaData(string imageAbsoluteUri, long imageID, string exifData, int flags, int height, double horizontalResolution, double verticalResolution, int width)
        {
            try
            {
                string discoveryPath = _discoveryManager.GetDiscoveryPath(_fileSystemStoragePath_Images_MetaData, imageAbsoluteUri, ".imd");

                _imagesMetaDataDataTable.Clear();

                _imagesMetaDataDataTable.AddImagesMetaDataRow(imageID, exifData, flags, height, horizontalResolution, verticalResolution, width);

                if (_imageMetaDataLock.TryEnterWriteLock(-1))
                {
                    _imagesMetaDataDataTable.WriteXml(discoveryPath);
                }

                Counters.GetInstance().WebPageMetaDataInserted();
            }
            catch (Exception exception)
            {
                InsertException(null, imageAbsoluteUri, exception, false);
            }
            finally
            {
                if (_imageMetaDataLock.IsWriteLockHeld)
                {
                    _imageMetaDataLock.ExitWriteLock();
                }
            }
        }

        public long? InsertWebPage(string webPageAbsoluteUri, string responseHeaders, byte[] source, int codePage, string fullTextIndexType, int crawlDepth, bool classifyAbsoluteUris)
        {
            try
            {
                string discoveryPath = _discoveryManager.GetDiscoveryPath(_fileSystemStoragePath_WebPages, webPageAbsoluteUri, ".wp");

                _webPagesDataTable.Clear();

                _webPagesDataTable.AddWebPagesRow(DateTime.Now, DateTime.Now, DateTime.Now, webPageAbsoluteUri, responseHeaders, source, codePage, fullTextIndexType, crawlDepth);

                if (_webPagesLock.TryEnterWriteLock(-1))
                {
                    _webPagesDataTable.WriteXml(discoveryPath);
                }

                Counters.GetInstance().WebPageInserted();

                _lastWebPageID = _hash.HashStringToLong(webPageAbsoluteUri);

                return _lastWebPageID;
            }
            catch (Exception exception)
            {
                InsertException(webPageAbsoluteUri, webPageAbsoluteUri, exception, false);
            }
            finally
            {
                if (_webPagesLock.IsWriteLockHeld)
                {
                    _webPagesLock.ExitWriteLock();
                }
            }

            return null;
        }

        public void InsertWebPageMetaData(long webPageID, string webPageAbsoluteUri, byte[] text, string xml)
        {
            try
            {
                string discoveryPath = _discoveryManager.GetDiscoveryPath(_fileSystemStoragePath_WebPages_MetaData, webPageAbsoluteUri, ".wpmd");

                _webPagesMetaDataDataTable.Clear();

                _webPagesMetaDataDataTable.AddWebPagesMetaDataRow(webPageID, text, xml, ".htm");

                if (_webPageMetaDataLock.TryEnterWriteLock(-1))
                {
                    _webPagesMetaDataDataTable.WriteXml(discoveryPath);
                }

                Counters.GetInstance().WebPageMetaDataInserted();
            }
            catch (Exception exception)
            {
                InsertException(webPageAbsoluteUri, null, exception, false);
            }
            finally
            {
                if (_webPageMetaDataLock.IsWriteLockHeld)
                {
                    _webPageMetaDataLock.ExitWriteLock();
                }
            }
        }
    }
}