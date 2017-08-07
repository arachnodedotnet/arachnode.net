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
using Arachnode.DataSource;
using Arachnode.SiteCrawler.Value.AbstractClasses;
using Arachnode.SiteCrawler.Value.Enums;
using Newtonsoft.Json;

#endregion

namespace Arachnode.SiteCrawler.Value
{
    /// <summary>
    /// 	A Discovery is an AbsoluteUri, whether discovered by a user or by a Crawl.
    /// 	An important distinction to make between a Discovery is that a Discovery exists elsewhere, while a CrawlRequest contains data returned from the Discovery.
    /// 	Additionally, a CrawlRequest contains behavioral instructions and conditions.
    /// </summary>
    public class Discovery<TArachnodeDAO> : ADisallowed<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        ///DO NOT USE - used for Json serialization...
        [JsonConstructor]
        public Discovery()
        {

        }

        /// <summary>
        /// 	Initializes a new instance of the <see cref = "Discovery" /> class.
        /// </summary>
        /// <param name = "absoluteUri">The absolute URI.</param>
        public Discovery(string absoluteUri) : this(absoluteUri, null)
        {
        }

        /// <summary>
        /// 	Initializes a new instance of the <see cref = "Discovery" /> class.
        /// </summary>
        /// <param name = "absoluteUri">The absolute URI.</param>
        public Discovery(Uri absoluteUri) : this(absoluteUri, null)
        {
            //ANODET: Missing error handling.
        }

        /// <summary>
        /// 	Initializes a new instance of the <see cref = "Discovery" /> class.
        /// </summary>
        /// <param name = "absoluteUri">The absolute URI.</param>
        /// <param name = "id">The id.</param>
        internal Discovery(string absoluteUri, long? id)
        {
            CacheKey = new Uri(absoluteUri.Replace("://www.", "://"));
            DiscoveryState = DiscoveryState.Undiscovered;
            //HACK:!!!
            HttpWebRequestRetriesRemaining = 5;
            ID = id;
            IsStorable = true;
            InsertDiscovery = true;
            Uri = new Uri(absoluteUri.TrimEnd('/').TrimEnd('#'));
            //WasUsingDesriedMaximumMemoryInMegabytes = MemoryManager.IsUsingDesiredMaximumMemoryInMegabytes();
        }

        /// <summary>
        /// 	Initializes a new instance of the <see cref = "Discovery" /> class.
        /// </summary>
        /// <param name = "absoluteUri">The absolute URI.</param>
        /// <param name = "id">The id.</param>
        internal Discovery(Uri absoluteUri, long? id)
        {
            CacheKey = new Uri(absoluteUri.AbsoluteUri.Replace("://www.", "://"));
            DiscoveryState = DiscoveryState.Undiscovered;
            //HACK:!!!
            HttpWebRequestRetriesRemaining = 5;
            ID = id;
            IsStorable = true;
            InsertDiscovery = true;
            Uri = new Uri(absoluteUri.AbsoluteUri.TrimEnd('/').TrimEnd('#'));
            //WasUsingDesriedMaximumMemoryInMegabytes = MemoryManager.IsUsingDesiredMaximumMemoryInMegabytes();
        }

        /// <summary>
        /// 	Initializes a new instance of the <see cref = "Discovery" /> class.
        /// </summary>
        /// <param name = "discoveriesRow">The discoveries row.</param>
        internal Discovery(ArachnodeDataSet.DiscoveriesRow discoveriesRow)
        {
            CacheKey = new Uri(discoveriesRow.AbsoluteUri.Replace("://www.", "://"));
            DiscoveryState = (DiscoveryState) discoveriesRow.DiscoveryStateID;
            DiscoveryType = (DiscoveryType) discoveriesRow.DiscoveryTypeID;
            //ANODET: Since this number isn't stored in CrawlRequests, this count is reset.
            //HACK:!!!
            HttpWebRequestRetriesRemaining = 5;
            if (!discoveriesRow.IsIDNull())
            {
                ID = discoveriesRow.ID;
            }
            IsStorable = true;
            InsertDiscovery = true;
            NumberOfTimesDiscovered = discoveriesRow.NumberOfTimesDiscovered;
            Uri = new Uri(discoveriesRow.AbsoluteUri.TrimEnd('/').TrimEnd('#'));
            //WasUsingDesriedMaximumMemoryInMegabytes = MemoryManager.IsUsingDesiredMaximumMemoryInMegabytes();
        }

        /// <summary>
        /// 	The Cache Key.  Removes 'www.' from the Uri and is used by the Cache.
        /// </summary>
        /// <value>The Cache Key.</value>
        [JsonProperty]
        public Uri CacheKey { get; set; }

        /// <summary>
        /// 	Gets or sets a value indicating whether [expect file or image].
        /// </summary>
        /// <value><c>true</c> if [expect file or image]; otherwise, <c>false</c>.</value>
        [JsonProperty]
        internal bool ExpectFileOrImage { get; set; }

        [JsonProperty]
        public int HttpWebRequestRetriesRemaining { get; set; }

        /// <summary>
        /// 	The ID of the Discovery in the database.
        /// </summary>
        /// <value>The ID.</value>
        [JsonProperty]
        public long? ID { get; internal set; }

        /// <summary>
        /// 	Gets or sets a value indicating whether this instance is new.
        /// </summary>
        /// <value><c>true</c> if this instance is new; otherwise, <c>false</c>.</value>
        [JsonProperty]
        internal bool IsNew { get; set; }

        /// <summary>
        /// 	The number of times the Discovery was discovered.
        /// </summary>
        /// <value>The number of times discovered.</value>
        [JsonProperty]
        public int NumberOfTimesDiscovered { get; set; }

        /// <summary>
        /// 	Set this value in CrawlRules to increase or decrease the Crawl Priority of a specific Discovery when a CrawlRequest is created by the CrawlRequestManager.
        /// 	The higher the value for 'PriorityBoost', the higher the Priority.
        /// </summary>
        /// <value>The priority boost.</value>
        [JsonProperty]
        public int PriorityBoost { get; set; }

        /// <summary>
        /// 	The Uri of the Discovery.
        /// </summary>
        /// <value>The URI.</value>
        [JsonProperty]
        public Uri Uri { get; set; }

        /// <summary>
        /// 	Gets or sets the UserData.
        /// </summary>
        /// <value>The user data.</value>
        [JsonProperty]
        public string UserData { get; set; }

        /// <summary>
        /// 	Gets or sets the state of the discovery.
        /// </summary>
        /// <value>The state of the discovery.</value>
        [JsonProperty]
        public DiscoveryState DiscoveryState { get; set; }

        /// <summary>
        /// 	Gets or sets the DiscoveryType.
        /// </summary>
        /// <value>The DiscoveryType.</value>
        [JsonProperty]
        public DiscoveryType DiscoveryType { get; set; }

        /// <summary>
        /// 	Returns a <see cref = "T:System.String"></see> that represents the current <see cref = "T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// 	A <see cref = "T:System.String"></see> that represents the current <see cref = "T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            return Uri != null ? DiscoveryType + " : " + Uri.AbsoluteUri + " : " + DiscoveryState : null;
        }
    }
}