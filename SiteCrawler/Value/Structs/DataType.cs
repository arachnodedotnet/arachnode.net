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
using Arachnode.SiteCrawler.Value.Enums;

#endregion

namespace Arachnode.SiteCrawler.Value.Structs
{
    /// <summary>
    /// 	A DataType is a mapping between the ResponseHeader 'Content-Type:' and the type to be used by SQL Server Full-Text Indexing.
    /// 	Synonyms are used to classify an AbsoluteUri when ResponseHeaders are not available.
    /// 	If no file name is specified in an AbsoluteUri and ResponseHeaders are not available no classification can be made.
    /// 	Overrides are more specific classifications, such as .aspx is more specific than .htm.
    /// </summary>
    public struct DataType
    {
        private string _contentType;
        private int _contentTypeID;
        private DiscoveryType _discoveryType;
        private string _fullTextIndexType;
        private List<string> _overrides;
        private List<string> _synonyms;

        /// <summary>
        /// 	Initializes a new instance of the <see cref = "DataType" /> struct.
        /// </summary>
        /// <param name = "contentType">Type of the content.</param>
        /// <param name = "contentTypeID">The content type ID.</param>
        /// <param name = "discoveryType">Type of the discovery.</param>
        /// <param name = "fullTextIndexType">Full type of the text index.</param>
        /// <param name = "synonyms">The synonyms.</param>
        /// <param name = "overrides">The overrides.</param>
        public DataType(string contentType, int contentTypeID, DiscoveryType discoveryType, string fullTextIndexType, List<string> synonyms, List<string> overrides)
        {
            _contentType = contentType;
            _contentTypeID = contentTypeID;
            _discoveryType = discoveryType;
            _fullTextIndexType = fullTextIndexType;
            _synonyms = synonyms;
            _overrides = overrides;
        }

        /// <summary>
        /// 	The ContentType of the DataType.
        /// </summary>
        /// <value>The type of the content.</value>
        public string ContentType
        {
            get { return _contentType; }
            internal set { _contentType = value; }
        }

        /// <summary>
        /// 	The ContentTypeID of the DataType.
        /// </summary>
        /// <value>The content type ID.</value>
        public int ContentTypeID
        {
            get
            {
                if (_contentTypeID == 0)
                {
                    _contentTypeID = 1;
                }

                return _contentTypeID;
            }
            internal set { _contentTypeID = value; }
        }

        /// <summary>
        /// 	The DiscoveryType of the DataType.
        /// </summary>
        /// <value>The type of the discovery.</value>
        public DiscoveryType DiscoveryType
        {
            get { return _discoveryType; }
            internal set { _discoveryType = value; }
        }

        /// <summary>
        /// 	The FullTextIndexType of the DataType.
        /// </summary>
        /// <value>The full type of the text index.</value>
        public string FullTextIndexType
        {
            get { return _fullTextIndexType; }
            internal set { _fullTextIndexType = value; }
        }

        /// <summary>
        /// 	The Synonyms of the DataType.
        /// </summary>
        /// <value>The synonyms.</value>
        public List<string> Synonyms
        {
            get { return _synonyms; }
            internal set { _synonyms = value; }
        }

        /// <summary>
        /// 	The Overrides of the DataType.
        /// </summary>
        /// <value>The overrides.</value>
        public List<string> Overrides
        {
            get { return _overrides; }
            internal set { _overrides = value; }
        }

        public override string ToString()
        {
            return !string.IsNullOrEmpty(ContentType) ? DiscoveryType + " : " + ContentType : DiscoveryType + " : UNKNOWN";
        }
    }
}