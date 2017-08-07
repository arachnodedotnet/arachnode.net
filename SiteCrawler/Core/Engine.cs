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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Messaging;
using System.Net;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Lifetime;
using System.Runtime.Remoting.Proxies;
using System.Security.Principal;
using System.Threading;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Managers;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.DataSource;
using Arachnode.Performance;
using Arachnode.Renderer.Value;
using Arachnode.Renderer.Value.Enums;
using Arachnode.SiteCrawler.Components;
using Arachnode.SiteCrawler.Managers;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.AbstractClasses;
using Arachnode.SiteCrawler.Value.Enums;
using Arachnode.Structures.Value;
using mshtml;
using Message = System.Messaging.Message;

#endregion

namespace Arachnode.SiteCrawler.Core
{
    /// <summary>
    /// 	Provides core thread management and CrawlRequest creation functionality.
    /// </summary>
    public class Engine<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        #region Delegates

        public delegate void OnCrawlStoppedEventHandler(Engine<TArachnodeDAO> engine);

        public delegate void OnCrawlCompletedEventHandler(Engine<TArachnodeDAO> engine);

        public delegate void OnCrawlRequestCanceledEventHandler(CrawlRequest<TArachnodeDAO> crawlRequest);

        public delegate void OnCrawlRequestCompletedEventHandler(CrawlRequest<TArachnodeDAO> sender);

        public delegate void OnCrawlRequestRetryingEventHandler(CrawlRequest<TArachnodeDAO> crawlRequest);

        public delegate void OnCrawlRequestThrottledEventHandler(CrawlRequest<TArachnodeDAO> crawlRequest);

        #endregion

        private readonly Stopwatch _stopwatch = new Stopwatch();

        private readonly object _arachnodeDAOLock = new object();
        private readonly object _assignCrawlRequestsToCrawlsLock = new object();
        private readonly object _startStopPauseResumeLock = new object();
        private readonly object _renderersLock = new object();

        private ApplicationSettings _applicationSettings;
        private WebSettings _webSettings;

        private Cache<TArachnodeDAO> _cache;

        private int _totalEnqueuedCrawlRequests;

        private ActionManager<TArachnodeDAO> _actionManager;
        private CacheManager<TArachnodeDAO> _cacheManager;
        private ConsoleManager<TArachnodeDAO> _consoleManager;
        private CookieManager _cookieManager;
        private CrawlRequestManager<TArachnodeDAO> _crawlRequestManager;
        private DataTypeManager<TArachnodeDAO> _dataTypeManager;
        private DiscoveryManager<TArachnodeDAO> _discoveryManager;
        private EncodingManager<TArachnodeDAO> _encodingManager;
        private HtmlManager<TArachnodeDAO> _htmlManager;
        private MemoryManager<TArachnodeDAO> _memoryManager;
        private PolitenessManager<TArachnodeDAO> _politenessManager;
        private ProxyManager<TArachnodeDAO> _proxyManager;
        private ReportingManager<TArachnodeDAO> _reportingManager;
        private RuleManager<TArachnodeDAO> _ruleManager;
        
        private readonly TArachnodeDAO _arachnodeDAO;

        private readonly Crawler<TArachnodeDAO> _crawler;
        private readonly bool _enableRenderers;

        private readonly Random _random = new Random();
        private readonly ManualResetEvent _stateControl = new ManualResetEvent(true);

        private bool _outputPopulateCrawlRequestsCrawlRequestsCount = true;
        private EngineState _engineState;

        private readonly Dictionary<int, Renderer.Renderer> _renderer_RendererServices;
        private readonly Dictionary<int, MessageQueue> _renderer_RendererMessageQueues;
        private readonly Dictionary<int, MessageQueue> _renderer_EngineMessageQueues;

        private DateTime _pauseTime;
        private DateTime _resumeTime;
        private DateTime _startTime;
        private DateTime _stopTime;

        private bool _stopEngineOnCrawlCompleted = true;

        /// <summary>
        /// 	The Engine.
        /// </summary>
        /// <param name = "crawler">The crawler.</param>
        /// <param name="actionManager"></param>
        /// <param name="consoleManager"></param>
        /// <param name="crawlRequestManager"></param>
        /// <param name="politenessManager"></param>
        /// <param name="ruleManager"></param>
        /// <param name="enableRenderers"></param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        internal Engine(ApplicationSettings applicationSettings, WebSettings webSettings, Crawler<TArachnodeDAO> crawler, Cache<TArachnodeDAO> cache, ActionManager<TArachnodeDAO> actionManager, CacheManager<TArachnodeDAO> cacheManager, ConsoleManager<TArachnodeDAO> consoleManager, CookieManager cookieManager, CrawlRequestManager<TArachnodeDAO> crawlRequestManager, DataTypeManager<TArachnodeDAO> dataTypeManager, DiscoveryManager<TArachnodeDAO> discoveryManager, EncodingManager<TArachnodeDAO> encodingManager, HtmlManager<TArachnodeDAO> htmlManager, MemoryManager<TArachnodeDAO> memoryManager, PolitenessManager<TArachnodeDAO> politenessManager, ProxyManager<TArachnodeDAO> proxyManager, ReportingManager<TArachnodeDAO> reportingManager, RuleManager<TArachnodeDAO> ruleManager, bool enableRenderers, TArachnodeDAO arachnodeDAO)
        {
            _applicationSettings = applicationSettings;
            _webSettings = webSettings;

            _crawler = crawler;
            Cache = cache;

            _enableRenderers = enableRenderers;

            _engineState = EngineState.Stop;

            ActionManager = actionManager;
            CacheManager = cacheManager;
            ConsoleManager = consoleManager;
            CookieManager = cookieManager;
            CrawlRequestManager = crawlRequestManager;
            DataTypeManager = dataTypeManager;
            DiscoveryManager = discoveryManager;
            EncodingManager = encodingManager;
            HtmlManager = htmlManager;
            MemoryManager = memoryManager;
            PolitenessManager = politenessManager;
            ProxyManager = proxyManager;
            ReportingManager = reportingManager;
            RuleManager = ruleManager;

            _arachnodeDAO = arachnodeDAO;

            LoadEngineActions(arachnodeDAO);

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            ConsoleManager.RefreshConsoleOutputLog();

            Crawls = new SortedList<int, Crawl<TArachnodeDAO>>();
            DiscoveryProcessors = new SortedList<int, DiscoveryProcessor<TArachnodeDAO>>();

            _renderer_RendererServices = new Dictionary<int, Renderer.Renderer>(ApplicationSettings.MaximumNumberOfCrawlThreads);
            _renderer_RendererMessageQueues = new Dictionary<int, MessageQueue>(ApplicationSettings.MaximumNumberOfCrawlThreads);
            _renderer_EngineMessageQueues = new Dictionary<int, MessageQueue>(ApplicationSettings.MaximumNumberOfCrawlThreads);
        }

        public Dictionary<string, AEngineAction<TArachnodeDAO>> EngineActions { get; private set; }

        /// <summary>
        /// 	Maintains a list of Crawls mananged by the Engine.
        /// </summary>
        internal SortedList<int, Crawl<TArachnodeDAO>> Crawls { get; set; }

        /// <summary>
        /// 	Maintains a list of DiscoveryProcessor mananged by the Engine.
        /// </summary>
        internal SortedList<int, DiscoveryProcessor<TArachnodeDAO>> DiscoveryProcessors { get; set; }

        internal static bool IsPopulatingCrawlCrawlRequests { get; set; }

        /// <summary>
        /// 	The crawling state of the Engine.
        /// </summary>
        /// <value>The state of the engine.</value>
        public EngineState State
        {
            get { return _engineState; }
        }

        /// <summary>
        /// 	A wait handle to pause or resume the crawl threads.
        /// </summary>
        /// <value>The state control.</value>
        internal ManualResetEvent StateControl
        {
            get { return _stateControl; }
        }

        /// <summary>
        /// 	Should the Engine stop when a crawl completes?  It is necessary to set this value to 'false' when using the Service.
        /// </summary>
        public bool StopEngineOnCrawlCompleted
        {
            get { return _stopEngineOnCrawlCompleted; }
            set { _stopEngineOnCrawlCompleted = value; }
        }

        internal ApplicationSettings ApplicationSettings
        {
            get { return _applicationSettings; }
            set { _applicationSettings = value; }
        }

        internal Cache<TArachnodeDAO> Cache
        {
            get { return _cache; }
            set { _cache = value; }
        }

        internal ActionManager<TArachnodeDAO> ActionManager
        {
            get { return _actionManager; }
            set { _actionManager = value; }
        }

        internal CacheManager<TArachnodeDAO> CacheManager
        {
            get { return _cacheManager; }
            set { _cacheManager = value; }
        }       

        internal ConsoleManager<TArachnodeDAO> ConsoleManager
        {
            get { return _consoleManager; }
            set { _consoleManager = value; }
        }

        internal CookieManager CookieManager
        {
            get { return _cookieManager; }
            set { _cookieManager = value; }
        }

        internal CrawlRequestManager<TArachnodeDAO> CrawlRequestManager
        {
            get { return _crawlRequestManager; }
            set { _crawlRequestManager = value; }
        }

        internal DataTypeManager<TArachnodeDAO> DataTypeManager
        {
            get { return _dataTypeManager; }
            set { _dataTypeManager = value; }
        }

        internal DiscoveryManager<TArachnodeDAO> DiscoveryManager
        {
            get { return _discoveryManager; }
            set { _discoveryManager = value; }
        }

        internal EncodingManager<TArachnodeDAO> EncodingManager
        {
            get { return _encodingManager; }
            set { _encodingManager = value; }
        }

        internal HtmlManager<TArachnodeDAO> HtmlManager
        {
            get { return _htmlManager; }
            set { _htmlManager = value; }
        }

        internal MemoryManager<TArachnodeDAO> MemoryManager
        {
            get { return _memoryManager; }
            set { _memoryManager = value; }
        }

        internal PolitenessManager<TArachnodeDAO> PolitenessManager
        {
            get { return _politenessManager; }
            set { _politenessManager = value; }
        }

        internal ProxyManager<TArachnodeDAO> ProxyManager
        {
            get { return _proxyManager; }
            set { _proxyManager = value; }
        }

        internal ReportingManager<TArachnodeDAO> ReportingManager
        {
            get { return _reportingManager; }
            set { _reportingManager = value; }
        }

        internal RuleManager<TArachnodeDAO> RuleManager
        {
            get { return _ruleManager; }
            set { _ruleManager = value; }
        }

        /// <summary>
        /// 	Catch-all Exception handler.  This level of exception handling is useful for new Rules and Actions for those not familiar with the Plug-in environment.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.UnhandledExceptionEventArgs" /> instance containing the event data.</param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e != null)
            {
                Exception exception = e.ExceptionObject as Exception;

                if (exception != null)
                {
                    lock (_arachnodeDAOLock)
                    {
                        _arachnodeDAO.InsertException(null, null, exception, true);
                    }
                }
            }
        }

        /// <summary>
        /// 	Occurs when [on crawl completed].
        /// </summary>
        public event OnCrawlStoppedEventHandler CrawlStopped;

        /// <summary>
        /// 	Occurs when [on crawl completed].
        /// </summary>
        public event OnCrawlCompletedEventHandler CrawlCompleted;

        /// <summary>
        /// 	Occurs when [on crawl request completed].
        /// </summary>
        public event OnCrawlRequestCompletedEventHandler CrawlRequestCompleted;

        /// <summary>
        /// 	Occurs when [on crawl request retrying].
        /// </summary>
        public event OnCrawlRequestCompletedEventHandler CrawlRequestRetrying;

        /// <summary>
        /// 	Occurs when [on crawl request canceled].
        /// </summary>
        public event OnCrawlRequestCanceledEventHandler CrawlRequestCanceled;

        /// <summary>
        /// 	Occurs when [on crawl request throttled].
        /// </summary>
        public event OnCrawlRequestThrottledEventHandler CrawlRequestThrottled;

        /// <summary>
        /// 	Gets the object handle.
        /// </summary>
        /// <param name = "assemblyName">Name of the assembly.</param>
        /// <param name = "typeName">Name of the type.</param>
        /// <returns></returns>
        internal static ObjectHandle GetObjectHandle(string assemblyName, string typeName, ApplicationSettings applicationSettings, WebSettings webSettings)
        {
            ObjectHandle objectHandle = null;

            if (assemblyName.IndexOf("\\") == -1)
            {
                Assembly assembly = Assembly.Load(assemblyName);
                Type type = null;

                foreach (Type type2 in assembly.GetTypes())
                {
                    if (!string.IsNullOrEmpty(type2.FullName))
                    {
                        string fullName = type2.FullName.Split("`".ToCharArray())[0];

                        if (fullName == typeName)
                        {
                            type = type2.MakeGenericType(typeof(TArachnodeDAO));
                        }
                    }
                }

                objectHandle = new ObjectHandle(Activator.CreateInstance(type, applicationSettings, webSettings));
            }
            else
            {
                objectHandle = Activator.CreateInstanceFrom(assemblyName, typeName, new object[] { applicationSettings, webSettings });
            }

            return objectHandle;
        }

        /// <summary>
        /// 	Submits a CrawlRequest to an internal queue for crawling.  This is used when submitting from the Crawler, from user code.
        /// </summary>
        /// <param name = "crawlRequest">The CrawlRequest to be crawled.</param>
        internal bool Crawl(CrawlRequest<TArachnodeDAO> crawlRequest)
        {
            lock (Cache.UncrawledCrawlRequestsLock)
            {
                if (_crawler.CrawlerPeerManager.CrawlerPeers == null || _crawler.CrawlerPeerManager.CrawlerPeers.Count == 0)
                {
                    return Cache.AddCrawlRequestToBeCrawled(crawlRequest, false, false, _arachnodeDAO);
                }

                return Cache.AddCrawlRequestToBeCrawled(crawlRequest, false, false, _arachnodeDAO);
            }
        }

        /// <summary>
        /// 	Renders the CrawlRequest by running all client-side scripts.
        /// </summary>
        /// <param name = "crawlRequest">The CrawlRequest to be rendered.</param>
        internal RendererResponse Render(CrawlRequest<TArachnodeDAO> crawlRequest, RenderAction renderAction, RenderType renderType)
        {
            try
            {
                if (_renderer_EngineMessageQueues.Count == 0)
                {
                    throw new Exception("Enable the renderers in the Crawler constructor or set the CrawlRequest 'RenderType' to 'None'.");
                }

                RendererMessage rendererMessage = new RendererMessage();

                rendererMessage.AbsoluteUri = crawlRequest.Discovery.Uri.AbsoluteUri;
                rendererMessage.CrawlRequestTimeoutInMinutes = ApplicationSettings.CrawlRequestTimeoutInMinutes;
                rendererMessage.RenderAction = renderAction;
                rendererMessage.RenderType = renderType;
                rendererMessage.ThreadNumber = crawlRequest.Crawl.CrawlInfo.ThreadNumber;
                rendererMessage.UserAgent = ApplicationSettings.UserAgent;
                IWebProxy webProxy = ProxyManager.GetNextProxy();
                if(webProxy != null)
                {
                    if (((WebProxy)webProxy).Address != null)
                    {
                        rendererMessage.ProxyServer = ((WebProxy) webProxy).Address.AbsoluteUri;
                    }
                }
                CookieManager.GetCookies(crawlRequest.Discovery.Uri.AbsoluteUri, crawlRequest.Crawl.Crawler.CookieContainer);
                if (crawlRequest.Crawl.Crawler.CookieContainer != null)
                {
                    foreach (Cookie cookie in crawlRequest.Crawl.Crawler.CookieContainer.GetCookies(crawlRequest.Discovery.Uri))
                    {
                        rendererMessage.Cookie += cookie.Name + "=" + cookie.Value + "; ";
                    }
                }

                //send the message...
                _renderer_RendererMessageQueues[crawlRequest.Crawl.CrawlInfo.ThreadNumber].Send(rendererMessage);

                //wait to receive the message...
                Message message = _renderer_EngineMessageQueues[crawlRequest.Crawl.CrawlInfo.ThreadNumber].Receive(TimeSpan.FromMinutes(ApplicationSettings.CrawlRequestTimeoutInMinutes*2));

                rendererMessage = (RendererMessage) message.Body;

                if (!_renderer_RendererServices.ContainsKey(crawlRequest.Crawl.CrawlInfo.ThreadNumber))
                {
                    lock (_arachnodeDAOLock)
                    {
                        if (_renderer_RendererServices.Count == 0)
                        {
                            BinaryClientFormatterSinkProvider clientProvider = new BinaryClientFormatterSinkProvider();
                            BinaryServerFormatterSinkProvider serverProvider = null;
                            //serverProvider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

                            var props = new Hashtable();
                            props["name"] = "Engine" + crawlRequest.Crawl.CrawlInfo.ThreadNumber;
                            props["portName"] = "Engine" + crawlRequest.Crawl.CrawlInfo.ThreadNumber;
                            props["authorizedGroup"] = WindowsIdentity.GetCurrent().Name;
                            //props["typeFilterLevel"] = TypeFilterLevel.Full;

                            IpcChannel channel = new IpcChannel(props, clientProvider, serverProvider);

                            ChannelServices.RegisterChannel(channel, false);

                            System.Runtime.Remoting.WellKnownClientTypeEntry remoteType = new WellKnownClientTypeEntry(typeof (Renderer.Renderer), "ipc://Renderer" + crawlRequest.Crawl.CrawlInfo.ThreadNumber + "/Renderer" + crawlRequest.Crawl.CrawlInfo.ThreadNumber);

                            RemotingConfiguration.RegisterWellKnownClientType(remoteType);

                            string objectUri;
                            System.Runtime.Remoting.Messaging.IMessageSink messageSink = channel.CreateMessageSink("ipc://Renderer" + crawlRequest.Crawl.CrawlInfo.ThreadNumber + "/Renderer" + crawlRequest.Crawl.CrawlInfo.ThreadNumber, null, out objectUri);
                        }

                        Renderer.Renderer renderer = (Renderer.Renderer) Activator.GetObject(typeof (Renderer.Renderer), "ipc://Renderer" + crawlRequest.Crawl.CrawlInfo.ThreadNumber + "/Renderer" + crawlRequest.Crawl.CrawlInfo.ThreadNumber);

                        _renderer_RendererServices.Add(crawlRequest.Crawl.CrawlInfo.ThreadNumber, renderer);
                    }
                }

                RendererResponse rendererResponse = new RendererResponse();

                HTMLDocumentClass htmlDocumentClass = _renderer_RendererServices[crawlRequest.Crawl.CrawlInfo.ThreadNumber].HtmlDocumentClass;

                rendererResponse.HTMLDocumentClass = htmlDocumentClass;
                rendererResponse.RendererMessage = rendererMessage;

                return rendererResponse;
            }
            catch (Exception exception)
            {
                crawlRequest.Crawl.ArachnodeDAO.InsertException(crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri, exception, false);

                int rendererCount = 0;

                foreach (Process process in Process.GetProcesses())
                {
                    if (process.ProcessName.ToLowerInvariant().Contains("arachnode.renderer"))
                    {
                        rendererCount++;
                    }
                }

                //we need to start a new Renderer process and send a 
                if (rendererCount != ApplicationSettings.MaximumNumberOfCrawlThreads)
                {
                    lock(_renderersLock)
                    {
                        try
                        {
                            //remove the old Renderer...
                            _renderer_RendererServices.Remove(crawlRequest.Crawl.CrawlInfo.ThreadNumber);

                            //send a new Renderer...
                            RendererMessage rendererMessage = new RendererMessage();

                            rendererMessage.ThreadNumber = crawlRequest.Crawl.CrawlInfo.ThreadNumber;

                            MessageQueue messageQueue = new MessageQueue(".\\private$\\Renderer_Renderers:" + 0);

                            messageQueue.Send(rendererMessage);

                            //start the new Renderer, which will pick up the initial message queue message...
                            Process process = Process.Start("Arachnode.Renderer.exe");
                        }
                        catch (Exception exception2)
                        {
                            crawlRequest.Crawl.ArachnodeDAO.InsertException(null, null, exception2, false);

                            //if we can't start a new Renderer process, then we need to stop...
                            Stop(true);
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 	Creates crawl and discovery processor threads, adds them to a list but does not start them.
        /// </summary>
        private void CreateWorkerThreads()
        {
            Crawls.Clear();
            DiscoveryProcessors.Clear();

            foreach (Process process in Process.GetProcessesByName("Arachnode.Renderer"))
            {
                try
                {
                    process.Kill();
                }
                catch
                {
                }
            }

            if (_enableRenderers)
            {
                if (MessageQueue.Exists(".\\private$\\Renderer_Renderers:" + 0))
                {
                    MessageQueue.Delete(".\\private$\\Renderer_Renderers:" + 0);
                }

                MessageQueue.Create(".\\private$\\Renderer_Renderers:" + 0);

                MessageQueue messageQueue = new MessageQueue(".\\private$\\Renderer_Renderers:" + 0);

                for (int i = 0; i < ApplicationSettings.MaximumNumberOfCrawlThreads; i++)
                {
                    RendererMessage rendererMessage = new RendererMessage();

                    rendererMessage.ThreadNumber = i + 1;

                    messageQueue.Send(rendererMessage);
                }
            }

            for (int i = 0; i < ApplicationSettings.MaximumNumberOfCrawlThreads; i++)
            {
                //create the Crawl...
                Crawl<TArachnodeDAO> crawl = new Crawl<TArachnodeDAO>(_applicationSettings, _webSettings, _crawler, _actionManager, _consoleManager, _cookieManager, _crawlRequestManager, _dataTypeManager, _discoveryManager, _encodingManager, _htmlManager, _politenessManager, _proxyManager, _ruleManager, true);

                Thread thread1 = new Thread(crawl.BeginCrawl);
                thread1.Name = "CrawlThread:" + (i + 1);

                crawl.Thread = thread1;

                Crawls.Add(i + 1, crawl);

                if (_applicationSettings.ProcessDiscoveriesAsynchronously)
                {
                    //create the DiscoveryProcessor...
                    DiscoveryProcessor<TArachnodeDAO> discoveryProcessor = new DiscoveryProcessor<TArachnodeDAO>(_applicationSettings, _crawler, CrawlRequestManager);

                    Thread thread2 = new Thread(discoveryProcessor.BeginDiscoveryProcessor);
                    thread2.Name = "DiscoveryProcessorThread:" + (i + 1);

                    discoveryProcessor.Thread = thread2;

                    DiscoveryProcessors.Add(i + 1, discoveryProcessor);
                }

                //create the associated Renderer...
                if (_enableRenderers)
                {
                    if (MessageQueue.Exists(".\\private$\\Renderer_Renderers:" + (i + 1)))
                    {
                        MessageQueue.Delete(".\\private$\\Renderer_Renderers:" + (i + 1));
                    }

                    MessageQueue.Create(".\\private$\\Renderer_Renderers:" + (i + 1));

                    MessageQueue messageQueue1 = new MessageQueue(".\\private$\\Renderer_Renderers:" + (i + 1));

                    _renderer_RendererMessageQueues.Add(i + 1, messageQueue1);

                    /**/

                    if (MessageQueue.Exists(".\\private$\\Renderer_Engine:" + (i + 1)))
                    {
                        MessageQueue.Delete(".\\private$\\Renderer_Engine:" + (i + 1));
                    }

                    MessageQueue.Create(".\\private$\\Renderer_Engine:" + (i + 1));

                    MessageQueue messageQueue2 = new MessageQueue(".\\private$\\Renderer_Engine:" + (i + 1));
                    messageQueue2.Formatter = new XmlMessageFormatter(new Type[] {typeof (RendererMessage)});

                    _renderer_EngineMessageQueues.Add(i + 1, messageQueue2);

                    Process process = Process.Start("Arachnode.Renderer.exe");
                }

                //assign the Partners
                if (ApplicationSettings.MaximumNumberOfCrawlThreads > 1)
                {
                    if (i == ApplicationSettings.MaximumNumberOfCrawlThreads - 1)
                    {
                        Crawls[1].Partner = Crawls[ApplicationSettings.MaximumNumberOfCrawlThreads];
                        Crawls[i + 1].Partner = Crawls[i];
                    }
                    else if (i != 0)
                    {
                        Crawls[i + 1].Partner = Crawls[i];
                    }
                }
            }
        }

        private void LoadEngineActions(TArachnodeDAO arachnodeDAO)
        {
            EngineActions = new Dictionary<string, AEngineAction<TArachnodeDAO>>();

            foreach (ArachnodeDataSet.EngineActionsRow engineActionsRow in arachnodeDAO.GetEngineActions())
            {
                ObjectHandle objectHandle = GetObjectHandle(engineActionsRow.AssemblyName, engineActionsRow.TypeName, _applicationSettings, _webSettings);

                AEngineAction<TArachnodeDAO> engineAction = (AEngineAction<TArachnodeDAO>)objectHandle.Unwrap();

                engineAction.AssemblyName = engineActionsRow.AssemblyName;
                engineAction.IsEnabled = engineActionsRow.IsEnabled;
                engineAction.Order = engineActionsRow.Order;
                engineAction.EngineActionType = (EngineActionType) Enum.Parse(typeof (EngineActionType), engineActionsRow.EngineActionTypeID.ToString());
                if (!engineActionsRow.IsSettingsNull())
                {
                    engineAction.Settings = engineActionsRow.Settings;
                }
                engineAction.TypeName = engineActionsRow.TypeName;

                EngineActions.Add(engineAction.TypeName, engineAction);

                ConsoleManager.OutputBehavior(engineAction);
            }
        }
        
        /// <summary>
        /// 	Populates the crawl crawl requests.  This is the most important method for the Engine.
        /// </summary>
        /// <param name = "crawl">The crawl.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        internal void PopulateCrawlCrawlRequests(Crawl<TArachnodeDAO> crawl, TArachnodeDAO arachnodeDAO)
        {
            if (Monitor.TryEnter(_arachnodeDAOLock, 1000))
            {
                try
                {
                    if (State == EngineState.Start)
                    {
                        //Will be used effectively to assign CrawlRequests to Crawls when Crawls are crawling at depth.
                        //(Crawls crawling at depth will create new CrawlRequests and place them in the Cache.)
                        int totalEnqueuedCrawlRequests = AssignCrawlRequestsToCrawls();

                        if (crawl.UncrawledCrawlRequests.Count == 0 && totalEnqueuedCrawlRequests <= ApplicationSettings.MaximumNumberOfCrawlRequestsToCreatePerBatch)
                        {
                            IsPopulatingCrawlCrawlRequests = true;

                            ActionManager.PerformEngineActions(Cache.UncrawledCrawlRequests, EngineActionType.PreGetCrawlRequests, _arachnodeDAO);

                            int crawlRequestsAdded = 0;

                            if (Cache.UncrawledCrawlRequests.Count == 0)
                            {
                                ConsoleManager.OutputString("Engine: PopulateCrawlRequests: [Please Wait]", ConsoleColor.White, ConsoleColor.Gray);

                                ArachnodeDataSet.CrawlRequestsDataTable crawlRequestsDataTable = _arachnodeDAO.GetCrawlRequests(ApplicationSettings.MaximumNumberOfCrawlRequestsToCreatePerBatch, ApplicationSettings.CreateCrawlRequestsFromDatabaseCrawlRequests, ApplicationSettings.CreateCrawlRequestsFromDatabaseFiles, ApplicationSettings.AssignCrawlRequestPrioritiesForFiles, ApplicationSettings.CreateCrawlRequestsFromDatabaseHyperLinks, ApplicationSettings.AssignCrawlRequestPrioritiesForHyperLinks, ApplicationSettings.CreateCrawlRequestsFromDatabaseImages, ApplicationSettings.AssignCrawlRequestPrioritiesForImages, ApplicationSettings.CreateCrawlRequestsFromDatabaseWebPages, ApplicationSettings.AssignCrawlRequestPrioritiesForWebPages);

                                if (_outputPopulateCrawlRequestsCrawlRequestsCount)
                                {
                                    ConsoleManager.OutputString("Engine: PopulateCrawlRequests: " + crawlRequestsDataTable.Count + " CrawlRequests Returned.", ConsoleColor.White, ConsoleColor.Gray);
                                }

                                _outputPopulateCrawlRequestsCrawlRequestsCount = crawlRequestsDataTable.Count != 0 ? true : false;                               

                                foreach (ArachnodeDataSet.CrawlRequestsRow crawlRequestsRow in crawlRequestsDataTable)
                                {
                                    //if a CrawlRequest comes from the Database we need to check that the AbsoluteUri isn't accounted for in either the Cache's UncrawledCrawlRequests, the Crawls' local UnassignedDiscoveries/UncrawledCrawlRequests and the Crawls' CurrentCrawlRequests.
                                    //we do so as completely synchronizing all threads with the Discoveries/CrawlRequests table via SERIALIZABLE incurs a massive performance hit.

                                    bool isTheAbsoluteUriAccountedFor = false;

                                    foreach (PriorityQueueItem<CrawlRequest<TArachnodeDAO>> crawlRequest in Cache.UncrawledCrawlRequests)
                                    {
                                        if(crawlRequest.Value.Discovery.Uri.AbsoluteUri == crawlRequestsRow.AbsoluteUri2)
                                        {
                                            isTheAbsoluteUriAccountedFor = true;
                                            break;
                                        }
                                    }

                                    if (isTheAbsoluteUriAccountedFor)
                                    {
                                        continue;
                                    }

                                    foreach (Crawl<TArachnodeDAO> crawl2 in Crawls.Values)
                                    {
                                        if (crawl2.CrawlInfo.CurrentCrawlRequest != null && crawl2.CrawlInfo.CurrentCrawlRequest.Discovery.Uri.AbsoluteUri == crawlRequestsRow.AbsoluteUri2)
                                        {
                                            isTheAbsoluteUriAccountedFor = true;
                                            break;
                                        }

                                        if (crawl2.UnassignedDiscoveries.Contains(crawlRequestsRow.AbsoluteUri2))
                                        {
                                            isTheAbsoluteUriAccountedFor = true;
                                            break;
                                        }

                                        foreach (PriorityQueueItem<CrawlRequest<TArachnodeDAO>> crawlRequest in crawl2.UncrawledCrawlRequests)
                                        {
                                            if (crawlRequest.Value.Discovery.Uri.AbsoluteUri == crawlRequestsRow.AbsoluteUri2)
                                            {
                                                isTheAbsoluteUriAccountedFor = true;
                                                break;
                                            }
                                        }

                                        if (isTheAbsoluteUriAccountedFor)
                                        {
                                            break;
                                        }
                                    }

                                    if (isTheAbsoluteUriAccountedFor)
                                    {
                                        continue;
                                    }

                                    /**/

                                    if (!isTheAbsoluteUriAccountedFor)
                                    {
                                        if (!string.IsNullOrEmpty(ApplicationSettings.UniqueIdentifier))
                                        {
                                            if (!crawlRequestsRow.IsAbsoluteUri0Null())
                                            {
                                                crawlRequestsRow.AbsoluteUri0 = crawlRequestsRow.AbsoluteUri1.Substring(0, crawlRequestsRow.AbsoluteUri0.LastIndexOf(ApplicationSettings.UniqueIdentifier));
                                            }
                                            crawlRequestsRow.AbsoluteUri1 = crawlRequestsRow.AbsoluteUri1.Substring(0, crawlRequestsRow.AbsoluteUri1.LastIndexOf(ApplicationSettings.UniqueIdentifier));
                                            crawlRequestsRow.AbsoluteUri2 = crawlRequestsRow.AbsoluteUri2.Substring(0, crawlRequestsRow.AbsoluteUri2.LastIndexOf(ApplicationSettings.UniqueIdentifier));
                                        }

                                        if (!Cache.AddCrawlRequestToBeCrawled(new CrawlRequest<TArachnodeDAO>(crawlRequestsRow, Cache, arachnodeDAO), true, false, _arachnodeDAO))
                                        {
                                            //ensure that the AbsoluteUri (and the CacheKey representation) are removed from the database.

                                            arachnodeDAO.DeleteCrawlRequest(crawlRequestsRow.AbsoluteUri1, crawlRequestsRow.AbsoluteUri2);
                                        }
                                        else
                                        {
                                            crawlRequestsAdded++;
                                        }
                                    }
                                }

                                if (crawlRequestsAdded != ApplicationSettings.MaximumNumberOfCrawlRequestsToCreatePerBatch)
                                {
                                }

                                if (crawlRequestsDataTable.Count != 0 && Cache.UncrawledCrawlRequests.Count == 0 && MemoryManager.IsUsingDesiredMaximumMemoryInMegabytes(false) && IsCrawlFinished())
                                {
                                    string message = "Engine: arachnode.net requires more RAM than specified by ApplicationSettings.DesiredMaximumMemoryUsageInMegabytes.\n" + "\tApplicationSettings.DesiredMaximumMemoryUsageInMegabytes: " + ApplicationSettings.DesiredMaximumMemoryUsageInMegabytes + " MB\n" + "\tEnvironment.WorkingSet: " + Environment.WorkingSet / 1024 / 1024 + " MB";

                                    ConsoleManager.OutputString(message, ConsoleColor.Red, ConsoleColor.Gray);

                                    try
                                    {
                                        throw new OutOfMemoryException(message);
                                    }
                                    catch (Exception exception)
                                    {
                                        _arachnodeDAO.InsertException(null, null, exception, false);
                                    }

                                    _crawler.Engine.Stop();

                                    _crawler.Engine.CrawlCompleted(_crawler.Engine);
                                }
                            }

                            ActionManager.PerformEngineActions(Cache.UncrawledCrawlRequests, EngineActionType.PostGetCrawlRequests, _arachnodeDAO);

                            //Check to see if there are any more CrawlRequests to crawl.
                            totalEnqueuedCrawlRequests = AssignCrawlRequestsToCrawls();

                            if (totalEnqueuedCrawlRequests == 0 || crawlRequestsAdded == 0)
                            {
                                _crawler.CrawlerPeerManager.SendCrawlRequestRequestMessageToCrawlerPeers(_arachnodeDAO);
                            }

                            IsPopulatingCrawlCrawlRequests = false;

                            //ensure that we don't have any more CrawlRequests in the database...
                            if (crawlRequestsAdded == 0 && IsCrawlFinished())
                            {
                                //if the crawl is finished, then the CrawlRequests table can be cleared (if not Crawled, then AbsoluteUris will be in the DisallowedAbsoluteUris table)...
                                //as well as the Discoveries table as the Discoveries table represents AN's 'MEMORY'...
                                if (StopEngineOnCrawlCompleted)
                                {
                                    Stop(false);
                                }
                                else
                                {
                                    //pause the current Crawl...
                                    crawl.CrawlInfo.CrawlState = CrawlState.Pause;

                                    //fire the completion event.
                                    if (_crawler.Engine.CrawlCompleted != null)
                                    {
                                        ConsoleManager.OutputString("Engine: CrawlCompleted", ConsoleColor.White, ConsoleColor.Gray);

                                        if (_crawler.Engine.CrawlCompleted != null)
                                        {
                                            _crawler.Engine.CrawlCompleted(_crawler.Engine);
                                        }
                                    }

                                    _crawler.CrawlerPeerManager.StopServer(true, arachnodeDAO);

                                    //clear the Discoveries...
                                    _crawler.ClearDiscoveries();

                                    //clear the Politeness...
                                    _crawler.ClearPolitenesses();

                                    //clear the uncrawled CrawlRequests...
                                    _crawler.ClearUncrawledCrawlRequests();

                                    //clear the DisallowedAbsoluteUris table...
                                    _crawler.ClearDisallowedAbsoluteUris();

                                    //resume the current Crawl...
                                    crawl.CrawlInfo.CrawlState = CrawlState.Start;

                                    Thread.Sleep(1000);

                                    _crawler.CrawlerPeerManager.StartServer(_crawler, arachnodeDAO);
                                }
                            }
                        }

                        if (crawl.UncrawledCrawlRequests.Count == 0 && crawl.Partner != null)
                        {
                            Crawl<TArachnodeDAO> crawl2 = crawl.Partner;

                            while (crawl2.UncrawledCrawlRequests.Count <= 1 && crawl2 != crawl)
                            {
                                crawl2 = crawl2.Partner;
                            }

                            if (crawl2.UncrawledCrawlRequests.Count != 0)
                            {
                                //HACK/TODO: - Need to lock here! (May not be a problem, actually...)
                                for (int i = 0; i < crawl2.UncrawledCrawlRequests.Count/2; i++)
                                {
                                    CrawlRequest<TArachnodeDAO> crawlRequest = crawl2.UncrawledCrawlRequests.Dequeue();

                                    if (crawlRequest != null)
                                    {
                                        crawl.UncrawledCrawlRequests.Enqueue(crawlRequest, crawlRequest.Priority);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    arachnodeDAO.InsertException(null, null, exception, false);
                }
                finally
                {
                    Monitor.Exit(_arachnodeDAOLock);
                }
            }
            else
            {
                //don't make other threads wait while we access the database...
                AssignCrawlRequestsToCrawls();
            }

            _crawler.CrawlerPeerManager.SendStatusMessageToCrawlerPeers(arachnodeDAO);
        }

        /// <summary>
        /// 	Assigns the crawl requests to crawls.
        /// </summary>
        private int AssignCrawlRequestsToCrawls()
        {
            int totalEnqueuedCrawlRequests = 0;

            lock (_assignCrawlRequestsToCrawlsLock)
            {
                foreach (Crawl<TArachnodeDAO> crawl in Crawls.Values)
                {
                    totalEnqueuedCrawlRequests += crawl.UncrawledCrawlRequests.Count;
                }

                totalEnqueuedCrawlRequests += Cache.UncrawledCrawlRequests.Count;

                //assign CrawlRequests to Crawls from the Cache...
                if (Cache.UncrawledCrawlRequests.Count != 0 && _engineState == EngineState.Start)
                {
                    lock (Cache.UncrawledCrawlRequestsLock)
                    {
                        //assign the CrawlRequests by Domain...
                        //be polite and round-robin the requests if we have the ability to do so.
                        Dictionary<string, List<CrawlRequest<TArachnodeDAO>>> crawlRequests = new Dictionary<string, List<CrawlRequest<TArachnodeDAO>>>();

                        CrawlRequest<TArachnodeDAO> crawlRequest = Cache.UncrawledCrawlRequests.Dequeue();

                        while (crawlRequest != null)
                        {
                            string domain = UserDefinedFunctions.ExtractDomain(crawlRequest.Discovery.Uri.AbsoluteUri).Value;

                            if (!crawlRequests.ContainsKey(domain))
                            {
                                crawlRequests.Add(domain, new List<CrawlRequest<TArachnodeDAO>>());
                            }

                            crawlRequests[domain].Add(crawlRequest);

                            crawlRequest = Cache.UncrawledCrawlRequests.Dequeue();
                        }

                        /**/

                        bool areCrawlRequestsAssigned = false;

                        while (!areCrawlRequestsAssigned)
                        {
                            areCrawlRequestsAssigned = true;

                            //TODO:  This sort/filter could be a bit faster, but at the expense of possibly copying the collection.  There are a relatively low number of domain that will be the Keys, as opposed to the potentially high number of CrawlRequests in the actual collection...
                            //sort such that the least recently accessed domains are enqueued first... (no Politeness means never seen before...)
                            foreach (KeyValuePair<string, List<CrawlRequest<TArachnodeDAO>>> keyValuePair in crawlRequests.Where(cr => !Cache.Politenesses.ContainsKey(cr.Key)))
                            {
                                if (keyValuePair.Value.Count != 0)
                                {
                                    Cache.UncrawledCrawlRequests.Enqueue(keyValuePair.Value[0], keyValuePair.Value[0].Priority);

                                    keyValuePair.Value.RemoveAt(0);

                                    areCrawlRequestsAssigned = false;
                                }
                            }
                        }

                        /**/

                        areCrawlRequestsAssigned = false;

                        while (!areCrawlRequestsAssigned)
                        {
                            areCrawlRequestsAssigned = true;

                            //TODO:  This sort/filter could be a bit faster, but at the expense of possibly copying the collection.  There are a relatively low number of domain that will be the Keys, as opposed to the potentially high number of CrawlRequests in the actual collection...
                            //sort such that the least recently accessed domains are enqueued first...
                            foreach (KeyValuePair<string, List<CrawlRequest<TArachnodeDAO>>> keyValuePair in crawlRequests.Where(cr => cr.Value.Count != 0).OrderBy(cr => Cache.Politenesses[cr.Key].LastHttpWebRequestRequested))
                            {
                                if (keyValuePair.Value.Count != 0)
                                {
                                    Cache.UncrawledCrawlRequests.Enqueue(keyValuePair.Value[0], keyValuePair.Value[0].Priority);

                                    keyValuePair.Value.RemoveAt(0);

                                    areCrawlRequestsAssigned = false;
                                }
                            }
                        }

                        /**/

                        int uncrawledCrawlRequests = 0;
                        int minimumCrawlDepth = int.MaxValue;

                        if (_crawler.CrawlMode == CrawlMode.BreadthFirstByPriority)
                        {
                            foreach (Crawl<TArachnodeDAO> crawl in Crawls.Values)
                            {
                                uncrawledCrawlRequests += crawl.UncrawledCrawlRequests.Count;

                                if (crawl.CrawlInfo.MaximumCrawlDepth < minimumCrawlDepth)
                                {
                                    minimumCrawlDepth = crawl.CrawlInfo.MaximumCrawlDepth;
                                }
                            }

                            int minimumCrawlRequestDepth = int.MaxValue;

                            if (uncrawledCrawlRequests == 0)
                            {
                                if (Cache.UncrawledCrawlRequests.Count != 0)
                                {
                                    foreach (PriorityQueueItem<CrawlRequest<TArachnodeDAO>> priorityQueueItem in Cache.UncrawledCrawlRequests)
                                    {
                                        if (priorityQueueItem.Value.CurrentDepth < minimumCrawlRequestDepth)
                                        {
                                            minimumCrawlRequestDepth = priorityQueueItem.Value.CurrentDepth;
                                        }
                                    }
                                }
                                else
                                {
                                    //nothing to crawl yet... reset....
                                    //HACK: The linkedin.com crawl...  :S
                                    //minimumCrawlRequestDepth = 1;
                                }

                                minimumCrawlDepth = minimumCrawlRequestDepth;

                                foreach (Crawl<TArachnodeDAO> crawl in Crawls.Values)
                                {
                                    crawl.CrawlInfo.MaximumCrawlDepth = minimumCrawlRequestDepth;
                                }
                            }
                        }

                        List<CrawlRequest<TArachnodeDAO>> crawlRequestsToReAddToCache = new List<CrawlRequest<TArachnodeDAO>>(totalEnqueuedCrawlRequests);

                        foreach (Crawl<TArachnodeDAO> crawl in Crawls.Values)
                        {
                            int numberOfCrawlRequestsToEnqueue = (totalEnqueuedCrawlRequests / Crawls.Count) - crawl.UncrawledCrawlRequests.Count;

                            for (int i = 0; i < numberOfCrawlRequestsToEnqueue; i++)
                            {
                                if (Cache.UncrawledCrawlRequests.Count != 0)
                                {
                                    crawlRequest = Cache.UncrawledCrawlRequests.Dequeue();

                                    if (!RuleManager.IsDisallowed(crawlRequest, CrawlRuleType.PreRequest, _arachnodeDAO))
                                    {
                                        if (crawlRequest.Discovery.HttpWebRequestRetriesRemaining != ApplicationSettings.HttpWebRequestRetries)
                                        {
                                            if (CrawlRequestRetrying != null)
                                            {
                                                CrawlRequestRetrying(crawlRequest);
                                            }
                                        }

                                        if (crawlRequest.CurrentDepth <= minimumCrawlDepth)
                                        {
                                            if (_crawler.CrawlMode == CrawlMode.BreadthFirstByPriority)
                                            {
                                                crawl.UncrawledCrawlRequests.Enqueue(crawlRequest, -crawlRequest.CurrentDepth);
                                            }
                                            else
                                            {
                                                crawl.UncrawledCrawlRequests.Enqueue(crawlRequest, crawlRequest.Priority);
                                            }
                                        }
                                        else
                                        {
                                            crawlRequestsToReAddToCache.Add(crawlRequest);
                                        }
                                    }
                                    else
                                    {
                                        _arachnodeDAO.DeleteCrawlRequest(crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri);

                                        if (ApplicationSettings.InsertDisallowedAbsoluteUris)
                                        {
                                            _arachnodeDAO.InsertDisallowedAbsoluteUri(crawlRequest.DataType.ContentTypeID, (int)crawlRequest.DataType.DiscoveryType, crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri, crawlRequest.IsDisallowedReason, ApplicationSettings.ClassifyAbsoluteUris);
                                        }

                                        ConsoleManager.OutputIsDisallowedReason(crawlRequest);
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }

                        while (Cache.UncrawledCrawlRequests.Count != 0)
                        {
                            crawlRequest = Cache.UncrawledCrawlRequests.Dequeue();

                            if (!RuleManager.IsDisallowed(crawlRequest, CrawlRuleType.PreRequest, _arachnodeDAO))
                            {
                                if (crawlRequest.Discovery.HttpWebRequestRetriesRemaining != ApplicationSettings.HttpWebRequestRetries)
                                {
                                    if (CrawlRequestRetrying != null)
                                    {
                                        CrawlRequestRetrying(crawlRequest);
                                    }
                                }

                                if (crawlRequest.CurrentDepth <= minimumCrawlDepth)
                                {
                                    if (_crawler.CrawlMode == CrawlMode.BreadthFirstByPriority)
                                    {
                                        Crawls[_random.Next(1, Crawls.Count)].UncrawledCrawlRequests.Enqueue(crawlRequest, -crawlRequest.CurrentDepth);
                                    }
                                    else
                                    {
                                        Crawls[_random.Next(1, Crawls.Count)].UncrawledCrawlRequests.Enqueue(crawlRequest, crawlRequest.Priority);
                                    }
                                }
                                else
                                {
                                    crawlRequestsToReAddToCache.Add(crawlRequest);
                                }
                            }
                            else
                            {
                                if (ApplicationSettings.InsertDisallowedAbsoluteUris)
                                {
                                    _arachnodeDAO.InsertDisallowedAbsoluteUri(crawlRequest.DataType.ContentTypeID, (int)crawlRequest.DataType.DiscoveryType, crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri, crawlRequest.IsDisallowedReason, ApplicationSettings.ClassifyAbsoluteUris);
                                }

                                _arachnodeDAO.DeleteCrawlRequest(crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri);
                            }
                        }

                        foreach (CrawlRequest<TArachnodeDAO> crawlRequest2 in crawlRequestsToReAddToCache)
                        {
                            Cache.UncrawledCrawlRequests.Enqueue(crawlRequest2, crawlRequest2.Priority);
                        }

                        totalEnqueuedCrawlRequests = 0;

                        foreach (Crawl<TArachnodeDAO> crawl in Crawls.Values)
                        {
                            totalEnqueuedCrawlRequests += crawl.UncrawledCrawlRequests.Count;
                        }

                        ConsoleManager.OutputString("Engine: AssignCrawlRequestsToCrawls: Total.UncrawledCrawlRequests: " + totalEnqueuedCrawlRequests + ".", ConsoleColor.White, ConsoleColor.Gray);

                        Cache.ManagePolitenesses();
                    }
                }
            }

            return totalEnqueuedCrawlRequests;
        }

        internal void OnCrawlRequestCanceled(CrawlRequest<TArachnodeDAO> crawlRequest)
        {
            if (CrawlRequestCanceled != null)
            {
                if (crawlRequest != null)
                {
                    CrawlRequestCanceled(crawlRequest);
                }
            }
        }
        
        internal void OnCrawlRequestCompleted(CrawlRequest<TArachnodeDAO> crawlRequest)
        {
            switch (crawlRequest.Discovery.DiscoveryType)
            {
                case DiscoveryType.File:
                    crawlRequest.Crawl.Crawler.NumberOfFilesCrawled++;
                    break;
                case DiscoveryType.Image:
                    crawlRequest.Crawl.Crawler.NumberOfImagesCrawled++;
                    break;
                case DiscoveryType.WebPage:
                    crawlRequest.Crawl.Crawler.NumberOfWebPagesCrawled++;
                    break;
            }

            if (CrawlRequestCompleted != null)
            {
                if (crawlRequest != null)
                {
                    CrawlRequestCompleted(crawlRequest);
                }
            }
        }

        internal void OnCrawlRequestThrottled(CrawlRequest<TArachnodeDAO> crawlRequest)
        {
            if (CrawlRequestThrottled != null)
            {
                if (crawlRequest != null)
                {
                    CrawlRequestThrottled(crawlRequest);
                }
            }
        }

        internal void OnCrawlCompleted()
        {
            if (_crawler.Engine.CrawlCompleted != null)
            {
                if (_crawler != null)
                {
                    _crawler.Engine.CrawlCompleted(_crawler.Engine);
                }
            }
        }

        /// <summary>
        /// 	Determines if crawl is finished.
        /// </summary>
        private bool IsCrawlFinished()
        {
            foreach (Crawl<TArachnodeDAO> crawl in Crawls.Values)
            {
                if (crawl.UncrawledCrawlRequests.Count != 0 || crawl.CrawlInfo.CurrentCrawlRequest != null || crawl.IsProcessingDiscoveriesAsynchronously)
                {
                    return false;
                }
            }

            foreach (DiscoveryProcessor<TArachnodeDAO> discoveryProcessor in DiscoveryProcessors.Values)
            {
                if (discoveryProcessor.IsAddingCrawlRequestToBeProcessed || discoveryProcessor.IsProcessingDiscoveries)
                {
                    return false;
                }
            }

            if (Cache.UncrawledCrawlRequests.Count == 0)
            {
                //then, check all CrawlerPeers...
                if (_crawler.CrawlerPeerManager.CrawlerPeers != null)
                {
                    bool hasAdditionalMessages = false;
                    int uncrawledCrawlRequests = 0;

                    foreach (CrawlerPeer crawlerPeer in _crawler.CrawlerPeerManager.CrawlerPeers)
                    {
                        ConsoleManager.OutputString("CrawlerPeer: " + crawlerPeer.IPEndPoint + " UncrawledCrawlRequests: " + crawlerPeer.UncrawledCrawlRequests);

                        hasAdditionalMessages |= crawlerPeer.HasAdditionalMessages;
                        uncrawledCrawlRequests += crawlerPeer.UncrawledCrawlRequests;
                    }

                    if (!hasAdditionalMessages && uncrawledCrawlRequests == 0)
                    {
                        return true;
                    }

                    return false;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 	Starts the Engine and the Crawl threads.
        /// </summary>
        public void Start()
        {
            lock (_startStopPauseResumeLock)
            {
                if (_engineState == EngineState.Stop)
                {
                    _stopwatch.Reset();
                    _stopwatch.Start();

                    new EventLog("Application", Environment.MachineName, "arachnode.net").WriteEntry("The arachnode.net Engine is starting.", EventLogEntryType.Information);

                    ConfigurationManager.CheckForIncorrectConfigurationValues(_applicationSettings);

                    _engineState = EngineState.Start;

                    ConsoleManager.OutputString("Engine: Start", ConsoleColor.White, ConsoleColor.Gray);

                    //these need to be preloaded else extensions and schemes will not be available at first crawl request...
                    ConsoleManager.OutputString("Engine: Refreshing Disallowed", ConsoleColor.White, ConsoleColor.Gray);
                    UserDefinedFunctions.RefreshDisallowed();

                    ConsoleManager.OutputString("Engine: Refreshing Allowed Extensions", ConsoleColor.White, ConsoleColor.Gray);
                    UserDefinedFunctions.RefreshAllowedExtensions(true);

                    ConsoleManager.OutputString("Engine: Refreshing Allowed Schemes", ConsoleColor.White, ConsoleColor.Gray);
                    UserDefinedFunctions.RefreshAllowedSchemes(true);

                    try
                    {
                        ConsoleManager.OutputString("Engine: Instantiating DataTypes", ConsoleColor.White, ConsoleColor.Gray);

                        DataTypeManager.RefreshDataTypes();

                        ConsoleManager.OutputString("Engine: Updating Priorities/Strengths", ConsoleColor.White, ConsoleColor.Gray);

                        ReportingManager.Update();

                        ConsoleManager.OutputString("Engine: Instantiating CrawlRules", ConsoleColor.White, ConsoleColor.Gray);

                        RuleManager.ProcessCrawlRules(_crawler);

                        ConsoleManager.OutputString("Engine: Instantiating CrawlActions", ConsoleColor.White, ConsoleColor.Gray);

                        ActionManager.ProcessCrawlActions(_crawler);

                        ConsoleManager.OutputString("Engine: Instantiating EngineActions", ConsoleColor.White, ConsoleColor.Gray);

                        ActionManager.ProcessEngineActions(_crawler);

                        ConsoleManager.OutputString("Engine: Creating Crawl Threads", ConsoleColor.White, ConsoleColor.Gray);

                        CreateWorkerThreads();

                        _startTime = DateTime.Now;

                        foreach (KeyValuePair<int, Crawl<TArachnodeDAO>> keyValuePair in Crawls)
                        {
                            keyValuePair.Value.Thread.Start(keyValuePair.Key);
                        }

                        foreach (KeyValuePair<int, DiscoveryProcessor<TArachnodeDAO>> keyValuePair in DiscoveryProcessors)
                        {
                            keyValuePair.Value.Thread.Start(keyValuePair.Key);
                        }

                        new EventLog("Application", Environment.MachineName, "arachnode.net").WriteEntry("The arachnode.net Engine has started.", EventLogEntryType.Information);

                        ConsoleManager.OutputString("Engine: Started", ConsoleColor.White, ConsoleColor.Gray);
                    }
                    catch (Exception exception)
                    {
                        _crawler.ProcessException(exception);
                    }
                }
            }
        }

        /// <summary>
        /// 	Stops the Engine and Crawl threads.
        /// 	All remaining CrawlRequests and existing Discoveries will be saved to the database tables 'CrawlRequests' and 'Discoveries'.
        /// </summary>
        public void Stop()
        {
            Stop(true);
        }

        /// <summary>
        /// 	Stops the Engine and the Crawl threads.  If called by the Engine, a Crawl has completed, and all Discoveries will be deleted from the database.
        /// </summary>
        private void Stop(bool saveDiscoveriesToDatabase)
        {
            lock (_startStopPauseResumeLock)
            {
                if (_engineState == EngineState.Start || _engineState == EngineState.Pause)
                {
                    new EventLog("Application", Environment.MachineName, "arachnode.net").WriteEntry("The arachnode.net Engine is stopping.", EventLogEntryType.Information);

                    _engineState = EngineState.Stop;

                    ConsoleManager.OutputString("Engine: Stop", ConsoleColor.White, ConsoleColor.Gray);

                    try
                    {
                        foreach (KeyValuePair<int, Crawl<TArachnodeDAO>> keyValuePair in Crawls)
                        {
                            DateTime startTime = DateTime.Now;

                            //wait for the Crawl to finish processing the CurrentCrawlRequest...
                            while (keyValuePair.Value.CrawlInfo.CurrentCrawlRequest != null)
                            {
                                try
                                {
                                    _crawler.CrawlerPeerManager.SendCrawlRequestResponseMessageToCrawlerPeers(keyValuePair.Value.CrawlInfo.CurrentCrawlRequest, keyValuePair.Value.ArachnodeDAO);

                                    if (keyValuePair.Value.WebClient.HttpWebRequest != null)
                                    {
                                        keyValuePair.Value.WebClient.HttpWebRequest.Abort();
                                    }
                                    if (keyValuePair.Value.WebClient.HttpWebResponse != null)
                                    {
                                        keyValuePair.Value.WebClient.HttpWebResponse.Close();
                                    }
                                }
                                catch
                                {
                                }

                                ConsoleManager.OutputString("Engine: Waiting for Crawl " + keyValuePair.Value.CrawlInfo.ThreadNumber + " to complete. " + DateTime.Now, ConsoleColor.White, ConsoleColor.White);
                                Thread.Sleep(1000);

                                if (DateTime.Now.Subtract(startTime).TotalMinutes >= ApplicationSettings.CrawlRequestTimeoutInMinutes)
                                {
                                    break;
                                }
                            }

#if !DEMO
                            //Save the remaining CrawlRequests to the Database...
                            keyValuePair.Value.SaveCrawlRequestsToDatabase();

                            keyValuePair.Value.CrawlInfo.CrawlState = CrawlState.Stop;
#else
                            while (keyValuePair.Value.UncrawledCrawlRequests.Count != 0)
                            {
                                keyValuePair.Value.UncrawledCrawlRequests.Dequeue();
                            }
#endif
                            if (_renderer_RendererMessageQueues.ContainsKey(keyValuePair.Value.CrawlInfo.ThreadNumber))
                            {
                                RendererMessage rendererMessage = new RendererMessage();

                                rendererMessage.Kill = true;

                                _renderer_RendererMessageQueues[keyValuePair.Value.CrawlInfo.ThreadNumber].Send(rendererMessage);
                            }
                        }

                        foreach (Process process in Process.GetProcessesByName("Arachnode.Renderer"))
                        {
                            try
                            {
                                process.Kill();
                            }
                            catch
                            {
                            }
                        }

                        //insert all cached CrawlRequests
                        while (Cache.UncrawledCrawlRequests.Count != 0)
                        {
#if !DEMO
                            ConsoleManager.OutputString("Engine: Saving Cache.UncrawledCrawlRequests: " + Cache.UncrawledCrawlRequests.Count + " CrawlRequests remaining to be dequeued.", ConsoleColor.White, ConsoleColor.Gray);
#endif

                            lock (Cache.UncrawledCrawlRequestsLock)
                            {
                                CrawlRequest<TArachnodeDAO> crawlRequest = Cache.UncrawledCrawlRequests.Dequeue();
#if !DEMO
                                if (!RuleManager.IsDisallowed(crawlRequest, CrawlRuleType.PreRequest, _arachnodeDAO))
                                {
                                    _crawler.CrawlerPeerManager.SendCrawlRequestResponseMessageToCrawlerPeers(crawlRequest, _arachnodeDAO);

                                    if (crawlRequest.Originator != null)
                                    {
                                        if (ApplicationSettings.InsertCrawlRequests)
                                        {
                                            _arachnodeDAO.InsertCrawlRequest(crawlRequest.Created, crawlRequest.Originator.Uri.AbsoluteUri, crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri, crawlRequest.CurrentDepth, crawlRequest.MaximumDepth, crawlRequest.RestrictCrawlTo, crawlRequest.RestrictDiscoveriesTo, crawlRequest.Priority, (byte) crawlRequest.RenderType, (byte) crawlRequest.RenderTypeForChildren);
                                        }
                                    }
                                    else
                                    {
                                        if (ApplicationSettings.InsertCrawlRequests)
                                        {
                                            _arachnodeDAO.InsertCrawlRequest(crawlRequest.Created, null, crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri, crawlRequest.CurrentDepth, crawlRequest.MaximumDepth, crawlRequest.RestrictCrawlTo, crawlRequest.RestrictDiscoveriesTo, crawlRequest.Priority, (byte) crawlRequest.RenderType, (byte) crawlRequest.RenderTypeForChildren);
                                        }
                                    }
                                }
                                else
                                {
                                    if (ApplicationSettings.InsertDisallowedAbsoluteUris)
                                    {
                                        _arachnodeDAO.InsertDisallowedAbsoluteUri(crawlRequest.DataType.ContentTypeID, (int)crawlRequest.DataType.DiscoveryType, crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri, crawlRequest.IsDisallowedReason, ApplicationSettings.ClassifyAbsoluteUris);
                                    }
                                }
#endif
                                Counters.GetInstance().CrawlRequestRemoved();
                            }
                        }

                        ConsoleManager.OutputString("Engine: Stopping Actions", ConsoleColor.White, ConsoleColor.Gray);

                        ActionManager.Stop();

                        ConsoleManager.OutputString("Engine: Stopping Rules", ConsoleColor.White, ConsoleColor.Gray);

                        RuleManager.Stop();

                        _crawler.CrawlerPeerManager.StopServer(false, _arachnodeDAO);

                        ConsoleManager.OutputString("Engine: Stopping CrawlerPeerManager Server", ConsoleColor.White, ConsoleColor.Gray);

#if DEMO
                        saveDiscoveriesToDatabase = false;
#endif

                        if (saveDiscoveriesToDatabase)
                        {
                            //Cache.SaveDiscoveriesToDatabase();
                        }
                        else
                        {
                            _crawler.ClearDiscoveries();
                        }

                        GC.Collect();
                        GC.WaitForFullGCComplete();
                        GC.WaitForPendingFinalizers();

                        _stopTime = DateTime.Now;

                        new EventLog("Application", Environment.MachineName, "arachnode.net").WriteEntry("The arachnode.net Engine has stopped.", EventLogEntryType.Information);

                        ConsoleManager.OutputString("Engine: CrawlStopped", ConsoleColor.White, ConsoleColor.Gray);

                        if (_crawler.Engine.CrawlStopped != null)
                        {
                            _crawler.Engine.CrawlStopped(_crawler.Engine);
                        }

                        if (IsCrawlFinished())
                        {
                            ConsoleManager.OutputString("Engine: CrawlCompleted", ConsoleColor.White, ConsoleColor.Gray);

                            if (_crawler.Engine.CrawlCompleted != null)
                            {
                                _crawler.Engine.CrawlCompleted(_crawler.Engine);
                            }
                        }

                        ConsoleManager.OutputString("Engine: Stopped", ConsoleColor.White, ConsoleColor.Gray);
                    }
                    catch (Exception exception)
                    {
                        _crawler.ProcessException(exception);
                    }
                    finally
                    {
                        _stopwatch.Stop();

                        ConsoleManager.OutputString("\nElapsed: " + _stopwatch.Elapsed, ConsoleColor.White, ConsoleColor.White);
                        ConsoleManager.OutputString("Files: " + _crawler.NumberOfFilesCrawled + " | " + (_crawler.NumberOfFilesCrawled / _stopwatch.Elapsed.TotalSeconds) + " Files/sec.", ConsoleColor.White, ConsoleColor.White);
                        ConsoleManager.OutputString("Images: " + _crawler.NumberOfImagesCrawled + " | " + (_crawler.NumberOfImagesCrawled / _stopwatch.Elapsed.TotalSeconds) + " Images/sec.", ConsoleColor.White, ConsoleColor.White);
                        ConsoleManager.OutputString("WebPages: " + _crawler.NumberOfWebPagesCrawled + " | " + (_crawler.NumberOfWebPagesCrawled / _stopwatch.Elapsed.TotalSeconds) + " WebPages/sec.", ConsoleColor.White, ConsoleColor.White);
                        ConsoleManager.OutputString("Discoveries: " + (_crawler.NumberOfFilesCrawled + _crawler.NumberOfImagesCrawled + _crawler.NumberOfWebPagesCrawled) + " | " + ((_crawler.NumberOfFilesCrawled + _crawler.NumberOfImagesCrawled + _crawler.NumberOfWebPagesCrawled) / _stopwatch.Elapsed.TotalSeconds) + " Discoveries/sec.", ConsoleColor.White, ConsoleColor.White);
#if DEMO
            ConsoleManager.OutputString("Crawl rate is limited by DEMO protection as well as web server restrictions.\nCrawl the test site for a maximum performance benchmark.");
#endif
                        ConsoleManager.OutputString("\nPress any key to terminate arachnode.net", ConsoleColor.White, ConsoleColor.Gray);
#if DEMO
            ConsoleManager.OutputString("\nSet 'Web' as the 'Startup Project' and examine 'Search.aspx' to view results.");
#endif

                    }
                }
            }
        }

        /// <summary>
        /// 	Pauses the Engine and the Crawl threads.
        /// </summary>
        public void Pause()
        {
            lock (_startStopPauseResumeLock)
            {
                new EventLog("Application", Environment.MachineName, "arachnode.net").WriteEntry("The arachnode.net Engine is pausing.", EventLogEntryType.Information);

                _engineState = EngineState.Pause;

                for (int i = 0; i < Crawls.Count; i++)
                {
                    Crawl<TArachnodeDAO> crawl = Crawls[i + 1];

                    if (crawl.CrawlInfo.CrawlState != CrawlState.Pause)
                    {
                        try
                        {
                            if (crawl.WebClient.HttpWebRequest != null)
                            {
                                crawl.WebClient.HttpWebRequest.Abort();
                            }
                            if (crawl.WebClient.HttpWebResponse != null)
                            {
                                crawl.WebClient.HttpWebResponse.Close();
                            }
                        }
                        catch
                        {
                        }

                        if (crawl.CrawlInfo.CrawlState != CrawlState.Stop)
                        {
                            i--;
                        }
                    }
                }

                new EventLog("Application", Environment.MachineName, "arachnode.net").WriteEntry("The arachnode.net Engine has paused.", EventLogEntryType.Information);

                ConsoleManager.OutputString("Engine: Pause", ConsoleColor.White, ConsoleColor.Gray);

                _stateControl.Reset();

                _pauseTime = DateTime.Now;
            }

            _stopwatch.Stop();
        }

        /// <summary>
        /// 	Resumes the Engine and the Crawl threads.
        /// </summary>
        public void Resume()
        {
            _stopwatch.Start();

            lock (_startStopPauseResumeLock)
            {
                new EventLog("Application", Environment.MachineName, "arachnode.net").WriteEntry("The arachnode.net Engine is resuming.", EventLogEntryType.Information);

                ConfigurationManager.CheckForIncorrectConfigurationValues(_applicationSettings);

                _engineState = EngineState.Start;

                new EventLog("Application", Environment.MachineName, "arachnode.net").WriteEntry("The arachnode.net Engine has resumed.", EventLogEntryType.Information);

                ConsoleManager.OutputString("Engine: Resume", ConsoleColor.White, ConsoleColor.Gray);

                _stateControl.Set();

                _resumeTime = DateTime.Now;
            }
        }
    }
}