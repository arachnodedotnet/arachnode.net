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
using Arachnode.Proxy.Clients;
using Arachnode.Proxy.Value.AbstractClasses;

#endregion

namespace Arachnode.Proxy.Listeners
{
    ///<summary>Listens on a specific port on the proxy server for incoming SOCKS4 and SOCKS5 requests.</summary>
    ///<remarks>This class also implements the SOCKS4a protocol.</remarks>
    public sealed class SocksListener : Listener
    {
        ///<summary>Initializes a new instance of the SocksListener class.</summary>
        ///<param name="Port">The port to listen on.</param>
        ///<remarks>The SocksListener will listen on all available network cards and it will not use an AuthenticationList.</remarks>
        public SocksListener(int Port) : this(IPAddress.Any, Port, null)
        {
        }

        ///<summary>Initializes a new instance of the SocksListener class.</summary>
        ///<param name="Port">The port to listen on.</param>
        ///<param name="Address">The address to listen on. You can specify IPAddress.Any to listen on all installed network cards.</param>
        ///<remarks>For the security of your server, try to avoid to listen on every network card (IPAddress.Any). Listening on a local IP address is usually sufficient and much more secure.</remarks>
        ///<remarks>The SocksListener object will not use an AuthenticationList.</remarks>
        public SocksListener(IPAddress Address, int Port) : this(Address, Port, null)
        {
        }

        ///<summary>Initializes a new instance of the SocksListener class.</summary>
        ///<param name="Port">The port to listen on.</param>
        ///<param name="AuthList">The list of valid login/password combinations. If you do not need password authentication, set this parameter to null.</param>
        ///<remarks>The SocksListener will listen on all available network cards.</remarks>
        public SocksListener(int Port, AuthenticationList AuthList) : this(IPAddress.Any, Port, AuthList)
        {
        }

        ///<summary>Initializes a new instance of the SocksListener class.</summary>
        ///<param name="Port">The port to listen on.</param>
        ///<param name="Address">The address to listen on. You can specify IPAddress.Any to listen on all installed network cards.</param>
        ///<param name="AuthList">The list of valid login/password combinations. If you do not need password authentication, set this parameter to null.</param>
        ///<remarks>For the security of your server, try to avoid to listen on every network card (IPAddress.Any). Listening on a local IP address is usually sufficient and much more secure.</remarks>
        public SocksListener(IPAddress Address, int Port, AuthenticationList AuthList) : base(Port, Address)
        {
            this.AuthList = AuthList;
        }

        ///<summary>Gets or sets the AuthenticationList to be used when a SOCKS5 client connects.</summary>
        ///<value>An AuthenticationList that is to be used when a SOCKS5 client connects.</value>
        ///<remarks>This value can be null.</remarks>
        private AuthenticationList AuthList { get; set; }

        ///<summary>Returns a string that holds all the construction information for this object.</summary>
        ///<value>A string that holds all the construction information for this object.</value>
        public override string ConstructString
        {
            get
            {
                if (AuthList == null)
                {
                    return "host:" + Address + ";int:" + Port + ";null";
                }
                else
                {
                    return "host:" + Address + ";int:" + Port + ";authlist";
                }
            }
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
                    SocksClient NewClient = new SocksClient(NewSocket, RemoveClient, AuthList);
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
            return "SOCKS service on " + Address + ":" + Port;
        }

        // private variables
    }
}