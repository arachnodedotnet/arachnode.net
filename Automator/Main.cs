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
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.Proxy;
using Arachnode.Security;
using Arachnode.SiteCrawler.Managers;
using Arachnode.Utilities;

#endregion

namespace Arachnode.Automator
{
    public partial class Main : Form, IMessageFilter
    {
        private List<string> _firstNames_Female = new List<string>();
        private List<string> _firstNames_Male = new List<string>();
        private string _initialAbsoluteUriOne = null;
        private string _initialAbsoluteUriTwo = null;
        private List<string> _lastNames = new List<string>();

        private string _styleMark = "BORDER-BOTTOM: green 2px dashed; BORDER-LEFT: green 2px dashed; BORDER-TOP: green 2px dashed; BORDER-RIGHT: green 2px dashed";
        private List<string> _wbTabOneHtmlElements = new List<string>();
        private List<string> _wbTabTwoHtmlElements = new List<string>();

        private ProxyManager<ArachnodeDAO> _proxyManager;

        private bool _isLoadingGrid;

        public Main()
        {            
            InitializeComponent();

            Application.AddMessageFilter(this);

            _initialAbsoluteUriOne = tbAbsoluteUriTabOne.Text;
            _initialAbsoluteUriTwo = tbAbsoluteUriTabTwo.Text;

            ApplicationSettings applicationSettings = new ApplicationSettings();
            WebSettings webSettings = new WebSettings();

            _proxyManager = new ProxyManager<ArachnodeDAO>(applicationSettings, webSettings, new ConsoleManager<ArachnodeDAO>(applicationSettings, webSettings));

            _proxyManager.OnProxyServerFailed += new Action<IWebProxy, TimeSpan, Exception>(ProxyManager_OnProxyServerFailed);
            _proxyManager.OnProxyServerPassed += new Action<IWebProxy, TimeSpan>(ProxyManager_OnProxyServerPassed);

            foreach(string line in File.ReadAllLines("ProxyServers.txt"))
            {
                tbProxyServers.Text += line + Environment.NewLine;
            }

            try
            {
                _lastNames.AddRange(File.ReadAllLines("TextDatabases\\LastNames.txt"));
                _firstNames_Male.AddRange(File.ReadAllLines("TextDatabases\\FirstNames_Male.txt"));
                _firstNames_Female.AddRange(File.ReadAllLines("TextDatabases\\FirstNames_Female.txt"));
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }   
        }

        void ProxyManager_OnProxyServerPassed(IWebProxy webProxy, TimeSpan timeSpan)
        {
            if(InvokeRequired)
            {
                BeginInvoke(new Action(delegate
                                {
                                    WebProxy webProxy2 = webProxy as WebProxy;

                                    if (webProxy2.Address != null)
                                    {
                                        tbProxyServersOutput.Text += "Passed: " + ((WebProxy)webProxy).Address.AbsoluteUri + " :: " + timeSpan + Environment.NewLine;
                                        cbProxyServers.Items.Add(((WebProxy)webProxy).Address.AbsoluteUri);
                                    }
                                    else
                                    {
                                        tbProxyServersOutput.Text += "Passed: http://127.0.0.1:80/ :: " + timeSpan + Environment.NewLine;
                                        cbProxyServers.Items.Add("http://127.0.0.1:80/");
                                    }
                                }));
            }
        }

        void ProxyManager_OnProxyServerFailed(IWebProxy webProxy, TimeSpan timeSpan, Exception exception)
        {
            if(InvokeRequired)
            {
                BeginInvoke(new Action(delegate
                                {
                                    WebProxy webProxy2 = webProxy as WebProxy;

                                    if (webProxy2.Address != null)
                                    {
                                        tbProxyServersOutput.Text += "Failed: " + ((WebProxy)webProxy).Address.AbsoluteUri + " :: " + timeSpan + Environment.NewLine;
                                    }
                                    else
                                    {
                                        tbProxyServersOutput.Text += "Failed:: http://127.0.0.1:80/ :: " + timeSpan + Environment.NewLine;
                                    }
                                }));
            }
        }

        #region IMessageFilter Members

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == 0x20A)
            {
                if (tcMain.SelectedIndex == 0)
                {
                    //SendMessage(tabPage1.Handle, m.Msg, m.WParam, m.LParam);
                    //SendMessage(wbTabOne.Handle, m.Msg, m.WParam, m.LParam);
                }
                else
                {
                    SendMessage(wbTabTwo.Handle, m.Msg, m.WParam, m.LParam);
                }

                return true;
            }

            return false;
        }

        #endregion

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);

        #region Tab Controls Events

        private void wbTabOne_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            LoadGrid(dgvOne);

            if (wbTabOne.Document != null)
            {
                tbCookieValueTabOne.Text = wbTabOne.Document.Cookie;
            }
        }

        private void wbTabTwo_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            LoadGrid(dgvTwo);

            if (wbTabTwo.Document != null)
            {
                tbCookieValueTabTwo.Text = wbTabTwo.Document.Cookie;
            }
        }

        private void tbAbsoluteUriTabOne_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (cbProxyServers.SelectedItem != null)
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        wbTabOne.Navigate(tbAbsoluteUriTabOne.Text);
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void tbAbsoluteUriTabTwo_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (cbProxyServers.SelectedItem != null)
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        wbTabTwo.Navigate(tbAbsoluteUriTabTwo.Text);
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void btnResetWbOne_Click(object sender, EventArgs e)
        {
            if (cbProxyServers.SelectedItem != null)
            {
                Internet.ClearIECache();

                if (wbTabOne.Document != null)
                {
                    wbTabOne.Document.Cookie = null;
                    wbTabTwo.Navigate("javascript:void((function(){var a,b,c,e,f;f=0;a=document.cookie.split('; ');for(e=0;e<a.length&&a[e];e++){f++;for(b='.'+location.host;b;b=b.replace(/^(?:%5C.|[^%5C.]+)/,'')){for(c=location.pathname;c;c=c.replace(/.$/,'')){document.cookie=(a[e]+'; domain='+b+'; path='+c+'; expires='+new Date((new Date()).getTime()-1e11).toGMTString());}}}})())");
                }

                tbAbsoluteUriTabOne.Text = _initialAbsoluteUriOne;

                tbAbsoluteUriTabOne_KeyUp(this, new KeyEventArgs(Keys.Enter));

                tbCookieValueTabTwo.Text = null;
            }
        }

        private void btnResetWbTwo_Click(object sender, EventArgs e)
        {
            if (cbProxyServers.SelectedItem != null)
            {
                Internet.ClearIECache();

                if (wbTabTwo.Document != null)
                {
                    wbTabTwo.Document.Cookie = null;
                    wbTabTwo.Navigate("javascript:void((function(){var a,b,c,e,f;f=0;a=document.cookie.split('; ');for(e=0;e<a.length&&a[e];e++){f++;for(b='.'+location.host;b;b=b.replace(/^(?:%5C.|[^%5C.]+)/,'')){for(c=location.pathname;c;c=c.replace(/.$/,'')){document.cookie=(a[e]+'; domain='+b+'; path='+c+'; expires='+new Date((new Date()).getTime()-1e11).toGMTString());}}}})())");
                }

                tbAbsoluteUriTabTwo.Text = _initialAbsoluteUriTwo;

                tbAbsoluteUriTabTwo_KeyUp(this, new KeyEventArgs(Keys.Enter));

                tbCookieValueTabTwo.Text = null;
            }
        }

        private void btnValidateProxyServers_Click(object sender, EventArgs e)
        {
            btnValidateProxyServers.Enabled = false;

            cbProxyServers.Items.Clear();
            tbProxyServersOutput.Text = null;

            cbProxyServers.Items.Add("http://127.0.0.1");

            List<Uri> absoluteUris = Strings.ExtractAbsoluteUris(tbProxyServers.Text);

            Thread thread = new Thread(delegate()
                                           {
                                               List<IWebProxy> proxies = _proxyManager.LoadProxyServers(absoluteUris, 2000, int.MaxValue, "http://www.infobyip.com/detectproxy.php", "Proxy", true);

                                               BeginInvoke(new Action(delegate
                                                                          {
                                                                              tbProxyServersOutput.Text = null;

                                                                              foreach (IWebProxy webProxy in proxies)
                                                                              {
                                                                                  tbProxyServersOutput.Text += ((WebProxy)webProxy).Address.AbsoluteUri + Environment.NewLine;
                                                                              }

                                                                              btnValidateProxyServers.Enabled = false;
                                                                          }));
                                           });

            thread.Start();
        }

        private void btnClearIE_Click(object sender, EventArgs e)
        {
            Internet.ClearIECache();
        }

        private void cbProxyServers_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string proxyServer = cbProxyServers.SelectedItem.ToString().TrimEnd('/');

                if (!proxyServer.Contains("127.0.0.1"))
                {
                    ConnectionProxy.SetConnectionProxy(proxyServer);
                }
                else
                {
                    ConnectionProxy.RestoreSystemProxy();
                }

                if (wbWhatIsMyIP.Document != null)
                {
                    wbWhatIsMyIP.Document.Cookie = null;
                }

                wbWhatIsMyIP.Navigate("http://www.infobyip.com/detectproxy.php");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        #endregion

        #region Button Events

        private void btnMark_Click(object sender, EventArgs e)
        {
            if (tcMain.SelectedIndex == 2)
            {
                MarkFromButton(wbTabOne, dgvOne, _wbTabOneHtmlElements);
            }
            else if (tcMain.SelectedIndex == 3)
            {
                MarkFromButton(wbTabTwo, dgvTwo, _wbTabTwoHtmlElements);
            }
        }

        private void btnUnmark_Click(object sender, EventArgs e)
        {
            if (tcMain.SelectedIndex == 2)
            {
                UnmarkFromButton(wbTabOne, dgvOne, _wbTabOneHtmlElements);
            }
            else if (tcMain.SelectedIndex == 3)
            {
                UnmarkFromButton(wbTabTwo, dgvTwo, _wbTabTwoHtmlElements);
            }
        }

        private void btnPopulateOne_Click(object sender, EventArgs e)
        {
            tbVariables.Text = string.Empty;

            PopulateFields(wbTabOne, wbTabTwo, dgvOne);
            PopulateFields(wbTabTwo, wbTabOne, dgvTwo);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach(string line in tbVariables.Text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                stringBuilder.Append(line + ",");
            }

            File.AppendAllText("Save.txt", stringBuilder.ToString() + Environment.NewLine);
        }
        
        #endregion

        #region DataGridView Events

        private void dgvOne_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            MarkFromDataGrid(wbTabOne, dgvOne.Rows[e.RowIndex], _wbTabOneHtmlElements);
        }

        private void dgvTwo_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            MarkFromDataGrid(wbTabTwo, dgvTwo.Rows[e.RowIndex], _wbTabTwoHtmlElements);
        }

        private void dgvOne_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            UnmarkFromDataGrid(wbTabOne, e.Row, _wbTabOneHtmlElements);
        }

        private void dgvTwo_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            UnmarkFromDataGrid(wbTabTwo, e.Row, _wbTabTwoHtmlElements);
        }

        private void dgvOne_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            SaveGrid(dgvOne);

            if (e.RowIndex != -1)
            {
                MarkFromDataGrid(wbTabOne, dgvOne.Rows[e.RowIndex], _wbTabOneHtmlElements);
            }
        }

        private void dgvTwo_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            SaveGrid(dgvTwo);

            if (e.RowIndex != -1)
            {
                MarkFromDataGrid(wbTabTwo, dgvTwo.Rows[e.RowIndex], _wbTabTwoHtmlElements);
            }
        }

        #endregion

        #region Methods

        private void LoadGrid(DataGridView dataGridView)
        {
            _isLoadingGrid = true;

            string fileName = dataGridView.Name;

            if (dataGridView == dgvOne)
            {
                fileName += "_" + new Hash(tbAbsoluteUriTabOne.Text);
            }
            else if (dataGridView == dgvTwo)
            {
                fileName += "_" + new Hash(tbAbsoluteUriTabTwo.Text);
            }

            dataGridView.Rows.Clear();

            if (File.Exists(fileName))
            {
                XDocument xDocument = XDocument.Load(fileName);

                foreach (XElement row in xDocument.Descendants("Row"))
                {
                    DataGridViewRow dataGridViewRow = new DataGridViewRow();

                    foreach (XElement column in row.Descendants("Column"))
                    {
                        if (dataGridViewRow.Cells.Count != 3)
                        {
                            dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell());

                            dataGridViewRow.Cells[dataGridViewRow.Cells.Count - 1].Value = column.Value;
                        }
                        else
                        {
                            dataGridViewRow.Cells.Add(new DataGridViewCheckBoxCell());

                            dataGridViewRow.Cells[dataGridViewRow.Cells.Count - 1].Value = bool.Parse(column.Value);
                        }
                    }

                    dataGridView.Rows.Add(dataGridViewRow);
                }
            }

            _isLoadingGrid = false;
        }

        private void SaveGrid(DataGridView dataGridView)
        {
            try
            {
                if (!_isLoadingGrid)
                {
                    XDocument xDocument = new XDocument();
                    XElement root = new XElement("Root");

                    xDocument.Add(root);

                    foreach (DataGridViewRow dataGridViewRow in dataGridView.Rows)
                    {
                        XElement row = new XElement("Row");

                        root.Add(row);

                        foreach (DataGridViewCell dataGridViewCell in dataGridViewRow.Cells)
                        {
                            XElement column = new XElement("Column");

                            if (dataGridViewCell is DataGridViewTextBoxCell)
                            {
                                column.Value = (string) dataGridViewCell.Value != null ? dataGridViewCell.Value.ToString() : "";

                                row.Add(column);
                            }
                            else if (dataGridViewCell is DataGridViewCheckBoxCell)
                            {
                                column.Value = dataGridViewCell.Value != null ? dataGridViewCell.Value.ToString() : "False";

                                row.Add(column);
                            }
                        }
                    }

                    string fileName = dataGridView.Name;

                    if (dataGridView == dgvOne)
                    {
                        fileName += "_" + new Hash(tbAbsoluteUriTabOne.Text);
                    }
                    else if (dataGridView == dgvTwo)
                    {
                        fileName += "_" + new Hash(tbAbsoluteUriTabTwo.Text);
                    }

                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }

                    File.WriteAllText(fileName, xDocument.ToString());
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void MarkFromButton(WebBrowser webBrowser, DataGridView dataGridView, List<string> htmlElements)
        {
            if (webBrowser.Document != null && webBrowser.Document.ActiveElement != null)
            {
                if (!htmlElements.Contains(webBrowser.Document.ActiveElement.Id))
                {
                    webBrowser.Document.ActiveElement.Style = webBrowser.Document.ActiveElement.Style + _styleMark;

                    if (!htmlElements.Contains(webBrowser.Document.ActiveElement.Id))
                    {
                        htmlElements.Add(webBrowser.Document.ActiveElement.Id);
                    }

                    DataGridViewRow dataGridViewRow = new DataGridViewRow();

                    dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell());
                    dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell());
                    dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell());
                    dataGridViewRow.Cells.Add(new DataGridViewCheckBoxCell());

                    dataGridViewRow.SetValues(webBrowser.Document.ActiveElement.Id, null);

                    dataGridView.Rows.Add(dataGridViewRow);
                }
            }

            SaveGrid(dataGridView);
        }

        private void UnmarkFromButton(WebBrowser webBrowser, DataGridView dataGridView, List<string> htmlElements)
        {
            if (webBrowser.Document != null && webBrowser.Document.ActiveElement != null)
            {
                if (htmlElements.Contains(webBrowser.Document.ActiveElement.Id))
                {
                    HtmlElement htmlElement = webBrowser.Document.GetElementById(webBrowser.Document.ActiveElement.Id);

                    if (htmlElement != null)
                    {
                        if (!string.IsNullOrEmpty(htmlElement.Style))
                        {
                            htmlElement.Style = htmlElement.Style.Replace(_styleMark, string.Empty);
                        }

                        htmlElements.Remove(htmlElement.Id);

                        foreach (DataGridViewRow dataGridViewRow in dataGridView.Rows)
                        {
                            if (webBrowser.Document.ActiveElement != null && dataGridViewRow.Cells[0].Value.ToString() == webBrowser.Document.ActiveElement.Id)
                            {
                                dataGridView.Rows.Remove(dataGridViewRow);

                                break;
                            }
                        }
                    }
                }
            }

            SaveGrid(dataGridView);
        }

        private void PopulateFields(WebBrowser webBrowser, WebBrowser linkedWebBrowser, DataGridView dataGridView)
        {
            try
            {
                Random random = new Random();

                foreach (DataGridViewRow dataGridViewRow in dataGridView.Rows)
                {
                    if (webBrowser != null && webBrowser.Document != null && dataGridViewRow.Cells[0].Value != null)
                    {
                        HtmlElement htmlElement = webBrowser.Document.GetElementById(dataGridViewRow.Cells[0].Value.ToString());

                        if (htmlElement != null)
                        {
                            string valueTemplate = null;

                            if (dataGridViewRow.Cells[1].Value != null)
                            {
                                valueTemplate = dataGridViewRow.Cells[1].Value.ToString();
                            }

                            /**/

                            if (linkedWebBrowser != null && linkedWebBrowser.Document != null && dataGridViewRow.Cells[2].Value != null)
                            {
                                string linkedValue = dataGridViewRow.Cells[2].Value.ToString();

                                if (!string.IsNullOrEmpty(linkedValue))
                                {
                                    HtmlElement linkedHtmlElement = linkedWebBrowser.Document.GetElementById(dataGridViewRow.Cells[2].Value.ToString());

                                    if (linkedHtmlElement != null)
                                    {
                                        htmlElement.SetAttribute("value", linkedHtmlElement.GetAttribute("value"));

                                        //link overrides the valueTemplate...
                                        valueTemplate = null;
                                    }
                                }
                            }

                            /**/

                            if (!string.IsNullOrEmpty(valueTemplate))
                            {
                                StringBuilder value = new StringBuilder();

                                for (int i = 0; i < valueTemplate.Length; i++)
                                {
                                    char c = valueTemplate[i];

                                    if (c != '[' && c != ']')
                                    {
                                        value.Append(c);
                                    }
                                    else
                                    {
                                        if (c == '[')
                                        {
                                            while (++i < valueTemplate.Length && c != ']')
                                            {
                                                c = valueTemplate[i];

                                                if (c == 'c')
                                                {
                                                    value.Append((char) random.Next('A', 'Z'));
                                                }

                                                if (c == 'd')
                                                {
                                                    value.Append(random.Next(0, 9));
                                                }

                                                if (c == '@')
                                                {
                                                    StringBuilder namedEntity = new StringBuilder();

                                                    while (++i < valueTemplate.Length)
                                                    {
                                                        c = valueTemplate[i];

                                                        if (c != '@')
                                                        {
                                                            namedEntity.Append(c);
                                                        }
                                                        else
                                                        {
                                                            break;
                                                        }
                                                    }

                                                    if (namedEntity.ToString().ToLowerInvariant() == "lastname")
                                                    {
                                                        value.Append(Strings.ToProperCase(_lastNames[random.Next(0, _lastNames.Count - 1)]));
                                                    }

                                                    if (namedEntity.ToString().ToLowerInvariant() == "firstname_male")
                                                    {
                                                        value.Append(Strings.ToProperCase(_firstNames_Male[random.Next(0, _firstNames_Male.Count - 1)]));
                                                    }

                                                    if (namedEntity.ToString().ToLowerInvariant() == "firstname_female")
                                                    {
                                                        value.Append(Strings.ToProperCase(_firstNames_Female[random.Next(0, _firstNames_Female.Count - 1)]));
                                                    }
                                                }

                                                if (c == '#')
                                                {
                                                    StringBuilder namedEntity = new StringBuilder();

                                                    while (++i < valueTemplate.Length && c != ']')
                                                    {
                                                        c = valueTemplate[i];

                                                        if (c != '#')
                                                        {
                                                            namedEntity.Append(c);
                                                        }
                                                        else
                                                        {
                                                            break;
                                                        }
                                                    }

                                                    HtmlElement htmlElement2 = webBrowser.Document.GetElementById(namedEntity.ToString());

                                                    if (htmlElement2 != null)
                                                    {
                                                        value.Append(htmlElement2.GetAttribute("value"));
                                                    }
                                                }
                                            }

                                            i--;
                                        }
                                    }
                                }

                                htmlElement.SetAttribute("value", value.ToString());
                                htmlElement.SetAttribute("option", value.ToString());
                            }

                            if (dataGridViewRow.Cells[3].Value != null && (bool)dataGridViewRow.Cells[3].Value)
                            {
                                tbVariables.AppendText(htmlElement.Id + ": " + htmlElement.GetAttribute("value") + Environment.NewLine);
                            }
                        }
                    }
                }

                tbVariables.AppendText(Environment.NewLine);
            }
            catch (Exception exception)
            {
                

                MessageBox.Show(exception.Message);
            }
        }

        private void MarkFromDataGrid(WebBrowser webBrowser, DataGridViewRow dataGridViewRow, List<string> htmlElements)
        {
            if (webBrowser != null && webBrowser.Document != null && dataGridViewRow.Cells[0].Value != null)
            {
                HtmlElement htmlElement = webBrowser.Document.GetElementById(dataGridViewRow.Cells[0].Value.ToString());

                if (htmlElement != null)
                {
                    if (string.IsNullOrEmpty(htmlElement.Style) || !htmlElement.Style.EndsWith(_styleMark))
                    {
                        htmlElement.Style = htmlElement.Style + _styleMark;
                    }

                    if (!htmlElements.Contains(htmlElement.Id))
                    {
                        htmlElements.Add(htmlElement.Id);
                    }
                }
            }
        }

        private void UnmarkFromDataGrid(WebBrowser webBrowser, DataGridViewRow dataGridViewRow, List<string> htmlElements)
        {
            if (webBrowser != null && webBrowser.Document != null && dataGridViewRow.Cells[0].Value != null)
            {
                HtmlElement htmlElement = webBrowser.Document.GetElementById(dataGridViewRow.Cells[0].Value.ToString());

                if (htmlElement != null)
                {
                    if (!string.IsNullOrEmpty(htmlElement.Style))
                    {
                        htmlElement.Style = htmlElement.Style.Replace(_styleMark, string.Empty);
                    }

                    htmlElements.Remove(htmlElement.Id);
                }
            }
        }

        #endregion
    }
}