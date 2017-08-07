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

#endregion

namespace Arachnode.Proxy.Handlers
{
    ///<summary>Defines the signature of the method that's called when the SOCKS negotiation is complete.</summary>
    ///<param name="Success">Indicates whether the negotiation was successful or not.</param>
    ///<param name="Remote">The connection with the remote server.</param>
    internal delegate void NegotiationCompleteDelegate(bool Success, Socket Remote);

    ///<summary>Implements a specific version of the SOCKS protocol.</summary>
    internal abstract class SocksHandler
    {
        /// <summary>Holds the value of the Buffer property.</summary>
        private readonly byte[] m_Buffer = new byte[1024];

        /// <summary>Holds the address of the method to call when the SOCKS negotiation is complete.</summary>
        private readonly NegotiationCompleteDelegate Signaler;

        /// <summary>Holds the value of the Connection property.</summary>
        private Socket m_Connection;

        /// <summary>Holds the value of the RemoteConnection property.</summary>
        private Socket m_RemoteConnection;

        /// <summary>Holds the value of the Username property.</summary>
        private string m_Username;

        ///<summary>Initializes a new instance of the SocksHandler class.</summary>
        ///<param name="ClientConnection">The connection with the client.</param>
        ///<param name="Callback">The method to call when the SOCKS negotiation is complete.</param>
        ///<exception cref="ArgumentNullException"><c>Callback</c> is null.</exception>
        public SocksHandler(Socket ClientConnection, NegotiationCompleteDelegate Callback)
        {
            if (Callback == null)
            {
                throw new ArgumentNullException();
            }
            Connection = ClientConnection;
            Signaler = Callback;
        }

        ///<summary>Gets or sets the username of the SOCKS user.</summary>
        ///<value>A String representing the username of the logged on user.</value>
        ///<exception cref="ArgumentNullException">The specified value is null.</exception>
        internal string Username
        {
            get { return m_Username; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                m_Username = value;
            }
        }

        ///<summary>Gets or sets the connection with the client.</summary>
        ///<value>A Socket representing the connection between the proxy server and the SOCKS client.</value>
        ///<exception cref="ArgumentNullException">The specified value is null.</exception>
        protected Socket Connection
        {
            get { return m_Connection; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                m_Connection = value;
            }
        }

        ///<summary>Gets a buffer that can be used when receiving bytes from the client.</summary>
        ///<value>A byte array that can be used when receiving bytes from the client.</value>
        protected byte[] Buffer
        {
            get { return m_Buffer; }
        }

        ///<summary>Gets or sets a byte array that can be used to store received bytes from the client.</summary>
        ///<value>A byte array that can be used to store bytes from the client.</value>
        protected byte[] Bytes { get; set; }

        ///<summary>Gets or sets the connection with the remote host.</summary>
        ///<value>A Socket representing the connection between the proxy server and the remote host.</value>
        ///<exception cref="ArgumentNullException">The specified value is null.</exception>
        protected Socket RemoteConnection
        {
            get { return m_RemoteConnection; }
            set
            {
                m_RemoteConnection = value;
                try
                {
                    m_RemoteConnection.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, 1);
                }
                catch
                {
                }
            }
        }

        ///<summary>Gets or sets the socket that is used to accept incoming connections.</summary>
        ///<value>A Socket that is used to accept incoming connections.</value>
        protected Socket AcceptSocket { get; set; }

        ///<summary>Gets or sets the IP address of the requested remote server.</summary>
        ///<value>An IPAddress object specifying the address of the requested remote server.</value>
        protected IPAddress RemoteBindIP { get; set; }

        ///<summary>Closes the listening socket if present, and signals the parent object that SOCKS negotiation is complete.</summary>
        ///<param name="Success">Indicates whether the SOCKS negotiation was successful or not.</param>
        protected void Dispose(bool Success)
        {
            if (AcceptSocket != null)
            {
                AcceptSocket.Close();
            }
            Signaler(Success, RemoteConnection);
        }

        ///<summary>Starts accepting bytes from the client.</summary>
        public void StartNegotiating()
        {
            try
            {
                Connection.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(OnReceiveBytes), Connection);
            }
            catch
            {
                Dispose(false);
            }
        }

        ///<summary>Called when we receive some bytes from the client.</summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        protected void OnReceiveBytes(IAsyncResult ar)
        {
            try
            {
                int Ret = Connection.EndReceive(ar);
                if (Ret <= 0)
                {
                    Dispose(false);
                }
                AddBytes(Buffer, Ret);
                if (IsValidRequest(Bytes))
                {
                    ProcessRequest(Bytes);
                }
                else
                {
                    Connection.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(OnReceiveBytes), Connection);
                }
            }
            catch
            {
                Dispose(false);
            }
        }

        ///<summary>Called when an OK reply has been sent to the client.</summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        protected void OnDisposeGood(IAsyncResult ar)
        {
            try
            {
                if (Connection.EndSend(ar) > 0)
                {
                    Dispose(true);
                    return;
                }
            }
            catch
            {
            }
            Dispose(false);
        }

        ///<summary>Called when a negative reply has been sent to the client.</summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        protected void OnDisposeBad(IAsyncResult ar)
        {
            try
            {
                Connection.EndSend(ar);
            }
            catch
            {
            }
            Dispose(false);
        }

        ///<summary>Adds some bytes to a byte aray.</summary>
        ///<param name="NewBytes">The new bytes to add.</param>
        ///<param name="Cnt">The number of bytes to add.</param>
        protected void AddBytes(byte[] NewBytes, int Cnt)
        {
            if (Cnt <= 0 || NewBytes == null || Cnt > NewBytes.Length)
            {
                return;
            }
            if (Bytes == null)
            {
                Bytes = new byte[Cnt];
            }
            else
            {
                byte[] tmp = Bytes;
                Bytes = new byte[Bytes.Length + Cnt];
                Array.Copy(tmp, 0, Bytes, 0, tmp.Length);
            }
            Array.Copy(NewBytes, 0, Bytes, Bytes.Length - Cnt, Cnt);
        }

        ///<summary>Called when the AcceptSocket should start accepting incoming connections.</summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        protected void OnStartAccept(IAsyncResult ar)
        {
            try
            {
                if (Connection.EndSend(ar) <= 0)
                {
                    Dispose(false);
                }
                else
                {
                    AcceptSocket.BeginAccept(new AsyncCallback(OnAccept), AcceptSocket);
                }
            }
            catch
            {
                Dispose(false);
            }
        }

        ///<summary>Called when there's an incoming connection in the AcceptSocket queue.</summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        protected abstract void OnAccept(IAsyncResult ar);

        ///<summary>Sends a reply to the client connection and disposes it afterwards.</summary>
        ///<param name="Value">A byte that contains the reply code to send to the client.</param>
        protected abstract void Dispose(byte Value);

        ///<summary>Checks whether a specific request is a valid SOCKS request or not.</summary>
        ///<param name="Request">The request array to check.</param>
        ///<returns>True is the specified request is valid, false otherwise</returns>
        protected abstract bool IsValidRequest(byte[] Request);

        ///<summary>Processes a SOCKS request from a client.</summary>
        ///<param name="Request">The request to process.</param>
        protected abstract void ProcessRequest(byte[] Request);

        // private variables
    }
}