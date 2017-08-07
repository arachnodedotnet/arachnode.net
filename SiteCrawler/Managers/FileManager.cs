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
using System.IO;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.AbstractClasses;

#endregion

namespace Arachnode.SiteCrawler.Managers
{
    public class FileManager<TArachnodeDAO> : AFileManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        /// <summary>
        /// 	Initializes a new instance of the <see cref = "FileManager" /> class.
        /// </summary>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        public FileManager(ApplicationSettings applicationSettings, WebSettings webSettings, DiscoveryManager<TArachnodeDAO> discoveryManager, IArachnodeDAO arachnodeDAO) : base(applicationSettings, webSettings, discoveryManager, arachnodeDAO)
        {
        }

        /// <summary>
        /// 	Manages the file.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        public override void ManageFile(CrawlRequest<TArachnodeDAO> crawlRequest)
        {
            //we want to prevent files and images from being created by bugs when we haven't explicitly allowed file and image crawling,
            //but want to allow specific requests for file and image AbsoluteUris...
            if (ApplicationSettings.AssignFileAndImageDiscoveries || crawlRequest.Discovery.Uri.AbsoluteUri == crawlRequest.Parent.Uri.AbsoluteUri)
            {
                if (ApplicationSettings.InsertFiles && crawlRequest.IsStorable)
                {
                    crawlRequest.Discovery.ID = _arachnodeDAO.InsertFile(crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri, ApplicationSettings.InsertFileResponseHeaders ? crawlRequest.WebClient.HttpWebResponse.Headers.ToString() : null, ApplicationSettings.InsertFileSource ? crawlRequest.Data : new byte[] { }, crawlRequest.DataType.FullTextIndexType, ApplicationSettings.ClassifyAbsoluteUris);
                }

                if (crawlRequest.Discovery.ID.HasValue)
                {
                    ManagedFile managedFile = ManageFile(crawlRequest, crawlRequest.Discovery.ID.Value, crawlRequest.Discovery.Uri.AbsoluteUri, crawlRequest.Data, crawlRequest.DataType.FullTextIndexType, ApplicationSettings.ExtractFileMetaData, ApplicationSettings.InsertFileMetaData, ApplicationSettings.SaveDiscoveredFilesToDisk);

                    crawlRequest.ManagedDiscovery = managedFile;
                }
            }
        }

        /// <summary>
        /// 	Manages the file.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "fileID">The file ID.</param>
        /// <param name = "absoluteUri">The absolute URI.</param>
        /// <param name = "source">The source.</param>
        /// <param name = "fullTextIndexType">Full type of the text index.</param>
        /// <param name = "extractFileMetaData">if set to <c>true</c> [extract file meta data].</param>
        /// <param name = "insertFileMetaData">if set to <c>true</c> [insert file meta data].</param>
        /// <param name = "saveFileToDisk">if set to <c>true</c> [save file to disk].</param>
        /// <returns></returns>
        public override ManagedFile ManageFile(CrawlRequest<TArachnodeDAO> crawlRequest, long fileID, string absoluteUri, byte[] source, string fullTextIndexType, bool extractFileMetaData, bool insertFileMetaData, bool saveFileToDisk)
        {
            try
            {
                ManagedFile managedFile = new ManagedFile();

                if (extractFileMetaData || saveFileToDisk)
                {
                }

                if (extractFileMetaData)
                {
                    //try
                    //{
                    //RssFeed rssFeed = null;

                    //if (crawlRequest.DataType.ContentType == "text/xml")
                    //{
                    //    rssFeed = new RssFeed();

                    //    using (MemoryStream memoryStream = new MemoryStream(crawlRequest.Data))
                    //    {
                    //        RssReader reader = null;

                    //        try
                    //        {
                    //            reader = new RssReader(memoryStream);

                    //            RssElement rssElement = null;

                    //            do
                    //            {
                    //                rssElement = reader.Read();

                    //                if (rssElement is RssCategory)
                    //                {
                    //                    //rssFeed.Categories.Add()
                    //                }
                    //                if (rssElement is RssChannel)
                    //                {
                    //                    rssFeed.Channels.Add((RssChannel)rssElement);
                    //                }
                    //            } while (rssElement != null);
                    //            rssFeed.Version = reader.Version;
                    //        }
                    //        finally
                    //        {
                    //            reader.Close();
                    //        }
                    //    }
                    //}

                    if (insertFileMetaData)
                    {
                        //if(rssFeed != null)
                        //{

                        //}
                    }
                }

                if (saveFileToDisk)
                {
                    managedFile.DiscoveryPath = _discoveryManager.GetDiscoveryPath(ApplicationSettings.DownloadedFilesDirectory, absoluteUri, fullTextIndexType);

                    File.WriteAllBytes(managedFile.DiscoveryPath, source);
                }

                return managedFile;
            }
            catch (Exception exception)
            {
                //ANODET: Long paths...
#if !DEMO
                if (crawlRequest != null)
                {
                    _arachnodeDAO.InsertException(crawlRequest.Parent.Uri.AbsoluteUri, absoluteUri, exception, false);
                }
                else
                {
                    _arachnodeDAO.InsertException(null, absoluteUri, exception, false);
                }
#endif
            }

            return null;
        }
    }
}