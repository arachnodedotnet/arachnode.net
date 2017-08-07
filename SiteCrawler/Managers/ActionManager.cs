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
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Enums;
using Arachnode.DataAccess.Value.Exceptions;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.AbstractClasses;
using Arachnode.SiteCrawler.Value.Enums;
using Arachnode.Structures;

#endregion

namespace Arachnode.SiteCrawler.Managers
{
    /// <summary>
    /// 	Provides pre- and post-request CrawlAction functionality.
    /// </summary>
    public class ActionManager<TArachnodeDAO> : AActionManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        public ActionManager(ApplicationSettings applicationSettings, WebSettings webSettings, ConsoleManager<TArachnodeDAO> consoleManager) : base(applicationSettings, webSettings, consoleManager)
        {
        }

        /// <summary>
        /// 	Processes and instantiates the CrawlActions specified by ApplicationSettings.CrawlActionsDotConfigLocation.
        /// </summary>
        public override void ProcessCrawlActions(Crawler<TArachnodeDAO> crawler)
        {
            _preRequestCrawlActions.Clear();
            _preGetCrawlActions.Clear();
            _postRequestCrawlActions.Clear();

            foreach (ACrawlAction<TArachnodeDAO> crawlAction in crawler.CrawlActions.Values)
            {
                if (crawlAction.IsEnabled)
                {
                    if (crawlAction.Settings != null)
                    {
                        Dictionary<string, string> settings = new Dictionary<string, string>();

                        foreach (string setting in crawlAction.Settings.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                        {
                            string[] keyAndValue = setting.Split("=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                            if (keyAndValue.Length == 1 || string.IsNullOrEmpty(keyAndValue[1]))
                            {
                                throw new InvalidConfigurationException(null, null, "\nThe following configuration setting is missing from database table 'cfg.CrawlActions' for CrawlAction '" + crawlAction.TypeName + "'\n\t" + keyAndValue[0], InvalidConfigurationExceptionSeverity.Error);
                            }

                            settings.Add(keyAndValue[0], keyAndValue[1]);
                        }

                        crawlAction.AssignSettings(settings);
                    }

                    switch (crawlAction.CrawlActionType)
                    {
                        case CrawlActionType.PreRequest:
                            if (!_preRequestCrawlActions.ContainsKey(crawlAction.Order))
                            {
                                _preRequestCrawlActions[crawlAction.Order] = new List<ACrawlAction<TArachnodeDAO>>();
                            }
                            _preRequestCrawlActions[crawlAction.Order].Add(crawlAction);
                            break;
                        case CrawlActionType.PreGet:
                            if (!_preGetCrawlActions.ContainsKey(crawlAction.Order))
                            {
                                _preGetCrawlActions[crawlAction.Order] = new List<ACrawlAction<TArachnodeDAO>>();
                            }
                            _preGetCrawlActions[crawlAction.Order].Add(crawlAction);
                            break;
                        case CrawlActionType.PostRequest:
                            if (!_postRequestCrawlActions.ContainsKey(crawlAction.Order))
                            {
                                _postRequestCrawlActions[crawlAction.Order] = new List<ACrawlAction<TArachnodeDAO>>();
                            }
                            _postRequestCrawlActions[crawlAction.Order].Add(crawlAction);
                            break;
                        case CrawlActionType.PreAndPostRequest:
                            if (!_preRequestCrawlActions.ContainsKey(crawlAction.Order))
                            {
                                _preRequestCrawlActions[crawlAction.Order] = new List<ACrawlAction<TArachnodeDAO>>();
                            }
                            _preRequestCrawlActions[crawlAction.Order].Add(crawlAction);
                            if (!_postRequestCrawlActions.ContainsKey(crawlAction.Order))
                            {
                                _postRequestCrawlActions[crawlAction.Order] = new List<ACrawlAction<TArachnodeDAO>>();
                            }
                            _postRequestCrawlActions[crawlAction.Order].Add(crawlAction);
                            break;
                    }
                }

                _consoleManager.OutputBehavior(crawlAction);
            }

            if (_preRequestCrawlActions.Count != 0 || _preGetCrawlActions.Count != 0 || _postRequestCrawlActions.Count != 0)
            {
                _consoleManager.OutputString("", ConsoleColor.Gray, ConsoleColor.Gray);
            }
        }

        /// <summary>
        /// 	Processes and instantiates the CrawlActions specified by ApplicationSettings.EngineActionsDotConfigLocation.
        /// </summary>
        public override void ProcessEngineActions(Crawler<TArachnodeDAO> crawler)
        {
            _preGetCrawlRequestsActions.Clear();
            _postGetCrawlRequestsActions.Clear();

            foreach (AEngineAction<TArachnodeDAO> engineAction in crawler.Engine.EngineActions.Values)
            {
                if (engineAction.IsEnabled)
                {
                    if (engineAction.Settings != null)
                    {
                        Dictionary<string, string> settings = new Dictionary<string, string>();

                        foreach (string setting in engineAction.Settings.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                        {
                            string[] keyAndValue = setting.Split("=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                            if (keyAndValue.Length == 1 || string.IsNullOrEmpty(keyAndValue[1]))
                            {
                                throw new InvalidConfigurationException(null, null, "\nThe following configuration setting is missing from database table 'cfg.EngineActions' for EngineAction '" + engineAction.TypeName + "'\n\t" + keyAndValue[0], InvalidConfigurationExceptionSeverity.Error);
                            }

                            settings.Add(keyAndValue[0], keyAndValue[1]);
                        }

                        engineAction.AssignSettings(settings);
                    }

                    switch (engineAction.EngineActionType)
                    {
                        case EngineActionType.PreGetCrawlRequests:
                            if (!_preGetCrawlRequestsActions.ContainsKey(engineAction.Order))
                            {
                                _preGetCrawlRequestsActions[engineAction.Order] = new List<AEngineAction<TArachnodeDAO>>();
                            }
                            _preGetCrawlRequestsActions[engineAction.Order].Add(engineAction);
                            break;
                        case EngineActionType.PostGetCrawlRequests:
                            if (!_postGetCrawlRequestsActions.ContainsKey(engineAction.Order))
                            {
                                _postGetCrawlRequestsActions[engineAction.Order] = new List<AEngineAction<TArachnodeDAO>>();
                            }
                            _postGetCrawlRequestsActions[engineAction.Order].Add(engineAction);
                            break;
                        case EngineActionType.PreAndPostGetCrawlRequests:
                            if (!_preGetCrawlRequestsActions.ContainsKey(engineAction.Order))
                            {
                                _preGetCrawlRequestsActions[engineAction.Order] = new List<AEngineAction<TArachnodeDAO>>();
                            }
                            _preGetCrawlRequestsActions[engineAction.Order].Add(engineAction);
                            if (!_postGetCrawlRequestsActions.ContainsKey(engineAction.Order))
                            {
                                _postGetCrawlRequestsActions[engineAction.Order] = new List<AEngineAction<TArachnodeDAO>>();
                            }
                            _postGetCrawlRequestsActions[engineAction.Order].Add(engineAction);
                            break;
                    }
                }

                _consoleManager.OutputBehavior(engineAction);
            }

            if (_preGetCrawlRequestsActions.Count != 0 || _postGetCrawlRequestsActions.Count != 0)
            {
                _consoleManager.OutputString("", ConsoleColor.Gray, ConsoleColor.Gray);
            }
        }

        /// <summary>
        /// 	Performs the crawl actions.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "crawlActionType">Type of the crawl action.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        public override void PerformCrawlActions(CrawlRequest<TArachnodeDAO> crawlRequest, CrawlActionType crawlActionType, IArachnodeDAO arachnodeDAO)
        {
            switch (crawlActionType)
            {
                case CrawlActionType.PreRequest:
                    foreach (List<ACrawlAction<TArachnodeDAO>> crawlActions in _preRequestCrawlActions.Values)
                    {
                        foreach (ACrawlAction<TArachnodeDAO> crawlAction in crawlActions)
                        {
                            if (crawlAction.IsEnabled)
                            {
                                try
                                {
                                    crawlAction.PerformAction(crawlRequest, arachnodeDAO);
                                }
                                catch (Exception exception)
                                {
                                    arachnodeDAO.InsertException(crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri, exception, false);
                                }
                            }
                        }
                    }
                    break;
                case CrawlActionType.PreGet:
                    foreach (List<ACrawlAction<TArachnodeDAO>> crawlActions in _preGetCrawlActions.Values)
                    {
                        foreach (ACrawlAction<TArachnodeDAO> crawlAction in crawlActions)
                        {
                            if (crawlAction.IsEnabled)
                            {
                                try
                                {
                                    crawlAction.PerformAction(crawlRequest, arachnodeDAO);
                                }
                                catch (Exception exception)
                                {
                                    arachnodeDAO.InsertException(crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri, exception, false);
                                }
                            }
                        }
                    }
                    break;
                case CrawlActionType.PostRequest:
                    foreach (List<ACrawlAction<TArachnodeDAO>> crawlActions in _postRequestCrawlActions.Values)
                    {
                        foreach (ACrawlAction<TArachnodeDAO> crawlAction in crawlActions)
                        {
                            if (crawlAction.IsEnabled)
                            {
                                try
                                {
                                    crawlAction.PerformAction(crawlRequest, arachnodeDAO);
                                }
                                catch (Exception exception)
                                {
                                    arachnodeDAO.InsertException(crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri, exception, false);
                                }
                            }
                        }
                    }

                    break;
            }
        }

        /// <summary>
        /// 	Performs the engine actions.
        /// </summary>
        /// <param name = "crawlRequests">The crawl requests.</param>
        /// <param name = "engineActionType">Type of the engine action.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        public override void PerformEngineActions(PriorityQueue<CrawlRequest<TArachnodeDAO>> crawlRequests, EngineActionType engineActionType, IArachnodeDAO arachnodeDAO)
        {
            switch (engineActionType)
            {
                case EngineActionType.PreGetCrawlRequests:
                    foreach (List<AEngineAction<TArachnodeDAO>> engineActions in _preGetCrawlRequestsActions.Values)
                    {
                        foreach (AEngineAction<TArachnodeDAO> engineAction in engineActions)
                        {
                            if (engineAction.IsEnabled)
                            {
                                try
                                {
                                    engineAction.PerformAction(crawlRequests, arachnodeDAO);
                                }
                                catch (Exception exception)
                                {
                                    arachnodeDAO.InsertException(null, null, exception, false);
                                }
                            }
                        }
                    }
                    break;
                case EngineActionType.PostGetCrawlRequests:
                    foreach (List<AEngineAction<TArachnodeDAO>> engineActions in _postGetCrawlRequestsActions.Values)
                    {
                        foreach (AEngineAction<TArachnodeDAO> engineAction in engineActions)
                        {
                            if (engineAction.IsEnabled)
                            {
                                try
                                {
                                    engineAction.PerformAction(crawlRequests, arachnodeDAO);
                                }
                                catch (Exception exception)
                                {
                                    arachnodeDAO.InsertException(null, null, exception, false);
                                }
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 	Stops this instance.
        /// </summary>
        public override void Stop()
        {
            foreach (List<AEngineAction<TArachnodeDAO>> engineActions in _preGetCrawlRequestsActions.Values)
            {
                foreach (AEngineAction<TArachnodeDAO> engineAction in engineActions)
                {
                    engineAction.Stop();
                }
            }

            foreach (List<AEngineAction<TArachnodeDAO>> engineActions in _postGetCrawlRequestsActions.Values)
            {
                foreach (AEngineAction<TArachnodeDAO> engineAction in engineActions)
                {
                    engineAction.Stop();
                }
            }

            foreach (List<ACrawlAction<TArachnodeDAO>> crawlActions in _preRequestCrawlActions.Values)
            {
                foreach (ACrawlAction<TArachnodeDAO> crawlAction in crawlActions)
                {
                    crawlAction.Stop();
                }
            }

            foreach (List<ACrawlAction<TArachnodeDAO>> crawlActions in _preGetCrawlActions.Values)
            {
                foreach (ACrawlAction<TArachnodeDAO> crawlAction in crawlActions)
                {
                    crawlAction.Stop();
                }
            }

            foreach (List<ACrawlAction<TArachnodeDAO>> crawlActions in _postRequestCrawlActions.Values)
            {
                foreach (ACrawlAction<TArachnodeDAO> crawlAction in crawlActions)
                {
                    crawlAction.Stop();
                }
            }
        }
    }
}