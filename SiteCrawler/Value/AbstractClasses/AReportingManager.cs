using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arachnode.Configuration;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.DataSource;
using Arachnode.SiteCrawler.Managers;

namespace Arachnode.SiteCrawler.Value.AbstractClasses
{
    public abstract class AReportingManager<TArachnodeDAO> : AManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        protected readonly Dictionary<string, double?> _priorities = new Dictionary<string, double?>();
        //HACK:  WRONG!
        protected readonly ReportingLinqToSqlDataContext _reportingLinqToSqlDataContext;
        protected readonly object _reportingManagerLock = new object();
        protected Dictionary<string, double?> _hyperLinks_MOST_POPULAR_HOSTS_BY_HOSTS = new Dictionary<string, double?>();
        protected ConsoleManager<TArachnodeDAO> _consoleManager;

        public AReportingManager(ApplicationSettings applicationSettings, WebSettings webSettings, ConsoleManager<TArachnodeDAO> consoleManager) : base(applicationSettings, webSettings)
        {
             _reportingLinqToSqlDataContext = new ReportingLinqToSqlDataContext(applicationSettings.ConnectionString);
            _consoleManager = consoleManager;
        }

        /// <summary>
        /// 	Updates reporting for use in Crawl Priority and indexing ranking/boosting.
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// 	Gets the strength for host.
        /// </summary>
        /// <param name = "absoluteUri">The absolute URI.</param>
        /// <returns></returns>
        public abstract double? GetStrengthForHost(string absoluteUri);

        /// <summary>
        /// 	Gets the priority for host.
        /// </summary>
        /// <param name = "absoluteUri">The absolute URI.</param>
        /// <returns></returns>
        public abstract double? GetPriorityForHost(string absoluteUri);
    }
}
