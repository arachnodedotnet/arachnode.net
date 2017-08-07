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
using System.Linq;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.DataSource;
using Arachnode.SiteCrawler.Value.AbstractClasses;

#endregion

namespace Arachnode.SiteCrawler.Managers
{
    public class ReportingManager<TArachnodeDAO> : AReportingManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        public ReportingManager(ApplicationSettings applicationSettings, WebSettings webSettings, ConsoleManager<TArachnodeDAO> consoleManager) : base(applicationSettings, webSettings, consoleManager)
        {
        }

        /// <summary>
        /// 	Updates reporting for use in Crawl Priority and indexing ranking/boosting.
        /// </summary>
        public override void Update()
        {
            _consoleManager.OutputString("ReportingManager: Updating Reporting.\n", ConsoleColor.White, ConsoleColor.Gray);

            lock (_reportingManagerLock)
            {
                _hyperLinks_MOST_POPULAR_HOSTS_BY_HOSTS = _reportingLinqToSqlDataContext.HyperLinks_MOST_POPULAR_HOSTS_BY_HOSTs.Take(ApplicationSettings.MaximumNumberOfHostsAndPrioritiesToSelect).OrderByDescending(hl => hl.Strength).ToDictionary(hl => hl.Host.Host1, hl => hl.Strength);

                TArachnodeDAO arachnodeDAO = (TArachnodeDAO)Activator.CreateInstance(typeof(TArachnodeDAO), ApplicationSettings.ConnectionString, ApplicationSettings, WebSettings, false, false);
                arachnodeDAO.ApplicationSettings = ApplicationSettings;

                _priorities.Clear();

                foreach (ArachnodeDataSet.PrioritiesRow prioritiesRow in arachnodeDAO.GetPriorities(ApplicationSettings.MaximumNumberOfHostsAndPrioritiesToSelect))
                {
                    _priorities.Add(prioritiesRow.Host, prioritiesRow.Value);
                }
            }
        }

        /// <summary>
        /// 	Gets the strength for host.
        /// </summary>
        /// <param name = "absoluteUri">The absolute URI.</param>
        /// <returns></returns>
        public override double? GetStrengthForHost(string absoluteUri)
        {
            double? strength;

            if (_hyperLinks_MOST_POPULAR_HOSTS_BY_HOSTS.TryGetValue(UserDefinedFunctions.ExtractHost(absoluteUri).Value, out strength))
            {
                return strength;
            }

            return 0;
        }

        /// <summary>
        /// 	Gets the priority for host.
        /// </summary>
        /// <param name = "absoluteUri">The absolute URI.</param>
        /// <returns></returns>
        public override double? GetPriorityForHost(string absoluteUri)
        {
            double? strength;

            if (_priorities.TryGetValue(UserDefinedFunctions.ExtractHost(absoluteUri).Value, out strength))
            {
                return strength;
            }

            return 0;
        }
    }
}