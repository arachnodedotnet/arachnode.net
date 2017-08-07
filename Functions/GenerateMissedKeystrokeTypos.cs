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
    /// 	Generates the missed keystroke typos.
    /// </summary>
    /// <param name = "term">The term.</param>
    /// <param name = "maximumNumberOfMissedKeystrokes">The maximum number of missed keystrokes.</param>
    /// <returns></returns>
    [SqlFunction]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString GenerateMissedKeystrokeTypos(string term, int maximumNumberOfMissedKeystrokes)
    {
        List<string> typographicalErrors = new List<string>();

        for (int i = 0; i < term.Length; i++)
        {
            for (int j = 1; j <= term.Length; j++)
            {
                if (i + j <= term.Length)
                {
                    string typographicalError = term.Substring(i, j);

                    if ((term == typographicalError || typographicalError.Length >= term.Length - maximumNumberOfMissedKeystrokes) && !typographicalErrors.Contains(typographicalError))
                    {
                        if (term != typographicalError)
                        {
                            typographicalErrors.Add(typographicalError);
                        }

                        if (typographicalError.Length > term.Length - maximumNumberOfMissedKeystrokes)
                        {
                            for (int k = 1; k < typographicalError.Length; k++)
                            {
                                string typographicalError2 = typographicalError.Remove(k, 1);

                                if (!typographicalErrors.Contains(typographicalError2))
                                {
                                    typographicalErrors.Add(typographicalError2);
                                }
                            }
                        }
                    }
                }
            }
        }

        return ConvertTypographicalErrorsToString(typographicalErrors);
    }
} ;