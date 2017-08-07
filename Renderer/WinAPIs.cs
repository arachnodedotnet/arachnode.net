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
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Arachnode.Renderer.Value;
using mshtml;

#endregion

namespace Arachnode.Renderer
{
    /// <summary>
    /// Windows API constants and functions
    /// </summary>
    public sealed class WinApis
    {
        public const int GWL_WNDPROC = -4;
        public const uint KEYEVENTF_EXTENDEDKEY = 0x01;
        public const uint KEYEVENTF_KEYUP = 0x02;
        public const uint MAX_PATH = 512;
        public const uint SHDVID_SSLSTATUS = 33;
        private const uint SPI_SETBEEP = 0x0002;
        private const uint SPIF_SENDWININICHANGE = 0x0002;
        private const uint SPIF_UPDATEINIFILE = 0x0001;
        public const uint STGM_READ = 0x00000000;
        public const uint URL_MK_LEGACY = 0;
        public const uint URL_MK_NO_CANONICALIZE = 2;
        public const uint URL_MK_UNIFORM = 1;

        public const short
            //defined inWTypes.h
            // 0 == FALSE, -1 == TRUE
            //typedef short VARIANT_BOOL;
            VAR_FALSE = 0;

        public const short
            //defined inWTypes.h
            // 0 == FALSE, -1 == TRUE
            //typedef short VARIANT_BOOL;
            VAR_TRUE = -1;

        #region Methods - GetWindowName - GetWindowClass

        public static int HiWord(int number)
        {
            if ((number & 0x80000000) == 0x80000000)
            {
                return (number >> 16);
            }
            else
            {
                return (number >> 16) & 0xffff;
            }
        }

        public static int LoWord(int number)
        {
            return number & 0xffff;
        }

        public static int GET_X_LPARAM(int lp)
        {
            return ((short) (lp & 0xffff));
        }

        public static int GET_Y_LPARAM(int lp)
        {
            if ((lp & 0x80000000) == 0x80000000)
            {
                return ((short) (lp >> 16));
            }
            else
            {
                return ((short) (lp >> 16) & 0xffff);
            }
        }

        public static int MakeLong(int LoWord, int HiWord)
        {
            return (HiWord << 16) | (LoWord & 0xffff);
        }

        public static IntPtr MakeLParam(int LoWord, int HiWord)
        {
            return (IntPtr) ((HiWord << 16) | (LoWord & 0xffff));
        }

        public static string GetWindowName(IntPtr Hwnd)
        {
            if (Hwnd == IntPtr.Zero)
            {
                return string.Empty;
            }
            // This function gets the name of a window from its handle
            StringBuilder Title = new StringBuilder((int) MAX_PATH);
            GetWindowText(Hwnd, Title, (int) MAX_PATH);

            return Title.ToString().Trim();
        }

        public static string GetWindowClass(IntPtr Hwnd)
        {
            if (Hwnd == IntPtr.Zero)
            {
                return string.Empty;
            }
            // This function gets the name of a window class from a window handle
            StringBuilder Title = new StringBuilder((int) MAX_PATH);
            RealGetWindowClass(Hwnd, Title, (int) MAX_PATH);

            return Title.ToString().Trim();
        }

        //public static FILETIME DateTimeToFiletime(DateTime time)
        //{
        //    FILETIME ft;
        //    long hFT1 = time.ToFileTimeUtc();
        //    ft.dwLowDateTime = (uint)(hFT1 & 0xFFFFFFFF);
        //    ft.dwHighDateTime = (uint)(hFT1 >> 32);
        //    return ft;
        //}

        //public static DateTime FiletimeToDateTime(FILETIME fileTime)
        //{
        //    if ((fileTime.dwHighDateTime == Int32.MaxValue) ||
        //        (fileTime.dwHighDateTime == 0 && fileTime.dwLowDateTime == 0))
        //    {
        //        // Not going to fit in the DateTime.  In the WinInet APIs, this is
        //        // what happens when there is no FILETIME attached to the cache entry.
        //        // We're going to use DateTime.MinValue as a marker for this case.
        //        return DateTime.MaxValue;
        //    }
        //    //long hFT2 = (((long)fileTime.dwHighDateTime) << 32) + fileTime.dwLowDateTime;
        //    //return DateTime.FromFileTimeUtc(hFT2);

        //    SYSTEMTIME syst = new SYSTEMTIME();
        //    SYSTEMTIME systLocal = new SYSTEMTIME();
        //    if (0 == FileTimeToSystemTime(ref fileTime, ref syst))
        //    {
        //        throw new ApplicationException("Error calling FileTimeToSystemTime: " + Marshal.GetLastWin32Error().ToString());
        //    }
        //    if (0 == SystemTimeToTzSpecificLocalTime(IntPtr.Zero, ref syst, out systLocal))
        //    {
        //        throw new ApplicationException("Error calling SystemTimeToTzSpecificLocalTime: " + Marshal.GetLastWin32Error().ToString());
        //    }
        //    return new DateTime(systLocal.Year, systLocal.Month, systLocal.Day, systLocal.Hour, systLocal.Minute, systLocal.Second);
        //}

        //public static string ToStringFromFileTime(FILETIME ft)
        //{
        //    DateTime dt = FiletimeToDateTime(ft);
        //    if (dt == DateTime.MinValue)
        //    {
        //        return string.Empty;
        //    }

        //    return dt.ToString();
        //}

        /// <summary>
        /// UrlCache functionality is taken from:
        /// Scott McMaster (smcmaste@hotmail.com)
        /// CodeProject article
        /// 
        /// There were some issues with preparing URLs
        /// for RegExp to work properly. This is
        /// demonstrated in AllForms.SetupCookieCachePattern method
        /// 
        /// urlPattern:
        /// . Dump the entire contents of the cache.
        /// Cookie: Lists all cookies on the system.
        /// Visited: Lists all of the history items.
        /// Cookie:.*\.example\.com Lists cookies from the example.com domain.
        /// http://www.example.com/example.html$: Lists the specific named file if present
        /// \.example\.com: Lists any and all entries from *.example.com.
        /// \.example\.com.*\.gif$: Lists the .gif files from *.example.com.
        /// \.js$: Lists the .js files in the cache.
        /// </summary>
        /// <param name="urlPattern"></param>
        /// <returns></returns>
        //public static ArrayList FindUrlCacheEntries(string urlPattern)
        //{
        //    ArrayList results = new ArrayList();
        //    IntPtr buffer = IntPtr.Zero;
        //    UInt32 structSize;
        //    //This call will fail but returns the size required in structSize
        //    //to allocate necessary buffer
        //    IntPtr hEnum = FindFirstUrlCacheEntry(null, buffer, out structSize);
        //    try
        //    {
        //        if (hEnum == IntPtr.Zero)
        //        {
        //            int lastError = Marshal.GetLastWin32Error();
        //            if (lastError == Hresults.ERROR_INSUFFICIENT_BUFFER)
        //            {
        //                //Allocate buffer
        //                buffer = Marshal.AllocHGlobal((int)structSize);
        //                //Call again, this time it should succeed
        //                hEnum = FindFirstUrlCacheEntry(urlPattern, buffer, out structSize);
        //            }
        //            else if (lastError == Hresults.ERROR_NO_MORE_ITEMS)
        //            {
        //                return results;
        //            }
        //        }
        //        INTERNET_CACHE_ENTRY_INFO result = (INTERNET_CACHE_ENTRY_INFO)Marshal.PtrToStructure(buffer, typeof(INTERNET_CACHE_ENTRY_INFO));
        //        try
        //        {
        //            if (Regex.IsMatch(result.lpszSourceUrlName, urlPattern, RegexOptions.IgnoreCase))
        //            {
        //                results.Add(result);
        //            }
        //        }
        //        catch (ArgumentException ae)
        //        {
        //            throw new ApplicationException("Invalid regular expression, details=" + ae.Message);
        //        }
        //        if (buffer != IntPtr.Zero)
        //        {
        //            try { Marshal.FreeHGlobal(buffer); }
        //            catch { }
        //            buffer = IntPtr.Zero;
        //            structSize = 0;
        //        }
        //        //Loop through all entries, attempt to find matches
        //        while (true)
        //        {
        //            long nextResult = FindNextUrlCacheEntry(hEnum, buffer, out structSize);
        //            if (nextResult != 1) //TRUE
        //            {
        //                int lastError = Marshal.GetLastWin32Error();
        //                if (lastError == Hresults.ERROR_INSUFFICIENT_BUFFER)
        //                {
        //                    buffer = Marshal.AllocHGlobal((int)structSize);
        //                    nextResult = FindNextUrlCacheEntry(hEnum, buffer, out structSize);
        //                }
        //                else if (lastError == Hresults.ERROR_NO_MORE_ITEMS)
        //                {
        //                    break;
        //                }
        //            }
        //            result = (INTERNET_CACHE_ENTRY_INFO)Marshal.PtrToStructure(buffer, typeof(INTERNET_CACHE_ENTRY_INFO));
        //            if (Regex.IsMatch(result.lpszSourceUrlName, urlPattern, RegexOptions.IgnoreCase))
        //            {
        //                results.Add(result);
        //            }
        //            if (buffer != IntPtr.Zero)
        //            {
        //                try { Marshal.FreeHGlobal(buffer); }
        //                catch { }
        //                buffer = IntPtr.Zero;
        //                structSize = 0;
        //            }
        //        }
        //    }
        //    finally
        //    {
        //        if (hEnum != IntPtr.Zero)
        //        {
        //            FindCloseUrlCache(hEnum);
        //        }
        //        if (buffer != IntPtr.Zero)
        //        {
        //            try { Marshal.FreeHGlobal(buffer); }
        //            catch { }
        //        }
        //    }
        //    return results;
        //}
        /// <summary>
        /// Attempts to delete a cookie or cache entry
        /// </summary>
        /// <param name="url">INTERNET_CACHE_ENTRY_INFO.lpszSourceUrlName</param>
        public static void DeleteFromUrlCache(string url)
        {
            long apiResult = DeleteUrlCacheEntry(url);
            if (apiResult != 0)
            {
                return;
            }

            int lastError = Marshal.GetLastWin32Error();
            if (lastError == Hresults.ERROR_ACCESS_DENIED)
            {
                throw new ApplicationException("Access denied: " + url);
            }
            else
            {
                throw new ApplicationException("Insufficient buffer: " + url);
            }
        }

        #endregion

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags,
                                              UIntPtr dwExtraInfo);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);

        [DllImport("user32.dll")]
        public static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int CallWindowProc(
            IntPtr lpPrevWndFunc, IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowLong(
            IntPtr hWnd, int nIndex, IntPtr newProc);

        [DllImport("ole32.dll", SetLastError = true,
            ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int RevokeObjectParam(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszKey);

        //[DllImport("urlmon.dll", SetLastError = true)]
        //public static extern int RegisterBindStatusCallback(
        //    [MarshalAs(UnmanagedType.Interface)] IBindCtx pBc,
        //    [MarshalAs(UnmanagedType.Interface)] DownloadManagerImpl.IBindStatusCallback pBSCb,
        //    [Out, MarshalAs(UnmanagedType.Interface)] out IBindStatusCallback ppBSCBPrev,
        //    [In, MarshalAs(UnmanagedType.U4)] UInt32 dwReserved); 

        //[DllImport("user32.dll", SetLastError = true)]
        //public static extern int GetClipboardFormatName(uint format, [Out] StringBuilder
        //   lpszFormatName, int cchMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        //MSDN
        //This function should no longer be used. Use the CoTaskMemFree and CoTaskMemAlloc functions in its place.
        //[DllImport("shell32.dll", SetLastError = true)]
        //public static extern int SHGetMalloc(out IMalloc ppMalloc);

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth,
                                                           int nHeight);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool DeleteObject(IntPtr hObject);

        //[DllImport("gdi32.dll", SetLastError = true)]
        //public static extern bool SetStretchBltMode(IntPtr hdc, StretchMode iStretchMode);

        //[DllImport("gdi32.dll", SetLastError = true)]
        //public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
        //    int nWidth, int nHeight,
        //    IntPtr hObjSource, int nXSrc, int nYSrc,
        //    TernaryRasterOperations dwRop);

        //[DllImport("gdi32.dll", SetLastError = true)]
        //public static extern bool StretchBlt(IntPtr hdcDest, int nXDest, int nYDest,
        //    int nWidthDest, int nHeightDest,
        //    IntPtr hdcSrc, int nXSrc, int nYSrc, int nWidthSrc, int nHeightSrc,
        //    TernaryRasterOperations dwRop);

        [DllImport("ole32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int CreateBindCtx(
            [MarshalAs(UnmanagedType.U4)] uint dwReserved,
            [Out, MarshalAs(UnmanagedType.Interface)] out IBindCtx ppbc);

        [DllImport("ole32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int CreateAsyncBindCtx(
            [MarshalAs(UnmanagedType.U4)] uint dwReserved,
            [MarshalAs(UnmanagedType.Interface)] IBindStatusCallback pbsc,
            [MarshalAs(UnmanagedType.Interface)] IEnumFORMATETC penumfmtetc,
            [Out, MarshalAs(UnmanagedType.Interface)] out IBindCtx ppbc);

        [DllImport("urlmon.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int CreateURLMoniker(
            [MarshalAs(UnmanagedType.Interface)] IMoniker pmkContext,
            [MarshalAs(UnmanagedType.LPWStr)] string szURL,
            [Out, MarshalAs(UnmanagedType.Interface)] out IMoniker ppmk);

        [DllImport("urlmon.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int CreateURLMonikerEx(
            [MarshalAs(UnmanagedType.Interface)] IMoniker pmkContext,
            [MarshalAs(UnmanagedType.LPWStr)] string szURL,
            [Out, MarshalAs(UnmanagedType.Interface)] out IMoniker ppmk,
            uint URL_MK_XXX); //Flags, one of 

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, uint Msg,
                                                IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, uint Msg,
                                                IntPtr wParam, ref StringBuilder lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern void SendMessage(HandleRef hWnd, uint msg,
                                              IntPtr wParam, ref tagRECT lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, uint msg,
                                                IntPtr wParam, ref tagPOINT lParam);

        [DllImport("ole32.dll", CharSet = CharSet.Auto)]
        public static extern int CreateStreamOnHGlobal(IntPtr hGlobal, bool fDeleteOnRelease,
                                                       [MarshalAs(UnmanagedType.Interface)] out IStream ppstm);

        [DllImport("user32.dll")]
        public static extern short GetKeyState(int nVirtKey);

        [DllImport("user32.dll")]
        public static extern bool CloseWindow(IntPtr hWnd);

        [DllImport("ole32.dll", ExactSpelling = true, PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public static extern object CoCreateInstance(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid rclsid,
            [MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter,
            CLSCTX dwClsContext,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid);

        //MessageBox(new IntPtr(0), "Text", "Caption", 0 );
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern uint MessageBox(
            IntPtr hWnd, String text, String caption, uint type);

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out tagRECT lpRect);

        [DllImport("user32.dll")]
        public static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, IntPtr windowTitle);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder title, int size);

        [DllImport("user32.dll")]
        public static extern uint RealGetWindowClass(IntPtr hWnd, StringBuilder pszType, uint cchType);

        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool CopyRect(
            [In, Out, MarshalAs(UnmanagedType.Struct)] ref tagRECT lprcDst,
            [In, MarshalAs(UnmanagedType.Struct)] ref tagRECT lprcSrc);

        //[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        //public static extern uint RegisterClipboardFormat(string lpszFormat);

        [DllImport("ole32.dll")]
        public static extern void ReleaseStgMedium(
            [In, MarshalAs(UnmanagedType.Struct)] ref STGMEDIUM pmedium);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern uint DragQueryFile(IntPtr hDrop, uint iFile,
                                                [Out] StringBuilder lpszFile, uint cch);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        public static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        public static extern UIntPtr GlobalSize(IntPtr hMem);

        //[DllImport("Kernel32.dll", SetLastError = true)]
        //public static extern long FileTimeToSystemTime(ref FILETIME FileTime,
        //    ref SYSTEMTIME SystemTime);

        //[DllImport("kernel32.dll", SetLastError = true)]
        //public static extern long SystemTimeToTzSpecificLocalTime(
        //    IntPtr lpTimeZoneInformation, ref SYSTEMTIME lpUniversalTime,
        //    out SYSTEMTIME lpLocalTime);

        [DllImport("wininet.dll", SetLastError = true)]
        public static extern long FindCloseUrlCache(IntPtr hEnumHandle);

        [DllImport("wininet.dll", SetLastError = true)]
        public static extern IntPtr FindFirstUrlCacheEntry(string lpszUrlSearchPattern, IntPtr lpFirstCacheEntryInfo, out UInt32 lpdwFirstCacheEntryInfoBufferSize);

        [DllImport("wininet.dll", SetLastError = true)]
        public static extern long FindNextUrlCacheEntry(IntPtr hEnumHandle, IntPtr lpNextCacheEntryInfo, out UInt32 lpdwNextCacheEntryInfoBufferSize);

        [DllImport("wininet.dll", SetLastError = true)]
        public static extern bool GetUrlCacheEntryInfo(string lpszUrlName, IntPtr lpCacheEntryInfo, out UInt32 lpdwCacheEntryInfoBufferSize);

        [DllImport("wininet.dll", SetLastError = true)]
        public static extern long DeleteUrlCacheEntry(string lpszUrlName);

        [DllImport("wininet.dll", SetLastError = true)]
        public static extern IntPtr RetrieveUrlCacheEntryStream(string lpszUrlName, IntPtr lpCacheEntryInfo, out UInt32 lpdwCacheEntryInfoBufferSize, long fRandomRead, UInt32 dwReserved);

        [DllImport("wininet.dll", SetLastError = true)]
        public static extern IntPtr ReadUrlCacheEntryStream(IntPtr hUrlCacheStream, UInt32 dwLocation, IntPtr lpBuffer, out UInt32 lpdwLen, UInt32 dwReserved);

        [DllImport("wininet.dll", SetLastError = true)]
        public static extern long UnlockUrlCacheEntryStream(IntPtr hUrlCacheStream, UInt32 dwReserved);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam,
                                                       IntPtr pvParam, uint fWinIni);

        //For older windows
        public static bool SetSystemBeep(bool bEnable)
        {
            if (bEnable)
            {
                return SystemParametersInfo(SPI_SETBEEP, 1, IntPtr.Zero, (SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE));
            }
            else
            {
                return SystemParametersInfo(SPI_SETBEEP, 0, IntPtr.Zero, (SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE));
            }
        }

        //Pass IntPtr.Zero for hInternet to indicate global
        //dwOption, one of INTERNET_OPTION_XXXX flags

        //To retrieve all cookies for a particular domain, call 
        //InternetGetCookie[Ex]. To delete them, call InternetSetCookie[Ex]: pass 
        //IntPtr.Zero for cookie data to delete a cookie.

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetSetOption(IntPtr hInternet,
                                                    int dwOption,
                                                    IntPtr lpBuffer,
                                                    //Len of lpBuffer in bytes
                                                    //If lpBuffer contains a string, the size is in TCHARs. If lpBuffer contains anything other than a string, the size is in bytes.
                                                    int lpdwBufferLength);

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetQueryOption(IntPtr hInternet,
                                                      int dwOption, IntPtr lpBuffer,
                                                      [In, Out] ref int lpdwBufferLength);


        //call DoOrganizeFavDlg( this.Handle.ToInt64(), null );
        [DllImport("shdocvw.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern long DoOrganizeFavDlg(long hWnd, string lpszRootFolder);

        [DllImport("shdocvw.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern long AddUrlToFavorites(long hwnd,
                                                    [MarshalAs(UnmanagedType.LPWStr)] string pszUrlW,
                                                    [MarshalAs(UnmanagedType.LPWStr)] string pszTitleW, //If null, url value is used
                                                    [MarshalAs(UnmanagedType.Bool)] bool fDisplayUI);

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetGetCookie(string lpszUrlName, string lpszCookieName,
                                                    [Out] string lpszCookieData, [MarshalAs(UnmanagedType.U4)] out int lpdwSize);

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetSetCookie(string lpszUrlName, string lpszCookieName,
                                                    IntPtr lpszCookieData);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo,
                                                 [In, Out] tagPOINT pt, int cPoints);
    }
}