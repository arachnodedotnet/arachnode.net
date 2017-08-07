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
using System.IO;
using System.Threading;
using System.Xml.Linq;
using Microsoft.SqlServer.Server;

#endregion

public partial class UserDefinedFunctions
{
    private static readonly object _allowedExtensionsLock = new object();
    private static Dictionary<string, string> _allowedExtensions;

    /// <summary>
    /// 	Gets or sets the allowed extensions.
    /// </summary>
    /// <value>The allowed extensions.</value>
    public static Dictionary<string, string> AllowedExtensions
    {
        get
        {
            if (_allowedExtensions == null)
            {
                RefreshAllowedExtensions(true);
            }
            return _allowedExtensions;
        }
        set { _allowedExtensions = value; }
    }

    /// <summary>
    /// 	Extracts the extension from an AbsoluteUri.  e.g. http://arachnode.net -&gt; .net
    /// </summary>
    /// <param name = "absoluteUri">The absolute URI.</param>
    /// <param name = "refreshAllowedExtensions">The refresh allowed extensions.</param>
    /// <returns></returns>
    [SqlFunction(DataAccess = DataAccessKind.Read)]
    public static SqlString ExtractExtension(SqlString absoluteUri, SqlBoolean refreshAllowedExtensions)
    {
        if (refreshAllowedExtensions || _allowedExtensions == null)
        {
            RefreshAllowedExtensions(refreshAllowedExtensions);
        }

        if (!ExtractIPAddress(absoluteUri).IsNull)
        {
            return "UNKNOWN";
        }
        else
        {
            SqlString host = ExtractHost(absoluteUri);

            if (host.IsNull)
            {
                return "UNKNOWN";
            }
            else
            {
                string[] split = host.Value.Split('.');

                if (split.Length < 2)
                {
                    return "UNKNOWN";
                }
                else if (split.Length == 2)
                {
                    return new SqlString(split[split.Length - 1]);
                }
                else
                {
                    if (AllowedExtensions.ContainsKey(split[split.Length - 2] + '.' + split[split.Length - 1]))
                    {
                        return new SqlString(split[split.Length - 2] + '.' + split[split.Length - 1]);
                    }
                    else
                    {
                        return new SqlString(split[split.Length - 1]);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 	Refresh all allowed Extensions.  This function pulls from cfg.AllowedExtensions.  This table should never need to be changed, unless new extensions are added.  e.g. nco = 'new country'.
    ///     This method will failover to cfg.AllowedExtensions.xml if SQL Server is unavailable.  The SQL Server connection timeout is 15 seconds.
    /// </summary>
    /// <param name = "refreshAllowedExtensions">The refresh allowed extensions.</param>
    /// <returns></returns>
    [SqlFunction(DataAccess = DataAccessKind.Read)]
    public static SqlBoolean RefreshAllowedExtensions(SqlBoolean refreshAllowedExtensions)
    {
        lock (_allowedExtensionsLock)
        {
            if (_allowedExtensions == null)
            {
                _allowedExtensions = new Dictionary<string, string>();
            }
            else
            {
                if (!refreshAllowedExtensions)
                {
                    return refreshAllowedExtensions;
                }

                _allowedExtensions.Clear();
            }

            try
            {
                SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(ConnectionString);

                //short connect timeout...
                sqlConnectionStringBuilder.ConnectTimeout = 15;
                
                SqlCommand sqlCommand = new SqlCommand("", new SqlConnection(sqlConnectionStringBuilder.ConnectionString));
                
                sqlCommand.Connection.Open();

                sqlCommand.CommandText = "dbo.arachnode_omsp_AllowedExtensions_SELECT";
                sqlCommand.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    while (sqlDataReader.Read())
                    {
                        if (!_allowedExtensions.ContainsKey(sqlDataReader.GetString(0)))
                        {
                            _allowedExtensions.Add(sqlDataReader.GetString(0), sqlDataReader.GetString(0));
                        }
                    }
                }

                sqlCommand.Connection.Close();
            }
            catch
            {
                try
                {
                    XDocument xDocument = XDocument.Load("cfg.AllowedExtensions.xml");

                    foreach(XElement xElement in xDocument.Descendants("Extension"))
                    {
                        if (!_allowedExtensions.ContainsKey(xElement.Value))
                        {
                            _allowedExtensions.Add(xElement.Value, xElement.Value);
                        }
                    }
                }
                catch (Exception exception2)
                {
                    throw new Exception("Unable to refresh allowed Extensions.", exception2);
                }
            }
        }

        return refreshAllowedExtensions;
    }
} ;