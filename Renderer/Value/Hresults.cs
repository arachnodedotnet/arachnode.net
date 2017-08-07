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

namespace Arachnode.Renderer.Value
{
    /// <summary>
    /// HRESULT constants
    /// </summary>
    public sealed class Hresults
    {
        public const int DV_E_LINDEX = unchecked((int) 0x80040068);
        public const int E_ABORT = unchecked((int) 0x80004004);
        public const int E_ACCESSDENIED = unchecked((int) 0x80070005);
        public const int E_FAIL = unchecked((int) 0x80004005);
        public const int E_FLAGS = unchecked(0x1000);
        public const int E_HANDLE = unchecked((int) 0x80070006);
        public const int E_INVALIDARG = unchecked((int) 0x80070057);
        public const int E_NOINTERFACE = unchecked((int) 0x80004002);
        public const int E_NOTIMPL = unchecked((int) 0x80004001);
        public const int E_OUTOFMEMORY = unchecked((int) 0x8007000E);
        public const int E_PENDING = unchecked((int) 0x8000000A);
        public const int E_POINTER = unchecked((int) 0x80004003);
        public const int E_UNEXPECTED = unchecked((int) 0x8000FFFF);

        //Wininet
        public const int ERROR_ACCESS_DENIED = 5;
        public const int ERROR_FILE_NOT_FOUND = 2;
        public const int ERROR_INSUFFICIENT_BUFFER = 122;
        public const int ERROR_NO_MORE_ITEMS = 259;
        public const int ERROR_SUCCESS = 0;
        public const int NOERROR = 0;
        public const int OLE_E_ADVF = unchecked((int) 0x80040001);
        public const int OLE_E_ADVISENOTSUPPORTED = unchecked((int) 0x80040003);
        public const int OLE_E_BLANK = unchecked((int) 0x80040007);
        public const int OLE_E_CANT_BINDTOSOURCE = unchecked((int) 0x8004000A);
        public const int OLE_E_CANT_GETMONIKER = unchecked((int) 0x80040009);
        public const int OLE_E_CANTCONVERT = unchecked((int) 0x80040011);
        public const int OLE_E_CLASSDIFF = unchecked((int) 0x80040008);
        public const int OLE_E_ENUM_NOMORE = unchecked((int) 0x80040002);

        //Ole Errors
        public const int OLE_E_FIRST = unchecked((int) 0x80040000);
        public const int OLE_E_INVALIDHWND = unchecked((int) 0x8004000F);
        public const int OLE_E_INVALIDRECT = unchecked((int) 0x8004000D);
        public const int OLE_E_LAST = unchecked((int) 0x800400FF);
        public const int OLE_E_NOCACHE = unchecked((int) 0x80040006);
        public const int OLE_E_NOCONNECTION = unchecked((int) 0x80040004);
        public const int OLE_E_NOSTORAGE = unchecked((int) 0x80040012);
        public const int OLE_E_NOT_INPLACEACTIVE = unchecked((int) 0x80040010);
        public const int OLE_E_NOTRUNNING = unchecked((int) 0x80040005);
        public const int OLE_E_OLEVERB = unchecked((int) 0x80040000);
        public const int OLE_E_PROMPTSAVECANCELLED = unchecked((int) 0x8004000C);
        public const int OLE_E_STATIC = unchecked((int) 0x8004000B);
        public const int OLE_E_WRONGCOMPOBJ = unchecked((int) 0x8004000E);
        public const int OLE_S_FIRST = unchecked(0x00040000);
        public const int OLE_S_LAST = unchecked(0x000400FF);
        public const int OLECMDERR_E_CANCELED = unchecked((OLECMDERR_E_FIRST + 3));
        //OLECMDERR_E_FIRST = 0x80040100
        public const int OLECMDERR_E_DISABLED = unchecked((OLECMDERR_E_FIRST + 1));
        public const int OLECMDERR_E_FIRST = unchecked((OLE_E_LAST + 1));
        public const int OLECMDERR_E_NOHELP = unchecked((OLECMDERR_E_FIRST + 2));
        public const int OLECMDERR_E_NOTSUPPORTED = unchecked((OLECMDERR_E_FIRST));
        public const int OLECMDERR_E_UNKNOWNGROUP = unchecked((OLECMDERR_E_FIRST + 4));
        public const int OLECMDID_SHOWSCRIPTERROR = 40;

        public const int OLEOBJ_E_NOVERBS = unchecked((int) 0x80040180);
        public const int OLEOBJ_S_CANNOT_DOVERB_NOW = unchecked(0x00040181);
        public const int OLEOBJ_S_INVALIDHWND = unchecked(0x00040182);
        public const int OLEOBJ_S_INVALIDVERB = unchecked(0x00040180);

        public const int RPC_E_RETRY = unchecked((int) 0x80010109);
        public const int S_FALSE = 1;
        public const int S_OK = 0;
        public const int VARIANT_FALSE = 0;
        public const int VARIANT_TRUE = -1;
    }
}