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
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.GraphicalUserInterface;
using Arachnode.Renderer.Value.Enums;
using Arachnode.SiteCrawler;
using Arachnode.SiteCrawler.Core;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.Enums;
using Arachnode.Utilities;
using Timer = System.Windows.Forms.Timer;

#endregion

namespace GraphicalUserInterface
{
    public partial class frmMain : Form
    {
        private readonly Stopwatch _stopwatchForApplication = new Stopwatch();
        private readonly Stopwatch _stopwatchForCrawler = new Stopwatch();
        private readonly Timer _timer = new Timer();
        private readonly object _timerLock = new object();

        private ApplicationSettings _applicationSettings;
        private WebSettings _webSettings;

        private ArachnodeDAO _arachnodeDAO;
        private ArachnodeDataSourceDataContext _arachnodeDataSourceDataContext;
        private Crawler<ArachnodeDAO> _crawler;

        private readonly Dictionary<int, PerformanceCounter> _arachnodeDAOCounters = new Dictionary<int, PerformanceCounter>();
        private readonly Dictionary<int, PerformanceCounter> _cacheCounters = new Dictionary<int, PerformanceCounter>();
        private readonly Dictionary<int, PerformanceCounter> _crawlCounters = new Dictionary<int, PerformanceCounter>();
        private readonly Dictionary<int, PerformanceCounter> _crawlerPeerCounters = new Dictionary<int, PerformanceCounter>();

        private string _formText;
        private ConsoleOutToQueue _consoleOutToQueue;

        public frmMain()
        {
            _stopwatchForApplication.Start();

            InitializeComponent();

            _formText = Text;

            _timer.Enabled = true;
            _timer.Interval = 1000;
            _timer.Tick += _timer_Tick;
        }

        private void Form1_Activated(object sender, EventArgs e)
        {

        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            ResetGUI();
        }

        private void ResetGUI()
        {
            try
            {
                var thread = new Thread(() =>
                                            {
                                                _timer.Start();
                                                _stopwatchForCrawler.Reset();

                                                BeginInvoke(new MethodInvoker(delegate
                                                                                  {
                                                                                      SuspendLayout();

                                                                                      rtbOutput.Clear();
                                                                                      tcMain.SelectedTab = tpCrawler;

                                                                                      ResumeLayout();
                                                                                  }));

                                                _consoleOutToQueue = new ConsoleOutToQueue(true, 75);
                                                Console.SetOut(_consoleOutToQueue);

                                                _applicationSettings = new ApplicationSettings();
                                                _webSettings = new WebSettings();

                                                _arachnodeDAO = new ArachnodeDAO(_applicationSettings.ConnectionString, _applicationSettings, _webSettings, true, true);
                                                _arachnodeDataSourceDataContext = new ArachnodeDataSourceDataContext(_applicationSettings.ConnectionString);

                                                //unwire existing crawlers...
                                                if(_crawler != null)
                                                {
                                                    _crawler.Engine.CrawlRequestCompleted -= Engine_CrawlRequestCompleted;
                                                    _crawler.Engine.CrawlCompleted -= Engine_CrawlCompleted;
                                                    _crawler.Engine.CrawlRequestThrottled -= Engine_CrawlRequestThrottled;
                                                }

                                                //instantiate the crawler...
                                                _crawler = new Crawler<ArachnodeDAO>(_applicationSettings, _webSettings, CrawlMode.BreadthFirstByPriority, false);

                                                _crawler.Engine.CrawlRequestCompleted += Engine_CrawlRequestCompleted;
                                                _crawler.Engine.CrawlCompleted += Engine_CrawlCompleted;
                                                _crawler.Engine.CrawlRequestThrottled += Engine_CrawlRequestThrottled;

                                                //CrawlActions, CrawlRules and EngineActions can be set from code, overriding Database settings.

                                                //cfg.CrawlActions
                                                foreach (var crawlAction in _crawler.CrawlActions.Values)
                                                {
                                                    if (crawlAction.TypeName != "Arachnode.Plugins.CrawlActions.ManageLuceneDotNetIndexes")
                                                    {
                                                        crawlAction.IsEnabled = false;
                                                    }
                                                }

                                                //cfg.CrawlRules
                                                foreach (var crawlRule in _crawler.CrawlRules.Values)
                                                {
                                                    if (crawlRule.TypeName != "Arachnode.Plugins.CrawlRules.AbsoluteUri")
                                                    {
                                                        crawlRule.IsEnabled = false;
                                                    }
                                                }

                                                //cfg.EngineActions
                                                foreach (var engineAction in _crawler.Engine.EngineActions.Values)
                                                {
                                                    engineAction.IsEnabled = false;
                                                }

                                                //_applicationSettings can be set from code, overriding Database settings found in cfg.Configuration.
                                                _applicationSettings.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                                                _applicationSettings.AcceptEncoding = "gzip,deflate,sdch";
                                                _applicationSettings.AllowAutoRedirect = true;
                                                _applicationSettings.AssignCrawlRequestPrioritiesForFiles = true;
                                                _applicationSettings.AssignCrawlRequestPrioritiesForHyperLinks = true;
                                                _applicationSettings.AssignCrawlRequestPrioritiesForImages = true;
                                                _applicationSettings.AssignCrawlRequestPrioritiesForWebPages = true;
                                                _applicationSettings.AssignEmailAddressDiscoveries = false;
                                                _applicationSettings.AssignFileAndImageDiscoveries = true;
                                                _applicationSettings.AssignHyperLinkDiscoveries = true;
                                                _applicationSettings.AutoThrottleHttpWebRequests = false;
                                                _applicationSettings.ClassifyAbsoluteUris = false;
                                                //_applicationSettings.ConnectionString = "";
                                                //_applicationSettings.ConsoleOutputLogsDirectory = "";
                                                _applicationSettings.CrawlRequestTimeoutInMinutes = 1;
                                                _applicationSettings.CreateCrawlRequestsFromDatabaseCrawlRequests = true;
                                                _applicationSettings.CreateCrawlRequestsFromDatabaseFiles = false;
                                                _applicationSettings.CreateCrawlRequestsFromDatabaseHyperLinks = false;
                                                _applicationSettings.CreateCrawlRequestsFromDatabaseImages = false;
                                                _applicationSettings.CreateCrawlRequestsFromDatabaseWebPages = false;
                                                _applicationSettings.DesiredMaximumMemoryUsageInMegabytes = 4096;
                                                _applicationSettings.DiscoverySlidingExpirationInSeconds = 120;
                                                //_applicationSettings.DownloadedFilesDirectory = "";
                                                //_applicationSettings.DownloadedImagesDirectory = "";
                                                //_applicationSettings.DownloadedWebPagesDirectory = "";
                                                _applicationSettings.EnableConsoleOutput = true;
                                                _applicationSettings.ExtractFileMetaData = false;
                                                _applicationSettings.ExtractImageMetaData = false;
                                                _applicationSettings.ExtractWebPageMetaData = false;
                                                _applicationSettings.HttpWebRequestRetries = 5;
                                                _applicationSettings.InsertCrawlRequests = true;
                                                _applicationSettings.InsertDisallowedAbsoluteUriDiscoveries = false;
                                                _applicationSettings.InsertDisallowedAbsoluteUris = true;
                                                _applicationSettings.InsertDisallowedDiscoveries = false;
                                                _applicationSettings.InsertDiscoveries = true;
                                                _applicationSettings.InsertEmailAddressDiscoveries = false;
                                                _applicationSettings.InsertEmailAddresses = false;
                                                _applicationSettings.InsertExceptions = true;
                                                _applicationSettings.InsertFileDiscoveries = true;
                                                _applicationSettings.InsertFileMetaData = false;
                                                _applicationSettings.InsertFiles = true;
                                                _applicationSettings.InsertFileResponseHeaders = true;
                                                _applicationSettings.InsertFileSource = false;
                                                _applicationSettings.InsertHyperLinkDiscoveries = false;
                                                _applicationSettings.InsertHyperLinks = true;
                                                _applicationSettings.InsertImageDiscoveries = true;
                                                _applicationSettings.InsertImageMetaData = false;
                                                _applicationSettings.InsertImages = true;
                                                _applicationSettings.InsertImageResponseHeaders = true;
                                                _applicationSettings.InsertImageSource = false;
                                                _applicationSettings.InsertWebPageMetaData = false;
                                                _applicationSettings.InsertWebPages = true;
                                                _applicationSettings.InsertWebPageResponseHeaders = true;
                                                _applicationSettings.InsertWebPageSource = false;
                                                _applicationSettings.MaximumNumberOfAutoRedirects = 50;
                                                _applicationSettings.MaximumNumberOfCrawlRequestsToCreatePerBatch = 1000;
                                                _applicationSettings.MaximumNumberOfCrawlThreads = 10;// _crawler.ProxyManager.Proxies.Count != 0 ? _crawler.ProxyManager.Proxies.Count : 10;
                                                _applicationSettings.MaximumNumberOfHostsAndPrioritiesToSelect = 10000;
                                                _applicationSettings.OutputConsoleToLogs = false;
                                                _applicationSettings.OutputWebExceptions = true;
                                                _applicationSettings.ProcessCookies = true;
                                                _applicationSettings.ProcessDiscoveriesAsynchronously = false;
                                                _applicationSettings.SaveDiscoveredFilesToDisk = true;
                                                _applicationSettings.SaveDiscoveredImagesToDisk = true;
                                                _applicationSettings.SaveDiscoveredWebPagesToDisk = true;
                                                _applicationSettings.SetRefererToParentAbsoluteUri = true;
                                                _applicationSettings.UniqueIdentifier = "";
                                                _applicationSettings.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/534.24 (KHTML, like Gecko) Chrome/11.0.696.71 Safari/534.24"; //If you find yourself blocked from crawling a website, change this to a common crawler string, such as 'Googlebot' or 'Slurp'...
                                                _applicationSettings.VerboseOutput = false;
                                                //enable VerboseOutput to see each Discovery and the the status of each Discovery returned from each Discovery.  (e.g. WebPages from each WebPage and Files/Images from each WebPage.)

                                                _crawler.ApplicationSettings = _applicationSettings;

                                                while (!Created)
                                                {
                                                    Application.DoEvents();

                                                    Thread.Sleep(1000);
                                                }

                                                BeginInvoke(new MethodInvoker(delegate
                                                                                  {
                                                                                      dgvCrawlRequests.Rows.Clear();

                                                                                      //load application settings...
                                                                                      dgvApplicationSettings.Rows.Clear();
                                                                                      foreach (PropertyInfo propertyInfo in _applicationSettings.GetType().GetProperties().OrderBy(pi => pi.Name))
                                                                                      {
                                                                                          object value = propertyInfo.GetValue(_applicationSettings, null);

                                                                                          if (propertyInfo.PropertyType.FullName == "System.Boolean")
                                                                                          {
                                                                                              var dataGridViewTextBoxCell = new DataGridViewTextBoxCell();
                                                                                              dataGridViewTextBoxCell.Value = propertyInfo.Name;

                                                                                              var dataGridViewComboBoxCell = new DataGridViewComboBoxCell();
                                                                                              dataGridViewComboBoxCell.FlatStyle = FlatStyle.Flat;
                                                                                              dataGridViewComboBoxCell.Items.Add(true);
                                                                                              dataGridViewComboBoxCell.Items.Add(false);
                                                                                              dataGridViewComboBoxCell.Value = value;

                                                                                              var dataGridViewRow = new DataGridViewRow();
                                                                                              dataGridViewRow.Cells.Add(dataGridViewTextBoxCell);
                                                                                              dataGridViewRow.Cells.Add(dataGridViewComboBoxCell);

                                                                                              dgvApplicationSettings.Rows.Add(dataGridViewRow);
                                                                                          }
                                                                                          else
                                                                                          {
                                                                                              dgvApplicationSettings.Rows.Add(propertyInfo.Name, value);
                                                                                          }
                                                                                      }

                                                                                      //load web settings...
                                                                                      dgvWebSettings.Rows.Clear();
                                                                                      foreach (PropertyInfo propertyInfo in _webSettings.GetType().GetProperties().OrderBy(pi => pi.Name))
                                                                                      {
                                                                                          object value = propertyInfo.GetValue(_webSettings, null);

                                                                                          if (propertyInfo.PropertyType.FullName == "System.Boolean")
                                                                                          {
                                                                                              var dataGridViewTextBoxCell = new DataGridViewTextBoxCell();
                                                                                              dataGridViewTextBoxCell.Value = propertyInfo.Name;

                                                                                              var dataGridViewComboBoxCell = new DataGridViewComboBoxCell();
                                                                                              dataGridViewComboBoxCell.FlatStyle = FlatStyle.Flat;
                                                                                              dataGridViewComboBoxCell.Items.Add(true);
                                                                                              dataGridViewComboBoxCell.Items.Add(false);
                                                                                              dataGridViewComboBoxCell.Value = value;

                                                                                              var dataGridViewRow = new DataGridViewRow();
                                                                                              dataGridViewRow.Cells.Add(dataGridViewTextBoxCell);
                                                                                              dataGridViewRow.Cells.Add(dataGridViewComboBoxCell);

                                                                                              dgvWebSettings.Rows.Add(dataGridViewRow);
                                                                                          }
                                                                                          else
                                                                                          {
                                                                                              dgvWebSettings.Rows.Add(propertyInfo.Name, value);
                                                                                          }
                                                                                      }

                                                                                      //load configuration data...
                                                                                      dgvTableName.Rows.Clear();
                                                                                      dgvConfiguration.Rows.Clear();
                                                                                      foreach (PropertyInfo propertyInfo in _arachnodeDataSourceDataContext.GetType().GetProperties().OrderBy(pi => pi.Name))
                                                                                      {
                                                                                          if (propertyInfo.PropertyType.FullName.StartsWith("System.Data.Linq.Table"))
                                                                                          {
                                                                                              dgvTableName.Rows.Add(propertyInfo.Name);
                                                                                          }
                                                                                      }

                                                                                      dgvTableName.Sort(dgvTableName.Columns[0], ListSortDirection.Ascending);
                                                                                      dgvTableName.Rows[0].Selected = true;

                                                                                      //reset the crawls...
                                                                                      dgvCrawler.Rows.Clear();

                                                                                      //reset the crawl info...
                                                                                      dgvCrawlInfo.Rows.Clear();

                                                                                      //initialize performance counters...
                                                                                      _arachnodeDAOCounters.Clear();
                                                                                      _cacheCounters.Clear();
                                                                                      _crawlCounters.Clear();
                                                                                      _crawlerPeerCounters.Clear();
                                                                                      dgvPerformanceCounters.Rows.Clear();
                                                                                      foreach (PerformanceCounterCategory performanceCounterCategory in PerformanceCounterCategory.GetCategories())
                                                                                      {
                                                                                          if (performanceCounterCategory.CategoryName == "arachnode.net:ArachnodeDAO")
                                                                                          {
                                                                                              foreach (PerformanceCounter performanceCounter in performanceCounterCategory.GetCounters())
                                                                                              {
                                                                                                  int index = dgvPerformanceCounters.Rows.Add(performanceCounter.CategoryName.Replace("arachnode.net:", string.Empty), performanceCounter.CounterName.Replace("arachnode.net - ", string.Empty), 0, 0, 0, 0);

                                                                                                  _arachnodeDAOCounters.Add(index, performanceCounter);
                                                                                              }
                                                                                          }

                                                                                          if (performanceCounterCategory.CategoryName == "arachnode.net:Cache")
                                                                                          {
                                                                                              foreach (PerformanceCounter performanceCounter in performanceCounterCategory.GetCounters())
                                                                                              {
                                                                                                  int index = dgvPerformanceCounters.Rows.Add(performanceCounter.CategoryName.Replace("arachnode.net:", string.Empty), performanceCounter.CounterName.Replace("arachnode.net - ", string.Empty), 0, 0, 0, 0);

                                                                                                  _cacheCounters.Add(index, performanceCounter);
                                                                                              }
                                                                                          }

                                                                                          if (performanceCounterCategory.CategoryName == "arachnode.net:Crawl")
                                                                                          {
                                                                                              foreach (PerformanceCounter performanceCounter in performanceCounterCategory.GetCounters())
                                                                                              {
                                                                                                  int index = dgvPerformanceCounters.Rows.Add(performanceCounter.CategoryName.Replace("arachnode.net:", string.Empty), performanceCounter.CounterName.Replace("arachnode.net - ", string.Empty), 0, 0, 0, 0);

                                                                                                  _crawlCounters.Add(index, performanceCounter);
                                                                                              }
                                                                                          }

                                                                                          if (performanceCounterCategory.CategoryName == "arachnode.net:CrawlerPeer")
                                                                                          {
                                                                                              foreach (PerformanceCounter performanceCounter in performanceCounterCategory.GetCounters())
                                                                                              {
                                                                                                  int index = dgvPerformanceCounters.Rows.Add(performanceCounter.CategoryName.Replace("arachnode.net:", string.Empty), performanceCounter.CounterName.Replace("arachnode.net - ", string.Empty), 0, 0, 0, 0);

                                                                                                  _crawlerPeerCounters.Add(index, performanceCounter);
                                                                                              }
                                                                                          }
                                                                                      }
                                                                                  }));
                                            });

                thread.Start();
            }
            catch (Exception exception)
            {
                rtbOutput.Text += Environment.NewLine + exception.Message + Environment.NewLine + exception.StackTrace;

                MessageBox.Show(exception.Message + Environment.NewLine + exception.StackTrace, _formText);

                Process.GetCurrentProcess().Kill();
            }
        }

        private void ParseCrawlRequestsDotText(string fileName)
        {
            try
            {
                foreach (string crawlRequest in File.ReadAllLines(fileName))
                {
                    if (crawlRequest.Trim().StartsWith("//") || string.IsNullOrWhiteSpace(crawlRequest))
                    {
                        continue;
                    }

                    string[] crawlRequestSplit = crawlRequest.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    string absoluteUri = crawlRequestSplit[0];
                    string depth = crawlRequestSplit[1];

                    UriClassificationType restrictCrawlTo = UriClassificationType.None;

                    foreach (string uriClassificationType in crawlRequestSplit[2].Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                    {
                        restrictCrawlTo |= (UriClassificationType)Enum.Parse(typeof(UriClassificationType), uriClassificationType);
                    }

                    UriClassificationType restrictDiscoveriesTo = UriClassificationType.None;

                    foreach (string uriClassificationType in crawlRequestSplit[3].Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                    {
                        restrictDiscoveriesTo |= (UriClassificationType)Enum.Parse(typeof(UriClassificationType), uriClassificationType);
                    }

                    string priority = crawlRequestSplit[4];

                    RenderType renderType = RenderType.None;

                    foreach (string renderType2 in crawlRequestSplit[5].Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                    {
                        renderType |= (RenderType)Enum.Parse(typeof(RenderType), renderType2);
                    }

                    RenderType renderTypeForChildren = RenderType.None;

                    foreach (string renderType2 in crawlRequestSplit[6].Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                    {
                        renderTypeForChildren |= (RenderType)Enum.Parse(typeof(RenderType), renderType2);
                    }

                    dgvCrawlRequests.Rows.Add(absoluteUri, depth, restrictCrawlTo, restrictDiscoveriesTo, priority, renderType, renderTypeForChildren);
                }

                tcMain.SelectedTab = tpCrawlRequests;

                Text = _formText + " - " + fileName;
            }
            catch (Exception exception)
            {
                rtbOutput.Text += Environment.NewLine + exception.Message + Environment.NewLine + exception.StackTrace;

                MessageBox.Show(exception.Message + Environment.NewLine + exception.StackTrace, _formText);
            }
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            if (!Monitor.TryEnter(_timerLock, 100))
            {
                return;
            }

            SuspendLayout();

            if (_crawler != null)
            {
                tsslEngineState.Text = "Engine State: " + _crawler.Engine.State;
            }
            else
            {
                tsslEngineState.Text = "Engine State: UNKNOWN";
            }
            tsslElapasedTime.Text = "/ Elapsed Time: " + _stopwatchForCrawler.Elapsed;
            tsslApplicationTime.Text = "Application Time: " + _stopwatchForApplication.Elapsed;

            foreach (var keyValuePair in _arachnodeDAOCounters)
            {
                dgvPerformanceCounters.Rows[keyValuePair.Key].Cells["dgvtbcValue"].Value = keyValuePair.Value.RawValue;
            }

            foreach (var keyValuePair in _cacheCounters)
            {
                dgvPerformanceCounters.Rows[keyValuePair.Key].Cells["dgvtbcValue"].Value = keyValuePair.Value.RawValue;
            }

            foreach (var keyValuePair in _crawlCounters)
            {
                dgvPerformanceCounters.Rows[keyValuePair.Key].Cells["dgvtbcValue"].Value = keyValuePair.Value.RawValue;
            }

            foreach (var keyValuePair in _crawlerPeerCounters)
            {
                dgvPerformanceCounters.Rows[keyValuePair.Key].Cells["dgvtbcValue"].Value = keyValuePair.Value.RawValue;
            }

            foreach (DataGridViewRow dataGridViewRow in dgvPerformanceCounters.Rows)
            {
                double value = double.Parse(dataGridViewRow.Cells["dgvtbcValue"].Value.ToString());

                /**/

                double total = double.Parse(dataGridViewRow.Cells["dgvtbcTotal"].Value.ToString());

                total += value;

                dataGridViewRow.Cells["dgvtbcTotal"].Value = total;

                /**/

                double maximum = Math.Max(value, double.Parse(dataGridViewRow.Cells["dgvtbcMaximum"].Value.ToString()));

                dataGridViewRow.Cells["dgvtbcMaximum"].Value = maximum;

                /**/

                double average = 0;

                if (_stopwatchForCrawler.Elapsed.TotalSeconds != 0)
                {
                    average = total / _stopwatchForCrawler.Elapsed.TotalSeconds;
                }

                dataGridViewRow.Cells["dgvtbcAverage"].Value = Math.Round(average, 2);

                /**/

                dataGridViewRow.Cells["dgvtbcValue"].Style.BackColor = Color.Empty;
                if (value > average)
                {
                    dataGridViewRow.Cells["dgvtbcValue"].Style.BackColor = Color.MediumSeaGreen;
                }
                else if (value == average)
                {
                    dataGridViewRow.Cells["dgvtbcValue"].Style.BackColor = Color.Empty;
                }
                else if (value < average)
                {
                    dataGridViewRow.Cells["dgvtbcValue"].Style.BackColor = Color.LightCoral;
                }
            }

            if (_consoleOutToQueue != null)
            {
                _consoleOutToQueue.UpdateRichTextBox(rtbOutput);
            }

            ResumeLayout(true);
        }

        private void rtbOutput_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }

        private void ResetDatabase()
        {
            System.Console.WriteLine("\nReset Database:");
            System.Console.WriteLine("  -> EXEC [dbo].[arachnode_usp_arachnode.net_RESET_DATABASE] (SQL).");
            System.Console.WriteLine("  -> Resets all user data (SQL).");

            System.Console.WriteLine("\nInitial setup tasks:");
            System.Console.WriteLine("  -> Populate settings for missing values in cfg.Configuration (SQL).");
            System.Console.WriteLine("  -> Populate settings for missing values in cfg.CrawlActions (SQL).");

            System.Console.WriteLine("\nReset Database and perform initial setup tasks?  (y/n)");

            //README: This directory will not contain files unless 'OutputConsoleToLogs' is 'true', and 'EnableConsoleOutput' is 'true'.
            string consoleOutputLogsDirectory = Path.Combine(Environment.CurrentDirectory, "ConsoleOutputLogs");

            string downloadedFilesDirectory = Path.Combine(Environment.CurrentDirectory, "DownloadedFiles");
            string downloadedImagesDirectory = Path.Combine(Environment.CurrentDirectory, "DownloadedImages");
            string downloadedWebPagesDirectory = Path.Combine(Environment.CurrentDirectory, "DownloadedWebPages");
            string luceneDotNetIndexDirectory = Path.Combine(Environment.CurrentDirectory, "LuceneDotNetIndex");

            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine("Resetting Database...");
#if DEMO
                    System.Console.WriteLine("If this hangs, check Configuration\\ConnectionStrings.config and that MS-SQL Server is installed.");
#endif
            System.Console.ForegroundColor = ConsoleColor.Gray;

            //reset the database...
            IArachnodeDAO arachnodeDAO = new ArachnodeDAO(_applicationSettings.ConnectionString);
            //running with '1' as the input parameter removes all user data.
            //running this stored procedure alleviates many installation issues.
            arachnodeDAO.ExecuteSql("EXEC [dbo].[arachnode_usp_arachnode.net_RESET_DATABASE]");
            arachnodeDAO.ExecuteSql("TRUNCATE TABLE Documents");

            //running with '1' as the input parameter resets the database to the intial configuration state and removes all user data.
            //arachnodeDAO.ExecuteSql("EXEC [dbo].[arachnode_usp_arachnode.net_RESET_DATABASE] 1");

            //perform common setup tasks.

            //update the config values for the on-disk storage.
            arachnodeDAO.ExecuteSql("UPDATE cfg.Configuration SET [Value] = '" + consoleOutputLogsDirectory + "' WHERE [Key] = 'ConsoleOutputLogsDirectory'");
            arachnodeDAO.ExecuteSql("UPDATE cfg.Configuration SET [Value] = '" + downloadedFilesDirectory + "' WHERE [Key] = 'DownloadedFilesDirectory'");
            arachnodeDAO.ExecuteSql("UPDATE cfg.Configuration SET [Value] = '" + downloadedImagesDirectory + "' WHERE [Key] = 'DownloadedImagesDirectory'");
            arachnodeDAO.ExecuteSql("UPDATE cfg.Configuration SET [Value] = '" + downloadedWebPagesDirectory + "' WHERE [Key] = 'DownloadedWebPagesDirectory'");
            arachnodeDAO.ExecuteSql("UPDATE cfg.Configuration SET [Value] = '" + luceneDotNetIndexDirectory + "' WHERE [Key] = 'LuceneDotNetIndexDirectory'");

            string settings = "AutoCommit=true|LuceneDotNetIndexDirectory=" + luceneDotNetIndexDirectory + "|CheckIndexes=false|IndexFiles=true|IndexImages=true|IndexWebPages=true|RebuildIndexOnLoad=false|FileIDLowerBound=1|FileIDUpperBound=100000|ImageIDLowerBound=1|ImageIDUpperBound=100000|WebPageIDLowerBound=1|WebPageIDUpperBound=100000";

            //update the indexing location for the lucene.net functionality.
            arachnodeDAO.ExecuteSql("UPDATE cfg.CrawlActions SET [Settings] = '" + settings + "' WHERE TypeName = 'Arachnode.Plugins.CrawlActions.ManageLuceneDotNetIndexes'");

            _crawler.ConsoleManager.OutputString("ResetDatabase: Complete");
        }

        private void ResetCrawler()
        {
            _crawler.Reset();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_crawler != null && _crawler.Engine != null)
            {
                if (_crawler.Engine.State != EngineState.Stop)
                {
                    if (MessageBox.Show("The Engine is not stopped.  Quit anyway?", _formText, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        Process.GetCurrentProcess().Kill();
                    }
                }
            }
        }

        #region Button Events

        private void btnLoadApplicationsSettings_Click(object sender, EventArgs e)
        {
            ofdEntityRows.FileName = "ApplicationSettings.xml";
            ofdEntityRows.Filter = "Xml files|*.xml";

            if (ofdEntityRows.ShowDialog() == DialogResult.OK)
            {
                DataTable dataTable = new DataTable();

                dataTable.ReadXml(ofdEntityRows.FileName);

                dgvApplicationSettings.Rows.Clear();
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    if (string.Join(null, dataRow.ItemArray.Where(i => i != DBNull.Value).Cast<string>().ToArray()).Length != 0)
                    {
                        dgvApplicationSettings.Rows.Add(dataRow.ItemArray);
                    }
                }
            }
        }

        private void btnSaveApplicationSettings_Click(object sender, EventArgs e)
        {
            sfdEntityRows.FileName = "ApplicationSettings.xml";
            sfdEntityRows.Filter = "Xml files|*.xml";

            if (sfdEntityRows.ShowDialog() == DialogResult.OK)
            {
                DataTable dataTable = DataGridViews.GetDataTableFromDataGridView(dgvApplicationSettings, "ApplicationSettings");

                dataTable.WriteXml(sfdEntityRows.FileName, XmlWriteMode.WriteSchema);
            }
        }

        private void btnLoadWebSettings_Click(object sender, EventArgs e)
        {
            ofdEntityRows.FileName = "WebSettings.xml";
            ofdEntityRows.Filter = "Xml files|*.xml";

            if (ofdEntityRows.ShowDialog() == DialogResult.OK)
            {
                DataTable dataTable = new DataTable();

                dataTable.ReadXml(ofdEntityRows.FileName);

                dgvWebSettings.Rows.Clear();
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    if (string.Join(null, dataRow.ItemArray.Where(i => i != DBNull.Value).Cast<string>().ToArray()).Length != 0)
                    {
                        dgvWebSettings.Rows.Add(dataRow.ItemArray);
                    }
                }
            }
        }

        private void btnSaveWebSettings_Click(object sender, EventArgs e)
        {
            sfdEntityRows.FileName = "WebSettings.xml";
            sfdEntityRows.Filter = "Xml files|*.xml";

            if (sfdEntityRows.ShowDialog() == DialogResult.OK)
            {
                DataTable dataTable = DataGridViews.GetDataTableFromDataGridView(dgvWebSettings, "WebSettings");

                dataTable.WriteXml(sfdEntityRows.FileName, XmlWriteMode.WriteSchema);
            }
        }

        private void btnLoadCredentialCache_Click(object sender, EventArgs e)
        {
            ofdEntityRows.FileName = "CredentialCache.xml";
            ofdEntityRows.Filter = "Xml files|*.xml";

            if (ofdEntityRows.ShowDialog() == DialogResult.OK)
            {
                DataTable dataTable = new DataTable();

                dataTable.ReadXml(ofdEntityRows.FileName);

                dgvCredentialCache.Rows.Clear();
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    if (string.Join(null, dataRow.ItemArray.Where(i => i != DBNull.Value).Cast<string>().ToArray()).Length != 0)
                    {
                        dgvCredentialCache.Rows.Add(dataRow.ItemArray);
                    }
                }
            }
        }

        private void btnSaveCredentialCache_Click(object sender, EventArgs e)
        {
            sfdEntityRows.FileName = "CredentialCache.xml";
            sfdEntityRows.Filter = "Xml files|*.xml";

            if (sfdEntityRows.ShowDialog() == DialogResult.OK)
            {
                DataTable dataTable = DataGridViews.GetDataTableFromDataGridView(dgvCredentialCache, "CredentialCache");

                dataTable.WriteXml(sfdEntityRows.FileName, XmlWriteMode.WriteSchema);
            }
        }

        private void btnLoadCookieContainer_Click(object sender, EventArgs e)
        {
            ofdEntityRows.FileName = "CookieContainer.xml";
            ofdEntityRows.Filter = "Xml files|*.xml";

            if (ofdEntityRows.ShowDialog() == DialogResult.OK)
            {
                DataTable dataTable = new DataTable();

                dataTable.ReadXml(ofdEntityRows.FileName);

                dgvCookieContainer.Rows.Clear();
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    if (string.Join(null, dataRow.ItemArray.Where(i => i != DBNull.Value).Cast<string>().ToArray()).Length != 0)
                    {
                        dgvCookieContainer.Rows.Add(dataRow.ItemArray);
                    }
                }
            }
        }

        private void btnSaveCookieContainer_Click(object sender, EventArgs e)
        {
            sfdEntityRows.FileName = "CookieContainer.xml";
            sfdEntityRows.Filter = "Xml files|*.xml";

            if (sfdEntityRows.ShowDialog() == DialogResult.OK)
            {
                DataTable dataTable = DataGridViews.GetDataTableFromDataGridView(dgvCookieContainer, "CookieContainer");

                dataTable.WriteXml(sfdEntityRows.FileName, XmlWriteMode.WriteSchema);
            }
        }

        private void btnLoadProxyServers_Click(object sender, EventArgs e)
        {
            ofdEntityRows.FileName = "ProxyServers.xml";
            ofdEntityRows.Filter = "Xml files|*.xml";

            if (ofdEntityRows.ShowDialog() == DialogResult.OK)
            {
                DataTable dataTable = new DataTable();

                dataTable.ReadXml(ofdEntityRows.FileName);

                dgvProxyServers.Rows.Clear();
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    if (string.Join(null, dataRow.ItemArray.Where(i => i != DBNull.Value).Cast<string>().ToArray()).Length != 0)
                    {
                        dgvProxyServers.Rows.Add(dataRow.ItemArray);
                    }
                }
            }
        }

        private void btnSaveProxyServers_Click(object sender, EventArgs e)
        {
            sfdEntityRows.FileName = "ProxyServers.xml";
            sfdEntityRows.Filter = "Xml files|*.xml";

            if (sfdEntityRows.ShowDialog() == DialogResult.OK)
            {
                DataTable dataTable = DataGridViews.GetDataTableFromDataGridView(dgvProxyServers, "ProxyServers");

                dataTable.WriteXml(sfdEntityRows.FileName, XmlWriteMode.WriteSchema);
            }
        }

        private void btnVerify_Click(object sender, EventArgs e)
        {
            _crawler.ProxyManager.Proxies.Clear();

            foreach (DataGridViewRow dataGridViewRow in dgvProxyServers.Rows)
            {
                dataGridViewRow.Cells["dgvtbcStatusCodeProxyServers"].Style.BackColor = Color.Empty;
                dataGridViewRow.Cells["dgvtbcStatusCodeProxyServers"].Value = null;
            }

            foreach (DataGridViewRow dataGridViewRow in dgvProxyServers.Rows)
            {
                try
                {
                    string scheme = dataGridViewRow.Cells["dgvcbcSchemeProxyServers"].Value.ToString();
                    string ipAddress = dataGridViewRow.Cells["dgvtbcIPAddress"].Value.ToString();
                    string port = dataGridViewRow.Cells["dgvtbcPort"].Value.ToString();
                    string absoluteUriToVerify = dataGridViewRow.Cells["dgvtbcAbsoluteUriToVerify"].Value.ToString();
                    string valueToVerify = dataGridViewRow.Cells["dgvtbcValueToVerify"].Value.ToString();
                    int timeoutInMilliseconds = int.Parse(dataGridViewRow.Cells["dgvtbcTimeoutInMilliseconds"].Value.ToString());
                    bool isEnabled = false;
                    if(dataGridViewRow.Cells["dgvcbcIsEnabledProxyServers"].Value != null)
                    {
                        isEnabled = bool.Parse(dataGridViewRow.Cells["dgvcbcIsEnabledProxyServers"].Value.ToString());
                    }

                    if (isEnabled)
                    {
                        Uri uri = new Uri(scheme + ipAddress + ":" + port);

                        List<IWebProxy> proxyServers = _crawler.ProxyManager.LoadProxyServers(new List<Uri> {uri}, timeoutInMilliseconds, 1, absoluteUriToVerify, valueToVerify, false);

                        if (proxyServers.Count != 0)
                        {
                            dataGridViewRow.Cells["dgvtbcStatusCodeProxyServers"].Style.BackColor = Color.MediumSeaGreen;
                        }
                        else
                        {
                            dataGridViewRow.Cells["dgvtbcStatusCodeProxyServers"].Style.BackColor = Color.Firebrick;
                        }

                        HttpStatusCode httpStatusCode = _crawler.ProxyManager.ProxiesAndHttpStatusCodes[proxyServers.First()];

                        dataGridViewRow.Cells["dgvtbcStatusCodeProxyServers"].Value = httpStatusCode;
                    }
                    else
                    {
                        dataGridViewRow.Cells["dgvtbcStatusCodeProxyServers"].Style.BackColor = Color.LightCoral;
                    }
                }
                catch
                {
                    dataGridViewRow.Cells["dgvtbcStatusCodeProxyServers"].Style.BackColor = Color.Firebrick;
                }
            }
        }

        private void btnLoadCrawlRequests_Click(object sender, EventArgs e)
        {
            ofdEntityRows.FileName = "CrawlRequests.txt";
            ofdEntityRows.Filter = "Text files|*.txt";

            if (ofdEntityRows.ShowDialog() == DialogResult.OK)
            {
                dgvCrawlRequests.Rows.Clear();

                ParseCrawlRequestsDotText(ofdEntityRows.FileName);
            }
        }

        private void btnSaveCrawlRequests_Click(object sender, EventArgs e)
        {
            sfdEntityRows.FileName = "CrawlRequests.txt";
            sfdEntityRows.Filter = "Text files|*.txt";

            if (sfdEntityRows.ShowDialog() == DialogResult.OK)
            {
                string fileName = sfdEntityRows.FileName;

                var stringBuilder = new StringBuilder();

                foreach (DataGridViewRow dataGridViewRow in dgvCrawlRequests.Rows)
                {
                    string entityRow = null;

                    foreach (DataGridViewCell dataGridViewCell in dataGridViewRow.Cells)
                    {
                        if (dataGridViewCell.Value == null || string.IsNullOrEmpty(dataGridViewCell.Value.ToString()))
                        {
                            //'Added'...
                            if (dataGridViewCell.ColumnIndex != 7)
                            {
                                entityRow = null;
                            }
                            break;
                        }

                        entityRow += dataGridViewCell.Value + ",";
                    }

                    if (!string.IsNullOrEmpty(entityRow))
                    {
                        stringBuilder.AppendLine(entityRow.TrimEnd(",".ToCharArray()));
                    }
                }

                File.WriteAllText(fileName, stringBuilder.ToString());
            }
        }

        private void btnLoadCrawler_Click(object sender, EventArgs e)
        {
            ofdEntityRows.FileName = "Crawler.xml";
            ofdEntityRows.Filter = "Xml files|*.xml";

            if (ofdEntityRows.ShowDialog() == DialogResult.OK)
            {
                DataTable dataTable = new DataTable();

                dataTable.ReadXml(ofdEntityRows.FileName);

                dgvCrawler.Rows.Clear();
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    if (string.Join(null, dataRow.ItemArray.Where(i => i != DBNull.Value).Cast<string>().ToArray()).Length != 0)
                    {
                        dgvCrawler.Rows.Add(dataRow.ItemArray);
                    }
                }
            }
        }

        private void btnSaveCrawler_Click(object sender, EventArgs e)
        {
            sfdEntityRows.FileName = "Crawler.xml";
            sfdEntityRows.Filter = "Xml files|*.xml";

            if (sfdEntityRows.ShowDialog() == DialogResult.OK)
            {
                DataTable dataTable = DataGridViews.GetDataTableFromDataGridView(dgvCrawler, "Crawler");

                dataTable.WriteXml(sfdEntityRows.FileName, XmlWriteMode.WriteSchema);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            btnPause.Enabled = true;
            btnResume.Enabled = false;

            dgvCrawler.Rows.Clear();

            //add to the CredentialCache...
            _crawler.CredentialCache = new CredentialCache();

            foreach (DataGridViewRow dataGridViewRow in dgvCredentialCache.Rows)
            {
                string scheme = null;
                if (dataGridViewRow.Cells["dgvcbcSchemeCredentialCache"].Value != null)
                {
                    scheme = dataGridViewRow.Cells["dgvcbcSchemeCredentialCache"].Value.ToString();
                }

                string domain = null;
                if (dataGridViewRow.Cells["dgvtbcDomainCredentialCache"].Value != null)
                {
                    domain = dataGridViewRow.Cells["dgvtbcDomainCredentialCache"].Value.ToString();
                }

                string userName = null;
                if (dataGridViewRow.Cells["dgvtbcUserName"].Value != null)
                {
                    userName = dataGridViewRow.Cells["dgvtbcUserName"].Value.ToString();
                }

                string password = null;
                if (dataGridViewRow.Cells["dgvtbcPassword"].Value != null)
                {
                    password = dataGridViewRow.Cells["dgvtbcPassword"].Value.ToString();
                }

                bool isEnabled = false;
                if (dataGridViewRow.Cells["dgvcbcIsEnabledCredentialCache"].Value != null)
                {
                    isEnabled = bool.Parse(dataGridViewRow.Cells["dgvcbcIsEnabledCredentialCache"].Value.ToString());
                }

                if(isEnabled)                   
                {
                    _crawler.CredentialCache.Add(new Uri(scheme + "://" + domain), "Basic", new NetworkCredential(userName, password));
                    _crawler.CredentialCache.Add(new Uri(scheme + "://" + domain), "Digest", new NetworkCredential(userName, password, domain));
                }
            }

            //add to the CookieContainer...
            _crawler.CookieContainer = new CookieContainer();

            foreach (DataGridViewRow dataGridViewRow in dgvCookieContainer.Rows)
            {
                //dataGridViewRow.Cells["dgvtbcStatusCodeCookieContainer"].Style.BackColor = Color.Empty;
                //dataGridViewRow.Cells["dgvtbcStatusCodeCookieContainer"].Value = null;
            }

            foreach (DataGridViewRow dataGridViewRow in dgvCookieContainer.Rows)
            {
                //string scheme = dataGridViewRow.Cells["dgvcbcSchemeProxyServers"].Value.ToString();
                //string domain = dataGridViewRow.Cells["dgvtbcDomainCredentialCache"].Value.ToString();
                //string value = dataGridViewRow.Cells["dgvtbcValue"].Value.ToString();
                //bool isEnabled = false;
                //if (dataGridViewRow.Cells["dgvcbcIsEnabledCookieContainer"].Value != null)
                //{
                    //isEnabled = bool.Parse(dataGridViewRow.Cells["dgvcbcIsEnabledCookieContainer"].Value.ToString());
                //}

                //if (isEnabled)
                //{
                    //_crawler.CookieContainer.Add(new Uri(scheme + "://" + domain), _crawler.CookieManager.BuildCookieCollection(value));
                //}
            }

            //verify the ProxyServers...
            btnVerify_Click(this, e);

            for (int i = 1; i <= _applicationSettings.MaximumNumberOfCrawlThreads; i++)
            {
                dgvCrawler.Rows.Add(i, null, null);
            }

            dgvCrawlInfo.Rows.Clear();

            for (int i = 1; i <= _applicationSettings.MaximumNumberOfCrawlThreads; i++)
            {
                dgvCrawlInfo.Rows.Add(i, null, null);
            }

            BeginInvoke(new MethodInvoker(delegate
                                              {
                                                  if (resetDatabaseOnStartToolStripMenuItem.Checked)
                                                  {
                                                      ResetDatabase();
                                                  }

                                                  if (resetCrawlerOnStartToolStripMenuItem.Checked)
                                                  {
                                                      ResetCrawler();
                                                  }

                                                  _stopwatchForCrawler.Reset();
                                                  _stopwatchForCrawler.Start();

                                                  foreach (DataGridViewRow dataGridViewRow in dgvCrawlRequests.Rows)
                                                  {
                                                      try
                                                      {
                                                          string absoluteUri = dataGridViewRow.Cells["dgvtbcCRAbsoluteUri"].Value.ToString();
                                                          int depth = int.Parse(dataGridViewRow.Cells["dgvtbcCRDepth"].Value.ToString());
                                                          double priority = double.Parse(dataGridViewRow.Cells["dgvtbcCRPriority"].Value.ToString());

                                                          var crawlRequest = new CrawlRequest<ArachnodeDAO>(new Discovery<ArachnodeDAO>(absoluteUri), depth, UriClassificationType.Domain, UriClassificationType.Domain, priority, RenderType.None, RenderType.None);

                                                          bool wasTheCrawlRequestAddedForCrawling = _crawler.Crawl(crawlRequest);

                                                          dataGridViewRow.Cells["dgvtbcCRAdded"].Value = wasTheCrawlRequestAddedForCrawling;

                                                          if (wasTheCrawlRequestAddedForCrawling)
                                                          {
                                                              dataGridViewRow.Cells["dgvtbcCRAdded"].Style.BackColor = Color.MediumSeaGreen;
                                                          }
                                                          else
                                                          {
                                                              dataGridViewRow.Cells["dgvtbcCRAdded"].Style.BackColor = Color.LightCoral;
                                                          }
                                                      }
                                                      catch
                                                      {
                                                          dataGridViewRow.Cells["dgvtbcCRAdded"].Style.BackColor = Color.Firebrick;
                                                      }
                                                  }

                                                  _crawler.Engine.Start();
                                              }));
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStop.Enabled = false;

            _crawler.Engine.Stop();

            _stopwatchForCrawler.Stop();

            btnStart.Enabled = true;
            btnPause.Enabled = false;
            btnResume.Enabled = false;
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            btnPause.Enabled = false;

            _crawler.Engine.Pause();

            _stopwatchForCrawler.Stop();

            btnStart.Enabled = false;
            btnStop.Enabled = true;
            btnResume.Enabled = true;
        }

        private void btnResume_Click(object sender, EventArgs e)
        {
            btnResume.Enabled = false;

            _stopwatchForCrawler.Stop();

            _crawler.Engine.Resume();

            btnStart.Enabled = false;
            btnStop.Enabled = true;
            btnPause.Enabled = true;
        }

        private void btnLoadCrawlInfo_Click(object sender, EventArgs e)
        {
            ofdEntityRows.FileName = "CrawlInfo.xml";
            ofdEntityRows.Filter = "Xml files|*.xml";

            if (ofdEntityRows.ShowDialog() == DialogResult.OK)
            {
                DataTable dataTable = new DataTable();

                dataTable.ReadXml(ofdEntityRows.FileName);

                dgvCrawlInfo.Rows.Clear();
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    if (string.Join(null, dataRow.ItemArray.Where(i => i != DBNull.Value).Cast<string>().ToArray()).Length != 0)
                    {
                        dgvCrawlInfo.Rows.Add(dataRow.ItemArray);
                    }
                }
            }
        }

        private void btnSaveCrawlInfo_Click(object sender, EventArgs e)
        {
            sfdEntityRows.FileName = "CrawlInfo.xml";
            sfdEntityRows.Filter = "Xml files|*.xml";

            if (sfdEntityRows.ShowDialog() == DialogResult.OK)
            {
                DataTable dataTable = DataGridViews.GetDataTableFromDataGridView(dgvCrawlInfo, "CrawlInfo");

                dataTable.WriteXml(sfdEntityRows.FileName, XmlWriteMode.WriteSchema);
            }
        }

        #endregion

        private void btnLoadPerformanceCounters_Click(object sender, EventArgs e)
        {
            ofdEntityRows.FileName = "PerformanceCounters.xml";
            ofdEntityRows.Filter = "Xml files|*.xml";

            if (ofdEntityRows.ShowDialog() == DialogResult.OK)
            {
                DataTable dataTable = new DataTable();

                dataTable.ReadXml(ofdEntityRows.FileName);

                dgvPerformanceCounters.Rows.Clear();
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    if (string.Join(null, dataRow.ItemArray.Where(i => i != DBNull.Value).Cast<string>().ToArray()).Length != 0)
                    {
                        dgvPerformanceCounters.Rows.Add(dataRow.ItemArray);
                    }
                }
            }
        }

        private void btnSavePerformanceCounters_Click(object sender, EventArgs e)
        {
            sfdEntityRows.FileName = "PerformanceCounters.xml";
            sfdEntityRows.Filter = "Xml files|*.xml";

            if (sfdEntityRows.ShowDialog() == DialogResult.OK)
            {
                DataTable dataTable = DataGridViews.GetDataTableFromDataGridView(dgvPerformanceCounters, "PerformanceCounters");

                dataTable.WriteXml(sfdEntityRows.FileName, XmlWriteMode.WriteSchema);
            }
        }

        #region Grid Events
        private void dgvTableName_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvTableName.SelectedRows.Count == 0)
            {
                return;
            }

            foreach (PropertyInfo propertyInfo in _arachnodeDataSourceDataContext.GetType().GetProperties().OrderBy(pi => pi.Name))
            {
                if (propertyInfo.PropertyType.FullName.StartsWith("System.Data.Linq.Table") &&
                    propertyInfo.Name == dgvTableName.SelectedRows[0].Cells["dgvtbcTableName"].Value.ToString())
                {
                    dgvConfiguration.DataSource = propertyInfo.GetValue(_arachnodeDataSourceDataContext, null);

                    foreach (DataGridViewColumn dataGridViewColumn in dgvConfiguration.Columns)
                    {
                        dataGridViewColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }

                    break;
                }
            }
        }

        private void dgvApplicationSettings_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_applicationSettings != null)
            {
                foreach (PropertyInfo propertyInfo in _applicationSettings.GetType().GetProperties())
                {
                    if (propertyInfo.Name == dgvApplicationSettings.Rows[e.RowIndex].Cells["dgvtbcApplicationSettingsName"].Value.ToString())
                    {
                        if (propertyInfo.PropertyType.FullName == "System.Boolean")
                        {
                            propertyInfo.SetValue(_applicationSettings, bool.Parse(dgvApplicationSettings.Rows[e.RowIndex].Cells["dgvtbcApplicationSettingsValue"].Value.ToString()), null);
                        }
                        else if (propertyInfo.PropertyType.FullName == "System.Int32")
                        {
                            propertyInfo.SetValue(_applicationSettings, int.Parse(dgvApplicationSettings.Rows[e.RowIndex].Cells["dgvtbcApplicationSettingsValue"].Value.ToString()), null);
                        }
                        else
                        {
                            propertyInfo.SetValue(_applicationSettings, dgvApplicationSettings.Rows[e.RowIndex].Cells["dgvtbcApplicationSettingsValue"].Value, null);
                        }

                        break;
                    }
                }
            }
        }

        private void dgvApplicationSettings_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        private void dgvWebSettings_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_webSettings != null)
            {
                foreach (PropertyInfo propertyInfo in _webSettings.GetType().GetProperties())
                {
                    if (propertyInfo.Name == dgvWebSettings.Rows[e.RowIndex].Cells["dgvtbcWebSettingsName"].Value.ToString())
                    {
                        if (propertyInfo.PropertyType.FullName == "System.Boolean")
                        {
                            propertyInfo.SetValue(_webSettings, bool.Parse(dgvWebSettings.Rows[e.RowIndex].Cells["dgvtbcWebSettingsValue"].Value.ToString()), null);
                        }
                        else if (propertyInfo.PropertyType.FullName == "System.Int32")
                        {
                            propertyInfo.SetValue(_webSettings, bool.Parse(dgvWebSettings.Rows[e.RowIndex].Cells["dgvtbcWebSettingsValue"].Value.ToString()), null);
                        }
                        else
                        {
                            propertyInfo.SetValue(_webSettings, dgvWebSettings.Rows[e.RowIndex].Cells["dgvtbcWebSettingsValue"].Value, null);
                        }

                        break;
                    }
                }
            }
        }

        private void dgvWebSettings_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }
        #endregion

        #region Engine Events

        private void Engine_CrawlRequestCompleted(CrawlRequest<ArachnodeDAO> crawlRequest)
        {
            while (crawlRequest.Crawl.IsProcessingDiscoveriesAsynchronously)
            {
                Thread.Sleep(1);
            }

            if (dgvCrawler.Rows.Count != 0)
            {
                dgvCrawler.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcThreadNumberCrawler"].Value = crawlRequest.Crawl.CrawlInfo.ThreadNumber;
                dgvCrawler.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcThreadNumberCrawler"].Style.BackColor = Color.Empty;
                dgvCrawler.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcAbsoluteUri"].Value = crawlRequest.Discovery.Uri.AbsoluteUri;
                //TODO: Implement?
                //dgvCrawler.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcLastRequested"].Value = crawlRequest.Politeness.LastHttpWebRequestRequested;
                //dgvCrawler.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcLastCanceled"].Value = crawlRequest.Politeness.LastHttpWebRequestCanceled;
                //dgvCrawler.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcLastCompleted"].Value = crawlRequest.Politeness.LastHttpWebRequestCompleted;
                if (crawlRequest.WebClient.HttpWebResponse != null)
                {
                    dgvCrawler.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcStatusCode"].Value = crawlRequest.WebClient.HttpWebResponse.StatusCode;
                }
                else
                {
                    dgvCrawler.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcStatusCode"].Value = "NULL";
                }
                if (dgvCrawler.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcStatusCode"].Value.ToString() == "OK")
                {
                    dgvCrawler.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcStatusCode"].Style.BackColor = Color.MediumSeaGreen;
                }
                else
                {
                    dgvCrawler.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcStatusCode"].Style.BackColor = Color.LightCoral;
                }
                dgvCrawler.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcDiscoveryType"].Value = crawlRequest.Discovery.DiscoveryType;
                dgvCrawler.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcHyperLinkDiscoveries"].Value = crawlRequest.Discoveries.HyperLinks.Count;
                dgvCrawler.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcFileAndImageDiscoveries"].Value = crawlRequest.Discoveries.FilesAndImages.Count;
                dgvCrawler.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcCurrentDepth"].Value = crawlRequest.CurrentDepth;
                dgvCrawler.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcMaximumDepth"].Value = crawlRequest.MaximumDepth;
            }

            if (dgvCrawlInfo.Rows.Count != 0)
            {
                dgvCrawlInfo.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcThreadNumberCrawlInfo"].Value = crawlRequest.Crawl.CrawlInfo.ThreadNumber;
                dgvCrawlInfo.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcParentDiscovery"].Value = crawlRequest.Crawl.CrawlInfo.CurrentCrawlRequest.Parent;
                dgvCrawlInfo.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcCurrentDiscovery"].Value = crawlRequest.Crawl.CrawlInfo.CurrentCrawlRequest.Discovery;
                dgvCrawlInfo.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcCrawlState"].Value =  crawlRequest.Crawl.CrawlInfo.CrawlState;
                dgvCrawlInfo.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcTotalCrawlFedCount"].Value = crawlRequest.Crawl.CrawlInfo.TotalCrawlFedCount;
                dgvCrawlInfo.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcTotalCrawlRequestsAssigned"].Value = crawlRequest.Crawl.CrawlInfo.TotalCrawlRequestsAssigned;
                dgvCrawlInfo.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcTotalCrawlStarvedCount"].Value = crawlRequest.Crawl.CrawlInfo.TotalCrawlStarvedCount;
                dgvCrawlInfo.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcTotalHttpWebResponseTime"].Value = crawlRequest.Crawl.CrawlInfo.TotalHttpWebResponseTime.ToString();
            }
        }

        private void Engine_CrawlRequestThrottled(CrawlRequest<ArachnodeDAO> crawlRequest)
        {
            if (dgvCrawler.Rows.Count != 0)
            {
                dgvCrawler.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcThreadNumber"].Style.BackColor = Color.Yellow;
                dgvCrawler.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcAbsoluteUri"].Value = crawlRequest.Discovery.Uri.AbsoluteUri;
                //TODO: Implement?
                //dgvCrawler.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcLastRequested"].Value = crawlRequest.Politeness.LastHttpWebRequestRequested;
                //dgvCrawler.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcLastCanceled"].Value = crawlRequest.Politeness.LastHttpWebRequestCanceled;
                //dgvCrawler.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcLastCompleted"].Value = crawlRequest.Politeness.LastHttpWebRequestCompleted;
                dgvCrawler.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcStatusCode"].Value = "NULL";
                dgvCrawler.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcStatusCode"].Style.BackColor = Color.Empty;
                dgvCrawler.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcDiscoveryType"].Value = crawlRequest.Discovery.DiscoveryType;
                dgvCrawler.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcHyperLinkDiscoveries"].Value = crawlRequest.Discoveries.HyperLinks.Count;
                dgvCrawler.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcFileAndImageDiscoveries"].Value = crawlRequest.Discoveries.FilesAndImages.Count;
                dgvCrawler.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcCurrentDepth"].Value = crawlRequest.CurrentDepth;
                dgvCrawler.Rows[crawlRequest.Crawl.CrawlInfo.ThreadNumber - 1].Cells["dgvtbcMaximumDepth"].Value = crawlRequest.MaximumDepth;
            }
        }

        private void Engine_CrawlCompleted(Engine<ArachnodeDAO> engine)
        {
            _stopwatchForCrawler.Stop();

            BeginInvoke(new MethodInvoker(delegate()
                                              {
                                                  btnStart.Enabled = true;
                                                  btnStop.Enabled = false;
                                                  btnPause.Enabled = false;
                                                  btnPause.Enabled = false;
                                              }));
        }

        #endregion

        #region Menu Events

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.FileName = "CrawlRequests.txt";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string fileName = openFileDialog.FileName;

                        ParseCrawlRequestsDotText(fileName);
                    }
                }
            }
            catch (Exception exception)
            {
                rtbOutput.Text += Environment.NewLine + exception.Message + Environment.NewLine + exception.StackTrace;

                MessageBox.Show(exception.Message + Environment.NewLine + exception.StackTrace, _formText);
            }
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void resetDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Reset Database?", "Confirm Action", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                ResetDatabase();

                _crawler.ConsoleManager.OutputString("ResetDatabase: Complete");
            }
        }

        private void resetDirectoriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Reset Directories?", "Confirm Action", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                string consoleOutputLogsDirectory = Path.Combine(Environment.CurrentDirectory, "ConsoleOutputLogs");

                string downloadedFilesDirectory = Path.Combine(Environment.CurrentDirectory, "DownloadedFiles");
                string downloadedImagesDirectory = Path.Combine(Environment.CurrentDirectory, "DownloadedImages");
                string downloadedWebPagesDirectory = Path.Combine(Environment.CurrentDirectory, "DownloadedWebPages");
                string luceneDotNetIndexDirectory = Path.Combine(Environment.CurrentDirectory, "LuceneDotNetIndex");

                System.Console.ForegroundColor = ConsoleColor.Green;
                System.Console.WriteLine("Resetting Directories...");
                System.Console.ForegroundColor = ConsoleColor.White;

                System.Console.WriteLine("Resetting " + new DirectoryInfo(consoleOutputLogsDirectory).FullName + "...");
                try
                {
                    if (Directory.Exists(consoleOutputLogsDirectory))
                    {
                        Directories.DeleteDirectory(consoleOutputLogsDirectory);
                    }
                }
                catch (Exception exception)
                {
                    System.Console.WriteLine(exception.Message);
                }

                System.Console.WriteLine("Resetting " + new DirectoryInfo(downloadedFilesDirectory).FullName + "...");
                try
                {
                    if (Directory.Exists(downloadedFilesDirectory))
                    {
                        Directories.DeleteDirectory(downloadedFilesDirectory);
                    }
                }
                catch (Exception exception)
                {
                    System.Console.WriteLine(exception.Message);
                }

                System.Console.WriteLine("Resetting " + new DirectoryInfo(downloadedImagesDirectory).FullName + "...");
                try
                {
                    if (Directory.Exists(downloadedImagesDirectory))
                    {
                        Directories.DeleteDirectory(downloadedImagesDirectory);
                    }
                }
                catch (Exception exception)
                {
                    System.Console.WriteLine(exception.Message);
                }

                System.Console.WriteLine("Resetting " + new DirectoryInfo(downloadedWebPagesDirectory).FullName + "...");
                try
                {
                    if (Directory.Exists(downloadedWebPagesDirectory))
                    {
                        Directories.DeleteDirectory(downloadedWebPagesDirectory);
                    }
                }
                catch (Exception exception)
                {
                    System.Console.WriteLine(exception.Message);
                }

                System.Console.WriteLine("Resetting " + new DirectoryInfo(luceneDotNetIndexDirectory).FullName + "...");
                try
                {
                    if (Directory.Exists(luceneDotNetIndexDirectory))
                    {
                        Directories.DeleteDirectory(luceneDotNetIndexDirectory);
                    }
                }
                catch (Exception exception)
                {
                    System.Console.WriteLine(exception.Message);
                }

                _crawler.ConsoleManager.OutputString("ResetDirectories: Complete");
            }
        }

        private void resetCrawlerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Reset Crawler?", "Confirm Action", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                ResetCrawler();

                _crawler.ConsoleManager.OutputString("ResetCrawler: Complete");
            }
        }

        private void resetGUIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Reset GUI?", "Confirm Action", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                ResetGUI();

                _crawler.ConsoleManager.OutputString("ResetGUI: Complete");
            }
        }

        private void automatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("Arachnode.Automator.exe");
        }

        private void browserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("Arachnode.Browser.exe");
        }

        private void postProcessingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("Arachnode.PostProcessing.exe");
        }

        private void scraperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("Arachnode.Scraper.exe");
        }

        private void openCurrentDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Environment.CurrentDirectory);
        }

        private void notepadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("notepad.exe");
        }

        private void performanceMonitorMSCtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("PerformanceMonitor.msc");
        }

        private void sQLServerManagementStudioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("ssms.exe");
        }

        private void taskManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("taskmgr.exe");
        }        

        #endregion

        #region Context Menu Events
        private void tsmiUndo_Click(object sender, EventArgs e)
        {
            rtbOutput.Undo();
        }

        private void tsmiCut_Click(object sender, EventArgs e)
        {
            rtbOutput.Cut();
        }

        private void tsmiCopy_Click(object sender, EventArgs e)
        {
            rtbOutput.Copy();
        }

        private void tsmiPaste_Click(object sender, EventArgs e)
        {
            rtbOutput.Paste();
        }

        private int _selectionStart = 0;
        private int _selectionLength = 0;    

        private void tsmiDelete_Click(object sender, EventArgs e)
        {
            if(_selectionStart != 1)
            {
                rtbOutput.Text = rtbOutput.Text.Substring(0, _selectionStart) + rtbOutput.Text.Substring(_selectionStart, _selectionLength - _selectionStart);
            }
        }

        private void tsmiSelectAll_Click(object sender, EventArgs e)
        {
            rtbOutput.SelectAll();
        }
        #endregion

        private void rtbOutput_SelectionChanged(object sender, EventArgs e)
        {
            //_selectionStart = rtbOutput.SelectionStart;
            //_selectionLength = rtbOutput.SelectionLength;
        }        
    }
}