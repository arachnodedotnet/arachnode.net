using System.Web.Services;
using Arachnode.Web.Value.AbstractClasses;

namespace Arachnode.Web.Services
{
    /// <summary>
    /// Summary description for ServiceService
    /// </summary>
    [WebService(Namespace = "http://arachnode.net/ServiceService")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ServiceService : AWebService
    {
        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
    }
}
