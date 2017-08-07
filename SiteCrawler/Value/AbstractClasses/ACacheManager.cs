using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arachnode.Configuration;
using Arachnode.DataAccess.Value.Interfaces;

namespace Arachnode.SiteCrawler.Value.AbstractClasses
{
    public abstract class ACacheManager<TArachnodeDAO> : AManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        protected ACacheManager(ApplicationSettings applicationSettings, WebSettings webSettings) : base(applicationSettings, webSettings)
        {
            
        }

        public abstract string GetCacheKey(string absoluteUri);
    }
}
