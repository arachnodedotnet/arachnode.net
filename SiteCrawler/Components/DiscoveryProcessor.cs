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
using System.Threading;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Managers;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.AbstractClasses;
using Arachnode.SiteCrawler.Value.Enums;

#endregion

namespace Arachnode.SiteCrawler.Components
{
    internal class DiscoveryProcessor<TArachnodeDAO> : AWorker<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        private ApplicationSettings _applicationSettings = new ApplicationSettings();

        private readonly object _crawlRequestLock = new object();

        private readonly Crawler<TArachnodeDAO> _crawler;
        private readonly CrawlRequestManager<TArachnodeDAO> _crawlRequestManager;
        private readonly IArachnodeDAO _arachnodeDAO;

        private Queue<CrawlRequest<TArachnodeDAO>> _unprocessedCrawlRequests = new Queue<CrawlRequest<TArachnodeDAO>>();

        internal DiscoveryProcessor(ApplicationSettings applicationSettings, Crawler<TArachnodeDAO> crawler, CrawlRequestManager<TArachnodeDAO> crawlRequestManager)
        {
            _applicationSettings = applicationSettings;

            _crawler = crawler;
            _crawlRequestManager = crawlRequestManager;

            _arachnodeDAO = (TArachnodeDAO)Activator.CreateInstance(typeof(TArachnodeDAO), _applicationSettings.ConnectionString);
            _arachnodeDAO.ApplicationSettings = _applicationSettings;
        }

        internal bool IsAddingCrawlRequestToBeProcessed { get; set; }
        internal bool IsProcessingDiscoveries { get; set; }

        internal Queue<CrawlRequest<TArachnodeDAO>> UnprocessedCrawlRequests
        {
            get { return _unprocessedCrawlRequests; }
            set { _unprocessedCrawlRequests = value; }
        }

        internal void AddCrawlRequestToBeProcessed(CrawlRequest<TArachnodeDAO> crawlRequest)
        {
            IsAddingCrawlRequestToBeProcessed = true;

            lock (_crawlRequestLock)
            {
                UnprocessedCrawlRequests.Enqueue(crawlRequest);
            }

            IsAddingCrawlRequestToBeProcessed = false;
        }

        internal void BeginDiscoveryProcessor(object o)
        {
            while (_crawler.Engine.State == EngineState.Start || _crawler.Engine.State == EngineState.Pause || _crawler.Engine.State == EngineState.None)
            {
                if (_crawler.Engine.State == EngineState.Start)
                {
                    _crawler.Engine.StateControl.WaitOne();

                    lock (_crawlRequestLock)
                    {
                        IsProcessingDiscoveries = true;

                        while (UnprocessedCrawlRequests.Count != 0)
                        {
                            CrawlRequest<TArachnodeDAO> crawlRequest = UnprocessedCrawlRequests.Dequeue();

                            crawlRequest.Crawl.IsProcessingDiscoveriesAsynchronously = true;

                            try
                            {
                                _crawlRequestManager.ProcessDiscoveries(crawlRequest, _arachnodeDAO);
                            }
                            catch (Exception exception)
                            {
                                _arachnodeDAO.InsertException(crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri, exception, false);
                            }

                            crawlRequest.Crawl.IsProcessingDiscoveriesAsynchronously = false;
                        }

                        IsProcessingDiscoveries = false;
                    }
                }

                Thread.Sleep(5);
            }

            IsProcessingDiscoveries = false;
        }
    }
}