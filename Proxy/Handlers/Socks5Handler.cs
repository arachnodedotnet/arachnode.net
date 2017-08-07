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
using System.Net;
using System.Net.Sockets;
using System.Text;
using Arachnode.Proxy.Authentication;
using Arachnode.Proxy.Value.AbstractClasses;

#endregion

namespace Arachnode.Proxy.Handlers
{
    ///<summary>Implements the SOCKS5 protocol.</summary>
    internal sealed class Socks5Handler : SocksHandler
    {
#pragma warning disable 612,618
        /// <summary>Holds the value of the AuthenticationMethod property.</summary>
        private AuthenticationBase m_AuthenticationMethod;

        ///<summary>Initializes a new instance of the Socks5Handler class.</summary>
        ///<param name="ClientConnection">The connection with the client.</param>
        ///<param name="Callback">The method to call when the SOCKS negotiation is complete.</param>
        ///<param name="AuthList">The authentication list to use when clients connect.</param>
        ///<exception cref="ArgumentNullException"><c>Callback</c> is null.</exception>
        ///<remarks>If the AuthList parameter is null, no authentication will be required when a client connects to the proxy server.</remarks>
        public Socks5Handler(Socket ClientConnection, NegotiationCompleteDelegate Callback, AuthenticationList AuthList) : base(ClientConnection, Callback)
        {
            this.AuthList = AuthList;
        }

        ///<summary>Initializes a new instance of the Socks5Handler class.</summary>
        ///<param name="ClientConnection">The connection with the client.</param>
        ///<param name="Callback">The method to call when the SOCKS negotiation is complete.</param>
        ///<exception cref="ArgumentNullException"><c>Callback</c> is null.</exception>
        public Socks5Handler(Socket ClientConnection, NegotiationCompleteDelegate Callback) : this(ClientConnection, Callback, null)
        {
        }

        ///<summary>Gets or sets the the AuthenticationBase object to use when trying to authenticate the SOCKS client.</summary>
        ///<value>The AuthenticationBase object to use when trying to authenticate the SOCKS client.</value>
        ///<exception cref="ArgumentNullException">The specified value is null.</exception>
        private AuthenticationBase AuthenticationMethod
        {
            get { return m_AuthenticationMethod; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                m_AuthenticationMethod = value;
            }
        }

        ///<summary>Gets or sets the AuthenticationList object to use when trying to authenticate the SOCKS client.</summary>
        ///<value>The AuthenticationList object to use when trying to authenticate the SOCKS client.</value>
        private AuthenticationList AuthList { get; set; }

        ///<summary>Checks whether a specific request is a valid SOCKS request or not.</summary>
        ///<param name="Request">The request array to check.</param>
        ///<returns>True is the specified request is valid, false otherwise</returns>
        protected override bool IsValidRequest(byte[] Request)
        {
            try
            {
                return (Request.Length == Request[0] + 1);
            }
            catch
            {
                return false;
            }
        }

        ///<summary>Processes a SOCKS request from a client and selects an authentication method.</summary>
        ///<param name="Request">The request to process.</param>
        protected override void ProcessRequest(byte[] Request)
        {
            try
            {
                byte Ret = 255;
                for (int Cnt = 1; Cnt < Request.Length; Cnt++)
                {
                    if (Request[Cnt] == 0 && AuthList == null)
                    {
                        //0 = No authentication
                        Ret = 0;
                        AuthenticationMethod = new AuthNone();
                        break;
                    }
                    else if (Request[Cnt] == 2 && AuthList != null)
                    {
                        //2 = user/pass
                        Ret = 2;
                        AuthenticationMethod = new AuthUserPass(AuthList);
                        if (AuthList != null)
                        {
                            break;
                        }
                    }
                }
                Connection.BeginSend(new byte[] {5, Ret}, 0, 2, SocketFlags.None, new AsyncCallback(OnAuthSent), Connection);
            }
            catch
            {
                Dispose(false);
            }
        }

        ///<summary>Called when client has been notified of the selected authentication method.</summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        private void OnAuthSent(IAsyncResult ar)
        {
            try
            {
                if (Connection.EndSend(ar) <= 0 || AuthenticationMethod == null)
                {
                    Dispose(false);
                    return;
                }
                AuthenticationMethod.StartAuthentication(Connection, OnAuthenticationComplete);
            }
            catch
            {
                Dispose(false);
            }
        }

        ///<summary>Called when the authentication is complete.</summary>
        ///<param name="Success">Indicates whether the authentication was successful ot not.</param>
        private void OnAuthenticationComplete(bool Success)
        {
            try
            {
                if (Success)
                {
                    Bytes = null;
                    Connection.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(OnRecvRequest), Connection);
                }
                else
                {
                    Dispose(false);
                }
            }
            catch
            {
                Dispose(false);
            }
        }

        ///<summary>Called when we received the request of the client.</summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        private void OnRecvRequest(IAsyncResult ar)
        {
            try
            {
                int Ret = Connection.EndReceive(ar);
                if (Ret <= 0)
                {
                    Dispose(false);
                    return;
                }
                AddBytes(Buffer, Ret);
                if (IsValidQuery(Bytes))
                {
                    ProcessQuery(Bytes);
                }
                else
                {
                    Connection.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(OnRecvRequest), Connection);
                }
            }
            catch
            {
                Dispose(false);
            }
        }

        ///<summary>Checks whether a specified query is a valid query or not.</summary>
        ///<param name="Query">The query to check.</param>
        ///<returns>True if the query is valid, false otherwise.</returns>
        private bool IsValidQuery(byte[] Query)
        {
            try
            {
                switch (Query[3])
                {
                    case 1: //IPv4 address
                        return (Query.Length == 10);
                    case 3: //Domain name
                        return (Query.Length == Query[4] + 7);
                    case 4: //IPv6 address
                        //Not supported
                        Dispose(8);
                        return false;
                    default:
                        Dispose(false);
                        return false;
                }
            }
            catch
            {
                return false;
            }
        }

        ///<summary>Processes a received query.</summary>
        ///<param name="Query">The query to process.</param>
        private void ProcessQuery(byte[] Query)
        {
            try
            {
                switch (Query[1])
                {
                    case 1: //CONNECT
                        IPAddress RemoteIP = null;
                        int RemotePort = 0;
                        if (Query[3] == 1)
                        {
                            RemoteIP = IPAddress.Parse(Query[4] + "." + Query[5] + "." + Query[6] + "." + Query[7]);
                            RemotePort = Query[8]*256 + Query[9];
                        }
                        else if (Query[3] == 3)
                        {
                            RemoteIP = Dns.Resolve(Encoding.ASCII.GetString(Query, 5, Query[4])).AddressList[0];
                            RemotePort = Query[4] + 5;
                            RemotePort = Query[RemotePort]*256 + Query[RemotePort + 1];
                        }
                        RemoteConnection = new Socket(RemoteIP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                        RemoteConnection.BeginConnect(new IPEndPoint(RemoteIP, RemotePort), OnConnected, RemoteConnection);
                        break;
                    case 2: //BIND
                        byte[] Reply = new byte[10];
                        long LocalIP = Listener.GetLocalExternalIP().Address;
                        AcceptSocket = new Socket(IPAddress.Any.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                        AcceptSocket.Bind(new IPEndPoint(IPAddress.Any, 0));
                        AcceptSocket.Listen(50);
                        Reply[0] = 5; //Version 5
                        Reply[1] = 0; //Everything is ok :)
                        Reply[2] = 0; //Reserved
                        Reply[3] = 1; //We're going to send a IPv4 address
                        Reply[4] = (byte) (Math.Floor((decimal) (LocalIP%256))); //IP Address/1
                        Reply[5] = (byte) (Math.Floor((decimal) (LocalIP%65536)/256)); //IP Address/2
                        Reply[6] = (byte) (Math.Floor((decimal) (LocalIP%16777216)/65536)); //IP Address/3
                        Reply[7] = (byte) (Math.Floor((decimal) LocalIP/16777216)); //IP Address/4
                        Reply[8] = (byte) (Math.Floor((decimal) ((IPEndPoint) AcceptSocket.LocalEndPoint).Port/256)); //Port/1
                        Reply[9] = (byte) (((IPEndPoint) AcceptSocket.LocalEndPoint).Port%256); //Port/2
                        Connection.BeginSend(Reply, 0, Reply.Length, SocketFlags.None, new AsyncCallback(OnStartAccept), Connection);
                        break;
                    case 3: //ASSOCIATE
                        //ASSOCIATE is not implemented (yet?)
                        Dispose(7);
                        break;
                    default:
                        Dispose(7);
                        break;
                }
            }
            catch
            {
                Dispose(1);
            }
        }

        ///<summary>Called when we're successfully connected to the remote host.</summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        private void OnConnected(IAsyncResult ar)
        {
            try
            {
                RemoteConnection.EndConnect(ar);
                Dispose(0);
            }
            catch
            {
                Dispose(1);
            }
        }

        ///<summary>Called when there's an incoming connection in the AcceptSocket queue.</summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        protected override void OnAccept(IAsyncResult ar)
        {
            try
            {
                RemoteConnection = AcceptSocket.EndAccept(ar);
                AcceptSocket.Close();
                AcceptSocket = null;
                Dispose(0);
            }
            catch
            {
                Dispose(1);
            }
        }

        ///<summary>Sends a reply to the client connection and disposes it afterwards.</summary>
        ///<param name="Value">A byte that contains the reply code to send to the client.</param>
        protected override void Dispose(byte Value)
        {
            byte[] ToSend;
            try
            {
                ToSend = new byte[]
                             {
                                 5, Value, 0, 1,
                                 (byte) (((IPEndPoint) RemoteConnection.LocalEndPoint).Address.Address%256),
                                 (byte) (Math.Floor((decimal) (((IPEndPoint) RemoteConnection.LocalEndPoint).Address.Address%65536)/256)),
                                 (byte) (Math.Floor((decimal) (((IPEndPoint) RemoteConnection.LocalEndPoint).Address.Address%16777216)/65536)),
                                 (byte) (Math.Floor((decimal) ((IPEndPoint) RemoteConnection.LocalEndPoint).Address.Address/16777216)),
                                 (byte) (Math.Floor((decimal) ((IPEndPoint) RemoteConnection.LocalEndPoint).Port/256)),
                                 (byte) (((IPEndPoint) RemoteConnection.LocalEndPoint).Port%256)
                             };
            }
            catch
            {
                ToSend = new byte[] {5, 1, 0, 1, 0, 0, 0, 0, 0, 0};
            }
            try
            {
                Connection.BeginSend(ToSend, 0, ToSend.Length, SocketFlags.None, (ToSend[1] == 0 ? new AsyncCallback(OnDisposeGood) : new AsyncCallback(OnDisposeBad)), Connection);
            }
            catch
            {
                Dispose(false);
            }
        }
    }
#pragma warning restore 612,618
}