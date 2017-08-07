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

using System.Data.SqlTypes;
using System.Net;
using Microsoft.SqlServer.Server;

#endregion

public partial class UserDefinedFunctions
{
    /// <summary>
    /// 	Extracts the IP Address from an AbsoluteUri.  e.g. http://192.168.1.1/logo.gif -&gt; 192.168.1.1
    /// </summary>
    /// <param name = "absoluteUri">The absolute URI.</param>
    /// <returns></returns>
    [SqlFunction(DataAccess = DataAccessKind.None)]
    public static SqlString ExtractIPAddress(SqlString absoluteUri)
    {
        SqlString host = ExtractHost(absoluteUri);

        if (host.IsNull)
        {
            return null;
        }
        else
        {
            IPAddress ipAddress;

            if (!IPAddress.TryParse(host.Value, out ipAddress))
            {
                return null;
            }
            else
            {
                return new SqlString(ipAddress.ToString());
            }
        }
    }
} ;