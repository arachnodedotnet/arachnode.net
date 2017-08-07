using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arachnode.Renderer.Value;
using mshtml;

namespace Arachnode.SiteCrawler.Value
{
    internal class RendererResponse
    {
        public HTMLDocumentClass HTMLDocumentClass { get; set; }
        public RendererMessage RendererMessage { get; set; }
    }
}
