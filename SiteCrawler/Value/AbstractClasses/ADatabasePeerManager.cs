using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;

namespace Arachnode.SiteCrawler.Value.AbstractClasses
{
    public abstract class ADatabasePeerManager<TArachnodeDAO> : AManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        private Crawler<TArachnodeDAO> _crawler;

        protected ADatabasePeerManager(ApplicationSettings applicationSettings, WebSettings webSettings, List<DatabasePeer> databasePeers) : base(applicationSettings, webSettings)
        {
            if (databasePeers == null)
            {
                return;
            }

            DatabasePeers = databasePeers;

            foreach (DatabasePeer databasePeer in DatabasePeers)
            {
                databasePeer.ArachnodeDAO = (TArachnodeDAO)Activator.CreateInstance(typeof(TArachnodeDAO), applicationSettings.ConnectionString, true, true);
                databasePeer.ArachnodeDAO.ApplicationSettings = applicationSettings;
            }
        }

        public abstract List<DatabasePeer> DatabasePeers { get; set; }
    }
}
