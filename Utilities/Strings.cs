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

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Arachnode.Utilities
{
    public class Strings
    {
        public static string ToProperCase(string input)
        {
            if(string.IsNullOrEmpty(input))
            {
                return null;
            }

            StringBuilder @return = new StringBuilder();

            bool nextCharToUpper = true;

            foreach(char c in input)
            {
                if(c == ' ')
                {
                    nextCharToUpper = true;
                }

                if(nextCharToUpper)
                {
                    @return.Append(c.ToString().ToUpperInvariant());

                    nextCharToUpper = false;
                }
                else
                {
                    @return.Append(c.ToString().ToLowerInvariant());
                }
            }

            return @return.ToString();
        }

        public static HashSet<string> ExtractDistinctStrings(IEnumerable<string> extractFrom, IEnumerable<string> stringsToExclude, bool convertToLowerInVariant, bool extractAlphaNumericCharacters, bool removeSpaces, bool removeDoubleSpaces, int minimumDistinctStringLength, bool includeEmptyString, bool includeNull, bool removeNumericOnlyStrings)
        {
            if (extractFrom == null)
            {
                return null;
            }

            int tryParse;
            HashSet<string> distinctStrings = new HashSet<string>();

            foreach (string @string in extractFrom)
            {
                string string2 = @string.Replace("�", string.Empty);

                if (!string.IsNullOrEmpty(string2))
                {
                    string2 = string2.Trim();

                    if (convertToLowerInVariant)
                    {
                        string2 = string2.ToLowerInvariant();
                    }

                    if (extractAlphaNumericCharacters)
                    {
                        string2 = UserDefinedFunctions.ExtractAlphaNumericCharacters(string2).Value;
                    }

                    if(removeDoubleSpaces)
                    {
                        while(string2.IndexOf("  ") != -1)
                        {
                            string2 = string2.Replace("  ", " ");
                        }
                    }

                    if(string2.Length < minimumDistinctStringLength)
                    {
                        continue;
                    }
                }

                if (!includeEmptyString)
                {
                    if (string2 == string.Empty)
                    {
                        continue;
                    }
                }

                if (!includeNull)
                {
                    if (string2 == null)
                    {
                        continue;
                    }
                }

                if (removeNumericOnlyStrings)
                {
                    if (int.TryParse(string2, out tryParse))
                    {
                        continue;
                    }
                }

                if (!distinctStrings.Contains(string2))
                {
                    if (stringsToExclude == null || stringsToExclude.Count() == 0)
                    {
                        distinctStrings.Add(string2);
                    }
                    else
                    {
                        if (!stringsToExclude.Contains(string2))
                        {
                            distinctStrings.Add(string2);
                        }
                    }
                }
            }

            return distinctStrings;
        }

        public static List<Uri> ExtractAbsoluteUris(string toParse)
        {
            List<Uri> uris = new List<Uri>();

            try
            {
                foreach (string line in toParse.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    Uri uri = null;

                    if (Uri.TryCreate(line, UriKind.Absolute, out uri))
                    {
                        uris.Add(uri);
                    }
                    else
                    {
                        if (Uri.TryCreate("http://" + line, UriKind.Absolute, out uri))
                        {
                            uris.Add(uri);
                        }
                    }
                }
            }
            catch
            {

            }

            return uris;
        }

        public static bool IsValidUSPhoneNumber(string input, bool verifyArithmetically)
        {
            if(string.IsNullOrEmpty(input))
            {
                return false;
            }

            try
            {
                input = input.ToLowerInvariant();

                if (input.Contains("x"))
                {
                    input = input.Split('x')[0];
                }

                if (input.Contains("e"))
                {
                    input = input.Split('e')[0];
                }

                string alphaNumericCharacters = UserDefinedFunctions.ExtractAlphaNumericCharacters(input).Value;

                if (alphaNumericCharacters.Length > 11 || alphaNumericCharacters.Length <= 9)
                {
                    return false;
                }

                if (alphaNumericCharacters.Length == 11)
                {
                    if (alphaNumericCharacters[0] != '1')
                    {
                        return false;
                    }

                    alphaNumericCharacters = alphaNumericCharacters.Substring(1, 10);
                }

                foreach (char c in alphaNumericCharacters)
                {
                    if (!char.IsDigit(c))
                    {
                        return false;
                    }
                }

                if (!verifyArithmetically)
                {
                    return true;
                }

                int areaCode = int.Parse(alphaNumericCharacters.Substring(0, 3));
                int firstThree = int.Parse(alphaNumericCharacters.Substring(3, 3));
                int lastFour = int.Parse(alphaNumericCharacters.Substring(6, 4));

                double phoneNumber = firstThree*80;
                phoneNumber += 1;
                phoneNumber *= 250;
                phoneNumber += lastFour;
                phoneNumber += lastFour;
                phoneNumber -= 250;
                phoneNumber /= 2;

                if (firstThree.ToString() + lastFour.ToString() != ((int) phoneNumber).ToString())
                {
                    return false;
                }
                //                Multiply by 80
                //Add 1
                //Multiply by 250
                //Add the last 4 digits of your phone number
                //Add the last 4 digits of your number again
                //Subtract 250
                //Divide by 2
                return true;
            }
            catch
            {
            }

            return false;
        }

        public static bool IsValidInternationalPhoneNumber(string input)
        {
            string pattern1 = @"^\+(?:[0-9] ?){6,14}[0-9]$";
            string pattern2 = @"^\+[0-9]{1,3}\.[0-9]{4,14}(?:x.+)?$";

            bool isMatch1 = Regex.IsMatch(input, pattern1);
            bool isMatch2 = Regex.IsMatch(input, pattern2);

            return isMatch1 || isMatch2;
        }

        public static ulong GenerateFuzzyHashCode(string input)
        {
            if (string.IsNullOrEmpty(input))
                return 0;

            input = input.ToLowerInvariant();
            StringBuilder stringBuilder = new StringBuilder(64);

            int vowelCount = 0;

            for (char c = '1'; c <= '9'; c++)
            {
                if (input.Contains(c))
                {
                    stringBuilder.Append("1");
                }
                else
                {
                    stringBuilder.Append("0");
                }
            }

            for (char c = 'a'; c <= 'z'; c++)
            {
                int replacedLength = input.Replace(c.ToString(), string.Empty).Length;
                if (input.Length - replacedLength == 0)
                {
                    stringBuilder.Append("00");
                }
                else if (input.Length - replacedLength == 1)
                {
                    stringBuilder.Append("01");
                }
                else if (input.Length - replacedLength == 2)
                {
                    stringBuilder.Append("10");
                }
                else if (input.Length - replacedLength == 3)
                {
                    stringBuilder.Append("11");
                }
                if (c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u' || c == 'y')
                {
                    vowelCount++;
                }
            }

            if (vowelCount > input.Length - vowelCount)
                stringBuilder.Append("1");
            else
                stringBuilder.Append("0");

            char firstCharacter = input.First();

            if (firstCharacter == 'a' || firstCharacter == 'e' || firstCharacter == 'i' || firstCharacter == 'o' || firstCharacter == 'u' || firstCharacter == 'y')
                stringBuilder.Append("1");
            else
                stringBuilder.Append("0");

            char lastCharacter = input.Last();

            if (lastCharacter == 'a' || lastCharacter == 'e' || lastCharacter == 'i' || lastCharacter == 'o' || lastCharacter == 'u' || lastCharacter == 'y')
                stringBuilder.Append("1");
            else
                stringBuilder.Append("0");

            while (stringBuilder.Length < 64)
            {
                stringBuilder.Append("0");
            }

            if (stringBuilder.Length > 64)
            {
                stringBuilder = new StringBuilder(stringBuilder.ToString().Substring(0, 64));
            }

            return Convert.ToUInt64(stringBuilder.ToString(), 2);
        }

        public static Dictionary<string, string> GenerateThesaurus(List<string> preseededThesaurusWords, List<string> wordsToRemove)
        {
            Dictionary<string, string> synonyms = new Dictionary<string, string>();

            if (preseededThesaurusWords != null)
            {
                foreach (string preseededThesaurusWord in preseededThesaurusWords.Where(_ => !string.IsNullOrEmpty(_)))
                {
                    if (!synonyms.ContainsKey(preseededThesaurusWord))
                        synonyms.Add(preseededThesaurusWord, preseededThesaurusWord);
                }
            }

            Action<string> process = new Action<string>(delegate(string line)
                                                            {
                                                                try
                                                                {
                                                                    string[] lineSplit = line.ToLowerInvariant().Split("|".ToCharArray());

                                                                    string words = lineSplit[0];

                                                                    string[] wordsSplit = words.Split(",".ToCharArray());

                                                                    if (wordsSplit.Length >= 2)
                                                                    {
                                                                        string commonWord = wordsSplit[0];

                                                                        if (commonWord.Contains(" "))
                                                                            return;

                                                                        if (wordsToRemove != null && wordsToRemove.Contains(commonWord))
                                                                            return;

                                                                        if (!synonyms.ContainsKey(commonWord))
                                                                            synonyms.Add(commonWord, commonWord);

                                                                        for (int i = 1; i < wordsSplit.Length; i++)
                                                                        {
                                                                            string synonym = wordsSplit[i];

                                                                            if (wordsToRemove != null && wordsToRemove.Contains(synonym))
                                                                                continue;

                                                                            if (!synonyms.ContainsKey(synonym))
                                                                                synonyms.Add(synonym, commonWord);
                                                                        }
                                                                    }
                                                                }
                                                                catch
                                                                {    
                                                                }
                                                            });

            foreach(string line in File.ReadAllLines("TextDatabases\\wn_database.lst").OrderBy(_ => _))
            {
                process(line);
            }

            foreach (string line in File.ReadAllLines("TextDatabases\\mobythes.aur").OrderBy(_ => _))
            {
                process(line);
            }

            //synonyms = synonyms.OrderBy(_ => _.Value).ToDictionary(_ => _.Key, _ => _.Value);

            return synonyms;
        }

        public static string ExtractInnerText(string input, string startString, string endString)
        {
            string inputToLowerInvariant = input.ToLowerInvariant();
            startString = startString.ToLowerInvariant();
            int rawIndex = inputToLowerInvariant.IndexOf(startString.Replace("|", ""));

            if (rawIndex == -1)
            {
                rawIndex = inputToLowerInvariant.IndexOf(startString.Replace("|", "'"));
            }

            if (rawIndex == -1)
            {
                rawIndex = inputToLowerInvariant.IndexOf(startString.Replace("|", "\""));
            }

            if (rawIndex != -1)
            {
                int rawIndexEnd = inputToLowerInvariant.IndexOf(endString.Replace("|", ""), rawIndex);

                if (rawIndexEnd == -1)
                {
                    rawIndexEnd = inputToLowerInvariant.IndexOf(endString.Replace("|", "'"));
                }

                if (rawIndexEnd == -1)
                {
                    rawIndexEnd = inputToLowerInvariant.IndexOf(endString.Replace("|", "\""));
                }

                if (rawIndexEnd != -1)
                {
                    if (rawIndexEnd - rawIndex >= 1)
                    {
                        string raw = input.Substring(rawIndex, rawIndexEnd - rawIndex);
                        string rawWithHyperLinks = raw;

                        rawWithHyperLinks = EncapsulateHyperLinks(rawWithHyperLinks);

                        raw = UserDefinedFunctions.ExtractText("<" + raw + ">").Value.Trim().TrimStart('<').TrimEnd('>').Trim().TrimStart('<').TrimEnd('>');
                        rawWithHyperLinks = UserDefinedFunctions.ExtractText("<" + rawWithHyperLinks + ">").Value.Trim().TrimStart('<').TrimEnd('>').Trim().TrimStart('<').TrimEnd('>');

                        return raw + "|" + rawWithHyperLinks;
                    }
                    else
                    {
                    }
                }
            }

            return null;
        }

        public static string EncapsulateHyperLinks(string rawWithHyperLinks)
        {
            HtmlDocument htmlDocument = new HtmlDocument();

            htmlDocument.LoadHtml(rawWithHyperLinks);

            HashSet<string> replacedHrefs = new HashSet<string>();

            foreach (HtmlNode htmlNode in htmlDocument.DocumentNode.Descendants("a"))
            {
                if (!htmlNode.Attributes["href"].Value.ToLowerInvariant().EndsWith("/info") && !htmlNode.Attributes["href"].Value.ToLowerInvariant().Contains("/profile.php?"))
                {
                    if (!replacedHrefs.Contains(htmlNode.Attributes["href"].Value))
                    {
                        rawWithHyperLinks = rawWithHyperLinks.Replace(htmlNode.Attributes["href"].Value, "> (" + htmlNode.Attributes["href"].Value + ") <");

                        replacedHrefs.Add(htmlNode.Attributes["href"].Value);
                    }
                }
            }

            return rawWithHyperLinks;
        }
    }
}