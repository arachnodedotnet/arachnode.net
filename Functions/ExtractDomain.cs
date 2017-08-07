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
using Microsoft.SqlServer.Server;

#endregion

public partial class UserDefinedFunctions
{
    /// <summary>
    /// 	Extracts the Domain from an AbsoluteUri.  e.g. http://subdomain.arachnode.net/ -&gt; arachnode.net
    /// </summary>
    /// <param name = "absoluteUri">The absolute URI.</param>
    /// <returns></returns>
    [SqlFunction(DataAccess = DataAccessKind.Read)]
    public static SqlString ExtractDomain(SqlString absoluteUri)
    {
        if (!ExtractIPAddress(absoluteUri).IsNull)
        {
            return ExtractIPAddress(absoluteUri);
        }
        else
        {
            SqlString host = ExtractHost(absoluteUri);

            if (host.IsNull)
            {
                return "UNKNOWN";
            }
            else
            {
                string[] split = host.Value.Split('.');

                //HACK: Concession for http://localhost and http://sub.localhost ...
                if (split[split.Length - 1].ToLowerInvariant() == "localhost")
                {
                    return new SqlString(split[split.Length - 1]);
                }

                if (split.Length < 3)
                {
                    return host;
                }
                else
                {
                    if (AllowedExtensions.ContainsKey(split[split.Length - 2] + '.' + split[split.Length - 1]) || split[split.Length - 2].Length == 2)
                    {
                        return new SqlString(split[split.Length - 3] + '.' + split[split.Length - 2] + '.' + split[split.Length - 1]);
                    }
                    else
                    {
                        return new SqlString(split[split.Length - 2] + '.' + split[split.Length - 1]);
                    }
                }
            }
        }
    }
} ;