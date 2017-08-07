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
    ///<summary>Listens on a specific port on the proxy server and forwards all incoming HTTP traffic to the appropriate server.</summary>
    public sealed class HttpListener : Listener
    {
        #region Delegates

        public delegate void ClientAdded(HttpClient httpClient);

        #endregion

        public event ClientAdded OnClientAdded;

        ///<summary>Initializes a new instance of the HttpListener class.</summary>
        ///<param name="Port">The port to listen on.</param>
        ///<remarks>The HttpListener will start listening on all installed network cards.</remarks>
        public HttpListener(int Port) : this(IPAddress.Any, Port)
        {
        }

        ///<summary>Initializes a new instance of the HttpListener class.</summary>
        ///<param name="Port">The port to listen on.</param>
        ///<param name="Address">The address to listen on. You can specify IPAddress.Any to listen on all installed network cards.</param>
        ///<remarks>For the security of your server, try to avoid to listen on every network card (IPAddress.Any). Listening on a local IP address is usually sufficient and much more secure.</remarks>
        public HttpListener(IPAddress Address, int Port) : base(Port, Address)
        {
        }

        ///<summary>Returns a string that holds all the construction information for this object.</summary>
        ///<value>A string that holds all the construction information for this object.</value>
        public override string ConstructString
        {
            get { return "host:" + Address + ";int:" + Port; }
        }

        ///<summary>Called when there's an incoming client connection waiting to be accepted.</summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        public override void OnAccept(IAsyncResult ar)
        {
            try
            {
                Socket NewSocket = ListenSocket.EndAccept(ar);
                try
                {
                    //Restart Listening
                    ListenSocket.BeginAccept(new AsyncCallback(OnAccept), ListenSocket);
                }
                catch
                {
                    Dispose();
                }
                if (NewSocket != null)
                {
                    HttpClient NewClient = new HttpClient(NewSocket, RemoveClient);
                    AddClient(NewClient);
                    NewClient.StartHandshake();

                    if (OnClientAdded != null)
                    {
                        OnClientAdded.Invoke(NewClient);
                    }
                }
            }
            catch
            {
            }
        }

        ///<summary>Returns a string representation of this object.</summary>
        ///<returns>A string with information about this object.</returns>
        public override string ToString()
        {
            return "HTTP service on " + Address + ":" + Port;
        }
    }
}