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

using System.IO;
using Arachnode.SiteCrawler.Value.Interfaces;
using HtmlAgilityPack;

#endregion

namespace Arachnode.SiteCrawler.Value
{
    public class ManagedWebPage : IManagedDiscovery
    {
        /// <summary>
        /// 	Gets or sets the stream writer.
        /// </summary>
        /// <value>The stream writer.</value>
        public StreamWriter StreamWriter { get; set; }

        /// <summary>
        /// 	The HtmlDocument representing the WebPage, as created by the HtmlAgilityPack.  This will be 'null' if 'ManageWebPageMetaData' is set to 'false'.
        /// </summary>
        /// <value>The HTML document.</value>
        public HtmlDocument HtmlDocument { get; set; }

        /// <summary>
        /// 	Gets or sets the tags.  This will be 'null' if 'ManageWebPageMetaData' is set to 'false'.
        /// </summary>
        /// <value>The tags.</value>
        public string Tags { get; set; }

        /// <summary>
        /// 	Gets or sets the text.  This will be 'null' if 'ManageWebPageMetaData' is set to 'false'.`
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; set; }

        #region IManagedDiscovery Members

        /// <summary>
        /// 	The on-disk location of the Discovery.
        /// </summary>
        /// <value></value>
        public string DiscoveryPath { get; set; }

        #endregion
    }
}