using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.AbstractClasses;
using Arachnode.SiteCrawler.Value.Enums;
using HtmlAgilityPack;

namespace Arachnode.Plugins.CrawlActions
{
    public class BusinessInformation<TArachnodeDAO> : ACrawlAction<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        public BusinessInformation(ApplicationSettings applicationSettings, WebSettings webSettings) : base(applicationSettings, webSettings)
        {
        }

        public override void AssignSettings(Dictionary<string, string> settings)
        {
            //throw new NotImplementedException();
        }

        public override void PerformAction(CrawlRequest<TArachnodeDAO> crawlRequest, IArachnodeDAO arachnodeDAO)
        {
            if (crawlRequest.DataType.DiscoveryType == DiscoveryType.WebPage)
            {
                try
                {
                    string name = null;
                    string address1 = null;
                    string address2 = null;
                    string city = null;
                    string state = null;
                    string zip = null;
                    string phoneNumber = null;
                    string category = null;
                    string latitude = null;
                    string longitude = null;

                    HtmlDocument htmlDocument = new HtmlDocument();

                    htmlDocument.LoadHtml(crawlRequest.DecodedHtml);

                    //code your specific logic here...

                    arachnodeDAO.InsertBusinessInformation(crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri, crawlRequest.Discovery.ID.Value, name, address1, address2, city, state, zip, phoneNumber, category, latitude, longitude);
                }
                catch (Exception exception)
                {
                    arachnodeDAO.InsertException(crawlRequest.Parent.Uri.AbsoluteUri, crawlRequest.Discovery.Uri.AbsoluteUri, exception, false);
                }
            }
        }

        public override void Stop()
        {
            //throw new NotImplementedException();
        }
    }
}
