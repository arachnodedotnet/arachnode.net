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
using System.Text;

#endregion

namespace Arachnode.Security
{
    public static class Obfuscation
    {
        private static SortedList<string, string> _characterTranslations;

        /// <summary>
        /// 	Obfuscates the specified to be obfuscated.
        /// </summary>
        /// <param name = "toBeObfuscated">To be obfuscated.</param>
        /// <returns></returns>
        public static string Obfuscate(string toBeObfuscated)
        {
            if (_characterTranslations == null)
            {
                _characterTranslations = new SortedList<string, string>();

                _characterTranslations.Add("0", "4");
                _characterTranslations.Add("1", "3");
                _characterTranslations.Add("2", "6");
                _characterTranslations.Add("3", "2");
                _characterTranslations.Add("4", "8");
                _characterTranslations.Add("5", "1");
                _characterTranslations.Add("6", "0");
                _characterTranslations.Add("7", "7");
                _characterTranslations.Add("8", "9");
                _characterTranslations.Add("9", "5");
            }

            if (toBeObfuscated == null)
            {
                return null;
            }
            else
            {
                StringBuilder stringBuilder = new StringBuilder();

                for (int i = toBeObfuscated.Length - 1; i >= 0; i--)
                {
                    stringBuilder.Append(_characterTranslations[toBeObfuscated[i].ToString()]);
                }

                return stringBuilder.ToString();
            }
        }

        /// <summary>
        /// 	Clarifies the specified to be clarified.
        /// </summary>
        /// <param name = "toBeClarified">To be clarified.</param>
        /// <returns></returns>
        public static string Clarify(string toBeClarified)
        {
            if (toBeClarified == null)
            {
                return null;
            }
            else
            {
                StringBuilder stringBuilder = new StringBuilder();

                for (int i = toBeClarified.Length - 1; i >= 0; i--)
                {
                    stringBuilder.Append(_characterTranslations.Keys[_characterTranslations.IndexOfValue(toBeClarified[i].ToString())]);
                }

                return stringBuilder.ToString();
            }
        }
    }
}