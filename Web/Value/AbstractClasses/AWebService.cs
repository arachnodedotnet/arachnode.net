using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using System.Web.Services;
using System.Xml;
using System.Xml.Linq;
using Arachnode.Configuration;
using Arachnode.Configuration.Value.Enums;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Managers;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler;

namespace Arachnode.Web.Value.AbstractClasses
{
    public class AWebService : WebService
    {
        private ApplicationSettings _applicationSettings = new ApplicationSettings();
        private WebSettings _webSettings = new WebSettings();

        [WebMethod(Description = "Gets the ApplicationSettings for the service instance.", EnableSession = true)]
        public virtual ApplicationSettings GetApplicationSettings()
        {
            if (Session["_applicationSettings"] != null)
            {
                return (ApplicationSettings) Session["_applicationSettings"];
            }

            Session["_applicationSettings"] = _applicationSettings;

            return _applicationSettings;
        }

        [WebMethod(Description = "Gets the WebSettings for the service instance.", EnableSession = true)]
        public virtual WebSettings GetWebSettings()
        {
            if (Session["_webSettings"] != null)
            {
                return (WebSettings) Session["_webSettings"];
            }

            Session["_webSettings"] = _webSettings;

            return _webSettings;
        }

        [WebMethod(Description = "Sets the ApplicationSettings for the service instance.", EnableSession = true)]
        public virtual void SetApplicationSettings(ApplicationSettings applicationSettings)
        {
            Session["_applicationSettings"] = applicationSettings;
        }

        [WebMethod(Description = "Sets the WebSettings for the service instance.", EnableSession = true)]
        public virtual void SetWebSettings(WebSettings webSettings)
        {
            Session["_webSettings"] = webSettings;
        }

        public AWebService()
        {
            //populates the Application and Web settings...
            IArachnodeDAO arachnodeDAO = ArachnodeDAO;
        }

        /// <summary>
        /// 	Gets the arachnode DAO.
        /// </summary>
        /// <value>The arachnode DAO.</value>
        protected IArachnodeDAO ArachnodeDAO
        {
            get
            {
                if (Session["_arachnodeDAO"] == null)
                {
                    Session["_arachnodeDAO"] = new ArachnodeDAO(_webSettings.ConnectionString, ApplicationSettings, WebSettings, true, true);
                }

                return (IArachnodeDAO)Session["_arachnodeDAO"];
            }
            set { Session["_arachnodeDAO"] = value; }
        }

        public ApplicationSettings ApplicationSettings
        {
            get { return GetApplicationSettings(); }
            set { Session["_applicationSettings"] = value; }
        }

        public WebSettings WebSettings
        {
            get { return GetWebSettings(); }
            set { Session["_webSettings"] = value; }
        }

        protected void Service_Start(object sender, EventArgs e)
        {
            InitializeConfiguration();
        }

        [WebMethod(EnableSession = true)]
        public void InitializeConfiguration()
        {
            ConfigurationManager.InitializeConfiguration(ref _applicationSettings, ref _webSettings, ConfigurationType.Application, ArachnodeDAO);
            ConfigurationManager.InitializeConfiguration(ref _applicationSettings, ref _webSettings, ConfigurationType.Web, ArachnodeDAO);
        }

        [WebMethod(EnableSession = true)]
        public string GetPerformanceCountersForArachnodeDotNet()
        {
            return GetPerformanceCounters("arachnode.net");
        }

        [WebMethod(EnableSession = true)]
        public string GetPerformanceCounters(string categoryNameStartsWith)
        {
            StringBuilder stringBuilder = new StringBuilder();

            try
            {
                DataTable dataTable = new DataTable("PerformanceCounters");
                dataTable.Columns.Add("Category");
                dataTable.Columns.Add("Name");
                dataTable.Columns.Add("Value");

                string category = null;

                foreach (PerformanceCounterCategory performanceCounterCategory in PerformanceCounterCategory.GetCategories().OrderBy(pc => pc.CategoryName))
                {
                    if (performanceCounterCategory.CategoryName.StartsWith(categoryNameStartsWith))
                    {
                        if (category == null || category != performanceCounterCategory.CategoryName)
                        {
                            category = performanceCounterCategory.CategoryName;
                        }

                        foreach (PerformanceCounter performanceCounter in performanceCounterCategory.GetCounters())
                        {
                            dataTable.Rows.Add(category, performanceCounter.CounterName, performanceCounter.RawValue);
                        }
                    }
                }

                XmlWriter xmlWriter = XmlWriter.Create(stringBuilder);

                dataTable.WriteXml(xmlWriter);
            }
            catch (Exception exception)
            {
                ArachnodeDAO.InsertException(null, null, exception, false);

                throw;
            }

            return stringBuilder.ToString();
        }

        [WebMethod(EnableSession = true)]
        public DataTable GetAllTableRowCounts()
        {
            return ArachnodeDAO.ExecuteSql2("SELECT DISTINCT s.name + ' ' + t.name, i.rows from    sys.tables t join sys.schemas s on t.schema_id = s.schema_id join sys.sysindexes i on t.object_id = i.id where   t.type_desc = 'USER_TABLE' and i.root is not null and t.name <> 'sysdiagrams' order by s.name + ' ' + t.name ASC");
        }
    }
}
