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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Arachnode.Configuration;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.DataSource;
using Arachnode.Security;
using Arachnode.SiteCrawler.Core;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.AbstractClasses;
using Arachnode.SiteCrawler.Value.Enums;

#endregion

namespace Arachnode.SiteCrawler.Managers
{
    public class DiscoveryManager<TArachnodeDAO> : ADiscoveryManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        //ANODET: Add support for 'onclick'.

        public DiscoveryManager(ApplicationSettings applicationSettings, WebSettings webSettings, Cache<TArachnodeDAO> cache, ActionManager<TArachnodeDAO> actionManager, CacheManager<TArachnodeDAO> cacheManager, MemoryManager<TArachnodeDAO> memoryManager, RuleManager<TArachnodeDAO> ruleManager) : base(applicationSettings, webSettings, cache, actionManager, cacheManager, memoryManager, ruleManager)
        {
        }

        public override Regex EmailAddressRegex
        {
            get { return _emailAddressRegex; }
        }

        public override Regex FileOrImageRegex
        {
            get { return _fileOrImageRegex; }
        }

        public override Regex HyperLinkRegex
        {
            get { return _hyperLinkRegex; }
        }

        /// <summary>
        /// 	Assigns the email address discoveries.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        public override void AssignEmailAddressDiscoveries(CrawlRequest<TArachnodeDAO> crawlRequest, IArachnodeDAO arachnodeDAO)
        {
            MatchCollection matchCollection = EmailAddressRegex.Matches(crawlRequest.Html);

            if (matchCollection.Count != 0)
            {
                var baseUri = new Uri(crawlRequest.Discovery.Uri.AbsoluteUri.TrimEnd('/').TrimEnd('#'));

                foreach (Match match in matchCollection)
                {
                    if (match.Groups["Tag"].Value.ToLowerInvariant() == "base")
                    {
                        string groupValue = GetGroupValue(match, "HyperLink");
                        if (!Uri.TryCreate(groupValue, UriKind.Absolute, out baseUri))
                        {
                            baseUri = new Uri(crawlRequest.Discovery.Uri.AbsoluteUri.TrimEnd('/').TrimEnd('#'));

                            break;
                        }
                    }
                }

                foreach (Match match in matchCollection)
                {
                    Uri emailAddressDiscovery;

                    if (Uri.TryCreate(Uri.UriSchemeMailto + Uri.SchemeDelimiter + match.Value, UriKind.Absolute, out emailAddressDiscovery))
                    {
                        if (!emailAddressDiscovery.IsAbsoluteUri)
                        {
                            Uri.TryCreate(baseUri, emailAddressDiscovery, out emailAddressDiscovery);
                        }

                        if (emailAddressDiscovery != null && !crawlRequest.Discoveries.EmailAddresses.ContainsKey(emailAddressDiscovery.AbsoluteUri))
                        {
                            if (emailAddressDiscovery.HostNameType != UriHostNameType.Unknown && emailAddressDiscovery.IsAbsoluteUri)
                            {
                                if (!IsDiscoveryRestricted(crawlRequest, emailAddressDiscovery.AbsoluteUri))
                                {
                                    Discovery<TArachnodeDAO> discovery = _cache.GetDiscovery(emailAddressDiscovery, arachnodeDAO);

                                    crawlRequest.Tag = match.Value;

                                    crawlRequest.Discoveries.EmailAddresses.Add(emailAddressDiscovery.AbsoluteUri, discovery);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 	Assigns the hyper link discoveries.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        public override void AssignHyperLinkDiscoveries(CrawlRequest<TArachnodeDAO> crawlRequest, IArachnodeDAO arachnodeDAO)
        {
            MatchCollection matchCollection = HyperLinkRegex.Matches(crawlRequest.Html);
            
            if (matchCollection.Count != 0)
            {
                var baseUri = new Uri(crawlRequest.Discovery.Uri.AbsoluteUri.TrimEnd('/').TrimEnd('#'), UriKind.Absolute);

                foreach (Match match in matchCollection)
                {
                    if (match.Groups["Tag"].Value.ToLowerInvariant() == "base")
                    {
                        if (!Uri.TryCreate(match.Groups["HyperLink"].Value, UriKind.Absolute, out baseUri))
                        {
                            baseUri = new Uri(crawlRequest.Discovery.Uri.AbsoluteUri.TrimEnd('/').TrimEnd('#'), UriKind.Absolute);

                            break;
                        }
                    }
                }

                var uriBuilder = new UriBuilder(baseUri);
                if (!baseUri.AbsoluteUri.EndsWith("/") && !baseUri.Segments[baseUri.Segments.Length - 1].Contains("."))
                {
                    baseUri = new Uri(baseUri.AbsoluteUri + "/");
                }

                foreach (Match match in matchCollection)
                {
                    Uri hyperLinkDiscovery;
                    string groupValue = GetGroupValue(match, "HyperLink").TrimEnd('/').TrimEnd('#');
                    if (Uri.TryCreate(groupValue, UriKind.RelativeOrAbsolute, out hyperLinkDiscovery))
                    {
                        if (!hyperLinkDiscovery.IsAbsoluteUri)
                        {
                            if (groupValue.StartsWith("?"))
                            {
                                uriBuilder.Query = groupValue.TrimStart('?');

                                hyperLinkDiscovery = uriBuilder.Uri;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(groupValue))
                                {
                                    Uri.TryCreate(baseUri, hyperLinkDiscovery, out hyperLinkDiscovery);
                                }
                                else
                                {
                                    hyperLinkDiscovery = new Uri(baseUri.AbsoluteUri.TrimEnd('/').TrimEnd('#'));
                                }
                            }
                        }

                        if (hyperLinkDiscovery != null && !crawlRequest.Discoveries.HyperLinks.ContainsKey(hyperLinkDiscovery.AbsoluteUri))
                        {
                            if (hyperLinkDiscovery.HostNameType != UriHostNameType.Unknown && hyperLinkDiscovery.IsAbsoluteUri)
                            {
                                if (!IsDiscoveryRestricted(crawlRequest, hyperLinkDiscovery.AbsoluteUri))
                                {
                                    if (UserDefinedFunctions.AllowedSchemes.ContainsKey(hyperLinkDiscovery.Scheme) && hyperLinkDiscovery.Scheme != "mailto")
                                    {
                                        Discovery<TArachnodeDAO> discovery = _cache.GetDiscovery(hyperLinkDiscovery, arachnodeDAO);

                                        crawlRequest.Tag = match.Value;

                                        crawlRequest.Discoveries.HyperLinks.Add(hyperLinkDiscovery.AbsoluteUri, discovery);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 	Assigns the file and image discoveries.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        public override void AssignFileAndImageDiscoveries(CrawlRequest<TArachnodeDAO> crawlRequest, IArachnodeDAO arachnodeDAO)
        {
            MatchCollection matchCollection = FileOrImageRegex.Matches(crawlRequest.Html);

            if (matchCollection.Count != 0)
            {
                var baseUri = new Uri(crawlRequest.Discovery.Uri.AbsoluteUri.TrimEnd('/').TrimEnd('#'));

                foreach (Match match in matchCollection)
                {
                    if (match.Groups["Tag"].Value.ToLowerInvariant() == "base")
                    {
                        if (!Uri.TryCreate(match.Groups["HyperLink"].Value, UriKind.Absolute, out baseUri))
                        {
                            baseUri = new Uri(crawlRequest.Discovery.Uri.AbsoluteUri.TrimEnd('/').TrimEnd('#'));

                            break;
                        }
                    }
                }

                var uriBuilder = new UriBuilder(baseUri);
                if (!baseUri.AbsoluteUri.EndsWith("/") && !baseUri.Segments[baseUri.Segments.Length - 1].Contains("."))
                {
                    baseUri = new Uri(baseUri.AbsoluteUri + "/");
                }

                foreach (Match match in matchCollection)
                {
                    if (IsFileAndImageMatchValid(match))
                    {
                        Uri fileOrImageDiscovery;
                        //ANODET: Check on this!!!  (Replace???). (Version 2.5+)
                        string groupValue = GetGroupValue(match, "FileOrImage").Replace("\"", "").Replace("'", "").Split(' ')[0].TrimEnd('/').TrimEnd('#');
                        if (Uri.TryCreate(groupValue, UriKind.RelativeOrAbsolute, out fileOrImageDiscovery))
                        {
                            if (!fileOrImageDiscovery.IsAbsoluteUri)
                            {
                                if (groupValue.StartsWith("?"))
                                {
                                    uriBuilder.Query = groupValue.TrimStart('?');

                                    fileOrImageDiscovery = uriBuilder.Uri;
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(groupValue))
                                    {
                                        Uri.TryCreate(baseUri, fileOrImageDiscovery, out fileOrImageDiscovery);
                                    }
                                    else
                                    {
                                        fileOrImageDiscovery = new Uri(baseUri.AbsoluteUri.TrimEnd('/').TrimEnd('#'));
                                    }
                                }
                            }

                            if (fileOrImageDiscovery != null && !crawlRequest.Discoveries.FilesAndImages.ContainsKey(fileOrImageDiscovery.AbsoluteUri))
                            {
                                if(fileOrImageDiscovery.HostNameType != UriHostNameType.Unknown && fileOrImageDiscovery.IsAbsoluteUri)
                                {
                                    if (!IsDiscoveryRestricted(crawlRequest, fileOrImageDiscovery.AbsoluteUri))
                                    {
                                        Discovery<TArachnodeDAO> discovery = _cache.GetDiscovery(fileOrImageDiscovery, arachnodeDAO);

                                        discovery.ExpectFileOrImage = true;

                                        crawlRequest.Tag = match.Value;

                                        crawlRequest.Discoveries.FilesAndImages.Add(fileOrImageDiscovery.AbsoluteUri, discovery);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Decodes a match value such that html encoded references will match the base href (if used).
        /// </summary>
        /// <param name="match"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public override string GetGroupValue(Match match, string groupName)
        {
            Group group = match.Groups[groupName];

            string value = group.Value;

            if (!string.IsNullOrEmpty(value))
            {
                value = value.Trim();
            }

            return HttpUtility.HtmlDecode(value);
        }

        /// <summary>
        /// The regular expression balances speed and accuracy and is correct the vast majority of the time.
        /// The grouping of the tags and the attribute names allows mixing and matching, as many browsers will account for invalid HTML and present the user with "This is what the HTML developer really meant".
        /// HTML such as "script source='\something'" is allowed when 'src' is the syntactically correct attribute name.
        /// There are cases where due to backreferencing (\3) which allows " AND ' to designate match boundaries that may occasionally result in matches contanining extraneous characters which may result in false positives for File and/or Image matches.
        /// If an AbsoluteUri is dicovered as a File or Image, it will not be allowed to be discovered as a WebPage.
        /// As the Tag and Attribute name are known after regular expression evaluation, it is beneficial to evaluate for proper tag and attribute name combinations.
        /// Evalating after regular expression evaluation is much faster than providing multiple regular expressions which specify concrete sets thereby requiring parsing the HTML multiple times.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public override bool IsFileAndImageMatchValid(Match match)
        {
            if (match.Groups["Tag"].Value.ToLowerInvariant() == "script" && match.Groups["AttributeName"].Value.ToLowerInvariant() == "href")
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 	Determines whether the specified crawl request is restricted.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "absoluteUri">The absolute URI.</param>
        /// <returns>
        /// 	<c>true</c> if the specified crawl request is restricted; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsCrawlRestricted(CrawlRequest<TArachnodeDAO> crawlRequest, string absoluteUri)
        {
            return IsRestricted(crawlRequest, absoluteUri, crawlRequest.RestrictCrawlTo);
        }

        /// <summary>
        /// 	Determines whether [is discovery restricted] [the specified crawl request].
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "absoluteUri">The absolute URI.</param>
        /// <returns>
        /// 	<c>true</c> if [is discovery restricted] [the specified crawl request]; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsDiscoveryRestricted(CrawlRequest<TArachnodeDAO> crawlRequest, string absoluteUri)
        {
            return IsRestricted(crawlRequest, absoluteUri, crawlRequest.RestrictDiscoveriesTo);
        }

        /// <summary>
        /// 	Determines whether the specified crawl request is restricted.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "absoluteUri">The absolute URI.</param>
        /// <param name = "uriClassificationType">Type of the URI classification.</param>
        /// <returns>
        /// 	<c>true</c> if the specified crawl request is restricted; otherwise, <c>false</c>.
        /// </returns>
        protected override bool IsRestricted(CrawlRequest<TArachnodeDAO> crawlRequest, string absoluteUri, short uriClassificationType)
        {
            if (uriClassificationType == (short)UriClassificationType.None)
            {
                return false;
            }

            if ((uriClassificationType & (short)UriClassificationType.Domain) == (short)UriClassificationType.Domain)
            {
                if (UserDefinedFunctions.ExtractDomain(crawlRequest.Discovery.Uri.AbsoluteUri) != UserDefinedFunctions.ExtractDomain(absoluteUri))
                {
                    return true;
                }
            }

            if ((uriClassificationType & (short)UriClassificationType.Extension) == (short)UriClassificationType.Extension)
            {
                if (UserDefinedFunctions.ExtractExtension(crawlRequest.Discovery.Uri.AbsoluteUri, false) != UserDefinedFunctions.ExtractExtension(absoluteUri, false))
                {
                    return true;
                }
            }

            if ((uriClassificationType & (short)UriClassificationType.FileExtension) == (short)UriClassificationType.FileExtension)
            {
                if (UserDefinedFunctions.ExtractFileExtension(crawlRequest.Discovery.Uri.AbsoluteUri) != UserDefinedFunctions.ExtractFileExtension(absoluteUri))
                {
                    return true;
                }
            }

            if ((uriClassificationType & (short)UriClassificationType.Host) == (short)UriClassificationType.Host)
            {
                if (UserDefinedFunctions.ExtractHost(crawlRequest.Discovery.Uri.AbsoluteUri) != UserDefinedFunctions.ExtractHost(absoluteUri))
                {
                    return true;
                }
            }

            if ((uriClassificationType & (short)UriClassificationType.Scheme) == (short)UriClassificationType.Scheme)
            {
                if (UserDefinedFunctions.ExtractScheme(crawlRequest.Discovery.Uri.AbsoluteUri, false) != UserDefinedFunctions.ExtractScheme(absoluteUri, false))
                {
                    return true;
                }
            }

            if (uriClassificationType >= (short)UriClassificationType.OriginalDirectoryLevelUp)
            {
                string crawlRequestOriginatorAbsoluteUriDirectory;

                if (crawlRequest.Originator == null)
                {
                    crawlRequestOriginatorAbsoluteUriDirectory = Path.GetDirectoryName(HttpUtility.HtmlEncode(crawlRequest.Parent.Uri.LocalPath));
                }
                else
                {
                    crawlRequestOriginatorAbsoluteUriDirectory = Path.GetDirectoryName(HttpUtility.HtmlEncode(crawlRequest.Originator.Uri.LocalPath));
                }

                string absoluteUriDirectory = Path.GetDirectoryName(HttpUtility.HtmlEncode(new Uri(absoluteUri).LocalPath));

                if (crawlRequestOriginatorAbsoluteUriDirectory == null)
                {
                    crawlRequestOriginatorAbsoluteUriDirectory = "\\";
                }

                if (absoluteUriDirectory == null)
                {
                    absoluteUriDirectory = "\\";
                }

                if ((uriClassificationType & (short)UriClassificationType.OriginalDirectoryLevelUp) == (short)UriClassificationType.OriginalDirectoryLevelUp)
                {
                    if ((uriClassificationType & (short)UriClassificationType.OriginalDirectory) == (short)UriClassificationType.OriginalDirectory || (uriClassificationType & (short)UriClassificationType.OriginalDirectoryLevel) == (short)UriClassificationType.OriginalDirectoryLevel)
                    {
                        if ((uriClassificationType & (short)UriClassificationType.OriginalDirectory) == (short)UriClassificationType.OriginalDirectory)
                        {
                            if (crawlRequestOriginatorAbsoluteUriDirectory.Length - crawlRequestOriginatorAbsoluteUriDirectory.Replace("\\", string.Empty).Length <= absoluteUriDirectory.Length - absoluteUriDirectory.Replace("\\", string.Empty).Length)
                            {
                                if (crawlRequestOriginatorAbsoluteUriDirectory != absoluteUriDirectory && absoluteUriDirectory != "\\")
                                {
                                    return true;
                                }

                                return false;
                            }
                        }

                        if ((uriClassificationType & (short)UriClassificationType.OriginalDirectoryLevel) == (short)UriClassificationType.OriginalDirectoryLevel)
                        {
                            if (crawlRequestOriginatorAbsoluteUriDirectory.Length - crawlRequestOriginatorAbsoluteUriDirectory.Replace("\\", string.Empty).Length < absoluteUriDirectory.Length - absoluteUriDirectory.Replace("\\", string.Empty).Length)
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (crawlRequestOriginatorAbsoluteUriDirectory.Length - crawlRequestOriginatorAbsoluteUriDirectory.Replace("\\", string.Empty).Length <= absoluteUriDirectory.Length - absoluteUriDirectory.Replace("\\", string.Empty).Length)
                        {
                            if (crawlRequest.Discovery.Uri.AbsoluteUri != absoluteUri)
                            {
                                return true;
                            }

                            if (crawlRequest.CurrentDepth == 1)
                            {
                                crawlRequest.IsStorable = false;

                                return false;
                            }
                        }
                    }
                }

                if ((uriClassificationType & (short)UriClassificationType.OriginalDirectoryLevelDown) == (short)UriClassificationType.OriginalDirectoryLevelDown)
                {
                    if ((uriClassificationType & (short)UriClassificationType.OriginalDirectory) == (short)UriClassificationType.OriginalDirectory || (uriClassificationType & (short)UriClassificationType.OriginalDirectoryLevel) == (short)UriClassificationType.OriginalDirectoryLevel)
                    {
                        if ((uriClassificationType & (short)UriClassificationType.OriginalDirectory) == (short)UriClassificationType.OriginalDirectory)
                        {
                            if (crawlRequestOriginatorAbsoluteUriDirectory.Length - crawlRequestOriginatorAbsoluteUriDirectory.Replace("\\", string.Empty).Length >= absoluteUriDirectory.Length - absoluteUriDirectory.Replace("\\", string.Empty).Length)
                            {
                                if (crawlRequestOriginatorAbsoluteUriDirectory != absoluteUriDirectory)
                                {
                                    return true;
                                }
                            }

                            if (!absoluteUriDirectory.StartsWith(crawlRequestOriginatorAbsoluteUriDirectory))
                            {
                                return true;
                            }
                        }

                        if ((uriClassificationType & (short)UriClassificationType.OriginalDirectoryLevel) == (short)UriClassificationType.OriginalDirectoryLevel)
                        {
                            if (crawlRequestOriginatorAbsoluteUriDirectory.Length - crawlRequestOriginatorAbsoluteUriDirectory.Replace("\\", string.Empty).Length > absoluteUriDirectory.Length - absoluteUriDirectory.Replace("\\", string.Empty).Length)
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (crawlRequestOriginatorAbsoluteUriDirectory.Length - crawlRequestOriginatorAbsoluteUriDirectory.Replace("\\", string.Empty).Length >= absoluteUriDirectory.Length - absoluteUriDirectory.Replace("\\", string.Empty).Length)
                        {
                            if (crawlRequest.Discovery.Uri.AbsoluteUri != absoluteUri)
                            {
                                return true;
                            }

                            if (crawlRequest.CurrentDepth == 1)
                            {
                                crawlRequest.IsStorable = false;

                                return false;
                            }
                        }
                    }
                }

                if ((uriClassificationType & (short)UriClassificationType.OriginalDirectoryLevelDown) != (short)UriClassificationType.OriginalDirectoryLevelDown && (uriClassificationType & (short)UriClassificationType.OriginalDirectoryLevelDown) != (short)UriClassificationType.OriginalDirectoryLevelDown)
                {
                    if ((uriClassificationType & (short)UriClassificationType.OriginalDirectory) == (short)UriClassificationType.OriginalDirectory)
                    {
                        if (crawlRequestOriginatorAbsoluteUriDirectory != absoluteUriDirectory)
                        {
                            return true;
                        }
                    }

                    if ((uriClassificationType & (short)UriClassificationType.OriginalDirectoryLevel) == (short)UriClassificationType.OriginalDirectoryLevel)
                    {
                        if (crawlRequestOriginatorAbsoluteUriDirectory.Length - crawlRequestOriginatorAbsoluteUriDirectory.Replace("\\", string.Empty).Length != absoluteUriDirectory.Length - absoluteUriDirectory.Replace("\\", string.Empty).Length)
                        {
                            return true;
                        }
                    }
                }
            }


            return false;
        }

        /// <summary>
        /// 	Manages the discovery.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "discoveryState">State of the discovery.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        public override void ManageDiscovery(CrawlRequest<TArachnodeDAO> crawlRequest, DiscoveryState discoveryState, IArachnodeDAO arachnodeDAO)
        {
            switch (discoveryState)
            {
                case DiscoveryState.PreRequest:

                    crawlRequest.Discovery.DiscoveryState = DiscoveryState.PreRequest;

                    crawlRequest.Crawl.Crawler.Cache.AddDiscoveryToInternalCache(crawlRequest.Discovery.CacheKey.AbsoluteUri + ApplicationSettings.UniqueIdentifier, crawlRequest.Discovery);

                    if (_memoryManager.IsUsingDesiredMaximumMemoryInMegabytes(false))
                    {
                        //we need to update the Discovery in the Database...
                        if (ApplicationSettings.InsertDiscoveries && crawlRequest.InsertDiscovery)
                        {
                            arachnodeDAO.InsertDiscovery(crawlRequest.Discovery.ID, crawlRequest.Discovery.CacheKey.AbsoluteUri + ApplicationSettings.UniqueIdentifier, (int)crawlRequest.Discovery.DiscoveryState, (int)crawlRequest.Discovery.DiscoveryType, crawlRequest.Discovery.ExpectFileOrImage, crawlRequest.Discovery.NumberOfTimesDiscovered);
                        }
                    }
                    break;
                case DiscoveryState.PostRequest:

                    crawlRequest.Discovery.DiscoveryState = DiscoveryState.PostRequest;
                    crawlRequest.Discovery.DiscoveryType = crawlRequest.DataType.DiscoveryType;

                    crawlRequest.Crawl.Crawler.Cache.AddDiscoveryToInternalCache(crawlRequest.Discovery.CacheKey.AbsoluteUri + ApplicationSettings.UniqueIdentifier, crawlRequest.Discovery);
                    break;
                case DiscoveryState.Discovered:
                    crawlRequest.Discovery.DiscoveryState = DiscoveryState.Discovered;

                    crawlRequest.Crawl.Crawler.Cache.AddDiscoveryToInternalCache(crawlRequest.Discovery.CacheKey.AbsoluteUri + ApplicationSettings.UniqueIdentifier, crawlRequest.Discovery);

                    //we need to update the Discovery in the Database...
                    if (ApplicationSettings.InsertDiscoveries && crawlRequest.InsertDiscovery)
                    {
                        arachnodeDAO.InsertDiscovery(crawlRequest.Discovery.ID, crawlRequest.Discovery.CacheKey.AbsoluteUri + ApplicationSettings.UniqueIdentifier, (int)crawlRequest.Discovery.DiscoveryState, (int)crawlRequest.Discovery.DiscoveryType, crawlRequest.Discovery.ExpectFileOrImage, crawlRequest.Discovery.NumberOfTimesDiscovered);
                    }

                    CloseAndDisposeManagedDiscovery(crawlRequest, arachnodeDAO);

                    break;
            }
        }

        public override bool WasCrawlRequestRedirected(CrawlRequest<TArachnodeDAO> crawlRequest)
        {
            if (crawlRequest.WebClient != null && crawlRequest.WebClient.HttpWebResponse != null)
            {
                //http://msdn.microsoft.com/en-us/library/system.net.httpstatuscode.aspx
                var statusCode = (int)crawlRequest.WebClient.HttpWebResponse.StatusCode;
                if ((statusCode >= 300 && statusCode <= 303) || statusCode == 307 || UserDefinedFunctions.ExtractHost(crawlRequest.WebClient.HttpWebRequest.RequestUri.AbsoluteUri).Value != UserDefinedFunctions.ExtractHost(crawlRequest.WebClient.HttpWebResponse.ResponseUri.AbsoluteUri).Value)
                {
                    return true;
                }
            }

            return false;
        }

        public override void CloseAndDisposeManagedDiscovery(CrawlRequest<TArachnodeDAO> crawlRequest, IArachnodeDAO arachnodeDAO)
        {
            try
            {
                if (crawlRequest.ManagedDiscovery != null)
                {
                    if (crawlRequest.ManagedDiscovery is ManagedFile)
                    {
                    }
                    else if (crawlRequest.ManagedDiscovery is ManagedImage)
                    {
                        var managedImage = ((ManagedImage) crawlRequest.ManagedDiscovery);

                        if (managedImage.Image != null)
                        {
                            managedImage.Image.Dispose();
                        }
                    }
                    if (crawlRequest.ManagedDiscovery is ManagedWebPage)
                    {
                        var managedWebPage = ((ManagedWebPage) crawlRequest.ManagedDiscovery);

                        if (managedWebPage.StreamWriter != null)
                        {
                            managedWebPage.StreamWriter.Close();
                            managedWebPage.StreamWriter.Dispose();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                arachnodeDAO.InsertException(crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri, exception, false);
            }
        }

        /// <summary>
        /// 	Gets the discovery path.
        /// </summary>
        /// <param name = "downloadedDiscoveryDirectory">The downloaded discovery directory.</param>
        /// <param name = "absoluteUri">The absolute URI.</param>
        /// <returns></returns>
        public override string GetDiscoveryPath(string downloadedDiscoveryDirectory, string absoluteUri, string fullTextIndexType)
        {
            string directory = UserDefinedFunctions.ExtractDirectory(downloadedDiscoveryDirectory, absoluteUri).Value;

            DirectoryInfo directoryInfo = null;

            if (!Directory.Exists(directory))
            {
                //ANODET: View the history of this... :)
                //http://msdn.microsoft.com/en-us/library/aa365247(v=vs.85).aspx#file_and_directory_names
                //CON, PRN, AUX, NUL, COM1, COM2, COM3, COM4, COM5, COM6, COM7, COM8, COM9, LPT1, LPT2, LPT3, LPT4, LPT5, LPT6, LPT7, LPT8, and LPT9
                directory += "\\";
                directory = Regex.Replace(directory.ToLower(), "\\\\com\\d\\\\", "\\com_\\");
                directory = Regex.Replace(directory.ToLower(), "\\\\lpt\\d\\\\", "\\lpt_\\");
                directory = directory.ToLower().Replace("\\aux\\", "\\aux_\\").Replace("\\con\\", "\\con_\\").Replace("\\nul\\", "\\nul_\\").Replace("\\prn\\", "\\prn_\\");
                directory = directory.Substring(0, directory.Length - 1);

                directoryInfo = Directory.CreateDirectory(directory);
            }
            else
            {
                directoryInfo = new DirectoryInfo(directory);
            }

            return Path.Combine(directoryInfo.FullName, "_" + new Hash(absoluteUri) + fullTextIndexType);
        }

        public override string GetDiscoveryPath(string downloadedDiscoveryVirtualDirectory, string absoluteUri)
        {
            string[] uriSplit = absoluteUri.Split("/".ToCharArray());

            return Path.Combine(UserDefinedFunctions.ExtractDirectory(downloadedDiscoveryVirtualDirectory, absoluteUri).Value, uriSplit[uriSplit.Length - 1]).Replace("\\", "/").Replace("//", "/");
        }

        public override bool DoesDiscoveryExist(string discoveryPath)
        {
            if (File.Exists(discoveryPath))
            {
                return true;
            }

            foreach (string file in Directory.GetFiles(Path.GetDirectoryName(discoveryPath)))
            {
                if (Path.GetFileNameWithoutExtension(discoveryPath) == Path.GetFileNameWithoutExtension(file))
                {
                    return true;
                }
            }

            return false;
        }

        public override string GetDiscoveryExtension(string discoveryPath)
        {
            if (File.Exists(discoveryPath))
            {
                return Path.GetExtension(discoveryPath);
            }

            foreach (string file in Directory.GetFiles(Path.GetDirectoryName(discoveryPath)))
            {
                if (Path.GetFileNameWithoutExtension(discoveryPath) == Path.GetFileNameWithoutExtension(file))
                {
                    return Path.GetExtension(file);
                }
            }

            return null;
        }

        public override byte[] GetFileSource(string fileAbsoluteUriOrID, IArachnodeDAO arachnodeDAO)
        {
            if (ApplicationSettings.DownloadedFilesDirectory == null)
            {
                throw new Exception("_applicationSettings.DownloadedFilesDirectory is null.  This is usually the result of failing to initialize the Application configuration from the ArachnodeDAO.");
            }

            ArachnodeDataSet.FilesRow filesRow = arachnodeDAO.GetFile(fileAbsoluteUriOrID);

            if (filesRow != null)
            {
                if (filesRow.Source.Length != 0)
                {
                    return filesRow.Source;
                }
                else
                {
                    string discoveryPath = GetDiscoveryPath(ApplicationSettings.DownloadedFilesDirectory, filesRow.AbsoluteUri, filesRow.FullTextIndexType);

                    if (!File.Exists(discoveryPath))
                    {
                        throw new Exception("Could not find the File Source in the database or on disk.");
                    }

                    return File.ReadAllBytes(discoveryPath);
                }
            }

            return null;
        }

        public override byte[] GetImageSource(string imageAbsoluteUriOrID, IArachnodeDAO arachnodeDAO)
        {
            var managedImage = new ManagedImage();

            if (ApplicationSettings.DownloadedImagesDirectory == null)
            {
                throw new Exception("_applicationSettings.DownloadedImagesDirectory is null.  This is usually the result of failing to initialize the Application configuration from the ArachnodeDAO.");
            }

            ArachnodeDataSet.ImagesRow imagesRow = arachnodeDAO.GetImage(imageAbsoluteUriOrID);

            if (imagesRow != null)
            {
                if (imagesRow.Source.Length != 0)
                {
                    return imagesRow.Source;
                }
                else
                {
                    string discoveryPath = GetDiscoveryPath(ApplicationSettings.DownloadedImagesDirectory, imagesRow.AbsoluteUri, imagesRow.FullTextIndexType);

                    if (!File.Exists(discoveryPath))
                    {
                        throw new Exception("Could not find the Image Source in the database or on disk.");
                    }

                    return File.ReadAllBytes(discoveryPath);
                }
            }

            return null;
        }

        public override string GetWebPageSource(string webPageAbsoluteUriOrID, IArachnodeDAO arachnodeDAO)
        {
            if (ApplicationSettings.DownloadedWebPagesDirectory == null)
            {
                throw new Exception("_applicationSettings.DownloadedWebPagesDirectory is null.  This is usually the result of failing to initialize the Application configuration from the ArachnodeDAO.");
            }

            string webPageSource = null;

            ArachnodeDataSet.WebPagesRow webPagesRow = arachnodeDAO.GetWebPage(webPageAbsoluteUriOrID);

            if (webPagesRow != null)
            {
                if (webPagesRow.Source.Length != 0)
                {
                    webPageSource = Encoding.GetEncoding(webPagesRow.CodePage).GetString(webPagesRow.Source);
                }
                else
                {
                    string discoveryPath = GetDiscoveryPath(ApplicationSettings.DownloadedWebPagesDirectory, webPagesRow.AbsoluteUri, webPagesRow.FullTextIndexType);

                    if (!File.Exists(discoveryPath))
                    {
                        throw new Exception("Could not find the WebPage Source in the database or on disk.");
                    }

                    webPageSource = File.ReadAllText(discoveryPath, Encoding.GetEncoding(webPagesRow.CodePage));
                }
            }

            return webPageSource;
        }
    }
}