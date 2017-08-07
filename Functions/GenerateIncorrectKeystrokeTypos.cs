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
using System.Text;
using Microsoft.SqlServer.Server;

#endregion

public partial class UserDefinedFunctions
{
    private static Dictionary<char, List<char>> _charactersNear;

    /// <summary>
    /// 	Instantiates the characters near.
    /// </summary>
    private static void InstantiateCharactersNear()
    {
        _charactersNear = new Dictionary<char, List<char>>();

        _charactersNear.Add('1', new List<char>("2q".ToCharArray()));
        _charactersNear.Add('2', new List<char>("1qw3".ToCharArray()));
        _charactersNear.Add('3', new List<char>("2we4".ToCharArray()));
        _charactersNear.Add('4', new List<char>("3er5".ToCharArray()));
        _charactersNear.Add('5', new List<char>("4rt6".ToCharArray()));
        _charactersNear.Add('6', new List<char>("5ty7".ToCharArray()));
        _charactersNear.Add('7', new List<char>("6yu8".ToCharArray()));
        _charactersNear.Add('8', new List<char>("7ui9".ToCharArray()));
        _charactersNear.Add('9', new List<char>("8io0".ToCharArray()));
        _charactersNear.Add('0', new List<char>("9op-".ToCharArray()));
        _charactersNear.Add('-', new List<char>("0p".ToCharArray()));

        _charactersNear.Add('q', new List<char>("12wa".ToCharArray()));
        _charactersNear.Add('w', new List<char>("qase32".ToCharArray()));
        _charactersNear.Add('e', new List<char>("wsdr43".ToCharArray()));
        _charactersNear.Add('r', new List<char>("edft54".ToCharArray()));
        _charactersNear.Add('t', new List<char>("rfgy65".ToCharArray()));
        _charactersNear.Add('y', new List<char>("tghu76".ToCharArray()));
        _charactersNear.Add('u', new List<char>("yhji87".ToCharArray()));
        _charactersNear.Add('i', new List<char>("ujko98".ToCharArray()));
        _charactersNear.Add('o', new List<char>("iklp09".ToCharArray()));
        _charactersNear.Add('p', new List<char>("ol-0".ToCharArray()));

        _charactersNear.Add('a', new List<char>("zswq".ToCharArray()));
        _charactersNear.Add('s', new List<char>("azxdew".ToCharArray()));
        _charactersNear.Add('d', new List<char>("sxcfre".ToCharArray()));
        _charactersNear.Add('f', new List<char>("dcvgtr".ToCharArray()));
        _charactersNear.Add('g', new List<char>("fvbhyt".ToCharArray()));
        _charactersNear.Add('h', new List<char>("gbnjyt".ToCharArray()));
        _charactersNear.Add('j', new List<char>("hnmkiu".ToCharArray()));
        _charactersNear.Add('k', new List<char>("jmloi".ToCharArray()));
        _charactersNear.Add('l', new List<char>("kpo".ToCharArray()));

        _charactersNear.Add('z', new List<char>("xsa".ToCharArray()));
        _charactersNear.Add('x', new List<char>("zcds".ToCharArray()));
        _charactersNear.Add('c', new List<char>("xvfd".ToCharArray()));
        _charactersNear.Add('v', new List<char>("cbgf".ToCharArray()));
        _charactersNear.Add('b', new List<char>("vnhg".ToCharArray()));
        _charactersNear.Add('n', new List<char>("bmjh".ToCharArray()));
        _charactersNear.Add('m', new List<char>("nkj".ToCharArray()));
    }

    /// <summary>
    /// 	Generates the incorrect keystroke typos.
    /// </summary>
    /// <param name = "term">The term.</param>
    /// <returns></returns>
    [SqlFunction]
    [return: SqlFacet(MaxSize = -1)]
    public static SqlString GenerateIncorrectKeystrokeTypos(string term)
    {
        if (_charactersNear == null)
        {
            InstantiateCharactersNear();
        }

        List<string> typographicalErrors = new List<string>();

        term = term.ToLower();

        for (int i = 0; i < term.Length; i++)
        {
            if (_charactersNear.ContainsKey(term[i]))
            {
                char[] typographicalError = term.ToCharArray();

                foreach (char character in _charactersNear[term[i]])
                {
                    typographicalError[i] = character;
                    typographicalErrors.Add(new string(typographicalError));
                }
            }
        }

        return ConvertTypographicalErrorsToString(typographicalErrors);
    }

    /// <summary>
    /// 	Converts the typographical errors to string.
    /// </summary>
    /// <param name = "typographicalErrors">The typographical errors.</param>
    /// <returns></returns>
    private static string ConvertTypographicalErrorsToString(IEnumerable<string> typographicalErrors)
    {
        StringBuilder stringBuilder = new StringBuilder();

        foreach (string typographicalError in typographicalErrors)
        {
            stringBuilder.Append(typographicalError + " ");
        }

        string returnString = stringBuilder.ToString().TrimEnd(' ');

        if (returnString.Length > 4000)
        {
            returnString = returnString.Substring(0, 3997) + "...";
        }

        return returnString;
    }
} ;