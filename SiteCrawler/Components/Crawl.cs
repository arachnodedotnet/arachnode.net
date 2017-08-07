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
using System.Diagnostics;
using System.Net;
using System.Threading;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.Performance;
using Arachnode.SiteCrawler.Managers;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.AbstractClasses;
using Arachnode.SiteCrawler.Value.Enums;
using Arachnode.Structures;
using Newtonsoft.Json;

#endregion

namespace Arachnode.SiteCrawler.Components
{
    public class Crawl<TArachnodeDAO> : AWorker<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        private ApplicationSettings _applicationSettings;
        private WebSettings _webSettings;

        private ActionManager<TArachnodeDAO> _actionManager;
        private ConsoleManager<TArachnodeDAO> _consoleManager;
        private CookieManager _cookieManager;
        private CrawlRequestManager<TArachnodeDAO> _crawlRequestManager;
        private DataTypeManager<TArachnodeDAO> _dataTypeManager;
        private DiscoveryManager<TArachnodeDAO> _discoveryManager;
        private EncodingManager<TArachnodeDAO> _encodingManager;
        private HtmlManager<TArachnodeDAO> _htmlManager;
        private PolitenessManager<TArachnodeDAO> _politenessManager;
        private ProxyManager<TArachnodeDAO> _proxyManager;
        private RuleManager<TArachnodeDAO> _ruleManager;

        private readonly TArachnodeDAO _arachnodeDAO;

        private readonly object _beginCrawlLock = new object();
        private readonly CrawlInfo<TArachnodeDAO> _crawlInfo = new CrawlInfo<TArachnodeDAO>();
        private readonly Crawler<TArachnodeDAO> _crawler;

        private readonly DataManager<TArachnodeDAO> _dataManager;
        private readonly FileManager<TArachnodeDAO> _fileManager;
        private readonly ImageManager<TArachnodeDAO> _imageManager;

        private readonly bool _processData;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly WebClient<TArachnodeDAO> _webClient;
        private readonly WebPageManager<TArachnodeDAO> _webPageManager;

        /// <summary>
        /// 	Initializes a new instance of the <see cref = "Crawl" /> class.
        /// </summary>
        /// <param name = "crawler">The crawler.</param>
        /// <param name="actionManager"></param>
        /// <param name="crawlRequestManager"></param>
        /// <param name="discoveryManager"></param>
        /// <param name="htmlManager"></param>
        /// <param name="politenessManager"></param>
        /// <param name="ruleManager"></param>
        /// <param name = "processData">if set to <c>true</c> [process data].</param>
        public Crawl(ApplicationSettings applicationSettings, WebSettings webSettings, Crawler<TArachnodeDAO> crawler, ActionManager<TArachnodeDAO> actionManager, ConsoleManager<TArachnodeDAO> consoleManager, CookieManager cookieManager, CrawlRequestManager<TArachnodeDAO> crawlRequestManager, DataTypeManager<TArachnodeDAO> dataTypeManager, DiscoveryManager<TArachnodeDAO> discoveryManager, EncodingManager<TArachnodeDAO> encodingManager, HtmlManager<TArachnodeDAO> htmlManager, PolitenessManager<TArachnodeDAO> politenessManager, ProxyManager<TArachnodeDAO> proxyManager, RuleManager<TArachnodeDAO> ruleManager, bool processData)
        {
            _applicationSettings = applicationSettings;
            _webSettings = webSettings;

            UncrawledCrawlRequests = new PriorityQueue<CrawlRequest<TArachnodeDAO>>();
            UnassignedDiscoveries = new HashSet<string>();

            _crawler = crawler;

            _crawlInfo.MaximumCrawlDepth = 1;

            _actionManager = actionManager;
            _consoleManager = consoleManager;
            _cookieManager = cookieManager;
            _crawlRequestManager = crawlRequestManager;
            _dataTypeManager = dataTypeManager;
            _discoveryManager = discoveryManager;
            _encodingManager = encodingManager;
            _htmlManager = htmlManager;
            _politenessManager = politenessManager;
            _proxyManager = proxyManager;
            _ruleManager = ruleManager;

            _processData = processData;

            _arachnodeDAO = (TArachnodeDAO)Activator.CreateInstance(typeof(TArachnodeDAO), _applicationSettings.ConnectionString, _applicationSettings, _webSettings, false, false);
            _arachnodeDAO.ApplicationSettings = applicationSettings;

            //_arachnodeDAO.OpenCommandConnections();

            _dataManager = new DataManager<TArachnodeDAO>(_applicationSettings, _webSettings, _actionManager, _dataTypeManager, _discoveryManager, _ruleManager, _arachnodeDAO);
            _fileManager = new FileManager<TArachnodeDAO>(_applicationSettings, _webSettings, _discoveryManager, _arachnodeDAO);
            _imageManager = new ImageManager<TArachnodeDAO>(_applicationSettings, _webSettings, _discoveryManager, _arachnodeDAO);
            _webClient = new WebClient<TArachnodeDAO>(_applicationSettings, _webSettings, _consoleManager, _cookieManager, _proxyManager);
            _webPageManager = new WebPageManager<TArachnodeDAO>(_applicationSettings, _webSettings, _discoveryManager, _htmlManager, _arachnodeDAO);
        }

        /// <summary>
        /// 	Used to allow asynchronous processing of Discoveries while the Crawl retrieves the next Discovery (File, Image or WebPage).
        /// </summary>
        public bool IsProcessingDiscoveriesAsynchronously { get; internal set; }

        /// <summary>
        /// 	Gets the partner Crawl.  Each Crawl has a partner that it can take CrawlRequests from should the Crawl become stuck on a WebPage
        /// 	that is slow to respond, or that is streaming large volumes of content...
        /// </summary>
        /// <value>The crawler.</value>
        [JsonIgnore]
        public Crawl<TArachnodeDAO> Partner { get; set; }

        /// <summary>
        /// 	Gets the crawler.
        /// </summary>
        /// <value>The crawler.</value>
        public Crawler<TArachnodeDAO> Crawler
        {
            get { return _crawler; }
        }

        /// <summary>
        /// 	Gets the crawl info.
        /// </summary>
        /// <value>The crawl info.</value>
        public CrawlInfo<TArachnodeDAO> CrawlInfo
        {
            get { return _crawlInfo; }
        }

        internal IArachnodeDAO ArachnodeDAO
        {
            get { return _arachnodeDAO; }
        }

        /// <summary>
        /// 	Gets the uncrawled crawl requests.
        /// </summary>
        /// <value>The uncrawled crawl requests.</value>
        internal PriorityQueue<CrawlRequest<TArachnodeDAO>> UncrawledCrawlRequests { get; private set; }

        /// <summary>
        /// 	Gets or sets the unassigned discoveries.
        /// </summary>
        /// <value>The unassigned discoveries.</value>
        internal HashSet<string> UnassignedDiscoveries { get; private set; }

        internal WebClient<TArachnodeDAO> WebClient
        {
            get { return _webClient; }
        }

        /// <summary>
        /// 	Begins the crawl.
        /// </summary>
        /// <param name = "o">The o.</param>
        internal void BeginCrawl(object o)
        {
            _crawlInfo.CrawlState = CrawlState.Start;

            _crawlInfo.ThreadNumber = (int) o;

            while (Crawler.Engine.State == EngineState.Start || Crawler.Engine.State == EngineState.Pause || Crawler.Engine.State == EngineState.None)
            {
                _crawlInfo.CrawlState = CrawlState.Pause;

                if (Crawler.Engine.State == EngineState.Start)
                {
                    Crawler.Engine.StateControl.WaitOne();

                    _crawlInfo.CrawlState = CrawlState.Start;

                    ProcessCrawlRequests();

                    Crawler.Engine.PopulateCrawlCrawlRequests(this, _arachnodeDAO);

                    if (UncrawledCrawlRequests.Count != 0)
                    {
                        CrawlInfo.TotalCrawlFedCount++;
                        CrawlInfo.TotalCrawlRequestsAssigned += UncrawledCrawlRequests.Count;
                    }
                    else
                    {
                        CrawlInfo.TotalCrawlStarvedCount++;
                    }
                }

                if (_crawlInfo.CrawlState == CrawlState.Stop)
                {
                    break;
                }

                Thread.Sleep(100);
            }

            _crawlInfo.CrawlState = CrawlState.Stop;
        }

        /// <summary>
        /// 	Begins a Crawl.  This method bypasses the Cache, and is experimental/for advanced users.
        /// 	This method does not function with the DEMO version.
        /// </summary>
        /// <param name = "crawlRequest"></param>
        /// <param name = "obeyCrawlRules"></param>
        /// <param name = "executeCrawlActions"></param>
        public void BeginCrawl(CrawlRequest<TArachnodeDAO> crawlRequest, bool obeyCrawlRules, bool executeCrawlActions, bool processDiscoveriesAsynchronously)
        {
#if DEMO
            return;
#endif
            _crawlInfo.ThreadNumber = -1;

            do
            {
                crawlRequest.Crawl = this;

                crawlRequest.Crawl.IsProcessingDiscoveriesAsynchronously = !processDiscoveriesAsynchronously;
                crawlRequest.CurrentDepth = crawlRequest.MaximumDepth;

                lock (_beginCrawlLock)
                {
                    ProcessCrawlRequest(crawlRequest, obeyCrawlRules, executeCrawlActions);

                    crawlRequest = UncrawledCrawlRequests.Dequeue();
                }
            } while (crawlRequest != null);
        }

        /// <summary>
        /// 	Processes the crawl requests.
        /// </summary>
        private void ProcessCrawlRequests()
        {
            while (Crawler.Engine.State == EngineState.Start && UncrawledCrawlRequests.Count != 0)
            {
                _crawlInfo.CurrentCrawlRequest = UncrawledCrawlRequests.Dequeue();
                _crawlInfo.CurrentCrawlRequest.Crawl = this;

                if (_crawlInfo.CurrentCrawlRequest != null)
                {
                    _crawlInfo.EnqueuedCrawlRequests = UncrawledCrawlRequests.Count;

                    if (_crawlInfo.CurrentCrawlRequest.CurrentDepth > _crawlInfo.MaximumCrawlDepth)
                    {
                        _crawlInfo.MaximumCrawlDepth = _crawlInfo.CurrentCrawlRequest.CurrentDepth;
                    }

                    ProcessCrawlRequest(_crawlInfo.CurrentCrawlRequest, true, true);

                    if (_crawlInfo.TotalCrawlRequestsProcessed % 10 == 0)
                    {
                        _crawler.CrawlerPeerManager.SendStatusMessageToCrawlerPeers(_arachnodeDAO);
                    }
                }
            }

            _crawlInfo.CurrentCrawlRequest = null;

            Thread.Sleep(100);
        }

        /// <summary>
        /// 	Processes the crawl request.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "obeyCrawlRules">if set to <c>true</c> [obey crawl rules].</param>
        /// <param name = "executeCrawlActions">if set to <c>true</c> [execute crawl actions].</param>
        public void ProcessCrawlRequest(CrawlRequest<TArachnodeDAO> crawlRequest, bool obeyCrawlRules, bool executeCrawlActions)
        {
            //HACK:!!!  Solve this!!!
//#if DEMO
//            return;
//#endif

            bool wasACacheHit = false;

            try
            {
                crawlRequest.WebClient = WebClient;

                if (crawlRequest.Discovery.DiscoveryState == DiscoveryState.Undiscovered)
                {
                    if (!_politenessManager.ManagePoliteness(crawlRequest, PolitenessState.HttpWebRequestRequested, _arachnodeDAO))
                    {
                        Crawler.Engine.OnCrawlRequestThrottled(crawlRequest);

                        return;
                    }

                    _consoleManager.OutputProcessCrawlRequest(_crawlInfo.ThreadNumber, crawlRequest);

                    _discoveryManager.ManageDiscovery(crawlRequest, DiscoveryState.PreRequest, _arachnodeDAO);

                    if (obeyCrawlRules)
                    {
                        _ruleManager.IsDisallowed(crawlRequest, CrawlRuleType.PreRequest, _arachnodeDAO);
                    }

                    if (executeCrawlActions)
                    {
                        _actionManager.PerformCrawlActions(crawlRequest, CrawlActionType.PreRequest, _arachnodeDAO);
                    }

                    if (!crawlRequest.IsDisallowed)
                    {
                        _stopwatch.Reset();
                        _stopwatch.Start();

                        try
                        {
                            _dataManager.ProcessCrawlRequest(crawlRequest, obeyCrawlRules, executeCrawlActions);
                        }
                        catch (Exception exception2)
                        {
                            throw new Exception(exception2.Message, exception2);
                        }
                        finally
                        {
                            _stopwatch.Stop();

                            _crawlInfo.TotalHttpWebResponseTime += _stopwatch.Elapsed;
                            crawlRequest.HttpWebResponseTime = _stopwatch.Elapsed;

                            _politenessManager.ManagePoliteness(crawlRequest, PolitenessState.HttpWebRequestCompleted, _arachnodeDAO);
                        }

                        Counters.GetInstance().TotalBytesDiscovered(crawlRequest.Data.LongLength);

                        _discoveryManager.ManageDiscovery(crawlRequest, DiscoveryState.PostRequest, _arachnodeDAO);

                        _encodingManager.ProcessCrawlRequest(crawlRequest, _arachnodeDAO);

                        if (obeyCrawlRules)
                        {
                            _ruleManager.IsDisallowed(crawlRequest, CrawlRuleType.PostRequest, _arachnodeDAO);
                        }

                        //the CrawlRequest could be Disallowed by a PreGet CrawlRule - specifically DataType.cs.
                        if (!crawlRequest.IsDisallowed)
                        {
                            if (_processData)
                            {
                                _crawlRequestManager.ProcessCrawlRequest(crawlRequest, _fileManager, _imageManager, _webPageManager, _arachnodeDAO);
                            }
                        }
                        else
                        {
                            if (crawlRequest.DataType.ContentType == null)
                            {
                                crawlRequest.DataType = _dataTypeManager.DetermineDataType(crawlRequest);
                            }

                            if (_applicationSettings.InsertDisallowedAbsoluteUris)
                            {
                                _arachnodeDAO.InsertDisallowedAbsoluteUri(crawlRequest.DataType.ContentTypeID, (int)crawlRequest.DataType.DiscoveryType, crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri, crawlRequest.IsDisallowedReason, _applicationSettings.ClassifyAbsoluteUris);
                            }

                            _consoleManager.OutputIsDisallowedReason(_crawlInfo, crawlRequest);
                        }
                    }
                    else
                    {
                        _politenessManager.ManagePoliteness(crawlRequest, PolitenessState.HttpWebRequestCompleted, _arachnodeDAO);

                        if (crawlRequest.DataType.ContentType == null)
                        {
                            crawlRequest.DataType = _dataTypeManager.DetermineDataType(crawlRequest);
                        }

                        if (_applicationSettings.InsertDisallowedAbsoluteUris)
                        {
                            _arachnodeDAO.InsertDisallowedAbsoluteUri(crawlRequest.DataType.ContentTypeID, (int)crawlRequest.DataType.DiscoveryType, crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri, crawlRequest.IsDisallowedReason, _applicationSettings.ClassifyAbsoluteUris);
                        }

                        _consoleManager.OutputIsDisallowedReason(_crawlInfo, crawlRequest);
                    }
                }
                else
                {
                    wasACacheHit = true;

                    //this should only occur when you submit a CR from a rule, or action...
                    _consoleManager.OutputCacheHit(_crawlInfo, crawlRequest, crawlRequest.Discovery);
                }
            }
            catch (Exception exception)
            {
                _stopwatch.Stop();

                if (Crawler.Engine.State != EngineState.Start)
                {
                    //the request was aborted as it was long running and Engine was requested to Stop.
                    if ((crawlRequest.WebClient.WebException != null && crawlRequest.WebClient.WebException.Status == WebExceptionStatus.RequestCanceled) || (exception.InnerException != null && exception.InnerException.Message == "The request was aborted: The request was canceled."))
                    {
                        return;
                    }
                }

                if (crawlRequest.WebClient.WebException != null && crawlRequest.Discovery.HttpWebRequestRetriesRemaining != 0 && crawlRequest.WebClient.WebException.Message.StartsWith("Unable to connect to the remote server"))
                {
                    _politenessManager.ResubmitCrawlRequest(crawlRequest, false, _arachnodeDAO);

                    _politenessManager.ManagePoliteness(crawlRequest, PolitenessState.HttpWebRequestCanceled, _arachnodeDAO);

                    return;
                }

                try
                {
                    _politenessManager.ManagePoliteness(crawlRequest, PolitenessState.HttpWebRequestCompleted, _arachnodeDAO);
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                }

                if (exception.InnerException == null)
                {
                    _arachnodeDAO.InsertException(crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri, exception, false);
                }
                else
                {
                    _arachnodeDAO.InsertException(crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri, exception.InnerException, false);
                }

                crawlRequest.DataType = _dataTypeManager.DetermineDataType(crawlRequest);

                if (_applicationSettings.InsertDisallowedAbsoluteUris)
                {
                    if (crawlRequest.Discovery.DiscoveryState == DiscoveryState.Undiscovered)
                    {
                        _arachnodeDAO.InsertDisallowedAbsoluteUri(crawlRequest.DataType.ContentTypeID, (int)crawlRequest.DataType.DiscoveryType, crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri, exception.Message, _applicationSettings.ClassifyAbsoluteUris);
                    }
                    else
                    {
                        if (_applicationSettings.InsertDisallowedAbsoluteUriDiscoveries)
                        {
                            _arachnodeDAO.InsertDisallowedAbsoluteUriDiscovery(crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri);
                        }
                    }
                }

                _consoleManager.OutputException(_crawlInfo.ThreadNumber, crawlRequest, _arachnodeDAO.LastExceptionID, _arachnodeDAO.LastExceptionMessage);
            }

            if (crawlRequest.IsFromDatabase)
            {
                _arachnodeDAO.DeleteCrawlRequest(crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri);
            }

            _discoveryManager.ManageDiscovery(crawlRequest, DiscoveryState.Discovered, _arachnodeDAO);

            if (!wasACacheHit)
            {
                if (executeCrawlActions)
                {
                    _actionManager.PerformCrawlActions(crawlRequest, CrawlActionType.PostRequest, _arachnodeDAO);
                }

                Crawler.Engine.OnCrawlRequestCompleted(crawlRequest);
            }

            _consoleManager.OutputProcessCrawlRequest(_crawlInfo.ThreadNumber, crawlRequest);

            Counters.GetInstance().ReportCurrentDepth(crawlRequest.CurrentDepth);

            Counters.GetInstance().CrawlRequestRemoved();

            Counters.GetInstance().CrawlRequestProcessed();

            _crawlInfo.TotalCrawlRequestsProcessed++;
        }

        /// <summary>
        /// 	Saves the crawl requests to database.
        /// </summary>
        internal void SaveCrawlRequestsToDatabase()
        {
            while (UncrawledCrawlRequests.Count != 0)
            {
                _consoleManager.OutputString("Saving Crawl.UncrawledCrawlRequests: " + _crawlInfo.ThreadNumber + " : " + UncrawledCrawlRequests.Count + " CrawlRequests remaining to be inserted.", ConsoleColor.Gray, ConsoleColor.Gray);

                CrawlRequest<TArachnodeDAO> crawlRequest = UncrawledCrawlRequests.Dequeue();

                if (!_ruleManager.IsDisallowed(crawlRequest, CrawlRuleType.PreRequest, _arachnodeDAO))
                {
                    if (crawlRequest.Originator != null)
                    {
                        if (_applicationSettings.InsertCrawlRequests)
                        {
                            _arachnodeDAO.InsertCrawlRequest(crawlRequest.Created, crawlRequest.Originator.Uri.AbsoluteUri, crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri, crawlRequest.CurrentDepth, crawlRequest.MaximumDepth, crawlRequest.RestrictCrawlTo, crawlRequest.RestrictDiscoveriesTo, crawlRequest.Priority, (byte) crawlRequest.RenderType, (byte) crawlRequest.RenderTypeForChildren);
                        }
                    }
                    else
                    {
                        if (_applicationSettings.InsertCrawlRequests)
                        {
                            _arachnodeDAO.InsertCrawlRequest(crawlRequest.Created, null, crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri, crawlRequest.CurrentDepth, crawlRequest.MaximumDepth, crawlRequest.RestrictCrawlTo, crawlRequest.RestrictDiscoveriesTo, crawlRequest.Priority, (byte) crawlRequest.RenderType, (byte) crawlRequest.RenderTypeForChildren);
                        }
                    }
                }
                else
                {
                    if (_applicationSettings.InsertDisallowedAbsoluteUris)
                    {
                        _arachnodeDAO.InsertDisallowedAbsoluteUri(crawlRequest.DataType.ContentTypeID, (int)crawlRequest.DataType.DiscoveryType, crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri, crawlRequest.IsDisallowedReason, _applicationSettings.ClassifyAbsoluteUris);
                    }
                }

                Counters.GetInstance().CrawlRequestRemoved();
            }
        }
    }
}