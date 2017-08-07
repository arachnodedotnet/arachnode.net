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
using System.Data.SqlTypes;
using System.Web;
using System.Web.Caching;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.DataSource;
using Arachnode.Performance;
using Arachnode.SiteCrawler.Managers;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.Enums;
using Arachnode.Structures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#endregion

namespace Arachnode.SiteCrawler.Core
{
    /// <summary>
    /// 	Provides caching mechanisms for Discoveries and Politeness.
    /// </summary>
    public class Cache<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        //private static readonly object _discoveriesLock = new object();

        private ApplicationSettings _applicationSettings;
        private WebSettings _webSettings;

        private Crawler<TArachnodeDAO> _crawler;

        private ActionManager<TArachnodeDAO> _actionManager;
        private readonly CacheManager<TArachnodeDAO> _cacheManager;
        private readonly CrawlerPeerManager<TArachnodeDAO> _crawlerPeerManager;
        private MemoryManager<TArachnodeDAO> _memoryManager;
        private RuleManager<TArachnodeDAO> _ruleManager;

        private readonly Dictionary<string, Politeness> _politenesses = new Dictionary<string, Politeness>();
        private readonly object _politenessesLock = new object();
        private readonly PriorityQueue<CrawlRequest<TArachnodeDAO>> _uncrawledCrawlRequests = new PriorityQueue<CrawlRequest<TArachnodeDAO>>();
        private readonly object _uncrawledCrawlRequestsLock = new object();

        private readonly CacheItemRemovedCallback _cacheItemRemovedCallback;
        private Dictionary<string, Discovery<TArachnodeDAO>> _discoveries = new Dictionary<string, Discovery<TArachnodeDAO>>();
        private int _databaseCrawlRequestCreatedHelper;

        public Cache(ApplicationSettings applicationSettings, WebSettings webSettings, Crawler<TArachnodeDAO> crawler, ActionManager<TArachnodeDAO> actionManager, CacheManager<TArachnodeDAO> cacheManager, CrawlerPeerManager<TArachnodeDAO> crawlerPeerManager, MemoryManager<TArachnodeDAO> memoryManager, RuleManager<TArachnodeDAO> ruleManager)
        {
            _applicationSettings = applicationSettings;
            _webSettings = webSettings;

            _crawler = crawler;

            _actionManager = actionManager;
            _cacheManager = cacheManager;
            _crawlerPeerManager = crawlerPeerManager;
            _memoryManager = memoryManager;
            _ruleManager = ruleManager;

            _cacheItemRemovedCallback = CacheItemRemoved;
        }

        /// <summary>
        /// 	Gets the uncrawled CrawlRequests lock.
        /// </summary>
        /// <value>The uncrawled CrawlRequests lock.</value>
        public object UncrawledCrawlRequestsLock
        {
            get { return _uncrawledCrawlRequestsLock; }
        }

        /// <summary>
        /// 	Gets the Discoveries.
        /// </summary>
        /// <value>The discoveries.</value>
        public Dictionary<string, Discovery<TArachnodeDAO>> Discoveries
        {
            get { return _discoveries; }
            private set { _discoveries = value; }
        }

        /// <summary>
        /// 	Gets the uncrawled crawl requests.
        /// </summary>
        /// <value>The uncrawled crawl requests.</value>
        public PriorityQueue<CrawlRequest<TArachnodeDAO>> UncrawledCrawlRequests
        {
            get { return _uncrawledCrawlRequests; }
        }

        /// <summary>
        /// 	Gets the politenesses.
        /// </summary>
        /// <value>The politenesses.</value>
        public Dictionary<string, Politeness> Politenesses
        {
            get { return _politenesses; }
        }

        /// <summary>
        /// 	Adds a Politeness to the Cache with the sliding expiration specified by TimeSpan.FromDays(1).
        /// </summary>
        /// <param name = "politeness">The Politeness to be added.</param>
        public void AddPoliteness(Politeness politeness)
        {
            try
            {
                lock (_politenessesLock)
                {
                    if (!Politenesses.ContainsKey(politeness.Host))
                    {
                        Politenesses.Add(politeness.Host, politeness);

                        Counters.GetInstance().PolitenessAdded();
                    }
                }
            }
            catch (Exception)
            {   
                throw;
            }
        }

        /// <summary>
        /// 	Returns a Politeness addressable by a Host.
        /// </summary>
        /// <param name = "host">The Host governed by the Politeness.</param>
        /// <returns>
        /// 	The Politeness governing the AbsoluteUri.
        /// </returns>
        public Politeness GetPoliteness(string host)
        {
            Politeness politeness = null;

            Politenesses.TryGetValue(host, out politeness);

            return politeness;
        }

        /// <summary>
        /// 	Gets the discovery.
        /// </summary>
        /// <param name = "absoluteUri">The file or image discovery.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        /// <returns></returns>
        public Discovery<TArachnodeDAO> GetDiscovery(string absoluteUri, IArachnodeDAO arachnodeDAO)
        {
            string cacheKey = _cacheManager.GetCacheKey(absoluteUri);

            Discovery<TArachnodeDAO> discovery;

            //this is a placeholder, if my memory serves me correctly, to expand the referenced functionality...
            if (_memoryManager.HasDesiredMaximumMemoryUsageInMegabytesEverBeenMet)
            {
                discovery = GetDiscovery(absoluteUri, cacheKey, arachnodeDAO);
            }
            else
            {
                discovery = GetDiscovery(absoluteUri, cacheKey, arachnodeDAO);
            }

            if (discovery.Uri.AbsoluteUri != absoluteUri)
            {
                discovery.Uri = new Uri(absoluteUri);
            }

            return discovery;
        }

        /// <summary>
        /// 	Gets the discovery.
        /// </summary>
        /// <param name = "uri">The file or image discovery.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        /// <returns></returns>
        public Discovery<TArachnodeDAO> GetDiscovery(Uri uri, IArachnodeDAO arachnodeDAO)
        {
            return GetDiscovery(uri.AbsoluteUri, arachnodeDAO);
        }

        private Discovery<TArachnodeDAO> GetDiscovery(string absoluteUri, string cacheKey, IArachnodeDAO arachnodeDAO)
        {
            Discovery<TArachnodeDAO> discovery = null;

            try
            {
                object o = HttpRuntime.Cache.Get(cacheKey);

                if (o != null && o is Discovery<TArachnodeDAO>)
                {
                    discovery = (Discovery<TArachnodeDAO>) o;

                    discovery.NumberOfTimesDiscovered++;
                }
                else
                {
                    //check the database...
                    ArachnodeDataSet.DiscoveriesRow discoveriesRow = arachnodeDAO.GetDiscovery(cacheKey);

                    if (discoveriesRow != null)
                    {
                        discovery = new Discovery<TArachnodeDAO>(discoveriesRow);
                        discovery.HttpWebRequestRetriesRemaining = _applicationSettings.HttpWebRequestRetries;

                        discovery.DiscoveryState = (DiscoveryState) discoveriesRow.DiscoveryStateID;
                        discovery.DiscoveryType = (DiscoveryType) discoveriesRow.DiscoveryTypeID;
                        discovery.ExpectFileOrImage = discoveriesRow.ExpectFileOrImage;
                        if (!discoveriesRow.IsIDNull())
                        {
                            discovery.ID = discoveriesRow.ID;
                        }
                        discovery.NumberOfTimesDiscovered = discoveriesRow.NumberOfTimesDiscovered;
                        discovery.Uri = new Uri(absoluteUri);

                        AddDiscoveryToInternalCache(cacheKey, discovery);
                    }
                    else
                    {
                        //check the CrawlerPeers...
                        if (_crawlerPeerManager != null)
                        {
                            if (_crawlerPeerManager.GetDiscovery(absoluteUri, cacheKey, arachnodeDAO))
                            {
                                discovery = new Discovery<TArachnodeDAO>(absoluteUri);

                                discovery.DiscoveryState = DiscoveryState.Discovered;
                                discovery.IsNew = false;
                            }
                        }

                        if (discovery == null)
                        {
                            discovery = new Discovery<TArachnodeDAO>(absoluteUri);

                            discovery.IsNew = true;

                            AddDiscoveryToInternalCache(cacheKey, discovery);
                        }

                        if (_applicationSettings.InsertDisallowedDiscoveries || !_ruleManager.IsDisallowed(discovery, CrawlRuleType.PreRequest, arachnodeDAO))
                        {
                            if (_applicationSettings.InsertDiscoveries && discovery.InsertDiscovery)
                            {
                                arachnodeDAO.InsertDiscovery(discovery.ID, cacheKey, (int) discovery.DiscoveryState, (int) discovery.DiscoveryType, discovery.ExpectFileOrImage, ++discovery.NumberOfTimesDiscovered);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                arachnodeDAO.InsertException(absoluteUri, absoluteUri, exception, false);

                discovery = new Discovery<TArachnodeDAO>("http://aninvalidabsoluteuriwasrequestedasadiscovery.com");

                discovery.IsNew = true;
            }

            return discovery;
        }

        internal void AddDiscoveryToInternalCache(string cacheKey, Discovery<TArachnodeDAO> discovery)
        {
            if (!_memoryManager.IsUsingDesiredMaximumMemoryInMegabytes(false))
            {
                HttpRuntime.Cache.Remove(cacheKey);

                HttpRuntime.Cache.Add(cacheKey, discovery, null, DateTime.MaxValue, TimeSpan.FromSeconds(_applicationSettings.DiscoverySlidingExpirationInSeconds), CacheItemPriority.Normal, _cacheItemRemovedCallback);

                Counters.GetInstance().DiscoveryAdded();
            }
        }

        private void CacheItemRemoved(string cacheKey, object o, CacheItemRemovedReason cacheItemRemovedReason)
        {
            if (o is Discovery<TArachnodeDAO>)
            {
                Counters.GetInstance().DiscoveryRemoved();

                if (Engine<TArachnodeDAO>.IsPopulatingCrawlCrawlRequests)
                {
                    //AddDiscoveryToInternalCache(cacheKey, (Discovery<TArachnodeDAO>)o, true);
                }
            }
        }

        public void ClearDiscoveries()
        {
            while (HttpRuntime.Cache.Count != 0)
            {
                //safety for user actions calling this method when the Engine is not stopped.
                Engine<TArachnodeDAO>.IsPopulatingCrawlCrawlRequests = false;

                List<string> keys = new List<string>();

                IDictionaryEnumerator enumerator = HttpRuntime.Cache.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    if (string.IsNullOrEmpty(_applicationSettings.UniqueIdentifier) || enumerator.Key.ToString().EndsWith(_applicationSettings.UniqueIdentifier))
                    {
                        keys.Add(enumerator.Key.ToString());
                    }
                }

                for (int i = 0; i < keys.Count; i++)
                {
                    HttpRuntime.Cache.Remove(keys[i]);
                }
            }
        }

        /// <summary>
        /// 	Adds the crawl request to be crawled.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        /// <returns></returns>
        public bool AddCrawlRequestToBeCrawled(CrawlRequest<TArachnodeDAO> crawlRequest, bool isCrawlRequestFromDatabaseOrFromCrawlerPeer, bool insertCrawlRequestIntoDatabase, IArachnodeDAO arachnodeDAO)
        {
            return AddCrawlRequestToBeCrawled(UncrawledCrawlRequests, crawlRequest, isCrawlRequestFromDatabaseOrFromCrawlerPeer, insertCrawlRequestIntoDatabase, arachnodeDAO, true);
        }

        /// <summary>
        /// 	When used publicly, this method is used to insert CrawlRequests into the main CrawlRequest PriorityQueue.
        /// 	When used internally, this method is used by Crawls to insert Files and Images into their own PriorityQueues.
        /// 	When crawling, Crawls prioritize Files and Images above HyperLinks.
        /// 	This method allows the Cache to manage the Discovery associated with the CrawlRequest.
        /// </summary>
        /// <param name = "priorityQueue">The priority queue.</param>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        /// <param name = "lockPriorityQueue">Crawls won't need to lock access their own PriorityQueues.</param>
        /// <returns>Was the CrawlRequest added to the PriorityQueue to be crawled?</returns>
        public bool AddCrawlRequestToBeCrawled(PriorityQueue<CrawlRequest<TArachnodeDAO>> priorityQueue, CrawlRequest<TArachnodeDAO> crawlRequest, bool isCrawlRequestFromDatabaseOrFromCrawlerPeer, bool insertCrawlRequestIntoDatabase, IArachnodeDAO arachnodeDAO, bool lockPriorityQueue)
        {
            Discovery<TArachnodeDAO> discovery = GetDiscovery(crawlRequest.Discovery.Uri.AbsoluteUri, arachnodeDAO);

            //crawlRequest.Discovery = discovery;

            if (discovery.IsNew || crawlRequest.Discovery.IsNew || isCrawlRequestFromDatabaseOrFromCrawlerPeer)
            {
                discovery.IsNew = false;
                crawlRequest.Discovery.IsNew = false;

                try
                {
                    if (!_memoryManager.IsUsingDesiredMaximumMemoryInMegabytes(true))
                    {
                        AddCrawlRequestToPriorityQueue(priorityQueue, crawlRequest, lockPriorityQueue);

                        if (insertCrawlRequestIntoDatabase)
                        {
                            InsertCrawlRequestIntoDatabase(crawlRequest, arachnodeDAO);
                        }
                    }
                    else
                    {
                        //if the PriorityQueue is the main HyperLink queue || the PriorityQueue belongs to a Crawl, and the Counts are appropriate then...
                        if ( /*ArachnodeDAO.NumberOfWebPagesInserted < _applicationSettings.MaximumNumberOfCrawlRequestsToCreatePerBatch || */(priorityQueue == UncrawledCrawlRequests && UncrawledCrawlRequests.Count < _applicationSettings.MaximumNumberOfCrawlRequestsToCreatePerBatch) || (priorityQueue != UncrawledCrawlRequests && priorityQueue.Count < _applicationSettings.MaximumNumberOfCrawlRequestsToCreatePerBatch/_applicationSettings.MaximumNumberOfCrawlThreads))
                        {
                            AddCrawlRequestToPriorityQueue(priorityQueue, crawlRequest, lockPriorityQueue);

                            if (insertCrawlRequestIntoDatabase)
                            {
                                InsertCrawlRequestIntoDatabase(crawlRequest, arachnodeDAO);
                            }
                        }
                        else
                        {
                            InsertCrawlRequestIntoDatabase(crawlRequest, arachnodeDAO);
                        }
                    }

                    return true;
                }
                catch (Exception exception)
                {
                    arachnodeDAO.InsertException(crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri, exception, false);
                }
            }

            return false;
        }

        private void InsertCrawlRequestIntoDatabase(CrawlRequest<TArachnodeDAO> crawlRequest, IArachnodeDAO arachnodeDAO)
        {
            if (crawlRequest.Originator != null)
            {
//ANOEDT: Could be improved... images from msn should be processed before images from joescrabshack.com.  TEST TEST TEST!!
                if (crawlRequest.Discovery.ExpectFileOrImage)
                {
                    if (_applicationSettings.InsertCrawlRequests)
                    {
                        arachnodeDAO.InsertCrawlRequest(SqlDateTime.MinValue.Value.AddSeconds(_databaseCrawlRequestCreatedHelper), crawlRequest.Originator.Uri.AbsoluteUri + _applicationSettings.UniqueIdentifier, crawlRequest.Parent.Uri.AbsoluteUri + _applicationSettings.UniqueIdentifier, crawlRequest.Discovery.Uri.AbsoluteUri + _applicationSettings.UniqueIdentifier, crawlRequest.CurrentDepth, crawlRequest.MaximumDepth, crawlRequest.RestrictCrawlTo, crawlRequest.RestrictDiscoveriesTo, crawlRequest.Priority + 1000000, (byte) crawlRequest.RenderType, (byte) crawlRequest.RenderTypeForChildren);
                    }
                }
                else
                {
                    if (_applicationSettings.InsertCrawlRequests)
                    {
                        arachnodeDAO.InsertCrawlRequest(crawlRequest.Created, crawlRequest.Originator.Uri.AbsoluteUri + _applicationSettings.UniqueIdentifier, crawlRequest.Parent.Uri.AbsoluteUri + _applicationSettings.UniqueIdentifier, crawlRequest.Discovery.Uri.AbsoluteUri + _applicationSettings.UniqueIdentifier, crawlRequest.CurrentDepth, crawlRequest.MaximumDepth, crawlRequest.RestrictCrawlTo, crawlRequest.RestrictDiscoveriesTo, crawlRequest.Priority, (byte) crawlRequest.RenderType, (byte) crawlRequest.RenderTypeForChildren);
                    }
                }
            }
            else
            {
                if (crawlRequest.Discovery.ExpectFileOrImage)
                {
                    if (_applicationSettings.InsertCrawlRequests)
                    {
                        arachnodeDAO.InsertCrawlRequest(SqlDateTime.MinValue.Value.AddSeconds(_databaseCrawlRequestCreatedHelper), null, crawlRequest.Parent.Uri.AbsoluteUri + _applicationSettings.UniqueIdentifier, crawlRequest.Discovery.Uri.AbsoluteUri + _applicationSettings.UniqueIdentifier, crawlRequest.CurrentDepth, crawlRequest.MaximumDepth, crawlRequest.RestrictCrawlTo, crawlRequest.RestrictDiscoveriesTo, crawlRequest.Priority + 1000000, (byte) crawlRequest.RenderType, (byte) crawlRequest.RenderTypeForChildren);
                    }
                }
                else
                {
                    if (_applicationSettings.InsertCrawlRequests)
                    {
                        arachnodeDAO.InsertCrawlRequest(crawlRequest.Created, null, crawlRequest.Parent.Uri.AbsoluteUri + _applicationSettings.UniqueIdentifier, crawlRequest.Discovery.Uri.AbsoluteUri + _applicationSettings.UniqueIdentifier, crawlRequest.CurrentDepth, crawlRequest.MaximumDepth, crawlRequest.RestrictCrawlTo, crawlRequest.RestrictDiscoveriesTo, crawlRequest.Priority, (byte) crawlRequest.RenderType, (byte) crawlRequest.RenderTypeForChildren);
                    }
                }
            }

            _databaseCrawlRequestCreatedHelper += 1;
        }

        private void AddCrawlRequestToPriorityQueue(PriorityQueue<CrawlRequest<TArachnodeDAO>> priorityQueue, CrawlRequest<TArachnodeDAO> crawlRequest, bool lockPriorityQueue)
        {
            if (lockPriorityQueue)
            {
                lock (UncrawledCrawlRequestsLock)
                {
                    priorityQueue.Enqueue(crawlRequest, crawlRequest.Priority);
                }
            }
            else
            {
                priorityQueue.Enqueue(crawlRequest, crawlRequest.Priority);
            }

            Counters.GetInstance().CrawlRequestAdded();
        }

        /// <summary>
        /// 	Manages the politenesses.
        /// </summary>
        public void ManagePolitenesses()
        {
            try
            {
                lock (_politenessesLock)
                {
                    string[] hosts = new string[_politenesses.Keys.Count];

                    _politenesses.Keys.CopyTo(hosts, 0);

                    foreach (string host in hosts)
                    {
                        if (DateTime.Now.Subtract(_politenesses[host].FirstHttpWebRequest).Hours >= 24)
                        {
                            _politenesses.Remove(host);

                            Counters.GetInstance().PolitenessRemoved();
                        }
                    }
                }
            }
            catch (Exception)
            {   
                throw;
            }
        }
    }
}