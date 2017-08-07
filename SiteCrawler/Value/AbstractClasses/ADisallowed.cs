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
using Arachnode.DataAccess.Value.Interfaces;

#endregion

namespace Arachnode.SiteCrawler.Value.AbstractClasses
{
    [Serializable]
    public class ADisallowed<TArachnodeDAO> : AStorable<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        /// <summary>
        /// 	Is the Discovery/CrawlRequest disallowed?  If the Discovery/CrawlRequest is disallowed a Rule may/must update this field.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is disallowed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisallowed { get; set; }

        /// <summary>
        /// 	The reason the Discovery/CrawlRequest is disallowed.  If the Discovery is disallowed a Rule may/must update this field.
        /// </summary>
        /// <value>The is disallowed reason.</value>
        public string IsDisallowedReason { get; set; }

        /// <summary>
        /// 	If the Discovery/CrawlRequest is disallowed a Rule may/must update this field.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [output is disallowed reason]; otherwise, <c>false</c>.
        /// </value>
        public bool OutputIsDisallowedReason { get; set; }
    }
}