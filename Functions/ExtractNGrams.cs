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
using System;

#endregion

public partial class UserDefinedFunctions
{
    /// <summary>
    /// 	Extracts the NGrams.
    /// </summary>
    /// <param name = "source">The source.</param>
    /// <param name = "nGramDepth">The NGram depth.</param>
    /// <param name = "returnDistinctWords">The return distinct words.</param>
    /// <returns></returns>
    [SqlFunction(FillRowMethodName = "FillRow", TableDefinition = "NGram NVARCHAR(4000)")]
    public static IEnumerable ExtractNGrams([SqlFacet(MaxSize = -1)] SqlString source, SqlInt32 nGramDepth, SqlBoolean returnDistinctNGrams)
    {
        if (!source.IsNull)
        {
            List<string> nGrams = new List<string>();

            string processedPostText = source.Value;// ProcessPostText(text);

            string[] processedPostTextSplit = processedPostText.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            string nGram = string.Empty;

            for (int i = 0; i < processedPostTextSplit.Length; i++)
            {
                nGram += processedPostTextSplit[i];

                if (i + nGramDepth <= processedPostTextSplit.Length)
                {
                    for (int j = 1; j < nGramDepth && i + j < processedPostTextSplit.Length; j++)
                    {
                        nGram += " " + processedPostTextSplit[i + j];
                    }

                    if (returnDistinctNGrams.IsTrue)
                    {
                        if (!nGrams.Contains(nGram.Trim()))
                        {
                            nGrams.Add(nGram.Trim());
                        }
                    }
                    else
                    {
                        nGrams.Add(nGram.Trim());
                    }

                    nGram = string.Empty;
                }
            }

            return nGrams;
        }

        return null;
    }
} ;