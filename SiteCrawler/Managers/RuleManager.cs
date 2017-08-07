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
using Arachnode.SiteCrawler.Core;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.AbstractClasses;
using Arachnode.SiteCrawler.Value.Enums;

#endregion

namespace Arachnode.SiteCrawler.Managers
{
    /// <summary>
    /// 	Provides pre- and post-request CrawlRule functionality.
    /// </summary>
    public class RuleManager<TArachnodeDAO> : ARuleManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        public RuleManager(ApplicationSettings applicationSettings, WebSettings webSettings, ConsoleManager<TArachnodeDAO> consoleManager) : base(applicationSettings, webSettings, consoleManager)
        {
        }

        /// <summary>
        /// 	Processes and instantiates the CrawlRules specified by ApplicationSettings.CrawlRulesDotConfigLocation.
        /// </summary>
        public override void ProcessCrawlRules(Crawler<TArachnodeDAO> crawler)
        {
            _preRequestCrawlRules.Clear();
            _preGetCrawlRules.Clear();
            _postRequestCrawlRules.Clear();

            foreach (ACrawlRule<TArachnodeDAO> crawlRule in crawler.CrawlRules.Values)
            {
                if (crawlRule.IsEnabled)
                {
                    if (crawlRule.Settings != null)
                    {
                        Dictionary<string, string> settings = new Dictionary<string, string>();

                        foreach (string setting in crawlRule.Settings.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                        {
                            string[] keyAndValue = setting.Split("=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                            if (keyAndValue.Length == 1 || string.IsNullOrEmpty(keyAndValue[1]))
                            {
                                throw new InvalidConfigurationException(null, null, "\nThe following configuration setting is missing from database table 'cfg.CrawlRules' for CrawlRule '" + crawlRule.TypeName + "'\n\t" + keyAndValue[0], InvalidConfigurationExceptionSeverity.Error);
                            }

                            settings.Add(keyAndValue[0], keyAndValue[1]);
                        }

                        crawlRule.AssignSettings(settings);
                    }

                    switch (crawlRule.CrawlRuleType)
                    {
                        case CrawlRuleType.PreRequest:
                            if (!_preRequestCrawlRules.ContainsKey(crawlRule.Order))
                            {
                                _preRequestCrawlRules[crawlRule.Order] = new List<ACrawlRule<TArachnodeDAO>>();
                            }
                            _preRequestCrawlRules[crawlRule.Order].Add(crawlRule);
                            break;
                        case CrawlRuleType.PreGet:
                            if (!_preGetCrawlRules.ContainsKey(crawlRule.Order))
                            {
                                _preGetCrawlRules[crawlRule.Order] = new List<ACrawlRule<TArachnodeDAO>>();
                            }
                            _preGetCrawlRules[crawlRule.Order].Add(crawlRule);
                            break;
                        case CrawlRuleType.PostRequest:
                            if (!_postRequestCrawlRules.ContainsKey(crawlRule.Order))
                            {
                                _postRequestCrawlRules[crawlRule.Order] = new List<ACrawlRule<TArachnodeDAO>>();
                            }
                            _postRequestCrawlRules[crawlRule.Order].Add(crawlRule);
                            break;
                        case CrawlRuleType.PreAndPostRequest:
                            if (!_preRequestCrawlRules.ContainsKey(crawlRule.Order))
                            {
                                _preRequestCrawlRules[crawlRule.Order] = new List<ACrawlRule<TArachnodeDAO>>();
                            }
                            _preRequestCrawlRules[crawlRule.Order].Add(crawlRule);
                            if (!_postRequestCrawlRules.ContainsKey(crawlRule.Order))
                            {
                                _postRequestCrawlRules[crawlRule.Order] = new List<ACrawlRule<TArachnodeDAO>>();
                            }
                            _postRequestCrawlRules[crawlRule.Order].Add(crawlRule);
                            break;
                    }
                }

                _consoleManager.OutputBehavior(crawlRule);
            }

            if (_preRequestCrawlRules.Count != 0 || _preGetCrawlRules.Count != 0 || _postRequestCrawlRules.Count != 0)
            {
                _consoleManager.OutputString("", ConsoleColor.Gray, ConsoleColor.Gray);
            }
        }

        /// <summary>
        /// 	Determines whether the specified crawl request is disallowed.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "crawlRuleType">Type of the rule.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        /// <returns>
        /// 	<c>true</c> if the specified crawl request is disallowed; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsDisallowed(CrawlRequest<TArachnodeDAO> crawlRequest, CrawlRuleType crawlRuleType, IArachnodeDAO arachnodeDAO)
        {
            switch (crawlRuleType)
            {
                case CrawlRuleType.PreRequest:
                    foreach (List<ACrawlRule<TArachnodeDAO>> crawlRules in _preRequestCrawlRules.Values)
                    {
                        foreach (ACrawlRule<TArachnodeDAO> crawlRule in crawlRules)
                        {
                            try
                            {
                                if (crawlRule.IsEnabled && crawlRule.IsDisallowed(crawlRequest, arachnodeDAO))
                                {
                                    crawlRequest.IsDisallowed = true;
                                    crawlRequest.Discovery.IsDisallowed = true;
                                    crawlRequest.Discovery.IsDisallowedReason = crawlRequest.IsDisallowedReason;

                                    return true;
                                }
                            }
                            catch (Exception exception)
                            {
                                arachnodeDAO.InsertException(crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri, exception, false);

                                return true;
                            }
                        }
                    }
                    break;
                case CrawlRuleType.PreGet:
                    foreach (List<ACrawlRule<TArachnodeDAO>> crawlRules in _preGetCrawlRules.Values)
                    {
                        foreach (ACrawlRule<TArachnodeDAO> crawlRule in crawlRules)
                        {
                            try
                            {
                                if (crawlRule.IsEnabled && crawlRule.IsDisallowed(crawlRequest, arachnodeDAO))
                                {
                                    crawlRequest.IsDisallowed = true;
                                    crawlRequest.Discovery.IsDisallowed = true;
                                    crawlRequest.Discovery.IsDisallowedReason = crawlRequest.IsDisallowedReason;

                                    return true;
                                }
                            }
                            catch (Exception exception)
                            {
                                arachnodeDAO.InsertException(crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri, exception, false);

                                return true;
                            }
                        }
                    }
                    break;
                case CrawlRuleType.PostRequest:
                    foreach (List<ACrawlRule<TArachnodeDAO>> crawlRules in _postRequestCrawlRules.Values)
                    {
                        foreach (ACrawlRule<TArachnodeDAO> crawlRule in crawlRules)
                        {
                            try
                            {
                                if (crawlRule.IsEnabled && crawlRule.IsDisallowed(crawlRequest, arachnodeDAO))
                                {
                                    crawlRequest.IsDisallowed = true;
                                    crawlRequest.Discovery.IsDisallowed = true;
                                    crawlRequest.Discovery.IsDisallowedReason = crawlRequest.IsDisallowedReason;

                                    return true;
                                }
                            }
                            catch (Exception exception)
                            {
                                arachnodeDAO.InsertException(crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri, exception, false);

                                return true;
                            }
                        }
                    }
                    break;
            }

            return false;
        }

        /// <summary>
        /// 	Determines whether the specified crawl request is disallowed.
        /// </summary>
        /// <param name = "discovery">The discovery.</param>
        /// <param name = "crawlRuleType">Type of the rule.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        /// <returns>
        /// 	<c>true</c> if the specified crawl request is disallowed; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsDisallowed(Discovery<TArachnodeDAO> discovery, CrawlRuleType crawlRuleType, IArachnodeDAO arachnodeDAO)
        {
            switch (crawlRuleType)
            {
                case CrawlRuleType.PreRequest:
                    foreach (List<ACrawlRule<TArachnodeDAO>> crawlRules in _preRequestCrawlRules.Values)
                    {
                        foreach (ACrawlRule<TArachnodeDAO> crawlRule in crawlRules)
                        {
                            try
                            {
                                if (crawlRule.IsEnabled && crawlRule.IsDisallowed(discovery, arachnodeDAO))
                                {
                                    discovery.IsDisallowed = true;

                                    return true;
                                }
                            }
                            catch (Exception exception)
                            {
                                arachnodeDAO.InsertException(discovery.Uri.AbsoluteUri, discovery.Uri.AbsoluteUri, exception, false);

                                return true;
                            }
                        }
                    }
                    break;
                case CrawlRuleType.PreGet:
                    foreach (List<ACrawlRule<TArachnodeDAO>> crawlRules in _preGetCrawlRules.Values)
                    {
                        foreach (ACrawlRule<TArachnodeDAO> crawlRule in crawlRules)
                        {
                            try
                            {
                                if (crawlRule.IsEnabled && crawlRule.IsDisallowed(discovery, arachnodeDAO))
                                {
                                    discovery.IsDisallowed = true;

                                    return true;
                                }
                            }
                            catch (Exception exception)
                            {
                                arachnodeDAO.InsertException(discovery.Uri.AbsoluteUri, discovery.Uri.AbsoluteUri, exception, false);

                                return true;
                            }
                        }
                    }
                    break;
                case CrawlRuleType.PostRequest:
                    foreach (List<ACrawlRule<TArachnodeDAO>> crawlRules in _postRequestCrawlRules.Values)
                    {
                        foreach (ACrawlRule<TArachnodeDAO> crawlRule in crawlRules)
                        {
                            try
                            {
                                if (crawlRule.IsEnabled && crawlRule.IsDisallowed(discovery, arachnodeDAO))
                                {
                                    discovery.IsDisallowed = true;

                                    return true;
                                }
                            }
                            catch (Exception exception)
                            {
                                arachnodeDAO.InsertException(discovery.Uri.AbsoluteUri, discovery.Uri.AbsoluteUri, exception, false);

                                return true;
                            }
                        }
                    }
                    break;
            }

            return false;
        }

        /// <summary>
        /// 	Stops this instance.
        /// </summary>
        public override void Stop()
        {
            foreach (List<ACrawlRule<TArachnodeDAO>> crawlRules in _preRequestCrawlRules.Values)
            {
                foreach (ACrawlRule<TArachnodeDAO> crawlRule in crawlRules)
                {
                    crawlRule.Stop();
                }
            }

            foreach (List<ACrawlRule<TArachnodeDAO>> crawlRules in _preGetCrawlRules.Values)
            {
                foreach (ACrawlRule<TArachnodeDAO> crawlRule in crawlRules)
                {
                    crawlRule.Stop();
                }
            }

            foreach (List<ACrawlRule<TArachnodeDAO>> crawlRules in _postRequestCrawlRules.Values)
            {
                foreach (ACrawlRule<TArachnodeDAO> crawlRule in crawlRules)
                {
                    crawlRule.Stop();
                }
            }
        }
    }
}