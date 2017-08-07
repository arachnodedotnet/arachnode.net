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
using Arachnode.Proxy.Value.AbstractClasses;

#endregion

namespace Arachnode.Proxy.Clients
{
    ///<summary>Relays data between a remote host and a local client.</summary>
    public sealed class PortMapClient : Client
    {
        /// <summary>Holds the value of the MapTo property.</summary>
        private IPEndPoint m_MapTo;

        ///<summary>Initializes a new instance of the PortMapClient class.</summary>
        ///<param name="ClientSocket">The <see cref ="Socket">Socket</see> connection between this proxy server and the local client.</param>
        ///<param name="Destroyer">The callback method to be called when this Client object disconnects from the local client and the remote server.</param>
        ///<param name="MapTo">The IP EndPoint to send the incoming data to.</param>
        public PortMapClient(Socket ClientSocket, DestroyDelegate Destroyer, IPEndPoint MapTo) : base(ClientSocket, Destroyer)
        {
            this.MapTo = MapTo;
        }

        ///<summary>Gets or sets the IP EndPoint to map all incoming traffic to.</summary>
        ///<value>An IPEndPoint that holds the IP address and port to use when redirecting incoming traffic.</value>
        ///<exception cref="ArgumentNullException">The specified value is null.</exception>
        ///<returns>An IP EndPoint specifying the host and port to map all incoming traffic to.</returns>
        private IPEndPoint MapTo
        {
            get { return m_MapTo; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                m_MapTo = value;
            }
        }

        ///<summary>Starts connecting to the remote host.</summary>
        public override void StartHandshake()
        {
            try
            {
                DestinationSocket = new Socket(MapTo.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                DestinationSocket.BeginConnect(MapTo, OnConnected, DestinationSocket);
            }
            catch
            {
                Dispose();
            }
        }

        ///<summary>Called when the socket is connected to the remote host.</summary>
        ///<remarks>When the socket is connected to the remote host, the PortMapClient begins relaying traffic between the host and the client, until one of them closes the connection.</remarks>
        ///<param name="ar">The result of the asynchronous operation.</param>
        private void OnConnected(IAsyncResult ar)
        {
            try
            {
                DestinationSocket.EndConnect(ar);
                StartRelay();
            }
            catch
            {
                Dispose();
            }
        }

        ///<summary>Returns text information about this PortMapClient object.</summary>
        ///<returns>A string representing this PortMapClient object.</returns>
        public override string ToString()
        {
            try
            {
                return "Forwarding port from " + ((IPEndPoint) ClientSocket.RemoteEndPoint).Address + " to " + MapTo;
            }
            catch
            {
                return "Incoming Port forward connection";
            }
        }

        // private variables
    }
}