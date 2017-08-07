using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Arachnode.Renderer.Value;
using mshtml;

namespace Arachnode.Renderer.RendererActions
{
    public class Inputs : Arachnode.Renderer.Value.AbstractClasses.ARendererAction
    {
        public override void PerformAction(RendererMessage rendererMessage, HTMLDocumentClass htmlDocumentClass)
        {
            foreach (IHTMLElement htmlElement in htmlDocumentClass.getElementsByTagName("input"))
            {
                if (rendererMessage != null && rendererMessage.PropertiesKeys != null && rendererMessage.PropertiesValues != null)
                {
                    object id = htmlElement.getAttribute("id");

                    if (id != null && id is string && !string.IsNullOrEmpty((string)id))
                    {
                        if (!rendererMessage.PropertiesValues.Contains(id))
                        {
                            rendererMessage.PropertiesKeys.Add("input_id");
                            rendererMessage.PropertiesValues.Add(id);
                        }
                    }
                }
            }
        }
    }
}
