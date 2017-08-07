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
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.SqlServer.Server;

#endregion

public partial class UserDefinedFunctions
{
    #region DisallowedType enum

    public enum DisallowedType
    {
        None = 0,
        AbsoluteUri = 1,
        Domain = 2,
        Extension = 3,
        FileExtension = 4,
        ResponseHeaders = 5,
        Scheme = 6,
        Source = 7
    }

    #endregion

    private static readonly object _isDisallowedLock = new object();

    private static string _connectionString;

    private static Dictionary<string, string> _disallowedAbsoluteUris;
    private static Dictionary<string, string> _disallowedDomains;
    private static Dictionary<string, string> _disallowedExtensions;
    private static Dictionary<string, string> _disallowedFileExtensions;
    private static Dictionary<string, string> _disallowedHosts;
    private static Dictionary<string, string> _disallowedSchemes;
    private static Dictionary<string, string> _disallowedWordsForAbsoluteUri;
    private static Dictionary<string, string> _disallowedWordsForResponseHeaders;
    private static Dictionary<string, string> _disallowedWordsForSource;

    /// <summary>
    /// 	Gets the connection string.
    /// </summary>
    /// <value>The connection string.</value>
    public static string ConnectionString
    {
        get
        {
            if (_connectionString == null)
            {
                if (AppDomain.CurrentDomain.FriendlyName.StartsWith("arachnode.net"))
                {
                    //being called from the database...
                    _connectionString = "Context Connection=true";
                }
                else
                {
                    _connectionString = ConfigurationManager.ConnectionStrings["arachnode_net_ConnectionString"].ConnectionString;
                }
            }
            return _connectionString;
        }
        set { _connectionString = value; }
    }

    /// <summary>
    /// 	Gets or sets the disallowed absolute uris.
    /// </summary>
    /// <value>The disallowed absolute uris.</value>
    public static Dictionary<string, string> DisallowedAbsoluteUris
    {
        get
        {
            if (_disallowedAbsoluteUris == null)
            {
                RefreshDisallowed();
            }
            return _disallowedAbsoluteUris;
        }
        set { _disallowedAbsoluteUris = value; }
    }

    /// <summary>
    /// 	Gets or sets the disallowed domains.
    /// </summary>
    /// <value>The disallowed domains.</value>
    public static Dictionary<string, string> DisallowedDomains
    {
        get
        {
            if (_disallowedDomains == null)
            {
                RefreshDisallowed();
            }
            return _disallowedDomains;
        }
        set { _disallowedDomains = value; }
    }

    /// <summary>
    /// 	Gets or sets the disallowed extensions.
    /// </summary>
    /// <value>The disallowed extensions.</value>
    public static Dictionary<string, string> DisallowedExtensions
    {
        get
        {
            if (_disallowedExtensions == null)
            {
                RefreshDisallowed();
            }
            return _disallowedExtensions;
        }
        set { _disallowedExtensions = value; }
    }

    /// <summary>
    /// 	Gets or sets the disallowed file extensions.
    /// </summary>
    /// <value>The disallowed file extensions.</value>
    public static Dictionary<string, string> DisallowedFileExtensions
    {
        get
        {
            if (_disallowedFileExtensions == null)
            {
                RefreshDisallowed();
            }
            return _disallowedFileExtensions;
        }
        set { _disallowedFileExtensions = value; }
    }

    /// <summary>
    /// 	Gets or sets the disallowed hosts.
    /// </summary>
    /// <value>The disallowed hosts.</value>
    public static Dictionary<string, string> DisallowedHosts
    {
        get
        {
            if (_disallowedHosts == null)
            {
                RefreshDisallowed();
            }
            return _disallowedHosts;
        }
        set { _disallowedHosts = value; }
    }

    /// <summary>
    /// 	Gets or sets the disallowed schemes.
    /// </summary>
    /// <value>The disallowed schemes.</value>
    public static Dictionary<string, string> DisallowedSchemes
    {
        get
        {
            if (_disallowedSchemes == null)
            {
                RefreshDisallowed();
            }
            return _disallowedSchemes;
        }
        set { _disallowedSchemes = value; }
    }

    /// <summary>
    /// 	Gets or sets the disallowed words for absolute URI.
    /// </summary>
    /// <value>The disallowed words for absolute URI.</value>
    public static Dictionary<string, string> DisallowedWordsForAbsoluteUri
    {
        get
        {
            if (_disallowedWordsForAbsoluteUri == null)
            {
                RefreshDisallowed();
            }
            return _disallowedWordsForAbsoluteUri;
        }
        set { _disallowedWordsForAbsoluteUri = value; }
    }

    /// <summary>
    /// 	Gets or sets the disallowed words for response headers.
    /// </summary>
    /// <value>The disallowed words for response headers.</value>
    public static Dictionary<string, string> DisallowedWordsForResponseHeaders
    {
        get
        {
            if (_disallowedWordsForResponseHeaders == null)
            {
                RefreshDisallowed();
            }
            return _disallowedWordsForResponseHeaders;
        }
        set { _disallowedWordsForResponseHeaders = value; }
    }

    /// <summary>
    /// 	Gets or sets the disallowed words for source.
    /// </summary>
    /// <value>The disallowed words for source.</value>
    public static Dictionary<string, string> DisallowedWordsForSource
    {
        get
        {
            if (_disallowedWordsForSource == null)
            {
                RefreshDisallowed();
            }
            return _disallowedWordsForSource;
        }
        set { _disallowedWordsForSource = value; }
    }

    /// <summary>
    /// 	Determines whether [is disallowed for absolute URI] [the specified absolute URI].
    /// </summary>
    /// <param name = "absoluteUri">The absolute URI.</param>
    /// <param name = "checkIsDisallowedForWords">The check is disallowed for words.</param>
    /// <param name = "refreshDisallowed">The refresh disallowed.</param>
    /// <returns>
    /// 	<c>true</c> if [is disallowed for absolute URI] [the specified absolute URI]; otherwise, <c>false</c>.
    /// </returns>
    [SqlFunction(DataAccess = DataAccessKind.Read)]
    public static bool IsDisallowedForAbsoluteUri(SqlString absoluteUri, SqlBoolean checkIsDisallowedForWords, SqlBoolean refreshDisallowed)
    {
        if (refreshDisallowed || _disallowedAbsoluteUris == null)
        {
            RefreshDisallowed();
        }

        if (!absoluteUri.IsNull && _disallowedAbsoluteUris.ContainsKey(absoluteUri.Value.ToLower()))
        {
            return true;
        }

        if (checkIsDisallowedForWords)
        {
            if (IsDisallowedForWords(absoluteUri, false))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 	Determines whether [is disallowed for domain] [the specified absolute URI].
    /// </summary>
    /// <param name = "absoluteUri">The absolute URI.</param>
    /// <param name = "checkIsDisallowedForWords">The check is disallowed for words.</param>
    /// <param name = "refreshDisallowed">The refresh disallowed.</param>
    /// <returns>
    /// 	<c>true</c> if [is disallowed for domain] [the specified absolute URI]; otherwise, <c>false</c>.
    /// </returns>
    [SqlFunction(DataAccess = DataAccessKind.Read)]
    public static bool IsDisallowedForDomain(SqlString absoluteUri, SqlBoolean checkIsDisallowedForWords, SqlBoolean refreshDisallowed)
    {
        if (refreshDisallowed || _disallowedDomains == null)
        {
            RefreshDisallowed();
        }

        string domain = ExtractDomain(absoluteUri).Value;

        if (domain != null && _disallowedDomains.ContainsKey(domain.ToLower()))
        {
            return true;
        }

        if (checkIsDisallowedForWords)
        {
            if (IsDisallowedForWords(domain, false))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 	Determines whether [is disallowed for extension] [the specified absolute URI].
    /// </summary>
    /// <param name = "absoluteUri">The absolute URI.</param>
    /// <param name = "checkIsDisallowedForWords">The check is disallowed for words.</param>
    /// <param name = "refreshDisallowed">The refresh disallowed.</param>
    /// <returns>
    /// 	<c>true</c> if [is disallowed for extension] [the specified absolute URI]; otherwise, <c>false</c>.
    /// </returns>
    [SqlFunction(DataAccess = DataAccessKind.Read)]
    public static bool IsDisallowedForExtension(SqlString absoluteUri, SqlBoolean checkIsDisallowedForWords, SqlBoolean refreshDisallowed)
    {
        if (refreshDisallowed || _disallowedExtensions == null)
        {
            RefreshDisallowed();
        }

        string extension = ExtractExtension(absoluteUri, false).Value;

        if (extension != null && _disallowedExtensions.ContainsKey(extension.ToLower()))
        {
            return true;
        }

        if (checkIsDisallowedForWords)
        {
            if (IsDisallowedForWords(extension, false))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 	Determines whether [is disallowed for file extension] [the specified absolute URI].
    /// </summary>
    /// <param name = "absoluteUri">The absolute URI.</param>
    /// <param name = "checkIsDisallowedForWords">The check is disallowed for words.</param>
    /// <param name = "refreshDisallowed">The refresh disallowed.</param>
    /// <returns>
    /// 	<c>true</c> if [is disallowed for file extension] [the specified absolute URI]; otherwise, <c>false</c>.
    /// </returns>
    [SqlFunction(DataAccess = DataAccessKind.Read)]
    public static bool IsDisallowedForFileExtension(SqlString absoluteUri, SqlBoolean checkIsDisallowedForWords, SqlBoolean refreshDisallowed)
    {
        if (refreshDisallowed || _disallowedFileExtensions == null)
        {
            RefreshDisallowed();
        }

        string fileExtension = ExtractFileExtension(absoluteUri).Value;

        if (fileExtension != null && _disallowedFileExtensions.ContainsKey(fileExtension.ToLower()))
        {
            return true;
        }

        if (checkIsDisallowedForWords)
        {
            if (IsDisallowedForWords(fileExtension, false))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 	Determines whether [is disallowed for host] [the specified absolute URI].
    /// </summary>
    /// <param name = "absoluteUri">The absolute URI.</param>
    /// <param name = "checkIsDisallowedForWords">The check is disallowed for words.</param>
    /// <param name = "refreshDisallowed">The refresh disallowed.</param>
    /// <returns>
    /// 	<c>true</c> if [is disallowed for host] [the specified absolute URI]; otherwise, <c>false</c>.
    /// </returns>
    [SqlFunction(DataAccess = DataAccessKind.Read)]
    public static bool IsDisallowedForHost(SqlString absoluteUri, SqlBoolean checkIsDisallowedForWords, SqlBoolean refreshDisallowed)
    {
        if (refreshDisallowed || _disallowedHosts == null)
        {
            RefreshDisallowed();
        }

        string host = ExtractHost(absoluteUri).Value;

        if (host != null && _disallowedHosts.ContainsKey(host.ToLower()))
        {
            return true;
        }

        if (checkIsDisallowedForWords)
        {
            if (IsDisallowedForWords(host, false))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 	Determines whether [is disallowed for response headers] [the specified response headers].
    /// </summary>
    /// <param name = "responseHeaders">The response headers.</param>
    /// <param name = "refreshDisallowed">The refresh disallowed.</param>
    /// <returns>
    /// 	<c>true</c> if [is disallowed for response headers] [the specified response headers]; otherwise, <c>false</c>.
    /// </returns>
    [SqlFunction(DataAccess = DataAccessKind.Read)]
    public static bool IsDisallowedForResponseHeaders(SqlString responseHeaders, SqlBoolean refreshDisallowed)
    {
        if (refreshDisallowed || _disallowedWordsForResponseHeaders == null)
        {
            RefreshDisallowed();
        }

        foreach (string disallowedWord in _disallowedWordsForResponseHeaders.Values)
        {
            if (Regex.IsMatch(responseHeaders.Value, disallowedWord, RegexOptions.IgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 	Determines whether [is disallowed for scheme] [the specified absolute URI].
    /// </summary>
    /// <param name = "absoluteUri">The absolute URI.</param>
    /// <param name = "checkIsDisallowedForWords">The check is disallowed for words.</param>
    /// <param name = "refreshDisallowed">The refresh disallowed.</param>
    /// <returns>
    /// 	<c>true</c> if [is disallowed for scheme] [the specified absolute URI]; otherwise, <c>false</c>.
    /// </returns>
    [SqlFunction(DataAccess = DataAccessKind.Read)]
    public static bool IsDisallowedForScheme(SqlString absoluteUri, SqlBoolean checkIsDisallowedForWords, SqlBoolean refreshDisallowed)
    {
        if (refreshDisallowed || _disallowedSchemes == null)
        {
            RefreshDisallowed();
        }

        string scheme = ExtractScheme(absoluteUri, false).Value;

        if (scheme != null && _disallowedHosts.ContainsKey(scheme.ToLower()))
        {
            return true;
        }

        if (checkIsDisallowedForWords)
        {
            if (IsDisallowedForWords(scheme, false))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 	Determines whether [is disallowed for source] [the specified source].
    /// </summary>
    /// <param name = "source">The source.</param>
    /// <param name = "refreshDisallowed">The refresh disallowed.</param>
    /// <returns>
    /// 	<c>true</c> if [is disallowed for source] [the specified source]; otherwise, <c>false</c>.
    /// </returns>
    [SqlFunction(DataAccess = DataAccessKind.Read)]
    public static bool IsDisallowedForSource(SqlString source, SqlBoolean refreshDisallowed)
    {
        if (refreshDisallowed || _disallowedWordsForSource == null)
        {
            RefreshDisallowed();
        }

        foreach (string disallowedWord in _disallowedWordsForSource.Values)
        {
            if (Regex.IsMatch(source.Value, disallowedWord, RegexOptions.IgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 	Determines whether [is disallowed for words] [the specified absolute URI].
    /// </summary>
    /// <param name = "absoluteUriOrAbsoluteUriClassification">The absolute URI.</param>
    /// <param name = "refreshDisallowed">The refresh disallowed.</param>
    /// <returns>
    /// 	<c>true</c> if [is disallowed for words] [the specified absolute URI]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsDisallowedForWords(SqlString absoluteUriOrAbsoluteUriClassification, SqlBoolean refreshDisallowed)
    {
        if (refreshDisallowed || _disallowedAbsoluteUris == null)
        {
            RefreshDisallowed();
        }

        if (absoluteUriOrAbsoluteUriClassification.Value != null)
        {
            foreach (string disallowedWord in _disallowedWordsForAbsoluteUri.Values)
            {
                if (Regex.IsMatch(absoluteUriOrAbsoluteUriClassification.Value, disallowedWord, RegexOptions.IgnoreCase))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 	Refreshes all Disallowed patterns.  Call this method before calling IsDisallowed to avoid a thread race condition.  This function pulls from DisallowedAbsoluteUris and cfg.Disallowed*.  DisallowedAbsoluteUris is populated by Crawl Rules.
    /// </summary>
    /// <returns></returns>
    [SqlFunction(DataAccess = DataAccessKind.Read)]
    public static SqlBoolean RefreshDisallowed()
    {
        lock (_isDisallowedLock)
        {
            if (_disallowedAbsoluteUris == null)
            {
                _disallowedAbsoluteUris = new Dictionary<string, string>();
            }
            else
            {
                _disallowedAbsoluteUris.Clear();
            }

            if (_disallowedDomains == null)
            {
                _disallowedDomains = new Dictionary<string, string>();
            }
            else
            {
                _disallowedDomains.Clear();
            }

            if (_disallowedExtensions == null)
            {
                _disallowedExtensions = new Dictionary<string, string>();
            }
            else
            {
                _disallowedExtensions.Clear();
            }

            if (_disallowedFileExtensions == null)
            {
                _disallowedFileExtensions = new Dictionary<string, string>();
            }
            else
            {
                _disallowedExtensions.Clear();
            }

            if (_disallowedHosts == null)
            {
                _disallowedHosts = new Dictionary<string, string>();
            }
            else
            {
                _disallowedHosts.Clear();
            }

            if (_disallowedSchemes == null)
            {
                _disallowedSchemes = new Dictionary<string, string>();
            }
            else
            {
                _disallowedSchemes.Clear();
            }

            if (_disallowedWordsForAbsoluteUri == null)
            {
                _disallowedWordsForAbsoluteUri = new Dictionary<string, string>();
            }
            else
            {
                _disallowedWordsForAbsoluteUri.Clear();
            }

            if (_disallowedWordsForResponseHeaders == null)
            {
                _disallowedWordsForResponseHeaders = new Dictionary<string, string>();
            }
            else
            {
                _disallowedWordsForResponseHeaders.Clear();
            }

            if (_disallowedWordsForSource == null)
            {
                _disallowedWordsForSource = new Dictionary<string, string>();
            }
            else
            {
                _disallowedWordsForSource.Clear();
            }
            
            SqlCommand sqlCommand = null;

            try
            {
                SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(ConnectionString);

                //short connect timeout...
                sqlConnectionStringBuilder.ConnectTimeout = 15;

                sqlCommand = new SqlCommand("", new SqlConnection(sqlConnectionStringBuilder.ConnectionString));

                sqlCommand.Connection.Open();
            }
            catch
            {

            }

            try
            {
                //Select the Configuration to set the SqlCommandTimeout for the UDF's.
                sqlCommand.CommandText = "dbo.arachnode_omsp_Configuration_SELECT";
                sqlCommand.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    while (sqlDataReader.Read())
                    {
                        if (sqlDataReader.GetString(1) == "SqlCommandTimeoutInMinutes")
                        {
                            sqlCommand.CommandTimeout = int.Parse(sqlDataReader.GetString(2))*60;
                        }
                    }
                }
            }
            catch
            {
                try
                {
                    XDocument xDocument = XDocument.Load("cfg.Configuration.xml");

                    foreach (XElement xElement in xDocument.Descendants("Configuration"))
                    {
                        string key = xElement.Descendants("Key").FirstOrDefault().Value;
                        string value = xElement.Descendants("Value").FirstOrDefault().Value;
                        
                        if (key == "SqlCommandTimeoutInMinutes")
                        {
                            //just to make sure we can parse it, sanity, code consistency...  :)
                            int commandTimeout = int.Parse(value) * 60;
                        }
                    }
                }
                catch (Exception exception2)
                {
                    throw new Exception("Unable to refresh the Configuration (cfg.Configuration.xml).", exception2);
                }
            }

            try
            {
                //Select all DisallowedAbsoluteUris that were disallowed due to WebExceptions or CrawlRules.
                sqlCommand.CommandText = "dbo.arachnode_omsp_DisallowedAbsoluteUris_SELECT";
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@AbsoluteUri", DBNull.Value));
                sqlCommand.Parameters.Add(new SqlParameter("@NumberOfDisallowedAbsoluteUrisToReturn", int.MaxValue));
                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    while (sqlDataReader.Read())
                    {
                        if (!_disallowedAbsoluteUris.ContainsKey(sqlDataReader.GetString(3).ToLower()))
                        {
                            _disallowedAbsoluteUris.Add(sqlDataReader.GetString(3).ToLower(), null);
                        }
                    }
                }
                sqlCommand.Parameters.Clear();
            }
            catch
            {
                try
                {
                    XDocument xDocument = XDocument.Load("dbo.DisallowedAbsoluteUris.xml");

                    foreach (XElement xElement in xDocument.Descendants("AbsoluteUri"))
                    {
                        if (!_disallowedAbsoluteUris.ContainsKey(xElement.Value))
                        {
                            _disallowedAbsoluteUris.Add(xElement.Value, null);
                        }
                    }
                }
                catch (Exception exception2)
                {
                    throw new Exception("Unable to refresh disallowed AbsoluteUris (dbo.DisallowedAbsoluteUris.xml).", exception2);
                }
            }

            try
            {
                sqlCommand.CommandText = "dbo.arachnode_omsp_DisallowedDomains_SELECT";
                sqlCommand.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    while (sqlDataReader.Read())
                    {
                        if (!_disallowedDomains.ContainsKey(sqlDataReader.GetString(0).ToLower()))
                        {
                            _disallowedDomains.Add(sqlDataReader.GetString(0).ToLower(), null);
                        }
                    }
                }
            }
            catch
            {
                try
                {
                    XDocument xDocument = XDocument.Load("cfg.DisallowedDomains.xml");

                    foreach (XElement xElement in xDocument.Descendants("Domain"))
                    {
                        if (!_disallowedDomains.ContainsKey(xElement.Value))
                        {
                            _disallowedDomains.Add(xElement.Value, null);
                        }
                    }
                }
                catch (Exception exception2)
                {
                    throw new Exception("Unable to refresh disallowed Domains (cfg.DisallowedDomains.xml).", exception2);
                }
            }

            try
            {
                sqlCommand.CommandText = "dbo.arachnode_omsp_DisallowedExtensions_SELECT";
                sqlCommand.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    while (sqlDataReader.Read())
                    {
                        if (!_disallowedExtensions.ContainsKey(sqlDataReader.GetString(0).ToLower()))
                        {
                            _disallowedExtensions.Add(sqlDataReader.GetString(0).ToLower(), null);
                        }
                    }
                }
            }
            catch
            {
                try
                {
                    XDocument xDocument = XDocument.Load("cfg.DisallowedExtensions.xml");

                    foreach (XElement xElement in xDocument.Descendants("Extensions"))
                    {
                        if (!_disallowedExtensions.ContainsKey(xElement.Value))
                        {
                            _disallowedExtensions.Add(xElement.Value, null);
                        }
                    }
                }
                catch (Exception exception2)
                {
                    throw new Exception("Unable to refresh disallowed Extensions (cfg.DisallowedExtensions.xml).", exception2);
                }
            }

            try
            {
                sqlCommand.CommandText = "dbo.arachnode_omsp_DisallowedFileExtensions_SELECT";
                sqlCommand.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    while (sqlDataReader.Read())
                    {
                        if (!_disallowedFileExtensions.ContainsKey(sqlDataReader.GetString(0).ToLower()))
                        {
                            _disallowedFileExtensions.Add(sqlDataReader.GetString(0).ToLower(), null);
                        }
                    }
                }
            }
            catch
            {
                try
                {
                    XDocument xDocument = XDocument.Load("cfg.DisallowedFileExtensions.xml");

                    foreach (XElement xElement in xDocument.Descendants("FileExtension"))
                    {
                        if (!_disallowedFileExtensions.ContainsKey(xElement.Value))
                        {
                            _disallowedFileExtensions.Add(xElement.Value, null);
                        }
                    }
                }
                catch (Exception exception2)
                {
                    throw new Exception("Unable to refresh disallowed FileExtensions (cfg.FileExtensions.xml).", exception2);
                }
            }

            try
            {
                sqlCommand.CommandText = "dbo.arachnode_omsp_DisallowedHosts_SELECT";
                sqlCommand.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    while (sqlDataReader.Read())
                    {
                        if (!_disallowedHosts.ContainsKey(sqlDataReader.GetString(0).ToLower()))
                        {
                            _disallowedHosts.Add(sqlDataReader.GetString(0), null);
                        }
                    }
                }
            }
            catch
            {
                try
                {
                    XDocument xDocument = XDocument.Load("cfg.DisallowedHosts.xml");

                    foreach (XElement xElement in xDocument.Descendants("Host"))
                    {
                        if (!_disallowedHosts.ContainsKey(xElement.Value))
                        {
                            _disallowedHosts.Add(xElement.Value, null);
                        }
                    }
                }
                catch (Exception exception2)
                {
                    throw new Exception("Unable to refresh disallowed Hosts (cfg.DisallowedHosts.xml).", exception2);
                }
            }

            try
            {
                sqlCommand.CommandText = "dbo.arachnode_omsp_DisallowedSchemes_SELECT";
                sqlCommand.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    while (sqlDataReader.Read())
                    {
                        if (!_disallowedSchemes.ContainsKey(sqlDataReader.GetString(0).ToLower()))
                        {
                            _disallowedSchemes.Add(sqlDataReader.GetString(0), null);
                        }
                    }
                }
            }
            catch
            {
                try
                {
                    XDocument xDocument = XDocument.Load("cfg.DisallowedSchemes.xml");

                    foreach (XElement xElement in xDocument.Descendants("Schemes"))
                    {
                        if (!_disallowedSchemes.ContainsKey(xElement.Value))
                        {
                            _disallowedSchemes.Add(xElement.Value, null);
                        }
                    }
                }
                catch (Exception exception2)
                {
                    throw new Exception("Unable to refresh disallowed Schemes (cfg.DisallowedSchemes.xml).", exception2);
                }
            }

            try
            {
                sqlCommand.CommandText = "dbo.arachnode_omsp_DisallowedWords_SELECT";
                sqlCommand.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    while (sqlDataReader.Read())
                    {
                        string word = sqlDataReader.GetString(0).ToLower();

                        if (sqlDataReader.GetBoolean(1))
                        {
                            if (!_disallowedWordsForAbsoluteUri.ContainsKey(word))
                            {
                                _disallowedWordsForAbsoluteUri.Add(word.Replace(" ", "%20"), word.Replace(" ", "%20"));
                            }
                        }

                        if (sqlDataReader.GetBoolean(2))
                        {
                            if (!_disallowedWordsForResponseHeaders.ContainsKey(word))
                            {
                                _disallowedWordsForResponseHeaders.Add(word, word);
                            }
                        }

                        if (sqlDataReader.GetBoolean(3))
                        {
                            if (!_disallowedWordsForSource.ContainsKey(word))
                            {
                                _disallowedWordsForSource.Add(word, word);
                            }
                        }
                    }
                }
            }
            catch
            {
                try
                {
                    XDocument xDocument = XDocument.Load("cfg.DisallowedWords.xml");

                    foreach (XElement xElement in xDocument.Descendants("DisallowedWords"))
                    {
                        string word = xElement.Descendants("Word").FirstOrDefault().Value;
                        bool isDisallowedForAbsoluteUri = bool.Parse(xElement.Descendants("IsDisallowedForAbsoluteUri").FirstOrDefault().Value);
                        bool isDisallowedForResponseHeaders = bool.Parse(xElement.Descendants("IsDisallowedForResponseHeaders").FirstOrDefault().Value);
                        bool isDisallowedForSource = bool.Parse(xElement.Descendants("IsDisallowedForSource").FirstOrDefault().Value);

                        if (isDisallowedForAbsoluteUri)
                        {
                            if (!_disallowedWordsForAbsoluteUri.ContainsKey(word))
                            {
                                _disallowedWordsForAbsoluteUri.Add(word.Replace(" ", "%20"), word.Replace(" ", "%20"));
                            }
                        }

                        if (isDisallowedForResponseHeaders)
                        {
                            if (!_disallowedWordsForResponseHeaders.ContainsKey(word))
                            {
                                _disallowedWordsForResponseHeaders.Add(word, word);
                            }
                        }

                        if (isDisallowedForSource)
                        {
                            if (!_disallowedWordsForSource.ContainsKey(word))
                            {
                                _disallowedWordsForSource.Add(word, word);
                            }
                        }
                    }
                }
                catch (Exception exception2)
                {
                    throw new Exception("Unable to refresh disallowed Schemes.", exception2);
                }
            }

            try
            {
                sqlCommand.Connection.Close();
            }
            catch
            {
            }
        }

        return true;
    }
} ;