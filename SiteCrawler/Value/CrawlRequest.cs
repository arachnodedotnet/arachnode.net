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
using System.Text;
using System.Windows.Forms;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.DataSource;
using Arachnode.Renderer.Value;
using Arachnode.Renderer.Value.Enums;
using Arachnode.SiteCrawler.Components;
using Arachnode.SiteCrawler.Core;
using Arachnode.SiteCrawler.Value.AbstractClasses;
using Arachnode.SiteCrawler.Value.Enums;
using Arachnode.SiteCrawler.Value.Interfaces;
using Arachnode.SiteCrawler.Value.Structs;
using mshtml;
using Newtonsoft.Json;

#endregion

namespace Arachnode.SiteCrawler.Value
{
    /// <summary>
    /// 	CrawlRequests contain a Discovery and instructions for a Crawl.
    /// 	An important distinction to make between a Discovery is that a Discovery exists elsewhere, while a CrawlRequest contains data returned from the Discovery.
    /// 	Additionally, a CrawlRequest contains behavioral instructions and conditions.
    /// </summary>
    public class CrawlRequest<TArachnodeDAO> : ADisallowed<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        ///DO NOT USE - used for Json serialization...
        [JsonConstructor]
        internal CrawlRequest()
        {
        }

        /// <summary>
        /// 	A CrawlRequest.
        /// </summary>
        /// <param name = "discovery">The Discovery to be crawled.</param>
        /// <param name = "depth">The depth to which this CrawlRequest should crawl.</param>
        /// <param name = "restrictCrawlTo">if set to <c>true</c> [restrict crawl to].</param>
        /// <param name = "restrictDiscoveriesTo">if set to <c>true</c> [restrict discoveries to].</param>
        /// <param name = "priority">The Priority of CrawlRequest.</param>
        public CrawlRequest(Discovery<TArachnodeDAO> discovery, int depth, UriClassificationType restrictCrawlTo, UriClassificationType restrictDiscoveriesTo, double priority, RenderType renderType, RenderType renderTypeForChildren)
        {
            if (depth <= 0)
            {
                throw new ArgumentException("Depth cannot be less than or equal to zero.", "depth");
            }

            //ANODET: This is a bug (Parent) - the user needs to be able to specify a file and a WebPage parent if the user is submitting an explicit Request for a File or an Image...
            Created = DateTime.Now;
            if (restrictCrawlTo >= UriClassificationType.OriginalDirectoryLevelUp || restrictDiscoveriesTo >= UriClassificationType.OriginalDirectoryLevelUp)
            {
                Originator = discovery;
            }
            Parent = discovery;
            CurrentDepth = 1;
            Discovery = discovery;
            IsStorable = true;
            InsertDiscovery = true;
            MaximumDepth = depth;
            RenderType = renderType;
            RenderTypeForChildren = renderTypeForChildren;
            RestrictCrawlTo = (short) restrictCrawlTo;
            RestrictDiscoveriesTo = (short) restrictDiscoveriesTo;
            Priority = priority;
            //WasUsingDesriedMaximumMemoryInMegabytes = MemoryManager.IsUsingDesiredMaximumMemoryInMegabytes();
            Discoveries = new Discoveries<TArachnodeDAO>();
        }

        /// <summary>
        /// 	A CrawlRequest.
        /// </summary>
        /// <param name = "parent">The discovering CrawlRequest.</param>
        /// <param name = "discovery">The Discovery to be crawled.</param>
        /// <param name = "currentDepth">The current depth.</param>
        /// <param name = "maximumDepth">The maximum depth to which this CrawlRequest should crawl.</param>
        /// <param name = "restrictCrawlTo">The restrict crawl to.</param>
        /// <param name = "restrictDiscoveriesTo">The restrict discoveries to.</param>
        /// <param name = "priority">The Priority of CrawlRequest.</param>
        public CrawlRequest(CrawlRequest<TArachnodeDAO> parent, Discovery<TArachnodeDAO> discovery, int currentDepth, int maximumDepth, short restrictCrawlTo, short restrictDiscoveriesTo, double priority, RenderType renderType, RenderType renderTypeForChildren)
        {
            Created = DateTime.Now;
            if (restrictCrawlTo >= (short) UriClassificationType.OriginalDirectoryLevelUp || restrictDiscoveriesTo >= (short) UriClassificationType.OriginalDirectoryLevelUp)
            {
                Originator = parent.Originator;
            }
            Parent = parent.Discovery;
            CurrentDepth = currentDepth;
            Discovery = discovery;
            IsStorable = true;
            InsertDiscovery = true;
            MaximumDepth = maximumDepth;
            RenderType = renderType;
            RenderTypeForChildren = renderTypeForChildren;
            RestrictCrawlTo = restrictCrawlTo;
            RestrictDiscoveriesTo = restrictDiscoveriesTo;
            Priority = priority;
            //WasUsingDesriedMaximumMemoryInMegabytes = MemoryManager.IsUsingDesiredMaximumMemoryInMegabytes();
            //ANODET: ExpectFileOrImage = expectFileOrImage;
            Discoveries = new Discoveries<TArachnodeDAO>();
        }

        /// <summary>
        /// 	A CrawlRequest.
        /// </summary>
        /// <param name = "originator">The discovering CrawlRequest's Orignator Discovery.</param>
        /// <param name = "parent">The discovering CrawlRequest's Discovery.</param>
        /// <param name = "discovery">The Discovery to be crawled.</param>
        /// <param name = "currentDepth">The current depth.</param>
        /// <param name = "maximumDepth">The maximum depth to which this CrawlRequest should crawl.</param>
        /// <param name = "restrictCrawlTo">The restrict crawl to.</param>
        /// <param name = "restrictDiscoveriesTo">The restrict discoveries to.</param>
        /// <param name = "priority">The Priority of CrawlRequest.</param>
        internal CrawlRequest(Discovery<TArachnodeDAO> originator, Discovery<TArachnodeDAO> parent, Discovery<TArachnodeDAO> discovery, int currentDepth, int maximumDepth, short restrictCrawlTo, short restrictDiscoveriesTo, double priority, RenderType renderType, RenderType renderTypeForChildren)
        {
            Created = DateTime.Now;
            if (restrictCrawlTo >= (short) UriClassificationType.OriginalDirectoryLevelUp || restrictDiscoveriesTo >= (short) UriClassificationType.OriginalDirectoryLevelUp)
            {
                Originator = originator;
            }
            Parent = parent;
            CurrentDepth = currentDepth;
            Discovery = discovery;
            IsStorable = true;
            InsertDiscovery = true;
            MaximumDepth = maximumDepth;
            RenderType = renderType;
            RenderTypeForChildren = renderTypeForChildren;
            RestrictCrawlTo = restrictCrawlTo;
            RestrictDiscoveriesTo = restrictDiscoveriesTo;
            Priority = priority;
            //WasUsingDesriedMaximumMemoryInMegabytes = MemoryManager.IsUsingDesiredMaximumMemoryInMegabytes();
            //ANODET: ExpectFileOrImage = expectFileOrImage;
            Discoveries = new Discoveries<TArachnodeDAO>();
        }

        /// <summary>
        /// 	A CrawlRequest.  (Used by the Engine).
        /// </summary>
        /// <param name = "crawlRequestsRow">The crawl requests row.</param>
        internal CrawlRequest(ArachnodeDataSet.CrawlRequestsRow crawlRequestsRow, Cache<TArachnodeDAO> cache, IArachnodeDAO arachnodeDAO)
        {
            Created = crawlRequestsRow.Created;
            if (crawlRequestsRow.RestrictCrawlTo >= (short) UriClassificationType.OriginalDirectoryLevelUp || crawlRequestsRow.RestrictDiscoveriesTo >= (short) UriClassificationType.OriginalDirectoryLevelUp)
            {
                Originator = cache.GetDiscovery(crawlRequestsRow.AbsoluteUri0, arachnodeDAO);
            }
            Parent = cache.GetDiscovery(crawlRequestsRow.AbsoluteUri1, arachnodeDAO);
            Discovery = cache.GetDiscovery(crawlRequestsRow.AbsoluteUri2, arachnodeDAO);
            CurrentDepth = crawlRequestsRow.CurrentDepth;
            IsStorable = true;
            InsertDiscovery = true;
            MaximumDepth = crawlRequestsRow.MaximumDepth;
            RenderType = (RenderType) crawlRequestsRow.RenderType;
            RenderTypeForChildren = (RenderType) crawlRequestsRow.RenderTypeForChildren;
            RestrictCrawlTo = crawlRequestsRow.RestrictCrawlTo;
            RestrictDiscoveriesTo = crawlRequestsRow.RestrictDiscoveriesTo;
            Priority = crawlRequestsRow.Priority;
            //WasUsingDesriedMaximumMemoryInMegabytes = MemoryManager.IsUsingDesiredMaximumMemoryInMegabytes();
            Discoveries = new Discoveries<TArachnodeDAO>();
            IsFromDatabase = true;
        }

        /// <summary>
        /// 	The current depth.
        /// </summary>
        /// <value>The current depth.</value>
        [JsonProperty]
        public int CurrentDepth { get; internal set; }

        /// <summary>
        /// 	Gets or sets the crawl.
        /// </summary>
        /// <value>The crawl.</value>
        public Crawl<TArachnodeDAO> Crawl { get; set; }

        /// <summary>
        /// 	Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        [JsonProperty]
        public DateTime Created { get; internal set; }

        /// <summary>
        /// 	The binary stream of data downloaded by the WebClient.
        /// </summary>
        /// <value>The data.</value>
        public byte[] Data { get; set; }

        /// <summary>
        /// 	The DataType of the CrawlRequest.
        /// </summary>
        /// <value>The type of the data.</value>
        [JsonProperty]
        public DataType DataType { get; internal set; }

        /// <summary>
        /// 	Gets or sets the decoded HTML.  If you wish to create custom plug-ins that index, say, .doc or .pdf files, assign this property the content you wish to index and it will be indexed by ManageLuceneDotNetIndexes.cs.
        /// </summary>
        /// <value>The decoded HTML.</value>
        public string DecodedHtml { get; set; }

        /// <summary>
        /// 	The submitted discovery.
        /// </summary>
        /// <value>The discovery.</value>
        [JsonProperty]
        public Discovery<TArachnodeDAO> Discovery { get; set; }

        /// <summary>
        /// 	The Discoveries of the DataType.
        /// </summary>
        /// <value>The discoveries.</value>
        [JsonProperty]
        public Discoveries<TArachnodeDAO> Discoveries { get; internal set; }

        /// <summary>
        /// 	Gets or sets the encoding.
        /// </summary>
        /// <value>The encoding.</value>
        public Encoding Encoding { get; set; }

        public string Html { get; set; }

        /// <summary>
        /// 	The HtmlDocument of the HtmlDocument.
        /// </summary>
        /// <value>The html document.</value>
        public HTMLDocumentClass HtmlDocument { get; set; }

        public RendererMessage RendererMessage { get; set; }

        public TimeSpan HttpWebResponseTime { get; internal set; }

        [JsonProperty]
        public bool IsFromCrawlerPeer { get; internal set; }

        [JsonProperty]
        internal bool IsFromDatabase { get; set; }

        /// <summary>
        /// 	Gets or sets the managed discovery.
        /// </summary>
        /// <value>The managed discovery.</value>
        public IManagedDiscovery ManagedDiscovery { get; internal set; }

        /// <summary>
        /// 	The maximum depth to which this CrawlRequest should crawl.
        /// </summary>
        /// <value>The maximum depth.</value>
        [JsonProperty]
        public int MaximumDepth { get; set; }

        /// <summary>
        /// 	Gets or sets the originator.  (Which Discovery originally discovered this CrawlRequest...)
        /// </summary>
        /// <value>The parent.</value>
        [JsonProperty] 
        public Discovery<TArachnodeDAO> Originator { get; internal set; }

        /// <summary>
        /// 	Gets or sets the parent.  (Which Discovery discovered this CrawlRequest...)
        /// </summary>
        /// <value>The parent.</value>
        [JsonProperty]
        public Discovery<TArachnodeDAO> Parent { get; internal set; }

        /// <summary>
        /// 	The Politeness of the CrawlRequest.
        /// </summary>
        /// <value>The politeness.</value>
        public Politeness Politeness { get; set; }

        /// <summary>
        /// 	The Priority of the CrawlRequest.  A higher number indicates a higher priority.
        /// </summary>
        /// <value>The priority.</value>
        [JsonProperty]
        public double Priority { get; internal set; }

        /// <summary>
        /// 	Gets or sets a value indicating whether [process data].
        /// 	If ProcessData == false, the CrawlRequest was not allowed to DownloadString from the WebClient as the LastModified HttpRequestHeader was older than LastDiscovered in the database.
        /// </summary>
        /// <value><c>true</c> if [process data]; otherwise, <c>false</c>.</value>
        public bool ProcessData { get; internal set; }

        [JsonProperty]
        public RenderType RenderType { get; internal set; }

        [JsonProperty]
        public RenderType RenderTypeForChildren { get; internal set; }

        /// <summary>
        /// 	Gets or sets the restrict crawl to.
        /// </summary>
        /// <value>The restrict crawl to.</value>
        [JsonProperty] 
        public short RestrictCrawlTo { get; internal set; }

        /// <summary>
        /// 	Gets or sets the restrict discoveries to.
        /// </summary>
        /// <value>The restrict discoveries to.</value>
        [JsonProperty] 
        public short RestrictDiscoveriesTo { get; internal set; }

        /// <summary>
        /// 	Gets or sets the Tag.
        /// </summary>
        /// <value>The tag.</value>
        [JsonProperty]
        public string Tag { get; set; }

        /// <summary>
        /// 	Gets or sets the UserData.
        /// </summary>
        /// <value>The user data.</value>
        [JsonProperty] 
        public string UserData { get; set; }

        /// <summary>
        /// 	The WebClient of the CrawlRequest.
        /// </summary>
        /// <value>The web client.</value>
        public WebClient<TArachnodeDAO> WebClient { get; internal set; }

        /// <summary>
        /// 	Returns a <see cref = "T:System.String"></see> that represents the current <see cref = "T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// 	A <see cref = "T:System.String"></see> that represents the current <see cref = "T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            return Discovery != null ? Discovery.Uri.AbsoluteUri : null;
        }
    }
}