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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#endregion

namespace Arachnode.Search
{
    public class SearchEngine
    {
        protected static readonly Regex _greaterThanWithoutSpace = new Regex(">", RegexOptions.Compiled);
        protected static readonly Regex _greaterThanWithSpace = new Regex("> ", RegexOptions.Compiled);
        protected static readonly Regex _lessThanWithoutSpace = new Regex("<", RegexOptions.Compiled);
        protected static readonly Regex _lessThanWithSpace = new Regex("< ", RegexOptions.Compiled);
        protected static readonly object _lock = new object();

        protected static readonly Regex _removeComments = new Regex("<!--.*?-->",
                                                                    RegexOptions.Compiled | RegexOptions.Singleline);

        protected static readonly Regex _removeScripts = new Regex("<[ s]cript.*?scrip[t ]>",
                                                                   RegexOptions.Compiled | RegexOptions.IgnoreCase |
                                                                   RegexOptions.Singleline);

        protected static readonly Regex _removeSelected = new Regex("<[ s]elect.*?selec[t ]>",
                                                                    RegexOptions.Compiled | RegexOptions.IgnoreCase |
                                                                    RegexOptions.Singleline);

        protected static readonly Regex _removeStyles = new Regex("<[s ]tyle.*?styl[e ]>",
                                                                  RegexOptions.Compiled | RegexOptions.IgnoreCase |
                                                                  RegexOptions.Singleline);

        protected static readonly Regex _removeTabsCarraigeReturnsAndNewlines = new Regex("[\t\r\n]",
                                                                                          RegexOptions.Compiled);

        protected static readonly Regex _removeTags = new Regex("<.*?>", RegexOptions.Compiled | RegexOptions.Singleline);
        protected static readonly Regex _removeWhiteSpace = new Regex(@"\s{2,}", RegexOptions.Compiled);
        protected readonly InvertedIndex _invertedIndex = new InvertedIndex();

        protected Document GetDocument(string uniqueKey, string text, Dictionary<object, object> properties)
        {
            Document document = new Document();

            document.UniqueKey = uniqueKey;
            document.Properties = properties;
            document.Text = ExtractText(text);
            document.Words = new Dictionary<string, Word>();

            return document;
        }

        public void CreateDocument(string uniqueKey, string text, Dictionary<object, object> properties)
        {
            ParseAndAddDocument(GetDocument(uniqueKey, text, properties));
        }

        private void ParseAndAddDocument(Document document)
        {
            List<string> words = GetWords(document.Text);

            foreach (string word in words)
            {
                if (!document.Words.ContainsKey(word))
                {
                    Word word2 = new Word();

                    document.Words.Add(word, word2);
                }

                document.Words[word].Count++;

                document.TotalWords++;
            }

            foreach (string word in words.Distinct())
            {
                lock (_lock)
                {
                    if (!_invertedIndex.ContainsKey(word))
                    {
                        _invertedIndex.Add(word, new List<Document>());
                    }
                }

                _invertedIndex[word].Add(document);
            }

            _invertedIndex.Documents.Add(document);

            _invertedIndex.Total++;
        }

        public static string ExtractText(string source)
        {
            string text = _removeScripts.Replace(source, string.Empty);
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

            return text;
        }

        protected static List<string> GetWords(string text)
        {
            List<string> words =
                new List<string>(text.ToLower().Split(" -~!@#$%^&*()_+`=<>,.?/\":;{}[]|\\".ToCharArray(),
                                                      StringSplitOptions.RemoveEmptyEntries));

            return words;
        }

        public List<Document> Search(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return _invertedIndex.Documents;
            }

            Dictionary<string, Document> documents = new Dictionary<string, Document>();

            foreach (string word in GetWords(text))
            {
                if (_invertedIndex.ContainsKey(word))
                {
                    foreach (Document document in _invertedIndex[word])
                    {
                        if (!documents.ContainsKey(document.UniqueKey))
                        {
                            Document document2 = new Document();

                            document2.UniqueKey = document.UniqueKey;
                            document2.Text = document.Text;

                            documents.Add(document.UniqueKey, document2);
                        }

                        documents[document.UniqueKey].Score += (document.Words[word].Count/document.TotalWords)*
                                                               Math.Log10((_invertedIndex.Total/_invertedIndex[word].Count) +
                                                                          1);
                    }
                }
            }

            List<Document> documents2 = new List<Document>(documents.Values);

            documents2.Sort((document1, document2) => -document1.Score.CompareTo(document2.Score));

            return documents2;
        }
    }
}