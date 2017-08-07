using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arachnode.Configuration;
using Arachnode.DataAccess.Value.Interfaces;

namespace Arachnode.SiteCrawler.Value.AbstractClasses
{
    public abstract class ARobotsDotTextManager<TArachnodeDAO> : AManager<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        public ARobotsDotTextManager(ApplicationSettings applicationSettings, WebSettings webSettings) : base(applicationSettings, webSettings)
        {
        }

        /// <summary>
        /// 	Parses the robots dot text.
        /// </summary>
        /// <param name = "baseUri">The base URI.</param>
        /// <param name = "robotsDotTextSource">The robots dot text source.</param>
        /// <returns></returns>
        public abstract RobotsDotText ParseRobotsDotTextSource(Uri baseUri, byte[] robotsDotTextSource);
    }
}
