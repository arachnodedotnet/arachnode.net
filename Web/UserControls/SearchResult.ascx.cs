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
using System.Web;
using System.Web.UI;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.Security;
using Arachnode.SiteCrawler.Managers;
using Lucene.Net.Documents;

#endregion

namespace Arachnode.Web.UserControls
{
    public partial class SearchResult : UserControlBase
    {
        /// <summary>
        /// 	Gets or sets the document.
        /// </summary>
        /// <value>The document.</value>
        public Document Document { get; set; }

        /// <summary>
        /// 	Gets or sets the summary.
        /// </summary>
        /// <value>The summary.</value>
        public string Summary { get; set; }

        /// <summary>
        /// 	Handles the Load event of the Page control.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.EventArgs" /> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Document != null)
            {
                string absoluteUri = Document.GetField("absoluteuri").StringValue();
                string pageTitle = Document.GetField("title").StringValue();
                string discoveryType = Document.GetField("discoverytype").StringValue();
                string discoveryID = Document.GetField("discoveryid").StringValue();

                uxHlTitle.NavigateUrl = Document.GetField("absoluteuri").StringValue();
                if (!string.IsNullOrEmpty(pageTitle))
                {
                    uxHlTitle.Text = pageTitle.Length > WebSettings.MaximumPageTitleLength ? pageTitle.Substring(0, WebSettings.MaximumPageTitleLength) + "..." : pageTitle;
                }
                else
                {
                    if (!UserDefinedFunctions.ExtractFileName(absoluteUri).IsNull)
                    {
                        uxHlTitle.Text = UserDefinedFunctions.ExtractFileName(absoluteUri).Value.Length > WebSettings.MaximumPageTitleLength ? UserDefinedFunctions.ExtractFileName(absoluteUri).Value.Substring(0, WebSettings.MaximumPageTitleLength) + "..." : UserDefinedFunctions.ExtractFileName(absoluteUri).Value;
                    }
                    else
                    {
                        uxHlTitle.Text = absoluteUri;
                    }
                }
                if (discoveryType != "image")
                {
                    uxLblSummary.Text = Summary;
                    uxImgImage.Visible = false;
                }
                else
                {
                    uxLblSummary.Visible = false;

                    uxImgImage.ImageUrl = HtmlManager.GetImageUrl(absoluteUri, Document.GetField("fulltextindextype").StringValue(), ArachnodeDAO);
                }
                uxLblAbsoluteUri.Text = absoluteUri;
                switch (discoveryType)
                {
                    case "file":
                        uxHlCached.NavigateUrl = HtmlManager.GetFileUrl(absoluteUri, Document.GetField("fulltextindextype").StringValue(), ArachnodeDAO);
                        break;
                    case "image":
                        uxHlCached.NavigateUrl = HtmlManager.GetImageUrl(absoluteUri, Document.GetField("fulltextindextype").StringValue(), ArachnodeDAO);
                        break;
                    case "webpage":
                        uxHlBrowse.Visible = true;
                        uxHlBrowse.NavigateUrl = "/Browse.aspx?discoveryID=" + discoveryID + "&absoluteUri=" + HttpUtility.UrlEncode(absoluteUri);
                        uxHlCached.NavigateUrl = "/Cached.aspx?discoveryID=" + discoveryID + "&absoluteUri=" + HttpUtility.UrlEncode(absoluteUri) + "&webPage=" + Encryption.EncryptRijndaelManaged(Document.GetField("discoverypath").StringValue()) + "&codePage=" + Document.GetField("codepage").StringValue() + "&fullTextIndexType=" + Document.GetField("fulltextindextype").StringValue();
                        break;
                }
                uxHlExplain.NavigateUrl = "/Explanation.aspx?query=" + Request.QueryString["query"] + "&absoluteUri=" + absoluteUri + "&documentID=" + Document.GetField("documentid").StringValue() + "&strength=" + Document.GetField("strength").StringValue();

                double score;

                double.TryParse(Document.GetField("relevancyscore").StringValue(), out score);

                double strength;

                double.TryParse(Document.GetField("strength").StringValue(), out strength);

                uxLblScoreAndStrength.Text = "Score:" + Math.Round(score, 2) + " Strength:" + Math.Round(strength, 2) + " = Total:" + (score*strength);
            }
        }
    }
}