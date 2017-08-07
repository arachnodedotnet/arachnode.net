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
using System.Security;

#endregion

namespace Arachnode.Renderer
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SECURITY_ATTRIBUTES
    {
        [MarshalAs(UnmanagedType.U4)] public uint nLength;
        public IntPtr lpSecurityDescriptor;
        [MarshalAs(UnmanagedType.Bool)] public bool bInheritHandle;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BINDINFO
    {
        [MarshalAs(UnmanagedType.U4)] public uint cbSize;
        [MarshalAs(UnmanagedType.LPWStr)] public string szExtraInfo;
        [MarshalAs(UnmanagedType.Struct)] public STGMEDIUM stgmedData;
        [MarshalAs(UnmanagedType.U4)] public uint grfBindInfoF;
        [MarshalAs(UnmanagedType.U4)] public uint dwBindVerb;
        [MarshalAs(UnmanagedType.LPWStr)] public string szCustomVerb;
        [MarshalAs(UnmanagedType.U4)] public uint cbstgmedData;
        [MarshalAs(UnmanagedType.U4)] public uint dwOptions;
        [MarshalAs(UnmanagedType.U4)] public uint dwOptionsFlags;
        [MarshalAs(UnmanagedType.U4)] public uint dwCodePage;
        [MarshalAs(UnmanagedType.Struct)] public SECURITY_ATTRIBUTES securityAttributes;
        public Guid iid;
        [MarshalAs(UnmanagedType.IUnknown)] public object punk;
        [MarshalAs(UnmanagedType.U4)] public uint dwReserved;
    }

    /// <summary>
    /// Contains parameters that describe a control's keyboard mnemonics and keyboard behavior.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class tagCONTROLINFO
    {
        /// <summary>
        /// Size of the CONTROLINFO structure.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)] public int cb;

        /// <summary>
        /// Handle to an array of Windows ACCEL structures, each structure describing a keyboard mnemonic.
        /// </summary>
        public IntPtr hAccel;

        /// <summary>
        /// Number of mnemonics described in the hAccel field.
        /// </summary>
        [MarshalAs(UnmanagedType.U2)] public short cAccel;

        /// <summary>
        /// Flags that indicate the keyboard behavior of the control.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)] public tagCTRLINFO dwFlags;

        /// <summary>
        /// Initializes a new instance of the <see cref="tagCONTROLINFO"/> class.
        /// </summary>
        public tagCONTROLINFO()
        {
            cb = Marshal.SizeOf(typeof (tagCONTROLINFO));
        }
    }

    [ComVisible(true), StructLayout(LayoutKind.Sequential)]
    public struct MSG
    {
        public IntPtr hwnd;
        [MarshalAs(UnmanagedType.I4)] public int message;
        public IntPtr wParam;
        public IntPtr lParam;
        [MarshalAs(UnmanagedType.I4)] public int time;
        // pt was a by-value POINT structure
        [MarshalAs(UnmanagedType.I4)] public int pt_x;
        [MarshalAs(UnmanagedType.I4)] public int pt_y;
        //public tagPOINT pt;
    }

    public struct tagPOINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct tagMSG
    {
        public IntPtr hwnd;
        public uint message;
        public uint wParam;
        public int lParam;
        public uint time;
        public tagPOINT pt;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct OLECMD
    {
        [MarshalAs(UnmanagedType.U4)] public int cmdID;
        [MarshalAs(UnmanagedType.U4)] public int cmdf;
    }

    /// <summary>
    /// Flags that indicate the keyboard behavior of the control.
    /// </summary>
    [Flags]
    public enum tagCTRLINFO
    {
        /// <summary>
        /// When the control has the focus, it will process the Return key.
        /// </summary>
        CTRLINFO_EATS_RETURN = 1,

        /// <summary>
        /// When the control has the focus, it will process the Escape key.
        /// </summary>
        CTRLINFO_EATS_ESCAPE = 2
    }

    public enum CLSCTX
    {
        CLSCTX_INPROC_SERVER = 0x1,
        CLSCTX_INPROC_HANDLER = 0x2,
        CLSCTX_LOCAL_SERVER = 0x4,
        CLSCTX_INPROC_SERVER16 = 0x8,
        CLSCTX_REMOTE_SERVER = 0x10,
        CLSCTX_INPROC_HANDLER16 = 0x20,
        CLSCTX_RESERVED1 = 0x40,
        CLSCTX_RESERVED2 = 0x80,
        CLSCTX_RESERVED3 = 0x100,
        CLSCTX_RESERVED4 = 0x200,
        CLSCTX_NO_CODE_DOWNLOAD = 0x400,
        CLSCTX_RESERVED5 = 0x800,
        CLSCTX_NO_CUSTOM_MARSHAL = 0x1000,
        CLSCTX_ENABLE_CODE_DOWNLOAD = 0x2000,
        CLSCTX_NO_FAILURE_LOG = 0x4000,
        CLSCTX_DISABLE_AAA = 0x8000,
        CLSCTX_ENABLE_AAA = 0x10000,
        CLSCTX_FROM_DEFAULT_CONTEXT = 0x20000,
        CLSCTX_INPROC = CLSCTX_INPROC_SERVER | CLSCTX_INPROC_HANDLER,
        CLSCTX_SERVER = CLSCTX_INPROC_SERVER | CLSCTX_LOCAL_SERVER | CLSCTX_REMOTE_SERVER,
        CLSCTX_ALL = CLSCTX_SERVER | CLSCTX_INPROC_HANDLER
    }

    public enum OLEDOVERB
    {
        OLEIVERB_DISCARDUNDOSTATE = -6,
        OLEIVERB_HIDE = -3,
        OLEIVERB_INPLACEACTIVATE = -5,
        OLECLOSE_NOSAVE = 1,
        OLEIVERB_OPEN = -2,
        OLEIVERB_PRIMARY = 0,
        OLEIVERB_PROPERTIES = -7,
        OLEIVERB_SHOW = -1,
        OLEIVERB_UIACTIVATE = -4
    }

    public enum OLECMDID
    {
        OLECMDID_OPEN = 1,
        OLECMDID_NEW = 2,
        OLECMDID_SAVE = 3,
        OLECMDID_SAVEAS = 4,
        OLECMDID_SAVECOPYAS = 5,
        OLECMDID_PRINT = 6,
        OLECMDID_PRINTPREVIEW = 7,
        OLECMDID_PAGESETUP = 8,
        OLECMDID_SPELL = 9,
        OLECMDID_PROPERTIES = 10,
        OLECMDID_CUT = 11,
        OLECMDID_COPY = 12,
        OLECMDID_PASTE = 13,
        OLECMDID_PASTESPECIAL = 14,
        OLECMDID_UNDO = 15,
        OLECMDID_REDO = 16,
        OLECMDID_SELECTALL = 17,
        OLECMDID_CLEARSELECTION = 18,
        OLECMDID_ZOOM = 19,
        OLECMDID_GETZOOMRANGE = 20,
        OLECMDID_UPDATECOMMANDS = 21,
        OLECMDID_REFRESH = 22,
        OLECMDID_STOP = 23,
        OLECMDID_HIDETOOLBARS = 24,
        OLECMDID_SETPROGRESSMAX = 25,
        OLECMDID_SETPROGRESSPOS = 26,
        OLECMDID_SETPROGRESSTEXT = 27,
        OLECMDID_SETTITLE = 28,
        OLECMDID_SETDOWNLOADSTATE = 29,
        OLECMDID_STOPDOWNLOAD = 30,
        OLECMDID_ONTOOLBARACTIVATED = 31,
        OLECMDID_FIND = 32,
        OLECMDID_DELETE = 33,
        OLECMDID_HTTPEQUIV = 34,
        OLECMDID_HTTPEQUIV_DONE = 35,
        OLECMDID_ENABLE_INTERACTION = 36,
        OLECMDID_ONUNLOAD = 37,
        OLECMDID_PROPERTYBAG2 = 38,
        OLECMDID_PREREFRESH = 39,
        OLECMDID_SHOWSCRIPTERROR = 40,
        OLECMDID_SHOWMESSAGE = 41,
        OLECMDID_SHOWFIND = 42,
        OLECMDID_SHOWPAGESETUP = 43,
        OLECMDID_SHOWPRINT = 44,
        OLECMDID_CLOSE = 45,
        OLECMDID_ALLOWUILESSSAVEAS = 46,
        OLECMDID_DONTDOWNLOADCSS = 47,
        OLECMDID_UPDATEPAGESTATUS = 48,
        OLECMDID_PRINT2 = 49,
        OLECMDID_PRINTPREVIEW2 = 50,
        OLECMDID_SETPRINTTEMPLATE = 51,
        OLECMDID_GETPRINTTEMPLATE = 52,
        OLECMDID_PAGEACTIONBLOCKED = 55,
        OLECMDID_PAGEACTIONUIQUERY = 56,
        OLECMDID_FOCUSVIEWCONTROLS = 57,
        OLECMDID_FOCUSVIEWCONTROLSQUERY = 58,
        OLECMDID_SHOWPAGEACTIONMENU = 59,
        OLECMDID_ADDTRAVELENTRY = 60,
        OLECMDID_UPDATETRAVELENTRY = 61,
        OLECMDID_UPDATEBACKFORWARDSTATE = 62,
        OLECMDID_OPTICAL_ZOOM = 63,
        OLECMDID_OPTICAL_GETZOOMRANGE = 64,
        OLECMDID_WINDOWSTATECHANGED = 65,
        //OLECMDID_IE7_SHOWSCRIPTERROR = 69
    }

    public enum OLECMDEXECOPT
    {
        OLECMDEXECOPT_DODEFAULT = 0,
        OLECMDEXECOPT_PROMPTUSER = 1,
        OLECMDEXECOPT_DONTPROMPTUSER = 2,
        OLECMDEXECOPT_SHOWHELP = 3
    }

    public enum OLECMDF
    {
        OLECMDF_SUPPORTED = 1,
        OLECMDF_ENABLED = 2,
        OLECMDF_LATCHED = 4,
        OLECMDF_NINCHED = 8,
        OLECMDF_INVISIBLE = 16,
        OLECMDF_DEFHIDEONCTXTMENU = 32
    }

    public enum tagURLZONE
    {
        URLZONE_PREDEFINED_MIN = 0,
        URLZONE_LOCAL_MACHINE = 0,
        URLZONE_INTRANET = URLZONE_LOCAL_MACHINE + 1,
        URLZONE_TRUSTED = URLZONE_INTRANET + 1,
        URLZONE_INTERNET = URLZONE_TRUSTED + 1,
        URLZONE_UNTRUSTED = URLZONE_INTERNET + 1,
        URLZONE_PREDEFINED_MAX = 999,
        URLZONE_USER_MIN = 1000,
        URLZONE_USER_MAX = 10000,
        URLZONE_ESC_FLAG = 0x100
    }

    public enum WinInetErrors
    {
        HTTP_STATUS_CONTINUE = 100, //The request can be continued.
        HTTP_STATUS_SWITCH_PROTOCOLS = 101, //The server has switched protocols in an upgrade header.
        HTTP_STATUS_OK = 200, //The request completed successfully.
        HTTP_STATUS_CREATED = 201, //The request has been fulfilled and resulted in the creation of a new resource.
        HTTP_STATUS_ACCEPTED = 202, //The request has been accepted for processing, but the processing has not been completed.
        HTTP_STATUS_PARTIAL = 203, //The returned meta information in the entity-header is not the definitive set available from the origin server.
        HTTP_STATUS_NO_CONTENT = 204, //The server has fulfilled the request, but there is no new information to send back.
        HTTP_STATUS_RESET_CONTENT = 205, //The request has been completed, and the client program should reset the document view that caused the request to be sent to allow the user to easily initiate another input action.
        HTTP_STATUS_PARTIAL_CONTENT = 206, //The server has fulfilled the partial GET request for the resource.
        HTTP_STATUS_AMBIGUOUS = 300, //The server couldn't decide what to return.
        HTTP_STATUS_MOVED = 301, //The requested resource has been assigned to a new permanent URI (Uniform Resource Identifier), and any future references to this resource should be done using one of the returned URIs.
        HTTP_STATUS_REDIRECT = 302, //The requested resource resides temporarily under a different URI (Uniform Resource Identifier).
        HTTP_STATUS_REDIRECT_METHOD = 303, //The response to the request can be found under a different URI (Uniform Resource Identifier) and should be retrieved using a GET HTTP verb on that resource.
        HTTP_STATUS_NOT_MODIFIED = 304, //The requested resource has not been modified.
        HTTP_STATUS_USE_PROXY = 305, //The requested resource must be accessed through the proxy given by the location field.
        HTTP_STATUS_REDIRECT_KEEP_VERB = 307, //The redirected request keeps the same HTTP verb. HTTP/1.1 behavior.

        HTTP_STATUS_BAD_REQUEST = 400,
        HTTP_STATUS_DENIED = 401,
        HTTP_STATUS_PAYMENT_REQ = 402,
        HTTP_STATUS_FORBIDDEN = 403,
        HTTP_STATUS_NOT_FOUND = 404,
        HTTP_STATUS_BAD_METHOD = 405,
        HTTP_STATUS_NONE_ACCEPTABLE = 406,
        HTTP_STATUS_PROXY_AUTH_REQ = 407,
        HTTP_STATUS_REQUEST_TIMEOUT = 408,
        HTTP_STATUS_CONFLICT = 409,
        HTTP_STATUS_GONE = 410,
        HTTP_STATUS_LENGTH_REQUIRED = 411,
        HTTP_STATUS_PRECOND_FAILED = 412,
        HTTP_STATUS_REQUEST_TOO_LARGE = 413,
        HTTP_STATUS_URI_TOO_LONG = 414,
        HTTP_STATUS_UNSUPPORTED_MEDIA = 415,
        HTTP_STATUS_RETRY_WITH = 449,
        HTTP_STATUS_SERVER_ERROR = 500,
        HTTP_STATUS_NOT_SUPPORTED = 501,
        HTTP_STATUS_BAD_GATEWAY = 502,
        HTTP_STATUS_SERVICE_UNAVAIL = 503,
        HTTP_STATUS_GATEWAY_TIMEOUT = 504,
        HTTP_STATUS_VERSION_NOT_SUP = 505,

        ERROR_INTERNET_ASYNC_THREAD_FAILED = 12047, //The application could not start an asynchronous thread.
        ERROR_INTERNET_BAD_AUTO_PROXY_SCRIPT = 12166, //There was an error in the automatic proxy configuration script.
        ERROR_INTERNET_BAD_OPTION_LENGTH = 12010, //The length of an option supplied to InternetQueryOption or InternetSetOption is incorrect for the type of option specified.
        ERROR_INTERNET_BAD_REGISTRY_PARAMETER = 12022, //A required registry value was located but is an incorrect type or has an invalid value.
        ERROR_INTERNET_CANNOT_CONNECT = 12029, //The attempt to connect to the server failed.
        ERROR_INTERNET_CHG_POST_IS_NON_SECURE = 12042, //The application is posting and attempting to change multiple lines of text on a server that is not secure.
        ERROR_INTERNET_CLIENT_AUTH_CERT_NEEDED = 12044, //The server is requesting client authentication.
        ERROR_INTERNET_CLIENT_AUTH_NOT_SETUP = 12046, //Client authorization is not set up on this computer.
        ERROR_INTERNET_CONNECTION_ABORTED = 12030, //The connection with the server has been terminated.
        ERROR_INTERNET_CONNECTION_RESET = 12031, //The connection with the server has been reset.
        ERROR_INTERNET_DIALOG_PENDING = 12049, //Another thread has a password dialog box in progress.
        ERROR_INTERNET_DISCONNECTED = 12163, //The Internet connection has been lost.
        ERROR_INTERNET_EXTENDED_ERROR = 12003, //An extended error was returned from the server. This is typically a string or buffer containing a verbose error message. Call InternetGetLastResponseInfo to retrieve the error text.
        ERROR_INTERNET_FAILED_DUETOSECURITYCHECK = 12171, //The function failed due to a security check.
        ERROR_INTERNET_FORCE_RETRY = 12032, //The function needs to redo the request.
        ERROR_INTERNET_FORTEZZA_LOGIN_NEEDED = 12054, //The requested resource requires Fortezza authentication.
        ERROR_INTERNET_HANDLE_EXISTS = 12036, //The request failed because the handle already exists.
        ERROR_INTERNET_HTTP_TO_HTTPS_ON_REDIR = 12039, //The application is moving from a non-SSL to an SSL connection because of a redirect.
        ERROR_INTERNET_HTTPS_HTTP_SUBMIT_REDIR = 12052, //The data being submitted to an SSL connection is being redirected to a non-SSL connection.
        ERROR_INTERNET_HTTPS_TO_HTTP_ON_REDIR = 12040, //The application is moving from an SSL to an non-SSL connection because of a redirect.
        ERROR_INTERNET_INCORRECT_FORMAT = 12027, //The format of the request is invalid.
        ERROR_INTERNET_INCORRECT_HANDLE_STATE = 12019, //The requested operation cannot be carried out because the handle supplied is not in the correct state.
        ERROR_INTERNET_INCORRECT_HANDLE_TYPE = 12018, //The type of handle supplied is incorrect for this operation.
        ERROR_INTERNET_INCORRECT_PASSWORD = 12014, //The request to connect and log on to an FTP server could not be completed because the supplied password is incorrect.
        ERROR_INTERNET_INCORRECT_USER_NAME = 12013, //The request to connect and log on to an FTP server could not be completed because the supplied user name is incorrect.
        ERROR_INTERNET_INSERT_CDROM = 12053, //The request requires a CD-ROM to be inserted in the CD-ROM drive to locate the resource requested.
        ERROR_INTERNET_INTERNAL_ERROR = 12004, //An internal error has occurred.
        ERROR_INTERNET_INVALID_CA = 12045, //The function is unfamiliar with the Certificate Authority that generated the server's certificate.
        ERROR_INTERNET_INVALID_OPERATION = 12016, //The requested operation is invalid.
        ERROR_INTERNET_INVALID_OPTION = 12009, //A request to InternetQueryOption or InternetSetOption specified an invalid option value.
        ERROR_INTERNET_INVALID_PROXY_REQUEST = 12033, //The request to the proxy was invalid.
        ERROR_INTERNET_INVALID_URL = 12005, //The URL is invalid.
        ERROR_INTERNET_ITEM_NOT_FOUND = 12028, //The requested item could not be located.
        ERROR_INTERNET_LOGIN_FAILURE = 12015, //The request to connect and log on to an FTP server failed.
        ERROR_INTERNET_LOGIN_FAILURE_DISPLAY_ENTITY_BODY = 12174, //The MS-Logoff digest header has been returned from the Web site. This header specifically instructs the digest package to purge credentials for the associated realm. This error will only be returned if INTERNET_ERROR_MASK_LOGIN_FAILURE_DISPLAY_ENTITY_BODY has been set.
        ERROR_INTERNET_MIXED_SECURITY = 12041, //The content is not entirely secure. Some of the content being viewed may have come from unsecured servers.
        ERROR_INTERNET_NAME_NOT_RESOLVED = 12007, //The server name could not be resolved.
        ERROR_INTERNET_NEED_MSN_SSPI_PKG = 12173, //Not currently implemented.
        ERROR_INTERNET_NEED_UI = 12034, //A user interface or other blocking operation has been requested.
        ERROR_INTERNET_NO_CALLBACK = 12025, //An asynchronous request could not be made because a callback function has not been set.
        ERROR_INTERNET_NO_CONTEXT = 12024, //An asynchronous request could not be made because a zero context value was supplied.
        ERROR_INTERNET_NO_DIRECT_ACCESS = 12023, //Direct network access cannot be made at this time.
        ERROR_INTERNET_NOT_INITIALIZED = 12172, //Initialization of the WinINet API has not occurred. Indicates that a higher-level function, such as InternetOpen, has not been called yet.
        ERROR_INTERNET_NOT_PROXY_REQUEST = 12020, //The request cannot be made via a proxy.
        ERROR_INTERNET_OPERATION_CANCELLED = 12017, //The operation was canceled, usually because the handle on which the request was operating was closed before the operation completed.
        ERROR_INTERNET_OPTION_NOT_SETTABLE = 12011, //The requested option cannot be set, only queried.
        ERROR_INTERNET_OUT_OF_HANDLES = 12001, //No more handles could be generated at this time.
        ERROR_INTERNET_POST_IS_NON_SECURE = 12043, //The application is posting data to a server that is not secure.
        ERROR_INTERNET_PROTOCOL_NOT_FOUND = 12008, //The requested protocol could not be located.
        ERROR_INTERNET_PROXY_SERVER_UNREACHABLE = 12165, //The designated proxy server cannot be reached.
        ERROR_INTERNET_REDIRECT_SCHEME_CHANGE = 12048, //The function could not handle the redirection, because the scheme changed (for example, HTTP to FTP).
        ERROR_INTERNET_REGISTRY_VALUE_NOT_FOUND = 12021, //A required registry value could not be located.
        ERROR_INTERNET_REQUEST_PENDING = 12026, //The required operation could not be completed because one or more requests are pending.
        ERROR_INTERNET_RETRY_DIALOG = 12050, //The dialog box should be retried.
        ERROR_INTERNET_SEC_CERT_CN_INVALID = 12038, //SSL certificate common name (host name field) is incorrect—for example, if you entered www.server.com and the common name on the certificate says www.different.com.
        ERROR_INTERNET_SEC_CERT_DATE_INVALID = 12037, //SSL certificate date that was received from the server is bad. The certificate is expired.
        ERROR_INTERNET_SEC_CERT_ERRORS = 12055, //The SSL certificate contains errors.
        ERROR_INTERNET_SEC_CERT_NO_REV = 12056,
        ERROR_INTERNET_SEC_CERT_REV_FAILED = 12057,
        ERROR_INTERNET_SEC_CERT_REVOKED = 12170, //SSL certificate was revoked.
        ERROR_INTERNET_SEC_INVALID_CERT = 12169, //SSL certificate is invalid.
        ERROR_INTERNET_SECURITY_CHANNEL_ERROR = 12157, //The application experienced an internal error loading the SSL libraries.
        ERROR_INTERNET_SERVER_UNREACHABLE = 12164, //The Web site or server indicated is unreachable.
        ERROR_INTERNET_SHUTDOWN = 12012, //WinINet support is being shut down or unloaded.
        ERROR_INTERNET_TCPIP_NOT_INSTALLED = 12159, //The required protocol stack is not loaded and the application cannot start WinSock.
        ERROR_INTERNET_TIMEOUT = 12002, //The request has timed out.
        ERROR_INTERNET_UNABLE_TO_CACHE_FILE = 12158, //The function was unable to cache the file.
        ERROR_INTERNET_UNABLE_TO_DOWNLOAD_SCRIPT = 12167, //The automatic proxy configuration script could not be downloaded. The INTERNET_FLAG_MUST_CACHE_REQUEST flag was set.

        INET_E_INVALID_URL = unchecked((int) 0x800C0002),
        INET_E_NO_SESSION = unchecked((int) 0x800C0003),
        INET_E_CANNOT_CONNECT = unchecked((int) 0x800C0004),
        INET_E_RESOURCE_NOT_FOUND = unchecked((int) 0x800C0005),
        INET_E_OBJECT_NOT_FOUND = unchecked((int) 0x800C0006),
        INET_E_DATA_NOT_AVAILABLE = unchecked((int) 0x800C0007),
        INET_E_DOWNLOAD_FAILURE = unchecked((int) 0x800C0008),
        INET_E_AUTHENTICATION_REQUIRED = unchecked((int) 0x800C0009),
        INET_E_NO_VALID_MEDIA = unchecked((int) 0x800C000A),
        INET_E_CONNECTION_TIMEOUT = unchecked((int) 0x800C000B),
        INET_E_DEFAULT_ACTION = unchecked((int) 0x800C0011),
        INET_E_INVALID_REQUEST = unchecked((int) 0x800C000C),
        INET_E_UNKNOWN_PROTOCOL = unchecked((int) 0x800C000D),
        INET_E_QUERYOPTION_UNKNOWN = unchecked((int) 0x800C0013),
        INET_E_SECURITY_PROBLEM = unchecked((int) 0x800C000E),
        INET_E_CANNOT_LOAD_DATA = unchecked((int) 0x800C000F),
        INET_E_CANNOT_INSTANTIATE_OBJECT = unchecked((int) 0x800C0010),
        INET_E_REDIRECT_FAILED = unchecked((int) 0x800C0014),
        INET_E_REDIRECT_TO_DIR = unchecked((int) 0x800C0015),
        INET_E_CANNOT_LOCK_REQUEST = unchecked((int) 0x800C0016),
        INET_E_USE_EXTEND_BINDING = unchecked((int) 0x800C0017),
        INET_E_TERMINATED_BIND = unchecked((int) 0x800C0018),
        INET_E_ERROR_FIRST = unchecked((int) 0x800C0002),
        INET_E_CODE_DOWNLOAD_DECLINED = unchecked((int) 0x800C0100),
        INET_E_RESULT_DISPATCHED = unchecked((int) 0x800C0200),
        INET_E_CANNOT_REPLACE_SFP_FILE = unchecked((int) 0x800C0300),

        HTTP_COOKIE_DECLINED = 12162, //The HTTP cookie was declined by the server.
        HTTP_COOKIE_NEEDS_CONFIRMATION = 12161, //The HTTP cookie requires confirmation.
        HTTP_DOWNLEVEL_SERVER = 12151, //The server did not return any headers.
        HTTP_HEADER_ALREADY_EXISTS = 12155, //The header could not be added because it already exists.
        HTTP_HEADER_NOT_FOUND = 12150, //The requested header could not be located.
        HTTP_INVALID_HEADER = 12153, //The supplied header is invalid.
        HTTP_INVALID_QUERY_REQUEST = 12154, //The request made to HttpQueryInfo is invalid.
        HTTP_INVALID_SERVER_RESPONSE = 12152, //The server response could not be parsed.
        HTTP_NOT_REDIRECTED = 12160, //The HTTP request was not redirected.
        HTTP_REDIRECT_FAILED = 12156, //The redirection failed because either the scheme changed (for example, HTTP to FTP) or all attempts made to redirect failed (default is five attempts).
        HTTP_REDIRECT_NEEDS_CONFIRMATION = 12168 //The redirection requires user confirmation.
    }

    public enum SZM_FLAGS
    {
        SZM_CREATE = 0,
        SZM_DELETE = 0x1
    }

    [ComVisible(true), Guid("0000011B-0000-0000-C000-000000000046"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IOleContainer
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int ParseDisplayName([In, MarshalAs(UnmanagedType.Interface)] Object pbc,
                             [In, MarshalAs(UnmanagedType.LPWStr)] String pszDisplayName, [Out,
                                                                                           MarshalAs(UnmanagedType.LPArray)] int[] pchEaten, [Out,
                                                                                                                                              MarshalAs(UnmanagedType.LPArray)] Object[] ppmkOut);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int EnumObjects([In, MarshalAs(UnmanagedType.U4)] uint grfFlags, [Out,
                                                                          MarshalAs(UnmanagedType.LPArray)] Object[] ppenum);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int LockContainer([In, MarshalAs(UnmanagedType.Bool)] Boolean fLock);
    }

    [ComVisible(true), Guid("00000118-0000-0000-C000-000000000046"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IOleClientSite
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SaveObject();

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetMoniker([In, MarshalAs(UnmanagedType.U4)] uint dwAssign,
                       [In, MarshalAs(UnmanagedType.U4)] uint dwWhichMoniker,
                       [Out, MarshalAs(UnmanagedType.Interface)] out Object ppmk);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetContainer([Out] out IOleContainer ppContainer);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int ShowObject();

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int OnShowWindow([In, MarshalAs(UnmanagedType.I4)] int fShow);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int RequestNewObjectLayout();
    }

    [ComVisible(true), ComImport,
     Guid("00000112-0000-0000-C000-000000000046"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IOleObject
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SetClientSite([In, MarshalAs(UnmanagedType.Interface)] IOleClientSite
                              pClientSite);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetClientSite([Out, MarshalAs(UnmanagedType.Interface)] out IOleClientSite site);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SetHostNames([In, MarshalAs(UnmanagedType.LPWStr)] String
                             szContainerApp, [In, MarshalAs(UnmanagedType.LPWStr)] String
                                                 szContainerObj);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int Close([In, MarshalAs(UnmanagedType.U4)] uint dwSaveOption);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SetMoniker([In, MarshalAs(UnmanagedType.U4)] uint dwWhichMoniker, [In,
                                                                               MarshalAs(UnmanagedType.Interface)] Object pmk);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetMoniker([In, MarshalAs(UnmanagedType.U4)] uint dwAssign, [In,
                                                                         MarshalAs(UnmanagedType.U4)] uint dwWhichMoniker, [Out, MarshalAs(UnmanagedType.Interface)] out Object moniker);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int InitFromData([In, MarshalAs(UnmanagedType.Interface)] Object
                             pDataObject, [In, MarshalAs(UnmanagedType.Bool)] Boolean fCreation, [In,
                                                                                                  MarshalAs(UnmanagedType.U4)] uint dwReserved);

        int GetClipboardData([In, MarshalAs(UnmanagedType.U4)] uint dwReserved, out
                                                                                    Object data);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int DoVerb([In, MarshalAs(UnmanagedType.I4)] int iVerb, [In] IntPtr lpmsg,
                   [In, MarshalAs(UnmanagedType.Interface)] IOleClientSite pActiveSite, [In,
                                                                                         MarshalAs(UnmanagedType.I4)] int lindex, [In] IntPtr hwndParent, [In] RECT
                                                                                                                                                              lprcPosRect);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int EnumVerbs(out Object e); // IEnumOLEVERB
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int OleUpdate();

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int IsUpToDate();

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetUserClassID([In, Out] ref Guid pClsid);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetUserType([In, MarshalAs(UnmanagedType.U4)] uint dwFormOfType, [Out,
                                                                              MarshalAs(UnmanagedType.LPWStr)] out String userType);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SetExtent([In, MarshalAs(UnmanagedType.U4)] uint dwDrawAspect, [In] Object pSizel); // tagSIZEL
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetExtent([In, MarshalAs(UnmanagedType.U4)] uint dwDrawAspect, [Out] Object pSizel); // tagSIZEL
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int Advise([In, MarshalAs(UnmanagedType.Interface)] IAdviseSink pAdvSink, out
                                                                                      int cookie);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int Unadvise([In, MarshalAs(UnmanagedType.U4)] int dwConnection);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int EnumAdvise(out Object e);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetMiscStatus([In, MarshalAs(UnmanagedType.U4)] uint dwAspect, out int
                                                                               misc);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SetColorScheme([In] Object pLogpal); // tagLOGPALETTE
    }

    [ComVisible(true), Guid("0000010f-0000-0000-C000-000000000046"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAdviseSink
    {
        void OnDataChange(
            [In] object pFormatetc,
            [In] object pStgmed
            );

        void OnViewChange(
            [In, MarshalAs(UnmanagedType.U4)] int dwAspect,
            [In, MarshalAs(UnmanagedType.I4)] int lindex
            );

        void OnRename(
            [In, MarshalAs(UnmanagedType.Interface)] IMoniker pmk
            );

        void OnSave();
        void OnClose();
    }

    [ComImport, ComVisible(true)]
    [Guid("C4D244B0-D43E-11CF-893B-00AA00BDCE1A")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDocHostShowUI
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int ShowMessage(
            IntPtr hwnd,
            [MarshalAs(UnmanagedType.LPWStr)] string lpstrText,
            [MarshalAs(UnmanagedType.LPWStr)] string lpstrCaption,
            [MarshalAs(UnmanagedType.U4)] uint dwType,
            [MarshalAs(UnmanagedType.LPWStr)] string lpstrHelpFile,
            [MarshalAs(UnmanagedType.U4)] uint dwHelpContext,
            [In, Out] ref int lpResult);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int ShowHelp(
            IntPtr hwnd,
            [MarshalAs(UnmanagedType.LPWStr)] string pszHelpFile,
            [MarshalAs(UnmanagedType.U4)] uint uCommand,
            [MarshalAs(UnmanagedType.U4)] uint dwData,
            [In, MarshalAs(UnmanagedType.Struct)] tagPOINT ptMouse,
            [Out, MarshalAs(UnmanagedType.IDispatch)] object pDispatchObjectHit);
    }

    /// <summary>
    /// Standard notification interface that supports bindable and request-edit properties.
    /// </summary>
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("9BFBBC02-EFF1-101A-84ED-00AA00341D07")]
    public interface IPropertyNotifySink
    {
        /// <summary>
        /// Notifies a sink that the [bindable] property specified by <paramref name="dispID"/> has changed
        /// </summary>
        /// <param name="dispID">Dispatch identifier of the property that changed, or DISPID_UNKNOWN if multiple properties have changed.</param>
        int OnChanged(
            int dispID);

        /// <summary>
        /// Notifies a sink that a [requestedit] property is about to change and that the object is asking the sink how to proceed.
        /// </summary>
        /// <param name="dispID">Dispatch identifier of the property that is about to change or DISPID_UNKNOWN if multiple properties are about to change.</param>
        int OnRequestEdit(
            int dispID);
    }

    /// <summary>
    /// Provides the features for supporting keyboard mnemonics (GetControlInfo, OnMnemonic), ambient properties (OnAmbientPropertyChange), and events (FreezeEvents) in control objects.
    /// </summary>
    [ComImport, Guid("B196B288-BAB4-101A-B69C-00AA00341D07"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), SuppressUnmanagedCodeSecurity]
    public interface IOleControl
    {
        /// <summary>
        /// Fills in a CONTROLINFO structure with information about a control's keyboard mnemonics and keyboard behavior.
        /// </summary>
        /// <param name="pCI">Pointer to the caller-allocated CONTROLINFO structure to be filled in.</param>
        /// <returns><see cref="F:NativeMethods.HRESULT.S_OK"/> if the structure was filled successfully; <see cref="F:NativeMethods.HRESULT.E_NOTIMPL"/> the control has no mnemonics.</returns>
        [PreserveSig]
        int GetControlInfo(
            [Out] tagCONTROLINFO pCI);

        /// <summary>
        /// Informs a control that the user has pressed a keystroke that matches one of the ACCEL entries in the mnemonic table returned through <see cref="IOleControl.GetControlInfo"/>. The control takes whatever action is appropriate for the keystroke.
        /// </summary>
        /// <param name="pMsg">Pointer to the <see cref="NativeMethods.MSG"/> structure describing the keystroke to be processed.</param>
        void OnMnemonic(
            [In] ref MSG pMsg);

        /// <summary>
        /// Informs a control that one or more of the container's ambient properties (available through the control site's IDispatch) has changed.
        /// </summary>
        /// <param name="dispID">Dispatch identifier of the ambient property that changed. If the dispID parameter is DISPID_UNKNOWN, it indicates that multiple properties changed.</param>
        void OnAmbientPropertyChange(
            int dispID);

        /// <summary>
        /// Indicates whether the container is ignoring or accepting events from the control.
        /// </summary>
        /// <param name="bFreeze">Indicates whether the container will ignore (<see langword="true"/>) or now process (<see langword="false"/>) events from the control.</param>
        void FreezeEvents(
            [Out, MarshalAs(UnmanagedType.Bool)] bool bFreeze);
    }

    [ComImport, ComVisible(true),
     Guid("79eac9c9-baf9-11ce-8c82-00aa004ba90b"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPersistMoniker
    {
        void GetClassID(
            [In, Out] ref Guid pClassID);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int IsDirty();

        void Load([In] int fFullyAvailable,
                  [In, MarshalAs(UnmanagedType.Interface)] IMoniker pmk,
                  [In, MarshalAs(UnmanagedType.Interface)] Object pbc,
                  [In, MarshalAs(UnmanagedType.U4)] uint grfMode);

        void SaveCompleted(
            [In, MarshalAs(UnmanagedType.Interface)] IMoniker pmk,
            [In, MarshalAs(UnmanagedType.Interface)] Object pbc);

        [return: MarshalAs(UnmanagedType.Interface)]
        IMoniker GetCurMoniker();
    }

    [ComImport, ComVisible(true),
     Guid("79EAC9C0-BAF9-11CE-8C82-00AA004BA90B"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBinding
    {
        void Abort();
        void Suspend();
        void Resume();
        void SetPriority([In] int nPriority);
        void GetPriority(out int pnPriority);

        void GetBindResult(out Guid pclsidProtocol,
                           out uint pdwResult,
                           [MarshalAs(UnmanagedType.LPWStr)] out string pszResult,
                           [In, Out] ref uint dwReserved);
    }

    [ComImport, ComVisible(true),
     Guid("79EAC9C1-BAF9-11CE-8C82-00AA004BA90B"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBindStatusCallback
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int OnStartBinding(
            [In] uint dwReserved,
            [In, MarshalAs(UnmanagedType.Interface)] IBinding pib);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetPriority(out int pnPriority);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int OnLowResource([In] uint reserved);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int OnProgress(
            [In] uint ulProgress,
            [In] uint ulProgressMax,
            //[In] BINDSTATUS ulStatusCode,
            [In, MarshalAs(UnmanagedType.U4)] uint ulStatusCode,
            [In, MarshalAs(UnmanagedType.LPWStr)] string szStatusText);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int OnStopBinding(
            [In, MarshalAs(UnmanagedType.Error)] uint hresult,
            [In, MarshalAs(UnmanagedType.LPWStr)] string szError);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetBindInfo(
            //out BINDF grfBINDF,
            [In, Out, MarshalAs(UnmanagedType.U4)] ref uint grfBINDF,
            [In, Out, MarshalAs(UnmanagedType.Struct)] ref BINDINFO pbindinfo);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int OnDataAvailable(
            [In, MarshalAs(UnmanagedType.U4)] uint grfBSCF,
            [In, MarshalAs(UnmanagedType.U4)] uint dwSize,
            [In, MarshalAs(UnmanagedType.Struct)] ref FORMATETC pFormatetc,
            [In, MarshalAs(UnmanagedType.Struct)] ref STGMEDIUM pStgmed);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int OnObjectAvailable(
            [In] ref Guid riid,
            [In, MarshalAs(UnmanagedType.IUnknown)] object punk);
    }

    [ComImport, Guid("0000010c-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPersist
    {
        void GetClassID(Guid pClassId);
    }

    [ComImport, Guid("7FD52380-4E07-101B-AE2D-08002B2EC713"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPersistStreamInit : IPersist
    {
        void GetClassID([In, Out] ref Guid pClassId);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int IsDirty();

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
#pragma warning disable 612,618
        void Load(UCOMIStream pStm); //System.Runtime.InteropServices.ComTypes.IStream
#pragma warning restore 612,618
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
#pragma warning disable 612,618
        void Save(UCOMIStream pStm, [In, MarshalAs(UnmanagedType.Bool)] bool fClearDirty); //System.Runtime.InteropServices.ComTypes.IStream
#pragma warning restore 612,618
        void GetMaxSize([Out] long pCbSize);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        void InitNew();
    }

    [ComImport, ComVisible(true)]
    [Guid("53DEC138-A51E-11d2-861E-00C04FA35C89")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IHostDialogHelper
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int ShowHTMLDialog(
            [In] IntPtr hwndParent,
            [In, MarshalAs(UnmanagedType.Interface)] IMoniker pMk,
            [In] ref object pvarArgIn,
            [In, MarshalAs(UnmanagedType.LPWStr)] string pchOptions,
            [In, Out] ref object pvarArgOut,
            [In, MarshalAs(UnmanagedType.IUnknown)] object punkHost);
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("B722BCCB-4E68-101B-A2BC-00AA00404770"), ComVisible(true)]
    public interface IOleCommandTarget
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int QueryStatus(ref Guid pguidCmdGroup, int cCmds, [In, Out] OLECMD prgCmds, [In, Out] IntPtr pCmdText);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int Exec(ref Guid pguidCmdGroup, int nCmdID, int nCmdexecopt, [In, MarshalAs(UnmanagedType.LPArray)] object[] pvaIn, ref int pvaOut);
    }

    [ComVisible(true), ComImport,
     Guid("79EAC9EE-BAF9-11CE-8C82-00AA004BA90B"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IInternetSecurityManager
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SetSecuritySite(
            [In] IntPtr pSite);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetSecuritySite(
            out IntPtr pSite);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int MapUrlToZone(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pwszUrl,
            out UInt32 pdwZone,
            [In] UInt32 dwFlags);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetSecurityId(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pwszUrl,
            [Out] IntPtr pbSecurityId, [In, Out] ref UInt32 pcbSecurityId,
            [In] ref UInt32 dwReserved);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int ProcessUrlAction(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pwszUrl,
            UInt32 dwAction,
            IntPtr pPolicy, UInt32 cbPolicy,
            IntPtr pContext, UInt32 cbContext,
            UInt32 dwFlags,
            UInt32 dwReserved);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int QueryCustomPolicy(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pwszUrl,
            ref Guid guidKey,
            out IntPtr ppPolicy, out UInt32 pcbPolicy,
            IntPtr pContext, UInt32 cbContext,
            UInt32 dwReserved);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SetZoneMapping(
            UInt32 dwZone,
            [In, MarshalAs(UnmanagedType.LPWStr)] string lpszPattern,
            UInt32 dwFlags);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetZoneMappings(
            [In] UInt32 dwZone, //One or more of tagURLZONE enums
            out IEnumString ppenumString,
            [In] UInt32 dwFlags);
    }

    [ComImport, SuppressUnmanagedCodeSecurity, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("34A715A0-6587-11D0-924A-0020AFC7AC4D")]
    public interface DWebBrowserEvents2
    {
        [DispId(0x66)]
        void StatusTextChange([MarshalAs(UnmanagedType.BStr)] string Text);

        [DispId(0x6c)]
        void ProgressChange(int Progress, int ProgressMax);

        [DispId(0x69)]
        void CommandStateChange(int Command, [MarshalAs(UnmanagedType.VariantBool)] bool Enable);

        [DispId(0x6a)]
        void DownloadBegin();

        [DispId(0x68)]
        void DownloadComplete();

        [DispId(0x71)]
        void TitleChange([MarshalAs(UnmanagedType.BStr)] string Text);

        [DispId(0x70)]
        void PropertyChange([MarshalAs(UnmanagedType.BStr)] string szProperty);

        [DispId(250)]
        void BeforeNavigate2([MarshalAs(UnmanagedType.IDispatch)] object pDisp, [In] ref object URL, [In] ref object Flags, [In] ref object TargetFrameName, [In] ref object PostData, [In] ref object Headers, [In, Out, MarshalAs(UnmanagedType.VariantBool)] ref bool Cancel);

        [DispId(0xfb)]
        void NewWindow2([In, Out, MarshalAs(UnmanagedType.IDispatch)] ref object ppDisp, [In, Out, MarshalAs(UnmanagedType.VariantBool)] ref bool Cancel);

        [DispId(0xfc)]
        void NavigateComplete2([MarshalAs(UnmanagedType.IDispatch)] object pDisp, [In] ref object URL);

        [DispId(0x103)]
        void DocumentComplete([MarshalAs(UnmanagedType.IDispatch)] object pDisp, [In] ref object URL);

        [DispId(0xfd)]
        void OnQuit();

        [DispId(0xfe)]
        void OnVisible([MarshalAs(UnmanagedType.VariantBool)] bool Visible);

        [DispId(0xff)]
        void OnToolBar([MarshalAs(UnmanagedType.VariantBool)] bool ToolBar);

        [DispId(0x100)]
        void OnMenuBar([MarshalAs(UnmanagedType.VariantBool)] bool MenuBar);

        [DispId(0x101)]
        void OnStatusBar([MarshalAs(UnmanagedType.VariantBool)] bool StatusBar);

        [DispId(0x102)]
        void OnFullScreen([MarshalAs(UnmanagedType.VariantBool)] bool FullScreen);

        [DispId(260)]
        void OnTheaterMode([MarshalAs(UnmanagedType.VariantBool)] bool TheaterMode);

        [DispId(0x106)]
        void WindowSetResizable([MarshalAs(UnmanagedType.VariantBool)] bool Resizable);

        [DispId(0x108)]
        void WindowSetLeft(int Left);

        [DispId(0x109)]
        void WindowSetTop(int Top);

        [DispId(0x10a)]
        void WindowSetWidth(int Width);

        [DispId(0x10b)]
        void WindowSetHeight(int Height);

        [DispId(0x107)]
        void WindowClosing([MarshalAs(UnmanagedType.VariantBool)] bool IsChildWindow, [In, Out, MarshalAs(UnmanagedType.VariantBool)] ref bool Cancel);

        [DispId(0x10c)]
        void ClientToHostWindow([In, Out] ref int CX, [In, Out] ref int CY);

        [DispId(0x10d)]
        void SetSecureLockIcon(int SecureLockIcon);

        [DispId(270)]
        void FileDownload([In, Out, MarshalAs(UnmanagedType.VariantBool)] ref bool Cancel);

        [DispId(0x10f)]
        void NavigateError([MarshalAs(UnmanagedType.IDispatch)] object pDisp, [In] ref object URL, [In] ref object Frame, [In] ref object StatusCode, [In, Out, MarshalAs(UnmanagedType.VariantBool)] ref bool Cancel);

        [DispId(0xe1)]
        void PrintTemplateInstantiation([MarshalAs(UnmanagedType.IDispatch)] object pDisp);

        [DispId(0xe2)]
        void PrintTemplateTeardown([MarshalAs(UnmanagedType.IDispatch)] object pDisp);

        [DispId(0xe3)]
        void UpdatePageStatus([MarshalAs(UnmanagedType.IDispatch)] object pDisp, [In] ref object nPage, [In] ref object fDone);

        [DispId(0x110)]
        void PrivacyImpactedStateChange([MarshalAs(UnmanagedType.VariantBool)] bool bImpacted);

        [DispId(0x111)]
        void NewWindow3([In, Out, MarshalAs(UnmanagedType.IDispatch)] ref object ppDisp, [In, Out, MarshalAs(UnmanagedType.VariantBool)] ref bool Cancel, uint dwFlags, [MarshalAs(UnmanagedType.BStr)] string bstrUrlContext, [MarshalAs(UnmanagedType.BStr)] string bstrUrl);
    }
}