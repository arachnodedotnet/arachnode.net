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
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Arachnode.Configuration;

#endregion

namespace Arachnode.Web
{
    public partial class Test : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                double numberOfTestWebPageRequests = double.Parse(Request.QueryString["numberOfTestWebPageRequests"]);
                Title = "RequestedTestWebPageNumber: " + string.Format("{0:000000000}", double.Parse(Request.QueryString["requestedTestWebPageNumber"]));

                //ANODET: Add a comment about how the number of links and the offset affect Discoveries and when CrawlRequests are cached to disk...
                double offset = (ApplicationSettings.MaximumNumberOfCrawlThreads*30*(numberOfTestWebPageRequests - 1));
                //offset = 0;

                for (int i = 0; i < ApplicationSettings.MaximumNumberOfCrawlThreads*30; i++)
                {
                    HyperLink hyperLink = new HyperLink();

                    hyperLink.NavigateUrl = "Test_" + string.Format("{0:000000000}", offset + (i + numberOfTestWebPageRequests)) + ".aspx";
                    hyperLink.Text = "HyperLink #" + string.Format("{0:000000000}", offset + (i + numberOfTestWebPageRequests));

                    uxPhHl.Controls.Add(hyperLink);

                    Literal literal = new Literal();

                    literal.Text = "<br/>";

                    uxPhHl.Controls.Add(literal);
                }

                Random random = new Random();

                StringBuilder stringBuilder = new StringBuilder();

                for (int i = 0; i < 1000; i++)
                {
                    stringBuilder.Append((char) random.Next(1, 254));
                }

                uxLRandomText.Text = HttpUtility.HtmlEncode(stringBuilder.ToString());
            }
            catch
            {
            }
        }
    }
}