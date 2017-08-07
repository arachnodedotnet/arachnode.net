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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Xml.Linq;
using Microsoft.SqlServer.Server;

#endregion

public partial class UserDefinedFunctions
{
    private static readonly object _allowedSchemesLock = new object();
    private static Dictionary<string, string> _allowedSchemes;

    /// <summary>
    /// 	Gets or sets the allowed schemes.
    /// </summary>
    /// <value>The allowed schemes.</value>
    public static Dictionary<string, string> AllowedSchemes
    {
        get
        {
            if (_allowedSchemes == null)
            {
                RefreshAllowedSchemes(true);
            }
            return _allowedSchemes;
        }
        set { _allowedSchemes = value; }
    }

    /// <summary>
    /// 	Extracts the Scheme from an AbsoluteUri.  http://subdomain.arachnode.net/ -&gt; http
    /// </summary>
    /// <param name = "absoluteUri">The absolute URI.</param>
    /// <param name = "refreshAllowedSchemes">The refresh allowed schemes.</param>
    /// <returns></returns>
    [SqlFunction(DataAccess = DataAccessKind.Read)]
    public static SqlString ExtractScheme(SqlString absoluteUri, SqlBoolean refreshAllowedSchemes)
    {
        if (refreshAllowedSchemes || _allowedSchemes == null)
        {
            RefreshAllowedSchemes(refreshAllowedSchemes);
        }

        if (!absoluteUri.IsNull)
        {
            Uri scheme;

            if (Uri.TryCreate(absoluteUri.Value, UriKind.Absolute, out scheme))
            {
                if (scheme.Scheme.Length == 0)
                {
                    return "UNKNOWN";
                }
                else
                {
                    return new SqlString(scheme.Scheme);
                }
            }
            else
            {
                return "UNKNOWN";
            }
        }
        else
        {
            return "UNKNOWN";
        }
    }

    /// <summary>
    /// 	Refresh all allowed Schemes.  This function pulls from cfg.AllowedSchemes.  This table should never need to be changed, unless new extensions are added.  e.g. httpr = 'new protocol'.
    /// </summary>
    /// <param name = "refreshAllowedSchemes">The refresh allowed schemes.</param>
    /// <returns></returns>
    [SqlFunction(DataAccess = DataAccessKind.Read)]
    public static SqlBoolean RefreshAllowedSchemes(SqlBoolean refreshAllowedSchemes)
    {
        lock (_allowedSchemesLock)
        {
            if (_allowedSchemes == null)
            {
                _allowedSchemes = new Dictionary<string, string>();
            }
            else
            {
                if (!refreshAllowedSchemes)
                {
                    return refreshAllowedSchemes;
                }

                _allowedSchemes.Clear();
            }

            try
            {
                SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(ConnectionString);

                //short connect timeout...
                sqlConnectionStringBuilder.ConnectTimeout = 15;

                SqlCommand sqlCommand = new SqlCommand("", new SqlConnection(sqlConnectionStringBuilder.ConnectionString));

                sqlCommand.Connection.Open();

                sqlCommand.CommandText = "dbo.arachnode_omsp_AllowedSchemes_SELECT";
                sqlCommand.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    while (sqlDataReader.Read())
                    {
                        if (!_allowedSchemes.ContainsKey(sqlDataReader.GetString(0)))
                        {
                            _allowedSchemes.Add(sqlDataReader.GetString(0), sqlDataReader.GetString(0));
                        }
                    }
                }

                sqlCommand.Connection.Close();
            }
            catch
            {
                try
                {
                    XDocument xDocument = XDocument.Load("cfg.AllowedSchemes.xml");

                    foreach (XElement xElement in xDocument.Descendants("Scheme"))
                    {
                        if (!_allowedSchemes.ContainsKey(xElement.Value))
                        {
                            _allowedSchemes.Add(xElement.Value, xElement.Value);
                        }
                    }
                }
                catch (Exception exception2)
                {
                    throw new Exception("Unable to refresh allowed Schemes.", exception2);
                }
            }
        }

        return refreshAllowedSchemes;
    }
} ;