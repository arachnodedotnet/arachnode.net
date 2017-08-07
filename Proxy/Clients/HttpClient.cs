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
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Arachnode.Proxy.Value.AbstractClasses;
using System.Linq;

#endregion

namespace Arachnode.Proxy.Clients
{
    ///<summary>Relays HTTP data between a remote host and a local client.</summary>
    ///<remarks>This class supports both HTTP and HTTPS.</remarks>
    public sealed class HttpClient : Client
    {
#pragma warning disable 612,618
        #region Delegates

        public delegate void ConnectedEnd(Socket destination, HttpClient httpClient);

        public delegate void ErrorSentEnd(Socket destination, HttpClient httpClient);

        public delegate void OkSentEnd(Socket client, HttpClient httpClient);

        public delegate void ProcessQueryEnd(Socket destination, HttpClient httpClient);

        public delegate void QuerySentEnd(Socket destination, HttpClient httpClient);

        public delegate void ReceiveQueryEnd(Socket destination, HttpClient httpClient);

        public delegate void StartHandshakeEnd(Socket destination, HttpClient httpClient);

        #endregion

        public event StartHandshakeEnd OnStartHandshakeEnd;
        public event ProcessQueryEnd OnProcessQueryEnd;
        public event ReceiveQueryEnd OnReceiveQueryEnd;
        public event ErrorSentEnd OnErrorSentEnd;
        public event ConnectedEnd OnConnectedEnd;
        public event QuerySentEnd OnQuerySentEnd;
        public event OkSentEnd OnOkSentEnd;

        /// <summary>Holds the POST data</summary>
        private string m_HttpPost;

        /// <summary>Holds the value of the CurrentHttpQuery property.</summary>
        private string m_CurrentHttpQuery = "";

        /// <summary>Holds the value of the AllHttpQueries property.</summary>
        private List<string> m_AllHttpQueries = new List<string>();

        /// <summary>Holds the value of the HttpRequestType property.</summary>
        private string m_HttpRequestType = "";

        /// <summary>Holds the value of the HttpVersion property.</summary>
        private string m_HttpVersion = "";

        ///<summary>Initializes a new instance of the HttpClient class.</summary>
        ///<param name="ClientSocket">The <see cref ="Socket">Socket</see> connection between this proxy server and the local client.</param>
        ///<param name="Destroyer">The callback method to be called when this Client object disconnects from the local client and the remote server.</param>
        public HttpClient(Socket ClientSocket, DestroyDelegate Destroyer) : base(ClientSocket, Destroyer)
        {
            this.OnClientHeadersParsed += new ClientHeadersParsed(HttpClient_OnClientHeadersParsed);
            this.OnRemoteHeadersParsed += new RemoteHeadersParsed(HttpClient_OnRemoteHeadersParsed);
        }

        ///<summary>Gets or sets a WebHeaderCollection that stores the header fields.</summary>
        ///<value>A WebHeaderCollection that stores the header fields.</value>
        public WebHeaderCollection HeaderFields { get; set; }

        ///<summary>Gets or sets the HTTP version the client uses.</summary>
        ///<value>A string representing the requested HTTP version.</value>
        public string HttpVersion
        {
            get { return m_HttpVersion; }
            private set { m_HttpVersion = value; }
        }

        ///<summary>Gets or sets the HTTP request type.</summary>
        ///<remarks>
        ///Usually, this string is set to one of the three following values:
        ///<list type="bullet">
        ///<item>GET</item>
        ///<item>POST</item>
        ///<item>CONNECT</item>
        ///</list>
        ///</remarks>
        ///<value>A string representing the HTTP request type.</value>
        public string HttpRequestType
        {
            get { return m_HttpRequestType; }
            private set { m_HttpRequestType = value; }
        }

        ///<summary>Gets or sets the orginally requested AbsoluteUri.  Style sheets and other content may be returned in the same client instance.
        ///  This property track was the original Request was.</summary>
        ///<value>A string representing the originally requested AbsoluteUri.</value>
        public string OriginallyRequestedAbsoluteUri { get; set; }

        ///<summary>Gets or sets the requested AbsoluteUri.</summary>
        ///<value>A string representing the requested AbsoluteUri.</value>
        public string RequestedAbsoluteUri { get; set; }

        ///<summary>Gets or sets the requested path.</summary>
        ///<value>A string representing the requested path.</value>
        public string RequestedPath { get; set; }

        ///<summary>Gets or sets the query string, received from the client.</summary>
        ///<value>A string representing the HTTP query string.</value>
        public string CurrentHttpQuery
        {
            get { return m_CurrentHttpQuery; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                m_CurrentHttpQuery = value;
            }
        }

        ///<summary>Gets all query strings received from the client.</summary>
        ///<value>A list of string representing all HTTP query strings.</value>
        public List<string> AllHttpQueries
        {
            get
            {
                if(m_AllHttpQueries.Count > 1)
                {
                    
                }
                
                return m_AllHttpQueries;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                m_AllHttpQueries = value;
            }
        }

        ///<summary>Starts receiving data from the client connection.</summary>
        public override void StartHandshake()
        {
            try
            {
                ClientSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(OnReceiveQuery), ClientSocket);

                if (OnStartHandshakeEnd != null)
                {
                    OnStartHandshakeEnd.Invoke(ClientSocket, this);
                }
            }
            catch
            {
                Dispose();
            }
        }

        ///<summary>Checks whether a specified string is a valid HTTP query string.</summary>
        ///<param name="Query">The query to check.</param>
        ///<returns>True if the specified string is a valid HTTP query, false otherwise.</returns>
        private bool IsValidQuery(string Query)
        {
            int index = Query.IndexOf("\r\n\r\n");
            if (index == -1)
            {
                return false;
            }
            HeaderFields = ParseQuery(Query);
            if (HttpRequestType.ToUpper().Equals("POST"))
            {
                try
                {
                    int length = int.Parse(HeaderFields["Content-Length"]);
                    return Query.Length >= index + 6 + length;
                }
                catch
                {
                    SendBadRequest();
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        public bool DoHeadersContainKey(WebHeaderCollection webHeaderCollection, string key)
        {
            if(webHeaderCollection == null || !webHeaderCollection.HasKeys() || string.IsNullOrEmpty(key))
            {
                return false;
            }

            foreach(string key2 in webHeaderCollection.Keys)
            {
                if(key2.ToLowerInvariant() == key.ToLowerInvariant())
                {
                    return true;
                }
            }

            return false;
        }

        ///<summary>Processes a specified query and connects to the requested HTTP web server.</summary>
        ///<param name="Query">A string containing the query to process.</param>
        ///<remarks>If there's an error while processing the HTTP request or when connecting to the remote server, the Proxy sends a "400 - Bad Request" error to the client.</remarks>
        private void ProcessQuery(string Query)
        {
            HeaderFields = ParseQuery(Query);
            if (HeaderFields == null || !HeaderFields.HasKeys() || !DoHeadersContainKey(HeaderFields, "Host"))
            {
                SendBadRequest();
                return;
            }

            if (OnProcessQueryEnd != null)
            {
                OnProcessQueryEnd.Invoke(DestinationSocket, this);
            }
            if(Cancel)
            {
                SendCancelledResponse();
                return;
            }

            int Port;
            string Host;
            int Ret;
            if (HttpRequestType.ToUpper().Equals("CONNECT"))
            {
                //HTTPS
                Ret = RequestedPath.IndexOf(":");
                if (Ret >= 0)
                {
                    Host = RequestedPath.Substring(0, Ret);
                    if (RequestedPath.Length > Ret + 1)
                    {
                        Port = int.Parse(RequestedPath.Substring(Ret + 1));
                    }
                    else
                    {
                        Port = 443;
                    }
                }
                else
                {
                    Host = RequestedPath;
                    Port = 443;
                }
            }
            else
            {
                //Normal HTTP
                Ret = (HeaderFields["Host"]).IndexOf(":");
                if (Ret > 0)
                {
                    Host = (HeaderFields["Host"]).Substring(0, Ret);
                    Port = int.Parse((HeaderFields["Host"]).Substring(Ret + 1));
                }
                else
                {
                    Host = HeaderFields["Host"];
                    Port = 80;
                }
                if (HttpRequestType.ToUpper().Equals("POST"))
                {
                    int index = Query.IndexOf("\r\n\r\n");
                    m_HttpPost = Query.Substring(index + 4);
                }
            }
            try
            {
                IPEndPoint DestinationEndPoint = new IPEndPoint(Dns.Resolve(Host).AddressList[0], Port);
                DestinationSocket = new Socket(DestinationEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                if (DoHeadersContainKey(HeaderFields, "Proxy-Connection") && HeaderFields["Proxy-Connection"].ToLower().Equals("keep-alive"))
                {
                    //DestinationSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, 1);
                }
                DestinationSocket.BeginConnect(DestinationEndPoint, OnConnected, DestinationSocket);
            }
            catch
            {
                SendBadRequest();
                return;
            }
        }

        ///<summary>Parses a specified HTTP query into its header fields.</summary>
        ///<param name="query">The HTTP query string to parse.</param>
        ///<returns>A WebHeaderCollection object containing all the header fields with their data.</returns>
        ///<exception cref="ArgumentNullException">The specified query is null.</exception>
        public WebHeaderCollection ParseQuery(string query)
        {
            string[] lines = query.Replace("\r\n", "\n").Split('\n');
            int index;
            //Extract requested URL
            if (lines.Length > 0)
            {
                //Parse the Http Request Type
                index = lines[0].IndexOf(' ');
                if (index > 0)
                {
                    HttpRequestType = lines[0].Substring(0, index);
                    lines[0] = lines[0].Substring(index).Trim();
                }
                //Parse the Http Version and the Requested Path
                index = lines[0].LastIndexOf(' ');
                if (index > 0)
                {
                    HttpVersion = lines[0].Substring(index).Trim();
                    RequestedPath = lines[0].Substring(0, index);
                }
                else
                {
                    RequestedPath = lines[0];
                }
                
                if (string.IsNullOrEmpty(OriginallyRequestedAbsoluteUri))
                {
                    OriginallyRequestedAbsoluteUri = new Uri(RequestedPath).AbsoluteUri;
                }
                if (string.IsNullOrEmpty(RequestedAbsoluteUri))
                {
                    RequestedAbsoluteUri = new Uri(RequestedPath).AbsoluteUri;
                }
                //Remove http:// if present
                if (RequestedPath.Length >= 7 && RequestedPath.Substring(0, 7).ToLower().Equals("http://"))
                {
                    index = RequestedPath.IndexOf('/', 7);
                    if (index == -1)
                    {
                        RequestedPath = "/";
                    }
                    else
                    {
                        RequestedPath = RequestedPath.Substring(index);
                    }
                }
            }

            return ParseHeaders(lines, false);
        }

        public WebHeaderCollection ParseHeaders(string query, bool addAbsoluteUriHeader)
        {
            string[] lines = query.Replace("\r\n", "\n").Split('\n');

            return ParseHeaders(lines, addAbsoluteUriHeader);
        }

        public WebHeaderCollection ParseHeaders(string[] lines, bool addStatusCodeHeader)
        {
            WebHeaderCollection webHeaderCollection = new WebHeaderCollection();
            int index;
            int count = 1;

            if (addStatusCodeHeader)
            {
                if (!lines[0].ToLowerInvariant().StartsWith("http"))
                {
                    //remove the request type...
                    index = lines[0].IndexOf(' ');
                    if (index != -1)
                    {
                        //lines[0] = lines[0].Substring(index).Trim();
                    }

                    //remove the protocol value...
                    index = lines[0].LastIndexOf(' ');
                    if (index != -1)
                    {
                        //lines[0] = lines[0].Substring(0, index).Trim();
                    }
                    if (string.IsNullOrEmpty(OriginallyRequestedAbsoluteUri))
                    {
                        OriginallyRequestedAbsoluteUri = lines[0];
                    }
                    RequestedAbsoluteUri = lines[0];
                    lines[0] = "Instructions: " + lines[0];
                }
                else
                {
                    lines[0] = "StatusCode: " + lines[0];
                }

                count--;
            }

            for (;count < lines.Length; count++)
            {
                index = lines[count].IndexOf(":");
                if (index > 0 && index < lines[count].Length - 1)
                {
                    try
                    {
                        webHeaderCollection.Add(lines[count].Substring(0, index), lines[count].Substring(index + 1).Trim());
                    }
                    catch
                    {
                    }
                }
            }

            try
            {
                webHeaderCollection["User-Agent"] = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";
            }
            catch (Exception)
            {
            }
            

            return webHeaderCollection;
        }

        ///<summary>Sends a "400 - Bad Request" error to the client.</summary>
        private void SendBadRequest()
        {
            string brs = "HTTP/1.1 400 Bad Request\r\nConnection: close\r\nContent-Type: text/html\r\n\r\n<html><head><title>400 Bad Request</title></head><body><div align=\"center\"><table border=\"0\" cellspacing=\"3\" cellpadding=\"3\" bgcolor=\"#C0C0C0\"><tr><td><table border=\"0\" width=\"500\" cellspacing=\"3\" cellpadding=\"3\"><tr><td bgcolor=\"#B2B2B2\"><p align=\"center\"><strong><font size=\"2\" face=\"Verdana\">400 Bad Request</font></strong></p></td></tr><tr><td bgcolor=\"#D1D1D1\"><font size=\"2\" face=\"Verdana\"> The proxy server could not understand the HTTP request!<br><br> Please contact your network administrator about this problem.</font></td></tr></table></center></td></tr></table></div></body></html>";
            try
            {
                ClientSocket.BeginSend(Encoding.ASCII.GetBytes(brs), 0, brs.Length, SocketFlags.None, new AsyncCallback(OnErrorSent), ClientSocket);
            }
            catch
            {
                Dispose();
            }
        }

        ///<summary>Sends a "200 - Head Response" error to the client.</summary>
        private void SendCancelledResponse()
        {
            string brs = "HTTP/1.1 200 Cancelled Response\r\nConnection: close\r\nContent-Type: text/html\r\n\r\n<html><head><title>200 Proxy Cancelled Response</title></head><body><div align=\"center\"><table border=\"0\" cellspacing=\"3\" cellpadding=\"3\" bgcolor=\"#C0C0C0\"><tr><td><table border=\"0\" width=\"500\" cellspacing=\"3\" cellpadding=\"3\"><tr><td bgcolor=\"#B2B2B2\"><p align=\"center\"><strong><font size=\"2\" face=\"Verdana\">400 Bad Request</font></strong></p></td></tr><tr><td bgcolor=\"#D1D1D1\"><font size=\"2\" face=\"Verdana\"> The proxy server cancelled the HTTP request!<br><br> Please contact your network administrator about this problem.</font></td></tr></table></center></td></tr></table></div></body></html>";
            try
            {
                ClientSocket.BeginSend(Encoding.ASCII.GetBytes(brs), 0, brs.Length, SocketFlags.None, null, ClientSocket);
            }
            catch
            {
                Dispose();
            }
        }

        ///<summary>Sends a "200 - Head Response" error to the client.</summary>
        private void SendHeadResponse()
        {
            string brs = _cachedRemoteBufferHeaders + "\r\n\r\n";
            try
            {
                ClientSocket.BeginSend(Encoding.ASCII.GetBytes(brs), 0, brs.Length, SocketFlags.None, null, ClientSocket);
            }
            catch
            {
                Dispose();
            }
        }

        ///<summary>Rebuilds the HTTP query, starting from the HttpRequestType, RequestedPath, HttpVersion and HeaderFields properties.</summary>
        ///<returns>A string representing the rebuilt HTTP query string.</returns>
        private string RebuildQuery()
        {
            string ret = HttpRequestType + " " + RequestedPath + " " + HttpVersion + "\r\n";
            if (HeaderFields != null)
            {
                foreach (string sc in HeaderFields.Keys)
                {
                    if (sc.Length < 6 || !sc.Substring(0, 6).Equals("proxy-"))
                    {
                        ret += CultureInfo.CurrentCulture.TextInfo.ToTitleCase(sc) + ": " + HeaderFields[sc] + "\r\n";
                    }
                }
                ret += "\r\n";
                if (m_HttpPost != null)
                {
                    ret += m_HttpPost;
                }
            }
            return ret;
        }

        ///<summary>Called when we received some data from the client connection.</summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        private void OnReceiveQuery(IAsyncResult ar)
        {
            int Ret;
            try
            {
                Ret = ClientSocket.EndReceive(ar);
            }
            catch
            {
                Ret = -1;
            }
            if (Ret <= 0)
            {
                //Connection is dead :(
                Dispose();
                return;
            }
            CurrentHttpQuery = Encoding.ASCII.GetString(Buffer, 0, Ret);
            //Console.WriteLine("\n" + CurrentHttpQuery + "\n");
            if (!AllHttpQueries.Contains(CurrentHttpQuery))
            {
                AllHttpQueries.Add(CurrentHttpQuery);
            }

            if (OnReceiveQueryEnd != null)
            {
                OnReceiveQueryEnd.Invoke(ClientSocket, this);
            }
            if (Cancel)
            {
                SendCancelledResponse();
                return;
            }

            try
            {
                //if received data is valid HTTP request...
                if (IsValidQuery(CurrentHttpQuery))
                {
                    ProcessQuery(CurrentHttpQuery);
                    //else, keep listening
                }
                else
                {
                    try
                    {
                        ClientSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(OnReceiveQuery), ClientSocket);
                    }
                    catch
                    {
                        Dispose();
                    }
                }                
            }
            catch
            {
            }
        }

        ///<summary>Called when the Bad Request error has been sent to the client.</summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        private void OnErrorSent(IAsyncResult ar)
        {
            try
            {
                ClientSocket.EndSend(ar);
            }
            catch
            {
            }
            finally
            {
                if (OnErrorSentEnd != null)
                {
                    OnErrorSentEnd.Invoke(ClientSocket, this);
                }
            }
            Dispose();
        }

        ///<summary>Called when we're connected to the requested remote host.</summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        private void OnConnected(IAsyncResult ar)
        {
            try
            {
                DestinationSocket.EndConnect(ar);

                if (OnConnectedEnd != null)
                {
                    OnConnectedEnd.Invoke(DestinationSocket, this);
                }
                if(Cancel)
                {
                    SendCancelledResponse();
                    return;
                }

                string rq;
                if (HttpRequestType.ToUpper().Equals("CONNECT"))
                {
                    //HTTPS
                    rq = HttpVersion + " 200 Connection established\r\nProxy-Agent: arachnode.net Proxy Server\r\n\r\n";
                    ClientSocket.BeginSend(Encoding.ASCII.GetBytes(rq), 0, rq.Length, SocketFlags.None, new AsyncCallback(OnOkSent), ClientSocket);
                }
                else
                {
                    //Normal HTTP
                    rq = RebuildQuery();
                    Console.WriteLine(rq);
                    DestinationSocket.BeginSend(Encoding.ASCII.GetBytes(rq), 0, rq.Length, SocketFlags.None, new AsyncCallback(OnQuerySent), DestinationSocket);
                }
            }
            catch
            {
                Dispose();
            }
        }

        ///<summary>Called when the HTTP query has been sent to the remote host.</summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        private void OnQuerySent(IAsyncResult ar)
        {
            try
            {
                if (DestinationSocket.EndSend(ar) == -1)
                {
                    Dispose();
                    return;
                }
                if (OnQuerySentEnd != null)
                {
                    OnQuerySentEnd.Invoke(DestinationSocket, this);
                }
                if (Cancel)
                {
                    SendCancelledResponse();
                    return;
                }
                StartRelay();
            }
            catch
            {
                Dispose();
            }
        }

        ///<summary>Called when an OK reply has been sent to the local client.</summary>
        ///<param name="ar">The result of the asynchronous operation.</param>
        private void OnOkSent(IAsyncResult ar)
        {
            try
            {
                if (ClientSocket.EndSend(ar) == -1)
                {
                    Dispose();
                    return;
                }
                StartRelay();
                if (OnOkSentEnd != null)
                {
                    OnOkSentEnd.Invoke(ClientSocket, this);
                }
            }
            catch
            {
                Dispose();
            }
        }

        void HttpClient_OnClientHeadersParsed(string headers)
        {
            //check to see that a request wasn't sent to a socket that would have no way of servicing the request...
            try
            {
                WebHeaderCollection webHeaderCollection = ParseHeaders(headers, false);

                if (DoHeadersContainKey(webHeaderCollection, "Host"))
                {
                    IPEndPoint destinationEndPoint = new IPEndPoint(Dns.Resolve(webHeaderCollection["Host"]).AddressList[0], 80);

                    if (DestinationSocket.RemoteEndPoint.ToString() != destinationEndPoint.ToString())
                    {
                        Dispose();
                    }
                }    
            }
            catch
            {
            }
        }

        void HttpClient_OnRemoteHeadersParsed(string headers)
        {
            if (DoHeadersContainKey(HeaderFields, "Proxy-Intended-Method"))
            {
                if (HeaderFields["Proxy-Intended-Method"] == "HEAD")
                {
                    SendHeadResponse();
                    return;
                }
            }
        }

        ///<summary>Returns text information about this HttpClient object.</summary>
        ///<returns>A string representing this HttpClient object.</returns>
        public override string ToString()
        {
            return ToString(false);
        }

        ///<summary>Returns text information about this HttpClient object.</summary>
        ///<returns>A string representing this HttpClient object.</returns>
        ///<param name="WithUrl">Specifies whether or not to include information about the requested URL.</param>
        public string ToString(bool WithUrl)
        {
            string Ret;
            try
            {
                if (DestinationSocket == null || DestinationSocket.RemoteEndPoint == null)
                {
                    Ret = "Incoming HTTP connection from " + ((IPEndPoint)ClientSocket.RemoteEndPoint).Address;
                }
                else
                {
                    Ret = "HTTP connection from " + ((IPEndPoint)ClientSocket.RemoteEndPoint).Address + " to " + ((IPEndPoint)DestinationSocket.RemoteEndPoint).Address + " on port " + ((IPEndPoint)DestinationSocket.RemoteEndPoint).Port;
                }
                if (HeaderFields != null && DoHeadersContainKey(HeaderFields, "Host") && RequestedPath != null)
                {
                    Ret += " requested URL: http://" + HeaderFields["Host"] + " : " + RequestedPath;
                }
            }
            catch
            {
                Ret = "HTTP Connection";
            }
            return Ret;
        }
    }
#pragma warning restore 612,618
}