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

using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Server;

#endregion

public partial class UserDefinedFunctions
{
    /// <summary>
    /// 	Extracts the phrases.
    /// </summary>
    /// <param name = "source">The source.</param>
    /// <param name = "extractText">The extract text.</param>
    /// <param name = "returnDistinctPhrases">The return distinct phrases.</param>
    /// <returns></returns>
    [SqlFunction(FillRowMethodName = "FillRow", TableDefinition = "Phrase NVARCHAR(4000)")]
    public static IEnumerable ExtractPhrases([SqlFacet(MaxSize = -1)] SqlString source, SqlBoolean extractText, SqlBoolean returnDistinctPhrases)
    {
        if (!source.IsNull)
        {
            if (extractText.IsTrue)
            {
                source = ExtractText(source);
            }

            List<string> phrases = new List<string>();

            foreach (string phrase in Regex.Split(source.Value, @"(?:\.|!|\?)+(?:\s+|\z)"))
            {
                if (phrase.Trim() != string.Empty)
                {
                    if (returnDistinctPhrases.IsTrue)
                    {
                        if (!phrases.Contains(phrase.Trim()))
                        {
                            phrases.Add(phrase.Trim());
                        }
                    }
                    else
                    {
                        phrases.Add(phrase.Trim());
                    }
                }
            }

            return phrases;
        }

        return null;
    }

    public static void FillRow(object row, out SqlString sqlString)
    {
        sqlString = new SqlString((string) row);
    }
} ;