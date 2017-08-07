#region License : arachnode.net

// // Copyright (c) 2015 http://arachnode.net, arachnode.net, LLC
// //  
// // Permission is hereby granted, upon purchase, to any person
// // obtaining a copy of this software and associated documentation
// // files (the "Software"), to deal in the Software without
// // restriction, including without limitation the rights to use,
// // copy, merge and modify copies of the Software, and to permit persons
// // to whom the Software is furnished to do so, subject to the following
// // conditions:
// // 
// // LICENSE (ALL VERSIONS/EDITIONS): http://arachnode.net/r.ashx?3
// // 
// // The above copyright notice and this permission notice shall be
// // included in all copies or substantial portions of the Software.
// // 
// // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// // EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// // OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// // NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// // HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// // WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// // FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// // OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using System;
using System.Reflection;
using Arachnode.Configuration;
using Arachnode.Configuration.Value.Enums;
using Arachnode.DataAccess.Value.Enums;
using Arachnode.DataAccess.Value.Exceptions;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.DataSource;
using System.Linq;

#endregion

namespace Arachnode.DataAccess.Managers
{
    public static class ConfigurationManager
    {
        /// <summary>
        /// 	Initializes the configuration.
        /// </summary>
        /// <param name="applicationSettings"></param>
        /// <param name="webSettings"></param>
        /// <param name = "configurationType">Type of the configuration.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        public static void InitializeConfiguration(ref ApplicationSettings applicationSettings, ref WebSettings webSettings, ConfigurationType configurationType, IArachnodeDAO arachnodeDAO)
        {
            switch (configurationType)
            {
                case ConfigurationType.Application:
                    if (applicationSettings == null)
                    {
                        applicationSettings = new ApplicationSettings();
                    }
                    break;
                case ConfigurationType.Web:
                    if (webSettings == null)
                    {
                        webSettings = new WebSettings();
                    }
                    break;
            }

            foreach (ArachnodeDataSet.ConfigurationRow configurationRow in arachnodeDAO.GetConfiguration())
            {
                if (configurationRow.ConfigurationTypeID == (byte)configurationType)
                {
                    PropertyInfo propertyInfo = null;

                    switch (configurationType)
                    {
                        case ConfigurationType.Application:
                            propertyInfo = applicationSettings.GetType().GetProperty(configurationRow.Key);

                            if (propertyInfo != null)
                            {
                                if (propertyInfo.PropertyType.FullName == "System.Boolean")
                                {
                                    propertyInfo.SetValue(applicationSettings, bool.Parse(configurationRow.Value.ToString()), null);
                                }
                                else if (propertyInfo.PropertyType.FullName == "System.Double")
                                {
                                    propertyInfo.SetValue(applicationSettings, double.Parse(configurationRow.Value.ToString()), null);
                                }
                                else if (propertyInfo.PropertyType.FullName == "System.Int32")
                                {
                                    propertyInfo.SetValue(applicationSettings, int.Parse(configurationRow.Value.ToString()), null);
                                }
                                else if (propertyInfo.PropertyType.FullName == "System.String")
                                {
                                    propertyInfo.SetValue(applicationSettings, configurationRow.Value, null);
                                }
                            }
                            break;
                        case ConfigurationType.Web:
                                propertyInfo = webSettings.GetType().GetProperty(configurationRow.Key);

                                if (propertyInfo != null)
                                {
                                    if (propertyInfo.PropertyType.FullName == "System.Boolean")
                                    {
                                        propertyInfo.SetValue(webSettings, bool.Parse(configurationRow.Value.ToString()), null);
                                    }
                                    else if (propertyInfo.PropertyType.FullName == "System.Double")
                                    {
                                        propertyInfo.SetValue(webSettings, double.Parse(configurationRow.Value.ToString()), null);
                                    }
                                    else if (propertyInfo.PropertyType.FullName == "System.Int32")
                                    {
                                        propertyInfo.SetValue(webSettings, int.Parse(configurationRow.Value.ToString()), null);
                                    }
                                    else if (propertyInfo.PropertyType.FullName == "System.String")
                                    {
                                        propertyInfo.SetValue(webSettings, configurationRow.Value, null);
                                    }
                                }
                            break;
                    }
                }
            }

            switch (configurationType)
            {
                case ConfigurationType.Application:
                    CheckForMissingConfigurationValues(applicationSettings, null);
                    break;
                case ConfigurationType.Web:
                    CheckForMissingConfigurationValues(null, webSettings);
                    break;
            }
        }

        private static void CheckForMissingConfigurationValues(ApplicationSettings applicationSettings, WebSettings webSettings)
        {
            string missingPropertyValues = string.Empty;

            if (applicationSettings != null)
            {
                //it's OK for the UniqueIdentifier to NOT have a value...
                foreach (PropertyInfo propertyInfo in applicationSettings.GetType().GetProperties().Where(p => p.Name != "UniqueIdentifier"))
                {
                    object propertyValue = propertyInfo.GetValue(applicationSettings, null);

                    if (propertyValue == null || string.IsNullOrEmpty(propertyValue.ToString()))
                    {
                        missingPropertyValues += "\tApplication: " + propertyInfo.Name + "\n";
                    }
                }
            }

            if (webSettings != null)
            {
                foreach (PropertyInfo propertyInfo in webSettings.GetType().GetProperties())
                {
                    object propertyValue = propertyInfo.GetValue(webSettings, null);

                    if (propertyValue == null || string.IsNullOrEmpty(propertyValue.ToString()))
                    {
                        missingPropertyValues += "\tWeb: " + propertyInfo.Name + "\n";
                    }
                }
            }

            if (missingPropertyValues != string.Empty)
            {
                throw new InvalidConfigurationException(applicationSettings, webSettings, "The following configuration settings are missing from database table 'cfg.Configuration'\n" + missingPropertyValues, InvalidConfigurationExceptionSeverity.Error);
            }

            //you may encounter an unhandled exception here when running the demo.
            //this UE is not present in the 'Licensed' version.
            //this is due to the obfuscation protection placed on Arachnode.SiteCrawler.dll.
            //press 'f5' and view the instructions in the console window.
        }

        public static void CheckForIncorrectConfigurationValues(ApplicationSettings applicationSettings)
        {
            if (applicationSettings.AssignEmailAddressDiscoveries && !applicationSettings.InsertEmailAddresses)
            {
                throw new InvalidConfigurationException(null, null, "ApplicationSettings.AssignEmailAddressDiscoveries is set to 'true', but ApplicationSettings.InsertEmailAddresses is set to 'false'.  No EmailAddresses will be submitted to the database.", InvalidConfigurationExceptionSeverity.Warning);
            }
        }
    }
}