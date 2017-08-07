#region License : arachnode.net

// Copyright (c) 2015 http://arachnode.net, arachnode.net, LLC
//  
// Permission is hereby granted, upon purchase, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, merge and modify copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following
// conditions:
// 
// LICENSE (ALL VERSIONS/EDITIONS): http://arachnode.net/r.ashx?3
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Arachnode.Configuration;
using Arachnode.Console.Override;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Managers;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.Plugins.CrawlActions;
using Arachnode.Renderer.Value.Enums;
using Arachnode.SiteCrawler;
using Arachnode.SiteCrawler.Core;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.AbstractClasses;
using Arachnode.SiteCrawler.Value.Enums;
using Arachnode.Utilities;
using Arachnode.SiteCrawler.Managers;
using Arachnode.Security;

#endregion

namespace Arachnode.Console
{
    internal class Program
    {
        //Installation Instructions: http://arachnode.net/Content/InstallationInstructions.aspx

        private static ApplicationSettings _applicationSettings = new ApplicationSettings();
        private static WebSettings _webSettings = new WebSettings();

        private static Crawler<ArachnodeDAO> _crawler;
        private static Stopwatch _stopwatch;
        private static bool _hasCrawlCompleted;

        //to generate the test site pages for the 'http://localhost:56830/Test/1.htm', execute '\Web\Test\TestSite.exe'

#if DEBUG
        private static bool? _generateTheTestSite = false;
        private static bool? _resetDatabase = false;
        private static bool? _resetDirectories = false;
        private static bool? _resetCrawler = false;
        private static bool? _clearIECache = false;
        private static bool? _resetIIS = false;
        private static bool? _startPerfmon = false;
        private static bool? _crawlTheTestSite = false;
#endif
#if DEMO
        private static bool? _generateTheTestSite = false;
        private static bool? _resetDatabase = true;
        private static bool? _resetDirectories = false;
        private static bool? _resetCrawler = true;
        private static bool? _clearIECache = false;
        private static bool? _resetIIS = false;
        private static bool? _startPerfmon = false;
        private static bool? _crawlTheTestSite = false;
#endif

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {            
            try
			{
                //README: Program.cs contains information/code necessary to understand how arachnode.net functions.  Please read this file.
                //README: If you receive the following error, execute the following T-SQL command: "EXEC [dbo].[arachnode_usp_arachnode.net_RESET_DATABASE]".
                //Error	1	CREATE ASSEMBLY for assembly 'Arachnode.Functions' failed because assembly 'Arachnode.Functions' is not authorized for PERMISSION_SET = UNSAFE.  The assembly is authorized when either of the following is true: the database owner (DBO) has UNSAFE ASSEMBLY permission and the database has the TRUSTWORTHY database property on; or the assembly is signed with a certificate or an asymmetric key that has a corresponding login with UNSAFE ASSEMBLY permission.	Functions

                //remove limits from service point manager
                ServicePointManager.MaxServicePoints = 10000;
                ServicePointManager.DefaultConnectionLimit = 10000;
                ServicePointManager.CheckCertificateRevocationList = true;
                ServicePointManager.Expect100Continue = false;
                ServicePointManager.MaxServicePointIdleTime = 1000*30;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                ServicePointManager.UseNagleAlgorithm = false;

				//Use if you encounter certificate errors in the WebClient.
                //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

                _stopwatch = new Stopwatch();

                //HELP: http://arachnode.net/media/p/41.aspx
                //HELP: http://arachnode.net/Content/FrequentlyAskedQuestions.aspx
                //HELP: http://arachnode.net/forums/

                //README: Connection string configuration: Project 'Configuration'\ConnectionStrings.config.

                #region !RELEASE helper code 1.

#if !RELEASE
                System.Console.WriteLine("arachnode.net !RELEASE helper code.");
                System.Console.WriteLine("Always run Visual Studio 2008/2010 as an Administrator.");
#if DEMO
                System.Console.ForegroundColor = ConsoleColor.Yellow;
                System.Console.WriteLine("\nDefault values have been supplied for the following prompts.");
                System.Console.WriteLine("Examine the private member variables in Program.cs to customize.  (<Type>?)");
                System.Console.ForegroundColor = ConsoleColor.Gray;
#endif
                System.Console.WriteLine("\nGenerate the TestSite? (y/n)");
                System.Console.WriteLine("  -> http://localhost:56830/Test/1.htm (14606 WebPages)");

                if (_generateTheTestSite.HasValue)
                {
                    System.Console.WriteLine(_generateTheTestSite.Value);
                }
                if ((_generateTheTestSite.HasValue && _generateTheTestSite.Value) || (!_generateTheTestSite.HasValue && System.Console.ReadLine().ToLower() == "y"))
                {
                    System.Console.ForegroundColor = ConsoleColor.Green;
                    System.Console.WriteLine("Generating " + 14606 + " WebPages...");
                    System.Console.ForegroundColor = ConsoleColor.Gray;

                    ProcessStartInfo processStartInfo = new ProcessStartInfo("TestSite.exe");
                    processStartInfo.WorkingDirectory = "..\\..\\..\\Web\\Test";

                    Process process = Process.Start(processStartInfo);

                    process.WaitForExit();
                }

                System.Console.WriteLine("\nReset Database:");
                System.Console.WriteLine("  -> EXEC [dbo].[arachnode_usp_arachnode.net_RESET_DATABASE] (SQL).");
                System.Console.WriteLine("  -> Resets all user data (SQL).");

                System.Console.WriteLine("\nInitial setup tasks:");
                System.Console.WriteLine("  -> Populate settings for missing values in cfg.Configuration (SQL).");
                System.Console.WriteLine("  -> Populate settings for missing values in cfg.CrawlActions (SQL).");

                System.Console.WriteLine("\nReset Database and perform initial setup tasks?  (y/n)");

                //README: This directory will not contain files unless 'OutputConsoleToLogs' is 'true', and 'EnableConsoleOutput' is 'true'.
                string consoleOutputLogsDirectory = Path.Combine(Environment.CurrentDirectory, "ConsoleOutputLogs");

                //README: These directories are necessary if you wish to store downloaded Discoveries on disk, and not in the database.
                string downloadedFilesDirectory = Path.Combine(Environment.CurrentDirectory, "DownloadedFiles");
                string downloadedImagesDirectory = Path.Combine(Environment.CurrentDirectory, "DownloadedImages");
                string downloadedWebPagesDirectory = Path.Combine(Environment.CurrentDirectory, "DownloadedWebPages");
                string luceneDotNetIndexDirectory = Path.Combine(Environment.CurrentDirectory, "LuceneDotNetIndex");

                if (_resetDatabase.HasValue)
                {
                    System.Console.WriteLine(_resetDatabase.Value);
                }
                if ((_resetDatabase.HasValue && _resetDatabase.Value) || (!_resetDatabase.HasValue && System.Console.ReadLine().ToLower() == "y"))
                {
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
                }

                System.Console.WriteLine("\nReset Directories:");
                System.Console.WriteLine("  -> Deletes cfg.Configuration.ConsoleOutputLogsDirectory (Filesystem/Disk).");
                System.Console.WriteLine("  -> Deletes cfg.Configuration.DownloadedFilesDirectory (Filesystem/Disk).");
                System.Console.WriteLine("  -> Deletes cfg.Configuration.DownloadedImagesDirectory (Filesystem/Disk).");
                System.Console.WriteLine("  -> Deletes cfg.Configuration.DownloadedWebPagesDirectory (Filesystem/Disk).");
                System.Console.WriteLine("  -> Deletes cfg.Configuration.LuceneDotNetIndexDirectory (Filesystem/Disk).");
                System.Console.WriteLine("  -> Resets all user data (Filesystem/Disk).\n");

                System.Console.WriteLine("Reset Directories?  (y/n)");

                if (_resetDirectories.HasValue)
                {
                    System.Console.WriteLine(_resetDirectories.Value);
                }
                if ((_resetDirectories.HasValue && _resetDirectories.Value) || (!_resetDirectories.HasValue && System.Console.ReadLine().ToLower() == "y"))
                {
                    try
                    {
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

                        System.Console.ForegroundColor = ConsoleColor.White;
                    }
                    catch (Exception)
                    {
                    }
                }
#endif
                System.Console.WriteLine();
#endregion

                #region multi-server caching...

                List<CrawlerPeer> crawlerPeers = new List<CrawlerPeer>();

                //crawlerPeers.Add(new CrawlerPeer(IPAddress.Parse("192.168.233.1"), 8080, true, true, 10, true));
//                crawlerPeers.Add(new CrawlerPeer(IPAddress.Parse("192.168.233.128"), 8081, false, true, 10, false));
                //crawlerPeers.Add(new CrawlerPeer(IPAddress.Parse("192.168.233.129"), 8082, false, false, 10, false));

                //List<DatabasePeer> databasePeers = new List<DatabasePeer>();

                //DatabasePeer databasePeer = new DatabasePeer(ApplicationSettings.ConnectionString);

                //databasePeers.Add(databasePeer);

                #endregion

                CookieContainer cookieContainer = new CookieContainer();
                CredentialCache credentialCache = new CredentialCache();

                _crawler = new Crawler<ArachnodeDAO>(_applicationSettings, _webSettings, CrawlMode.DepthFirstByPriority, cookieContainer, credentialCache, crawlerPeers, null, false);

                /**/

                #region authentication / custom cookie processing...

                //CookieCollection cookieCollection = _crawler.CookieManager.BuildCookieCollection("[CookieValueHere]");

                //_crawler.CookieManager.AddCookieCollectionToCookieContainer("https://arachnode.net", cookieContainer, cookieCollection);

                //_crawler.CredentialCache.Add(new Uri("http://arachnode.net/"), "Basic", new NetworkCredential("[UserNameValueHere]", "[PasswordValueHere]", "arachnode.net"));

                #endregion

                //as every 'Manager' is assignable, you are able to override core crawling functionality with your own customizations.
                //the reference chain must be followed up to the parent Manager type.
                //_crawler.DiscoveryManager = new CustomDiscoveryManager(_crawler.Cache, _crawler.ActionManager, _crawler.CacheManager, _crawler.MemoryManager, _crawler.RuleManager);
                //_crawler.CrawlRequestManager = new CrawlRequestManager<ArachnodeDAO>(_crawler.Cache, _crawler.ConsoleManager, _crawler.DiscoveryManager);

                //also, you may specify a new IArachnodeDAO.
                //e.g. _crawler = new Crawler<ArachnodeDAOMongo>(CrawlMode.DepthFirstByPriority, false);

                #region !RELEASE helper code 2.

                //wire up an event handler to detect application exit and shutdown events.
                System.Console.TreatControlCAsInput = false;
                System.Console.CancelKeyPress += Console_CancelKeyPress;

                System.Console.ForegroundColor = ConsoleColor.Green;
                System.Console.WriteLine("Press CTRL+C to properly terminate the Console.");
                System.Console.ForegroundColor = ConsoleColor.Gray;

#if !RELEASE
                System.Console.WriteLine("\nReset Crawler:");
                System.Console.WriteLine("  -> Deletes Discoveries in RAM and dbo.Discoveries (SQL).");
                System.Console.WriteLine("  -> Deletes Politenesses in RAM.");
                System.Console.WriteLine("  -> Deletes dbo.CrawlRequests (SQL).");
                System.Console.WriteLine("  -> Deletes dbo.DisallowedAbsoluteUris (SQL).");
                System.Console.WriteLine("  -> Retains all user data (SQL and Filesystem/Disk).\n");

                System.Console.WriteLine("Reset Crawler?  (y/n)");

                if (_resetCrawler.HasValue)
                {
                    System.Console.WriteLine(_resetCrawler.Value);
                }
                if ((_resetCrawler.HasValue && _resetCrawler.Value) || (!_resetCrawler.HasValue && System.Console.ReadLine().ToLower() == "y"))
                {
                    System.Console.ForegroundColor = ConsoleColor.Green;
                    System.Console.WriteLine("Resetting Crawler...");
                    System.Console.ForegroundColor = ConsoleColor.Gray;

                    _crawler.Reset();
                }

                System.Console.WriteLine("\nClear IE's cache?  (Applicable if using the Renderers and need cachable content (re-)downloaded with eash request.)  (y/n)");

                if (_clearIECache.HasValue)
                {
                    System.Console.WriteLine(_clearIECache.Value);
                }
                if ((_clearIECache.HasValue && _clearIECache.Value) || (!_clearIECache.HasValue && System.Console.ReadLine().ToLower() == "y"))
                {
                    Internet.ClearIECache();
                }

                //useful when testing crawler performance against [Web Project]\Test.aspx
                System.Console.WriteLine("\nReset IIS?  (Applicable if running the Web applications under IIS.)  (y/n)");

                if (_resetIIS.HasValue)
                {
                    System.Console.WriteLine(_resetIIS.Value);
                }
                if ((_resetIIS.HasValue && _resetIIS.Value) || (!_resetIIS.HasValue && System.Console.ReadLine().ToLower() == "y"))
                {
                    System.Console.ForegroundColor = ConsoleColor.Green;
                    System.Console.WriteLine("Resetting IIS...");
                    System.Console.ForegroundColor = ConsoleColor.Gray;

                    Internet.ResetIIS();
                }

                //arachnode.net provides performance counters in three categories.
                // 1.) ArachnodeDAOMongo (how is the Database performing)
                // 2.) Cache (how is the Cache performing)
                // 3.) Crawl (how is the Crawl performing)
                //http://arachnode.net/blogs/arachnode_net/archive/2011/05/05/performance-counters.aspx
                System.Console.WriteLine("\nStart perfmon.exe and monitor arachnode.net's performance?  (y/n)");

                if (_startPerfmon.HasValue)
                {
                    System.Console.WriteLine(_startPerfmon.Value);
                }
                if ((_startPerfmon.HasValue && _startPerfmon.Value) || (!_startPerfmon.HasValue && System.Console.ReadLine().ToLower() == "y"))
                {
                    Process.Start("perfmon.exe");
                }
#endif
                #endregion

                /***********************************/

                /**/

                if (_crawler.Engine != null)
                {
                    /**/

                    //assign settings before starting a crawl
#if DEBUG
                    AssignApplicationSettingsForDebug();
#endif

#if DEMO
                    AssignApplicationSettingsForDemo();
#endif

#if RELEASE
                    AssignApplicationSettingsForRelease();
#endif
                    ConfigurationManager.CheckForIncorrectConfigurationValues(_applicationSettings);

                    /**/

                    _crawler.Engine.CrawlRequestCanceled += Engine_CrawlRequestCanceled;
                    _crawler.Engine.CrawlRequestCompleted += Engine_CrawlRequestCompleted;
                    _crawler.Engine.CrawlRequestRetrying += Engine_CrawlRequestRetrying;
                    _crawler.Engine.CrawlRequestThrottled += Engine_CrawlRequestThrottled;
                    _crawler.Engine.CrawlCompleted += Engine_CrawlCompleted;

                    /**/

                    //add crawl requests

                    //A depth of 1 means 'crawl this page only'.  A depth of two means crawl the first page, and follow every HyperLink from the first page.
                    //Setting the Depth to int.Max means to crawl the first page, and then int.MaxValue - 1 hops away from the initial CrawlRequest AbsoluteUri.
                    //The higher the value for 'Priority', the higher the Priority.
#if !RELEASE
                    System.Console.WriteLine("\nCrawl the TestSite? (y/n)");
                    System.Console.WriteLine("  -> http://localhost:56830/Test/1.htm (14606 WebPages)");

                    int depth = 1;
                    UriClassificationType restrictCrawlTo = UriClassificationType.Domain;
                    UriClassificationType restrictDiscoveriesTo = UriClassificationType.None;
                    RenderType renderType = _crawler.AreRenderersEnabled ? RenderType.Render : RenderType.None;
                    RenderType renderTypeForChildren = renderType;

                    bool wasTheCrawlRequestAddedForCrawling;
                    int count = 0;

                    if (_crawlTheTestSite.HasValue)
                    {
                        System.Console.WriteLine(_crawlTheTestSite.Value);
                    }
                    if ((_crawlTheTestSite.HasValue && _crawlTheTestSite.Value) || (!_crawlTheTestSite.HasValue && System.Console.ReadLine().ToLower() == "y"))
                    {
                        for (int i = 1; i <= 14606; i++)
                        {
                            wasTheCrawlRequestAddedForCrawling = _crawler.Crawl(new CrawlRequest<ArachnodeDAO>(new Discovery<ArachnodeDAO>("http://localhost:56830/Test/" + i + ".htm"), depth, restrictCrawlTo, restrictDiscoveriesTo, 1, renderType, renderTypeForChildren));
                            //wasTheCrawlRequestAddedForCrawling = _crawler.Crawl(new CrawlRequest(new Discovery("http://test.arachnode.net/" + i + ".htm"), depth, restrictCrawlTo, restrictDiscoveriesTo, 1, renderType, renderTypeForChildren));
#if !DEMO
                            //helpful if you are loading a large list of AbsoluteUris/CrawlRequests and don't want to wait for the full list to load before starting the crawl...
                            if (++count == 1000)
                            {
                                _stopwatch.Start();
                                _crawler.Engine.Start();
                            }
#endif
                        }
                    }
                    else
                    {
                        //READ ME: http://arachnode.net/blogs/arachnode_net/archive/2010/04/29/troubleshooting-crawl-result-differences-between-different-crawl-environments.aspx

                        foreach (string crawlRequest in File.ReadAllLines("CrawlRequests.txt"))
                        {
                            if (crawlRequest.Trim().StartsWith("//") || string.IsNullOrWhiteSpace(crawlRequest))
                            {
                                continue;
                            }

                            try
                            {
                                string[] crawlRequestSplit = crawlRequest.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                                restrictCrawlTo = UriClassificationType.None;

                                foreach (string uriClassificationType in crawlRequestSplit[2].Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                                {
                                    restrictCrawlTo |= (UriClassificationType)Enum.Parse(typeof(UriClassificationType), uriClassificationType);
                                }

                                restrictDiscoveriesTo = UriClassificationType.None;

                                foreach (string uriClassificationType in crawlRequestSplit[3].Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                                {
                                    restrictDiscoveriesTo |= (UriClassificationType)Enum.Parse(typeof(UriClassificationType), uriClassificationType);
                                }

                                if (crawlRequest.StartsWith("http://") || crawlRequest.StartsWith("https://"))
                                {
                                    wasTheCrawlRequestAddedForCrawling = _crawler.Crawl(new CrawlRequest<ArachnodeDAO>(new Discovery<ArachnodeDAO>(crawlRequestSplit[0]), int.Parse(crawlRequestSplit[1]), restrictCrawlTo, restrictDiscoveriesTo, double.Parse(crawlRequestSplit[4]), (RenderType)Enum.Parse(typeof(RenderType), crawlRequestSplit[5]), (RenderType)Enum.Parse(typeof(RenderType), crawlRequestSplit[6])));
                                }
                                else
                                {
                                    wasTheCrawlRequestAddedForCrawling = _crawler.Crawl(new CrawlRequest<ArachnodeDAO>(new Discovery<ArachnodeDAO>("http://" + crawlRequestSplit[0]), int.Parse(crawlRequestSplit[1]), restrictCrawlTo, restrictDiscoveriesTo, double.Parse(crawlRequestSplit[4]), (RenderType)Enum.Parse(typeof(RenderType), crawlRequestSplit[5]), (RenderType)Enum.Parse(typeof(RenderType), crawlRequestSplit[6])));
                                }
                            }
                            catch (Exception exception)
                            {
                                System.Console.WriteLine(exception.Message + ":" + crawlRequest);
                            }

                            //helpful if you are loading a large list of AbsoluteUris/CrawlRequests and don't want to wait for the full list to load before starting the crawl...
                            if (++count == 1000)
                            {
                                _stopwatch.Start();
                                _crawler.Engine.Start();
                            }
                        }
#if !DEMO
                        
#endif
                    }
#endif
                    /**/

                    //If you stop a Crawl all CrawlRequests and Discoveries will be saved to the CrawlRequests and Discoveries tables.  These tables are used to maintain Crawl state and should not be modified by the user.

                    //If you are questioning why an AbsoluteUri wasn't crawled, check the database tables 'DisallowedAbsoluteUris' and 'Exceptions'.
#if DEMO
                    _crawler.ConsoleManager.OutputString("\narachnode.net DEMO restrictions:", ConsoleColor.White, ConsoleColor.Gray);
                    _crawler.ConsoleManager.OutputString("1.) Crawl limited to 2 minutes.", ConsoleColor.White, ConsoleColor.Gray);
                    _crawler.ConsoleManager.OutputString("2.) Crawl rate limited by decompilation protection.", ConsoleColor.White, ConsoleColor.Gray);
                    _crawler.ConsoleManager.OutputString("The LICENSED version does not impose these restrictions.", ConsoleColor.White, ConsoleColor.Gray);
                    _crawler.ConsoleManager.OutputString("Press [ENTER] to continue.", ConsoleColor.White, ConsoleColor.Gray);

                    System.Console.ReadLine();
#endif
                    //common task of parsing a WebPage for scraping...
                    //_crawler.AddCrawlAction(new BusinessInformation(), CrawlActionType.PostRequest, true, int.MaxValue);
                    
                    _stopwatch.Start();
                    _crawler.ConsoleManager.OutputString("Elapsed: " + _stopwatch.Elapsed + "\n");

                    //add all CrawlRequests before starting the Engine...
                    if (_crawler.ProxyManager.GetNextProxy() != null)
                    {
                        _crawler.ApplicationSettings = _crawler.ApplicationSettings;
                        _crawler.Engine.Start();
                    }
                    else
                    {
                        _crawler.ConsoleManager.OutputString("No valid ProxyServers.  Stopping Engine...", ConsoleColor.Red, ConsoleColor.Yellow);
                    }
                }
            }
            catch (Exception exception)
            {
                System.Console.WriteLine("\n" + exception.StackTrace + "\n");
                System.Console.ForegroundColor = ConsoleColor.Yellow;
                System.Console.WriteLine(exception.Message);
                System.Console.ForegroundColor = ConsoleColor.Gray;
            }

            /**/

            //necessary for the Rendering functionality.
            if (_crawler != null)
            {
                //may be null if all configuration settings are not initialized in the database...
                while (!_hasCrawlCompleted && _crawler.AreRenderersEnabled)
                {
                    Application.DoEvents();

                    Thread.Sleep(100);
                }
            }

            System.Console.ReadLine();

            if (_crawler != null && _crawler.Engine != null)
            {
                _crawler.Engine.Stop();
            }

            //if you would like to view Files and Images when running the Web project, see here: http://arachnode.net/forums/p/1027/12031.aspx
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
                            //if (proxyServers2.Count(ps => ps.GetProxy(uri).AbsoluteUri == iWebProxy.GetProxy(uri).AbsoluteUri) == 0 
                            //    || iWebProxy.GetProxy(uri).ToString() == IPAddress.Loopback.ToString())
                            //{
                                proxyServers2.Add(iWebProxy);
                            //}
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

        private static void AssignApplicationSettingsForDebug()
        {
            LoadProxyServers();

            //CrawlActions, CrawlRules and EngineActions can be added and set from code, augmenting and/or overriding Database settings.

            //cfg.CrawlActions
            foreach (ACrawlAction<ArachnodeDAO> crawlAction in _crawler.CrawlActions.Values)
            {
                if (crawlAction.TypeName != "Arachnode.Plugins.CrawlActions.ManageLuceneDotNetIndexes" && crawlAction.TypeName != "Arachnode.Plugins.CrawlActions.BusinessInformation")
                {
                    crawlAction.IsEnabled = false;
                }
            }

            //cfg.CrawlRules
            //CrawlRules limit where the Crawl can go, where not limited by 'UriClassificationType' in the CrawlRequest constructor.
            foreach (ACrawlRule<ArachnodeDAO> crawlRule in _crawler.CrawlRules.Values)
            {
                if (crawlRule.TypeName != "Arachnode.Plugins.CrawlRules.DataType")
                {
                    crawlRule.IsEnabled = false;
                }
            }

            //cfg.EngineActions
            //EngineActions are invoked when CrawlRequests are retrived from the database.  Most users wil not need these.
            foreach (AEngineAction<ArachnodeDAO> engineAction in _crawler.Engine.EngineActions.Values)
            {
                engineAction.IsEnabled = false;
            }

            //applicationSettings can be set from code, overriding Database settings found in cfg.Configuration.
            _crawler.ApplicationSettings.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            _crawler.ApplicationSettings.AcceptEncoding = "gzip,deflate,sdch";
            _crawler.ApplicationSettings.AllowAutoRedirect = true;
            _crawler.ApplicationSettings.AssignCrawlRequestPrioritiesForFiles = true;
            _crawler.ApplicationSettings.AssignCrawlRequestPrioritiesForHyperLinks = true;
            _crawler.ApplicationSettings.AssignCrawlRequestPrioritiesForImages = true;
            _crawler.ApplicationSettings.AssignCrawlRequestPrioritiesForWebPages = true;
            _crawler.ApplicationSettings.AssignEmailAddressDiscoveries = false;
            _crawler.ApplicationSettings.AssignFileAndImageDiscoveries = true;
            _crawler.ApplicationSettings.AssignHyperLinkDiscoveries = true;
            _crawler.ApplicationSettings.AutoThrottleHttpWebRequests = false;
            _crawler.ApplicationSettings.ClassifyAbsoluteUris = false;
            //_crawler.ApplicationSettings.ConnectionString = "";
            //_crawler.ApplicationSettings.ConsoleOutputLogsDirectory = "";
            _crawler.ApplicationSettings.CrawlRequestTimeoutInMinutes = 1;
            _crawler.ApplicationSettings.CreateCrawlRequestsFromDatabaseCrawlRequests = true;
            _crawler.ApplicationSettings.CreateCrawlRequestsFromDatabaseFiles = false;
            _crawler.ApplicationSettings.CreateCrawlRequestsFromDatabaseHyperLinks = false;
            _crawler.ApplicationSettings.CreateCrawlRequestsFromDatabaseImages = false;
            _crawler.ApplicationSettings.CreateCrawlRequestsFromDatabaseWebPages = false;
            _crawler.ApplicationSettings.DesiredMaximumMemoryUsageInMegabytes = 4096;
            _crawler.ApplicationSettings.DiscoverySlidingExpirationInSeconds = 120;
            //_crawler.ApplicationSettings.DownloadedFilesDirectory = "";
            //_crawler.ApplicationSettings.DownloadedImagesDirectory = "";
            //_crawler.ApplicationSettings.DownloadedWebPagesDirectory = "";
            _crawler.ApplicationSettings.EnableConsoleOutput = true;
            _crawler.ApplicationSettings.ExtractFileMetaData = false;
            _crawler.ApplicationSettings.ExtractImageMetaData = false;
            _crawler.ApplicationSettings.ExtractWebPageMetaData = false;
            _crawler.ApplicationSettings.HttpWebRequestRetries = 5;
            _crawler.ApplicationSettings.InsertCrawlRequests = true;
            _crawler.ApplicationSettings.InsertDisallowedAbsoluteUriDiscoveries = false;
            _crawler.ApplicationSettings.InsertDisallowedAbsoluteUris = true;
            _crawler.ApplicationSettings.InsertDisallowedDiscoveries = false;
            _crawler.ApplicationSettings.InsertDiscoveries = true;
            _crawler.ApplicationSettings.InsertEmailAddressDiscoveries = false;
            _crawler.ApplicationSettings.InsertEmailAddresses = false;
            _crawler.ApplicationSettings.InsertExceptions = true;
            _crawler.ApplicationSettings.InsertFileDiscoveries = true;
            _crawler.ApplicationSettings.InsertFileMetaData = false;
            _crawler.ApplicationSettings.InsertFiles = true;
            _crawler.ApplicationSettings.InsertFileResponseHeaders = true;
            _crawler.ApplicationSettings.InsertFileSource = false;
            _crawler.ApplicationSettings.InsertHyperLinkDiscoveries = false;
            _crawler.ApplicationSettings.InsertHyperLinks = true;
            _crawler.ApplicationSettings.InsertImageDiscoveries = true;
            _crawler.ApplicationSettings.InsertImageMetaData = false;
            _crawler.ApplicationSettings.InsertImages = true;
            _crawler.ApplicationSettings.InsertImageResponseHeaders = true;
            _crawler.ApplicationSettings.InsertImageSource = false;
            _crawler.ApplicationSettings.InsertWebPageMetaData = false;
            _crawler.ApplicationSettings.InsertWebPages = true;
            _crawler.ApplicationSettings.InsertWebPageResponseHeaders = true;
            _crawler.ApplicationSettings.InsertWebPageSource = false;
            _crawler.ApplicationSettings.MaximumNumberOfAutoRedirects = 50;
            _crawler.ApplicationSettings.MaximumNumberOfCrawlRequestsToCreatePerBatch = 1000;
            _crawler.ApplicationSettings.MaximumNumberOfCrawlThreads = _crawler.ProxyManager.Proxies.Count != 0 ? _crawler.ProxyManager.Proxies.Count : 10;
            _crawler.ApplicationSettings.MaximumNumberOfHostsAndPrioritiesToSelect = 10000;
            _crawler.ApplicationSettings.OutputConsoleToLogs = false;
            _crawler.ApplicationSettings.OutputWebExceptions = true;
            _crawler.ApplicationSettings.ProcessCookies = true;
            _crawler.ApplicationSettings.ProcessDiscoveriesAsynchronously = false;
            _crawler.ApplicationSettings.SaveDiscoveredFilesToDisk = true;
            _crawler.ApplicationSettings.SaveDiscoveredImagesToDisk = true;
            _crawler.ApplicationSettings.SaveDiscoveredWebPagesToDisk = true;
            _crawler.ApplicationSettings.SetRefererToParentAbsoluteUri = true;
            _crawler.ApplicationSettings.UniqueIdentifier = "";
            _crawler.ApplicationSettings.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/534.24 (KHTML, like Gecko) Chrome/11.0.696.71 Safari/534.24"; //If you find yourself blocked from crawling a website, change this to a common crawler string, such as 'Googlebot' or 'Slurp'...
            _crawler.ApplicationSettings.VerboseOutput = false;
            //enable VerboseOutput to see each Discovery and the the status of each Discovery returned from each Discovery.  (e.g. WebPages from each WebPage and Files/Images from each WebPage.)
        }

        private static void AssignApplicationSettingsForDemo()
        {
            LoadProxyServers();

            //CrawlActions, CrawlRules and EngineActions can be added and set from code, augmenting and/or overriding Database settings.

            //cfg.CrawlActions
            //for demo purposes, all CrawlActions are disabled except for 'Arachnode.Plugins.CrawlActions.ManageLuceneDotNetIndexes'.
            foreach (ACrawlAction<ArachnodeDAO> crawlAction in _crawler.CrawlActions.Values)
            {
                if (crawlAction.TypeName != "Arachnode.Plugins.CrawlActions.ManageLuceneDotNetIndexes" && crawlAction.TypeName != "Arachnode.Plugins.CrawlActions.BusinessInformation")
                {
                    crawlAction.IsEnabled = false;
                }
            }

            //cfg.CrawlRules
            //for demo purposes, all CrawlRules are disabled.
            //CrawlRules limit where the Crawl can go, where not limited by 'UriClassificationType' in the CrawlRequest constructor.
            foreach (ACrawlRule<ArachnodeDAO> crawlRule in _crawler.CrawlRules.Values)
            {
                if (crawlRule.TypeName != "Arachnode.Plugins.CrawlRules.DataType")
                {
                    crawlRule.IsEnabled = false;
                }
            }

            //cfg.EngineActions
            //for demo purposes, all EngineActions are disabled.
            //EngineActions are invoked when CrawlRequests are retrived from the database.  Most users wil not need these.
            foreach (AEngineAction<ArachnodeDAO> engineAction in _crawler.Engine.EngineActions.Values)
            {
                engineAction.IsEnabled = false;
            }

            //_crawler.ApplicationSettings can be set from code, overriding Database settings found in cfg.Configuration.
            _crawler.ApplicationSettings.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            _crawler.ApplicationSettings.AcceptEncoding = "gzip,deflate,sdch";
            _crawler.ApplicationSettings.AllowAutoRedirect = true;
            _crawler.ApplicationSettings.AssignCrawlRequestPrioritiesForFiles = true;
            _crawler.ApplicationSettings.AssignCrawlRequestPrioritiesForHyperLinks = true;
            _crawler.ApplicationSettings.AssignCrawlRequestPrioritiesForImages = true;
            _crawler.ApplicationSettings.AssignCrawlRequestPrioritiesForWebPages = true;
            _crawler.ApplicationSettings.AssignEmailAddressDiscoveries = false;
            _crawler.ApplicationSettings.AssignFileAndImageDiscoveries = true;
            _crawler.ApplicationSettings.AssignHyperLinkDiscoveries = true;
            _crawler.ApplicationSettings.AutoThrottleHttpWebRequests = false;
            _crawler.ApplicationSettings.ClassifyAbsoluteUris = false;
            //_crawler.ApplicationSettings.ConnectionString = "";
            //_crawler.ApplicationSettings.ConsoleOutputLogsDirectory = "";
            _crawler.ApplicationSettings.CrawlRequestTimeoutInMinutes = 1;
            _crawler.ApplicationSettings.CreateCrawlRequestsFromDatabaseCrawlRequests = true;
            _crawler.ApplicationSettings.CreateCrawlRequestsFromDatabaseFiles = false;
            _crawler.ApplicationSettings.CreateCrawlRequestsFromDatabaseHyperLinks = false;
            _crawler.ApplicationSettings.CreateCrawlRequestsFromDatabaseImages = false;
            _crawler.ApplicationSettings.CreateCrawlRequestsFromDatabaseWebPages = false;
            _crawler.ApplicationSettings.DesiredMaximumMemoryUsageInMegabytes = 4096;
            _crawler.ApplicationSettings.DiscoverySlidingExpirationInSeconds = 120;
            //_crawler.ApplicationSettings.DownloadedFilesDirectory = "";
            //_crawler.ApplicationSettings.DownloadedImagesDirectory = "";
            //_crawler.ApplicationSettings.DownloadedWebPagesDirectory = "";
            _crawler.ApplicationSettings.EnableConsoleOutput = true;
            _crawler.ApplicationSettings.ExtractFileMetaData = false;
            _crawler.ApplicationSettings.ExtractImageMetaData = false;
            _crawler.ApplicationSettings.ExtractWebPageMetaData = false;
            _crawler.ApplicationSettings.HttpWebRequestRetries = 5;
            _crawler.ApplicationSettings.InsertCrawlRequests = true;
            _crawler.ApplicationSettings.InsertDisallowedAbsoluteUriDiscoveries = false;
            _crawler.ApplicationSettings.InsertDisallowedAbsoluteUris = true;
            _crawler.ApplicationSettings.InsertDisallowedDiscoveries = false;
            _crawler.ApplicationSettings.InsertDiscoveries = true;
            _crawler.ApplicationSettings.InsertEmailAddressDiscoveries = false;
            _crawler.ApplicationSettings.InsertEmailAddresses = false;
            _crawler.ApplicationSettings.InsertExceptions = true;
            _crawler.ApplicationSettings.InsertFileDiscoveries = true;
            _crawler.ApplicationSettings.InsertFileMetaData = false;
            _crawler.ApplicationSettings.InsertFiles = true;
            _crawler.ApplicationSettings.InsertFileResponseHeaders = true;
            _crawler.ApplicationSettings.InsertFileSource = false;
            _crawler.ApplicationSettings.InsertHyperLinkDiscoveries = false;
            _crawler.ApplicationSettings.InsertHyperLinks = false;
            _crawler.ApplicationSettings.InsertImageDiscoveries = true;
            _crawler.ApplicationSettings.InsertImageMetaData = false;
            _crawler.ApplicationSettings.InsertImages = true;
            _crawler.ApplicationSettings.InsertImageResponseHeaders = true;
            _crawler.ApplicationSettings.InsertImageSource = false;
            _crawler.ApplicationSettings.InsertWebPageMetaData = false;
            _crawler.ApplicationSettings.InsertWebPages = true;
            _crawler.ApplicationSettings.InsertWebPageResponseHeaders = true;
            _crawler.ApplicationSettings.InsertWebPageSource = false;
            _crawler.ApplicationSettings.MaximumNumberOfAutoRedirects = 50;
            _crawler.ApplicationSettings.MaximumNumberOfCrawlRequestsToCreatePerBatch = 1000;
            _crawler.ApplicationSettings.MaximumNumberOfCrawlThreads = _crawler.ProxyManager.Proxies.Count != 0 ? _crawler.ProxyManager.Proxies.Count : 10;
            _crawler.ApplicationSettings.MaximumNumberOfHostsAndPrioritiesToSelect = 10000;
            _crawler.ApplicationSettings.OutputConsoleToLogs = false;
            _crawler.ApplicationSettings.OutputWebExceptions = true;
            _crawler.ApplicationSettings.ProcessCookies = true;
            _crawler.ApplicationSettings.ProcessDiscoveriesAsynchronously = false;
            _crawler.ApplicationSettings.SaveDiscoveredFilesToDisk = true;
            _crawler.ApplicationSettings.SaveDiscoveredImagesToDisk = true;
            _crawler.ApplicationSettings.SaveDiscoveredWebPagesToDisk = true;
            _crawler.ApplicationSettings.SetRefererToParentAbsoluteUri = true;
            _crawler.ApplicationSettings.UniqueIdentifier = "";
            _crawler.ApplicationSettings.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/534.24 (KHTML, like Gecko) Chrome/11.0.696.71 Safari/534.24"; //If you find yourself blocked from crawling a website, change this to a common crawler string, such as 'Googlebot' or 'Slurp'...
            _crawler.ApplicationSettings.VerboseOutput = false;
            //enable VerboseOutput to see each Discovery and the the status of each Discovery returned from each Discovery.  (e.g. WebPages from each WebPage and Files/Images from each WebPage.)
        }

        private static void AssignApplicationSettingsForRelease()
        {
            LoadProxyServers();

            //CrawlActions, CrawlRules and EngineActions can be added and set from code, augmenting and/or overriding Database settings.

            //cfg.CrawlActions
            foreach (ACrawlAction<ArachnodeDAO> crawlAction in _crawler.CrawlActions.Values)
            {
                if (crawlAction.TypeName != "Arachnode.Plugins.CrawlActions.ManageLuceneDotNetIndexes" && crawlAction.TypeName != "Arachnode.Plugins.CrawlActions.BusinessInformation")
                {
                    crawlAction.IsEnabled = false;
                }
            }

            //cfg.CrawlRules
            //CrawlRules limit where the Crawl can go, where not limited by 'UriClassificationType' in the CrawlRequest constructor.
            foreach (ACrawlRule<ArachnodeDAO> crawlRule in _crawler.CrawlRules.Values)
            {
                if (crawlRule.TypeName != "Arachnode.Plugins.CrawlRules.DataType")
                {
                    crawlRule.IsEnabled = false;
                }
            }

            //cfg.EngineActions
            //EngineActions are invoked when CrawlRequests are retrived from the database.  Most users wil not need these.
            foreach (AEngineAction<ArachnodeDAO> engineAction in _crawler.Engine.EngineActions.Values)
            {
                engineAction.IsEnabled = false;
            }

            //_crawler.ApplicationSettings can be set from code, overriding Database settings found in cfg.Configuration.
            _crawler.ApplicationSettings.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            _crawler.ApplicationSettings.AcceptEncoding = "gzip,deflate,sdch";
            _crawler.ApplicationSettings.AllowAutoRedirect = true;
            _crawler.ApplicationSettings.AssignCrawlRequestPrioritiesForFiles = true;
            _crawler.ApplicationSettings.AssignCrawlRequestPrioritiesForHyperLinks = true;
            _crawler.ApplicationSettings.AssignCrawlRequestPrioritiesForImages = true;
            _crawler.ApplicationSettings.AssignCrawlRequestPrioritiesForWebPages = true;
            _crawler.ApplicationSettings.AssignEmailAddressDiscoveries = false;
            _crawler.ApplicationSettings.AssignFileAndImageDiscoveries = true;
            _crawler.ApplicationSettings.AssignHyperLinkDiscoveries = true;
            _crawler.ApplicationSettings.AutoThrottleHttpWebRequests = false;
            _crawler.ApplicationSettings.ClassifyAbsoluteUris = false;
            //_crawler.ApplicationSettings.ConnectionString = "";
            //_crawler.ApplicationSettings.ConsoleOutputLogsDirectory = "";
            _crawler.ApplicationSettings.CrawlRequestTimeoutInMinutes = 1;
            _crawler.ApplicationSettings.CreateCrawlRequestsFromDatabaseCrawlRequests = true;
            _crawler.ApplicationSettings.CreateCrawlRequestsFromDatabaseFiles = false;
            _crawler.ApplicationSettings.CreateCrawlRequestsFromDatabaseHyperLinks = false;
            _crawler.ApplicationSettings.CreateCrawlRequestsFromDatabaseImages = false;
            _crawler.ApplicationSettings.CreateCrawlRequestsFromDatabaseWebPages = false;
            _crawler.ApplicationSettings.DesiredMaximumMemoryUsageInMegabytes = 4096;
            _crawler.ApplicationSettings.DiscoverySlidingExpirationInSeconds = 120;
            //_crawler.ApplicationSettings.DownloadedFilesDirectory = "";
            //_crawler.ApplicationSettings.DownloadedImagesDirectory = "";
            //_crawler.ApplicationSettings.DownloadedWebPagesDirectory = "";
            _crawler.ApplicationSettings.EnableConsoleOutput = false;
            _crawler.ApplicationSettings.ExtractFileMetaData = false;
            _crawler.ApplicationSettings.ExtractImageMetaData = false;
            _crawler.ApplicationSettings.ExtractWebPageMetaData = false;
            _crawler.ApplicationSettings.HttpWebRequestRetries = 5;
            _crawler.ApplicationSettings.InsertCrawlRequests = true;
            _crawler.ApplicationSettings.InsertDisallowedAbsoluteUriDiscoveries = false;
            _crawler.ApplicationSettings.InsertDisallowedAbsoluteUris = true;
            _crawler.ApplicationSettings.InsertDisallowedDiscoveries = false;
            _crawler.ApplicationSettings.InsertDiscoveries = true;
            _crawler.ApplicationSettings.InsertEmailAddressDiscoveries = false;
            _crawler.ApplicationSettings.InsertEmailAddresses = false;
            _crawler.ApplicationSettings.InsertExceptions = true;
            _crawler.ApplicationSettings.InsertFileDiscoveries = true;
            _crawler.ApplicationSettings.InsertFileMetaData = false;
            _crawler.ApplicationSettings.InsertFileResponseHeaders = true;
            _crawler.ApplicationSettings.InsertFileSource = false;
            _crawler.ApplicationSettings.InsertHyperLinkDiscoveries = false;
            _crawler.ApplicationSettings.InsertHyperLinks = false;
            _crawler.ApplicationSettings.InsertImageDiscoveries = true;
            _crawler.ApplicationSettings.InsertImageMetaData = false;
            _crawler.ApplicationSettings.InsertImages = true;
            _crawler.ApplicationSettings.InsertImageResponseHeaders = true;
            _crawler.ApplicationSettings.InsertImageSource = false;
            _crawler.ApplicationSettings.InsertWebPageMetaData = false;
            _crawler.ApplicationSettings.InsertWebPages = true;
            _crawler.ApplicationSettings.InsertWebPageResponseHeaders = true;
            _crawler.ApplicationSettings.InsertWebPageSource = false;
            _crawler.ApplicationSettings.InsertWebPages = true;
            _crawler.ApplicationSettings.InsertWebPageSource = false;
            _crawler.ApplicationSettings.MaximumNumberOfAutoRedirects = 50;
            _crawler.ApplicationSettings.MaximumNumberOfCrawlRequestsToCreatePerBatch = 1000;
            _crawler.ApplicationSettings.MaximumNumberOfCrawlThreads = _crawler.ProxyManager.Proxies.Count != 0 ? _crawler.ProxyManager.Proxies.Count : 10;
            _crawler.ApplicationSettings.MaximumNumberOfHostsAndPrioritiesToSelect = 10000;
            _crawler.ApplicationSettings.OutputConsoleToLogs = false;
            _crawler.ApplicationSettings.OutputWebExceptions = true;
            _crawler.ApplicationSettings.ProcessCookies = true;
            _crawler.ApplicationSettings.ProcessDiscoveriesAsynchronously = false;
            _crawler.ApplicationSettings.SaveDiscoveredFilesToDisk = true;
            _crawler.ApplicationSettings.SaveDiscoveredImagesToDisk = true;
            _crawler.ApplicationSettings.SaveDiscoveredWebPagesToDisk = true;
            _crawler.ApplicationSettings.SetRefererToParentAbsoluteUri = true;
            _crawler.ApplicationSettings.UniqueIdentifier = "";
            _crawler.ApplicationSettings.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/534.24 (KHTML, like Gecko) Chrome/11.0.696.71 Safari/534.24"; //If you find yourself blocked from crawling a website, change this to a common crawler string, such as 'Googlebot' or 'Slurp'...
            _crawler.ApplicationSettings.VerboseOutput = false;
            //enable VerboseOutput to see each Discovery and the the status of each Discovery returned from each Discovery.  (e.g. WebPages from each WebPage and Files/Images from each WebPage.)
        }

        private static void Engine_CrawlRequestCanceled(CrawlRequest<ArachnodeDAO> crawlRequest)
        {
            //if (crawlRequest != null)
            //{
            //    System.Console.ForegroundColor = ConsoleColor.Red;

            //    System.Console.WriteLine("Canceled: " + crawlRequest.Discovery.Uri.AbsoluteUri + " : " + crawlRequest.HttpWebResponseTime);

            //    System.Console.ForegroundColor = ConsoleColor.White;
            //}
        }

        private static void Engine_CrawlRequestCompleted(CrawlRequest<ArachnodeDAO> crawlRequest)
        {
            //if (crawlRequest != null)
            //{
            //    System.Console.ForegroundColor = ConsoleColor.Green;

            //    System.Console.WriteLine("Completed: " + crawlRequest.Discovery.Uri.AbsoluteUri + " : " + crawlRequest.HttpWebResponseTime);

            //    System.Console.ForegroundColor = ConsoleColor.White;
            //}

            if (crawlRequest.WebClient != null && crawlRequest.WebClient.HttpWebResponse != null)
            {
                if (crawlRequest.WebClient.HttpWebResponse.StatusCode == HttpStatusCode.Forbidden)
                {
                    _crawler.ProxyManager.RemoveProxy(crawlRequest.WebClient.HttpWebRequest.Proxy);

                    if (_crawler.ProxyManager.GetNextProxy() == null)
                    {
                        _crawler.ConsoleManager.OutputString("No valid ProxyServers.  Stopping Engine...", ConsoleColor.Red, ConsoleColor.Yellow);

                        _crawler.Engine.Stop();
                    }
                }
            }
        }

        private static void Engine_CrawlRequestRetrying(CrawlRequest<ArachnodeDAO> crawlRequest)
        {
            //if (crawlRequest != null)
            //{
            //    System.Console.ForegroundColor = ConsoleColor.Green;

            //    System.Console.WriteLine("Retrying: " + crawlRequest.Discovery.Uri.AbsoluteUri);

            //    System.Console.ForegroundColor = ConsoleColor.White;
            //}
        }

        private static void Engine_CrawlRequestThrottled(CrawlRequest<ArachnodeDAO> crawlRequest)
        {
            //if (crawlRequest != null)
            //{
            //    System.Console.ForegroundColor = ConsoleColor.Yellow;

            //    System.Console.WriteLine("Throttled: " + crawlRequest.Discovery.Uri.AbsoluteUri);

            //    System.Console.ForegroundColor = ConsoleColor.White;
            //}
        }

        /// <summary>
        /// Handles the OnCrawlCompleted event of the Engine control.
        /// </summary>
        /// <param name="engine">The Engine.</param>
        private static void Engine_CrawlCompleted(Engine<ArachnodeDAO> engine)
        {
            _hasCrawlCompleted = true;
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;

            _crawler.Engine.Stop();
        }
    }
}