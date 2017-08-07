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
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Server;

#endregion

public partial class UserDefinedFunctions
{
    private static readonly Regex _invalidPathCharacters = new Regex(@"[^a-zA-Z0-9 ]", RegexOptions.Compiled);

    /// <summary>
    /// 	Extracts a Windows safe Directory path from an AbsoluteUri.  e.g. http://arachnode.net/sitefiles/1000/logo.gif -> http/arachnode/net/sitefiles/1000/
    /// 	The maximum length for any directory returned is 200.
    /// </summary>
    /// <param name = "absoluteUri"></param>
    /// <param name = "baseDirectory"></param>
    /// <returns></returns>
    [SqlFunction(DataAccess = DataAccessKind.None)]
    public static SqlString ExtractDirectory(SqlString baseDirectory, SqlString absoluteUri)
    {
        if (string.IsNullOrEmpty(baseDirectory.Value) || string.IsNullOrEmpty(absoluteUri.Value))
        {
            return null;
        }
        else
        {
            string directory = absoluteUri.Value;

            if (directory.Length > 200)
            {
                directory = directory.Substring(0, 200);
            }

            directory = Path.Combine(baseDirectory.Value, _invalidPathCharacters.Replace(Path.GetDirectoryName(directory), "\\"));

            if (directory.Length > 200)
            {
                return directory.Substring(0, 200);
            }

            return directory;
        }
    }
} ;