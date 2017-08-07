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
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Server;

#endregion

public partial class UserDefinedFunctions
{
    private static readonly Regex _greaterThanWithoutSpace = new Regex(">", RegexOptions.Compiled);
    private static readonly Regex _greaterThanWithSpace = new Regex("> ", RegexOptions.Compiled);
    private static readonly Regex _lessThanWithoutSpace = new Regex("<", RegexOptions.Compiled);
    private static readonly Regex _lessThanWithSpace = new Regex("< ", RegexOptions.Compiled);
    private static readonly Regex _removeComments = new Regex("<!--.*?-->", RegexOptions.Compiled | RegexOptions.Singleline);
    private static readonly Regex _removeScripts = new Regex("<[ s]cript.*?scrip[t ]>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
    private static readonly Regex _removeSelected = new Regex("<[ s]elect.*?selec[t ]>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
    private static readonly Regex _removeStyles = new Regex("<[s ]tyle.*?styl[e ]>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

    private static readonly Regex _removeTabsCarraigeReturnsAndNewlines = new Regex("[\t\r\n]", RegexOptions.Compiled);
    private static readonly Regex _removeTags = new Regex("<.*?>", RegexOptions.Compiled | RegexOptions.Singleline);
    private static readonly Regex _removeWhiteSpace = new Regex(@"\s{2,}", RegexOptions.Compiled);

    /// <summary>
    /// 	Extracts the text.
    /// </summary>
    /// <param name = "source">The source.</param>
    /// <returns></returns>
    [SqlFunction]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString ExtractText([SqlFacet(MaxSize = -1)] SqlString source)
    {
        if (!source.IsNull)
        {
            string text = _removeScripts.Replace(source.Value, string.Empty);
            text = _removeSelected.Replace(text, string.Empty);
            text = _removeStyles.Replace(text, string.Empty);
            text = _removeComments.Replace(text, string.Empty);

            text = _lessThanWithoutSpace.Replace(text, "< ");
            text = _greaterThanWithoutSpace.Replace(text, "> ");
            text = _removeTags.Replace(text, string.Empty);

            text = _lessThanWithSpace.Replace(text, "<");
            text = _greaterThanWithSpace.Replace(text, ">");
            text = _removeTags.Replace(text, string.Empty);

            text = _removeTabsCarraigeReturnsAndNewlines.Replace(text, string.Empty);

            text = _removeWhiteSpace.Replace(text, " ");

            if(!string.IsNullOrEmpty(text))
            {
                text = text.Trim();
            }

            return text;
        }

        return source;
    }
} ;