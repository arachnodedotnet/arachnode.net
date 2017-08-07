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
using Arachnode.Configuration;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Value.AbstractClasses;

#endregion

namespace Arachnode.SiteCrawler.Managers
{    
    public class MemoryManager<TArachnodeDAO> : AMemoryManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        public MemoryManager(ApplicationSettings applicationSettings, WebSettings webSettings) : base(applicationSettings, webSettings)
        {
            
        }

        public override bool HasDesiredMaximumMemoryUsageInMegabytesEverBeenMet { get; set; }

        /// <summary>
        /// 	Determines whether [is using desired maximum memory in megabytes].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is using desired maximum memory in megabytes]; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsUsingDesiredMaximumMemoryInMegabytes(bool isForCrawlRequestConsideration)
        {
            if (!HasDesiredMaximumMemoryUsageInMegabytesEverBeenMet && Environment.WorkingSet/1024/1024 > ApplicationSettings.DesiredMaximumMemoryUsageInMegabytes)
            {
                HasDesiredMaximumMemoryUsageInMegabytesEverBeenMet = true;
            }

            if (isForCrawlRequestConsideration)
            {
                return Environment.WorkingSet/1024/1024 > ApplicationSettings.DesiredMaximumMemoryUsageInMegabytes/2;
            }
            else
            {
                return Environment.WorkingSet/1024/1024 > ApplicationSettings.DesiredMaximumMemoryUsageInMegabytes;
            }
        }
    }
}