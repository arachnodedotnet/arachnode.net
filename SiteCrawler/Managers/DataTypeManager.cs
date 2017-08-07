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
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.DataSource;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.AbstractClasses;
using Arachnode.SiteCrawler.Value.Enums;
using Arachnode.SiteCrawler.Value.Structs;

#endregion

namespace Arachnode.SiteCrawler.Managers
{
    public class DataTypeManager<TArachnodeDAO> : ADataTypeManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        public DataTypeManager(ApplicationSettings applicationSettings, WebSettings webSettings) : base(applicationSettings, webSettings)
        {
            
        }

        /// <summary>
        /// 	Gets the allowed data types.
        /// </summary>
        /// <value>The allowed data types.</value>
        public override Dictionary<string, DataType> AllowedDataTypes
        {
            get { return _allowedDataTypes; }
        }

        /// <summary>
        /// 	Refreshes the data types.
        /// </summary>
        public override void RefreshDataTypes()
        {
            AllowedDataTypes.Clear();
            _contentTypesByID.Clear();
            _contentTypesByName.Clear();

            foreach (ArachnodeDataSet.ContentTypesRow contentTypesRow in _arachnodeDAO.GetContentTypes())
            {
                _contentTypesByID.Add(contentTypesRow.ID, contentTypesRow.Name);
                _contentTypesByName.Add(contentTypesRow.Name, contentTypesRow.ID);
            }

            foreach (ArachnodeDataSet.AllowedDataTypesRow dataTypesRow in _arachnodeDAO.GetAllowedDataTypes())
            {
                DataType dataType = new DataType();

                dataType.ContentType = _contentTypesByID[dataTypesRow.ContentTypeID];
                dataType.ContentTypeID = dataTypesRow.ContentTypeID;
                dataType.DiscoveryType = (DiscoveryType) dataTypesRow.DiscoveryTypeID;
                dataType.FullTextIndexType = dataTypesRow.FullTextIndexType;
                if (!dataTypesRow.IsOverridesNull())
                {
                    dataType.Overrides = new List<string>();
                    dataType.Overrides.AddRange(dataTypesRow.Overrides.Split(','));
                }

                AllowedDataTypes.Add(dataType.ContentType, dataType);
            }
        }

        /// <summary>
        /// 	Determines the type of the data.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <returns></returns>
        public override DataType DetermineDataType(CrawlRequest<TArachnodeDAO> crawlRequest)
        {
            DataType dataType;

            string extension = UserDefinedFunctions.ExtractFileExtension(crawlRequest.Discovery.Uri.AbsoluteUri.ToLower()).Value;

            if (crawlRequest.WebClient.HttpWebResponse != null && !string.IsNullOrEmpty(crawlRequest.WebClient.HttpWebResponse.ContentType))
            {
                string contentType = crawlRequest.WebClient.HttpWebResponse.ContentType.Split(';')[0].ToLowerInvariant().Replace("\"", "");

                if (AllowedDataTypes.ContainsKey(contentType))
                {
                    dataType = DetermineDataType(contentType, extension);
                }
                else
                {
                    if (_contentTypesByName.ContainsKey(contentType))
                    {
                        dataType = new DataType(contentType, _contentTypesByName[contentType], DiscoveryType.None, extension, null, null);
                    }
                    else
                    {
                        dataType = new DataType(contentType, _contentTypesByName["UNKNOWN"], DiscoveryType.None, extension, null, null);
                    }
                }
            }
            else
            {
                dataType = new DataType(null, _contentTypesByName["UNKNOWN"], DiscoveryType.None, null, null, null);
            }

            return dataType;
        }

        /// <summary>
        /// 	Determines the type of the data.
        /// </summary>
        /// <param name = "contentType">Type of the content.</param>
        /// <param name = "extension">The extension.</param>
        /// <returns></returns>
        public override DataType DetermineDataType(string contentType, string extension)
        {
            if (string.IsNullOrEmpty(contentType) || string.IsNullOrEmpty(extension))
            {
                return new DataType();
            }

            if (contentType.Contains(";"))
            {
                contentType = contentType.Split(';')[0].ToLowerInvariant().Replace("\"", "");
            }

            DataType dataType = new DataType(contentType, AllowedDataTypes[contentType].ContentTypeID, AllowedDataTypes[contentType].DiscoveryType, AllowedDataTypes[contentType].FullTextIndexType, AllowedDataTypes[contentType].Synonyms, AllowedDataTypes[contentType].Overrides);

            if (extension != "UNKNOWN")
            {
                if (dataType.Overrides != null)
                {
                    if (dataType.Overrides.Contains(extension))
                    {
                        dataType.FullTextIndexType = extension;
                    }
                }
            }

            return dataType;
        }
    }
}