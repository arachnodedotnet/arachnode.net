using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mshtml;

namespace Arachnode.Renderer.Value.AbstractClasses
{
    public abstract class ARendererAction
    {
        public abstract void PerformAction(RendererMessage rendererMessage, HTMLDocumentClass htmlDocumentClass);
    }
}
