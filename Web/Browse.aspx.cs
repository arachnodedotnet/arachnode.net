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
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.DataSource;
using Arachnode.SiteCrawler.Managers;
using Arachnode.SiteCrawler.Value.Enums;

#endregion

namespace Arachnode.Web
{
    public partial class Browse : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            EnableViewState = false;

            try
            {
                if (Request.QueryString.Count == 2 && Request.QueryString.AllKeys[0] == "discoveryID" && Request.QueryString.AllKeys[1] == "absoluteUri")
                {
                    ArachnodeDataSet.WebPagesRow webPagesRow = ArachnodeDAO.GetWebPage(Request.QueryString["discoveryID"]);

                    if (webPagesRow != null)
                    {
                        string source = null;

                        if (webPagesRow.Source == null || webPagesRow.Source.Length == 0)
                        {
                            string discoveryPath = DiscoveryManager.GetDiscoveryPath(ApplicationSettings.DownloadedWebPagesDirectory, webPagesRow.AbsoluteUri, webPagesRow.FullTextIndexType);

                            if (File.Exists(discoveryPath))
                            {
                                source = File.ReadAllText(discoveryPath, Encoding.GetEncoding(webPagesRow.CodePage));
                            }
                            else
                            {
                                uxLblException.Text = "The WebPage source was not found in the database or on disk.";
                                uxLblException.Visible = true;

                                return;
                            }
                        }
                        else
                        {
                            source = Encoding.GetEncoding(webPagesRow.CodePage).GetString(webPagesRow.Source);
                        }

                        //Request.Url.Scheme + "://" + Request.Url.Authority

                        //ANODET: Should this be a configuration setting?  Perhaps - hotlinking isn't exactly polite, but does provide the best user experience.  (Version 1.5)
                        uxLWebPage.Text = HtmlManager.CreateHtmlDocument(webPagesRow.AbsoluteUri, webPagesRow.FullTextIndexType, source, UriQualificationType.AbsoluteWhenDownloadedDiscoveryIsUnavailable, ArachnodeDAO, true).DocumentNode.OuterHtml;
                    }
                    else
                    {
                        uxLblException.Text = "The WebPage was not found in the database.";
                        uxLblException.Visible = true;
                    }
                }
            }
            catch (Exception exception)
            {
                uxLblException.Text = exception.Message;
                uxLblException.Visible = true;

                ArachnodeDAO.InsertException(null, null, exception, false);
            }
        }
    }
}