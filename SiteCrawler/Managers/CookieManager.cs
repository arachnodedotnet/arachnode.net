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
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using Arachnode.Configuration;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Value.AbstractClasses;

#endregion

namespace Arachnode.SiteCrawler.Managers
{
    public class CookieManager : ACookieManager
    {
        public override CookieCollection BuildCookieCollection(string cookieHeaders)
        {
            CookieCollection cookieCollection = new CookieCollection();

            if (!string.IsNullOrEmpty(cookieHeaders))
            {
                foreach (string cookieValue in cookieHeaders.Split(";,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] cookieValueSplit = cookieValue.Split("=".ToCharArray());

                    try
                    {
                        Cookie cookie = new Cookie(cookieValueSplit[0].Trim(), cookieValue.Replace(cookieValueSplit[0] + "=", string.Empty).Trim());

                        cookieCollection.Add(cookie);
                    }
                    catch
                    {
                        //TODO: Make this log where it came from...
                    }
                }
            }

            return cookieCollection;
        }

        /// <summary>
        /// 	Creates cookie entries for http://domain.com AND http://www.domain.com.  Necessary for the cookie process in WebClient.cs to properly process cookies.
        /// </summary>
        /// <param name = "absoluteUri"></param>
        /// <param name = "cookieContainer"></param>
        /// <param name = "cookieCollection"></param>
        public override void AddCookieCollectionToCookieContainer(string absoluteUri, CookieContainer cookieContainer, CookieCollection cookieCollection)
        {
            if (!string.IsNullOrEmpty(absoluteUri) && cookieContainer != null && cookieCollection != null)
            {
                UpdateCookies(absoluteUri, cookieContainer, cookieCollection);
            }
        }

        public override void UpdateCookies(string absoluteUri, CookieContainer cookieContainer, CookieCollection cookieCollection)
        {
            try
            {
                lock (_cookieContainerLock)
                {
                    if (cookieContainer != null && cookieCollection != null)
                    {
                        Uri uri = new Uri(absoluteUri);

                        uri = new Uri(uri.Scheme + Uri.SchemeDelimiter + UserDefinedFunctions.ExtractDomain(absoluteUri).Value);

                        cookieContainer.Add(uri, cookieCollection);

                        uri = new Uri(uri.Scheme + Uri.SchemeDelimiter + "www." + uri.Host);

                        cookieContainer.Add(uri, cookieCollection);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public override void GetCookies(string absoluteUri, CookieContainer cookieContainer)
        {
            try
            {
                lock (_cookieContainerLock)
                {
                    if (cookieContainer == null)
                    {
                        cookieContainer = new CookieContainer();
                    }

                    Uri uri = new Uri(absoluteUri);

                    uri = new Uri(uri.Scheme + Uri.SchemeDelimiter + UserDefinedFunctions.ExtractDomain(absoluteUri).Value);

                    if (cookieContainer.GetCookies(uri).Count == 0)
                    {
                        string cookieHeaders = InternetGetCookieEx(absoluteUri);

                        if (cookieHeaders != null)
                        {
                            CookieCollection cookieCollection = BuildCookieCollection(cookieHeaders);

                            cookieContainer.Add(uri, cookieCollection);
                        }
                    }

                    uri = new Uri(uri.Scheme + Uri.SchemeDelimiter + "www." + uri.Host);

                    if (cookieContainer.GetCookies(uri).Count == 0)
                    {
                        string cookieHeaders = InternetGetCookieEx(absoluteUri);

                        if (cookieHeaders != null)
                        {
                            CookieCollection cookieCollection = BuildCookieCollection(cookieHeaders);

                            cookieContainer.Add(uri, cookieCollection);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public override string InternetGetCookieEx(string absoluteUri)
        {
            StringBuilder cookieHeader = new StringBuilder(new String(' ', 256), 256);

            int length = cookieHeader.Length;

            if (!InternetGetCookie(absoluteUri, null, cookieHeader, ref length))
            {
                if (length < 0)
                {
                    return String.Empty;
                }

                cookieHeader = new StringBuilder(length);  //resize with the new length...

                InternetGetCookie(absoluteUri, null, cookieHeader, ref length);
            }

            return cookieHeader.ToString();
        }
    }
}