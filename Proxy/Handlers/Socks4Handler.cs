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
using Arachnode.Proxy.Value.AbstractClasses;

#endregion

namespace Arachnode.Proxy.Handlers
{
    ///<summary>Implements the SOCKS4 and SOCKS4a protocols.</summary>
    internal sealed class Socks4Handler : SocksHandler
    {
#pragma warning disable 612,618

        ///<summary>Initializes a new instance of the Socks4Handler class.</summary>
        ///<param name="ClientConnection">The connection with the client.</param>
        ///<param name="Callback">The method to call when the SOCKS negotiation is complete.</param>
        ///<exception cref="ArgumentNullException"><c>Callback</c> is null.</exception>
        public Socks4Handler(Socket ClientConnection, NegotiationCompleteDelegate Callback) : base(ClientConnection, Callback)
        {
        }

        ///<summary>Checks whether a specific request is a valid SOCKS request or not.</summary>
        ///<param name="Request">The request array to check.</param>
        ///<returns>True is the specified request is valid, false otherwise</returns>
        protected override bool IsValidRequest(byte[] Request)
        {
            try
            {
                if (Request[0] != 1 && Request[0] != 2)
                {
                    //CONNECT or BIND
                    Dispose(false);
                }
                else
                {
                    if (Request[3] == 0 && Request[4] == 0 && Request[5] == 0 && Request[6] != 0)
                    {
                        //Use remote DNS
                        int Ret = Array.IndexOf(Request, (byte) 0, 7);
                        if (Ret > -1)
                        {
                            return Array.IndexOf(Request, (byte) 0, Ret + 1) != -1;
                        }
                    }
                    else
                    {
                        return Array.IndexOf(Request, (byte) 0, 7) != -1;
                    }
                }
            }
            catch
            {
            }
            return false;
        }

        ///<summary>Processes a SOCKS request from a client.</summary>
        ///<param name="Request">The request to process.</param>
        protected override void ProcessRequest(byte[] Request)
        {
            int Ret;
            try
            {
                if (Request[0] == 1)
                {
                    // CONNECT
                    IPAddress RemoteIP;
                    int RemotePort = Request[1]*256 + Request[2];
                    Ret = Array.IndexOf(Request, (byte) 0, 7);
                    Username = Encoding.ASCII.GetString(Request, 7, Ret - 7);
                    if (Request[3] == 0 && Request[4] == 0 && Request[5] == 0 && Request[6] != 0)
                    {
// Use remote DNS
                        Ret = Array.IndexOf(Request, (byte) 0, Ret + 1);
                        RemoteIP = Dns.Resolve(Encoding.ASCII.GetString(Request, Username.Length + 8, Ret - Username.Length - 8)).AddressList[0];
                    }
                    else
                    {
                        //Do not use remote DNS
                        RemoteIP = IPAddress.Parse(Request[3] + "." + Request[4] + "." + Request[5] + "." + Request[6]);
                    }
                    RemoteConnection = new Socket(RemoteIP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    RemoteConnection.BeginConnect(new IPEndPoint(RemoteIP, RemotePort), OnConnected, RemoteConnection);
                }
                else if (Request[0] == 2)
                {
                    // BIND
                    byte[] Reply = new byte[8];
                    long LocalIP = Listener.GetLocalExternalIP().Address;
                    AcceptSocket = new Socket(IPAddress.Any.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    AcceptSocket.Bind(new IPEndPoint(IPAddress.Any, 0));
                    AcceptSocket.Listen(50);
                    RemoteBindIP = IPAddress.Parse(Request[3] + "." + Request[4] + "." + Request[5] + "." + Request[6]);
                    Reply[0] = 0; //Reply version 0
                    Reply[1] = 90; //Everything is ok :)
                    Reply[2] = (byte) (Math.Floor((decimal) ((IPEndPoint) AcceptSocket.LocalEndPoint).Port/256)); //Port/1
                    Reply[3] = (byte) (((IPEndPoint) AcceptSocket.LocalEndPoint).Port%256); //Port/2
                    Reply[4] = (byte) (Math.Floor((decimal) (LocalIP%256))); //IP Address/1
                    Reply[5] = (byte) (Math.Floor((decimal) (LocalIP%65536)/256)); //IP Address/2
                    Reply[6] = (byte) (Math.Floor((decimal) (LocalIP%16777216)/65536)); //IP Address/3
                    Reply[7] = (byte) (Math.Floor((decimal) LocalIP/16777216)); //IP Address/4
                    Connection.BeginSend(Reply, 0, Reply.Length, SocketFlags.None, new AsyncCallback(OnStartAccept), Connection);
                }
            }
            catch
            {
                Dispose(91);
            }
        }

        ///<summary>Called when we're successfully connected to the remote host.</summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        private void OnConnected(IAsyncResult ar)
        {
            try
            {
                RemoteConnection.EndConnect(ar);
                Dispose(90);
            }
            catch
            {
                Dispose(91);
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
                                 0, Value, (byte) (Math.Floor((decimal) ((IPEndPoint) RemoteConnection.RemoteEndPoint).Port/256)),
                                 (byte) (((IPEndPoint) RemoteConnection.RemoteEndPoint).Port%256),
                                 #pragma warning disable 612,618
                                 (byte) (Math.Floor((decimal) (((IPEndPoint) RemoteConnection.RemoteEndPoint).Address.Address%256))),
                                 (byte) (Math.Floor((decimal) (((IPEndPoint) RemoteConnection.RemoteEndPoint).Address.Address%65536)/256)),
                                 (byte) (Math.Floor((decimal) (((IPEndPoint) RemoteConnection.RemoteEndPoint).Address.Address%16777216)/65536)),
                                 (byte) (Math.Floor((decimal) ((IPEndPoint) RemoteConnection.RemoteEndPoint).Address.Address/16777216))
                                 #pragma warning restore 612,618
                             };
            }
            catch
            {
                ToSend = new byte[] {0, 91, 0, 0, 0, 0, 0, 0};
            }
            try
            {
                Connection.BeginSend(ToSend, 0, ToSend.Length, SocketFlags.None, (ToSend[1] == 90 ? new AsyncCallback(OnDisposeGood) : new AsyncCallback(OnDisposeBad)), Connection);
            }
            catch
            {
                Dispose(false);
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
                if (RemoteBindIP.Equals(((IPEndPoint) RemoteConnection.RemoteEndPoint).Address))
                {
                    Dispose(90);
                }
                else
                {
                    Dispose(91);
                }
            }
            catch
            {
                Dispose(91);
            }
        }
    }
#pragma warning restore 612,618
}