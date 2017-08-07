using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Web.Profile;
using System.Web.Services;
using Arachnode.Web.Value;

namespace Arachnode.Web.Services
{
    /// <summary>
    /// Summary description for ProfileService
    /// </summary>
    [WebService(Namespace = "http://arachnode.net/ProfileService")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
        // [System.Web.Script.Services.ScriptService]
    public class ProfileService : WebService
    {
        private SqlProfileProvider _sqlProfileProvider = new SqlProfileProvider();

        public ProfileService()
        {
            //HACK: Hardcoded values!!!
            NameValueCollection config = new NameValueCollection();
            config.Add("connectionStringName", "arachnode_net_ConnectionString");
            config.Add("applicationName", "arachnode.net");

            SqlProfileProvider.Initialize("SqlProfileProvider", config);
        }

        public SqlProfileProvider SqlProfileProvider
        {
            get { return _sqlProfileProvider; }
            set { _sqlProfileProvider = value; }
        }

        [WebMethod]
        public int DeleteInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            return SqlProfileProvider.DeleteInactiveProfiles(authenticationOption, userInactiveSinceDate);
        }

        [WebMethod]
        public int DeleteProfiles(string[] usernames)
        {
            return SqlProfileProvider.DeleteProfiles(usernames);
        }

        //[WebMethod]
        public int DeleteProfiles(ProfileInfoCollection profiles)
        {
            return SqlProfileProvider.DeleteProfiles(profiles);
        }

        [WebMethod]
        public List<ProfileInfo> FindInactiveProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            return SqlProfileProvider.FindInactiveProfilesByUserName(authenticationOption, usernameToMatch, userInactiveSinceDate, pageIndex, pageSize, out totalRecords).Cast<ProfileInfo>().ToList();
        }

        [WebMethod]
        public List<ProfileInfo> FindProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            return SqlProfileProvider.FindProfilesByUserName(authenticationOption, usernameToMatch, pageIndex, pageSize, out totalRecords).Cast<ProfileInfo>().ToList();
        }

        [WebMethod]
        public List<ProfileInfo> GetAllInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            return SqlProfileProvider.GetAllInactiveProfiles(authenticationOption, userInactiveSinceDate, pageIndex, pageSize, out totalRecords).Cast<ProfileInfo>().ToList();
        }

        [WebMethod]
        public List<ProfileInfo> GetAllProfiles(ProfileAuthenticationOption authenticationOption, int pageIndex, int pageSize, out int totalRecords)
        {
            return SqlProfileProvider.GetAllProfiles(authenticationOption, pageIndex, pageSize, out totalRecords).Cast<ProfileInfo>().ToList();
        }

        [WebMethod]
        public int GetNumberOfInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            return SqlProfileProvider.GetNumberOfInactiveProfiles(authenticationOption, userInactiveSinceDate);
        }

        /// <summary>
        /// SettingsContext is not supported for serialization as it implemented IDictionary.
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        [WebMethod]
        public List<SettingsPropertyValueSerializable> GetPropertyValues(List<object> keys, List<object> values, List<SettingsPropertyValueSerializable> properties)
        {
            SettingsContext sc = new SettingsContext();

            for (int i = 0; i < keys.Count; i++)
            {
                sc.Add(keys[i], values[i]);
            }

            SettingsPropertyCollection properties2 = new SettingsPropertyCollection();

            foreach (SettingsPropertyValue settingsPropertyValue in properties)
            {
                properties2.Add(new SettingsProperty(settingsPropertyValue.Name, settingsPropertyValue.Property.PropertyType, settingsPropertyValue.Property.Provider, settingsPropertyValue.Property.IsReadOnly, settingsPropertyValue.Property.DefaultValue, settingsPropertyValue.Property.SerializeAs, settingsPropertyValue.Property.Attributes, settingsPropertyValue.Property.ThrowOnErrorDeserializing, settingsPropertyValue.Property.ThrowOnErrorSerializing));
            }

            return SqlProfileProvider.GetPropertyValues(sc, properties2).Cast<SettingsPropertyValueSerializable>().ToList(); ;
        }

        /// <summary>
        /// SettingsContext is not supported for serialization as it implemented IDictionary.
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        [WebMethod]
        public void SetPropertyValues(List<object> keys, List<object> values, List<SettingsPropertyValueSerializable> properties)
        {
            SettingsContext sc = new SettingsContext();

            for (int i = 0; i < keys.Count; i++)
            {
                sc.Add(keys[i], values[i]);
            }

            SettingsPropertyValueCollection properties2 = new SettingsPropertyValueCollection();

            foreach (SettingsPropertyValue settingsPropertyValue in properties)
            {
                properties2.Add(settingsPropertyValue);
            }
            
            SqlProfileProvider.SetPropertyValues(sc, properties2);
        }
    }
}