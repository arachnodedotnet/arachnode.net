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
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

#endregion

public partial class UserDefinedFunctions
{
    /// <summary>
    /// 	Extracts the Host from an AbsoluteUri.  http://subdomain.arachnode.net/ -&gt; subdomain.arachnode.net
    /// </summary>
    /// <param name = "absoluteUri">The absolute URI.</param>
    /// <returns></returns>
    [SqlFunction(DataAccess = DataAccessKind.None)]
    public static SqlString ExtractHost(SqlString absoluteUri)
    {
        if (!absoluteUri.IsNull)
        {
            Uri host;

            if (Uri.TryCreate(absoluteUri.Value, UriKind.Absolute, out host))
            {
                if (string.IsNullOrEmpty(host.Host))
                {
                    if (absoluteUri.Value.Contains("@"))
                    {
                        return new SqlString(absoluteUri.Value.Split('@')[1]);
                    }
                    else
                    {
                        return "UNKNOWN";
                    }
                }
                else
                {
                    if (host.Host.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
                    {
                        return new SqlString(host.Host.Substring(4, host.Host.Length - 4));
                    }
                    else
                    {
                        return new SqlString(host.Host);
                    }
                }
            }
            else
            {
                return "UNKNOWN";
            }
        }
        else
        {
            return "UNKNOWN";
        }
    }
} ;