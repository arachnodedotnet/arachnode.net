#region License : arachnode.net

// Copyright (c) 2015 http://arachnode.net, arachnode.net, LLC
//  
// Permission is hereby granted, upon purchase, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, merge and modify copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following
// conditions:
// 
// LICENSE (ALL VERSIONS/EDITIONS): http://arachnode.net/r.ashx?3
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using System;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using Arachnode.Proxy.Authentication;
using Arachnode.Proxy.Value.AbstractClasses;

#endregion

namespace Arachnode.Proxy
{
    /// <summary>
    /// Stores the configuration settings of this proxy server.
    /// </summary>
    public sealed class ProxyConfig
    {
        /// <summary>Holds the value of the File property.</summary>
        private readonly string m_File;

        /// <summary>Holds the value of the Parent property.</summary>
        private readonly Proxy m_Parent;

        /// <summary>Holds the value of the Settings property.</summary>
        private readonly StringDictionary m_Settings = new StringDictionary();

        /// <summary>Holds the value of the UserList property.</summary>
        private readonly AuthenticationList m_UserList = new AuthenticationList();

        /// <summary>
        /// Initializes a new ProxyConfig instance.
        /// </summary>
        /// <param name="parent">The parent of this ProxyCondif instance.</param>
        /// <param name="file">The XML file to read data from and store data to.</param>
        /// <exception cref="ArgumentNullException"><c>file</c> is null -or- <c>parent</c> is null.</exception>
        public ProxyConfig(Proxy parent, string file)
        {
            if (file == null || parent == null)
            {
                throw new ArgumentNullException();
            }
            m_File = file;
            m_Parent = parent;
        }

        /// <summary>
        /// Gets the full path to the XML data file.
        /// </summary>
        /// <value>A String that holds the full path to the XML data file.</value>
        public string File
        {
            get { return m_File; }
        }

        /// <summary>
        /// Gets the parent object of this ProxyConfig class.
        /// </summary>
        /// <value>An instance of the Proxy class.</value>
        public Proxy Parent
        {
            get { return m_Parent; }
        }

        /// <summary>
        /// Gets the dictionary that holds the settings.
        /// </summary>
        /// <value>An instance of the StringDictionary class that holds the settings.</value>
        private StringDictionary Settings
        {
            get { return m_Settings; }
        }

        /// <summary>
        /// Gets the userlist.
        /// </summary>
        /// <value>An instance of the AuthenticationList class that holds all the users and their corresponding password hashes.</value>
        internal AuthenticationList UserList
        {
            get { return m_UserList; }
        }

        /// <summary>
        /// Reads a string from the settings section.
        /// </summary>
        /// <param name="name">The key to read from.</param>
        /// <returns>The string value that corresponds with the specified key, or an empty string if the specified key was not found in the collection.</returns>
        public string ReadString(string name)
        {
            return ReadString(name, "");
        }

        /// <summary>
        /// Reads a string from the settings section.
        /// </summary>
        /// <param name="name">The key to read from.</param>
        /// <param name="def">The default string to return.</param>
        /// <returns>The string value that corresponds with the specified key, or <c>def</c> if the specified key was not found in the collection.</returns>
        public string ReadString(string name, string def)
        {
            if (name == null)
            {
                return def;
            }
            if (!Settings.ContainsKey(name))
            {
                return def;
            }
            return Settings[name];
        }

        /// <summary>
        /// Reads a byte array from the settings section.
        /// </summary>
        /// <param name="name">The key to read from.</param>
        /// <returns>The array of bytes that corresponds with the specified key, or <c>null</c> if the specified key was not found in the collection.</returns>
        public byte[] ReadBytes(string name)
        {
            string ret = ReadString(name, null);
            if (ret == null)
            {
                return null;
            }
            return Convert.FromBase64String(ret);
        }

        /// <summary>
        /// Reads an integer from the settings section.
        /// </summary>
        /// <param name="name">The key to read from.</param>
        /// <returns>The integer that corresponds with the specified key, or 0 if the specified key was not found in the collection.</returns>
        public int ReadInt(string name)
        {
            return ReadInt(name, 0);
        }

        /// <summary>
        /// Reads an integer from the settings section.
        /// </summary>
        /// <param name="name">The key to read from.</param>
        /// <param name="def">The default integer to return.</param>
        /// <returns>The integer that corresponds with the specified key, or <c>def</c> if the specified key was not found in the collection.</returns>
        public int ReadInt(string name, int def)
        {
            if (name == null)
            {
                return def;
            }
            if (!Settings.ContainsKey(name))
            {
                return def;
            }
            return int.Parse(Settings[name]);
        }

        /// <summary>
        /// Saves a string to the settings section.
        /// </summary>
        /// <param name="name">The key of the setting.</param>
        /// <param name="data">The string data of the setting.</param>
        public void SaveSetting(string name, string data)
        {
            SaveSetting(name, data, true);
        }

        /// <summary>
        /// Saves a string to the settings section.
        /// </summary>
        /// <param name="name">The key of the setting.</param>
        /// <param name="data">The string data of the setting.</param>
        /// <param name="saveData">True if the data has to be written to the XML file, false otherwise.</param>
        public void SaveSetting(string name, string data, bool saveData)
        {
            if (name == null || data == null)
            {
                throw new ArgumentNullException();
            }
            if (Settings.ContainsKey(name))
            {
                Settings[name] = data;
            }
            else
            {
                Settings.Add(name, data);
            }
            if (saveData)
            {
                SaveData();
            }
        }

        /// <summary>
        /// Saves an integer to the settings section.
        /// </summary>
        /// <param name="name">The key of the setting.</param>
        /// <param name="data">The integer data of the setting.</param>
        public void SaveSetting(string name, int data)
        {
            SaveSetting(name, data, true);
        }

        /// <summary>
        /// Saves an integer to the settings section.
        /// </summary>
        /// <param name="name">The key of the setting.</param>
        /// <param name="data">The integer data of the setting.</param>
        /// <param name="saveData">True if the data has to be written to the XML file, false otherwise.</param>
        public void SaveSetting(string name, int data, bool saveData)
        {
            SaveSetting(name, data.ToString(), saveData);
        }

        /// <summary>
        /// Saves an array of bytes to the settings section.
        /// </summary>
        /// <param name="name">The key of the setting.</param>
        /// <param name="data">The byte data of the setting.</param>
        public void SaveSetting(string name, byte[] data)
        {
            SaveSetting(name, data, true);
        }

        /// <summary>
        /// Saves an array of bytes to the settings section.
        /// </summary>
        /// <param name="name">The key of the setting.</param>
        /// <param name="data">The byte data of the setting.</param>
        /// <param name="saveData">True if the data has to be written to the XML file, false otherwise.</param>
        public void SaveSetting(string name, byte[] data, bool saveData)
        {
            if (data == null)
            {
                throw new ArgumentNullException();
            }
            SaveSetting(name, Convert.ToBase64String(data), saveData);
        }

        /// <summary>
        /// Saves a username and password combination to the authentication list.
        /// </summary>
        /// <param name="username">The username to add.</param>
        /// <param name="password">The password to add.</param>
        /// <exception cref="ArgumentNullException"><c>username</c> is null -or- <c>password</c> is null.</exception>
        /// <exception cref="ArgumentException">The specified username is invalid.</exception>
        /// <remarks><p>If the user already exists in the collection, the old password will be changed to the new one.</p><p>The username 'users' is invalid because it is used internally to store the usernames.</p></remarks>
        public void SaveUserPass(string username, string password)
        {
            SaveUserPass(username, password, true);
        }

        /// <summary>
        /// Saves a username and password combination to the authentication list.
        /// </summary>
        /// <param name="username">The username to add.</param>
        /// <param name="password">The password to add.</param>
        /// <param name="saveData">True if the data has to be written to the XML file, false otherwise.</param>
        /// <exception cref="ArgumentNullException"><c>username</c> is null -or- <c>password</c> is null.</exception>
        /// <exception cref="ArgumentException">The specified username is invalid.</exception>
        /// <remarks><p>If the user already exists in the collection, the old password will be changed to the new one.</p><p>The username 'users' is invalid because it is used internally to store the usernames.</p><p>The password will be hashed before it is stored in the authentication list.</p></remarks>
        public void SaveUserPass(string username, string password, bool saveData)
        {
            if (username == null || password == null)
            {
                throw new ArgumentNullException();
            }
            if (username.ToLower() == "users" || username == "")
            {
                throw new ArgumentException();
            }
            if (UserList.IsUserPresent(username))
            {
                UserList.RemoveItem(username);
            }
            UserList.AddItem(username, password);
            if (saveData)
            {
                SaveData();
            }
        }

        /// <summary>
        /// Saves a username and password hash combination to the authentication list.
        /// </summary>
        /// <param name="username">The username to add.</param>
        /// <param name="passHash">The password hash to add.</param>
        /// <exception cref="ArgumentNullException"><c>username</c> is null -or- <c>passHash</c> is null.</exception>
        /// <exception cref="ArgumentException">The specified username is invalid.</exception>
        /// <remarks><p>If the user already exists in the collection, the old password hash will be changed to the new one.</p><p>The username 'users' is invalid because it is used internally to store the usernames.</p><p>The password will <em>not</em> be hashed before it is stored in the authentication list. The user must make sure it is a valid MD5 hash.</p></remarks>
        public void SaveUserHash(string username, string passHash)
        {
            SaveUserHash(username, passHash, true);
        }

        /// <summary>
        /// Saves a username and password hash combination to the authentication list.
        /// </summary>
        /// <param name="username">The username to add.</param>
        /// <param name="passHash">The password hash to add.</param>
        /// <param name="saveData">True if the data has to be written to the XML file, false otherwise.</param>
        /// <exception cref="ArgumentNullException"><c>username</c> is null -or- <c>passHash</c> is null.</exception>
        /// <exception cref="ArgumentException">The specified username is invalid.</exception>
        /// <remarks><p>If the user already exists in the collection, the old password hash will be changed to the new one.</p><p>The username 'users' is invalid because it is used internally to store the usernames.</p><p>The password will <em>not</em> be hashed before it is stored in the authentication list. The user must make sure it is a valid MD5 hash.</p></remarks>
        public void SaveUserHash(string username, string passHash, bool saveData)
        {
            if (username == null || passHash == null)
            {
                throw new ArgumentNullException();
            }
            if (username.ToLower() == "users" || username == "")
            {
                throw new ArgumentException();
            }
            UserList.AddHash(username, passHash);
            if (saveData)
            {
                SaveData();
            }
        }

        /// <summary>
        /// Removes a user from the authentication list.
        /// </summary>
        /// <param name="user">The user to remove.</param>
        public void RemoveUser(string user)
        {
            RemoveUser(user, true);
        }

        /// <summary>
        /// Removes a user from the authentication list.
        /// </summary>
        /// <param name="user">The user to remove.</param>
        /// <param name="save">True if the data has to be written to the XML file, false otherwise.</param>
        public void RemoveUser(string user, bool save)
        {
            if (user == null)
            {
                throw new ArgumentNullException();
            }
            UserList.RemoveItem(user);
            if (save)
            {
                SaveData();
            }
        }

        /// <summary>
        /// Saves the data in this class to an XML file.
        /// </summary>
        public void SaveData()
        {
            XmlTextWriter writer = null;
            try
            {
                writer = new XmlTextWriter(File, Encoding.ASCII);
                writer.Indentation = 2;
                writer.Formatting = Formatting.Indented;
                writer.WriteStartElement("MentalisProxy");
                // Write the version
                writer.WriteStartElement("Version");
                writer.WriteStartAttribute("", "value", "");
                writer.WriteString(Assembly.GetCallingAssembly().GetName().Version.ToString(2));
                writer.WriteEndAttribute();
                writer.WriteEndElement();
                // Write the settings
                SaveSettings(writer);
                // Write the Authentication list to the file
                SaveUsers(writer);
                // Write the Listeners list to the file
                SaveListeners(writer);
                // Clean up
                writer.WriteEndElement();
            }
            catch
            {
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
        }

        /// <summary>
        /// Saves the settings in this class to an XML writer.
        /// </summary>
        /// <param name="writer">The XML writer to save the data to.</param>
        private void SaveSettings(XmlTextWriter writer)
        {
            writer.WriteStartElement("Settings");
            string[] keys = new string[Settings.Count];
            Settings.Keys.CopyTo(keys, 0);
            for (int i = 0; i < keys.Length; i++)
            {
                writer.WriteStartElement(keys[i]);
                writer.WriteStartAttribute("", "value", "");
                writer.WriteString(Settings[keys[i]]);
                writer.WriteEndAttribute();
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Saves the authentication list to an XML writer.
        /// </summary>
        /// <param name="writer">The XML writer to save the users to.</param>
        private void SaveUsers(XmlTextWriter writer)
        {
            writer.WriteStartElement("Users");
            string[] keys = UserList.Keys;
            string[] hashes = UserList.Hashes;
            for (int i = 0; i < keys.Length; i++)
            {
                writer.WriteStartElement(keys[i]);
                writer.WriteStartAttribute("", "value", "");
                writer.WriteString(hashes[i]);
                writer.WriteEndAttribute();
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Saves the listeners to an XML writer.
        /// </summary>
        /// <param name="writer">The XML writer to save the users to.</param>
        private void SaveListeners(XmlTextWriter writer)
        {
            writer.WriteStartElement("Listeners");
            lock (Parent)
            {
                for (int i = 0; i < Parent.ListenerCount; i++)
                {
                    writer.WriteStartElement("listener");
                    // Write the type, eg 'Org.Mentalis.Proxy.Http.HttpListener'
                    writer.WriteStartAttribute("", "type", "");
                    writer.WriteString(Parent[i].GetType().FullName);
                    writer.WriteEndAttribute();
                    // Write the construction string
                    writer.WriteStartAttribute("", "value", "");
                    writer.WriteString(Parent[i].ConstructString);
                    writer.WriteEndAttribute();
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Loads the data from an XML file.
        /// </summary>
        public void LoadData()
        {
            if (!System.IO.File.Exists(File))
            {
                throw new FileNotFoundException();
            }
            XmlTextReader reader = null;
            try
            {
                reader = new XmlTextReader(File);
                // Read until we reach the MentalisProxy element
                while (reader.Read() && reader.Name.ToLower() != "mentalisproxy")
                {
                }
                // Read until we reach the MentalisProxy element again (the end tag)
                while (reader.Read() && reader.Name.ToLower() != "mentalisproxy")
                {
                    if (!reader.IsEmptyElement)
                    {
                        switch (reader.Name.ToLower())
                        {
                            case "settings":
                                Settings.Clear();
                                LoadSettings(reader);
                                break;
                            case "users":
                                UserList.Clear();
                                LoadUsers(reader);
                                break;
                            case "listeners":
                                LoadListeners(reader);
                                break;
                        }
                    }
                }
            }
            catch
            {
                throw new XmlException("Malformed XML initialisation file.", null);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        /// <summary>
        /// Loads the settings from an XML file.
        /// </summary>
        /// <param name="reader">The XML reader to read the settings from.</param>
        private void LoadSettings(XmlTextReader reader)
        {
            // Read until we reach the Settings element end tag
            while (reader.Read() && reader.Name.ToLower() != "settings")
            {
                if (reader.Name != null && reader["value"] != null)
                {
                    SaveSetting(reader.Name, reader["value"], false);
                }
            }
        }

        /// <summary>
        /// Loads the userlist from an XML file.
        /// </summary>
        /// <param name="reader">The XML reader to read the users from.</param>
        private void LoadUsers(XmlTextReader reader)
        {
            // Read until we reach the Settings element end tag
            while (reader.Read() && reader.Name.ToLower() != "users")
            {
                if (reader.Name != null && reader["value"] != null)
                {
                    SaveUserHash(reader.Name, reader["value"], false);
                }
            }
        }

        /// <summary>
        /// Loads the listeners list from an XML file.
        /// </summary>
        /// <param name="reader">The XML reader to read the users from.</param>
        private void LoadListeners(XmlTextReader reader)
        {
            // Read until we reach the Listeners element end tag
            Listener listener = null;
            while (reader.Read() && reader.Name.ToLower() != "listeners")
            {
                if (reader.Name != null && reader["value"] != null && reader["type"] != null)
                {
                    listener = Parent.CreateListener(reader["type"], reader["value"]);
                    if (listener != null)
                    {
                        try
                        {
                            listener.Start();
                        }
                        catch
                        {
                        }
                        Parent.AddListener(listener);
                    }
                }
            }
        }
    }
}

/*
<MentalisProxy>
  <Version value="1.0" />
  <Settings>
    <authorize value="never" />
    <admin_hash value="Gh3JHJBzJcaScd3wyUS8cg==" />
    <config_ip value="localhost" />
    <config_port value="4" />
    <errorlog value="errors.log" />
  </Settings>
  <Users>
    <user value="myhash" />
    <pieter value="WhBei51A4TKXgNYuoiZdig==" />
    <kris value="X03MO1qnZdYdgyfeuILPmQ==" />
  </Users>
  <Listeners>
    <listener type="Org.Mentalis.Proxy.Http.HttpListener" value="host:0.0.0.0;int:100" />
    <listener type="Org.Mentalis.Proxy.Ftp.FtpListener" value="host:0.0.0.0;int:21" />
    <listener type="Org.Mentalis.Proxy.Socks.SocksListener" value="host:0.0.0.0;int:1080;authlist" />
    <listener type="Org.Mentalis.Proxy.PortMap.PortMapListener" value="host:0.0.0.0;int:12345;host:msnews.microsoft.com;int:119" />
  </Listeners>
</MentalisProxy>
*/