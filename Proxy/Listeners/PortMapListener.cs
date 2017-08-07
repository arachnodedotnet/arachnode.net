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
using Arachnode.Proxy.Clients;
using Arachnode.Proxy.Value.AbstractClasses;

#endregion

namespace Arachnode.Proxy.Listeners
{
    ///<summary>Listens on a specific port on the proxy server and forwards all incoming data to a specific port on another server.</summary>
    public sealed class PortMapListener : Listener
    {
        /// <summary>Holds the value of the MapTo property.</summary>
        private IPEndPoint m_MapTo;

        ///<summary>Initializes a new instance of the PortMapListener class.</summary>
        ///<param name="Port">The port to listen on.</param>
        ///<param name="MapToIP">The address to forward to.</param>
        ///<remarks>The object will listen on all network addresses on the computer.</remarks>
        ///<exception cref="ArgumentException"><paramref name="Port">Port</paramref> is not positive.</exception>
        ///<exception cref="ArgumentNullException"><paramref name="MapToIP">MapToIP</paramref> is null.</exception>
        public PortMapListener(int Port, IPEndPoint MapToIP) : this(IPAddress.Any, Port, MapToIP)
        {
        }

        ///<summary>Initializes a new instance of the PortMapListener class.</summary>
        ///<param name="Port">The port to listen on.</param>
        ///<param name="Address">The network address to listen on.</param>
        ///<param name="MapToIP">The address to forward to.</param>
        ///<remarks>For security reasons, <paramref name="Address">Address</paramref> should not be IPAddress.Any.</remarks>
        ///<exception cref="ArgumentNullException">Address or <paramref name="MapToIP">MapToIP</paramref> is null.</exception>
        ///<exception cref="ArgumentException">Port is not positive.</exception>
        public PortMapListener(IPAddress Address, int Port, IPEndPoint MapToIP) : base(Port, Address)
        {
            MapTo = MapToIP;
        }

        ///<summary>Initializes a new instance of the PortMapListener class.</summary>
        ///<param name="Port">The port to listen on.</param>
        ///<param name="Address">The network address to listen on.</param>
        ///<param name="MapToPort">The port to forward to.</param>
        ///<param name="MapToAddress">The IP address to forward to.</param>
        ///<remarks>For security reasons, Address should not be IPAddress.Any.</remarks>
        ///<exception cref="ArgumentNullException">Address or MapToAddress is null.</exception>
        ///<exception cref="ArgumentException">Port or MapToPort is invalid.</exception>
        public PortMapListener(IPAddress Address, int Port, IPAddress MapToAddress, int MapToPort) : this(Address, Port, new IPEndPoint(MapToAddress, MapToPort))
        {
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

        ///<summary>Returns a string that holds all the construction information for this object.</summary>
        ///<value>A string that holds all the construction information for this object.</value>
        public override string ConstructString
        {
            get { return "host:" + Address + ";int:" + Port + ";host:" + MapTo.Address + ";int:" + MapTo.Port; }
        }

        ///<summary>Called when there's an incoming client connection waiting to be accepted.</summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        public override void OnAccept(IAsyncResult ar)
        {
            try
            {
                Socket NewSocket = ListenSocket.EndAccept(ar);
                if (NewSocket != null)
                {
                    PortMapClient NewClient = new PortMapClient(NewSocket, RemoveClient, MapTo);
                    AddClient(NewClient);
                    NewClient.StartHandshake();
                }
            }
            catch
            {
            }
            try
            {
                //Restart Listening
                ListenSocket.BeginAccept(new AsyncCallback(OnAccept), ListenSocket);
            }
            catch
            {
                Dispose();
            }
        }

        ///<summary>Returns a string representation of this object.</summary>
        ///<returns>A string with information about this object.</returns>
        public override string ToString()
        {
            return "PORTMAP service on " + Address + ":" + Port;
        }
    }
}