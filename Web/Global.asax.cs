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
using System.IO;
using System.Threading;
using System.Web;
using System.Web.Security;
using Arachnode.Configuration;
using Arachnode.Configuration.Value.Enums;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Managers;
using Arachnode.DataAccess.Value.Exceptions;
using Arachnode.DataAccess.Value.Interfaces;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;

#endregion

namespace Arachnode.Web
{
    public class Global : HttpApplication
    {
#pragma warning disable 612,618
        private static ApplicationSettings _applicationSettings = new ApplicationSettings();
        private static WebSettings _webSettings = new WebSettings();
        private static readonly object _arachnodeDAOLock = new object();
        private static readonly QueryParser _customQueryParser = new QueryParser(Version.LUCENE_29, "", new StandardAnalyzer(Version.LUCENE_29));
        private static readonly MultiFieldQueryParser _defaultQueryParser = new MultiFieldQueryParser(Version.LUCENE_29, new[] {"absoluteuri", "host", "text", "title"}, new StandardAnalyzer(Version.LUCENE_29));
        private static IArachnodeDAO _arachnodeDAO;
        private static DateTime _cacheRefresh;
        private static IndexSearcher _indexSearcher;

#if !RELEASE
        private static int _maximumNumberOfTestWebPageRequests = 1000000;
        private static int _numberOfTestWebPageRequests;
        private static object _testLock = new object();
#endif

        /// <summary>
        /// 	Gets the arachnode DAO.
        /// </summary>
        /// <value>The arachnode DAO.</value>
        private static IArachnodeDAO ArachnodeDAO
        {
            get
            {
                if (_arachnodeDAO == null)
                {
                    _arachnodeDAO = new ArachnodeDAO(_webSettings.ConnectionString, _applicationSettings, _webSettings, true, true);
                }

                return _arachnodeDAO;
            }
        }

        /// <summary>
        /// 	Gets the index searcher.
        /// </summary>
        /// <value>The index searcher.</value>
        internal static IndexSearcher IndexSearcher
        {
            get { return _indexSearcher; }
        }

        /// <summary>
        /// 	Gets the default query parser.
        /// </summary>
        /// <value>The default query parser.</value>
        internal static QueryParser DefaultQueryParser
        {
            get { return _defaultQueryParser; }
        }

        /// <summary>
        /// 	Gets the custom query parser.
        /// </summary>
        /// <value>The custom query parser.</value>
        internal static QueryParser CustomQueryParser
        {
            get { return _customQueryParser; }
        }

        /// <summary>
        /// 	Refreshes the index searcher.
        /// </summary>
        internal static void RefreshIndexSearcher()
        {
            try
            {
                if (_indexSearcher == null || DateTime.Now.Subtract(_cacheRefresh).Minutes >= _webSettings.CacheTimeoutInMinutes)
                {
                    _cacheRefresh = DateTime.Now;

                    _indexSearcher = new IndexSearcher(FSDirectory.Open(new DirectoryInfo(_webSettings.LuceneDotNetIndexDirectory)), true);
                }
            }
            catch (Exception exception)
            {
                if (exception.Message.Contains("no segments"))
                {
                    throw new Exception(exception.Message + "<br /><br />Solution: Set 'luceneDotNetIndexDirectory' in database table 'Configuration' to the location of the Lucene.NET indexes.  (e.g. C:\\LuceneDotNetIndex)", exception);
                }

                throw (exception);
            }
        }

        /// <summary>
        /// 	Handles the Start event of the Application control.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.EventArgs" /> instance containing the event data.</param>
        protected void Application_Start(object sender, EventArgs e)
        {
            try
            {
                ConfigurationManager.InitializeConfiguration(ref _applicationSettings, ref _webSettings, ConfigurationType.Application, ArachnodeDAO);
                ConfigurationManager.InitializeConfiguration(ref _applicationSettings, ref _webSettings, ConfigurationType.Web, ArachnodeDAO);
            }
            catch (InvalidConfigurationException invalidConfigurationException)
            {
                throw new Exception(invalidConfigurationException.Message);
            }
        }

        /// <summary>
        /// 	Handles the End event of the Application control.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.EventArgs" /> instance containing the event data.</param>
        protected void Application_End(object sender, EventArgs e)
        {
        }

#if !RELEASE
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (Request.Url.AbsoluteUri.ToLowerInvariant().Contains("http://localhost"))
            {
                if ((Request.Path.Contains(@"Test.aspx") || Request.Path.Contains(@"Test_")) && !Request.Path.Contains(@"Depth"))
                {
                    if (_numberOfTestWebPageRequests < _maximumNumberOfTestWebPageRequests)
                    {
                        Interlocked.Increment(ref _numberOfTestWebPageRequests);

                        double requestedPageNumber = 1;

                        if (Request.Path.Contains("_"))
                        {
                            requestedPageNumber = double.Parse(Request.Path.Split('_')[1].Replace(".aspx", ""));
                        }

                        HttpContext.Current.RewritePath("~/Test.aspx?numberOfTestWebPageRequests=" + _numberOfTestWebPageRequests + "&requestedTestWebPageNumber=" + requestedPageNumber);
                    }
                }
            }
        }
#endif
    }
#pragma warning restore 612,618
}