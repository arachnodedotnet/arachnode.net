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

namespace Arachnode.Proxy.Connections
{
    ///<summary>Relays FTP data between a remote host and a local client.</summary>
    internal sealed class FtpDataConnection : Client
    {
        /// <summary>Holds the value of the FtpReply property.</summary>
        private string m_FtpReply = "";

        /// <summary>Holds the value of the ListenSocket property.</summary>
        private Socket m_ListenSocket;

        ///<summary>Gets or sets the Socket that's used to listen for incoming connections.</summary>
        ///<value>A Socket that's used to listen for incoming connections.</value>
        private Socket ListenSocket
        {
            get { return m_ListenSocket; }
            set
            {
                if (m_ListenSocket != null)
                {
                    m_ListenSocket.Close();
                }
                m_ListenSocket = value;
            }
        }

        ///<summary>Gets or sets the parent of this FtpDataConnection.</summary>
        ///<value>The FtpClient object that's the parent of this FtpDataConnection object.</value>
        private FtpClient Parent { get; set; }

        ///<summary>Gets or sets a string that stores the reply that has been sent from the remote FTP server.</summary>
        ///<value>A string that stores the reply that has been sent from the remote FTP server.</value>
        private string FtpReply
        {
            get { return m_FtpReply; }
            set { m_FtpReply = value; }
        }

        ///<summary>Gets or sets a boolean value that indicates whether the FtpDataConnection expects a reply from the remote FTP server or not.</summary>
        ///<value>A boolean value that indicates whether the FtpDataConnection expects a reply from the remote FTP server or not.</value>
        internal bool ExpectsReply { get; set; }

        ///<summary>Initializes a new instance of the FtpDataConnection class.</summary>
        ///<param name="RemoteAddress">The address on the local FTP client to connect to.</param>
        ///<returns>The PORT command string to send to the FTP server.</returns>
        public string ProcessPort(IPEndPoint RemoteAddress)
        {
            try
            {
                ListenSocket = new Socket(IPAddress.Any.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                ListenSocket.Bind(new IPEndPoint(IPAddress.Any, 0));
                ListenSocket.Listen(1);
                ListenSocket.BeginAccept(new AsyncCallback(OnPortAccept), ListenSocket);
                ClientSocket = new Socket(RemoteAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                ClientSocket.BeginConnect(RemoteAddress, OnPortConnected, ClientSocket);
                return "PORT " + Listener.GetLocalExternalIP().ToString().Replace('.', ',') + "," + Math.Floor((decimal) ((IPEndPoint) ListenSocket.LocalEndPoint).Port/256) + "," + (((IPEndPoint) ListenSocket.LocalEndPoint).Port%256) + "\r\n";
            }
            catch
            {
                Dispose();
                return "PORT 0,0,0,0,0,0\r\n";
            }
        }

        ///<summary>Called when we're connected to the data port on the local FTP client.</summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        private void OnPortConnected(IAsyncResult ar)
        {
            try
            {
                ClientSocket.EndConnect(ar);
                StartHandshake();
            }
            catch
            {
                Dispose();
            }
        }

        ///<summary>Called when there's a connection from the remote FTP server waiting to be accepted.</summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        private void OnPortAccept(IAsyncResult ar)
        {
            try
            {
                DestinationSocket = ListenSocket.EndAccept(ar);
                ListenSocket.Close();
                ListenSocket = null;
                StartHandshake();
            }
            catch
            {
                Dispose();
            }
        }

        ///<summary>Starts relaying data between the remote FTP server and the local FTP client.</summary>
        public override void StartHandshake()
        {
            if (DestinationSocket != null && ClientSocket != null && DestinationSocket.Connected && ClientSocket.Connected)
            {
                StartRelay();
            }
        }

        ///<summary>Called when the proxy server processes a PASV command.</summary>
        ///<param name="Parent">The parent FtpClient object.</param>
        public void ProcessPasv(FtpClient Parent)
        {
            this.Parent = Parent;
            ExpectsReply = true;
        }

        ///<summary>Called when the FtpClient receives a reply on the PASV command from the server.</summary>
        ///<param name="Input">The received reply.</param>
        ///<returns>True if the input has been processed successfully, false otherwise.</returns>
        internal bool ProcessPasvReplyRecv(string Input)
        {
            FtpReply += Input;
            if (FtpClient.IsValidReply(FtpReply))
            {
                ExpectsReply = false;
                ProcessPasvReply(FtpReply);
                FtpReply = "";
                return true;
            }
            return false;
        }

        ///<summary>Processes a PASV reply from the server.</summary>
        ///<param name="Reply">The reply to process.</param>
        private void ProcessPasvReply(string Reply)
        {
            try
            {
                IPEndPoint ConnectTo = ParsePasvIP(Reply);
                DestinationSocket = new Socket(ConnectTo.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                DestinationSocket.BeginConnect(ConnectTo, OnPasvConnected, DestinationSocket);
            }
            catch
            {
                Dispose();
            }
        }

        ///<summary>Parses a PASV reply into an instance of the IPEndPoint class.</summary>
        ///<param name="Reply">The reply to parse into an IPEndPoint.</param>
        ///<returns>An instance of the IPEndPoint class when successful, null otherwise.</returns>
        private IPEndPoint ParsePasvIP(string Reply)
        {
            int StartIndex, StopIndex;
            string IPString;
            StartIndex = Reply.IndexOf("(");
            if (StartIndex == -1)
            {
                return null;
            }
            else
            {
                StopIndex = Reply.IndexOf(")", StartIndex);
                if (StopIndex == -1)
                {
                    return null;
                }
                else
                {
                    IPString = Reply.Substring(StartIndex + 1, StopIndex - StartIndex - 1);
                }
            }
            string[] Parts = IPString.Split(',');
            if (Parts.Length == 6)
            {
                return new IPEndPoint(IPAddress.Parse(String.Join(".", Parts, 0, 4)), int.Parse(Parts[4])*256 + int.Parse(Parts[5]));
            }
            else
            {
                return null;
            }
        }

        ///<summary>Called when we're connected to the data port of the remote FTP server.</summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        private void OnPasvConnected(IAsyncResult ar)
        {
            try
            {
                DestinationSocket.EndConnect(ar);
                ListenSocket = new Socket(IPAddress.Any.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                ListenSocket.Bind(new IPEndPoint(IPAddress.Any, 0));
                ListenSocket.Listen(1);
                ListenSocket.BeginAccept(new AsyncCallback(OnPasvAccept), ListenSocket);
                Parent.SendCommand("227 Entering Passive Mode (" + Listener.GetLocalInternalIP().ToString().Replace('.', ',') + "," + Math.Floor((decimal) ((IPEndPoint) ListenSocket.LocalEndPoint).Port/256) + "," + (((IPEndPoint) ListenSocket.LocalEndPoint).Port%256) + ").\r\n");
            }
            catch
            {
                Dispose();
            }
        }

        ///<summary>Called when there's a connection from the local FTP client waiting to be accepted.</summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        private void OnPasvAccept(IAsyncResult ar)
        {
            try
            {
                ClientSocket = ListenSocket.EndAccept(ar);
                StartHandshake();
            }
            catch
            {
                Dispose();
            }
        }

        // private variables
    }
}