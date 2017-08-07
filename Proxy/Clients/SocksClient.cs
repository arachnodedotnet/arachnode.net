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
using Arachnode.Proxy.Authentication;
using Arachnode.Proxy.Handlers;
using Arachnode.Proxy.Value.AbstractClasses;

#endregion

namespace Arachnode.Proxy.Clients
{
    ///<summary>Relays data between a remote host and a local client, using the SOCKS protocols.</summary>
    ///<remarks>This class implements the SOCKS4, SOCKS4a and SOCKS5 protocols.</remarks>
    ///<remarks>If the MustAuthenticate property is set, only SOCKS5 connections are allowed and the AuthList parameter of the constructor should not be null.</remarks>
    public sealed class SocksClient : Client
    {
        /// <summary>Holds the value of the Handler property.</summary>
        private SocksHandler m_Handler;

        ///<summary>Initializes a new instance of the SocksClient class.</summary>
        ///<param name="ClientSocket">The Socket connection between this proxy server and the local client.</param>
        ///<param name="Destroyer">The method to be called when this SocksClient object disconnects from the local client and the remote server.</param>
        ///<param name="AuthList">The list with valid username/password combinations.</param>
        ///<remarks>If the AuthList is non-null, every client has to authenticate before he can use this proxy server to relay data. If it is null, the clients don't have to authenticate.</remarks>
        public SocksClient(Socket ClientSocket, DestroyDelegate Destroyer, AuthenticationList AuthList) : base(ClientSocket, Destroyer)
        {
            this.AuthList = AuthList;
        }

        ///<summary>Gets or sets the SOCKS handler to be used when communicating with the client.</summary>
        ///<value>The SocksHandler to be used when communicating with the client.</value>
        internal SocksHandler Handler
        {
            get { return m_Handler; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                m_Handler = value;
            }
        }

        ///<summary>Gets or sets the SOCKS handler to be used when communicating with the client.</summary>
        ///<value>The SocksHandler to be used when communicating with the client.</value>
        public bool MustAuthenticate { get; set; }

        ///<summary>Gets or sets the AuthenticationList to use when a computer tries to authenticate on the proxy server.</summary>
        ///<value>An instance of the AuthenticationList class that contains all the valid username/password combinations.</value>
        private AuthenticationList AuthList { get; set; }

        ///<summary>Starts communication with the client.</summary>
        public override void StartHandshake()
        {
            try
            {
                ClientSocket.BeginReceive(Buffer, 0, 1, SocketFlags.None, new AsyncCallback(OnStartSocksProtocol), ClientSocket);
            }
            catch
            {
                Dispose();
            }
        }

        ///<summary>Called when we have received some data from the client.</summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        private void OnStartSocksProtocol(IAsyncResult ar)
        {
            int Ret;
            try
            {
                Ret = ClientSocket.EndReceive(ar);
                if (Ret <= 0)
                {
                    Dispose();
                    return;
                }
                if (Buffer[0] == 4)
                {
                    //SOCKS4 Protocol
                    if (MustAuthenticate)
                    {
                        Dispose();
                        return;
                    }
                    else
                    {
                        Handler = new Socks4Handler(ClientSocket, OnEndSocksProtocol);
                    }
                }
                else if (Buffer[0] == 5)
                {
                    //SOCKS5 Protocol
                    if (MustAuthenticate && AuthList == null)
                    {
                        Dispose();
                        return;
                    }
                    Handler = new Socks5Handler(ClientSocket, OnEndSocksProtocol, AuthList);
                }
                else
                {
                    Dispose();
                    return;
                }
                Handler.StartNegotiating();
            }
            catch
            {
                Dispose();
            }
        }

        ///<summary>Called when the SOCKS protocol has ended. We can no start relaying data, if the SOCKS authentication was successful.</summary>
        ///<param name="Success">Specifies whether the SOCKS negotiation was successful or not.</param>
        ///<param name="Remote">The connection with the remote server.</param>
        private void OnEndSocksProtocol(bool Success, Socket Remote)
        {
            DestinationSocket = Remote;
            if (Success)
            {
                StartRelay();
            }
            else
            {
                Dispose();
            }
        }

        ///<summary>Returns text information about this SocksClient object.</summary>
        ///<returns>A string representing this SocksClient object.</returns>
        public override string ToString()
        {
            try
            {
                if (Handler != null)
                {
                    return Handler.Username + " (" + ((IPEndPoint) ClientSocket.LocalEndPoint).Address + ") connected to " + DestinationSocket.RemoteEndPoint;
                }
                else
                {
                    return "SOCKS connection from " + ((IPEndPoint) ClientSocket.LocalEndPoint).Address;
                }
            }
            catch
            {
                return "Incoming SOCKS connection";
            }
        }

        // private variables
    }
}