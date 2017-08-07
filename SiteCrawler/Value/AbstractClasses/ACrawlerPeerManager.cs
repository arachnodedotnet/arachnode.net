using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Arachnode.Configuration;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Core;
using Arachnode.SiteCrawler.Managers;
using Arachnode.SiteCrawler.Value.Enums;

namespace Arachnode.SiteCrawler.Value.AbstractClasses
{
    public abstract class ACrawlerPeerManager<TArachnodeDAO> : AManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        protected object _arachnodeDAOLock = new object();

        protected Crawler<TArachnodeDAO> _crawler;
        protected IArachnodeDAO _arachnodeDAO;
        
        protected bool _executeServer;

        protected ACrawlerPeerManager(ApplicationSettings applicationSettings, WebSettings webSettings, List<CrawlerPeer> crawlerPeers, IArachnodeDAO arachnodeDAO) : base(applicationSettings, webSettings)
        {
            CrawlerPeers = crawlerPeers;

            _arachnodeDAO = arachnodeDAO;
        }

        protected CrawlerPeerManagerState _crawlerPeerManagerState;
        protected int _messageID;
        protected int _crawlGenerationID = 1;

        public abstract List<CrawlerPeer> CrawlerPeers { get; protected set; }

        internal abstract void StartServer(Crawler<TArachnodeDAO> crawler, IArachnodeDAO arachnodeDAO);
        internal abstract void StopServer(bool executeServer, IArachnodeDAO arachnodeDAO);
        public abstract void SendStatusMessageToCrawlerPeers(IArachnodeDAO arachnodeDAO);
        public abstract void SendDiscoveryMessageToCrawlerPeers(string absoluteUri, IArachnodeDAO arachnodeDAO);
        public abstract void SendCrawlRequestRequestMessageToCrawlerPeers(IArachnodeDAO arachnodeDAO);
        public abstract void SendCrawlRequestResponseMessageToCrawlerPeers(CrawlRequest<TArachnodeDAO> crawlRequest, IArachnodeDAO arachnodeDAO);
        public abstract void SendCrawlRequestResponseMessageToCrawlerPeer(CrawlerPeer crawlerPeer, CrawlRequest<TArachnodeDAO> crawlRequest, IArachnodeDAO arachnodeDAO);
    }
}