#region License : arachnode.net

// Copyright (c) 2015 http://arachnode.net, arachnode.net, LLC
//  
// Permission is hereby granted, upon purchase, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, merge and modify copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following
// conditions:
// 
// LICENSE (ALL VERSIONS/EDITIONS): http://arachnode.net/r.ashx?3
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using System;

#endregion

namespace Arachnode.Renderer
{
    [Flags]
    public enum BrowserOptions : uint
    {
        /// <summary>
        /// No flags are set.
        /// </summary>
        None = 0,
        /// <summary>
        /// The browser will operate in offline mode. Equivalent to DLCTL_FORCEOFFLINE.
        /// </summary>
        AlwaysOffline = 0x10000000,
        /// <summary>
        /// The browser will play background sounds. Equivalent to DLCTL_BGSOUNDS.
        /// </summary>
        BackgroundSounds = 0x00000040,
        /// <summary>
        /// Specifies that the browser will not run Active-X controls. Use this setting
        /// to disable Flash movies. Equivalent to DLCTL_NO_RUNACTIVEXCTLS.
        /// </summary>
        DontRunActiveX = 0x00000200,
        DownloadOnly = 0x00000800,
        /// <summary>
        ///  Specifies that the browser should fetch the content from the server. If the server's
        ///  content is the same as the cache, the cache is used.Equivalent to DLCTL_RESYNCHRONIZE.
        /// </summary>
        IgnoreCache = 0x00002000,
        /// <summary>
        ///  The browser will force the request from the server, and ignore the proxy, even if the
        ///  proxy indicates the content is up to date.Equivalent to DLCTL_PRAGMA_NO_CACHE.
        /// </summary>
        IgnoreProxy = 0x00004000,
        /// <summary>
        ///  Specifies that the browser should download and display images. This is set by default.
        ///  Equivalent to DLCTL_DLIMAGES.
        /// </summary>
        Images = 0x00000010,
        /// <summary>
        ///  Disables downloading and installing of Active-X controls.Equivalent to DLCTL_NO_DLACTIVEXCTLS.
        /// </summary>
        NoActiveXDownload = 0x00000400,
        /// <summary>
        ///  Disables web behaviours.Equivalent to DLCTL_NO_BEHAVIORS.
        /// </summary>
        NoBehaviours = 0x00008000,
        /// <summary>
        ///  The browser suppresses any HTML charset specified.Equivalent to DLCTL_NO_METACHARSET.
        /// </summary>
        NoCharSets = 0x00010000,
        /// <summary>
        ///  Indicates the browser will ignore client pulls.Equivalent to DLCTL_NO_CLIENTPULL.
        /// </summary>
        NoClientPull = 0x20000000,
        /// <summary>
        ///  The browser will not download or display Java applets.Equivalent to DLCTL_NO_JAVA.
        /// </summary>
        NoJava = 0x00000100,
        /// <summary>
        ///  The browser will download framesets and parse them, but will not download the frames
        ///  contained inside those framesets.Equivalent to DLCTL_NO_FRAMEDOWNLOAD.
        /// </summary>
        NoFrameDownload = 0x00080000,
        NoMetaCharset = unchecked(0x00010000),
        /// <summary>
        ///  The browser will not execute any scripts.Equivalent to DLCTL_NO_SCRIPTS.
        /// </summary>
        NoScripts = 0x00000080,
        /// <summary>
        ///  If the browser cannot detect any internet connection, this causes it to default to
        ///  offline mode.Equivalent to DLCTL_OFFLINEIFNOTCONNECTED.
        /// </summary>
        OfflineIfNotConnected = 0x80000000,

        PRAGMA_NO_CACHE = unchecked(0x00004000),

        ReSynchronize = unchecked(0x00002000),

        UrlEncodingDisableUTF8 = unchecked(0x00020000),
        UrlEncodingEnableUTF8 = unchecked(0x00040000),
        /// <summary>
        ///  Specifies that UTF8 should be used.Equivalent to DLCTL_URL_ENCODING_ENABLE_UTF8.
        /// </summary>
        UTF8 = 0x00040000,
        /// <summary>
        ///  The browser will download and display video media.Equivalent to DLCTL_VIDEOS.
        /// </summary>
        Videos = 0x00000020,

        Silent = unchecked(0x40000000)
    }
}