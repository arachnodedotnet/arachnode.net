using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Web.Security;
using System.Web.Services;

namespace Arachnode.Web.Services
{
    /// <summary>
    /// Summary description for MembershipService
    /// </summary>
    [WebService(Namespace = "http://arachnode.net/MembershipService")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
        // [System.Web.Script.Services.ScriptService]
    public class MembershipService : WebService
    {
        private SqlMembershipProvider _sqlMembershipProvider = new SqlMembershipProvider();

        public MembershipService()
        {
            //HACK: Hardcoded values!!!
            NameValueCollection config = new NameValueCollection();
            config.Add("name", "SqlMembershipProvider");
            config.Add("connectionStringName", "arachnode_net_ConnectionString");
            config.Add("applicationName", "arachnode.net");

            SqlMembershipProvider.Initialize("SqlMembershipProvider", config);
        }

        public SqlMembershipProvider SqlMembershipProvider
        {
            get { return _sqlMembershipProvider; }
            set { _sqlMembershipProvider = value; }
        }

        [WebMethod]
        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            return SqlMembershipProvider.ChangePassword(username, oldPassword, newPassword);
        }

        [WebMethod]
        public bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            return SqlMembershipProvider.ChangePasswordQuestionAndAnswer(username, password, newPasswordQuestion, newPasswordAnswer);
        }

        [WebMethod]
        public MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            return SqlMembershipProvider.CreateUser(username, password, email, passwordQuestion, passwordAnswer, isApproved, providerUserKey, out status);
        }

        [WebMethod]
        public bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            return SqlMembershipProvider.DeleteUser(username, deleteAllRelatedData);
        }

        [WebMethod]
        public List<MembershipUser> FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            return SqlMembershipProvider.FindUsersByEmail(emailToMatch, pageIndex, pageSize, out totalRecords).Cast<MembershipUser>().ToList();
        }

        [WebMethod]
        public List<MembershipUser> FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            return SqlMembershipProvider.FindUsersByName(usernameToMatch, pageIndex, pageSize, out totalRecords).Cast<MembershipUser>().ToList(); ;
        }

        [WebMethod]
        public string GeneratePassword()
        {
            return SqlMembershipProvider.GeneratePassword();
        }

        [WebMethod]
        public List<MembershipUser> GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            return SqlMembershipProvider.GetAllUsers(pageIndex, pageSize, out totalRecords).Cast<MembershipUser>().ToList(); ;
        }

        [WebMethod]
        public int GetNumberOfUsersOnline()
        {
            return SqlMembershipProvider.GetNumberOfUsersOnline();
        }

        [WebMethod]
        public string GetPassword(string username, string passwordAnswer)
        {
            return SqlMembershipProvider.GetPassword(username, passwordAnswer);
        }

        [WebMethod]
        public MembershipUser GetUser(string username, bool userIsOnline)
        {
            return SqlMembershipProvider.GetUser(username, userIsOnline);
        }

        //[WebMethod]
        public MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            return SqlMembershipProvider.GetUser(providerUserKey, userIsOnline);
        }

        [WebMethod]
        public string GetUserNameByEmail(string email)
        {
            return SqlMembershipProvider.GetUserNameByEmail(email);
        }

        [WebMethod]
        public string ResetPassword(string username, string passwordAnswer)
        {
            return SqlMembershipProvider.ResetPassword(username, passwordAnswer);
        }

        [WebMethod]
        public bool UnlockUser(string username)
        {
            return SqlMembershipProvider.UnlockUser(username);
        }

        [WebMethod]
        public void UpdateUser(MembershipUser user)
        {
            SqlMembershipProvider.UpdateUser(user);
        }

        [WebMethod]
        public bool ValidateUser(string username, string password)
        {
            return SqlMembershipProvider.ValidateUser(username, password);
        }
    }
}