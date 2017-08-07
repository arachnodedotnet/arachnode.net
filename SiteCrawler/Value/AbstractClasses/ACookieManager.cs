using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using Arachnode.Configuration;
using Arachnode.DataAccess.Value.Interfaces;

namespace Arachnode.SiteCrawler.Value.AbstractClasses
{
    public abstract class ACookieManager
    {
        protected readonly object _cookieContainerLock = new object();

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetGetCookie(string url, string name, StringBuilder data, ref int dataSize);

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetSetCookie(string url, string cookieName, string cookieData);

        public abstract CookieCollection BuildCookieCollection(string cookieHeaders);

        /// <summary>
        /// 	Creates cookie entries for http://domain.com AND http://www.domain.com.  Necessary for the cookie process in WebClient.cs to properly process cookies.
        /// </summary>
        /// <param name = "absoluteUri"></param>
        /// <param name = "cookieContainer"></param>
        /// <param name = "cookieCollection"></param>
        public abstract void AddCookieCollectionToCookieContainer(string absoluteUri, CookieContainer cookieContainer, CookieCollection cookieCollection);

        public abstract void UpdateCookies(string absoluteUri, CookieContainer cookieContainer, CookieCollection cookieCollection);
        public abstract void GetCookies(string absoluteUri, CookieContainer cookieContainer);
        public abstract string InternetGetCookieEx(string absoluteUri);
    }
}
