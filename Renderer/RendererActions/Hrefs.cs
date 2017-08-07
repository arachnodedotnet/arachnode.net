using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arachnode.Renderer.Value;
using HtmlAgilityPack;
using mshtml;

namespace Arachnode.Renderer.RendererActions
{
    public class Hrefs : Arachnode.Renderer.Value.AbstractClasses.ARendererAction
    {
        public override void PerformAction(RendererMessage rendererMessage, HTMLDocumentClass htmlDocumentClass)
        {
            foreach(IHTMLElement htmlElement in htmlDocumentClass.all)
            {
                if (rendererMessage != null && rendererMessage.PropertiesKeys != null && rendererMessage.PropertiesValues != null)
                {
                    object href = htmlElement.getAttribute("href");

                    if (href != null && href is string && !string.IsNullOrEmpty((string)href))
                    {
                        if (!rendererMessage.PropertiesValues.Contains(href))
                        {
                            rendererMessage.PropertiesKeys.Add("element_href");
                            rendererMessage.PropertiesValues.Add(href);
                        }
                    }

                    if (htmlElement.innerHTML != null && htmlElement.innerHTML.ToLowerInvariant().Contains("href"))
                    {
                        HtmlDocument htmlDocument = new HtmlDocument();

                        htmlDocument.LoadHtml(htmlElement.innerHTML);

                        if (htmlDocument.DocumentNode.Attributes != null)
                        {
                            foreach (HtmlAgilityPack.HtmlNode htmlNode in htmlDocument.DocumentNode.Descendants())
                            {
                                if (htmlNode.Attributes != null)
                                {
                                    foreach (HtmlAttribute htmlAttribute in htmlNode.Attributes)
                                    {
                                        if (htmlAttribute.Name.ToLowerInvariant().Contains("href"))
                                        {
                                            if (!rendererMessage.PropertiesValues.Contains(htmlAttribute.Value))
                                            {
                                                rendererMessage.PropertiesKeys.Add("element_href");
                                                rendererMessage.PropertiesValues.Add(htmlAttribute.Value);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
