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
using System.Net.Sockets;
using System.Text;
using Arachnode.Proxy.Value.AbstractClasses;

#endregion

namespace Arachnode.Proxy.Authentication
{
    ///<summary>Authenticates a user on a SOCKS5 server according to the username/password authentication subprotocol.</summary>
    internal sealed class AuthUserPass : AuthenticationBase
    {
        ///<summary>Initializes a new instance of the AuthUserPass class.</summary>
        ///<param name="AuthList">An AuthenticationList object that contains the list of all valid username/password combinations.</param>
        ///<remarks>If the AuthList parameter is null, any username/password combination will be accepted.</remarks>
        public AuthUserPass(AuthenticationList AuthList)
        {
            this.AuthList = AuthList;
        }

        ///<summary>Gets or sets the AuthenticationList to use when a computer tries to authenticate on the proxy server.</summary>
        ///<value>An instance of the AuthenticationList class that contains all the valid username/password combinations.</value>
        private AuthenticationList AuthList { get; set; }

        ///<summary>Starts the authentication process.</summary>
        ///<param name="Connection">The connection with the SOCKS client.</param>
        ///<param name="Callback">The method to call when the authentication is complete.</param>
        internal override void StartAuthentication(Socket Connection, AuthenticationCompleteDelegate Callback)
        {
            this.Connection = Connection;
            this.Callback = Callback;
            try
            {
                Bytes = null;
                Connection.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(OnRecvRequest), Connection);
            }
            catch
            {
                Callback(false);
            }
        }

        ///<summary>Called when we have received the initial authentication data from the SOCKS client.</summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        private void OnRecvRequest(IAsyncResult ar)
        {
            try
            {
                int Ret = Connection.EndReceive(ar);
                if (Ret <= 0)
                {
                    Callback(false);
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
                Callback(false);
            }
        }

        ///<summary>Checks whether the specified authentication query is a valid one.</summary>
        ///<param name="Query">The query to check.</param>
        ///<returns>True if the query is a valid authentication query, false otherwise.</returns>
        private bool IsValidQuery(byte[] Query)
        {
            try
            {
                return (Query.Length == Query[1] + Query[Query[1] + 2] + 3);
            }
            catch
            {
                return false;
            }
        }

        ///<summary>Processes an authentication query.</summary>
        ///<param name="Query">The query to process.</param>
        private void ProcessQuery(byte[] Query)
        {
            try
            {
                string User = Encoding.ASCII.GetString(Query, 2, Query[1]);
                string Pass = Encoding.ASCII.GetString(Query, Query[1] + 3, Query[Query[1] + 2]);
                byte[] ToSend;
                if (AuthList == null || AuthList.IsItemPresent(User, Pass))
                {
                    ToSend = new byte[] {5, 0};
                    Connection.BeginSend(ToSend, 0, ToSend.Length, SocketFlags.None, new AsyncCallback(OnOkSent), Connection);
                }
                else
                {
                    ToSend = new Byte[] {5, 1};
                    Connection.BeginSend(ToSend, 0, ToSend.Length, SocketFlags.None, new AsyncCallback(OnUhohSent), Connection);
                }
            }
            catch
            {
                Callback(false);
            }
        }

        ///<summary>Called when an OK reply has been sent to the client.</summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        private void OnOkSent(IAsyncResult ar)
        {
            try
            {
                if (Connection.EndSend(ar) <= 0)
                {
                    Callback(false);
                }
                else
                {
                    Callback(true);
                }
            }
            catch
            {
                Callback(false);
            }
        }

        ///<summary>Called when a negatiev reply has been sent to the client.</summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        private void OnUhohSent(IAsyncResult ar)
        {
            try
            {
                Connection.EndSend(ar);
            }
            catch
            {
            }
            Callback(false);
        }

        // private variables
    }
}