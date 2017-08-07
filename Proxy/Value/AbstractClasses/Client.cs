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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

#endregion

namespace Arachnode.Proxy.Value.AbstractClasses
{
    /// <summary>References the callback method to be called when the <c>Client</c> object disconnects from the local client and the remote server.</summary>
    /// <param name="client">The <c>Client</c> that has closed its connections.</param>
    public delegate void DestroyDelegate(Client client);

    ///<summary>Specifies the basic methods and properties of a <c>Client</c> object. This is an abstract class and must be inherited.</summary>
    ///<remarks>The Client class provides an abstract base class that represents a connection to a local client and a remote server. Descendant classes further specify the protocol that is used between those two connections.</remarks>
    public abstract class Client : IDisposable
    {
        #region Delegates

        public delegate void ClientReceivedEnd(Socket clientSocket, Client client, string headers, List<byte> buffer, bool isComplete);

        public delegate void ClientSentEnd(Socket clientSocket, Client client, string headers, List<byte> buffer, bool isComplete);

        public delegate void RemoteReceivedEnd(Socket destinationSocket, Client client, string headers, List<byte> buffer, bool isComplete);

        public delegate void RemoteSentEnd(Socket destinationSocket, Client client, string headers, List<byte> buffer, bool isComplete);

        public delegate void ClientHeadersParsed(string headers);

        public delegate void RemoteHeadersParsed(string headers);

        #endregion

        public event ClientReceivedEnd OnClientReceiveEnd;
        public event RemoteSentEnd OnRemoteSentEnd;
        public event RemoteReceivedEnd OnRemoteReceivedEnd;
        public event ClientSentEnd OnClientSentEnd;
        public event ClientHeadersParsed OnClientHeadersParsed;
        public event RemoteHeadersParsed OnRemoteHeadersParsed;

        private object _idLock = new object();
        private static int _id;
        public int InstanceID { get; private set; }

        private object _bufferLock = new object();

        /// <summary>Holds the address of the method to call when this client is ready to be destroyed.</summary>
        private readonly DestroyDelegate Destroyer;

        /// <summary>Holds the value of the Buffer property.</summary>
        private readonly byte[] m_Buffer = new byte[4096]; //0<->4095 = 4096

        /// <summary>Holds the value of the RemoteBuffer property.</summary>
        private readonly byte[] m_RemoteBuffer = new byte[1024];

        /// <summary>Holds the value of the ClientSocket property.</summary>
        private Socket m_ClientSocket;

        /// <summary>Holds the value of the DestinationSocket property.</summary>
        private Socket m_DestinationSocket;

        protected bool _cacheClientBufferHeaders;
        protected List<byte> _cachedClientBuffer;
        protected string _cachedClientBufferHeaders { get; set; }

        protected bool _cacheRemoteBufferHeaders;
        protected List<byte> _cachedRemoteBuffer { get; set; }
        protected string _cachedRemoteBufferHeaders { get; set; }

        public bool Cancel { get; set; }
        private object _disposeLock = new object();

        ///<summary>Initializes a new instance of the Client class.</summary>
        ///<param name="ClientSocket">The <see cref ="Socket">Socket</see> connection between this proxy server and the local client.</param>
        ///<param name="Destroyer">The callback method to be called when this Client object disconnects from the local client and the remote server.</param>
        public Client(Socket ClientSocket, DestroyDelegate Destroyer)
        {
            lock (_idLock)
            {
                _id++;
                InstanceID = _id;
            }

            _cacheClientBufferHeaders = true;
            _cacheRemoteBufferHeaders = true;
            _cachedClientBuffer = new List<byte>();
            _cachedRemoteBuffer = new List<byte>();

            this.ClientSocket = ClientSocket;
            this.Destroyer = Destroyer;
        }

        ///<summary>Initializes a new instance of the Client object.</summary>
        ///<remarks>Both the ClientSocket property and the DestroyDelegate are initialized to null.</remarks>
        public Client()
        {
            lock (_idLock)
            {
                _id++;
                InstanceID = _id;
            }

            _cacheClientBufferHeaders = true;
            _cacheRemoteBufferHeaders = true;
            _cachedClientBuffer = new List<byte>();
            _cachedRemoteBuffer = new List<byte>();

            ClientSocket = null;
            Destroyer = null;
        }

        ///<summary>Gets or sets the Socket connection between the proxy server and the local client.</summary>
        ///<value>A Socket instance defining the connection between the proxy server and the local client.</value>
        ///<seealso cref ="DestinationSocket"/>
        public Socket ClientSocket
        {
            get { return m_ClientSocket; }
            internal set
            {
                lock (_bufferLock)
                {
                    if (m_ClientSocket != null)
                    {
                        m_ClientSocket.Close();
                    }
                    m_ClientSocket = value;
                }
            }
        }

        ///<summary>Gets or sets the Socket connection between the proxy server and the remote host.</summary>
        ///<value>A Socket instance defining the connection between the proxy server and the remote host.</value>
        ///<seealso cref ="ClientSocket"/>
        public Socket DestinationSocket
        {
            get { return m_DestinationSocket; }
            internal set
            {
                lock (_bufferLock)
                {
                    if (m_DestinationSocket != null)
                    {
                        m_DestinationSocket.Close();
                    }
                    m_DestinationSocket = value;
                }
            }
        }

        ///<summary>Gets the buffer to store all the incoming data from the local client.</summary>
        ///<value>An array of bytes that can be used to store all the incoming data from the local client.</value>
        ///<seealso cref ="RemoteBuffer"/>
        protected byte[] Buffer
        {
            get { return m_Buffer; }
        }

        ///<summary>Gets the buffer to store all the incoming data from the remote host.</summary>
        ///<value>An array of bytes that can be used to store all the incoming data from the remote host.</value>
        ///<seealso cref ="Buffer"/>
        protected byte[] RemoteBuffer
        {
            get { return m_RemoteBuffer; }
        }

        #region IDisposable Members

        ///<summary>Disposes of the resources (other than memory) used by the Client.</summary>
        ///<remarks>Closes the connections with the local client and the remote host. Once <c>Dispose</c> has been called, this object should not be used anymore.</remarks>
        ///<seealso cref ="System.IDisposable"/>
        public void Dispose()
        {
            try
            {
                lock (_bufferLock)
                {
                    lock (_disposeLock)
                    {
                        try
                        {
                            ClientSocket.Shutdown(SocketShutdown.Both);
                        }
                        catch
                        {
                        }
                        if (ClientSocket != null)
                        {
                            ClientSocket.Close();
                        }

                        try
                        {
                            DestinationSocket.Shutdown(SocketShutdown.Both);
                        }
                        catch
                        {
                        }
                        if (DestinationSocket != null)
                        {
                            DestinationSocket.Close();
                        }

                        ClientSocket = null;
                        DestinationSocket = null;

                        if (Destroyer != null)
                        {
                            Destroyer(this);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        #endregion

        ///<summary>Returns text information about this Client object.</summary>
        ///<returns>A string representing this Client object.</returns>
        public override string ToString()
        {
            try
            {
                return "Incoming connection from " + ((IPEndPoint) DestinationSocket.RemoteEndPoint).Address;
            }
            catch
            {
                return "Client connection";
            }
        }

        ///<summary>Starts relaying data between the remote host and the local client.</summary>
        ///<remarks>This method should only be called after all protocol specific communication has been finished.</remarks>
        public void StartRelay()
        {
            lock (_bufferLock)
            {
                try
                {
                    if (!Cancel)
                    {
                        ClientSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(OnClientReceive), ClientSocket);
                        DestinationSocket.BeginReceive(RemoteBuffer, 0, RemoteBuffer.Length, SocketFlags.None, new AsyncCallback(OnRemoteReceive), DestinationSocket);
                    }
                    else
                    {
                        Dispose();
                    }
                }
                catch
                {
                    Dispose();
                }
            }
        }

        ///<summary>Called when we have received data from the local client.<br>Incoming data will immediately be forwarded to the remote host.</br></summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        protected void OnClientReceive(IAsyncResult ar)
        {
            lock (_bufferLock)
            {
                try
                {
                    if(ClientSocket == null)
                    {
                        return;
                    }

                    int Ret = ClientSocket.EndReceive(ar);
                    if (Ret <= 0)
                    {
                        if (OnClientReceiveEnd != null)
                        {
                            if (_cachedClientBuffer != null)
                            {
                                //OnClientReceivedEnd.BeginInvoke(DestinationSocket, this, _cachedClientBufferHeaders, _cachedClientBuffer, true, null, null);
                                OnClientReceiveEnd.Invoke(DestinationSocket, this, _cachedClientBufferHeaders, _cachedClientBuffer.Count == 0 ? null : _cachedClientBuffer, true);
                            }
                            else
                            {
                                OnClientReceiveEnd.Invoke(DestinationSocket, this, _cachedClientBufferHeaders, _cachedClientBuffer, true);
                            }
                        }

                        if (OnRemoteSentEnd != null)
                        {
                            if (_cachedClientBufferHeaders != null)
                            {
                                //OnClientSentEnd.BeginInvoke(ClientSocket, this, _cachedClientBufferHeaders, _cachedClientBuffer, true, null, null);
                                OnClientSentEnd.Invoke(ClientSocket, this, _cachedClientBufferHeaders, _cachedClientBuffer.Count == 0 ? null : _cachedClientBuffer, true);
                            }
                            else
                            {
                                OnClientSentEnd.Invoke(ClientSocket, this, _cachedClientBufferHeaders, _cachedClientBuffer, true);
                            }
                        }

                        Dispose();
                        return;
                    }
                    _cachedClientBuffer.AddRange(Buffer);
                    if (_cacheClientBufferHeaders)
                    {
                        _cachedClientBufferHeaders = Encoding.Default.GetString(_cachedClientBuffer.ToArray()).TrimEnd("\0".ToCharArray());

                        int index = _cachedClientBufferHeaders.IndexOf("\r\n\r\n");
                        if (index != -1)
                        {
                            _cachedClientBufferHeaders = _cachedClientBufferHeaders.Substring(0, index);

                            _cacheClientBufferHeaders = false;
                            if (Ret != index + 4)
                            {
                                _cachedClientBuffer = _cachedClientBuffer.GetRange(index + 4, _cachedClientBuffer.Count - index - 4);
                            }
                            else
                            {
                                _cachedClientBuffer.Clear();
                                _cachedRemoteBuffer.Clear();
                            }

                            if (OnClientHeadersParsed != null)
                            {
                                OnClientHeadersParsed.Invoke(_cachedClientBufferHeaders);
                            }
                        }
                    }
                    if (OnClientReceiveEnd != null)
                    {
                        OnClientReceiveEnd.Invoke(ClientSocket, this, _cachedClientBufferHeaders, _cachedClientBuffer.Count == 0 ? null : _cachedClientBuffer, false);
                        //OnClientReceiveEnd.BeginInvoke(ClientSocket, this, _cachedClientBufferHeaders, _cachedClientBuffer.Count == 0 ? null : _cachedClientBuffer, false, null, null);
                    }
                    if (!Cancel)
                    {
                        DestinationSocket.BeginSend(Buffer, 0, Ret, SocketFlags.None, new AsyncCallback(OnRemoteSent), DestinationSocket);
                    }
                    else
                    {
                        Dispose();
                    }
                }
                catch
                {
                    Dispose();
                }
            }
        }

        ///<summary>Called when we have sent data to the remote host.<br>When all the data has been sent, we will start receiving again from the local client.</br></summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        protected void OnRemoteSent(IAsyncResult ar)
        {
            lock (_bufferLock)
            {
                try
                {
                    if(DestinationSocket == null)
                    {
                        return;
                    }

                    int Ret = DestinationSocket.EndSend(ar);
                    if (Ret > 0)
                    {
                        if (OnRemoteSentEnd != null)
                        {
                            //OnRemoteSentEnd.BeginInvoke(DestinationSocket, this, _cachedClientBufferHeaders, _cachedClientBuffer, false, null, null);
                            OnRemoteSentEnd.Invoke(DestinationSocket, this, _cachedClientBufferHeaders, _cachedClientBuffer.Count == 0 ? null : _cachedClientBuffer, false);
                        }
                        if (!Cancel)
                        {
                            ClientSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(OnClientReceive), ClientSocket);
                        }
                        else
                        {
                            Dispose();
                        }
                        return;
                    }
                }
                catch
                {
                }
                Dispose();
            }
        }

        ///<summary>Called when we have received data from the remote host.<br>Incoming data will immediately be forwarded to the local client.</br></summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        protected void OnRemoteReceive(IAsyncResult ar)
        {
            lock (_bufferLock)
            {
                try
                {
                    if(DestinationSocket == null)
                    {
                        return;
                    }

                    int Ret = DestinationSocket.EndReceive(ar);
                    if (Ret <= 0)
                    {
                        if (OnRemoteReceivedEnd != null)
                        {
                            if (_cachedRemoteBuffer != null)
                            {
                                //OnRemoteReceivedEnd.BeginInvoke(DestinationSocket, this, _cachedRemoteBufferHeaders, _cachedRemoteBuffer, true, null, null);
                                OnRemoteReceivedEnd.Invoke(DestinationSocket, this, _cachedRemoteBufferHeaders, _cachedRemoteBuffer.Count == 0 ? null : _cachedRemoteBuffer, true);
                            }
                            else
                            {
                                OnRemoteReceivedEnd.Invoke(DestinationSocket, this, _cachedRemoteBufferHeaders, _cachedRemoteBuffer, true);
                            }
                        }

                        if (OnClientSentEnd != null)
                        {
                            if (_cachedRemoteBufferHeaders != null)
                            {
                                //OnClientSentEnd.BeginInvoke(ClientSocket, this, _cachedRemoteBufferHeaders, _cachedRemoteBuffer, true, null, null);
                                OnClientSentEnd.Invoke(ClientSocket, this, _cachedRemoteBufferHeaders, _cachedRemoteBuffer.Count == 0 ? null : _cachedRemoteBuffer, true);
                            }
                            else
                            {
                                OnClientSentEnd.Invoke(ClientSocket, this, _cachedRemoteBufferHeaders, _cachedRemoteBuffer, true);
                            }
                        }

                        Dispose();
                        return;
                    }
                    _cachedRemoteBuffer.AddRange(RemoteBuffer);
                    if (_cacheRemoteBufferHeaders)
                    {
                        _cachedRemoteBufferHeaders = Encoding.Default.GetString(_cachedRemoteBuffer.ToArray()).TrimEnd("\0".ToCharArray());

                        int index = _cachedRemoteBufferHeaders.IndexOf("\r\n\r\n");
                        if (index != -1)
                        {
                            _cachedRemoteBufferHeaders = _cachedRemoteBufferHeaders.Substring(0, index);

                            _cacheRemoteBufferHeaders = false;
                            if (Ret != index + 4)
                            {
                                _cachedRemoteBuffer = _cachedRemoteBuffer.GetRange(index + 4, _cachedRemoteBuffer.Count - index - 4);
                            }
                            else
                            {
                                _cachedClientBuffer.Clear();
                                _cachedRemoteBuffer.Clear();
                            }

                            if (OnRemoteHeadersParsed != null)
                            {
                                OnRemoteHeadersParsed.Invoke(_cachedRemoteBufferHeaders);
                            }
                        }
                    }
                    if (OnRemoteReceivedEnd != null)
                    {
                        OnRemoteReceivedEnd.Invoke(DestinationSocket, this, _cachedRemoteBufferHeaders, _cachedRemoteBuffer.Count == 0 ? null : _cachedRemoteBuffer, false);
                        //OnRemoteReceivedEnd.BeginInvoke(DestinationSocket, this, _cachedRemoteBufferHeaders, _cachedRemoteBuffer.Count == 0 ? null : _cachedRemoteBuffer, false, null, null);
                    }
                    if (!Cancel)
                    {
                        ClientSocket.BeginSend(RemoteBuffer, 0, Ret, SocketFlags.None, new AsyncCallback(OnClientSent), ClientSocket);
                    }
                    else
                    {
                        Dispose();
                    }
                }
                catch
                {
                    Dispose();
                }
            }
        }

        ///<summary>Called when we have sent data to the local client.<br>When all the data has been sent, we will start receiving again from the remote host.</br></summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        protected void OnClientSent(IAsyncResult ar)
        {
            lock (_bufferLock)
            {
                try
                {
                    if(ClientSocket == null)
                    {
                        return;
                    }

                    int Ret = ClientSocket.EndSend(ar);
                    if (Ret > 0)
                    {
                        if (OnClientSentEnd != null)
                        {
                            //OnClientSentEnd.BeginInvoke(ClientSocket, this, _cachedRemoteBufferHeaders, _cachedRemoteBuffer, false, null, null);
                            OnClientSentEnd.Invoke(ClientSocket, this, _cachedRemoteBufferHeaders, _cachedRemoteBuffer.Count == 0 ? null : _cachedRemoteBuffer, false);
                        }
                        if (!Cancel)
                        {
                            DestinationSocket.BeginReceive(RemoteBuffer, 0, RemoteBuffer.Length, SocketFlags.None, new AsyncCallback(OnRemoteReceive), DestinationSocket);
                        }
                        else
                        {
                            Dispose();
                        }

                        return;
                    }
                }
                catch
                {
                }
                Dispose();
            }
        }

        ///<summary>Starts communication with the local client.</summary>
        public abstract void StartHandshake();

        // private variables
    }
}