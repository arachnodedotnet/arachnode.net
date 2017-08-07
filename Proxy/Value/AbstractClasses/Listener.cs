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
using System.Net;
using System.Net.Sockets;

#endregion

namespace Arachnode.Proxy.Value.AbstractClasses
{
    ///<summary>Specifies the basic methods and properties of a <c>Listener</c> object. This is an abstract class and must be inherited.</summary>
    ///<remarks>The Listener class provides an abstract base class that represents a listening socket of the proxy server. Descendant classes further specify the protocol that is used between those two connections.</remarks>
    public abstract class Listener : IDisposable
    {
#pragma warning disable 612,618
        #region Delegates

        public delegate void ClientRemoved(Client client);

        #endregion

        private readonly object m_ClientsLock = new object();

        /// <summary>Holds the value of the Clients property.</summary>
        private readonly ArrayList m_Clients = new ArrayList();

        /// <summary>Holds the value of the Address property.</summary>
        private IPAddress m_Address;

        /// <summary>Holds the value of the IsDisposed property.</summary>
        private bool m_IsDisposed;

        /// <summary>Holds the value of the ListenSocket property.</summary>
        private Socket m_ListenSocket;

        /// <summary>Holds the value of the Port property.</summary>
        private int m_Port;

        ///<summary>Initializes a new instance of the Listener class.</summary>
        ///<param name="Port">The port to listen on.</param>
        ///<param name="Address">The address to listen on. You can specify IPAddress.Any to listen on all installed network cards.</param>
        ///<remarks>For the security of your server, try to avoid to listen on every network card (IPAddress.Any). Listening on a local IP address is usually sufficient and much more secure.</remarks>
        public Listener(int Port, IPAddress Address)
        {
            this.Port = Port;
            this.Address = Address;
        }

        ///<summary>Gets or sets the port number on which to listen on.</summary>
        ///<value>An integer defining the port number to listen on.</value>
        ///<seealso cref ="Address"/>
        ///<exception cref="ArgumentException">The specified value is less than or equal to zero.</exception>
        protected int Port
        {
            get { return m_Port; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException();
                }
                m_Port = value;
                Restart();
            }
        }

        ///<summary>Gets or sets the address on which to listen on.</summary>
        ///<value>An IPAddress instance defining the IP address to listen on.</value>
        ///<seealso cref ="Port"/>
        ///<exception cref="ArgumentNullException">The specified value is null.</exception>
        protected IPAddress Address
        {
            get { return m_Address; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                m_Address = value;
                Restart();
            }
        }

        ///<summary>Gets or sets the listening Socket.</summary>
        ///<value>An instance of the Socket class that's used to listen for incoming connections.</value>
        ///<exception cref="ArgumentNullException">The specified value is null.</exception>
        protected Socket ListenSocket
        {
            get { return m_ListenSocket; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                m_ListenSocket = value;
            }
        }

        ///<summary>Gets the list of connected clients.</summary>
        ///<value>An instance of the ArrayList class that's used to store all the connections.</value>
        protected ArrayList Clients
        {
            get { return m_Clients; }
        }

        ///<summary>Gets a value indicating whether the Listener has been disposed or not.</summary>
        ///<value>An boolean that specifies whether the object has been disposed or not.</value>
        public bool IsDisposed
        {
            get { return m_IsDisposed; }
        }

        ///<summary>Gets a value indicating whether the Listener is currently listening or not.</summary>
        ///<value>A boolean that indicates whether the Listener is currently listening or not.</value>
        public bool Listening
        {
            get { return ListenSocket != null; }
        }

        ///<summary>Returns a string that holds all the construction information for this object.</summary>
        ///<value>A string that holds all the construction information for this object.</value>
        public abstract string ConstructString { get; }

        #region IDisposable Members

        ///<summary>Disposes of the resources (other than memory) used by the Listener.</summary>
        ///<remarks>Stops listening and disposes <em>all</em> the client objects. Once disposed, this object should not be used anymore.</remarks>
        ///<seealso cref ="System.IDisposable"/>
        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }
            while (Clients.Count > 0)
            {
                ((Client) Clients[0]).Dispose();
            }
            try
            {
                ListenSocket.Shutdown(SocketShutdown.Both);
            }
            catch
            {
            }
            if (ListenSocket != null)
            {
                ListenSocket.Close();
            }
            m_IsDisposed = true;
        }

        #endregion

        public event ClientRemoved OnClientRemoved;

        ///<summary>Starts listening on the selected IP address and port.</summary>
        ///<exception cref="SocketException">There was an error while creating the listening socket.</exception>
        public void Start()
        {
            try
            {
                ListenSocket = new Socket(Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                ListenSocket.Bind(new IPEndPoint(Address, Port));
                ListenSocket.Listen(50);
                ListenSocket.BeginAccept(new AsyncCallback(OnAccept), ListenSocket);
            }
            catch
            {
                ListenSocket = null;
                throw new SocketException();
            }
        }

        public void Stop()
        {
            Dispose();
        }

        ///<summary>Restarts listening on the selected IP address and port.</summary>
        ///<remarks>This method is automatically called when the listening port or the listening IP address are changed.</remarks>
        ///<exception cref="SocketException">There was an error while creating the listening socket.</exception>
        protected void Restart()
        {
            //If we weren't listening, do nothing
            if (ListenSocket == null)
            {
                return;
            }
            ListenSocket.Close();
            Start();
        }

        ///<summary>Adds the specified Client to the client list.</summary>
        ///<remarks>A client will never be added twice to the list.</remarks>
        ///<param name="client">The client to add to the client list.</param>
        protected void AddClient(Client client)
        {
            if (Clients.IndexOf(client) == -1)
            {
                Clients.Add(client);
            }
        }

        ///<summary>Removes the specified Client from the client list.</summary>
        ///<param name="client">The client to remove from the client list.</param>
        protected void RemoveClient(Client client)
        {
            lock (m_ClientsLock)
            {
                if (Clients.Contains(client))
                {
                    Clients.Remove(client);

                    if (OnClientRemoved != null)
                    {
                        OnClientRemoved.Invoke(client);
                    }
                }
            }
        }

        ///<summary>Returns the number of clients in the client list.</summary>
        ///<returns>The number of connected clients.</returns>
        public int GetClientCount()
        {
            return Clients.Count;
        }

        ///<summary>Returns the requested client from the client list.</summary>
        ///<param name="Index">The index of the requested client.</param>
        ///<returns>The requested client.</returns>
        ///<remarks>If the specified index is invalid, the GetClientAt method returns null.</remarks>
        public Client GetClientAt(int Index)
        {
            if (Index < 0 || Index >= GetClientCount())
            {
                return null;
            }
            return (Client) Clients[Index];
        }

        ///<summary>Finalizes the Listener.</summary>
        ///<remarks>The destructor calls the Dispose method.</remarks>
        ~Listener()
        {
            Dispose();
        }

        ///<summary>Returns an external IP address of this computer, if present.</summary>
        ///<returns>Returns an external IP address of this computer; if this computer does not have an external IP address, it returns the first local IP address it can find.</returns>
        ///<remarks>If this computer does not have any configured IP address, this method returns the IP address 0.0.0.0.</remarks>
        public static IPAddress GetLocalExternalIP()
        {
            try
            {
                IPHostEntry he = Dns.Resolve(Dns.GetHostName());
                for (int Cnt = 0; Cnt < he.AddressList.Length; Cnt++)
                {
                    if (IsRemoteIP(he.AddressList[Cnt]))
                    {
                        return he.AddressList[Cnt];
                    }
                }
                return he.AddressList[0];
            }
            catch
            {
                return IPAddress.Any;
            }
        }

        ///<summary>Checks whether the specified IP address is a remote IP address or not.</summary>
        ///<param name="IP">The IP address to check.</param>
        ///<returns>True if the specified IP address is a remote address, false otherwise.</returns>
        protected static bool IsRemoteIP(IPAddress IP)
        {
            byte First = (byte) Math.Floor((decimal) IP.Address%256);
            byte Second = (byte) Math.Floor((decimal) (IP.Address%65536)/256);
            //Not 10.x.x.x And Not 172.16.x.x <-> 172.31.x.x And Not 192.168.x.x
            //And Not Any And Not Loopback And Not Broadcast
            return (First != 10) &&
                   (First != 172 || (Second < 16 || Second > 31)) &&
                   (First != 192 || Second != 168) &&
                   (!IP.Equals(IPAddress.Any)) &&
                   (!IP.Equals(IPAddress.Loopback)) &&
                   (!IP.Equals(IPAddress.Broadcast));
        }

        ///<summary>Checks whether the specified IP address is a local IP address or not.</summary>
        ///<param name="IP">The IP address to check.</param>
        ///<returns>True if the specified IP address is a local address, false otherwise.</returns>
        protected static bool IsLocalIP(IPAddress IP)
        {
            byte First = (byte) Math.Floor((decimal) IP.Address%256);
            byte Second = (byte) Math.Floor((decimal) (IP.Address%65536)/256);
            //10.x.x.x Or 172.16.x.x <-> 172.31.x.x Or 192.168.x.x
            return (First == 10) ||
                   (First == 172 && (Second >= 16 && Second <= 31)) ||
                   (First == 192 && Second == 168);
        }

        ///<summary>Returns an internal IP address of this computer, if present.</summary>
        ///<returns>Returns an internal IP address of this computer; if this computer does not have an internal IP address, it returns the first local IP address it can find.</returns>
        ///<remarks>If this computer does not have any configured IP address, this method returns the IP address 0.0.0.0.</remarks>
        public static IPAddress GetLocalInternalIP()
        {
            try
            {
                IPHostEntry he = Dns.Resolve(Dns.GetHostName());
                for (int Cnt = 0; Cnt < he.AddressList.Length; Cnt++)
                {
                    if (IsLocalIP(he.AddressList[Cnt]))
                    {
                        return he.AddressList[Cnt];
                    }
                }
                return he.AddressList[0];
            }
            catch
            {
                return IPAddress.Any;
            }
        }

        ///<summary>Called when there's an incoming client connection waiting to be accepted.</summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        public abstract void OnAccept(IAsyncResult ar);

        ///<summary>Returns a string representation of this object.</summary>
        ///<returns>A string with information about this object.</returns>
        public abstract override string ToString();
    }
#pragma warning restore 612,618
}