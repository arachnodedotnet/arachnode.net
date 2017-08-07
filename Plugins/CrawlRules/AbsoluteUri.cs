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
using System.Text.RegularExpressions;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.AbstractClasses;

#endregion

namespace Arachnode.Plugins.CrawlRules
{
    public class AbsoluteUri<TArachnodeDAO> : ACrawlRule<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        private static readonly Regex _detectRepeatingAbsoluteUrisRegex = new Regex(@"-.*(/[^/]+)/[^/]+\1/[^/]+\1\/", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //private static readonly Regex _detectAbsoluteUrisWithNamedAnchorRegex = new Regex(@"#", RegexOptions.Compiled | RegexOptions.RightToLeft);
        //private static readonly Regex _detectAbsoluteUrisWithQueryStringRegex = new Regex(@"\?", RegexOptions.Compiled);

        private bool _disallowNamedAnchors;
        private bool _disallowQueryStrings;
        private int _maximumDirectoryDepth;
        private bool _negateIsDisallowedForAbsoluteUri;
        private bool _negateIsDisallowedForDomain;
        private bool _negateIsDisallowedForExtension;
        private bool _negateIsDisallowedForFileExtension;
        private bool _negateIsDisallowedForHost;
        private bool _negateIsDisallowedForMaximumDirectoryDepth;
        private bool _negateIsDisallowedForNamedAnchor;
        private bool _negateIsDisallowedForQueryString;
        private bool _negateIsDisallowedForRepeatingAbsoluteUri;
        private bool _negateIsDisallowedForScheme;

        public AbsoluteUri(ApplicationSettings applicationSettings, WebSettings webSettings)
            : base(applicationSettings, webSettings)
        {
        }

        /// <summary>
        /// 	Assigns the additional parameters.
        /// </summary>
        /// <param name = "xmlNode">The XML node.</param>
        public override void AssignSettings(Dictionary<string, string> settings)
        {
            _negateIsDisallowedForAbsoluteUri = bool.Parse(settings["NegateIsDisallowedForAbsoluteUri"]);
            _negateIsDisallowedForDomain = bool.Parse(settings["NegateIsDisallowedForDomain"]);
            _negateIsDisallowedForExtension = bool.Parse(settings["NegateIsDisallowedForExtension"]);
            _negateIsDisallowedForFileExtension = bool.Parse(settings["NegateIsDisallowedForFileExtension"]);
            _negateIsDisallowedForHost = bool.Parse(settings["NegateIsDisallowedForHost"]);
            _negateIsDisallowedForScheme = bool.Parse(settings["NegateIsDisallowedForScheme"]);
            _negateIsDisallowedForRepeatingAbsoluteUri = bool.Parse(settings["NegateIsDisallowedForRepeatingAbsoluteUri"]);
            _negateIsDisallowedForMaximumDirectoryDepth = bool.Parse(settings["NegateIsDisallowedForMaximumDirectoryDepth"]);
            _negateIsDisallowedForNamedAnchor = bool.Parse(settings["NegateIsDisallowedForNamedAnchor"]);
            _negateIsDisallowedForQueryString = bool.Parse(settings["NegateIsDisallowedForQueryString"]);
            _disallowNamedAnchors = bool.Parse(settings["DisallowNamedAnchors"]);
            _disallowQueryStrings = bool.Parse(settings["DisallowQueryStrings"]);
            _maximumDirectoryDepth = int.Parse(settings["MaximumDirectoryDepth"]);
        }

        /// <summary>
        /// 	Determines whether the specified crawl request is disallowed.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        /// <returns>
        /// 	<c>true</c> if the specified crawl request is disallowed; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsDisallowed(CrawlRequest<TArachnodeDAO> crawlRequest, IArachnodeDAO arachnodeDAO)
        {
            return IsDisallowed(crawlRequest, crawlRequest.Discovery.Uri);
        }

        /// <summary>
        /// 	Determines whether the specified crawl request is disallowed.
        /// </summary>
        /// <param name = "discovery">The discovery.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        /// <returns>
        /// 	<c>true</c> if the specified crawl request is disallowed; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsDisallowed(Discovery<TArachnodeDAO> discovery, IArachnodeDAO arachnodeDAO)
        {
            return IsDisallowed(discovery, discovery.Uri);
        }

        /// <summary>
        /// 	Determines whether the specified a disallowed is disallowed.
        /// </summary>
        /// <param name = "aDisallowed">A disallowed.</param>
        /// <param name = "uri">The URI.</param>
        /// <returns>
        /// 	<c>true</c> if the specified a disallowed is disallowed; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsDisallowed(ADisallowed<TArachnodeDAO> aDisallowed, Uri uri)
        {
            bool isDisallowed = false;

            aDisallowed.OutputIsDisallowedReason = OutputIsDisallowedReason;

            #region Disallowed by AbsoluteUri.

            if (UserDefinedFunctions.IsDisallowedForAbsoluteUri(uri.AbsoluteUri, true, false))
            {
                isDisallowed = true;
            }

            if (_negateIsDisallowedForAbsoluteUri)
            {
                isDisallowed = !isDisallowed;
            }

            if (isDisallowed)
            {
                aDisallowed.IsDisallowedReason = "Disallowed by AbsoluteUri.";

                return true;
            }

            #endregion

            #region Disallowed by Domain.

            if (UserDefinedFunctions.IsDisallowedForDomain(uri.AbsoluteUri, false, false))
            {
                isDisallowed = true;
            }

            if (_negateIsDisallowedForDomain)
            {
                isDisallowed = !isDisallowed;
            }

            if (isDisallowed)
            {
                aDisallowed.IsDisallowedReason = "Disallowed by Domain.";

                return true;
            }

            #endregion

            #region Disallowed by Extension.

            if (UserDefinedFunctions.IsDisallowedForExtension(uri.AbsoluteUri, false, false))
            {
                isDisallowed = true;
            }

            if (_negateIsDisallowedForExtension)
            {
                isDisallowed = !isDisallowed;
            }

            if (isDisallowed)
            {
                aDisallowed.IsDisallowedReason = "Disallowed by Extension.";

                return true;
            }

            #endregion

            #region Disallowed by FileExtension.

            if (UserDefinedFunctions.IsDisallowedForFileExtension(uri.AbsoluteUri, false, false))
            {
                isDisallowed = true;
            }

            if (_negateIsDisallowedForFileExtension)
            {
                isDisallowed = !isDisallowed;
            }

            if (isDisallowed)
            {
                aDisallowed.IsDisallowedReason = "Disallowed by FileExtension.";

                return true;
            }

            #endregion

            #region Disallowed by Host.

            if (UserDefinedFunctions.IsDisallowedForHost(uri.AbsoluteUri, false, false))
            {
                isDisallowed = true;
            }

            if (_negateIsDisallowedForHost)
            {
                isDisallowed = !isDisallowed;
            }

            if (isDisallowed)
            {
                aDisallowed.IsDisallowedReason = "Disallowed by Host.";

                return true;
            }

            #endregion

            #region Disallowed by Scheme.

            if (UserDefinedFunctions.IsDisallowedForScheme(uri.AbsoluteUri, false, false))
            {
                isDisallowed = true;
            }

            if (_negateIsDisallowedForScheme)
            {
                isDisallowed = !isDisallowed;
            }

            if (isDisallowed)
            {
                aDisallowed.IsDisallowedReason = "Disallowed by Scheme.";

                return true;
            }

            #endregion

            #region Disallowed by repeating AbsoluteUri.

            if (_detectRepeatingAbsoluteUrisRegex.IsMatch(uri.AbsoluteUri))
            {
                isDisallowed = true;
            }

            if (_negateIsDisallowedForRepeatingAbsoluteUri)
            {
                isDisallowed = !isDisallowed;
            }

            if (isDisallowed)
            {
                aDisallowed.IsDisallowedReason = "Disallowed by repeating AbsoluteUri.";

                return true;
            }

            #endregion

            #region Disallowed by maximum directory depth.

            if (uri.AbsoluteUri.Replace("/", string.Empty).Length < uri.AbsoluteUri.Length - (_maximumDirectoryDepth + 2))
            {
                isDisallowed = true;
            }

            if (_negateIsDisallowedForMaximumDirectoryDepth)
            {
                isDisallowed = !isDisallowed;
            }

            if (isDisallowed)
            {
                aDisallowed.IsDisallowedReason = "Disallowed by maximum directory depth.";

                return true;
            }

            #endregion

            #region Disallowed by named anchor.

            //if (_disallowNamedAnchors && _detectAbsoluteUrisWithNamedAnchorRegex.IsMatch(discovery.Uri.AbsoluteUri))
            if (_disallowNamedAnchors && !string.IsNullOrEmpty(uri.Fragment))
            {
                isDisallowed = true;
            }

            if (_negateIsDisallowedForNamedAnchor)
            {
                isDisallowed = !isDisallowed;
            }

            if (isDisallowed)
            {
                aDisallowed.IsDisallowedReason = "Disallowed by named anchor.";

                return true;
            }

            #endregion

            #region Disallowed by query string.

            //if (_disallowQueryStrings && _detectAbsoluteUrisWithQueryStringRegex.IsMatch(discovery.Uri.AbsoluteUri))
            if (_disallowQueryStrings && !string.IsNullOrEmpty(uri.Query))
            {
                isDisallowed = true;
            }

            if (_negateIsDisallowedForQueryString)
            {
                isDisallowed = !isDisallowed;
            }

            if (isDisallowed)
            {
                aDisallowed.IsDisallowedReason = "Disallowed by query string.";

                return true;
            }

            #endregion

            return false;
        }

        /// <summary>
        /// 	Stops this instance.
        /// </summary>
        public override void Stop()
        {
        }
    }
}