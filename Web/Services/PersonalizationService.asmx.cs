using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web.Services;
using System.Web.UI.WebControls.WebParts;

namespace Arachnode.Web.Services
{
    /// <summary>
    /// Summary description for PersonalizationService
    /// </summary>
    [WebService(Namespace = "http://arachnode.net/PersonalizationService")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
        // [System.Web.Script.Services.ScriptService]
    public class PersonalizationService : WebService
    {
        private readonly SqlPersonalizationProvider _sqlPersonalizationProvider = new SqlPersonalizationProvider();

        public PersonalizationService()
        {
            //HACK: Hardcoded values!!!
            var config = new NameValueCollection();
            config.Add("connectionStringName", "arachnode_net_ConnectionString");
            config.Add("applicationName", "arachnode.net");

            SqlPersonalizationProvider.Initialize("SqlPersonalizationProvider", config);
        }

        public SqlPersonalizationProvider SqlPersonalizationProvider
        {
            get { return _sqlPersonalizationProvider; }
        }

        public PersonalizationScope DetermineInitialScope(WebPartManager webPartManager, PersonalizationState loadedState)
        {
            return SqlPersonalizationProvider.DetermineInitialScope(webPartManager, loadedState);
        }

        public IDictionary DetermineUserCapabilities(WebPartManager webPartManager)
        {
            return SqlPersonalizationProvider.DetermineUserCapabilities(webPartManager);
        }

        [WebMethod]
        public PersonalizationStateInfoCollection FindState(PersonalizationScope scope, PersonalizationStateQuery query, int pageIndex, int pageSize, out int totalRecords)
        {
            return SqlPersonalizationProvider.FindState(scope, query, pageIndex, pageSize, out totalRecords);
        }

        [WebMethod]
        public int GetCountOfState(PersonalizationScope scope, PersonalizationStateQuery query)
        {
            return SqlPersonalizationProvider.GetCountOfState(scope, query);
        }

        public PersonalizationState LoadPersonalizationState(WebPartManager webPartManager, bool ignoreCurrentUser)
        {
            return SqlPersonalizationProvider.LoadPersonalizationState(webPartManager, ignoreCurrentUser);
        }

        public void ResetPersonalizationState(WebPartManager webPartManager)
        {
            SqlPersonalizationProvider.ResetPersonalizationState(webPartManager);
        }

        [WebMethod]
        public int ResetState(PersonalizationScope scope, string[] paths, string[] usernames)
        {
            return SqlPersonalizationProvider.ResetState(scope, paths, usernames);
        }

        [WebMethod]
        public int ResetUserState(string path, DateTime userInactiveSinceDate)
        {
            return SqlPersonalizationProvider.ResetUserState(path, userInactiveSinceDate);
        }

        [WebMethod]
        public void SavePersonalizationState(PersonalizationState state)
        {
            SqlPersonalizationProvider.SavePersonalizationState(state);
        }
    }
}