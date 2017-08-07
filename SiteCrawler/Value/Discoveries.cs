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

using System.Collections.Generic;
using Arachnode.DataAccess.Value.Interfaces;

#endregion

namespace Arachnode.SiteCrawler.Value
{
    /// <summary>
    /// 	A helper class used by Crawl and DiscoveryManager.
    /// </summary>
    public class Discoveries<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        private Dictionary<string, Discovery<TArachnodeDAO>> _emailAddresses;
        private Dictionary<string, Discovery<TArachnodeDAO>> _filesAndImages;
        private Dictionary<string, Discovery<TArachnodeDAO>> _hyperLinks;

        /// <summary>
        /// 	A Dictionary containing HyperLink discoveries.
        /// </summary>
        /// <value>The hyper links.</value>
        public Dictionary<string, Discovery<TArachnodeDAO>> HyperLinks
        {
            get
            {
                if (_hyperLinks == null)
                {
                    _hyperLinks = new Dictionary<string, Discovery<TArachnodeDAO>>();
                }
                return _hyperLinks;
            }
            set { _hyperLinks = value; }
        }

        /// <summary>
        /// 	A Dictionary containing File and Image discoveries.
        /// </summary>
        /// <value>The files and images.</value>
        public Dictionary<string, Discovery<TArachnodeDAO>> FilesAndImages
        {
            get
            {
                if (_filesAndImages == null)
                {
                    _filesAndImages = new Dictionary<string, Discovery<TArachnodeDAO>>();
                }
                return _filesAndImages;
            }
            set { _filesAndImages = value; }
        }

        /// <summary>
        /// 	A Dictionary containing EmailAddress discoveries.
        /// </summary>
        /// <value>The email addresses.</value>
        public Dictionary<string, Discovery<TArachnodeDAO>> EmailAddresses
        {
            get
            {
                if (_emailAddresses == null)
                {
                    _emailAddresses = new Dictionary<string, Discovery<TArachnodeDAO>>();
                }
                return _emailAddresses;
            }
            set { _emailAddresses = value; }
        }
    }
}