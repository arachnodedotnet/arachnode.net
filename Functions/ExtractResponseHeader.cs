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
    /// 	Extracts the specified ResponseHeader from the ResponseHeaders.  e.g. 'Cache-Control: no-cache  Content-Type: text/html; charset=iso-8859-1' -&gt; text/html or 'Cache-Control: no-cache  Content-Type: text/html; charset=iso-8859-1' -&gt; text/html; charset=iso-8859-1
    /// </summary>
    /// <param name = "responseHeaders">The ResponseHeaders to parse.</param>
    /// <param name = "responseHeader">The ResponseHeader value to extract.</param>
    /// <param name = "includeParameters">Should parameters occuring after semicolons be included?</param>
    /// <returns>A ResponseHeader value.</returns>
    [SqlFunction(DataAccess = DataAccessKind.None)]
    public static SqlString ExtractResponseHeader(SqlString responseHeaders, SqlString responseHeader, SqlBoolean includeParameters)
    {
        if (responseHeaders.IsNull || responseHeader.IsNull)
        {
            return null;
        }
        else
        {
            int startIndex = responseHeaders.Value.ToLower().IndexOf(responseHeader.Value.ToLower()) + responseHeader.Value.Length;

            if (startIndex == responseHeader.Value.Length - 1)
            {
                return null;
            }
            else
            {
                int endIndex = responseHeaders.Value.IndexOf("\r\n", startIndex);

                try
                {
                    if (includeParameters.IsTrue)
                    {
                        return new SqlString(responseHeaders.Value.Substring(startIndex, endIndex - startIndex).Trim().TrimStart(": ".ToCharArray()));
                    }
                    else
                    {
                        return new SqlString(responseHeaders.Value.Substring(startIndex, endIndex - startIndex).Split(';')[0].Trim().TrimStart(": ".ToCharArray()));
                    }
                }
                catch
                {
                    return null;
                }
            }
        }
    }
} ;