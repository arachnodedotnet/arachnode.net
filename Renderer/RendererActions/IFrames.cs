using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arachnode.Renderer.Value;
using mshtml;

namespace Arachnode.Renderer.RendererActions
{
    public class IFrames : Arachnode.Renderer.Value.AbstractClasses.ARendererAction
    {
        public override void PerformAction(RendererMessage rendererMessage, HTMLDocumentClass htmlDocumentClass)
        {
            foreach(IHTMLElement htmlElement in htmlDocumentClass.getElementsByTagName("iframe"))
            {
                if (rendererMessage != null && rendererMessage.PropertiesKeys != null && rendererMessage.PropertiesValues != null)
                {
                    rendererMessage.PropertiesKeys.Add("iframe_src");
                    rendererMessage.PropertiesValues.Add(htmlElement.getAttribute("src"));
                }
            }
        }
    }
}
