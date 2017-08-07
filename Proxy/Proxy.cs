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
using System.Collections;
using System.IO;
using System.Net;
using System.Reflection;
using Arachnode.Proxy.Value.AbstractClasses;

#endregion

namespace Arachnode.Proxy
{
    /// <summary>
    /// Represents an item in a Listeners collection.
    /// </summary>
    public struct ListenEntry
    {
        /// <summary>
        /// The Listener's ID. It must be unique troughout the Listeners collection.
        /// </summary>
        public Guid guid;

        /// <summary>
        /// The Listener object.
        /// </summary>
        public Listener listener;

        /// <summary>
        /// Determines whether the specified Object is equal to the current Object.
        /// </summary>
        /// <param name="obj">The Object to compare with the current Object.</param>
        /// <returns>True if the specified Object is equal to the current Object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return ((ListenEntry) obj).guid.Equals(guid);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    /// <summary>
    /// Defines the class that controls the settings and listener objects.
    /// </summary>
    public class Proxy
    {
        /// <summary>Holds the value of the Listeners property.</summary>
        private readonly ArrayList m_Listeners = new ArrayList();

        /// <summary>
        /// Initializes a new Proxy instance.
        /// </summary>
        /// <param name="file">The XML configuration file to use.</param>
        public Proxy(string file)
        {
            Config = new ProxyConfig(this, file);
        }

        /// <summary>
        /// Gets the collection that contains all the Listener objects.
        /// </summary>
        /// <value>An ArrayList object that contains all the Listener objects.</value>
        protected ArrayList Listeners
        {
            get { return m_Listeners; }
        }

        /// <summary>
        /// Gets the number of Listener objects.
        /// </summary>
        /// <value>An integer specifying the number of Listener objects.</value>
        internal int ListenerCount
        {
            get { return Listeners.Count; }
        }

        /// <summary>
        /// Gets the Listener object at the specified position.
        /// </summary>
        /// <value>The Listener instance at position <c>index</c>.</value>
        internal virtual Listener this[int index]
        {
            get { return ((ListenEntry) Listeners[index]).listener; }
        }

        /// <summary>
        /// Gets or sets the date when this Proxy server was first started.
        /// </summary>
        /// <value>A DateTime structure that indicates when this Proxy server was first started.</value>
        protected DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the configuration object for this Proxy server.
        /// </summary>
        /// <value>A ProxyConfig instance that represents the configuration object for this Proxy server.</value>
        protected ProxyConfig Config { get; set; }

        /// <summary>
        /// Entry point of the application.
        /// </summary>
        public static void Main()
        {
            try
            {
                string dir = Environment.CurrentDirectory;
                if (!dir.Substring(dir.Length - 1, 1).Equals(@"\"))
                {
                    dir += @"\";
                }
                Proxy prx = new Proxy(dir + "config.xml");
                prx.Start();
            }
            catch
            {
                Console.WriteLine("The program ended abnormally!");
            }
        }

        /// <summary>
        /// Starts a new Proxy server by reading the data from the configuration file and start listening on the specified ports.
        /// </summary>
        public void Start()
        {
            // Initialize some objects
            StartTime = DateTime.Now;
            if (File.Exists(Config.File))
            {
                Config.LoadData();
            }
            // Start the proxy
            string command;
            Console.WriteLine("\r\n  Mentalis.org Proxy\r\n  ~~~~~~~~~~~~~~~~~~\r\n\r\n (type 'help' for the command list)");
            Console.Write("\r\n>");
            command = Console.ReadLine().ToLower();
            while (!command.Equals("exit"))
            {
                switch (command)
                {
                    case "help":
                        ShowHelp();
                        break;
                    case "uptime":
                        ShowUpTime();
                        break;
                    case "version":
                        ShowVersion();
                        break;
                    case "adduser":
                        ShowAddUser();
                        break;
                    case "deluser":
                        ShowDelUser();
                        break;
                    case "listusers":
                        ShowUsers();
                        break;
                    case "addlistener":
                        ShowAddListener();
                        break;
                    case "listlisteners":
                        ShowListeners();
                        break;
                    case "dellistener":
                        ShowDelListener();
                        break;
                    default:
                        Console.WriteLine("Command not understood.");
                        break;
                }
                Console.Write("\r\n>");
                command = Console.ReadLine().ToLower();
            }
            Stop();
            Console.WriteLine("Goodbye...");
        }

        /// <summary>
        /// Asks the user which listener to delete.
        /// </summary>
        protected void ShowDelListener()
        {
            Console.WriteLine("Please enter the ID of the listener you want to delete:\r\n (use the 'listlisteners' command to show all the listener IDs)");
            string id = Console.ReadLine();
            if (id != "")
            {
                try
                {
                    ListenEntry le = new ListenEntry();
                    le.guid = new Guid(id);
                    if (!Listeners.Contains(le))
                    {
                        Console.WriteLine("Specified ID not found in list!");
                        return;
                    }
                    else
                    {
                        this[Listeners.IndexOf(le)].Dispose();
                        Listeners.Remove(le);
                        Config.SaveData();
                    }
                }
                catch
                {
                    Console.WriteLine("Invalid ID tag!");
                    return;
                }
                Console.WriteLine("Listener removed from the list.");
            }
        }

        /// <summary>
        /// Shows the Listeners list.
        /// </summary>
        protected void ShowListeners()
        {
            for (int i = 0; i < Listeners.Count; i++)
            {
                Console.WriteLine(((ListenEntry) Listeners[i]).listener.ToString());
                Console.WriteLine("  id: " + ((ListenEntry) Listeners[i]).guid.ToString("N"));
            }
        }

        /// <summary>
        /// Asks the user which listener to add.
        /// </summary>
        protected void ShowAddListener()
        {
            Console.WriteLine("Please enter the full class name of the Listener object you're trying to add:\r\n (ie. Org.Mentalis.Proxy.Http.HttpListener)");
            string classtype = Console.ReadLine();
            if (classtype == "")
            {
                return;
            }
            else if (Type.GetType(classtype) == null)
            {
                Console.WriteLine("The specified class does not exist!");
                return;
            }
            Console.WriteLine("Please enter the construction parameters:");
            string construct = Console.ReadLine();
            object listenObject = CreateListener(classtype, construct);
            if (listenObject == null)
            {
                Console.WriteLine("Invalid construction string.");
                return;
            }
            Listener listener;
            try
            {
                listener = (Listener) listenObject;
            }
            catch
            {
                Console.WriteLine("The specified object is not a valid Listener object.");
                return;
            }
            try
            {
                listener.Start();
                AddListener(listener);
            }
            catch
            {
                Console.WriteLine("Error while staring the Listener.\r\n(Perhaps the specified port is already in use?)");
                return;
            }
            Config.SaveData();
        }

        /// <summary>
        /// Shows a list of commands in the console.
        /// </summary>
        protected void ShowHelp()
        {
            Console.WriteLine("          help - Shows this help message\r\n        uptime - Shows the uptime of the proxy server\r\n       version - Prints the version of this program\r\n     listusers - Lists all users\r\n       adduser - Adds a user to the user list\r\n       deluser - Deletes a user from the user list\r\n listlisteners - Lists all the listeners\r\n   addlistener - Adds a new listener\r\n   dellistener - Deletes a listener\r\n\r\n Read the readme.txt file for more help.");
        }

        /// <summary>
        /// Shows the uptime of this proxy server.
        /// </summary>
        protected void ShowUpTime()
        {
            TimeSpan uptime = DateTime.Now.Subtract(StartTime);
            Console.WriteLine("Up " + uptime);
        }

        /// <summary>
        /// Shows the version number of this proxy server.
        /// </summary>
        protected void ShowVersion()
        {
            Console.WriteLine("This is version " + Assembly.GetCallingAssembly().GetName().Version.ToString(3) + " of the Mentalis.org proxy server.");
        }

        /// <summary>
        /// Asks the user which username to add.
        /// </summary>
        protected void ShowAddUser()
        {
            Console.Write("Please enter the username to add: ");
            string name = Console.ReadLine();
            if (Config.UserList.IsUserPresent(name))
            {
                Console.WriteLine("Username already exists in database.");
                return;
            }
            Console.Write("Please enter the password: ");
            ConsoleAttributes.EchoInput = false;
            string pass1 = Console.ReadLine();
            Console.Write("\r\nPlease enter the password again: ");
            string pass2 = Console.ReadLine();
            ConsoleAttributes.EchoInput = true;
            if (!pass1.Equals(pass2))
            {
                Console.WriteLine("\r\nThe passwords do not match.");
                return;
            }
            Config.SaveUserPass(name, pass1);
            Console.WriteLine("\r\nUser successfully added.");
        }

        /// <summary>
        /// Asks the user which username to delete.
        /// </summary>
        protected void ShowDelUser()
        {
            Console.Write("Please enter the username to remove: ");
            string name = Console.ReadLine();
            if (!Config.UserList.IsUserPresent(name))
            {
                Console.WriteLine("Username not present in database.");
                return;
            }
            Config.RemoveUser(name);
            Console.WriteLine("User '" + name + "' successfully removed.");
        }

        /// <summary>
        /// Shows a list of usernames in the console.
        /// </summary>
        protected void ShowUsers()
        {
            if (Config.UserList == null || Config.UserList.Keys.Length == 0)
            {
                Console.WriteLine("There are no users in the user list.");
            }
            else
            {
                Console.WriteLine("The following " + Config.UserList.Keys.Length + " users are allowed to use the SOCKS5 proxy:");
                Console.WriteLine(string.Join(", ", Config.UserList.Keys));
            }
        }

        /// <summary>
        /// Stops the proxy server.
        /// </summary>
        /// <remarks>When this method is called, all listener and client objects will be disposed.</remarks>
        public void Stop()
        {
            // Stop listening and clear the Listener list
            for (int i = 0; i < ListenerCount; i++)
            {
                Console.WriteLine(this[i] + " stopped.");
                this[i].Dispose();
            }
            Listeners.Clear();
        }

        /// <summary>
        /// Adds a listener to the Listeners list.
        /// </summary>
        /// <param name="newItem">The new Listener to add.</param>
        public void AddListener(Listener newItem)
        {
            if (newItem == null)
            {
                throw new ArgumentNullException();
            }
            ListenEntry le = new ListenEntry();
            le.listener = newItem;
            le.guid = Guid.NewGuid();
            while (Listeners.Contains(le))
            {
                le.guid = Guid.NewGuid();
            }
            Listeners.Add(le);
            Console.WriteLine(newItem + " started.");
        }

        /// <summary>
        /// Creates a new Listener obejct from a given listener name and a given listener parameter string.
        /// </summary>
        /// <param name="type">The type of object to instantiate.</param>
        /// <param name="cpars"></param>
        /// <returns></returns>
        public Listener CreateListener(string type, string cpars)
        {
            try
            {
                string[] parts = cpars.Split(';');
                object[] pars = new object[parts.Length];
                string oval = null, otype = null;
                int ret;
                // Start instantiating the objects to give to the constructor
                for (int i = 0; i < parts.Length; i++)
                {
                    ret = parts[i].IndexOf(':');
                    if (ret >= 0)
                    {
                        otype = parts[i].Substring(0, ret);
                        oval = parts[i].Substring(ret + 1);
                    }
                    else
                    {
                        otype = parts[i];
                    }
                    switch (otype.ToLower())
                    {
                        case "int":
                            pars[i] = int.Parse(oval);
                            break;
                        case "host":
#pragma warning disable 612,618
                            pars[i] = Dns.Resolve(oval).AddressList[0];
#pragma warning restore 612,618
                            break;
                        case "authlist":
                            pars[i] = Config.UserList;
                            break;
                        case "null":
                            pars[i] = null;
                            break;
                        case "string":
                            pars[i] = oval;
                            break;
                        case "ip":
                            pars[i] = IPAddress.Parse(oval);
                            break;
                        default:
                            pars[i] = null;
                            break;
                    }
                }
                return (Listener) Activator.CreateInstance(Type.GetType(type), pars);
            }
            catch
            {
                return null;
            }
        }
    }
}