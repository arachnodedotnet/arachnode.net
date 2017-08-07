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
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using FILETIME=System.Runtime.InteropServices.ComTypes.FILETIME;

#endregion

namespace Arachnode.Utilities
{
    public static class Internet
    {
        [DllImport(@"wininet",
            SetLastError = true,
            CharSet = CharSet.Auto,
            EntryPoint = "FindFirstUrlCacheGroup",
            CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr FindFirstUrlCacheGroup(
            int dwFlags,
            int dwFilter,
            IntPtr lpSearchCondition,
            int dwSearchCondition,
            ref long lpGroupId,
            IntPtr lpReserved);

        // For PInvoke Retrieves the next cache group in a cache group enumeration
        [DllImport(@"wininet",
            SetLastError = true,
            CharSet = CharSet.Auto,
            EntryPoint = "FindNextUrlCacheGroup",
            CallingConvention = CallingConvention.StdCall)]
        public static extern bool FindNextUrlCacheGroup(
            IntPtr hFind,
            ref long lpGroupId,
            IntPtr lpReserved);

        // For PInvoke Releases the specified GROUPID and any associated state in the cache index file
        [DllImport(@"wininet",
            SetLastError = true,
            CharSet = CharSet.Auto,
            EntryPoint = "DeleteUrlCacheGroup",
            CallingConvention = CallingConvention.StdCall)]
        public static extern bool DeleteUrlCacheGroup(
            long GroupId,
            int dwFlags,
            IntPtr lpReserved);

        // For PInvoke Begins the enumeration of the Internet cache
        [DllImport(@"wininet",
            SetLastError = true,
            CharSet = CharSet.Auto,
            EntryPoint = "FindFirstUrlCacheEntryA",
            CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr FindFirstUrlCacheEntry(
            [MarshalAs(UnmanagedType.LPTStr)] string lpszUrlSearchPattern,
            IntPtr lpFirstCacheEntryInfo,
            ref int lpdwFirstCacheEntryInfoBufferSize);

        // For PInvoke Retrieves the next entry in the Internet cache
        [DllImport(@"wininet",
            SetLastError = true,
            CharSet = CharSet.Auto,
            EntryPoint = "FindNextUrlCacheEntryA",
            CallingConvention = CallingConvention.StdCall)]
        public static extern bool FindNextUrlCacheEntry(
            IntPtr hFind,
            IntPtr lpNextCacheEntryInfo,
            ref int lpdwNextCacheEntryInfoBufferSize);

        // For PInvoke Removes the file that is associated with the source name from the cache, if the file exists
        [DllImport(@"wininet",
            SetLastError = true,
            CharSet = CharSet.Auto,
            EntryPoint = "DeleteUrlCacheEntryA",
            CallingConvention = CallingConvention.StdCall)]
        public static extern bool DeleteUrlCacheEntry(
            IntPtr lpszUrlName);

        public static void ResetASPDOTNETWebServers()
        {
            foreach (Process process in Process.GetProcessesByName("WebDev.WebServer"))
            {
                process.Kill();
            }
        }

        public static void ResetIIS()
        {
            Process.Start("iisreset");
        }

        public static string GetReverseDNS(string ip, int timeout)
        {
            try
            {
                GetHostEntryHandler callback = Dns.GetHostEntry;

                IAsyncResult result = callback.BeginInvoke(ip, null, null);

                if (result.AsyncWaitHandle.WaitOne(timeout, false))
                {
                    return callback.EndInvoke(result).HostName;
                }

                return ip;
            }
            catch (Exception)
            {
                return ip;
            }
        }

        /// <summary>
        /// Clears all browinsg data and files.  If this method appears to run indefinitely ensure that no browser are open/streaming data...
        /// </summary>
        /// <returns></returns>
        public static bool ClearIECache()
        {
            try
            {
                // Indicates that all of the cache groups in the user's system should be enumerated
                const int CACHEGROUP_SEARCH_ALL = 0x0;
                // Indicates that all the cache entries that are associated with the cache group
                // should be deleted, unless the entry belongs to another cache group.
                const int CACHEGROUP_FLAG_FLUSHURL_ONDELETE = 0x2;
                // File not found.
                const int ERROR_FILE_NOT_FOUND = 0x2;
                // No more items have been found.
                const int ERROR_NO_MORE_ITEMS = 259;
                // Pointer to a GROUPID variable
                long groupId = 0;

                // Local variables
                int cacheEntryInfoBufferSizeInitial = 0;
                int cacheEntryInfoBufferSize = 0;
                IntPtr cacheEntryInfoBuffer = IntPtr.Zero;
                INTERNET_CACHE_ENTRY_INFOA internetCacheEntry;
                IntPtr enumHandle = IntPtr.Zero;
                bool returnValue = false;

                // Delete the groups first.
                // Groups may not always exist on the system.
                // For more information, visit the following Microsoft Web site
                // http//msdn.microsoft.com/library/?url=/workshop/networking/wininet/overview/cache.asp      
                // By default, a URL does not belong to any group. Therefore, that cache may become
                // empty even when the CacheGroup APIs are not used because the existing URL does not belong to any group.      
                enumHandle = FindFirstUrlCacheGroup(0, CACHEGROUP_SEARCH_ALL, IntPtr.Zero, 0, ref groupId, IntPtr.Zero);
                // If there are no items in the Cache, you are finished.
                if (enumHandle != IntPtr.Zero && ERROR_NO_MORE_ITEMS == Marshal.GetLastWin32Error())
                {
                    return true;
                }

                // Loop through Cache Group, and then delete entries.
                while (true)
                {
                    if (ERROR_NO_MORE_ITEMS == Marshal.GetLastWin32Error() || ERROR_FILE_NOT_FOUND == Marshal.GetLastWin32Error())
                    {
                        break;
                    }
                    // Delete a particular Cache Group.
                    returnValue = DeleteUrlCacheGroup(groupId, CACHEGROUP_FLAG_FLUSHURL_ONDELETE, IntPtr.Zero);
                    if (!returnValue && ERROR_FILE_NOT_FOUND == Marshal.GetLastWin32Error())
                    {
                        returnValue = FindNextUrlCacheGroup(enumHandle, ref groupId, IntPtr.Zero);
                    }

                    if (!returnValue && (ERROR_NO_MORE_ITEMS == Marshal.GetLastWin32Error() || ERROR_FILE_NOT_FOUND == Marshal.GetLastWin32Error()))
                    {
                        break;
                    }
                }

                // Start to delete URLs that do not belong to any group.
                enumHandle = FindFirstUrlCacheEntry(null, IntPtr.Zero, ref cacheEntryInfoBufferSizeInitial);
                if (enumHandle != IntPtr.Zero && ERROR_NO_MORE_ITEMS == Marshal.GetLastWin32Error())
                {
                    return true;
                }

                cacheEntryInfoBufferSize = cacheEntryInfoBufferSizeInitial;
                cacheEntryInfoBuffer = Marshal.AllocHGlobal(cacheEntryInfoBufferSize);
                enumHandle = FindFirstUrlCacheEntry(null, cacheEntryInfoBuffer, ref cacheEntryInfoBufferSizeInitial);

                int numberOfItems = 0;

                while (true)
                {
                    internetCacheEntry = (INTERNET_CACHE_ENTRY_INFOA) Marshal.PtrToStructure(cacheEntryInfoBuffer, typeof (INTERNET_CACHE_ENTRY_INFOA));
                    if (ERROR_NO_MORE_ITEMS == Marshal.GetLastWin32Error() || 87 == Marshal.GetLastWin32Error())
                    {
                        break;
                    }

                    numberOfItems++;

                    cacheEntryInfoBufferSizeInitial = cacheEntryInfoBufferSize;
                    returnValue = DeleteUrlCacheEntry(internetCacheEntry.lpszSourceUrlName);
                    if (!returnValue)
                    {
                        returnValue = FindNextUrlCacheEntry(enumHandle, cacheEntryInfoBuffer, ref cacheEntryInfoBufferSizeInitial);
                    }
                    if (!returnValue && ERROR_NO_MORE_ITEMS == Marshal.GetLastWin32Error())
                    {
                        break;
                    }
                    if (!returnValue && cacheEntryInfoBufferSizeInitial > cacheEntryInfoBufferSize)
                    {
                        cacheEntryInfoBufferSize = cacheEntryInfoBufferSizeInitial;
                        cacheEntryInfoBuffer = Marshal.ReAllocHGlobal(cacheEntryInfoBuffer, (IntPtr) cacheEntryInfoBufferSize);
                        returnValue = FindNextUrlCacheEntry(enumHandle, cacheEntryInfoBuffer, ref cacheEntryInfoBufferSizeInitial);
                    }
                }
                Marshal.FreeHGlobal(cacheEntryInfoBuffer);

                Directories.DeleteDirectory(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache));

                return true;
            }
            catch
            {
            }

            return false;
        }

        #region Nested type: GetHostEntryHandler

        private delegate IPHostEntry GetHostEntryHandler(string ip);

        #endregion

        #region Nested type: INTERNET_CACHE_ENTRY_INFOA

        [StructLayout(LayoutKind.Explicit, Size = 80)]
        public struct INTERNET_CACHE_ENTRY_INFOA
        {
            [FieldOffset(0)] public uint dwStructSize;
            [FieldOffset(4)] public IntPtr lpszSourceUrlName;
            [FieldOffset(8)] public IntPtr lpszLocalFileName;
            [FieldOffset(12)] public uint CacheEntryType;
            [FieldOffset(16)] public uint dwUseCount;
            [FieldOffset(20)] public uint dwHitRate;
            [FieldOffset(24)] public uint dwSizeLow;
            [FieldOffset(28)] public uint dwSizeHigh;
            [FieldOffset(32)] public FILETIME LastModifiedTime;
            [FieldOffset(40)] public FILETIME ExpireTime;
            [FieldOffset(48)] public FILETIME LastAccessTime;
            [FieldOffset(56)] public FILETIME LastSyncTime;
            [FieldOffset(64)] public IntPtr lpHeaderInfo;
            [FieldOffset(68)] public uint dwHeaderInfoSize;
            [FieldOffset(72)] public IntPtr lpszFileExtension;
            [FieldOffset(76)] public uint dwReserved;
            [FieldOffset(76)] public uint dwExemptDelta;
        }

        #endregion
    }
}