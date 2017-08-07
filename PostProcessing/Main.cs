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
using System.Threading;
using System.Windows.Forms;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataSource;
using Arachnode.SiteCrawler;
using Arachnode.SiteCrawler.Utilities;
using Arachnode.SiteCrawler.Value.Enums;

#endregion

namespace Arachnode.PostProcessing
{
    public partial class Main : Form
    {
        private readonly ApplicationSettings _applicationSettings;
        private readonly WebSettings _webSettings;

        public Main()
        {
            InitializeComponent();

            _applicationSettings = new ApplicationSettings();
            _webSettings = new WebSettings();

            WebPageUtilities<ArachnodeDAO>.OnWebPageProcessed += WebPageUtilities_OnWebPageProcessed;
            FileUtilities<ArachnodeDAO>.OnFileProcessed += FileUtilities_OnFileProcessed;
            ImageUtilities<ArachnodeDAO>.OnImageProcessed += ImageUtilities_OnImageProcessed;
        }

        private void SetCrawlActionsCrawlRulesEngineActionsAndApplicationSettings(Crawler<ArachnodeDAO> crawler)
        {
            //CrawlActions, CrawlRules and EngineActions can be set from code, overriding Database settings.

            //cfg.CrawlActions
            foreach (var crawlAction in crawler.CrawlActions.Values)
            {
                if (crawlAction.TypeName != "Arachnode.Plugins.CrawlActions.ManageLuceneDotNetIndexes")
                {
                    crawlAction.IsEnabled = false;
                }
            }

            //cfg.CrawlRules
            foreach (var crawlRule in crawler.CrawlRules.Values)
            {
                crawlRule.IsEnabled = false;
            }

            //cfg.EngineActions
            foreach (var engineAction in crawler.Engine.EngineActions.Values)
            {
                engineAction.IsEnabled = false;
            }

            //_applicationSettings can be set from code, overriding Database settings found in cfg.Configuration.
            _applicationSettings.AssignCrawlRequestPrioritiesForFiles = true;
            _applicationSettings.AssignCrawlRequestPrioritiesForHyperLinks = true;
            _applicationSettings.AssignCrawlRequestPrioritiesForImages = true;
            _applicationSettings.AssignCrawlRequestPrioritiesForWebPages = true;
            _applicationSettings.AssignEmailAddressDiscoveries = false;
            _applicationSettings.AssignFileAndImageDiscoveries = true;
            _applicationSettings.AssignHyperLinkDiscoveries = true;
            _applicationSettings.ClassifyAbsoluteUris = true;
            //_applicationSettings.ConnectionString = "";
            //_applicationSettings.ConsoleOutputLogsDirectory = "";
            _applicationSettings.CrawlRequestTimeoutInMinutes = 1;
            _applicationSettings.CreateCrawlRequestsFromDatabaseCrawlRequests = true;
            _applicationSettings.CreateCrawlRequestsFromDatabaseFiles = false;
            _applicationSettings.CreateCrawlRequestsFromDatabaseHyperLinks = false;
            _applicationSettings.CreateCrawlRequestsFromDatabaseImages = false;
            _applicationSettings.CreateCrawlRequestsFromDatabaseWebPages = false;
            _applicationSettings.DesiredMaximumMemoryUsageInMegabytes = 1024;
            //_applicationSettings.DownloadedFilesDirectory = "";
            //_applicationSettings.DownloadedImagesDirectory = "";
            //_applicationSettings.DownloadedWebPagesDirectory = "";
            _applicationSettings.EnableConsoleOutput = true;
            _applicationSettings.ExtractFileMetaData = true;
            _applicationSettings.ExtractImageMetaData = true;
            _applicationSettings.ExtractWebPageMetaData = true;
            _applicationSettings.HttpWebRequestRetries = 5;
            _applicationSettings.InsertDisallowedAbsoluteUriDiscoveries = true;
            _applicationSettings.InsertDisallowedAbsoluteUris = true;
            _applicationSettings.InsertEmailAddressDiscoveries = true;
            _applicationSettings.InsertEmailAddresses = true;
            _applicationSettings.InsertExceptions = true;
            _applicationSettings.InsertFileDiscoveries = true;
            _applicationSettings.InsertFileMetaData = true;
            _applicationSettings.InsertFiles = true;
            _applicationSettings.InsertFileSource = false;
            _applicationSettings.InsertHyperLinkDiscoveries = true;
            _applicationSettings.InsertHyperLinks = true;
            _applicationSettings.InsertImageDiscoveries = true;
            _applicationSettings.InsertImageMetaData = true;
            _applicationSettings.InsertImages = true;
            _applicationSettings.InsertImageSource = false;
            _applicationSettings.InsertWebPageMetaData = true;
            _applicationSettings.InsertWebPages = true;
            _applicationSettings.InsertWebPageSource = false;
            _applicationSettings.MaximumNumberOfAutoRedirects = 50;
            _applicationSettings.MaximumNumberOfCrawlRequestsToCreatePerBatch = 1000;
            _applicationSettings.MaximumNumberOfCrawlThreads = 10;
            _applicationSettings.MaximumNumberOfHostsAndPrioritiesToSelect = 10000;
            _applicationSettings.OutputConsoleToLogs = false;
            _applicationSettings.SaveDiscoveredFilesToDisk = true;
            _applicationSettings.SaveDiscoveredImagesToDisk = true;
            _applicationSettings.SaveDiscoveredWebPagesToDisk = true;
            _applicationSettings.UserAgent = "Your unique UserAgent string.";
            _applicationSettings.VerboseOutput = false;

            crawler.ApplicationSettings = _applicationSettings;
        }

        private void btnProcessWebPages_Click(object sender, EventArgs e)
        {
            btnProcessWebPages.Enabled = false;

            var thread = new Thread(delegate()
                                        {
                                            ArachnodeDAO arachnodeDAO = new ArachnodeDAO(_applicationSettings.ConnectionString, _applicationSettings, _webSettings, true, true);

                                            //used to load the CrawlActions, CrawlRules and EngineActions...
                                            var crawler = new Crawler<ArachnodeDAO>(_applicationSettings, _webSettings, CrawlMode.BreadthFirstByPriority, false);

                                            SetCrawlActionsCrawlRulesEngineActionsAndApplicationSettings(crawler);

                                            WebPageUtilities<ArachnodeDAO>.ProcessWebPages(crawler, (long) nudWebPageIDLowerBound.Value, (long) nudWebPageIDUpperBound.Value);

                                            BeginInvoke(new MethodInvoker(delegate { btnProcessWebPages.Enabled = true; }));
                                        }
                );

            thread.Start();
        }

        private void btnProcessFiles_Click(object sender, EventArgs e)
        {
            btnProcessFiles.Enabled = false;

            var thread = new Thread(delegate()
                                        {
                                            ArachnodeDAO arachnodeDAO = new ArachnodeDAO(_applicationSettings.ConnectionString, _applicationSettings, _webSettings, true, true);

                                            //used to load the CrawlActions, CrawlRules and EngineActions...
                                            var crawler = new Crawler<ArachnodeDAO>(_applicationSettings, _webSettings, CrawlMode.BreadthFirstByPriority, false);

                                            SetCrawlActionsCrawlRulesEngineActionsAndApplicationSettings(crawler);

                                            FileUtilities<ArachnodeDAO>.ProcessFiles(crawler, (long) nudFileIDLowerBound.Value, (long) nudFileIDUpperBound.Value);

                                            BeginInvoke(new MethodInvoker(delegate { btnProcessFiles.Enabled = true; }));
                                        }
                );

            thread.Start();
        }

        private void btnProcessImages_Click(object sender, EventArgs e)
        {
            btnProcessImages.Enabled = false;

            var thread = new Thread(delegate()
                                        {
                                            ArachnodeDAO arachnodeDAO = new ArachnodeDAO(_applicationSettings.ConnectionString, _applicationSettings, _webSettings, true, true);

                                            //used to load the CrawlActions, CrawlRules and EngineActions...
                                            var crawler = new Crawler<ArachnodeDAO>(_applicationSettings, _webSettings, CrawlMode.BreadthFirstByPriority, false);

                                            SetCrawlActionsCrawlRulesEngineActionsAndApplicationSettings(crawler);

                                            ImageUtilities<ArachnodeDAO>.ProcessImages(crawler, (long) nudImagesIDLowerBound.Value, (long) nudImagesIDUpperBound.Value);

                                            BeginInvoke(new MethodInvoker(delegate { btnProcessImages.Enabled = true; }));
                                        }
                );

            thread.Start();
        }

        private void ImageUtilities_OnImageProcessed(ArachnodeDataSet.ImagesRow imagesRow, string message)
        {
            BeginInvoke(new MethodInvoker(delegate
                                              {
                                                  rtbPostProcessingStatus.Text = message + Environment.NewLine + rtbPostProcessingStatus.Text;

                                                  if (rtbPostProcessingStatus.Text.Length > 10000)
                                                  {
                                                      rtbPostProcessingStatus.Text = rtbPostProcessingStatus.Text.Substring(0, 10000);
                                                  }
                                              }));

            //Application.DoEvents();

            //Thread.Sleep(100);
        }

        private void FileUtilities_OnFileProcessed(ArachnodeDataSet.FilesRow filesRow, string message)
        {
            BeginInvoke(new MethodInvoker(delegate
                                              {
                                                  rtbPostProcessingStatus.Text = message + Environment.NewLine + rtbPostProcessingStatus.Text;

                                                  if (rtbPostProcessingStatus.Text.Length > 10000)
                                                  {
                                                      rtbPostProcessingStatus.Text = rtbPostProcessingStatus.Text.Substring(0, 10000);
                                                  }
                                              }));

            //Application.DoEvents();

            //Thread.Sleep(100);
        }

        private void WebPageUtilities_OnWebPageProcessed(ArachnodeDataSet.WebPagesRow webPagesRow, string message)
        {
            BeginInvoke(new MethodInvoker(delegate
                                              {
                                                  rtbPostProcessingStatus.Text = message + Environment.NewLine + rtbPostProcessingStatus.Text;

                                                  if (rtbPostProcessingStatus.Text.Length > 10000)
                                                  {
                                                      rtbPostProcessingStatus.Text = rtbPostProcessingStatus.Text.Substring(0, 10000);
                                                  }
                                              }));

            //Application.DoEvents();

            //Thread.Sleep(100);
        }
    }
}