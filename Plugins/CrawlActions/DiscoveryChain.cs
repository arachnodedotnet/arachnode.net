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

using System.Collections.Generic;
using System.IO;
using System.Text;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.AbstractClasses;

#endregion

namespace Arachnode.Plugins.CrawlActions
{
    internal class DiscoveryChain<TArachnodeDAO> : ACrawlAction<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        private readonly Dictionary<string, StringBuilder> _discoveryChain = new Dictionary<string, StringBuilder>();
        private readonly object _discoveryChainLock = new object();
        private string _discoveryChainPath;

        public DiscoveryChain(ApplicationSettings applicationSettings, WebSettings webSettings)
            : base(applicationSettings, webSettings)
        {
        }

        public override void AssignSettings(Dictionary<string, string> settings)
        {
            _discoveryChainPath = settings["DiscoveryChainPath"];
        }

        public override void Stop()
        {
            StringBuilder discoveryChain = new StringBuilder();

            foreach (KeyValuePair<string, StringBuilder> keyValuePair in _discoveryChain)
            {
                if (keyValuePair.Value[0] != '|')
                {
                    discoveryChain.AppendLine(keyValuePair.Value.ToString());

                    keyValuePair.Value.Insert(0, "|");
                }
            }

            File.WriteAllText(_discoveryChainPath, discoveryChain.ToString());
        }

        public override void PerformAction(CrawlRequest<TArachnodeDAO> crawlRequest, IArachnodeDAO arachnodeDAO)
        {
            lock (_discoveryChainLock)
            {
                if (!_discoveryChain.ContainsKey(crawlRequest.Parent.Uri.AbsoluteUri))
                {
                    _discoveryChain.Add(crawlRequest.Parent.Uri.AbsoluteUri, new StringBuilder());
                }

                if (!_discoveryChain.ContainsKey(crawlRequest.Discovery.Uri.AbsoluteUri))
                {
                    _discoveryChain.Add(crawlRequest.Discovery.Uri.AbsoluteUri, null);

                    _discoveryChain[crawlRequest.Discovery.Uri.AbsoluteUri] = _discoveryChain[crawlRequest.Parent.Uri.AbsoluteUri];
                }

                _discoveryChain[crawlRequest.Discovery.Uri.AbsoluteUri].Append(crawlRequest.Discovery.Uri.AbsoluteUri + "|");
            }
        }
    }
}