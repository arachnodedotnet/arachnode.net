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

#endregion

namespace Arachnode.Plugins.CrawlActions.Value
{
    public class Document
    {
        /// <summary>
        /// 	Gets or sets the absolute URI.
        /// </summary>
        /// <value>The absolute URI.</value>
        public string AbsoluteUri { get; set; }

        /// <summary>
        /// 	Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created { get; set; }

        /// <summary>
        /// 	Gets or sets the discovery ID.
        /// </summary>
        /// <value>The discovery ID.</value>
        public long DiscoveryID { get; set; }

        /// <summary>
        /// 	Gets or sets the discovery path.
        /// </summary>
        /// <value>The discovery path.</value>
        public string DiscoveryPath { get; set; }

        /// <summary>
        /// 	Gets or sets the domain.
        /// </summary>
        /// <value>The domain.</value>
        public string Domain { get; set; }

        /// <summary>
        /// 	Gets or sets the extension.
        /// </summary>
        /// <value>The extension.</value>
        public string Extension { get; set; }

        /// <summary>
        /// 	Gets or sets the host.
        /// </summary>
        /// <value>The host.</value>
        public string Host { get; set; }

        /// <summary>
        /// 	Gets or sets the scheme.
        /// </summary>
        /// <value>The scheme.</value>
        public string Scheme { get; set; }

        /// <summary>
        /// 	Gets or sets the score.
        /// </summary>
        /// <value>The score.</value>
        public float Score { get; set; }

        /// <summary>
        /// 	Gets or sets the strength.
        /// </summary>
        /// <value>The strength.</value>
        public float Strength { get; set; }

        /// <summary>
        /// 	Gets or sets the summary.
        /// </summary>
        /// <value>The summary.</value>
        public string Summary { get; set; }

        /// <summary>
        /// 	Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

        /// <summary>
        /// 	Gets or sets the updated.
        /// </summary>
        /// <value>The updated.</value>
        public DateTime? Updated { get; set; }
    }
}