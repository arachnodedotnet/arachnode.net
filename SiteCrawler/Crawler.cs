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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.Remoting;
using System.Threading;
using System.Web.Caching;
using Arachnode.Configuration;
using Arachnode.Configuration.Value.Enums;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Managers;
using Arachnode.DataAccess.Value.Enums;
using Arachnode.DataAccess.Value.Exceptions;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.DataSource;
using Arachnode.Performance;
using Arachnode.SiteCrawler.Core;
using Arachnode.SiteCrawler.Managers;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.AbstractClasses;
using Arachnode.SiteCrawler.Value.Enums;

#endregion

namespace Arachnode.SiteCrawler
{
    /// <summary>
    /// </summary>
    public class Crawler<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly object _lock = new object();

        private ApplicationSettings _applicationSettings;
        private WebSettings _webSettings;
    
        private Cache<TArachnodeDAO> _cache;

        private ActionManager<TArachnodeDAO> _actionManager;
        private CacheManager<TArachnodeDAO> _cacheManager;
        private ConsoleManager<TArachnodeDAO> _consoleManager;
        private CookieManager _cookieManager;
        private CrawlerPeerManager<TArachnodeDAO> _crawlerPeerManager;
        private CrawlRequestManager<TArachnodeDAO> _crawlRequestManager;
        private DatabasePeerManager<TArachnodeDAO> _databasePeerManager;
        private DataTypeManager<TArachnodeDAO> _dataTypeManager;
        private DiscoveryManager<TArachnodeDAO> _discoveryManager;
        private EncodingManager<TArachnodeDAO> _encodingManager;
        private HtmlManager<TArachnodeDAO> _htmlManager;
        private MemoryManager<TArachnodeDAO> _memoryManager;
        private PolitenessManager<TArachnodeDAO> _politenessManager;
        private ProxyManager<TArachnodeDAO> _proxyManager;
        private ReportingManager<TArachnodeDAO> _reportingManager;
        private RuleManager<TArachnodeDAO> _ruleManager;

        private List<CrawlerPeer> _crawlerPeers;
        private List<DatabasePeer> _databasePeers;

        private readonly TArachnodeDAO _arachnodeDAO;

        private int _numberOfFilesCrawled;
        public int NumberOfFilesCrawled
        {
            get
            {
                return _numberOfFilesCrawled;
            }
            internal set
            {
                Interlocked.Increment(ref _numberOfFilesCrawled);
            }
        }

        private int _numberOfImagesCrawled;
        public int NumberOfImagesCrawled
        {
            get
            {
                return _numberOfImagesCrawled;
            }
            internal set
            {
                Interlocked.Increment(ref _numberOfImagesCrawled);
            }
        }

        private int _numberOfWebPagesCrawled;
        public int NumberOfWebPagesCrawled
        {
            get
            {
                return _numberOfWebPagesCrawled;
            }
            internal set
            {
                Interlocked.Increment(ref _numberOfWebPagesCrawled);
            }
        }

        /// <summary>
        /// 	Initializes a new instance of the <see cref = "Crawler" /> class.
        /// </summary>
        public Crawler(ApplicationSettings applicationSettings, WebSettings webSettings, CrawlMode crawlMode, List<CrawlerPeer> crawlerPeers, List<DatabasePeer> databasePeers, bool enableRenderers)
        {
            Guid = Guid.NewGuid();

            try
            {
                _applicationSettings = applicationSettings;
                _webSettings = webSettings;

                _arachnodeDAO = (TArachnodeDAO)Activator.CreateInstance(typeof(TArachnodeDAO), _applicationSettings.ConnectionString, _applicationSettings, _webSettings, true, true);
                _applicationSettings = _arachnodeDAO.ApplicationSettings;

                _consoleManager = new ConsoleManager<TArachnodeDAO>(_applicationSettings, _webSettings);

                _consoleManager.OutputString("arachnode.net " + Assembly.GetExecutingAssembly().GetName().Version, ConsoleColor.Green, ConsoleColor.Gray);

                _actionManager = new ActionManager<TArachnodeDAO>(_applicationSettings, _webSettings, _consoleManager);
                _ruleManager = new RuleManager<TArachnodeDAO>(_applicationSettings, _webSettings, _consoleManager);

                _memoryManager = new MemoryManager<TArachnodeDAO>(_applicationSettings, _webSettings);
                _cacheManager = new CacheManager<TArachnodeDAO>(_applicationSettings, _webSettings);

                _cookieManager = new CookieManager();
                _cacheManager = new CacheManager<TArachnodeDAO>(_applicationSettings, _webSettings);

                CrawlerPeers = crawlerPeers;
                DatabasePeers = databasePeers;

                _crawlerPeerManager = new CrawlerPeerManager<TArachnodeDAO>(_applicationSettings, _webSettings, CrawlerPeers, (TArachnodeDAO)Activator.CreateInstance(typeof(TArachnodeDAO), _applicationSettings.ConnectionString, _applicationSettings, _webSettings, true, true));
                _databasePeerManager = new DatabasePeerManager<TArachnodeDAO>(_applicationSettings, _webSettings, DatabasePeers);

                _cache = new Cache<TArachnodeDAO>(_applicationSettings, _webSettings, this, _actionManager, _cacheManager, _crawlerPeerManager, _memoryManager, _ruleManager);

                _dataTypeManager = new DataTypeManager<TArachnodeDAO>(_applicationSettings, _webSettings);
                _discoveryManager = new DiscoveryManager<TArachnodeDAO>(_applicationSettings, _webSettings, _cache, _actionManager, _cacheManager, _memoryManager, _ruleManager);
                _crawlRequestManager = new CrawlRequestManager<TArachnodeDAO>(_applicationSettings, _webSettings, _cache, _consoleManager, _discoveryManager);
                _encodingManager = new EncodingManager<TArachnodeDAO>(_applicationSettings, _webSettings);
                _htmlManager = new HtmlManager<TArachnodeDAO>(_applicationSettings, _webSettings, _discoveryManager);
                _politenessManager = new PolitenessManager<TArachnodeDAO>(_applicationSettings, _webSettings, _cache);
                _proxyManager = new ProxyManager<TArachnodeDAO>(_applicationSettings, _webSettings, _consoleManager);
                _reportingManager = new ReportingManager<TArachnodeDAO>(_applicationSettings, _webSettings, _consoleManager);

                //create required directories...
                if (!Directory.Exists(_applicationSettings.ConsoleOutputLogsDirectory))
                {
                    Directory.CreateDirectory(_applicationSettings.ConsoleOutputLogsDirectory);
                }

                if (!Directory.Exists(_applicationSettings.DownloadedFilesDirectory))
                {
                    Directory.CreateDirectory(_applicationSettings.DownloadedFilesDirectory);
                }

                if (!Directory.Exists(_applicationSettings.DownloadedImagesDirectory))
                {
                    Directory.CreateDirectory(_applicationSettings.DownloadedImagesDirectory);
                }

                if (!Directory.Exists(_applicationSettings.DownloadedWebPagesDirectory))
                {
                    Directory.CreateDirectory(_applicationSettings.DownloadedWebPagesDirectory);
                }

                QueryProcessor = new QueryProcessor<TArachnodeDAO>();

                _consoleManager.OutputString("Crawler: Initializing Configuration/Database Connection.", ConsoleColor.White, ConsoleColor.Gray);

                LoadCrawlActions(_arachnodeDAO);
                LoadCrawlRules(_arachnodeDAO);

                AreRenderersEnabled = enableRenderers;

                Engine = new Engine<TArachnodeDAO>(_applicationSettings, _webSettings, this, _cache, _actionManager, _cacheManager, _consoleManager, _cookieManager, _crawlRequestManager, _dataTypeManager, _discoveryManager, _encodingManager, _htmlManager, _memoryManager, _politenessManager, _proxyManager, _reportingManager, _ruleManager, enableRenderers, (TArachnodeDAO)Activator.CreateInstance(typeof(TArachnodeDAO), _applicationSettings.ConnectionString, _applicationSettings, _webSettings, true, true));

                CrawlMode = crawlMode;

                /**/

                if (CrawlerPeerManager != null && CrawlerPeerManager.CrawlerPeers != null && CrawlerPeerManager.CrawlerPeers.Count != 0)
                {
                    ConsoleManager.OutputString("Crawler: Starting CrawlerPeerManager Server", ConsoleColor.White, ConsoleColor.Gray);

                    CrawlerPeerManager.StartServer(this, _arachnodeDAO);

                    _crawlerPeerManager.SendStatusMessageToCrawlerPeers(_arachnodeDAO);
                }

                /**/

                if (Debugger.IsAttached)
                {
                    _consoleManager.OutputString("Debugger: Attached - Expect Performance Degradation.", ConsoleColor.Yellow, ConsoleColor.Gray);
                }

                //update all core/components/managers with the updated ApplicationSettings...
#if DEMO
                Engine.CrawlRequestCompleted += Engine_CrawlRequestCompleted;

                _stopwatch.Start();
#endif
            }
            catch (InvalidConfigurationException invalidConfigurationException)
            {
                ProcessException(invalidConfigurationException);

                throw new InvalidConfigurationException(invalidConfigurationException.ApplicationSettings, invalidConfigurationException.WebSettings, invalidConfigurationException.Message, InvalidConfigurationExceptionSeverity.Error);
            }
            catch (Exception exception)
            {
                ProcessException(exception);

                throw new Exception(exception.Message, exception);
            }
        }

        public Crawler(ApplicationSettings applicationSettings, WebSettings webSettings, CrawlMode crawlMode, bool enableRenderers)
            : this(applicationSettings, webSettings, crawlMode, null, null, null, enableRenderers)
        {
        }

        public Crawler(ApplicationSettings applicationSettings, WebSettings webSettings, CrawlMode crawlMode, CredentialCache credentialCache, bool enableRenderers)
            : this(applicationSettings, webSettings, crawlMode, enableRenderers)
        {
            CredentialCache = credentialCache;
        }

        public Crawler(ApplicationSettings applicationSettings, WebSettings webSettings, CrawlMode crawlMode, CredentialCache credentialCache, List<CrawlerPeer> crawlerPeers, List<DatabasePeer> databasePeers, bool enableRenderers)
            : this(applicationSettings, webSettings, crawlMode, crawlerPeers, databasePeers, enableRenderers)
        {
            CredentialCache = credentialCache;
        }

        public Crawler(ApplicationSettings applicationSettings, WebSettings webSettings, CrawlMode crawlMode, CookieContainer cookieContainer, CredentialCache credentialCache, bool enableRenderers)
            : this(applicationSettings, webSettings, crawlMode, enableRenderers)
        {
            CookieContainer = cookieContainer;
            CredentialCache = credentialCache;
        }

        public Crawler(ApplicationSettings applicationSettings, WebSettings webSettings, CrawlMode crawlMode, CookieContainer cookieContainer, CredentialCache credentialCache, List<CrawlerPeer> crawlerPeers, List<DatabasePeer> databasePeers, bool enableRenderers)
            : this(applicationSettings, webSettings, crawlMode, crawlerPeers, databasePeers, enableRenderers)
        {
            CookieContainer = cookieContainer;
            CredentialCache = credentialCache;
        }

        public Dictionary<string, ACrawlAction<TArachnodeDAO>> CrawlActions { get; set; }
        public Dictionary<string, ACrawlRule<TArachnodeDAO>> CrawlRules { get; set; }

        public bool AreRenderersEnabled { get; set; }

        public Guid Guid { get; private set; }

        /// <summary>
        /// 	The QueryProcessor.  (reserved for future use)
        /// </summary>
        /// <value>The query processor.</value>
        public QueryProcessor<TArachnodeDAO> QueryProcessor { get; private set; }

        /// <summary>
        /// 	The Engine.
        /// </summary>
        /// <value>The engine.</value>
        public Engine<TArachnodeDAO> Engine { get; private set; }

        /// <summary>
        /// 	Gets or sets the credential cache.
        /// </summary>
        /// <value>The credential cache.</value>
        public CredentialCache CredentialCache { get; set; }

        /// <summary>
        /// 	Gets or sets the cookie container.
        /// </summary>
        /// <value>The cookie container.</value>
        public CookieContainer CookieContainer { get; set; }

        public IWebProxy Proxy { get; set; }

        public CrawlMode CrawlMode { get; internal set; }

        public ApplicationSettings ApplicationSettings
        {
            get { return _applicationSettings; }
            set
            {                
                _applicationSettings = value;

                //set each class' type value...
                foreach (MemberInfo memberInfo in GetType().GetMembers())
                {
                    foreach (PropertyInfo propertyInfo2 in memberInfo.ReflectedType.GetProperties())
                    {
                        if (propertyInfo2.Name.Contains("_get") || propertyInfo2.Name.Contains("_set"))
                        {
                            //TODO: Figure this out!  Make 'ApplicationSettings' dynamic...
                            if (propertyInfo2.PropertyType == typeof (ApplicationSettings))
                            {
                                propertyInfo2.SetValue(this, value, null);
                            }
                        }
                    }
                }
            }
        }

        public WebSettings WebSettings
        {
            get { return _webSettings; }
            set
            {
                _webSettings = value;

                //set each class' type value...
                foreach (MemberInfo memberInfo in GetType().GetMembers())
                {
                    foreach (PropertyInfo propertyInfo2 in memberInfo.ReflectedType.GetProperties())
                    {
                        if (propertyInfo2.Name.Contains("_get") || propertyInfo2.Name.Contains("_set"))
                        {
                            //TODO: Figure this out!  Make 'WebSettings' dynamic...
                            if (propertyInfo2.PropertyType == typeof(WebSettings))
                            {
                                propertyInfo2.SetValue(this, value, null);
                            }
                        }
                    }
                }
            }
        }

        public Cache<TArachnodeDAO> Cache
        {
            get { return _cache; }
            set
            {
                _cache = value;

                //set each class' type value...
                foreach (MemberInfo memberInfo in GetType().GetMembers())
                {
                    foreach (PropertyInfo propertyInfo2 in memberInfo.ReflectedType.GetProperties())
                    {
                        if (propertyInfo2.Name.Contains("_get") || propertyInfo2.Name.Contains("_set"))
                        {
                            //TODO: Figure this out!  Make 'ApplicationSettings' dynamic...
                            if (propertyInfo2.PropertyType == typeof (Cache<TArachnodeDAO>))
                            {
                                propertyInfo2.SetValue(this, value, null);
                            }
                        }
                    }
                }
            }
        }

        public ActionManager<TArachnodeDAO> ActionManager
        {
            get { return _actionManager; }
            set
            {
                _actionManager = value;

                //set each class' type value...
                foreach (MemberInfo memberInfo in GetType().GetMembers())
                {
                    foreach (PropertyInfo propertyInfo2 in memberInfo.ReflectedType.GetProperties())
                    {
                        if (propertyInfo2.Name.Contains("_get") || propertyInfo2.Name.Contains("_set"))
                        {
                            //TODO: Figure this out!  Make 'ApplicationSettings' dynamic...
                            if (propertyInfo2.PropertyType == typeof (AActionManager<TArachnodeDAO>))
                            {
                                propertyInfo2.SetValue(this, value, null);
                            }
                        }
                    }
                }
            }
        }

        public CacheManager<TArachnodeDAO> CacheManager
        {
            get { return _cacheManager; }
            set
            {
                _cacheManager = value;

                //set each class' type value...
                foreach (MemberInfo memberInfo in GetType().GetMembers())
                {
                    foreach (PropertyInfo propertyInfo2 in memberInfo.ReflectedType.GetProperties())
                    {
                        if (propertyInfo2.Name.Contains("_get") || propertyInfo2.Name.Contains("_set"))
                        {
                            //TODO: Figure this out!  Make 'ApplicationSettings' dynamic...
                            if (propertyInfo2.PropertyType == typeof (CacheManager<TArachnodeDAO>))
                            {
                                propertyInfo2.SetValue(this, value, null);
                            }
                        }
                    }
                }
            }
        }

        public ConsoleManager<TArachnodeDAO> ConsoleManager
        {
            get { return _consoleManager; }
            set
            {
                _consoleManager = value;

                //set each class' type value...
                foreach (MemberInfo memberInfo in GetType().GetMembers())
                {
                    foreach (PropertyInfo propertyInfo2 in memberInfo.ReflectedType.GetProperties())
                    {
                        if (propertyInfo2.Name.Contains("_get") || propertyInfo2.Name.Contains("_set"))
                        {
                            //TODO: Figure this out!  Make 'ApplicationSettings' dynamic...
                            if (propertyInfo2.PropertyType == typeof(ConsoleManager<TArachnodeDAO>))
                            {
                                propertyInfo2.SetValue(this, value, null);
                            }
                        }
                    }
                }
            }
        }

        public CookieManager CookieManager
        {
            get { return _cookieManager; }
            set
            {
                _cookieManager = value;

                //set each class' type value...
                foreach (MemberInfo memberInfo in GetType().GetMembers())
                {
                    foreach (PropertyInfo propertyInfo2 in memberInfo.ReflectedType.GetProperties())
                    {
                        if (propertyInfo2.Name.Contains("_get") || propertyInfo2.Name.Contains("_set"))
                        {
                            //TODO: Figure this out!  Make 'ApplicationSettings' dynamic...
                            if (propertyInfo2.PropertyType == typeof(CookieManager))
                            {
                                propertyInfo2.SetValue(this, value, null);
                            }
                        }
                    }
                }
            }
        }

        public CrawlerPeerManager<TArachnodeDAO> CrawlerPeerManager
        {
            get { return _crawlerPeerManager; }
            set
            {
                _crawlerPeerManager = value;

                //set each class' type value...
                foreach (MemberInfo memberInfo in GetType().GetMembers())
                {
                    foreach (PropertyInfo propertyInfo2 in memberInfo.ReflectedType.GetProperties())
                    {
                        if (propertyInfo2.Name.Contains("_get") || propertyInfo2.Name.Contains("_set"))
                        {
                            //TODO: Figure this out!  Make 'ApplicationSettings' dynamic...
                            if (propertyInfo2.PropertyType == typeof (CrawlerPeerManager<TArachnodeDAO>))
                            {
                                propertyInfo2.SetValue(this, value, null);
                            }
                        }
                    }
                }
            }
        }

        public CrawlRequestManager<TArachnodeDAO> CrawlRequestManager
        {
            get { return _crawlRequestManager; }
            set
            {
                _crawlRequestManager = value;

                //set each class' type value...
                foreach (MemberInfo memberInfo in GetType().GetMembers())
                {
                    foreach (PropertyInfo propertyInfo2 in memberInfo.ReflectedType.GetProperties())
                    {
                        if (propertyInfo2.Name.Contains("_get") || propertyInfo2.Name.Contains("_set"))
                        {
                            //TODO: Figure this out!  Make 'ApplicationSettings' dynamic...
                            if (propertyInfo2.PropertyType == typeof(ACrawlRequestManager<TArachnodeDAO>))
                            {
                                propertyInfo2.SetValue(this, value, null);
                            }
                        }
                    }
                }
            }
        }

        public DatabasePeerManager<TArachnodeDAO> DatabasePeerManager
        {
            get { return _databasePeerManager; }
            set
            {
                _databasePeerManager = value;

                //set each class' type value...
                foreach (MemberInfo memberInfo in GetType().GetMembers())
                {
                    foreach (PropertyInfo propertyInfo2 in memberInfo.ReflectedType.GetProperties())
                    {
                        if (propertyInfo2.Name.Contains("_get") || propertyInfo2.Name.Contains("_set"))
                        {
                            //TODO: Figure this out!  Make 'ApplicationSettings' dynamic...
                            if (propertyInfo2.PropertyType == typeof(ADatabasePeerManager<TArachnodeDAO>))
                            {
                                propertyInfo2.SetValue(this, value, null);
                            }
                        }
                    }
                }
            }
        }

        public DataTypeManager<TArachnodeDAO> DataTypeManager
        {
            get { return _dataTypeManager; }
            set
            {
                _dataTypeManager = value;

                //set each class' type value...
                foreach (MemberInfo memberInfo in GetType().GetMembers())
                {
                    foreach (PropertyInfo propertyInfo2 in memberInfo.ReflectedType.GetProperties())
                    {
                        if (propertyInfo2.Name.Contains("_get") || propertyInfo2.Name.Contains("_set"))
                        {
                            //TODO: Figure this out!  Make 'ApplicationSettings' dynamic...
                            if (propertyInfo2.PropertyType == typeof (DataTypeManager<TArachnodeDAO>))
                            {
                                propertyInfo2.SetValue(this, value, null);
                            }
                        }
                    }
                }
            }
        }

        public DiscoveryManager<TArachnodeDAO> DiscoveryManager
        {
            get { return _discoveryManager; }
            set
            {
                _discoveryManager = value;

                //set each class' type value...
                foreach (MemberInfo memberInfo in GetType().GetMembers())
                {
                    foreach (PropertyInfo propertyInfo2 in memberInfo.ReflectedType.GetProperties())
                    {
                        if (propertyInfo2.Name.Contains("_get") || propertyInfo2.Name.Contains("_set"))
                        {
                            //TODO: Figure this out!  Make 'ApplicationSettings' dynamic...
                            if (propertyInfo2.PropertyType == typeof(ADiscoveryManager<TArachnodeDAO>))
                            {
                                propertyInfo2.SetValue(this, value, null);
                            }
                        }
                    }
                }
            }
        }

        public EncodingManager<TArachnodeDAO> EncodingManager
        {
            get { return _encodingManager; }
            set
            {
                _encodingManager = value;

                //set each class' type value...
                foreach (MemberInfo memberInfo in GetType().GetMembers())
                {
                    foreach (PropertyInfo propertyInfo2 in memberInfo.ReflectedType.GetProperties())
                    {
                        if (propertyInfo2.Name.Contains("_get") || propertyInfo2.Name.Contains("_set"))
                        {
                            //TODO: Figure this out!  Make 'ApplicationSettings' dynamic...
                            if (propertyInfo2.PropertyType == typeof(AEncodingManager<TArachnodeDAO>))
                            {
                                propertyInfo2.SetValue(this, value, null);
                            }
                        }
                    }
                }
            }
        }

        public HtmlManager<TArachnodeDAO> HtmlManager
        {
            get { return _htmlManager; }
            set
            {                
                _htmlManager = value;

                //set each class' type value...
                foreach (MemberInfo memberInfo in GetType().GetMembers())
                {
                    foreach (PropertyInfo propertyInfo2 in memberInfo.ReflectedType.GetProperties())
                    {
                        if (propertyInfo2.Name.Contains("_get") || propertyInfo2.Name.Contains("_set"))
                        {
                            //TODO: Figure this out!  Make 'ApplicationSettings' dynamic...
                            if (propertyInfo2.PropertyType == typeof(AHtmlManager<TArachnodeDAO>))
                            {
                                propertyInfo2.SetValue(this, value, null);
                            }
                        }
                    }
                }
            }
        }

        public MemoryManager<TArachnodeDAO> MemoryManager
        {
            get { return _memoryManager; }
            set
            {
                _memoryManager = value;

                //set each class' type value...
                foreach (MemberInfo memberInfo in GetType().GetMembers())
                {
                    foreach (PropertyInfo propertyInfo2 in memberInfo.ReflectedType.GetProperties())
                    {
                        if (propertyInfo2.Name.Contains("_get") || propertyInfo2.Name.Contains("_set"))
                        {
                            //TODO: Figure this out!  Make 'ApplicationSettings' dynamic...
                            if (propertyInfo2.PropertyType == typeof(AMemoryManager<TArachnodeDAO>))
                            {
                                propertyInfo2.SetValue(this, value, null);
                            }
                        }
                    }
                }
            }
        }

        public PolitenessManager<TArachnodeDAO> PolitenessManager
        {
            get { return _politenessManager; }
            set
            {
                _politenessManager = value;

                //set each class' type value...
                foreach (MemberInfo memberInfo in GetType().GetMembers())
                {
                    foreach (PropertyInfo propertyInfo2 in memberInfo.ReflectedType.GetProperties())
                    {
                        if (propertyInfo2.Name.Contains("_get") || propertyInfo2.Name.Contains("_set"))
                        {
                            //TODO: Figure this out!  Make 'ApplicationSettings' dynamic...
                            if (propertyInfo2.PropertyType == typeof(APolitenessManager<TArachnodeDAO>))
                            {
                                propertyInfo2.SetValue(this, value, null);
                            }
                        }
                    }
                }
            }
        }

        public ProxyManager<TArachnodeDAO> ProxyManager
        {
            get { return _proxyManager; }
            set
            {
                _proxyManager = value;

                //set each class' type value...
                foreach (MemberInfo memberInfo in GetType().GetMembers())
                {
                    foreach (PropertyInfo propertyInfo2 in memberInfo.ReflectedType.GetProperties())
                    {
                        if (propertyInfo2.Name.Contains("_get") || propertyInfo2.Name.Contains("_set"))
                        {
                            //TODO: Figure this out!  Make 'ApplicationSettings' dynamic...
                            if (propertyInfo2.PropertyType == typeof(AProxyManager<TArachnodeDAO>))
                            {
                                propertyInfo2.SetValue(this, value, null);
                            }
                        }
                    }
                }
            }
        }

        public RuleManager<TArachnodeDAO> RuleManager
        {
            get { return _ruleManager; }
            set
            {
                _ruleManager = value;

                //set each class' type value...
                foreach (MemberInfo memberInfo in GetType().GetMembers())
                {
                    foreach (PropertyInfo propertyInfo2 in memberInfo.ReflectedType.GetProperties())
                    {
                        //TODO: Figure this out!  Make 'ApplicationSettings' dynamic...
                        if (propertyInfo2.PropertyType == typeof(ProxyManager<TArachnodeDAO>))
                        {
                            propertyInfo2.SetValue(this, value, null);
                        }
                    }
                }
            }
        }

        public ReportingManager<TArachnodeDAO> ReportingManager
        {
            get { return _reportingManager; }
            set
            {
                _reportingManager = value;

                //set each class' type value...
                foreach (MemberInfo memberInfo in GetType().GetMembers())
                {
                    foreach (PropertyInfo propertyInfo2 in memberInfo.ReflectedType.GetProperties())
                    {
                        if (propertyInfo2.Name.Contains("_get") || propertyInfo2.Name.Contains("_set"))
                        {
                            //TODO: Figure this out!  Make 'ApplicationSettings' dynamic...
                            if (propertyInfo2.PropertyType == typeof(AReportingManager<TArachnodeDAO>))
                            {
                                propertyInfo2.SetValue(this, value, null);
                            }
                        }
                    }
                }
            }
        }

        public List<CrawlerPeer> CrawlerPeers
        {
            get { return _crawlerPeers; }
            set { _crawlerPeers = value; }
        }

        public List<DatabasePeer> DatabasePeers
        {
            get { return _databasePeers; }
            set { _databasePeers = value; }
        }

        private void Engine_CrawlRequestCompleted(CrawlRequest<TArachnodeDAO> sender)
        {
            if (_stopwatch.Elapsed.TotalMinutes >= 2)
            {
                Thread thread = new Thread(EngineStopThread);

                thread.Start(Engine);
            }
        }

        private void EngineStopThread(object o)
        {
            ((Engine<TArachnodeDAO>)o).Stop();

            _consoleManager.OutputString("Crawl time is limited to 2 minutes in DEMO.", ConsoleColor.White, ConsoleColor.White);
        }

        /// <summary>
        /// 	Reinitializes the configuration.  Refreshes the Crawler configuration without having to create a new Crawler.
        /// </summary>
        /// <param name = "initializeApplicationConfiguration"></param>
        /// <param name = "initializeWebConfiguration"></param>
        public void ReinitializeConfiguration(bool initializeApplicationConfiguration, bool initializeWebConfiguration)
        {
            if (initializeApplicationConfiguration)
            {
                ConfigurationManager.InitializeConfiguration(ref _applicationSettings, ref _webSettings, ConfigurationType.Application, _arachnodeDAO);
            }

            if (initializeWebConfiguration)
            {
                ConfigurationManager.InitializeConfiguration(ref _applicationSettings, ref _webSettings, ConfigurationType.Web, _arachnodeDAO);
            }
        }

        /// <summary>
        /// 	Clears all DisallowedAbsoluteUris on disk.
        /// </summary>
        public void ClearDisallowedAbsoluteUris()
        {
            //ANODET: Remove the dynamic SQL.
            _arachnodeDAO.ExecuteSql("DELETE FROM dbo.DisallowedAbsoluteUris");
        }

        /// <summary>
        /// 	Clears the Discoveries in RAM and on disk.  This method is not thread-safe and should not be called while the Engine is running.
        /// </summary>
        public void ClearDiscoveries()
        {
            _cache.ClearDiscoveries();

            _arachnodeDAO.DeleteDiscoveries();
        }

        /// <summary>
        /// 	Clears the UncrawledCrawlRequests in RAM and on disk.  This method is not thread-safe and should not be called while the Engine is running.
        /// </summary>
        public void ClearUncrawledCrawlRequests()
        {
            while (_cache.UncrawledCrawlRequests.Dequeue() != null)
            {
                Counters.GetInstance().CrawlRequestRemoved();
            }

            _arachnodeDAO.DeleteCrawlRequest(null, null);
        }

        /// <summary>
        /// 	Clears the Politenesses in RAM.  This method is not thread-safe and should not be called while the Engine is running.
        /// </summary>
        public void ClearPolitenesses()
        {
            _cache.Politenesses.Clear();
        }

        /// <summary>
        /// 	Submits a CrawlRequest to the Engine for crawling.
        /// 	If Crawl(CrawlRequest crawlRequest) returns 'false' the Cache is already aware of the Discovery and the Discovery is awaiting processing or has already been processed and was loaded from the database.
        /// 	If a Crawl is resuming, the Discoveries found in the database table 'Discoveries' will be loaded into RAM, allowing arachnode.net to resume the memory state of the crawl.
        /// 	To re-crawl an AbsoluteUri that was already discovered, delete the row from database table 'Discoveries'.  To clear all Discoveries, call 'crawler.ClearDiscoveries'.
        /// </summary>
        /// <param name = "crawlRequest">The CrawlRequest to be crawled.</param>
        public bool Crawl(CrawlRequest<TArachnodeDAO> crawlRequest)
        {
            bool wasTheCrawlRequestAddedForCrawling = false;

            if (crawlRequest != null)
            {
                if (QueryProcessor == null || Engine == null)
                {
                    Exception exception = new Exception("There was a problem instantiating the Crawler.");

                    ProcessException(exception);

                    return false;
                }

                QueryProcessor.ProcessQuery(crawlRequest);

                wasTheCrawlRequestAddedForCrawling = Engine.Crawl(crawlRequest);
            }

            ConsoleColor foregroundColor = Console.ForegroundColor;

            if(wasTheCrawlRequestAddedForCrawling)
            {
                _consoleManager.OutputString("Crawler: Crawl: " + crawlRequest + " WasTheCrawlRequestAddedForCrawling: " + wasTheCrawlRequestAddedForCrawling, ConsoleColor.Green, foregroundColor);
            }
            else
            {
                _consoleManager.OutputString("Crawler: Crawl: " + crawlRequest + " WasTheCrawlRequestAddedForCrawling: " + wasTheCrawlRequestAddedForCrawling, ConsoleColor.Yellow, foregroundColor);
            }

            return wasTheCrawlRequestAddedForCrawling;
        }

        /// <summary>
        /// 	Throws the exception.
        /// </summary>
        /// <param name = "exception">The exception.</param>
        internal void ProcessException(object exception)
        {
            if(exception == null)
            {
                return;
            }

            ApplicationSettings.EnableConsoleOutput = true;

            if(_consoleManager == null)
            {
                lock(_lock)
                {
                    if (_consoleManager == null)
                    {
                        _consoleManager = new ConsoleManager<TArachnodeDAO>(_applicationSettings, _webSettings);
                    }
                }
            }

            if (exception is InvalidConfigurationException)
            {
                _consoleManager.OutputString(((InvalidConfigurationException)exception).Message, ConsoleColor.Yellow, ConsoleColor.White);
            }
            else
            {
                _consoleManager.OutputString(((Exception)exception).Message, ConsoleColor.Yellow, ConsoleColor.White);
                _consoleManager.OutputString(((Exception)exception).StackTrace, ConsoleColor.Yellow, ConsoleColor.White);

                if (((Exception) exception).InnerException != null)
                {
                    _consoleManager.OutputString(Environment.NewLine + "Inner Exception:", ConsoleColor.Yellow, ConsoleColor.White);

                    if (((Exception)exception).InnerException is InvalidConfigurationException)
                    {
                        _consoleManager.OutputString(((InvalidConfigurationException)((Exception)exception).InnerException).Message, ConsoleColor.Yellow, ConsoleColor.White);
                    }
                    else
                    {
                        _consoleManager.OutputString(((Exception)exception).InnerException.Message, ConsoleColor.Yellow, ConsoleColor.White);
                        _consoleManager.OutputString(((Exception)exception).InnerException.StackTrace, ConsoleColor.Yellow, ConsoleColor.White);    
                    }
                }
            }

            if (_arachnodeDAO != null)
            {
                _arachnodeDAO.InsertException(null, null, ((Exception) exception), true);

                if (((Exception) exception).InnerException != null)
                {
                    _arachnodeDAO.InsertException(null, null, ((Exception) exception).InnerException, true);
                }

                _consoleManager.OutputString("Examine table 'dbo.Exceptions'", ConsoleColor.White, ConsoleColor.White);
            }
            else
            {
                Exception exception2 = ((Exception) exception);

                ArachnodeDAO.InsertExceptionIntoWindowsApplicationLog(exception2.Message, exception2.StackTrace);

                _consoleManager.OutputString("Examine the 'Application' event log.", ConsoleColor.White, ConsoleColor.White);
            }

            //if (exception is InvalidConfigurationException)
            //{
            //    throw new InvalidConfigurationException(((InvalidConfigurationException)exception).ApplicationSettings, ((InvalidConfigurationException)exception).WebSettings, ((InvalidConfigurationException)exception).Message);    
            //}
            //else
            //{
            //    throw new Exception(((Exception)exception).Message, (Exception)exception);    
            //}
        }

        internal void LoadCrawlActions(IArachnodeDAO arachnodeDAO)
        {
            CrawlActions = new Dictionary<string, ACrawlAction<TArachnodeDAO>>();

            foreach (ArachnodeDataSet.CrawlActionsRow crawlActionsRow in arachnodeDAO.GetCrawlActions())
            {
                ObjectHandle objectHandle = Engine<TArachnodeDAO>.GetObjectHandle(crawlActionsRow.AssemblyName, crawlActionsRow.TypeName, _applicationSettings, _webSettings);

                ACrawlAction<TArachnodeDAO> crawlAction = (ACrawlAction<TArachnodeDAO>)objectHandle.Unwrap();

                crawlAction.AssemblyName = crawlActionsRow.AssemblyName;
                crawlAction.IsEnabled = crawlActionsRow.IsEnabled;
                crawlAction.Order = crawlActionsRow.Order;
                crawlAction.CrawlActionType = (CrawlActionType) Enum.Parse(typeof (CrawlActionType), crawlActionsRow.CrawlActionTypeID.ToString());
                if (!crawlActionsRow.IsSettingsNull())
                {
                    crawlAction.Settings = crawlActionsRow.Settings;
                }
                crawlAction.TypeName = crawlActionsRow.TypeName;

                CrawlActions.Add(crawlAction.TypeName, crawlAction);
            }
        }

        internal void LoadCrawlRules(IArachnodeDAO arachnodeDAO)
        {
            CrawlRules = new Dictionary<string, ACrawlRule<TArachnodeDAO>>();

            foreach (ArachnodeDataSet.CrawlRulesRow crawlRulesRow in arachnodeDAO.GetCrawlRules())
            {
                ObjectHandle objectHandle = Engine<TArachnodeDAO>.GetObjectHandle(crawlRulesRow.AssemblyName, crawlRulesRow.TypeName, _applicationSettings, _webSettings);

                ACrawlRule<TArachnodeDAO> crawlRule = (ACrawlRule<TArachnodeDAO>)objectHandle.Unwrap();

                crawlRule.AssemblyName = crawlRulesRow.AssemblyName;
                crawlRule.IsEnabled = crawlRulesRow.IsEnabled;
                crawlRule.Order = crawlRulesRow.Order;
                crawlRule.OutputIsDisallowedReason = crawlRulesRow.OutputIsDisallowedReason;
                crawlRule.CrawlRuleType = (CrawlRuleType) Enum.Parse(typeof (CrawlRuleType), crawlRulesRow.CrawlRuleTypeID.ToString());
                if (!crawlRulesRow.IsSettingsNull())
                {
                    crawlRule.Settings = crawlRulesRow.Settings;
                }
                crawlRule.TypeName = crawlRulesRow.TypeName;

                CrawlRules.Add(crawlRule.TypeName, crawlRule);
            }
        }

        public void AddCrawlAction(ACrawlAction<TArachnodeDAO> crawlAction, CrawlActionType crawlActionType, bool isEnabled, int order)
        {
            //TODO: AssemblyName should be Namespace...
            crawlAction.AssemblyName = crawlAction.GetType().Namespace;
            crawlAction.CrawlActionType = crawlActionType;
            crawlAction.IsEnabled = isEnabled;
            crawlAction.Order = order;
            crawlAction.TypeName = crawlAction.GetType().FullName;

            CrawlActions.Add(crawlAction.TypeName, crawlAction);
        }

        public void AddCrawlRule(ACrawlRule<TArachnodeDAO> crawlRule, CrawlRuleType crawlRuleType, bool isEnabled, int order)
        {
            //TODO: AssemblyName should be Namespace...
            crawlRule.AssemblyName = crawlRule.GetType().Namespace;
            crawlRule.CrawlRuleType = crawlRuleType;
            crawlRule.IsEnabled = isEnabled;
            crawlRule.Order = order;
            crawlRule.TypeName = crawlRule.GetType().FullName;

            CrawlRules.Add(crawlRule.TypeName, crawlRule);
        }

        public void Reset()
        {
            //Discoveries keep track of where the crawler has been.  See the database table 'Discoveries' after Stopping a crawl with 'CTRL+C' - rows should be present in 'dbo.Discoveries'.
            //No Discoveries will be present in the database table 'dbo.Discoveries' if a crawl completes.
            //If you do not clear dbo.Discoveries (or the Discoveries in RAM), then the Crawler will treat a 'new' request for an AbsoluteUri as 'already crawled' if the Discovery (AbsoluteUri) is found in memory, or in the database table 'dbo.Discoveries'.
            ClearDiscoveries();

            //this method clears the information collected about how frequently the crawler has visited a Domain.
            //it isn't necessary to call this before each crawl, but may be useful to call if you wish to reset the politeness counts, while a crawl is paused.
            ClearPolitenesses();

            //if you want to reset a crawl, call this method to remove CrawlRequests that may still be on disk.
            //also, call _crawler.ClearDiscoveries()
            ClearUncrawledCrawlRequests();

            //and, check the DisallowedAbsoluteUris table - you may need to clear this to give broken links a chance to resolve, or to allow new AbsoluteUris to be crawled as a result of rule changes...
            ClearDisallowedAbsoluteUris();
        }

        public void ResetCrawlActions()
        {
            if (Engine.State != EngineState.Start)
            {
                ActionManager.Stop();

                LoadCrawlActions(_arachnodeDAO);

                ActionManager.ProcessCrawlActions(this);
            }
        }

        public void ResetCrawlRules()
        {
            if (Engine.State != EngineState.Start)
            {
                RuleManager.Stop();

                LoadCrawlRules(_arachnodeDAO);

                RuleManager.ProcessCrawlRules(this);
            }
        }
    }
}