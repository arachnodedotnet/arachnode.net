using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arachnode.Configuration;
using Arachnode.DataAccess.Value.Interfaces;

namespace Arachnode.SiteCrawler.Value.AbstractClasses
{
    public abstract class AMemoryManager<TArachnodeDAO> : AManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        protected AMemoryManager(ApplicationSettings applicationSettings, WebSettings webSettings) : base(applicationSettings, webSettings)
        {
            
        }

        public abstract bool HasDesiredMaximumMemoryUsageInMegabytesEverBeenMet { get; set; }

        /// <summary>
        /// 	Determines whether [is using desired maximum memory in megabytes].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is using desired maximum memory in megabytes]; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool IsUsingDesiredMaximumMemoryInMegabytes(bool isForCrawlRequestConsideration);
    }
}
