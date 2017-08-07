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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.Renderer.Value.Enums;
using Arachnode.SiteCrawler;
using Arachnode.SiteCrawler.Core;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.AbstractClasses;
using Arachnode.SiteCrawler.Value.Enums;
using Arachnode.Utilities;

#endregion

namespace Arachnode.Service
{
    public partial class Service1 : ServiceBase
    {
        private ApplicationSettings _applicationSettings = new ApplicationSettings();
        private WebSettings _webSettings = new WebSettings();

        private static Crawler<ArachnodeDAO> _crawler;
        private static readonly object _crawlRequestsLock = new object();
        private static FileSystemWatcher _fileSystemWatcher;

        /// <summary>
        /// 	Initializes a new instance of the <see cref = "Service1" /> class.
        /// </summary> 
        public Service1()
        {
            InitializeComponent();

            //README: To install the Service, open the VS2008 command prompt and type "installutil /I [Path to Arachnode.Service.exe]".
            //README: Use 'net start arachnode.net' and 'net stop arachnode.net' to start and stop the service while debugging.
            //README: If the Windows Service Manager is open, and viewable to the user, the machine may need to be rebooted to complete the installation or removal.
            //README: Thus, use 'net start/stop'.
        }

        internal void DebugOnStart(string[] args)
        {
            OnStart(args);
        }

        /// <summary>
        /// 	When implemented in a derived class, executes when a Start command is sent to the service by the Service Control Manager (SCM) or when the operating system starts (for a service that starts automatically). Specifies actions to take when the service starts.
        /// </summary>
        /// <param name = "args">Data passed by the start command.</param>
        protected override void OnStart(string[] args)
        {
            //If you receive a TypeInitializationException, copy ConnectionStrings.config from \Console\bin\[Debug/Release] to \Service\bin\[Debug/Release]...

            try
            {
                //remove limits from service point manager
                ServicePointManager.MaxServicePoints = 10000;
                ServicePointManager.DefaultConnectionLimit = 10000;
                ServicePointManager.CheckCertificateRevocationList = true;
                ServicePointManager.Expect100Continue = false;
                ServicePointManager.MaxServicePointIdleTime = 1000 * 30;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                ServicePointManager.UseNagleAlgorithm = false;

                //Use if you encounter certificate errors in the WebClient.
                //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

                new EventLog("Application", Environment.MachineName, "arachnode.net").WriteEntry("The arachnode.net service has started.", EventLogEntryType.Information);

                if (!File.Exists("ConnectionStrings.config"))
                {
                    if (File.Exists(@"..\..\..\Configuration\ConnectionStrings.config"))
                    {
                        File.Copy(@"..\..\..\Configuration\ConnectionStrings.config", "ConnectionStrings.config");
                    }
                }

                _crawler = new Crawler<ArachnodeDAO>(_applicationSettings, _webSettings, CrawlMode.BreadthFirstByPriority, false);

                if (!File.Exists("CrawlRequests.txt"))
                {
                    File.Create("CrawlRequests.txt");
                }

                _fileSystemWatcher = new FileSystemWatcher(Path.GetDirectoryName(Environment.CommandLine.Replace("\"", string.Empty)), "CrawlRequests.txt");
                _fileSystemWatcher.EnableRaisingEvents = true;
                _fileSystemWatcher.Changed += _fileSystemWatcher_Changed;

                //Be careful with this setting.  Setting this to ProcessPriorityClass.RealTime may lock the machine.
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Normal;

                ResetCrawler();

                //CrawlActions, CrawlRules and EngineActions can be set from code, overriding Database settings.
                OverrideDatabaseSettings();

                ParseCrawlRequests();

                _crawler.Engine.StopEngineOnCrawlCompleted = false;
                _crawler.Engine.Start();
                _crawler.Engine.CrawlRequestCompleted += Engine_CrawlRequestCompleted;
                _crawler.Engine.CrawlCompleted += Engine_CrawlCompleted;
            }
            catch (Exception exception)
            {
                new ArachnodeDAO(_applicationSettings.ConnectionString, _applicationSettings, _webSettings, false, false).InsertException(null, null, exception, true);

                OnStop();
            }
        }

        private static void LoadProxyServers()
        {
            string path = @"ProxyServers.txt";

            if (File.Exists(path))
            {
                Uri uri = new Uri("http://arachnode.net");
                string stringToVerify = uri.Host;

                List<Uri> proxyServers = Files.ExtractAbsoluteUris(path);

                int numberOfProxyServersDesired = proxyServers.Count;
                int proxyServerTimeout = 1000;

                if (numberOfProxyServersDesired > proxyServers.Count)
                {
                    numberOfProxyServersDesired = proxyServers.Count;
                }

                List<IWebProxy> proxyServers2 = _crawler.ProxyManager.LoadFastestProxyServers(proxyServers, proxyServerTimeout, numberOfProxyServersDesired, uri.AbsoluteUri, stringToVerify, true);

                if (proxyServers2 == null)
                {
                    proxyServers2 = new List<IWebProxy>();
                }

                while (numberOfProxyServersDesired - proxyServers2.Count != 0 && proxyServerTimeout < 10000)
                {
                    proxyServerTimeout += 1000;

                    var proxyServersToLoad = proxyServers.Where(ps => !proxyServers2.Select(ps2 => ps2.GetProxy(uri).AbsoluteUri).Contains(ps.AbsoluteUri)).ToList();

                    List<IWebProxy> proxyServers3 = _crawler.ProxyManager.LoadFastestProxyServers(proxyServersToLoad, proxyServerTimeout, numberOfProxyServersDesired - proxyServers2.Count, uri.AbsoluteUri, stringToVerify, false);

                    if (proxyServers3 != null)
                    {
                        foreach (IWebProxy iWebProxy in proxyServers3)
                        {
                            proxyServers2.Add(iWebProxy);
                        }
                    }
                }

                foreach (IWebProxy iWebProxy in proxyServers2)
                {
                    _crawler.ConsoleManager.OutputString("ProxyServer: " + iWebProxy.GetProxy(uri).AbsoluteUri);
                }
            }

            System.Console.Beep();
        }

        private void OverrideDatabaseSettings()
        {
            LoadProxyServers();

            foreach (ACrawlAction<ArachnodeDAO> crawlAction in _crawler.CrawlActions.Values)
            {
                if (crawlAction.TypeName != "Arachnode.Plugins.CrawlActions.ManageLuceneDotNetIndexes")
                {
                    crawlAction.IsEnabled = false;
                }
            }

            //cfg.CrawlRules
            foreach (ACrawlRule<ArachnodeDAO> crawlRule in _crawler.CrawlRules.Values)
            {
                crawlRule.IsEnabled = false;
            }

            //cfg.EngineActions
            foreach (AEngineAction<ArachnodeDAO> engineAction in _crawler.Engine.EngineActions.Values)
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
            _applicationSettings.ClassifyAbsoluteUris = false;
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
            _applicationSettings.ExtractFileMetaData = false;
            _applicationSettings.ExtractImageMetaData = false;
            _applicationSettings.ExtractWebPageMetaData = false;
            _applicationSettings.HttpWebRequestRetries = 5;
            _applicationSettings.InsertDisallowedAbsoluteUriDiscoveries = false;
            _applicationSettings.InsertDisallowedAbsoluteUris = true;
            _applicationSettings.InsertEmailAddressDiscoveries = false;
            _applicationSettings.InsertEmailAddresses = false;
            _applicationSettings.InsertExceptions = true;
            _applicationSettings.InsertFileDiscoveries = false;
            _applicationSettings.InsertFileMetaData = false;
            _applicationSettings.InsertFiles = true;
            _applicationSettings.InsertFileSource = false;
            _applicationSettings.InsertHyperLinkDiscoveries = false;
            _applicationSettings.InsertHyperLinks = false;
            _applicationSettings.InsertImageDiscoveries = false;
            _applicationSettings.InsertImageMetaData = false;
            _applicationSettings.InsertImages = true;
            _applicationSettings.InsertImageSource = false;
            _applicationSettings.InsertWebPageMetaData = false;
            _applicationSettings.InsertWebPages = true;
            _applicationSettings.InsertWebPageSource = false;
            _applicationSettings.MaximumNumberOfCrawlRequestsToCreatePerBatch = 1000;
            _applicationSettings.MaximumNumberOfCrawlThreads = _crawler.ProxyManager.Proxies.Count != 0 ? _crawler.ProxyManager.Proxies.Count : 10;
            _applicationSettings.MaximumNumberOfHostsAndPrioritiesToSelect = 10000;
            _applicationSettings.OutputConsoleToLogs = false;
            _applicationSettings.SaveDiscoveredFilesToDisk = true;
            _applicationSettings.SaveDiscoveredImagesToDisk = true;
            _applicationSettings.SaveDiscoveredWebPagesToDisk = true;
            _applicationSettings.UserAgent = "Your unique UserAgent string.";
            _applicationSettings.VerboseOutput = false;

            _crawler.ApplicationSettings = _applicationSettings;
        }

        private void _fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            ParseCrawlRequests();
        }

        private void ParseCrawlRequests()
        {
            try
            {
                lock (_crawlRequestsLock)
                {
                    string[] crawlRequests = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Environment.CommandLine.Replace("\"", string.Empty)), "CrawlRequests.txt"));

                    foreach (string crawlRequest in crawlRequests)
                    {
                        if (crawlRequest.Trim().StartsWith("//") || string.IsNullOrWhiteSpace(crawlRequest))
                        {
                            continue;
                        }

                        string[] crawlRequestSplit = crawlRequest.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                        UriClassificationType restrictCrawlTo = UriClassificationType.None;

                        foreach (string uriClassificationType in crawlRequestSplit[2].Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                        {
                            restrictCrawlTo |= (UriClassificationType) Enum.Parse(typeof (UriClassificationType), uriClassificationType);
                        }

                        UriClassificationType restrictDiscoveriesTo = UriClassificationType.None;

                        foreach (string uriClassificationType in crawlRequestSplit[3].Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                        {
                            restrictDiscoveriesTo |= (UriClassificationType) Enum.Parse(typeof (UriClassificationType), uriClassificationType);
                        }

                        CrawlRequest<ArachnodeDAO> crawlRequest2 = new CrawlRequest<ArachnodeDAO>(new Discovery<ArachnodeDAO>(crawlRequestSplit[0]), int.Parse(crawlRequestSplit[1]), restrictCrawlTo, restrictDiscoveriesTo, double.Parse(crawlRequestSplit[4]), (RenderType)Enum.Parse(typeof(RenderType), crawlRequestSplit[5]), (RenderType)Enum.Parse(typeof(RenderType), crawlRequestSplit[6]));

                        _crawler.Crawl(crawlRequest2);
                    }
                }
            }
            catch (Exception exception)
            {
                new ArachnodeDAO(_applicationSettings.ConnectionString, _applicationSettings, _webSettings, false, false).InsertException(null, null, exception, true);
            }
        }

        /// <summary>
        /// 	When implemented in a derived class, executes when a Stop command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service stops running.
        /// </summary>
        protected override void OnStop()
        {
            _crawler.Engine.Stop();

            new EventLog("Application", Environment.MachineName, "arachnode.net").WriteEntry("The arachnode.net service has stopped.", EventLogEntryType.Information);
        }

        protected override void OnPause()
        {
            base.OnPause();

            _crawler.Engine.Pause();

            new EventLog("Application", Environment.MachineName, "arachnode.net").WriteEntry("The arachnode.net service has paused.", EventLogEntryType.Information);
        }

        protected override void OnContinue()
        {
            base.OnContinue();

            _crawler.Engine.Resume();

            new EventLog("Application", Environment.MachineName, "arachnode.net").WriteEntry("The arachnode.net service has resumed.", EventLogEntryType.Information);
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();

            new EventLog("Application", Environment.MachineName, "arachnode.net").WriteEntry("The arachnode.net service is stopping due to system shutdown.", EventLogEntryType.Warning);

            _crawler.Engine.Stop();
        }

        private void Engine_CrawlRequestCompleted(CrawlRequest<ArachnodeDAO> crawlRequest)
        {
        }

        /// <summary>
        /// 	Handles the OnCrawlCompleted event of the Engine control.
        /// </summary>
        private void Engine_CrawlCompleted(Engine<ArachnodeDAO> engine)
        {
            //logic for re-crawling a list of CrawlRequests from CrawlRequests.txt.

            new EventLog("Application", Environment.MachineName, "arachnode.net").WriteEntry("The arachnode.net Crawl has completed.", EventLogEntryType.Information);

            try
            {
                //double locking is used to coordinate a possible SCM pause and changes to the CrawlRequests.txt file...
                //we want to ensure that if CrawlRequests.txt has been modified that it will wait until our work here is done, or vice versa, and
                //if we receive a SCM message for Pause, that we honor this Pause and allow the event to be refired from the Engine once Resume or Stop is called...
                if (_crawler.Engine.State == EngineState.Start)
                {
                    lock (_crawlRequestsLock)
                    {
                        if (_crawler.Engine.State == EngineState.Start)
                        {
                            //all crawl theads will complete their current action before pause completes.
                            //the double locking above should never occur as Pause DOES wait for all Crawls to stop at the ResetEvent.
                            _crawler.Engine.Pause();

                            //resets the state of the Crawler to as it was when the service was first started...
                            ResetCrawler();

                            //resets the Actions and Rules, loading settings from the database...
                            _crawler.ResetCrawlActions();
                            _crawler.ResetCrawlRules();

                            //provides application specific settings, like in Console\Program.cs...
                            OverrideDatabaseSettings();

                            //read the CrawlRequests from CrawlRequests.txt...
                            ParseCrawlRequests();

                            //resume the Engine...
                            _crawler.Engine.Resume();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                new ArachnodeDAO(_applicationSettings.ConnectionString, _applicationSettings, _webSettings, false, false).InsertException(null, null, exception, true);
            }
        }

        private void ResetCrawler()
        {
            //clear the Discoveries...
            _crawler.ClearDiscoveries();

            //clear the Politeness...
            _crawler.ClearPolitenesses();

            //clear the uncrawled CrawlRequests...
            _crawler.ClearUncrawledCrawlRequests();

            //clear the DisallowedAbsoluteUris table...
            _crawler.ClearDisallowedAbsoluteUris();
        }
    }
}