using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.DataSource;
using Arachnode.SiteCrawler.Core;
using Arachnode.SiteCrawler.Managers;

namespace Arachnode.Browser
{
    public partial class Main : Form
    {
        private ApplicationSettings _applicationSettings = new ApplicationSettings();
        private WebSettings _webSettings = new WebSettings();

        private IArachnodeDAO _arachnodeDAO;

        private ArachnodeDataSet.WebPagesRow _webPagesRow = null;
        private string _webPageDiscoveryPath = null;

        private ArachnodeDataSet.FilesRow _filesRow = null;
        private string _fileDiscoveryPath = null;

        private ArachnodeDataSet.ImagesRow _imagesRow = null;
        private string _imageDiscoveryPath = null;

        private Cache<ArachnodeDAO> _cache;
        private CacheManager<ArachnodeDAO> _cacheManager;
        private ActionManager<ArachnodeDAO> _actionManager;
        private ConsoleManager<ArachnodeDAO> _consoleManager;
        private CrawlerPeerManager<ArachnodeDAO> _crawlerPeerManager;
        private DiscoveryManager<ArachnodeDAO> _discoveryManager;
        private MemoryManager<ArachnodeDAO> _memoryManager;
        private RuleManager<ArachnodeDAO> _ruleManager;

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            try
            {
                _arachnodeDAO = new ArachnodeDAO(_applicationSettings.ConnectionString, _applicationSettings, _webSettings, true, true);

                _actionManager = new ActionManager<ArachnodeDAO>(_applicationSettings, _webSettings, _consoleManager);
                _consoleManager = new ConsoleManager<ArachnodeDAO>(_applicationSettings, _webSettings);
                _memoryManager = new MemoryManager<ArachnodeDAO>(_applicationSettings, _webSettings);
                _cacheManager = new CacheManager<ArachnodeDAO>(_applicationSettings, _webSettings);
                _crawlerPeerManager = new CrawlerPeerManager<ArachnodeDAO>(_applicationSettings, _webSettings, null, _arachnodeDAO);
                _cache = new Cache<ArachnodeDAO>(_applicationSettings, _webSettings, null, _actionManager, _cacheManager, _crawlerPeerManager, _memoryManager, _ruleManager);

                _ruleManager = new RuleManager<ArachnodeDAO>(_applicationSettings, _webSettings, _consoleManager);
                _discoveryManager = new DiscoveryManager<ArachnodeDAO>(_applicationSettings, _webSettings, _cache, _actionManager, _cacheManager, _memoryManager, _ruleManager);

                nudWebPageID_ValueChanged(null, null);
                nudFileID_ValueChanged(null, null);
                nudImageID_ValueChanged(null, null);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + " ::" + exception.StackTrace, "Browser");
            }
        }

        private void btnViewWebPage_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(_webPageDiscoveryPath))
                {
                    Regex scriptRegex = new Regex("<script.*?>.*?</script>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

                    if (cbRemoveScriptsFromWebPages.Checked)
                    {
                        File.WriteAllText(Path.Combine(Path.GetTempPath(), "WebPage" + _webPagesRow.FullTextIndexType), scriptRegex.Replace(File.ReadAllText(_webPageDiscoveryPath), string.Empty));
                    }
                    else
                    {
                        File.WriteAllText(Path.Combine(Path.GetTempPath(), "WebPage" + _webPagesRow.FullTextIndexType), File.ReadAllText(_webPageDiscoveryPath));
                    }

                    wbMain.Navigate(Path.Combine(Path.GetTempPath(), "WebPage.htm"));
                }
                else
                {
                    if (_webPagesRow.Source.Length != 0)
                    {
                        Regex scriptRegex = new Regex("<script.*?>.*?</script>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

                        File.WriteAllText(Path.Combine(Path.GetTempPath(), "WebPage" + _webPagesRow.FullTextIndexType), scriptRegex.Replace(Encoding.GetEncoding(_webPagesRow.CodePage).GetString(_webPagesRow.Source), string.Empty));

                        wbMain.Navigate(Path.Combine(Path.GetTempPath(), "WebPage.htm"));
                    }
                    else
                    {
                        wbMain.DocumentText = "The Data (Source) could not be found in the 'WebPages' database table or at " + _applicationSettings.DownloadedWebPagesDirectory +  " (ApplicationSettings.DownloadedWebPagesDirectory).";
                    }
                }
            }
            catch(Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void nudWebPageID_ValueChanged(object sender, EventArgs e)
        {
            _webPagesRow = _arachnodeDAO.GetWebPage(nudWebPageID.Value.ToString());

            if (_webPagesRow != null)
            {
                _webPageDiscoveryPath = _discoveryManager.GetDiscoveryPath(_applicationSettings.DownloadedWebPagesDirectory, _webPagesRow.AbsoluteUri, _webPagesRow.FullTextIndexType);

                llWebPageDiscoveryPathDirectory.Visible = true;
                llWebPageDiscoveryPathDirectory.Text = Path.GetDirectoryName(_webPageDiscoveryPath);

                if (cbAutoView.Checked)
                {
                    btnViewWebPage_Click(sender, e);
                }
            }
            else
            {
                llWebPageDiscoveryPathDirectory.Visible = false;
                wbMain.DocumentText = "The WebPage with the ID of " + nudWebPageID.Value + " does not exist.";
            }
        }

        private void llWebPageDiscoveryPathDirectory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(llWebPageDiscoveryPathDirectory.Text);
        }

        private void btnViewFile_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(_fileDiscoveryPath))
                {
                    wbMain.Navigate(_fileDiscoveryPath);
                }
                else
                {
                    if (_filesRow.Source.Length != 0)
                    {
                        File.WriteAllBytes(Path.Combine(Path.GetTempPath(), "File" + _filesRow.FullTextIndexType), _filesRow.Source);

                        wbMain.Navigate(Path.Combine(Path.GetTempPath(), "File" + _filesRow.FullTextIndexType));
                    }
                    else
                    {
                        wbMain.DocumentText = "The Data (Source) could not be found in the 'Files' database table or at " + _applicationSettings.DownloadedFilesDirectory + " (ApplicationSettings.DownloadedFilesDirectory).";
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void nudFileID_ValueChanged(object sender, EventArgs e)
        {
            _filesRow = _arachnodeDAO.GetFile(nudFileID.Value.ToString());

            if (_filesRow != null)
            {
                _fileDiscoveryPath = _discoveryManager.GetDiscoveryPath(_applicationSettings.DownloadedFilesDirectory, _filesRow.AbsoluteUri, _filesRow.FullTextIndexType);

                llFileDiscoveryPathDirectory.Visible = true;
                llFileDiscoveryPathDirectory.Text = Path.GetDirectoryName(_fileDiscoveryPath);

                if (cbAutoView.Checked)
                {
                    btnViewFile_Click(sender, e);
                }
            }
            else
            {
                llFileDiscoveryPathDirectory.Visible = false;
                wbMain.DocumentText = "The File with the ID of " + nudFileID.Value + " does not exist.";
            }
        }

        private void llFileDiscoveryPathDirectory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(llFileDiscoveryPathDirectory.Text);
        }

        private void btnViewImage_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(_imageDiscoveryPath))
                {
                    wbMain.Navigate(_imageDiscoveryPath);
                }
                else
                {
                    if (_imagesRow.Source.Length != 0)
                    {
                        File.WriteAllBytes(Path.Combine(Path.GetTempPath(), "Image" + _imagesRow.FullTextIndexType), _imagesRow.Source);

                        wbMain.Navigate(Path.Combine(Path.GetTempPath(), "Image" + _imagesRow.FullTextIndexType));
                    }
                    else
                    {
                        wbMain.DocumentText = "The Data (Source) could not be found in the 'Images' database table or at " + _applicationSettings.DownloadedImagesDirectory + " (ApplicationSettings.DownloadedImageDirectory).";
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void nudImageID_ValueChanged(object sender, EventArgs e)
        {
            _imagesRow = _arachnodeDAO.GetImage(nudImageID.Value.ToString());

            if (_imagesRow != null)
            {
                _imageDiscoveryPath = _discoveryManager.GetDiscoveryPath(_applicationSettings.DownloadedImagesDirectory, _imagesRow.AbsoluteUri, _imagesRow.FullTextIndexType);

                llImageDiscoveryPathDirectory.Visible = true;
                llImageDiscoveryPathDirectory.Text = Path.GetDirectoryName(_imageDiscoveryPath);

                if (cbAutoView.Checked)
                {
                    btnViewImage_Click(sender, e);
                }
            }
            else
            {
                llImageDiscoveryPathDirectory.Visible = false;
                wbMain.DocumentText = "The Image with the ID of " + nudImageID.Value + " does not exist.";
            }
        }

        private void llImageDiscoveryPathDirectory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(llImageDiscoveryPathDirectory.Text);
        }
    }
}
