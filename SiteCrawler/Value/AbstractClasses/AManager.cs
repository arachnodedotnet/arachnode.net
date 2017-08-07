using Arachnode.Configuration;
using Arachnode.DataAccess.Value.Interfaces;
using System;

namespace Arachnode.SiteCrawler.Value.AbstractClasses
{
    public abstract class AManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        protected AManager(ApplicationSettings applicationSettings, WebSettings webSettings)
        {
            _applicationSettings = applicationSettings;
            _webSettings = webSettings;
        }

        private Guid _guid = Guid.NewGuid();

        protected static object _staticLock = new object();
        protected object _instanceLock = new object();

        private ApplicationSettings _applicationSettings;

        protected internal ApplicationSettings ApplicationSettings
        {
            get { return _applicationSettings; }
            set { _applicationSettings = value; }
        }

        private WebSettings _webSettings;

        protected internal WebSettings WebSettings
        {
            get { return _webSettings; }
            set { _webSettings = value; }
        }
    }
}