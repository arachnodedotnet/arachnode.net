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
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Caching;
using Arachnode.Configuration;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.Performance;
using Arachnode.SiteCrawler.Components;
using Arachnode.SiteCrawler.Core;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.AbstractClasses;
using Arachnode.SiteCrawler.Value.Enums;
using Newtonsoft.Json;

#endregion

namespace Arachnode.SiteCrawler.Managers
{
    public class CrawlerPeerManager<TArachnodeDAO> : ACrawlerPeerManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        private readonly CacheItemRemovedCallback _cacheItemRemovedCallback;

        public CrawlerPeerManager(ApplicationSettings applicationSettings, WebSettings webSettings, List<CrawlerPeer> crawlerPeers, IArachnodeDAO arachnodeDAO) : base(applicationSettings, webSettings, crawlerPeers, arachnodeDAO)
        {
            CrawlerPeers = crawlerPeers;

            if (CrawlerPeers == null || CrawlerPeers.Count == 0)
            {
                return;
            }

            _cacheItemRemovedCallback = CacheItemRemoved;

            /**/

            //TODO: Fix virtual member constructor call...

            _localCrawlerPeers = new HashSet<IPEndPoint>();

            foreach (CrawlerPeer crawlerPeer in CrawlerPeers.Where(_ => _.IsLocal))
            {
                _localCrawlerPeers.Add(crawlerPeer.IPEndPoint);
            }

            if (_localCrawlerPeers.Count == 0)
            {
                throw new Exception("No local CrawlerPeers were assigned.");
            }

            _initialCrawlerPeerCheckins = new HashSet<IPEndPoint>();

            _acceptingThreads = new HashSet<string>();
            _receivingThreads = new Dictionary<IPEndPoint, Thread>();
        }

        public override List<CrawlerPeer> CrawlerPeers { get; protected set; }

        private HashSet<IPEndPoint> _localCrawlerPeers;
        private bool _wasInitialCrawlerPeerCheckinsCheckMade;

        private HashSet<string> _acceptingThreads;

        private object _receivingThreadsLock = new object();
        private Dictionary<IPEndPoint, Thread> _receivingThreads;

        public bool WasInitialCrawlerPeerCheckinsCheckMade
        {
            get { return _wasInitialCrawlerPeerCheckinsCheckMade; }
            private set { _wasInitialCrawlerPeerCheckinsCheckMade = value; }
        }

        private HashSet<IPEndPoint> _initialCrawlerPeerCheckins;

        ~CrawlerPeerManager()
        {
        }

        internal override void StartServer(Crawler<TArachnodeDAO> crawler, IArachnodeDAO arachnodeDAO)
        {
            
        }

        internal override void StopServer(bool executeServer, IArachnodeDAO arachnodeDAO)
        {
            
        }

        public bool GetDiscovery(string absoluteUri, string cacheKey, IArachnodeDAO arachnodeDAO)
        {
            return false;
        }

        public override void SendStatusMessageToCrawlerPeers(IArachnodeDAO arachnodeDAO)
        {
            
        }

        public override void SendDiscoveryMessageToCrawlerPeers(string absoluteUri, IArachnodeDAO arachnodeDAO)
        {
            
        }

        public override void SendCrawlRequestRequestMessageToCrawlerPeers(IArachnodeDAO arachnodeDAO)
        {
            
        }

        public override void SendCrawlRequestResponseMessageToCrawlerPeers(CrawlRequest<TArachnodeDAO> crawlRequest, IArachnodeDAO arachnodeDAO)
        {
            
        }

        public override void SendCrawlRequestResponseMessageToCrawlerPeer(CrawlerPeer crawlerPeer, CrawlRequest<TArachnodeDAO> crawlRequest, IArachnodeDAO arachnodeDAO)
        {
            
        }

        private void SendMessageToCrawlerPeer(CrawlerPeer crawlerPeer, StringBuilder message, IArachnodeDAO arachnodeDAO)
        {
            
        }

        private void BuildMessageHeader(CrawlerPeerMessageType crawlerPeerMessageType, StringBuilder message)
        {
            
        }

        protected void AddCrawlerPeerToPrivateCache(string cacheKey, CrawlerPeer crawlerPeer)
        {
            
        }

        protected void CacheItemRemoved(string cacheKey, object o, CacheItemRemovedReason cacheItemRemovedReason)
        {
            
        }
    }
}
