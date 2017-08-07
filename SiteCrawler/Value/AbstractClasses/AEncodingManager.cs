using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Arachnode.Configuration;
using Arachnode.DataAccess.Value.Interfaces;

namespace Arachnode.SiteCrawler.Value.AbstractClasses
{
    public abstract class AEncodingManager<TArachnodeDAO> : AManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        protected AEncodingManager(ApplicationSettings applicationSettings, WebSettings webSettings) : base(applicationSettings, webSettings)
        {
            
        }

        protected readonly Regex _charsetRegex = new Regex("charset\\s*=\\s*(?<Charset>[^\"\';>]*)\\s*[\"\']?\\s*/?\\s*>", RegexOptions.Compiled);
        public abstract void ProcessCrawlRequest(CrawlRequest<TArachnodeDAO> crawlRequest, IArachnodeDAO arachnodeDAO);
    }
}
