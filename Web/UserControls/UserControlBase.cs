using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Core;
using Arachnode.SiteCrawler.Managers;

namespace Arachnode.Web.UserControls
{
    public class UserControlBase : UserControl
    {
        public UserControlBase()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            //populates the Application and Web settings...
            IArachnodeDAO arachnodeDAO = ArachnodeDAO;
        }

        protected ApplicationSettings ApplicationSettings
        {
            get
            {
                if (Session["_applicationSettings"] == null)
                {
                    Session["_applicationSettings"] = new ApplicationSettings();
                }

                return (ApplicationSettings)Session["_applicationSettings"];
            }
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
                    Session["_arachnodeDAO"] = new ArachnodeDAO(WebSettings.ConnectionString, ApplicationSettings, WebSettings, true, true);
                }

                return (IArachnodeDAO)Session["_arachnodeDAO"];
            }
        }

        protected ActionManager<ArachnodeDAO> ActionManager
        {
            get
            {
                if (Session["_actionManager"] == null)
                {
                    Session["_actionManager"] = new ActionManager<ArachnodeDAO>(ApplicationSettings, WebSettings, ConsoleManager);
                }

                return (ActionManager<ArachnodeDAO>)Session["_actionManager"];
            }
        }

        protected new Cache<ArachnodeDAO> Cache
        {
            get
            {
                if (Session["_cache"] == null)
                {
                    Session["_cache"] = new Cache<ArachnodeDAO>(ApplicationSettings, WebSettings, null, ActionManager, CacheManager, null, MemoryManager, RuleManager);
                }

                return (Cache<ArachnodeDAO>)Session["_cache"];
            }
        }

        protected CacheManager<ArachnodeDAO> CacheManager
        {
            get
            {
                if (Session["_cacheManager"] == null)
                {
                    Session["_cacheManager"] = new CacheManager<ArachnodeDAO>(ApplicationSettings, WebSettings);
                }

                return (CacheManager<ArachnodeDAO>)Session["_cacheManager"];
            }
        }

        protected ConsoleManager<ArachnodeDAO> ConsoleManager
        {
            get
            {
                if (Session["_consoleManager"] == null)
                {
                    Session["_consoleManager"] = new ConsoleManager<ArachnodeDAO>(ApplicationSettings, WebSettings);
                }

                return (ConsoleManager<ArachnodeDAO>)Session["_consoleManager"];
            }
        }

        protected DiscoveryManager<ArachnodeDAO> DiscoveryManager
        {
            get
            {
                if (Session["_discoveryManager"] == null)
                {
                    Session["_discoveryManager"] = new DiscoveryManager<ArachnodeDAO>(ApplicationSettings, WebSettings, Cache, ActionManager, CacheManager, MemoryManager, RuleManager);
                }

                return (DiscoveryManager<ArachnodeDAO>)Session["_discoveryManager"];
            }
        }

        protected HtmlManager<ArachnodeDAO> HtmlManager
        {
            get
            {
                if (Session["_htmlManager"] == null)
                {
                    Session["_htmlManager"] = new HtmlManager<ArachnodeDAO>(ApplicationSettings, WebSettings, DiscoveryManager);
                }

                return (HtmlManager<ArachnodeDAO>)Session["_htmlManager"];
            }
        }

        protected MemoryManager<ArachnodeDAO> MemoryManager
        {
            get
            {
                if (Session["_memoryManager"] == null)
                {
                    Session["_memoryManager"] = new MemoryManager<ArachnodeDAO>(ApplicationSettings, WebSettings);
                }

                return (MemoryManager<ArachnodeDAO>)Session["_memoryManager"];
            }
        }

        protected RuleManager<ArachnodeDAO> RuleManager
        {
            get
            {
                if (Session["_ruleManager"] == null)
                {
                    Session["_ruleManager"] = new RuleManager<ArachnodeDAO>(ApplicationSettings, WebSettings, ConsoleManager);
                }

                return (RuleManager<ArachnodeDAO>)Session["_ruleManager"];
            }
        }

        protected WebSettings WebSettings
        {
            get
            {
                if (Session["_webSettings"] == null)
                {
                    Session["_webSettings"] = new WebSettings();
                }

                return (WebSettings)Session["_webSettings"];
            }
        }
    }
}
