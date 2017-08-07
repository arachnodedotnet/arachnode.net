using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.SiteCrawler.Core;
using Arachnode.SiteCrawler.Managers;
using Arachnode.Utilities;
using System.Drawing;
using System.Xml;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Threading;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using System.Linq;

namespace Arachnode.Scraper
{
    public partial class Main : Form
    {
        private static readonly Regex _hyperLinkRegex = new Regex("<\\s*(?<Tag>(a|base|form|frame))\\s*.*?(?<AttributeName>(action|href|src))\\s*=\\s*([\\\"\\'])(?<HyperLink>.*?)\\3", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private bool _assignScraperIDs;
        private HashSet<HtmlElement> _clickHandlers = new HashSet<HtmlElement>();
        HtmlAgilityPack.HtmlDocument _htmlDocument;

        private ApplicationSettings _applicationSettings = new ApplicationSettings();
        private WebSettings _webSettings = new WebSettings();

        private Cache<ArachnodeDAO> _cache;
        private CacheManager<ArachnodeDAO> _cacheManager;
        private ActionManager<ArachnodeDAO> _actionManager;
        private ConsoleManager<ArachnodeDAO> _consoleManager;
        private CrawlerPeerManager<ArachnodeDAO> _crawlerPeerManager;
        private DiscoveryManager<ArachnodeDAO> _discoveryManager;
        private MemoryManager<ArachnodeDAO> _memoryManager;
        private RuleManager<ArachnodeDAO> _ruleManager;

        private List<TreeNode> _treeNodes = new List<TreeNode>();

        private string _formText;

        public Main()
        {
            InitializeComponent();

            _formText = Text;

            _actionManager = new ActionManager<ArachnodeDAO>(_applicationSettings, _webSettings, _consoleManager);
            _consoleManager = new ConsoleManager<ArachnodeDAO>(_applicationSettings, _webSettings);
            _memoryManager = new MemoryManager<ArachnodeDAO>(_applicationSettings, _webSettings);
            _cacheManager = new CacheManager<ArachnodeDAO>(_applicationSettings, _webSettings);
            _crawlerPeerManager = new CrawlerPeerManager<ArachnodeDAO>(_applicationSettings, _webSettings, null, null);
            _cache = new Cache<ArachnodeDAO>(_applicationSettings, _webSettings, null, _actionManager, _cacheManager, _crawlerPeerManager, _memoryManager, _ruleManager);

            _ruleManager = new RuleManager<ArachnodeDAO>(_applicationSettings, _webSettings, _consoleManager);
            _discoveryManager = new DiscoveryManager<ArachnodeDAO>(_applicationSettings, _webSettings, _cache, _actionManager, _cacheManager, _memoryManager, _ruleManager);

            tbAbsoluteUri_KeyUp(this, new KeyEventArgs(Keys.Enter));
        }

        private void tbAbsoluteUri_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {                
                if(!tbAbsoluteUri.Text.ToLowerInvariant().StartsWith("http://") && !tbAbsoluteUri.Text.ToLowerInvariant().StartsWith("https://"))
                {
                    tbAbsoluteUri.Text = "http://" + tbAbsoluteUri.Text;
                }

                _assignScraperIDs = true;

                wbBrowser.Navigate(tbAbsoluteUri.Text);
            }
        }

        private void tcMain_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.XButton1)
            {
                if (tcMain.SelectedTab.Name == "tpBrowser")
                {
                }
            }
        }

        private void wbBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            _clickHandlers.Clear();
        }

        private void wbBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
        }

        private void wbBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                tbAbsoluteUri.Text = e.Url.AbsoluteUri;

                if (_assignScraperIDs)
                {
                    _assignScraperIDs = false;

                    short scraperIdForDivs = 1;
                    int numberOfDivsInHtmlDocument1 = 0;

                    foreach (HtmlElement htmlElement in wbBrowser.Document.All)
                    {
                        switch (htmlElement.TagName.ToLowerInvariant())
                        {
                            case "a":
                            case "h1":
                            case "h2":
                            case "h3":
                            case "h4":
                            case "h5":
                            case "h6":
                            case "table":
                            case "th":
                            case "tr":
                            case "td":
                            case "div":
                            case "span":
                            //default:
                                numberOfDivsInHtmlDocument1++;

                                if (!_clickHandlers.Contains(htmlElement))
                                {
                                    htmlElement.Click += new HtmlElementEventHandler(htmlElement_Click);
                                    htmlElement.TabIndex = scraperIdForDivs++;

                                    _clickHandlers.Add(htmlElement);
                                }
                                else
                                {
                                    htmlElement.TabIndex = 0;
                                }
                                break;
                        }
                    }

                    scraperIdForDivs = 1;
                    int numberOfDivsInHtmlDocument2 = 0;

                    _htmlDocument = new HtmlAgilityPack.HtmlDocument();
                    _htmlDocument.LoadHtml(wbBrowser.DocumentText);

                    foreach (HtmlNode htmlNode in _htmlDocument.DocumentNode.DescendantsAndSelf())
                    {
                        switch (htmlNode.Name.ToLowerInvariant())
                        {
                            case "a":
                            case "h1":
                            case "h2":
                            case "h3":
                            case "h4":
                            case "h5":
                            case "h6":
                            case "table":
                            case "th":
                            case "tr":
                            case "td":
                            case "div":
                            case "span":
                            //default:
                                numberOfDivsInHtmlDocument2++;
                                htmlNode.Attributes.Add("arachnode_scraper_id", (scraperIdForDivs++).ToString());

                                break;
                        }
                    }

                    if (numberOfDivsInHtmlDocument1 != numberOfDivsInHtmlDocument2)
                    {
                    }
                }

                /**/

                tvBrowser.Nodes.Clear();
                _treeNodes.Clear();

                //inefficient...
                foreach (HtmlElement htmlElement in wbBrowser.Document.All)
                {
                    TreeNode treeNode = new TreeNode(htmlElement.TagName);
                    treeNode.ToolTipText = htmlElement.InnerHtml;
                    if (string.IsNullOrEmpty(treeNode.ToolTipText))
                    {
                        treeNode.ToolTipText = htmlElement.InnerText;
                    }
                    if (string.IsNullOrEmpty(treeNode.ToolTipText))
                    {
                        treeNode.ToolTipText = htmlElement.OuterHtml;
                    }
                    if (string.IsNullOrEmpty(treeNode.ToolTipText))
                    {
                        treeNode.ToolTipText = htmlElement.OuterText;
                    }
                    if (!string.IsNullOrEmpty(treeNode.ToolTipText))
                    {
                        treeNode.ToolTipText = treeNode.ToolTipText.Trim();
                    }
                    string toolTipText = treeNode.ToolTipText;
                    if(!string.IsNullOrEmpty(treeNode.ToolTipText) && treeNode.ToolTipText.Length > 250)
                    {
                        treeNode.ToolTipText = treeNode.ToolTipText.Substring(0, 250) + "...";
                    }
                    if (!string.IsNullOrEmpty(treeNode.ToolTipText))
                    {
                        treeNode.ToolTipText += Environment.NewLine + "------------" + Environment.NewLine + UserDefinedFunctions.ExtractText(toolTipText).Value;
                    }
                    if (!string.IsNullOrEmpty(treeNode.ToolTipText) && treeNode.ToolTipText.Length > 500)
                    {
                        treeNode.ToolTipText = treeNode.ToolTipText.Substring(0, 500) + "...";
                    }
                    treeNode.Tag = htmlElement;

                    if (htmlElement.Parent == null)
                    {
                        tvBrowser.Nodes.Add(treeNode);
                        _treeNodes.Add(treeNode);
                    }
                    else
                    {

                        foreach (TreeNode treeNode2 in _treeNodes)
                        {
                            if (((HtmlElement) treeNode.Tag).Parent == (HtmlElement) treeNode2.Tag)
                            {
                                treeNode2.Nodes.Add(treeNode);
                                _treeNodes.Add(treeNode);

                                break;
                            }
                        }
                    }
                }

                /**/

                rtbViewSource.Text = wbBrowser.DocumentText;
                HighlightRTF(rtbViewSource);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + Environment.NewLine + exception.StackTrace, _formText);
            }
        }

        void htmlElement_Click(object sender, HtmlElementEventArgs e)
        {
            if (wbBrowser.Document.ActiveElement != null)
            {
                foreach (HtmlNode htmlNode in _htmlDocument.DocumentNode.DescendantsAndSelf())
                {
                    if (htmlNode.GetAttributeValue("arachnode_scraper_id", string.Empty) == wbBrowser.Document.ActiveElement.TabIndex.ToString())
                    {
                        TreeNode treeNode = _treeNodes.Where(tn => (HtmlElement) tn.Tag == wbBrowser.Document.ActiveElement).First();
                        tvBrowser.SelectedNode = treeNode;
                        tvBrowser.HideSelection = false;

                        tbXPath.Text = htmlNode.XPath;
                        tbResult.Text = null;
                        if (!string.IsNullOrEmpty(wbBrowser.Document.ActiveElement.InnerHtml))
                        {
                            tbResult.Text = UserDefinedFunctions.ExtractText(wbBrowser.Document.ActiveElement.InnerHtml).Value;
                        }
                    }
                }
            }
        }

        private void tvBrowser_AfterSelect(object sender, TreeViewEventArgs e)
        {
            HtmlElement htmlElement = (HtmlElement) e.Node.Tag;

            htmlElement.Focus();

            htmlElement_Click(sender, null);
        }

        private void btnGetPaths_Click(object sender, EventArgs e)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(tbGetPathsFrom.Text);
                httpWebRequest.ReadWriteTimeout = 30000;
                httpWebRequest.Timeout = 30000;
                httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/534.24 (KHTML, like Gecko) Chrome/11.0.696.71 Safari/534.24";

                HttpWebResponse httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse();

                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                    {
                        string toEnd = streamReader.ReadToEnd();

                        List<string> paths = new List<string>();

                        MatchCollection matchCollection = _hyperLinkRegex.Matches(toEnd);

                        if (matchCollection.Count != 0)
                        {
                            Uri baseUri = new Uri(tbGetPathsFrom.Text.TrimEnd('/').TrimEnd('#'), UriKind.Absolute);

                            foreach (Match match in matchCollection)
                            {
                                if (match.Groups["Tag"].Value.ToLowerInvariant() == "base")
                                {
                                    if (!Uri.TryCreate(match.Groups["HyperLink"].Value, UriKind.Absolute, out baseUri))
                                    {
                                        baseUri = new Uri(tbGetPathsFrom.Text.TrimEnd('/').TrimEnd('#'), UriKind.Absolute);

                                        break;
                                    }
                                }
                            }

                            UriBuilder uriBuilder = new UriBuilder(baseUri);
                            if (!baseUri.AbsoluteUri.EndsWith("/") && !baseUri.Segments[baseUri.Segments.Length - 1].Contains("."))
                            {
                                baseUri = new Uri(baseUri.AbsoluteUri + "/");
                            }

                            foreach (Match match in matchCollection)
                            {
                                Uri hyperLinkDiscovery;
                                string groupValue = _discoveryManager.GetGroupValue(match, "HyperLink").TrimEnd('/').TrimEnd('#');
                                if (Uri.TryCreate(groupValue, UriKind.RelativeOrAbsolute, out hyperLinkDiscovery))
                                {
                                    if (!hyperLinkDiscovery.IsAbsoluteUri)
                                    {
                                        if (groupValue.StartsWith("?"))
                                        {
                                            uriBuilder.Query = groupValue.TrimStart('?');

                                            hyperLinkDiscovery = uriBuilder.Uri;
                                        }
                                        else
                                        {
                                            if (!string.IsNullOrEmpty(groupValue))
                                            {
                                                Uri.TryCreate(baseUri, hyperLinkDiscovery, out hyperLinkDiscovery);
                                            }
                                            else
                                            {
                                                hyperLinkDiscovery = new Uri(baseUri.AbsoluteUri.TrimEnd('/').TrimEnd('#'));
                                            }
                                        }
                                    }
                                }

                                if (!string.IsNullOrEmpty(hyperLinkDiscovery.AbsolutePath))
                                {
                                    string directoryName = Path.GetDirectoryName(hyperLinkDiscovery.AbsolutePath);

                                    if (!string.IsNullOrEmpty(directoryName))
                                    {
                                        directoryName = directoryName.Replace("\\", "/");

                                        if (!string.IsNullOrEmpty(directoryName) && directoryName != "/")
                                        {
                                            if (!paths.Contains(directoryName))
                                            {
                                                paths.Add(directoryName);
                                            }
                                        }
                                    }
                                }
                            }

                            dgvPathFilter.Rows.Clear();

                            foreach(string path in paths.OrderBy(p => p))
                            {
                                dgvPathFilter.Rows.Add(path, "Crawl", "Scrape");
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + Environment.NewLine + exception.StackTrace, _formText);
            }
        }

        public static void HighlightRTF(RichTextBox rtb)
        {
            int k = 0;

            string str = rtb.Text;

            int st, en;
            int lasten = -1;
            while (k < str.Length)
            {
                st = str.IndexOf('<', k);

                if (st < 0)
                    break;

                if (lasten > 0)
                {
                    rtb.Select(lasten + 1, st - lasten - 1);
                    rtb.SelectionColor = HighlightColors.HC_INNERTEXT;
                }

                en = str.IndexOf('>', st + 1);
                if (en < 0)
                    break;

                k = en + 1;
                lasten = en;

                if (str[st + 1] == '!')
                {
                    rtb.Select(st + 1, en - st - 1);
                    rtb.SelectionColor = HighlightColors.HC_COMMENT;
                    continue;

                }
                string nodeText = str.Substring(st + 1, en - st - 1);


                bool inString = false;

                int lastSt = -1;
                int state = 0;
                /* 0 = before node name
                 * 1 = in node name
                   2 = after node name
                   3 = in attribute
                   4 = in string
                   */
                int startNodeName = 0, startAtt = 0;
                for (int i = 0; i < nodeText.Length; ++i)
                {
                    if (nodeText[i] == '"')
                        inString = !inString;

                    if (inString && nodeText[i] == '"')
                        lastSt = i;
                    else
                        if (nodeText[i] == '"')
                        {
                            rtb.Select(lastSt + st + 2, i - lastSt - 1);
                            rtb.SelectionColor = HighlightColors.HC_STRING;
                        }

                    switch (state)
                    {
                        case 0:
                            if (!char.IsWhiteSpace(nodeText, i))
                            {
                                startNodeName = i;
                                state = 1;
                            }
                            break;
                        case 1:
                            if (char.IsWhiteSpace(nodeText, i))
                            {
                                rtb.Select(startNodeName + st, i - startNodeName + 1);
                                rtb.SelectionColor = HighlightColors.HC_NODE;
                                state = 2;
                            }
                            break;
                        case 2:
                            if (!char.IsWhiteSpace(nodeText, i))
                            {
                                startAtt = i;
                                state = 3;
                            }
                            break;

                        case 3:
                            if (char.IsWhiteSpace(nodeText, i) || nodeText[i] == '=')
                            {
                                rtb.Select(startAtt + st, i - startAtt + 1);
                                rtb.SelectionColor = HighlightColors.HC_ATTRIBUTE;
                                state = 4;
                            }
                            break;
                        case 4:
                            if (nodeText[i] == '"' && !inString)
                                state = 2;
                            break;


                    }

                }
                if (state == 1)
                {
                    rtb.Select(st + 1, nodeText.Length);
                    rtb.SelectionColor = HighlightColors.HC_NODE;
                }
            }
        }

        private void btnEvaluateXPath_Click(object sender, EventArgs e)
        {
            try
            {
                Evaluate evaluate = new Evaluate();
                evaluate.dataGridView1.Rows.Clear();

                //foreach (HtmlNode htmlNode in _htmlDocument.DocumentNode.SelectSingleNode(tbXPath.Text))
                //{
                //    evaluate.dataGridView1.Rows.Add(UserDefinedFunctions.ExtractText(htmlNode.InnerHtml).Value);
                //}
                string innerHtml = _htmlDocument.DocumentNode.SelectSingleNode(tbXPath.Text).InnerHtml;

                evaluate.dataGridView1.Rows.Add(UserDefinedFunctions.ExtractText(innerHtml).Value);

                evaluate.ShowDialog(this);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + Environment.NewLine + exception.StackTrace, _formText);
            }
        }
    }

    public class HighlightColors
    {
        public static Color HC_NODE = Color.Firebrick;
        public static Color HC_STRING = Color.Blue;
        public static Color HC_ATTRIBUTE = Color.Red;
        public static Color HC_COMMENT = Color.GreenYellow;
        public static Color HC_INNERTEXT = Color.Black;
    }
}
