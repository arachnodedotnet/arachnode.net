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

using System.Collections.Generic;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

#endregion

public partial class UserDefinedFunctions
{
    /// <summary>
    /// 	Generates the transposed keystroke typos.
    /// </summary>
    /// <param name = "term">The term.</param>
    /// <param name = "maximumNumberOfTransposedKeystrokes">The maximum number of transposed keystrokes.</param>
    /// <returns></returns>
    [SqlFunction]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString GenerateTransposedKeystrokeTypos(string term, int maximumNumberOfTransposedKeystrokes)
    {
        List<string> typographicalErrors = new List<string>();

        typographicalErrors.Add(term.ToLower());

        for (int i = 1; i <= maximumNumberOfTransposedKeystrokes; i++)
        {
            int typographicalErrorsCount = typographicalErrors.Count;

            for (int j = 0; j < typographicalErrorsCount; j++)
            {
                for (int k = 1; k < typographicalErrors[j].Length; k++)
                {
                    char[] typographicalError = typographicalErrors[j].ToCharArray();
                    char temporaryCharacter = typographicalError[k - 1];

                    typographicalError[k - 1] = typographicalError[k];

                    typographicalError[k] = temporaryCharacter;

                    if (!typographicalErrors.Contains(new string(typographicalError)))
                    {
                        typographicalErrors.Add(new string(typographicalError));
                    }
                }
            }
        }

        typographicalErrors.Remove(term.ToLower());

        return ConvertTypographicalErrorsToString(typographicalErrors);
    }
} ;