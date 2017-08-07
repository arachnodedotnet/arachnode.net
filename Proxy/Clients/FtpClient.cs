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
using Arachnode.Proxy.Connections;
using Arachnode.Proxy.Value.AbstractClasses;

#endregion

namespace Arachnode.Proxy.Clients
{
    ///<summary>Relays FTP commands between a remote host and a local client.</summary>
    ///<remarks>This class supports the 'OPEN' command, 'USER user@host:port' and 'USER user@host port'.</remarks>
    public sealed class FtpClient : Client
    {
#pragma warning disable 612,618
        /// <summary>Holds the value of the FtpCommand property.</summary>
        private string m_FtpCommand = "";

        /// <summary>Holds the value of the FtpReply property.</summary>
        private string m_FtpReply = "";

        /// <summary>Holds the value of the User property.</summary>
        private string m_User = "";

        ///<summary>Initializes a new instance of the FtpClient class.</summary>
        ///<param name="ClientSocket">The Socket connection between this proxy server and the local client.</param>
        ///<param name="Destroyer">The callback method to be called when this Client object disconnects from the local client and the remote server.</param>
        public FtpClient(Socket ClientSocket, DestroyDelegate Destroyer) : base(ClientSocket, Destroyer)
        {
        }

        ///<summary>Gets or sets a property that can be used to store the received FTP command.</summary>
        ///<value>A string that can be used to store the received FTP command.</value>
        private string FtpCommand
        {
            get { return m_FtpCommand; }
            set { m_FtpCommand = value; }
        }

        ///<summary>Gets or sets a property that can be used to store the received FTP reply.</summary>
        ///<value>A string that can be used to store the received FTP reply.</value>
        private string FtpReply
        {
            get { return m_FtpReply; }
            set { m_FtpReply = value; }
        }

        ///<summary>Gets or sets a string containing the logged on username.</summary>
        ///<value>A string containing the logged on username.</value>
        private string User
        {
            get { return m_User; }
            set { m_User = value; }
        }

        ///<summary>Gets or sets the dataconnection object used to transmit files and other data from and to the FTP server.</summary>
        ///<value>An FtpDataConnection object that's used to transmit files and other data from and to the FTP server.</value>
        internal FtpDataConnection DataForward { get; set; }

        ///<summary>Sends a welcome message to the client.</summary>
        public override void StartHandshake()
        {
            try
            {
                string ToSend = "220 Mentalis.org FTP proxy server ready.\r\n";
                ClientSocket.BeginSend(Encoding.ASCII.GetBytes(ToSend), 0, ToSend.Length, SocketFlags.None, new AsyncCallback(OnHelloSent), ClientSocket);
            }
            catch
            {
                Dispose();
            }
        }

        ///<summary>Called when the welcome message has been sent to the client.</summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        private void OnHelloSent(IAsyncResult ar)
        {
            try
            {
                if (ClientSocket.EndSend(ar) <= 0)
                {
                    Dispose();
                    return;
                }
                ClientSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(OnReceiveCommand), ClientSocket);
            }
            catch
            {
                Dispose();
            }
        }

        ///<summary>Called when we have received some bytes from the client.</summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        private void OnReceiveCommand(IAsyncResult ar)
        {
            try
            {
                int Ret = ClientSocket.EndReceive(ar);
                string Command;
                if (Ret <= 0)
                {
                    Dispose();
                    return;
                }
                FtpCommand += Encoding.ASCII.GetString(Buffer, 0, Ret);
                if (IsValidCommand(FtpCommand))
                {
                    Command = FtpCommand;
                    if (ProcessCommand(Command))
                    {
                        DestinationSocket.BeginSend(Encoding.ASCII.GetBytes(Command), 0, Command.Length, SocketFlags.None, new AsyncCallback(OnCommandSent), DestinationSocket);
                    }
                    FtpCommand = "";
                }
                else
                {
                    ClientSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(OnReceiveCommand), ClientSocket);
                }
            }
            catch
            {
                Dispose();
            }
        }

        ///<summary>Processes an FTP command, sent from the client.</summary>
        ///<param name="Command">The command to process.</param>
        ///<returns>True if the command may be sent to the server, false otherwise.</returns>
        private bool ProcessCommand(string Command)
        {
            try
            {
                int Ret = Command.IndexOf(' ');
                if (Ret < 0)
                {
                    Ret = Command.Length;
                }
                switch (Command.Substring(0, Ret).ToUpper().Trim())
                {
                    case "OPEN":
                        ConnectTo(ParseIPPort(Command.Substring(Ret + 1)));
                        break;
                    case "USER":
                        Ret = Command.IndexOf('@');
                        if (Ret < 0)
                        {
                            return true;
                        }
                        else
                        {
                            User = Command.Substring(0, Ret).Trim() + "\r\n";
                            ConnectTo(ParseIPPort(Command.Substring(Ret + 1)));
                        }
                        break;
                    case "PORT":
                        ProcessPortCommand(Command.Substring(5).Trim());
                        break;
                    case "PASV":
                        DataForward = new FtpDataConnection();
                        DataForward.ProcessPasv(this);
                        return true;
                    default:
                        return true;
                }
                return false;
            }
            catch
            {
                Dispose();
                return false;
            }
        }

        ///<summary>Processes a PORT command, sent from the client.</summary>
        ///<param name="Input">The parameters of the PORT command.</param>
        private void ProcessPortCommand(string Input)
        {
            try
            {
                string[] Parts = Input.Split(',');
                if (Parts.Length == 6)
                {
                    DataForward = new FtpDataConnection();
                    string Reply = DataForward.ProcessPort(new IPEndPoint(IPAddress.Parse(String.Join(".", Parts, 0, 4)), int.Parse(Parts[4])*256 + int.Parse(Parts[5])));
                    DestinationSocket.BeginSend(Encoding.ASCII.GetBytes(Reply), 0, Reply.Length, SocketFlags.None, new AsyncCallback(OnCommandSent), DestinationSocket);
                }
            }
            catch
            {
                Dispose();
            }
        }

        ///<summary>Parses an IP address and port from a specified input string.</summary>
        ///<remarks>The input string is of the following form:<br>  <c>HOST:PORT</c></br><br><em>or</em></br><br>  <c>HOST PORT</c></br></remarks>
        ///<param name="Input">The string to parse.</param>
        ///<returns>An instance of the IPEndPoint class if successful, null otherwise.</returns>
        private IPEndPoint ParseIPPort(string Input)
        {
            Input = Input.Trim();
            int Ret = Input.IndexOf(':');
            if (Ret < 0)
            {
                Ret = Input.IndexOf(' ');
            }
            try
            {
                if (Ret > 0)
                {
                    return new IPEndPoint(Dns.Resolve(Input.Substring(0, Ret)).AddressList[0], int.Parse(Input.Substring(Ret + 1)));
                }
                else
                {
                    return new IPEndPoint(Dns.Resolve(Input).AddressList[0], 21);
                }
            }
            catch
            {
                return null;
            }
        }

        ///<summary>Connects to the specified endpoint.</summary>
        ///<param name="RemoteServer">The IPEndPoint to connect to.</param>
        ///<exception cref="SocketException">There was an error connecting to the specified endpoint</exception>
        private void ConnectTo(IPEndPoint RemoteServer)
        {
            if (DestinationSocket != null)
            {
                try
                {
                    DestinationSocket.Shutdown(SocketShutdown.Both);
                }
                catch
                {
                }
                finally
                {
                    DestinationSocket.Close();
                }
            }
            try
            {
                DestinationSocket = new Socket(RemoteServer.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                DestinationSocket.BeginConnect(RemoteServer, OnRemoteConnected, DestinationSocket);
            }
            catch
            {
                throw new SocketException();
            }
        }

        ///<summary>Called when we're connected to the remote FTP server.</summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        private void OnRemoteConnected(IAsyncResult ar)
        {
            try
            {
                DestinationSocket.EndConnect(ar);
                ClientSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(OnReceiveCommand), ClientSocket);
                if (User.Equals(""))
                {
                    DestinationSocket.BeginReceive(RemoteBuffer, 0, RemoteBuffer.Length, SocketFlags.None, new AsyncCallback(OnReplyReceived), DestinationSocket);
                }
                else
                {
                    DestinationSocket.BeginReceive(RemoteBuffer, 0, RemoteBuffer.Length, SocketFlags.None, new AsyncCallback(OnIgnoreReply), DestinationSocket);
                }
            }
            catch
            {
                Dispose();
            }
        }

        ///<summary>Called when we receive a reply from the FTP server that should be ignored.</summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        private void OnIgnoreReply(IAsyncResult ar)
        {
            try
            {
                int Ret = DestinationSocket.EndReceive(ar);
                if (Ret <= 0)
                {
                    Dispose();
                    return;
                }
                FtpReply += Encoding.ASCII.GetString(RemoteBuffer, 0, Ret);
                if (IsValidReply(FtpReply))
                {
                    DestinationSocket.BeginReceive(RemoteBuffer, 0, RemoteBuffer.Length, SocketFlags.None, new AsyncCallback(OnReplyReceived), DestinationSocket);
                    DestinationSocket.BeginSend(Encoding.ASCII.GetBytes(User), 0, User.Length, SocketFlags.None, new AsyncCallback(OnCommandSent), DestinationSocket);
                }
                else
                {
                    DestinationSocket.BeginReceive(RemoteBuffer, 0, RemoteBuffer.Length, SocketFlags.None, new AsyncCallback(OnIgnoreReply), DestinationSocket);
                }
            }
            catch
            {
                Dispose();
            }
        }

        ///<summary>Called when an FTP command has been successfully sent to the FTP server.</summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        private void OnCommandSent(IAsyncResult ar)
        {
            try
            {
                if (DestinationSocket.EndSend(ar) <= 0)
                {
                    Dispose();
                    return;
                }
                ClientSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(OnReceiveCommand), ClientSocket);
            }
            catch
            {
                Dispose();
            }
        }

        ///<summary>Called when we receive a reply from the FTP server.</summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        private void OnReplyReceived(IAsyncResult ar)
        {
            try
            {
                int Ret = DestinationSocket.EndReceive(ar);
                if (Ret <= 0)
                {
                    Dispose();
                    return;
                }
                if (DataForward != null && DataForward.ExpectsReply)
                {
                    if (!DataForward.ProcessPasvReplyRecv(Encoding.ASCII.GetString(RemoteBuffer, 0, Ret)))
                    {
                        DestinationSocket.BeginReceive(RemoteBuffer, 0, RemoteBuffer.Length, SocketFlags.None, new AsyncCallback(OnReplyReceived), DestinationSocket);
                    }
                }
                else
                {
                    ClientSocket.BeginSend(RemoteBuffer, 0, Ret, SocketFlags.None, new AsyncCallback(OnReplySent), ClientSocket);
                }
            }
            catch
            {
                Dispose();
            }
        }

        ///<summary>Called when the reply from the FTP server has been sent to the local FTP client.</summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        private void OnReplySent(IAsyncResult ar)
        {
            try
            {
                int Ret = ClientSocket.EndSend(ar);
                if (Ret <= 0)
                {
                    Dispose();
                    return;
                }
                DestinationSocket.BeginReceive(RemoteBuffer, 0, RemoteBuffer.Length, SocketFlags.None, new AsyncCallback(OnReplyReceived), DestinationSocket);
            }
            catch
            {
                Dispose();
            }
        }

        ///<summary>Sends a string to the local FTP client.</summary>
        ///<param name="Command">The result of the asynchronous operation.</param>
        internal void SendCommand(string Command)
        {
            ClientSocket.BeginSend(Encoding.ASCII.GetBytes(Command), 0, Command.Length, SocketFlags.None, new AsyncCallback(OnReplySent), ClientSocket);
        }

        ///<summary>Checks whether a specified command is a complete FTP command or not.</summary>
        ///<param name="Command">A string containing the command to check.</param>
        ///<returns>True if the command is complete, false otherwise.</returns>
        internal static bool IsValidCommand(string Command)
        {
            return (Command.IndexOf("\r\n") >= 0);
        }

        ///<summary>Checks whether a specified reply is a complete FTP reply or not.</summary>
        ///<param name="Input">A string containing the reply to check.</param>
        ///<returns>True is the reply is complete, false otherwise.</returns>
        internal static bool IsValidReply(string Input)
        {
            string[] Lines = Input.Split('\n');
            try
            {
                if (Lines[Lines.Length - 2].Trim().Substring(3, 1).Equals(" "))
                {
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        ///<summary>Returns text information about this FtpClient object.</summary>
        ///<returns>A string representing this FtpClient object.</returns>
        public override string ToString()
        {
            try
            {
                return "FTP connection from " + ((IPEndPoint) ClientSocket.RemoteEndPoint).Address + " to " + ((IPEndPoint) DestinationSocket.RemoteEndPoint).Address;
            }
            catch
            {
                return "Incoming FTP connection";
            }
        }

        // private variables
    }
#pragma warning restore 612,618
}