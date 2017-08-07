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
using System.Diagnostics;
using System.IO;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.AbstractClasses;
using Arachnode.SiteCrawler.Value.Enums;
using NClassifier;

#endregion

namespace Arachnode.Plugins.CrawlActions
{
    public class BayesianClassifier<TArachnodeDAO> : ACrawlAction<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        private readonly NClassifier.Bayesian.BayesianClassifier _class1BayesianClassifierFalsePositive = new NClassifier.Bayesian.BayesianClassifier();
        private readonly NClassifier.Bayesian.BayesianClassifier _class2BayesianClassifierFalsePositive = new NClassifier.Bayesian.BayesianClassifier();
        private NClassifier.Bayesian.BayesianClassifier _class1BayesianClassifier = new NClassifier.Bayesian.BayesianClassifier();
        private NClassifier.Bayesian.BayesianClassifier _class2BayesianClassifier = new NClassifier.Bayesian.BayesianClassifier();

        public BayesianClassifier(ApplicationSettings applicationSettings, WebSettings webSettings)
            : base(applicationSettings, webSettings)
        {
        }

        /// <summary>
        /// 	Assigns the additional parameters.
        /// </summary>
        /// <param name = "settings"></param>
        public override void AssignSettings(Dictionary<string, string> settings)
        {
            //create the Class1 classifier.
            foreach (string class1Exemplar in Directory.GetFiles(settings["Class1ExemplarDirectory"]))
            {
                using (StreamReader streamReader = File.OpenText(class1Exemplar))
                {
                    foreach (string sentence in NClassifier.Utilities.GetSentences(streamReader.ReadToEnd()))
                    {
                        _class1BayesianClassifier.TeachMatch(sentence);
                        _class2BayesianClassifier.TeachNonMatch(sentence);
                    }
                }
            }

            //create the Class2 classifier.
            foreach (string class2Exemplar in Directory.GetFiles(settings["Class2ExemplarDirectory"]))
            {
                using (StreamReader streamReader = File.OpenText(class2Exemplar))
                {
                    foreach (string sentence in NClassifier.Utilities.GetSentences(streamReader.ReadToEnd()))
                    {
                        _class1BayesianClassifier.TeachNonMatch(sentence);
                        _class2BayesianClassifier.TeachMatch(sentence);
                    }
                }
            }

            HashSet<string> falsePostiveSentences = new HashSet<string>();

            double numberOfExemplars = 0;
            double numberOfFalsePostives = 0;
            //int maximumIterations = 10;

            while (numberOfExemplars == 0 || numberOfExemplars/(numberOfExemplars + numberOfFalsePostives) <= 0.99)
            {
                numberOfExemplars = 0;
                numberOfFalsePostives = 0;

                //validate the classifiers.
                NClassifier.Bayesian.BayesianClassifier class1BayesianClassifier2 = new NClassifier.Bayesian.BayesianClassifier();
                NClassifier.Bayesian.BayesianClassifier class2BayesianClassifier2 = new NClassifier.Bayesian.BayesianClassifier();

                //validate the Class1 classifier.
                foreach (string class1Exemplar in Directory.GetFiles(settings["Class1ExemplarDirectory"]))
                {
                    using (StreamReader streamReader = File.OpenText(class1Exemplar))
                    {
                        foreach (string sentence in NClassifier.Utilities.GetSentences(streamReader.ReadToEnd()))
                        {
                            if (!falsePostiveSentences.Contains(sentence))
                            {
                                numberOfExemplars++;

                                if (DetermineClass(_class1BayesianClassifier.Classify(sentence), _class2BayesianClassifier.Classify(sentence), false) == 1)
                                {
                                    class1BayesianClassifier2.TeachMatch(sentence);
                                    class2BayesianClassifier2.TeachNonMatch(sentence);
                                }
                                else
                                {
                                    falsePostiveSentences.Add(sentence);

                                    numberOfFalsePostives++;

                                    _class1BayesianClassifierFalsePositive.TeachMatch(sentence);
                                    _class2BayesianClassifierFalsePositive.TeachNonMatch(sentence);
                                }
                            }
                        }
                    }
                }

                //validate the Class2 classifier.
                foreach (string class2Exemplar in Directory.GetFiles(settings["Class2ExemplarDirectory"]))
                {
                    using (StreamReader streamReader = File.OpenText(class2Exemplar))
                    {
                        foreach (string sentence in NClassifier.Utilities.GetSentences(streamReader.ReadToEnd()))
                        {
                            if (!falsePostiveSentences.Contains(sentence))
                            {
                                numberOfExemplars++;

                                if (DetermineClass(_class1BayesianClassifier.Classify(sentence), _class2BayesianClassifier.Classify(sentence), false) == 2)
                                {
                                    class1BayesianClassifier2.TeachNonMatch(sentence);
                                    class2BayesianClassifier2.TeachMatch(sentence);
                                }
                                else
                                {
                                    falsePostiveSentences.Add(sentence);

                                    numberOfFalsePostives++;

                                    _class1BayesianClassifierFalsePositive.TeachNonMatch(sentence);
                                    _class2BayesianClassifierFalsePositive.TeachMatch(sentence);
                                }
                            }
                        }
                    }
                }

                _class1BayesianClassifier = class1BayesianClassifier2;
                _class2BayesianClassifier = class2BayesianClassifier2;

                double dingle = numberOfExemplars/(numberOfExemplars + numberOfFalsePostives);
            }

            double oneClassify = _class1BayesianClassifier.Classify("left-wing");
            double oneClassifyFalsePositive = _class1BayesianClassifierFalsePositive.Classify("left-wing");

            double twoClassify = _class2BayesianClassifier.Classify("left-wing");
            double twoClassifyFalsePositive = _class2BayesianClassifierFalsePositive.Classify("left-wing");

            Debug.Assert(oneClassify > oneClassifyFalsePositive); //false positive detection.
            Debug.Assert(twoClassify < twoClassifyFalsePositive); //false positive detection.
            Debug.Assert(oneClassify > twoClassify); //main classification.

            /**/

            double oneClassify2 = _class1BayesianClassifier.Classify("right-wing");
            double oneClassifyFalsePositive2 = _class1BayesianClassifierFalsePositive.Classify("right-wing");

            double twoClassify2 = _class2BayesianClassifier.Classify("right-wing");
            double twoClassifyFalsePositive2 = _class2BayesianClassifierFalsePositive.Classify("right-wing");

            Debug.Assert(oneClassify2 < oneClassifyFalsePositive2); //false positive detection.
            Debug.Assert(twoClassify2 > twoClassifyFalsePositive2); //false positive detection.
            Debug.Assert(oneClassify2 < twoClassify2); //main classification.
        }

        /// <summary>
        /// 	Performs the action.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        public override void PerformAction(CrawlRequest<TArachnodeDAO> crawlRequest, IArachnodeDAO arachnodeDAO)
        {
            if (crawlRequest.IsDisallowed || !crawlRequest.ProcessData || crawlRequest.WebClient.WebException != null)
            {
                return;
            }

            if (crawlRequest.DataType.DiscoveryType == DiscoveryType.WebPage)
            {
                if (crawlRequest.Data != null)
                {
                    //uncomment to see what was analyzed...
                    string text = UserDefinedFunctions.ExtractText(crawlRequest.DecodedHtml).Value;

                    double class1Classification = _class1BayesianClassifier.Classify(crawlRequest.DecodedHtml);
                    double class2Classification = _class2BayesianClassifier.Classify(crawlRequest.DecodedHtml);

                    byte @class = DetermineClass(class1Classification, class2Classification, true);

                    double class1ClassificationFalsePositive = _class1BayesianClassifierFalsePositive.Classify(crawlRequest.DecodedHtml);
                    double class2ClassificationFalsePositive = _class2BayesianClassifierFalsePositive.Classify(crawlRequest.DecodedHtml);

                    switch (@class)
                    {
                        case 1:
                            if (class1ClassificationFalsePositive >= class1Classification)
                            {
                                @class = 0;
                            }
                            else
                            {
                            }
                            break;
                        case 2:
                            if (class2ClassificationFalsePositive >= class2Classification)
                            {
                                @class = 0;
                            }
                            else
                            {
                            }
                            break;
                    }
                }
            }
        }

        private byte DetermineClass(double classification1, double classification2, bool checkForFalsePositive)
        {
            if (classification1 > classification2 && classification1 > 0.75)
            {
                //Class1Match
                return 1;
            }
            if (classification2 > classification1 && classification2 > 0.75)
            {
                //Class2Match
                return 2;
            }
            else
            {
                //no match
                return 0;
            }
        }

        /// <summary>
        /// 	Stops this instance.
        /// </summary>
        public override void Stop()
        {
        }
    }
}