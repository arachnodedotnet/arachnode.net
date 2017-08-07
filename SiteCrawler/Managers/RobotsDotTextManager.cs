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
using System.IO;
using System.Text.RegularExpressions;
using Arachnode.Configuration;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.AbstractClasses;

#endregion

namespace Arachnode.SiteCrawler.Managers
{
    /// <summary>
    /// 	Provides robots.txt parsing functionality.
    /// </summary>
    public class RobotsDotTextManager<TArachnodeDAO> : ARobotsDotTextManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        public RobotsDotTextManager(ApplicationSettings applicationSettings, WebSettings webSettings) : base(applicationSettings, webSettings)
        {
            
        }

        /// <summary>
        /// 	Parses the robots dot text.
        /// </summary>
        /// <param name = "baseUri">The base URI.</param>
        /// <param name = "robotsDotTextSource">The robots dot text source.</param>
        /// <returns></returns>
        public override RobotsDotText ParseRobotsDotTextSource(Uri baseUri, byte[] robotsDotTextSource)
        {
            RobotsDotText robotsDotText = new RobotsDotText();

            robotsDotText.DisallowedPaths = new List<string>();

            if (robotsDotTextSource != null)
            {
                using (StreamReader streamReader = new StreamReader(new MemoryStream(robotsDotTextSource)))
                {
                    string currentUserAgent = string.Empty;
                    bool addToDisallowedPaths = false;

                    while (!streamReader.EndOfStream)
                    {
                        string originalLine = streamReader.ReadLine();
                        string lineForSyntaxEvaluation = originalLine.ToLowerInvariant().Trim();

                        if (lineForSyntaxEvaluation.StartsWith("#") || string.IsNullOrEmpty(lineForSyntaxEvaluation))
                        {
                            continue;
                        }

                        if (lineForSyntaxEvaluation.StartsWith("crawl-delay:"))
                        {
                            if (currentUserAgent.Replace("user-agent:", string.Empty).Trim() == "*" || currentUserAgent.Contains(ApplicationSettings.UserAgent.ToLowerInvariant()))
                            {
                                lineForSyntaxEvaluation = lineForSyntaxEvaluation.Replace("crawl-delay:", string.Empty);

                                int crawlDelay;

                                if (int.TryParse(lineForSyntaxEvaluation, out crawlDelay))
                                {
                                    robotsDotText.CrawlDelay = crawlDelay;
                                }
                            }

                            continue;
                        }

                        if (lineForSyntaxEvaluation.StartsWith("user-agent:"))
                        {
                            if (lineForSyntaxEvaluation.Replace("user-agent:", string.Empty).Trim() == "*" || lineForSyntaxEvaluation.Contains(ApplicationSettings.UserAgent.ToLowerInvariant()))
                            {
                                currentUserAgent = lineForSyntaxEvaluation.Replace("user-agent:", string.Empty).Trim();

                                addToDisallowedPaths = true;
                            }
                            else
                            {
                                currentUserAgent = string.Empty;

                                addToDisallowedPaths = false;
                            }

                            continue;
                        }

                        if (addToDisallowedPaths)
                        {
                            if (lineForSyntaxEvaluation.StartsWith("disallow:"))
                            {
                                lineForSyntaxEvaluation = Regex.Replace(originalLine, "disallow:", string.Empty, RegexOptions.IgnoreCase).Trim();

                                if (!string.IsNullOrEmpty(lineForSyntaxEvaluation))
                                {
                                    Uri uri;
                                    if (Uri.TryCreate(baseUri, lineForSyntaxEvaluation, out uri))
                                    {
                                        if (!robotsDotText.DisallowedPaths.Contains(uri.AbsoluteUri))
                                        {
                                            robotsDotText.DisallowedPaths.Add(uri.AbsoluteUri);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return robotsDotText;
        }
    }
}