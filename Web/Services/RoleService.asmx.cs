using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web.Security;
using System.Web.Services;

namespace Arachnode.Web.Services
{
    /// <summary>
    /// Summary description for RoleService
    /// </summary>
    [WebService(Namespace = "http://arachnode.net/RoleService")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
        // [System.Web.Script.Services.ScriptService]
    public class RoleService : WebService
    {
        private readonly SqlRoleProvider _sqlRoleProvider = new SqlRoleProvider();

        public RoleService()
        {
            //HACK: Hardcoded values!!!
            NameValueCollection config = new NameValueCollection();
            config.Add("connectionStringName", "arachnode_net_ConnectionString");
            config.Add("applicationName", "arachnode.net");

            SqlRoleProvider.Initialize("SqlRoleProvider", config);
        }

        public SqlRoleProvider SqlRoleProvider
        {
            get { return _sqlRoleProvider; }
        }

        [WebMethod]
        public void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            SqlRoleProvider.AddUsersToRoles(usernames, roleNames);
        }

        [WebMethod]
        public void CreateRole(string roleName)
        {
            SqlRoleProvider.CreateRole(roleName);
        }

        [WebMethod]
        public bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            return SqlRoleProvider.DeleteRole(roleName, throwOnPopulatedRole);
        }

        [WebMethod]
        public string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            return SqlRoleProvider.FindUsersInRole(roleName, usernameToMatch);
        }

        [WebMethod]
        public string[] GetAllRoles()
        {
            return SqlRoleProvider.GetAllRoles();
        }

        [WebMethod]
        public string[] GetRolesForUser(string username)
        {
            return SqlRoleProvider.GetRolesForUser(username);
        }

        [WebMethod]
        public string[] GetUsersInRole(string roleName)
        {
            return SqlRoleProvider.GetUsersInRole(roleName);
        }

        [WebMethod]
        public bool IsUserInRole(string username, string roleName)
        {
            return SqlRoleProvider.IsUserInRole(username, roleName);
        }

        [WebMethod]
        public void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            SqlRoleProvider.RemoveUsersFromRoles(usernames, roleNames);
        }

        [WebMethod]
        public bool RoleExists(string roleName)
        {
            return SqlRoleProvider.RoleExists(roleName);
        }
    }
}