using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arachnode.Utilities
{
    public class SpellCheck
    {
        private Dictionary<string, ulong> _stringsAndHashes = new Dictionary<string, ulong>();
        private Dictionary<ulong, HashSet<string>> _hashesAndStrings = new Dictionary<ulong, HashSet<string>>();

        public void AddToDictionary(string input)
        {
            if(string.IsNullOrEmpty(input))
            {
                return;
            }

            input = input.ToLowerInvariant();

            ulong hash = Strings.GenerateFuzzyHashCode(input);

            if(!_stringsAndHashes.ContainsKey(input))
            {
                _stringsAndHashes.Add(input, hash);
            }

            if(!_hashesAndStrings.ContainsKey(hash))
            {
                _hashesAndStrings.Add(hash, new HashSet<string>());
            }

            if(!_hashesAndStrings[hash].Contains(input))
            {
                _hashesAndStrings[hash].Add(input);
            }
        }

        public bool Check(string input)
        {
            return _stringsAndHashes.ContainsKey(input);
        }

        public string Suggest(string input)
        {
            if(!Check(input))
            {
                ulong hash = Strings.GenerateFuzzyHashCode(input);

                if(_hashesAndStrings.ContainsKey(hash))
                {
                    HashSet<string> hashSet = _hashesAndStrings[hash];

                    Dictionary<string, int> sortedSuggestions = new Dictionary<string, int>();

                    foreach(string suggestion in hashSet)
                    {
                        sortedSuggestions.Add(suggestion, UserDefinedFunctions.ComputeLevenstheinDistance(input, suggestion).Value);
                    }

                    return sortedSuggestions.OrderBy(ss => ss.Value).First().Key;
                }
                else
                {
                    ulong numberOfSetBits = Numbers.NumberOfSetBits(hash);

                    Dictionary<string, int> sortedSuggestions = new Dictionary<string, int>();

                    foreach (KeyValuePair<ulong, HashSet<string>> keyValuePair in _hashesAndStrings)
                    {
                        if (Numbers.NumberOfSetBits(hash & keyValuePair.Key) >= numberOfSetBits * 0.75)
                        {
                            foreach (string suggestion in keyValuePair.Value)
                            {
                                int levenstheinDistance = UserDefinedFunctions.ComputeLevenstheinDistance(input, suggestion).Value;

                                sortedSuggestions.Add(suggestion, levenstheinDistance);

                                if (levenstheinDistance <= 3)
                                {
                                    return suggestion;
                                }
                            }
                        }
                    }

                    if (sortedSuggestions.Count != 0)
                    {
                        return sortedSuggestions.OrderBy(ss => ss.Value).First().Key;
                    }
                }

                return null;
            }

            return input;
        }

        #region SpellCheck Test Code
        //Stopwatch stopwatch = new Stopwatch();
        //stopwatch.Start();

        //SpellCheck spellCheck = new SpellCheck();

        //foreach (string firstName in File.ReadAllLines(@"TextDatabases\FirstNames_Male.txt").Take(1000))
        //{
        //    foreach (string lastName in File.ReadAllLines(@"TextDatabases\FirstNames_Female.txt").Take(1000))
        //    {
        //        string fullName = (firstName + "" + lastName).ToLowerInvariant();

        //        spellCheck.AddToDictionary(fullName);
        //    }
        //}
        //System.Console.WriteLine(stopwatch.Elapsed);
        //stopwatch.Reset();
        //stopwatch.Start();

        ////there should be 100% recall...
        //foreach (string firstName in File.ReadAllLines(@"TextDatabases\FirstNames_Male.txt").Take(1000))
        //{
        //    foreach (string lastName in File.ReadAllLines(@"TextDatabases\FirstNames_Female.txt").Take(1000))
        //    {
        //        string fullName = (firstName + "" + lastName).ToLowerInvariant();

        //        Debug.Assert(spellCheck.Check(fullName));
        //    }
        //}
        //System.Console.WriteLine(stopwatch.Elapsed);
        //stopwatch.Reset();
        //stopwatch.Start();

        ////there should be 100% recall and the suggestion should be the input...
        //foreach (string firstName in File.ReadAllLines(@"TextDatabases\FirstNames_Male.txt").Take(1000))
        //{
        //    foreach (string lastName in File.ReadAllLines(@"TextDatabases\FirstNames_Female.txt").Take(1000))
        //    {
        //        string fullName = (firstName + "" + lastName).ToLowerInvariant();

        //        Debug.Assert(fullName == spellCheck.Suggest(fullName));
        //    }
        //}
        //System.Console.WriteLine(stopwatch.Elapsed);
        //stopwatch.Reset();
        //stopwatch.Start();

        //double correctTotal = 0;
        //double incorrectTotal = 0;

        ////ok, let's generate some typos...
        //double correct = 0;
        //double incorrect = 0;
        //foreach (string firstName in File.ReadAllLines(@"TextDatabases\FirstNames_Male.txt").Take(50))
        //{
        //    foreach (string lastName in File.ReadAllLines(@"TextDatabases\FirstNames_Female.txt").Take(50))
        //    {
        //        string fullName = (firstName + "" + lastName).ToLowerInvariant();

        //        string[] incorrectKeystrokeTyposSplit = UserDefinedFunctions.GenerateIncorrectKeystrokeTypos(fullName).Value.Split(' ');

        //        for (int i = 0; i < incorrectKeystrokeTyposSplit.Length; i++)
        //        {
        //            string fullNameTypo = incorrectKeystrokeTyposSplit[i];

        //            string suggestion = spellCheck.Suggest(fullNameTypo);

        //            if (fullName == suggestion)
        //            {
        //                correct++;
        //            }
        //            else
        //            {
        //                incorrect++;
        //            }
        //        }
        //    }

        //    System.Console.WriteLine("GenerateIncorrectKeystrokeTypos: Total: " + (correct + incorrect));
        //    System.Console.WriteLine("Accuracy: " + correct / (correct + incorrect) + "%");
        //}
        //System.Console.WriteLine(stopwatch.Elapsed);
        //stopwatch.Reset();
        //stopwatch.Start();
        //System.Console.ReadLine();

        ////ok, let's generate some typos...
        //correctTotal += correct;
        //incorrectTotal += incorrect;
        //correct = 0;
        //incorrect = 0;
        //foreach (string firstName in File.ReadAllLines(@"TextDatabases\FirstNames_Male.txt").Take(50))
        //{
        //    foreach (string lastName in File.ReadAllLines(@"TextDatabases\FirstNames_Female.txt").Take(50))
        //    {
        //        string fullName = (firstName + "" + lastName).ToLowerInvariant();

        //        string[] missedKeystrokeTyposSplit = UserDefinedFunctions.GenerateMissedKeystrokeTypos(fullName, 1).Value.Split(' ');

        //        for (int i = 0; i < missedKeystrokeTyposSplit.Length; i++)
        //        {
        //            string fullNameTypo = missedKeystrokeTyposSplit[i];

        //            string suggestion = spellCheck.Suggest(fullNameTypo);

        //            if (fullName == suggestion)
        //            {
        //                correct++;
        //            }
        //            else
        //            {
        //                incorrect++;
        //            }
        //        }
        //    }

        //    System.Console.WriteLine("GenerateMissedKeystrokeTypos: Total: " + (correct + incorrect));
        //    System.Console.WriteLine("Accuracy: " + correct / (correct + incorrect) + "%");
        //}
        //System.Console.WriteLine(stopwatch.Elapsed);
        //stopwatch.Reset();
        //stopwatch.Start();
        //System.Console.ReadLine();

        ////ok, let's generate some typos...
        //correctTotal += correct;
        //incorrectTotal += incorrect;
        //correct = 0;
        //incorrect = 0;
        //foreach (string firstName in File.ReadAllLines(@"TextDatabases\FirstNames_Male.txt").Take(50))
        //{
        //    foreach (string lastName in File.ReadAllLines(@"TextDatabases\FirstNames_Female.txt").Take(50))
        //    {
        //        string fullName = (firstName + "" + lastName).ToLowerInvariant();

        //        string[] repeatedKeystrokeTyposSplit = UserDefinedFunctions.GenerateRepeatedKeystrokeTypos(fullName, 1).Value.Split(' ');

        //        for (int i = 0; i < repeatedKeystrokeTyposSplit.Length; i++)
        //        {
        //            string fullNameTypo = repeatedKeystrokeTyposSplit[i];

        //            string suggestion = spellCheck.Suggest(fullNameTypo);

        //            if (fullName == suggestion)
        //            {
        //                correct++;
        //            }
        //            else
        //            {
        //                incorrect++;
        //            }
        //        }
        //    }

        //    System.Console.WriteLine("GenerateMissedKeystrokeTypos: Total: " + (correct + incorrect));
        //    System.Console.WriteLine("Accuracy: " + correct / (correct + incorrect) + "%");
        //}
        //System.Console.WriteLine(stopwatch.Elapsed);
        //stopwatch.Reset();
        //stopwatch.Start();
        //System.Console.ReadLine();

        ////ok, let's generate some typos...
        //correctTotal += correct;
        //incorrectTotal += incorrect;
        //correct = 0;
        //incorrect = 0;
        //foreach (string firstName in File.ReadAllLines(@"TextDatabases\FirstNames_Male.txt").Take(50))
        //{
        //    foreach (string lastName in File.ReadAllLines(@"TextDatabases\FirstNames_Female.txt").Take(50))
        //    {
        //        string fullName = (firstName + "" + lastName).ToLowerInvariant();

        //        string[] transposedKeystrokeTyposSplit = UserDefinedFunctions.GenerateTransposedKeystrokeTypos(fullName, 1).Value.Split(' ');

        //        for (int i = 0; i < transposedKeystrokeTyposSplit.Length; i++)
        //        {
        //            string fullNameTypo = transposedKeystrokeTyposSplit[i];

        //            string suggestion = spellCheck.Suggest(fullNameTypo);

        //            if (fullName == suggestion)
        //            {
        //                correct++;
        //            }
        //            else
        //            {
        //                incorrect++;
        //            }
        //        }
        //    }

        //    System.Console.WriteLine("GenerateTransposedKeystrokeTypos: Total: " + (correct + incorrect));
        //    System.Console.WriteLine("Accuracy: " + correct / (correct + incorrect) + "%");
        //}
        //System.Console.WriteLine(stopwatch.Elapsed);
        //stopwatch.Reset();
        //stopwatch.Start();

        //System.Console.WriteLine("Final: Total: " + (correctTotal + incorrectTotal));
        //System.Console.WriteLine("Accuracy: " + correctTotal / (correctTotal + incorrectTotal) + "%");

        //System.Console.ReadLine();
        #endregion
    }
}
