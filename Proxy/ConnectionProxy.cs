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

#endregion

namespace Arachnode.Proxy
{
    public static class ConnectionProxy
    {
        private const int INTERNET_OPEN_TYPE_DIRECT = 1; // direct to net
        private const int INTERNET_OPEN_TYPE_PRECONFIG = 0; // read registry
        public static string applicationName;

        [DllImport("wininet.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr InternetOpen(string lpszAgent, int dwAccessType, string lpszProxyName, string lpszProxyBypass, int dwFlags);

        [DllImport("wininet.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool InternetCloseHandle(IntPtr hInternet);

        /// <summary>
        /// Sets an Internet option.
        /// </summary>
        [DllImport("wininet.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, INTERNET_OPTION dwOption, IntPtr lpBuffer, int lpdwBufferLength);

        /// <summary>
        /// Queries an Internet option on the specified handle. The Handle will be always 0.
        /// </summary>
        [DllImport("wininet.dll", CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "InternetQueryOption")]
        private static extern bool InternetQueryOptionList(IntPtr Handle, INTERNET_OPTION OptionFlag, ref INTERNET_PER_CONN_OPTION_LIST OptionList, ref int size);

        /// <summary>
        /// Set the proxy server for LAN connection.
        /// </summary>
        public static bool SetConnectionProxy(string proxyServer)
        {
            IntPtr hInternet = InternetOpen(applicationName, INTERNET_OPEN_TYPE_DIRECT, null, null, 0);

            //// Create 3 options.
            //INTERNET_PER_CONN_OPTION[] Options = new INTERNET_PER_CONN_OPTION[3];

            // Create 2 options.
            INTERNET_PER_CONN_OPTION[] Options = new INTERNET_PER_CONN_OPTION[2];

            // Set PROXY flags.
            Options[0] = new INTERNET_PER_CONN_OPTION();
            Options[0].dwOption = (int) INTERNET_PER_CONN_OptionEnum.INTERNET_PER_CONN_FLAGS;
            Options[0].Value.dwValue = (int) INTERNET_OPTION_PER_CONN_FLAGS.PROXY_TYPE_PROXY;

            // Set proxy name.
            Options[1] = new INTERNET_PER_CONN_OPTION();
            Options[1].dwOption =
                (int) INTERNET_PER_CONN_OptionEnum.INTERNET_PER_CONN_PROXY_SERVER;
            Options[1].Value.pszValue = Marshal.StringToHGlobalAnsi(proxyServer);

            //// Set proxy bypass.
            //Options[2] = new INTERNET_PER_CONN_OPTION();
            //Options[2].dwOption =
            //    (int)INTERNET_PER_CONN_OptionEnum.INTERNET_PER_CONN_PROXY_BYPASS;
            //Options[2].Value.pszValue = Marshal.StringToHGlobalAnsi("local");

            //// Allocate a block of memory of the options.
            //System.IntPtr buffer = Marshal.AllocCoTaskMem(Marshal.SizeOf(Options[0])
            //    + Marshal.SizeOf(Options[1]) + Marshal.SizeOf(Options[2]));

            // Allocate a block of memory of the options.
            System.IntPtr buffer = Marshal.AllocCoTaskMem(Marshal.SizeOf(Options[0]) + Marshal.SizeOf(Options[1]));

            System.IntPtr current = buffer;

            // Marshal data from a managed object to an unmanaged block of memory.
            for (int i = 0; i < Options.Length; i++)
            {
                Marshal.StructureToPtr(Options[i], current, false);
                current = (System.IntPtr) ((int) current + Marshal.SizeOf(Options[i]));
            }

            // Initialize a INTERNET_PER_CONN_OPTION_LIST instance.
            INTERNET_PER_CONN_OPTION_LIST option_list = new INTERNET_PER_CONN_OPTION_LIST();

            // Point to the allocated memory.
            option_list.pOptions = buffer;

            // Return the unmanaged size of an object in bytes.
            option_list.Size = Marshal.SizeOf(option_list);

            // IntPtr.Zero means LAN connection.
            option_list.Connection = IntPtr.Zero;

            option_list.OptionCount = Options.Length;
            option_list.OptionError = 0;
            int size = Marshal.SizeOf(option_list);

            // Allocate memory for the INTERNET_PER_CONN_OPTION_LIST instance.
            IntPtr intptrStruct = Marshal.AllocCoTaskMem(size);

            // Marshal data from a managed object to an unmanaged block of memory.
            Marshal.StructureToPtr(option_list, intptrStruct, true);

            // Set internet settings.
            bool bReturn = InternetSetOption(hInternet, INTERNET_OPTION.INTERNET_OPTION_PER_CONNECTION_OPTION, intptrStruct, size);

            // Free the allocated memory.
            Marshal.FreeCoTaskMem(buffer);
            Marshal.FreeCoTaskMem(intptrStruct);
            InternetCloseHandle(hInternet);

            // Throw an exception if this operation failed.
            if (!bReturn)
            {
                throw new ApplicationException(" Set Internet Option Failed!");
            }

            return bReturn;
        }


        /// <summary>
        /// Backup the current options for LAN connection.
        /// Make sure free the memory after restoration. 
        /// </summary>
        private static INTERNET_PER_CONN_OPTION_LIST GetSystemProxy()
        {
            // Query following options. 
            INTERNET_PER_CONN_OPTION[] Options = new INTERNET_PER_CONN_OPTION[3];

            Options[0] = new INTERNET_PER_CONN_OPTION();
            Options[0].dwOption = (int) INTERNET_PER_CONN_OptionEnum.INTERNET_PER_CONN_FLAGS;
            Options[1] = new INTERNET_PER_CONN_OPTION();
            Options[1].dwOption = (int) INTERNET_PER_CONN_OptionEnum.INTERNET_PER_CONN_PROXY_SERVER;
            Options[2] = new INTERNET_PER_CONN_OPTION();
            Options[2].dwOption = (int) INTERNET_PER_CONN_OptionEnum.INTERNET_PER_CONN_PROXY_BYPASS;

            // Allocate a block of memory of the options.
            System.IntPtr buffer = Marshal.AllocCoTaskMem(Marshal.SizeOf(Options[0]) + Marshal.SizeOf(Options[1]) + Marshal.SizeOf(Options[2]));

            System.IntPtr current = (System.IntPtr) buffer;

            // Marshal data from a managed object to an unmanaged block of memory.
            for (int i = 0; i < Options.Length; i++)
            {
                Marshal.StructureToPtr(Options[i], current, false);
                current = (System.IntPtr) ((int) current + Marshal.SizeOf(Options[i]));
            }

            // Initialize a INTERNET_PER_CONN_OPTION_LIST instance.
            INTERNET_PER_CONN_OPTION_LIST Request = new INTERNET_PER_CONN_OPTION_LIST();

            // Point to the allocated memory.
            Request.pOptions = buffer;

            Request.Size = Marshal.SizeOf(Request);

            // IntPtr.Zero means LAN connection.
            Request.Connection = IntPtr.Zero;

            Request.OptionCount = Options.Length;
            Request.OptionError = 0;
            int size = Marshal.SizeOf(Request);

            // Query internet options. 
            bool result = InternetQueryOptionList(IntPtr.Zero, INTERNET_OPTION.INTERNET_OPTION_PER_CONNECTION_OPTION, ref Request, ref size);
            if (!result)
            {
                throw new ApplicationException(" Set Internet Option Failed! ");
            }

            return Request;
        }

        /// <summary>
        /// Restore the options for LAN connection.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool RestoreSystemProxy()
        {
            IntPtr hInternet = InternetOpen(applicationName, INTERNET_OPEN_TYPE_DIRECT, null, null, 0);

            INTERNET_PER_CONN_OPTION_LIST request = GetSystemProxy();
            int size = Marshal.SizeOf(request);

            // Allocate memory. 
            IntPtr intptrStruct = Marshal.AllocCoTaskMem(size);

            // Convert structure to IntPtr 
            Marshal.StructureToPtr(request, intptrStruct, true);

            // Set internet options.
            bool bReturn = InternetSetOption(hInternet, INTERNET_OPTION.INTERNET_OPTION_PER_CONNECTION_OPTION, intptrStruct, size);

            // Free the allocated memory.
            Marshal.FreeCoTaskMem(request.pOptions);
            Marshal.FreeCoTaskMem(intptrStruct);

            if (!bReturn)
            {
                throw new ApplicationException(" Set Internet Option Failed! ");
            }

            // Notify the system that the registry settings have been changed and cause
            // the proxy data to be reread from the registry for a handle.
            InternetSetOption(hInternet, INTERNET_OPTION.INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            InternetSetOption(hInternet, INTERNET_OPTION.INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);

            InternetCloseHandle(hInternet);

            return bReturn;
        }

        #region Nested type: INTERNET_OPTION

        private enum INTERNET_OPTION
        {
            // Sets or retrieves an INTERNET_PER_CONN_OPTION_LIST structure that specifies
            // a list of options for a particular connection.
            INTERNET_OPTION_PER_CONNECTION_OPTION = 75,

            // Notify the system that the registry settings have been changed so that
            // it verifies the settings on the next call to InternetConnect.
            INTERNET_OPTION_SETTINGS_CHANGED = 39,

            // Causes the proxy data to be reread from the registry for a handle.
            INTERNET_OPTION_REFRESH = 37
        }

        #endregion

        #region Nested type: INTERNET_OPTION_PER_CONN_FLAGS

        /// <summary>
        /// Constants used in INTERNET_PER_CONN_OPTON struct.
        /// </summary>
        private enum INTERNET_OPTION_PER_CONN_FLAGS
        {
            PROXY_TYPE_DIRECT = 0x00000001, // direct to net
            PROXY_TYPE_PROXY = 0x00000002, // via named proxy
            PROXY_TYPE_AUTO_PROXY_URL = 0x00000004, // autoproxy URL
            PROXY_TYPE_AUTO_DETECT = 0x00000008 // use autoproxy detection
        }

        #endregion

        #region Nested type: INTERNET_PER_CONN_OPTION

        [StructLayout(LayoutKind.Sequential)]
        private struct INTERNET_PER_CONN_OPTION
        {
            // A value in INTERNET_PER_CONN_OptionEnum.
            public int dwOption;
            public INTERNET_PER_CONN_OPTION_OptionUnion Value;
        }

        #endregion

        #region Nested type: INTERNET_PER_CONN_OPTION_LIST

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        private struct INTERNET_PER_CONN_OPTION_LIST
        {
            public int Size;

            // The connection to be set. NULL means LAN.
            public System.IntPtr Connection;

            public int OptionCount;
            public int OptionError;

            // List of INTERNET_PER_CONN_OPTIONs.
            public System.IntPtr pOptions;
        }

        #endregion

        #region Nested type: INTERNET_PER_CONN_OPTION_OptionUnion

        /// <summary>
        /// Used in INTERNET_PER_CONN_OPTION.
        /// When create a instance of OptionUnion, only one filed will be used.
        /// The StructLayout and FieldOffset attributes could help to decrease the struct size.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        private struct INTERNET_PER_CONN_OPTION_OptionUnion
        {
            // A value in INTERNET_OPTION_PER_CONN_FLAGS.
            [FieldOffset(0)] public int dwValue;
            [FieldOffset(0)] public System.IntPtr pszValue;
            [FieldOffset(0)] public System.Runtime.InteropServices.ComTypes.FILETIME ftValue;
        }

        #endregion

        #region Nested type: INTERNET_PER_CONN_OptionEnum

        private enum INTERNET_PER_CONN_OptionEnum
        {
            INTERNET_PER_CONN_FLAGS = 1,
            INTERNET_PER_CONN_PROXY_SERVER = 2,
            INTERNET_PER_CONN_PROXY_BYPASS = 3,
            INTERNET_PER_CONN_AUTOCONFIG_URL = 4,
            INTERNET_PER_CONN_AUTODISCOVERY_FLAGS = 5,
            INTERNET_PER_CONN_AUTOCONFIG_SECONDARY_URL = 6,
            INTERNET_PER_CONN_AUTOCONFIG_RELOAD_DELAY_MINS = 7,
            INTERNET_PER_CONN_AUTOCONFIG_LAST_DETECT_TIME = 8,
            INTERNET_PER_CONN_AUTOCONFIG_LAST_DETECT_URL = 9,
            INTERNET_PER_CONN_FLAGS_UI = 10
        }

        #endregion
    }
}