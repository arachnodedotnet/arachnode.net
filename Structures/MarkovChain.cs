#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Arachnode.Utilities;
using Newtonsoft.Json;

#endregion

namespace Arachnode.Structures
{
    [Serializable]
    public class MarkovChainInputNodeString
    {
        public string String { get; internal set; }
        public Dictionary<string, double> Attributes { get; internal set; }

        public override string ToString()
        {
            return String;
        }
    }

    [Serializable]
    public class MarkovChainInputString
    {
        public List<MarkovChainInputNodeString> Nodes { get; set; }

        public override string ToString()
        {
            string toReturn = null;

            if (Nodes != null)
            {
                foreach(MarkovChainInputNodeString markovChainInputNodeString in Nodes)
                {
                    toReturn += markovChainInputNodeString.String + " ";
                }

                if(!string.IsNullOrEmpty(toReturn))
                {
                    toReturn = toReturn.Trim();
                }
            }

            return toReturn;
        }
    }

    [Serializable]
    public class MarkovChainNodeString : AMarkovChainStorage
    {
        public Dictionary<string, double> Attributes { get; set; }
        public int ChainCount { get; set; }
        //the derialization process is not able to instantiate the Abstract class...  (it is recalled from Disk anyway, no need to load and then re-load properties (Directories)...  :))
        [JsonIgnore]
        public AMarkovChainNodeStorageString Children { get; set; }
        public int Index { get; set; }
        public bool? IsWordBoundary { get; set; }
        public bool? IsInputBoundary { get; set; }
        public MarkovChainNodeString Parent { get; set; }

        internal MarkovChainNodeString()
        {
        }

        internal MarkovChainNodeString(Type aMarkovChainNodeStorageType, bool clear, string onDiskDirectoryBasePath, string path)
        {
            Children = (AMarkovChainNodeStorageString)Activator.CreateInstance(aMarkovChainNodeStorageType, onDiskDirectoryBasePath);
            Children.Path = path;

            if (clear)
            {
                Children.Clear();
            }

            Attributes = new Dictionary<string, double>();

            _onDiskDirectoryBasePath = onDiskDirectoryBasePath;
            Path = path;
        }

        internal void Update()
        {
            if (!string.IsNullOrEmpty(_onDiskDirectoryBasePath))
            {
                string filePath = GetActualOnDiskFilePath(null);

                Serialization.SerializeObject(filePath, this);
            }
        }

        public override string ToString()
        {
            string toReturn = null;

            if (Children != null && Children.Count != 0)
            {
                toReturn = Children.First().Key.ToString();
            }

            return toReturn + ", ChainCount: " + ChainCount + ", IsWordBoundary: " + IsWordBoundary;
        }
    }

    [Serializable]
    public abstract class MarkovChainBase
    {
        public decimal ChainCountKnown { get; set; }
        public decimal ChainCountTotalKnown { get; set; }
        public decimal ChainCount { get; set; }
        public decimal ChainCountTotal { get; set; }
        public string Input { get; set; }
        public bool? IsBrokenChain { get; set; }
        public string String { get; set; }

        public decimal Score
        {
            get
            {
                if (string.IsNullOrEmpty(String) || string.IsNullOrEmpty(Input))
                    return 0;

                if (ChainCountKnown * ChainCount == 0 || ChainCountTotalKnown * ChainCountTotal == 0)
                    return 0;

                return (Input.Length / (decimal)String.Length) * (ChainCountKnown / ChainCount) * (ChainCountTotalKnown / ChainCountTotal);
            }
        }

        public override string ToString()
        {
            return "String: " + String +
                   ", ChainCountKnown: " + ChainCountKnown +
                   ", ChainCount: " + ChainCount +
                   ", ChainCountTotalKnown: " + ChainCountTotalKnown +
                   ", ChainCountTotal: " + ChainCountTotal;
        }
    }

    [Serializable]
    public class MarkovChainString : MarkovChainBase
    {
        public List<MarkovChainNodeString> Nodes { get; set; }
    }

    [Serializable]
    public class MarkovChain<MCStorageString> where MCStorageString : AMarkovChainNodeStorageString
    {
        private readonly bool _extractDistinctWords;
        private readonly bool _extractText;
        private readonly bool _isCaseSensitive;

        private string _onDiskDirectoryBasePath;

        private MarkovChainNodeString _markovChainString;

        public MarkovChain(bool extractText, bool extractDistinctWords, bool isCaseSensitive)
        {
            _extractText = extractText;
            _extractDistinctWords = extractDistinctWords;
            _isCaseSensitive = isCaseSensitive;

            _markovChainString = new MarkovChainNodeString(typeof(MCStorageString), false, _onDiskDirectoryBasePath, null);
        }

        public MarkovChain(bool extractText, bool extractDistinctWords, bool isCaseSensitive, bool clear, string onDiskDirectoryBasePath)
        {
            _extractText = extractText;
            _extractDistinctWords = extractDistinctWords;
            _isCaseSensitive = isCaseSensitive;

            _onDiskDirectoryBasePath = onDiskDirectoryBasePath;

            _markovChainString = new MarkovChainNodeString(typeof(MCStorageString), clear, _onDiskDirectoryBasePath, null);
        }

        public MarkovChainNodeString MarkovChainString
        {
            get { return _markovChainString; }
        }

        #region Strings

        public MarkovChainInputString Preprocess(string input, int maximumNumberOfNodesToProcess)
        {
            MarkovChainInputString markovChainInputString = new MarkovChainInputString();

            markovChainInputString.Nodes = new List<MarkovChainInputNodeString>();

            foreach (string word in UserDefinedFunctions.ExtractWords(input, _extractText, _extractDistinctWords))
            {
                MarkovChainInputNodeString markovChainInputNodeString = new MarkovChainInputNodeString();

                markovChainInputNodeString.String = word;
                markovChainInputNodeString.Attributes = new Dictionary<string, double>();

                markovChainInputString.Nodes.Add(markovChainInputNodeString);
            }

            markovChainInputString.Nodes = markovChainInputString.Nodes.Take(maximumNumberOfNodesToProcess).ToList();

            return markovChainInputString;
        }

        public MarkovChainNodeString AddMarkovChainString(string input, bool addMarkovChainBreaks, Dictionary<string, double> attributes)
        {
            return AddMarkovChainString(Preprocess(input, int.MaxValue), addMarkovChainBreaks, attributes);
        }

        public MarkovChainNodeString AddMarkovChainString(MarkovChainInputString markovChainInputString, bool addMarkovChainBreaks, Dictionary<string, double> attributes)
        {
            if (markovChainInputString == null || markovChainInputString.Nodes == null || markovChainInputString.Nodes.Count == 0 || string.IsNullOrEmpty(markovChainInputString.ToString()))
            {
                return null;
            }

            MarkovChainNodeString parent = null;
            MarkovChainNodeString currentMarkovChainNodeString = null;
            bool isFirstWord = true;
            string lastWord = null;

            if (!_isCaseSensitive)
            {
                for (int i = 0; i < markovChainInputString.Nodes.Count; i++)
                {
                    markovChainInputString.Nodes[i].String = markovChainInputString.Nodes[i].String.ToLowerInvariant();
                }
            }

            int index = 1;
            string path = null;

            foreach (MarkovChainInputNodeString markovChainInputNodeString in markovChainInputString.Nodes)
            {
                string word2 = string.Intern(markovChainInputNodeString.String.Trim());
                lastWord = word2;
                path += word2 + "\\";

                if (isFirstWord)
                {
                    isFirstWord = false;

                    if (!MarkovChainString.Children.ContainsKey(word2))
                    {
                        MarkovChainNodeString markovChainNodeString = new MarkovChainNodeString(typeof (MCStorageString), false, _onDiskDirectoryBasePath, path);

                        MarkovChainString.Children.Add(word2, markovChainNodeString);
                    }

                    MarkovChainString.ChainCount++;

                    ManageAttributes(MarkovChainString, attributes);

                    MarkovChainString.Update();

                    /**/

                    currentMarkovChainNodeString = MarkovChainString.Children[word2];
                    currentMarkovChainNodeString.ChainCount++;
                    currentMarkovChainNodeString.Children.Path = path;
                    currentMarkovChainNodeString.IsInputBoundary = false;
                    currentMarkovChainNodeString.IsWordBoundary = true;
                    //commented to allow the model to be serialiazed to JSON...  should be re-added once disk-backed storage is added...
                    //currentMarkovChainNodeString.Parent = MarkovChainString
                    currentMarkovChainNodeString.Index = index++;

                    ManageAttributes(currentMarkovChainNodeString, attributes);

                    MarkovChainString.Children[word2] = currentMarkovChainNodeString;
                    //currentMarkovChainNodeString.Update();

                    continue;
                }

                if (!currentMarkovChainNodeString.Children.ContainsKey(word2))
                {
                    MarkovChainNodeString markovChainNodeString = new MarkovChainNodeString(typeof(MCStorageString), false, _onDiskDirectoryBasePath, path);

                    currentMarkovChainNodeString.Children.Add(word2, markovChainNodeString);
                }

                parent = currentMarkovChainNodeString;
                currentMarkovChainNodeString = currentMarkovChainNodeString.Children[word2];
                currentMarkovChainNodeString.Path = path;
                currentMarkovChainNodeString.ChainCount++;
                currentMarkovChainNodeString.Children.Path = path;
                currentMarkovChainNodeString.IsInputBoundary = false;
                currentMarkovChainNodeString.IsWordBoundary = true;
                //commented to allow the model to be serialiazed to JSON...  should be re-added once disk-backed storage is added...
                //currentMarkovChainNodeString.Parent = parent;
                currentMarkovChainNodeString.Index = index++;

                ManageAttributes(currentMarkovChainNodeString, attributes);

                if (addMarkovChainBreaks)
                {
                    //experimental...
                    if (!MarkovChainString.Children.ContainsKey(word2))
                    {
                        MarkovChainString.Children.Add(word2, currentMarkovChainNodeString);
                    }
                    else
                    {
                        ManageAttributes(currentMarkovChainNodeString, currentMarkovChainNodeString.Attributes);
                    }
                }

                parent.Children[word2] = currentMarkovChainNodeString;
                //currentMarkovChainNodeString.Update();
            }

            if (currentMarkovChainNodeString != null && parent != null && !string.IsNullOrEmpty(lastWord))
            {
                currentMarkovChainNodeString.IsInputBoundary = true;
                parent.Children[lastWord] = currentMarkovChainNodeString;
                //currentMarkovChainNodeString.Update();
            }

            return MarkovChainString;
        }

        private void ManageAttributes(MarkovChainNodeString currentMarkovChainNodeString, Dictionary<string, double> attributes)
        {
            if (attributes != null)
            {
                if (currentMarkovChainNodeString.Attributes == null)
                    currentMarkovChainNodeString.Attributes = new Dictionary<string, double>();

                List<string> keys = attributes.Keys.ToList();

                foreach (string key in keys)
                {
                    if (!currentMarkovChainNodeString.Attributes.ContainsKey(key))
                        currentMarkovChainNodeString.Attributes.Add(key, attributes[key]);
                    else
                        currentMarkovChainNodeString.Attributes[key] += attributes[key];
                }
            }
        }

        public void PrepareMarkovChainString()
        {
            PrepareMarkovChainString(MarkovChainString);
        }

        private void PrepareMarkovChainString(MarkovChainNodeString markovChainNodeString)
        {
            markovChainNodeString.Children = new MarkovChainNodeStorageInMemoryDictionaryString(markovChainNodeString.Children.OrderByDescendingChainCount(), _onDiskDirectoryBasePath);

            foreach (KeyValuePair<string, MarkovChainNodeString> keyValuePair in markovChainNodeString.Children)
            {
                PrepareMarkovChainString(keyValuePair.Value);
            }
        }

        public MarkovChainString GetMarkovChainString(string input, bool decrementChainCount, bool alwaysContinueToEndOfChain)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            MarkovChainString markovChainString = new MarkovChainString();
            markovChainString.IsBrokenChain = false;
            markovChainString.Nodes = new List<MarkovChainNodeString>();
            markovChainString.Input = input;

            bool continueProcessing = true;

            if (MarkovChainString.Children.Count != 0)
            {
                MarkovChainNodeString startingNodeString = MarkovChainString;
                decimal chainCount = 0;
                decimal chainCountTotal = 0;
                string path = null;

                StringBuilder chainString = new StringBuilder();

                if (!string.IsNullOrEmpty(input))
                {
                    if (!_isCaseSensitive)
                    {
                        input = input.ToLowerInvariant();
                    }

                    Func<string, bool> processWord = new Func<string, bool>(delegate(string word)
                    {
                        try
                        {
                            markovChainString.Nodes.Add(startingNodeString);

                            chainCount += startingNodeString.Children[word].ChainCount;
                            chainCountTotal += startingNodeString.Children.Values.Sum(_ => _.ChainCount);

                            chainString.Append(word + " ");

                            startingNodeString = startingNodeString.Children[word];
                            if (decrementChainCount)
                            {
                                startingNodeString.ChainCount--;
                            }

                            startingNodeString.Path = path;
                            startingNodeString.Children.Path = path;
                        }
                        catch (Exception)
                        {
                            return false;
                        }

                        return true;
                    });

                    List<string> words = UserDefinedFunctions.ExtractWords(input, _extractText, _extractDistinctWords).Cast<string>().ToList();

                    foreach (string word in words)
                    {
                        path += word + "\\";

                        if (startingNodeString.Children.ContainsKey(word))
                        {
                            processWord(word);
                        }
                        else if (MarkovChainString.Children.ContainsKey(word))
                        {
                            startingNodeString = MarkovChainString;

                            markovChainString.IsBrokenChain = true;

                            processWord(word);
                        }
                        else
                        {
                            if (startingNodeString == MarkovChainString)
                            {
                                return markovChainString;
                            }

                            break;
                        }
                    }

                    if (markovChainString.Nodes.Count == words.Count())
                    {
                        continueProcessing = false;
                    }
                }

                /**/

                markovChainString.ChainCountKnown = chainCount;
                markovChainString.ChainCountTotalKnown = chainCountTotal;

                if (continueProcessing || alwaysContinueToEndOfChain)
                {
                    KeyValuePair<string, MarkovChainNodeString> keyValuePair = startingNodeString.Children.OrderByDescendingChainCount().FirstOrDefault();

                    while (keyValuePair.Key != null && startingNodeString.Children.Values.Count != 0)
                    {
                        markovChainString.Nodes.Add(startingNodeString);

                        chainCount += startingNodeString.Children[keyValuePair.Key].ChainCount;
                        chainCountTotal += startingNodeString.Children.Values.Sum(_ => _.ChainCount);

                        chainString.Append(keyValuePair.Key + " ");

                        startingNodeString = startingNodeString.Children[keyValuePair.Key];
                        if (decrementChainCount)
                        {
                            keyValuePair.Value.ChainCount--;
                        }

                        path += keyValuePair.Key + "\\";

                        startingNodeString.Path = path;
                        startingNodeString.Children.Path = path;

                        keyValuePair = startingNodeString.Children.OrderByDescendingChainCount().FirstOrDefault();
                    }
                }

                markovChainString.String = chainString.ToString().Trim();
                markovChainString.ChainCount = chainCount;
                markovChainString.ChainCountTotal = chainCountTotal;
            }

            return markovChainString;
        }

        #endregion

        public void ResetString()
        {
            _markovChainString = new MarkovChainNodeString(typeof(MCStorageString), true, _onDiskDirectoryBasePath, null);
        }

        #region Test
        public void Test()
        {
            //TestBaseline();

            TestBrokenChains();

            TestChainRecall();
            
            TestChainRecallWithReset();

            TestDecrementChainCount();

            TestLongFormInput();

            TestDirectoryStress();

            TestLongPaths();

            TestPerInputNodeAttributes();

#if DEBUG
            Console.WriteLine("Test: Successful.");
#endif
        }

        private void TestBaseline()
        {
            _markovChainString.Children.Clear();
            MarkovChainString mostMarkovChainUsingStrings = null;

            //test simple and w/o Attributes...
            AddMarkovChainString(Preprocess("I love.", int.MaxValue), false, null);
            AddMarkovChainString(Preprocess("I love.", int.MaxValue), false, null);
            AddMarkovChainString(Preprocess("I like.", int.MaxValue), false, null);

            /**/

            mostMarkovChainUsingStrings = GetMarkovChainString("I", false, true);

#if DEBUG
            Console.WriteLine(mostMarkovChainUsingStrings.String + " :: " + mostMarkovChainUsingStrings.ChainCount);
#endif
            Debug.Assert(mostMarkovChainUsingStrings.ChainCount == 5);
            Debug.Assert(mostMarkovChainUsingStrings.ChainCountTotal == 6);
            if (!_isCaseSensitive)
                Debug.Assert(mostMarkovChainUsingStrings.String == "i love");
            else
                Debug.Assert(mostMarkovChainUsingStrings.String == "I love");

            /**/

            mostMarkovChainUsingStrings = GetMarkovChainString("I love", false, true);

#if DEBUG
            Console.WriteLine(mostMarkovChainUsingStrings.String + " :: " + mostMarkovChainUsingStrings.ChainCount);
#endif
            Debug.Assert(mostMarkovChainUsingStrings.ChainCount == 5);
            Debug.Assert(mostMarkovChainUsingStrings.ChainCountTotal == 6);
            if (!_isCaseSensitive)
                Debug.Assert(mostMarkovChainUsingStrings.String == "i love");
            else
                Debug.Assert(mostMarkovChainUsingStrings.String == "I love");

            /**/

            mostMarkovChainUsingStrings = GetMarkovChainString("I like", false, true);

#if DEBUG
            Console.WriteLine(mostMarkovChainUsingStrings.String + " :: " + mostMarkovChainUsingStrings.ChainCount);
#endif
            Debug.Assert(mostMarkovChainUsingStrings.ChainCount == 4);
            Debug.Assert(mostMarkovChainUsingStrings.ChainCountTotal == 6);
            if (!_isCaseSensitive)
                Debug.Assert(mostMarkovChainUsingStrings.String == "i like");
            else
                Debug.Assert(mostMarkovChainUsingStrings.String == "I like");

            /**/

            _markovChainString = new MarkovChainNodeString(typeof(MCStorageString), true, _onDiskDirectoryBasePath, null);

            //test with Attributes...
            Dictionary<string, double> attributes = new Dictionary<string, double>();
            attributes.Add("Attribute3", 3);
            attributes.Add("Attribute1", 1);

            AddMarkovChainString(Preprocess("I love cats more than I love dogs.", int.MaxValue), false, attributes); //3,3,2,2,2,2
            AddMarkovChainString(Preprocess("I love cats more than I love dogs.", int.MaxValue), false, attributes);
            AddMarkovChainString(Preprocess("I love dogs more than I love cats.", int.MaxValue), false, attributes);

            /**/

            mostMarkovChainUsingStrings = GetMarkovChainString("I love", false, true);

#if DEBUG
            Console.WriteLine(mostMarkovChainUsingStrings.String + " :: " + mostMarkovChainUsingStrings.ChainCount);
#endif
            Debug.Assert(mostMarkovChainUsingStrings.ChainCount == 18);
            Debug.Assert(mostMarkovChainUsingStrings.ChainCountTotal == 19);
            if (!_isCaseSensitive)
                Debug.Assert(mostMarkovChainUsingStrings.String == "i love cats more than i love dogs");
            else
                Debug.Assert(mostMarkovChainUsingStrings.String == "I love cats more than I love dogs");

            /**/

            mostMarkovChainUsingStrings = GetMarkovChainString("I love cats", false, true);

#if DEBUG
            Console.WriteLine(mostMarkovChainUsingStrings.String + " :: " + mostMarkovChainUsingStrings.ChainCount);
#endif
            Debug.Assert(mostMarkovChainUsingStrings.ChainCount == 18);
            Debug.Assert(mostMarkovChainUsingStrings.ChainCountTotal == 19);
            if (!_isCaseSensitive)
                Debug.Assert(mostMarkovChainUsingStrings.String == "i love cats more than i love dogs");
            else
                Debug.Assert(mostMarkovChainUsingStrings.String == "I love cats more than I love dogs");
        }

        private void TestBrokenChains()
        {
            _markovChainString = new MarkovChainNodeString(typeof(MCStorageString), true, _onDiskDirectoryBasePath, null);
            MarkovChainString mostMarkovChainUsingStrings = null;

            AddMarkovChainString(Preprocess("I love pizza.", int.MaxValue), true, null);
            AddMarkovChainString(Preprocess("I love pizza.", int.MaxValue), true, null);
            AddMarkovChainString(Preprocess("I love hot dogs.", int.MaxValue), true, null);
            AddMarkovChainString(Preprocess("I love hot dogs.", int.MaxValue), true, null);
            AddMarkovChainString(Preprocess("I love hot dogs.", int.MaxValue), true, null);
            AddMarkovChainString(Preprocess("I love hot dogs.", int.MaxValue), true, null);
            AddMarkovChainString(Preprocess("love pizza.", int.MaxValue), true, null);
            AddMarkovChainString(Preprocess("love pizza pets.", int.MaxValue), true, null);
            AddMarkovChainString(Preprocess("love pizza pals.", int.MaxValue), true, null);
            AddMarkovChainString(Preprocess("love pizza pals.", int.MaxValue), true, null);
            AddMarkovChainString(Preprocess("love pizza pals.", int.MaxValue), true, null);

            /**/

            mostMarkovChainUsingStrings = GetMarkovChainString("I", false, true);
#if DEBUG
            Debug.Assert(mostMarkovChainUsingStrings.ToString().StartsWith("String: i love pizza pals"));
#endif

            mostMarkovChainUsingStrings = GetMarkovChainString("love", false, true);
#if DEBUG
            Debug.Assert(mostMarkovChainUsingStrings.ToString().StartsWith("String: love pizza pals"));
#endif

            mostMarkovChainUsingStrings = GetMarkovChainString("pizza", false, true);
#if DEBUG
            Debug.Assert(mostMarkovChainUsingStrings.ToString().StartsWith("String: pizza pals"));
#endif
            mostMarkovChainUsingStrings = GetMarkovChainString("pals", false, true);
#if DEBUG
            Debug.Assert(mostMarkovChainUsingStrings.ToString().StartsWith("String: pals"));
#endif
        }

        private void TestChainRecall()
        {
            _markovChainString = new MarkovChainNodeString(typeof(MCStorageString), true, _onDiskDirectoryBasePath, null);
            MarkovChainString mostMarkovChainUsingStrings = null;

            Dictionary<string, double> hikingAttributes = new Dictionary<string, double>();
            hikingAttributes.Add("hiking", 1);
            hikingAttributes.Add("bacon", 0);

            AddMarkovChainString(Preprocess("I love hiking.", int.MaxValue), false, hikingAttributes);
            AddMarkovChainString(Preprocess("I love hiking.", int.MaxValue), false, hikingAttributes);

            mostMarkovChainUsingStrings = GetMarkovChainString("I love hiking.", false, false);
#if DEBUG
            Debug.Assert(mostMarkovChainUsingStrings.ToString().StartsWith("String: i love hiking"));

            Debug.Assert(mostMarkovChainUsingStrings.Nodes.Sum(_ => _.Attributes["hiking"]) == 6);
            Debug.Assert(mostMarkovChainUsingStrings.Nodes.Sum(_ => _.Attributes["bacon"]) == 0);
#endif
            /**/

            Dictionary<string, double> baconAttributes = new Dictionary<string, double>();
            baconAttributes.Add("hiking", 0);
            baconAttributes.Add("bacon", 1);

            AddMarkovChainString(Preprocess("I love bacon.", int.MaxValue), false, baconAttributes);
            AddMarkovChainString(Preprocess("I love bacon.", int.MaxValue), false, baconAttributes);

            mostMarkovChainUsingStrings = GetMarkovChainString("I love bacon.", false, false);
#if DEBUG
            Debug.Assert(mostMarkovChainUsingStrings.ToString().StartsWith("String: i love bacon"));

            Debug.Assert(mostMarkovChainUsingStrings.Nodes.Sum(_ => _.Attributes["hiking"]) == 6);
            Debug.Assert(mostMarkovChainUsingStrings.Nodes.Sum(_ => _.Attributes["bacon"]) == 6);
#endif
        }

        private void TestChainRecallWithReset()
        {
            _markovChainString = new MarkovChainNodeString(typeof(MCStorageString), true, _onDiskDirectoryBasePath, null);
            MarkovChainString mostMarkovChainUsingStrings = null;

            Dictionary<string, double> hikingAttributes = new Dictionary<string, double>();
            hikingAttributes.Add("hiking", 1);
            hikingAttributes.Add("bacon", 0);

            AddMarkovChainString(Preprocess("I love hiking.", int.MaxValue), false, hikingAttributes);
            AddMarkovChainString(Preprocess("I love hiking.", int.MaxValue), false, hikingAttributes);

            mostMarkovChainUsingStrings = GetMarkovChainString("I love hiking.", false, false);
#if DEBUG
            Debug.Assert(mostMarkovChainUsingStrings.ToString().StartsWith("String: i love hiking"));

            Debug.Assert(mostMarkovChainUsingStrings.Nodes.Sum(_ => _.Attributes["hiking"]) == 6);
            Debug.Assert(mostMarkovChainUsingStrings.Nodes.Sum(_ => _.Attributes["bacon"]) == 0);
#endif
            /**/

            ResetString();

            /**/

            Dictionary<string, double> baconAttributes = new Dictionary<string, double>();
            baconAttributes.Add("hiking", 0);
            baconAttributes.Add("bacon", 1);

            AddMarkovChainString(Preprocess("I love bacon.", int.MaxValue), false, baconAttributes);
            AddMarkovChainString(Preprocess("I love bacon.", int.MaxValue), false, baconAttributes);

            mostMarkovChainUsingStrings = GetMarkovChainString("I love bacon.", false, false);
#if DEBUG
            Debug.Assert(mostMarkovChainUsingStrings.ToString().StartsWith("String: i love bacon"));

            Debug.Assert(mostMarkovChainUsingStrings.Nodes.Sum(_ => _.Attributes["hiking"]) == 0);
            Debug.Assert(mostMarkovChainUsingStrings.Nodes.Sum(_ => _.Attributes["bacon"]) == 6);
#endif
        }

        private void TestDecrementChainCount()
        {
            _markovChainString = new MarkovChainNodeString(typeof(MCStorageString), true, _onDiskDirectoryBasePath, null);
            MarkovChainString mostMarkovChainUsingStrings = null;

            AddMarkovChainString(Preprocess("I love Hawaii.", int.MaxValue), true, null);
            AddMarkovChainString(Preprocess("I love Hawaii.", int.MaxValue), true, null);
            AddMarkovChainString(Preprocess("I love Hawaii.", int.MaxValue), true, null);
            AddMarkovChainString(Preprocess("I love Hawaii.", int.MaxValue), true, null);
            AddMarkovChainString(Preprocess("I love Hawaii.", int.MaxValue), true, null);

            mostMarkovChainUsingStrings = GetMarkovChainString("I love Hawaii.", true, false);
            Debug.Assert(mostMarkovChainUsingStrings.ChainCount == 15);

            mostMarkovChainUsingStrings = GetMarkovChainString("I love Hawaii.", true, false);
            Debug.Assert(mostMarkovChainUsingStrings.ChainCount == 12);

            mostMarkovChainUsingStrings = GetMarkovChainString("I love Hawaii.", true, false);
            Debug.Assert(mostMarkovChainUsingStrings.ChainCount == 9);

            mostMarkovChainUsingStrings = GetMarkovChainString("I love Hawaii.", true, false);
            Debug.Assert(mostMarkovChainUsingStrings.ChainCount == 6);

            mostMarkovChainUsingStrings = GetMarkovChainString("I love Hawaii.", true, false);
            Debug.Assert(mostMarkovChainUsingStrings.ChainCount == 3);

            mostMarkovChainUsingStrings = GetMarkovChainString("I love Hawaii.", true, false);
            Debug.Assert(mostMarkovChainUsingStrings.ChainCount == 0);
        }

        private void TestLongFormInput()
        {
            _markovChainString.Children.Clear();
            MarkovChainString mostMarkovChainUsingStrings = null;

            int numberOfExemplars = 0;

            Stopwatch stopwatch = new Stopwatch();

            int numberOfLinesToTake = 1000;

            foreach(string line in File.ReadAllLines("TextDatabases\\Dracula.txt").Take(numberOfLinesToTake))
            {
                string line2 = string.Join(" ",  UserDefinedFunctions.ExtractWords(line, true, false).Cast<string>().ToArray());

                if(string.IsNullOrEmpty(line2))
                {
                    continue;
                }

                numberOfExemplars++;

                stopwatch.Start();
                AddMarkovChainString(Preprocess(line2, int.MaxValue), false, null);

                mostMarkovChainUsingStrings = GetMarkovChainString(line2, false, false);
                stopwatch.Stop();
#if DEBUG
                if (numberOfExemplars % 100 == 0)
                {
                    Console.WriteLine("TestLongFormInput: R/W :: " + (numberOfExemplars / stopwatch.Elapsed.TotalSeconds) + "/sec. :: " + numberOfExemplars);
                }
#endif
                if (!_isCaseSensitive)
                    Debug.Assert(mostMarkovChainUsingStrings.String == line2.ToLowerInvariant().TrimEnd());
                else
                    Debug.Assert(mostMarkovChainUsingStrings.String == line2.TrimEnd());
            }

            _markovChainString.Children.Clear();

            numberOfExemplars = 0;
            stopwatch.Reset();

            foreach (string line in File.ReadAllLines("TextDatabases\\Dracula.txt").Take(numberOfLinesToTake))
            {
                string line2 = string.Join(" ", UserDefinedFunctions.ExtractWords(line, true, false).Cast<string>().ToArray());

                if (string.IsNullOrEmpty(line2))
                {
                    continue;
                }

                numberOfExemplars++;

                stopwatch.Start();
                AddMarkovChainString(Preprocess(line2, int.MaxValue), false, null);
                stopwatch.Stop();
#if DEBUG
                if (numberOfExemplars % 100 == 0)
                {
                    Console.WriteLine("TestLongFormInput: W :: " + (numberOfExemplars / stopwatch.Elapsed.TotalSeconds) + "/sec. :: " + numberOfExemplars);
                }
#endif
            }

            numberOfExemplars = 0;
            stopwatch.Reset();

            foreach (string line in File.ReadAllLines("TextDatabases\\Dracula.txt").Take(numberOfLinesToTake))
            {
                string line2 = string.Join(" ", UserDefinedFunctions.ExtractWords(line, true, false).Cast<string>().ToArray());

                if (string.IsNullOrEmpty(line2))
                {
                    continue;
                }

                numberOfExemplars++;

                stopwatch.Start();
                mostMarkovChainUsingStrings = GetMarkovChainString(line2, false, false);
                stopwatch.Stop();
#if DEBUG
                if (numberOfExemplars % 100 == 0)
                {
                    Console.WriteLine("TestLongFormInput: R :: " + (numberOfExemplars / stopwatch.Elapsed.TotalSeconds) + "/sec. :: " + numberOfExemplars);
                }
#endif
                if (!_isCaseSensitive)
                    Debug.Assert(mostMarkovChainUsingStrings.String == line2.ToLowerInvariant().TrimEnd());
                else
                    Debug.Assert(mostMarkovChainUsingStrings.String == line2.TrimEnd());
            }
        }

        private void TestDirectoryStress()
        {
            _markovChainString.Children.Clear();
            MarkovChainString mostMarkovChainUsingStrings = null;

            char[] directoryName = new char[3];
            for (int i = 0; i < directoryName.Length; i++)
            {
                directoryName[i] = 'a';
            }

            int pointer = directoryName.Length - 1;
            bool loop = true;
            string directoryName2 = null;
            int numberOfWords = 0;
            int numberOfWordsPerInputString = 20;
            StringBuilder stringBuilder = new StringBuilder();

            int numberOfDirectories = 0;

            Stopwatch stopwatch = new Stopwatch();

            while (loop)
            {
                directoryName2 = new string(directoryName);
                numberOfDirectories++;

                stringBuilder.Append(directoryName2 + " ");
                numberOfWords++;

                if (numberOfWords == numberOfWordsPerInputString)
                {
                    stopwatch.Start();
                    AddMarkovChainString(Preprocess(stringBuilder.ToString(), int.MaxValue), false, null);

                    string input = stringBuilder.ToString();
                    string[] inputSplit = input.Split(" ".ToCharArray());

                    mostMarkovChainUsingStrings = GetMarkovChainString(inputSplit[0], false, true);
                    stopwatch.Stop();
#if DEBUG
                    if (numberOfDirectories % 100 == 0)
                    {
                        Console.WriteLine(numberOfDirectories / stopwatch.Elapsed.TotalSeconds + "/sec. :: " + numberOfDirectories);
                    }
#endif
                    if (!_isCaseSensitive)
                        Debug.Assert(mostMarkovChainUsingStrings.String == input.ToLowerInvariant().TrimEnd());
                    else
                        Debug.Assert(mostMarkovChainUsingStrings.String == input.TrimEnd());

                    stringBuilder.Remove(0, stringBuilder.Length);
                    numberOfWords = 0;
                }
             
                /**/

                directoryName[pointer]++;

                while (directoryName[pointer] > 'z')
                {
                    if (pointer == 0)
                    {
                        loop = false;
                        break;
                    }

                    directoryName[pointer] = 'a';
                    if (pointer - 1 >= 0)
                    {
                        directoryName[pointer - 1]++;
                        pointer--;
                    }
                }
                pointer = directoryName.Length - 1;
            }
        }

        private void TestLongPaths()
        {
            _markovChainString.Children.Clear();
            MarkovChainString mostMarkovChainUsingStrings = null;

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < 1000; i++)
            {
                stringBuilder.Append("Long ");
            }
           
            AddMarkovChainString(Preprocess(stringBuilder.ToString(), int.MaxValue), false, null);

            /**/

            mostMarkovChainUsingStrings = GetMarkovChainString("Long", false, true);

#if DEBUG
            Console.WriteLine(mostMarkovChainUsingStrings.String + " :: " + mostMarkovChainUsingStrings.ChainCount);
#endif            
            if (!_isCaseSensitive)
                Debug.Assert(mostMarkovChainUsingStrings.String == stringBuilder.ToString().ToLowerInvariant().TrimEnd());
            else
                Debug.Assert(mostMarkovChainUsingStrings.String == stringBuilder.ToString().TrimEnd());

        }

        private void TestPerInputNodeAttributes()
        {
            _markovChainString.Children.Clear();

            string inputString = "This string contains per node attributes.";

            MarkovChainInputString markovChainInputString = Preprocess(inputString, int.MaxValue);

            foreach(MarkovChainInputNodeString markovChainInputNodeString in markovChainInputString.Nodes)
            {
                if(markovChainInputNodeString.String.Contains("i"))
                {
                    markovChainInputNodeString.Attributes.Add("perNodeAttribute", 1);
                }
            }

            Dictionary<string, double> globalAttributes = new Dictionary<string, double>();

            globalAttributes.Add("globalAttribute1", 1);
            globalAttributes.Add("globalAttribute2", 1);

            AddMarkovChainString(markovChainInputString, false, globalAttributes);

            MarkovChainString markovChainString = GetMarkovChainString(inputString, false, false);
        }

        #endregion
    }
}