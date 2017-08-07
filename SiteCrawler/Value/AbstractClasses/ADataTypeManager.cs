using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Value.Structs;

namespace Arachnode.SiteCrawler.Value.AbstractClasses
{
    public abstract class ADataTypeManager<TArachnodeDAO> : AManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        protected ADataTypeManager(ApplicationSettings applicationSettings, WebSettings webSettings): base(applicationSettings, webSettings)
        {
            _arachnodeDAO = (TArachnodeDAO) Activator.CreateInstance(typeof (TArachnodeDAO), applicationSettings.ConnectionString, applicationSettings, webSettings, false, false);
            _arachnodeDAO.ApplicationSettings = applicationSettings;
        }

        protected readonly Dictionary<string, DataType> _allowedDataTypes = new Dictionary<string, DataType>();
        protected readonly IArachnodeDAO _arachnodeDAO;
        protected readonly Dictionary<int, string> _contentTypesByID = new Dictionary<int, string>();
        protected readonly Dictionary<string, int> _contentTypesByName = new Dictionary<string, int>();

        /// <summary>
        /// 	Gets the allowed data types.
        /// </summary>
        /// <value>The allowed data types.</value>
        public abstract Dictionary<string, DataType> AllowedDataTypes { get; }

        /// <summary>
        /// 	Refreshes the data types.
        /// </summary>
        public abstract void RefreshDataTypes();

        /// <summary>
        /// 	Determines the type of the data.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <returns></returns>
        public abstract DataType DetermineDataType(CrawlRequest<TArachnodeDAO> crawlRequest);

        /// <summary>
        /// 	Determines the type of the data.
        /// </summary>
        /// <param name = "contentType">Type of the content.</param>
        /// <param name = "extension">The extension.</param>
        /// <returns></returns>
        public abstract DataType DetermineDataType(string contentType, string extension);
    }
}
