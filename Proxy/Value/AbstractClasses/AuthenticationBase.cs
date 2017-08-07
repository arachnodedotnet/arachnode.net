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

#endregion

namespace Arachnode.Proxy.Value.AbstractClasses
{
    ///<summary>Defines the signature of the method to be called when the authentication is complete.</summary>
    ///<param name="Success">Specifies whether the authentication was successfull or not.</param>
    internal delegate void AuthenticationCompleteDelegate(bool Success);

    ///<summary>Authenticates a user on a SOCKS5 server according to the implemented subprotocol.</summary>
    ///<remarks>This is an abstract class. The subprotocol that's used to authenticate a user is specified in the subclasses of this base class.</remarks>
    internal abstract class AuthenticationBase
    {
        /// <summary>Holds the value of the Buffer property.</summary>
        private readonly byte[] m_Buffer = new byte[1024];

        ///<summary>The method to call when the authentication is complete.</summary>
        protected AuthenticationCompleteDelegate Callback;

        // private variables
        /// <summary>Holds the value of the Connection property.</summary>
        private Socket m_Connection;

        ///<summary>Gets or sets the Socket connection between the proxy server and the SOCKS client.</summary>
        ///<value>A Socket instance defining the connection between the proxy server and the local client.</value>
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

        ///<summary>Gets a buffer that can be used to receive data from the client connection.</summary>
        ///<value>An array of bytes that can be used to receive data from the client connection.</value>
        protected byte[] Buffer
        {
            get { return m_Buffer; }
        }

        ///<summary>Gets or sets an array of bytes that can be used to store all received data.</summary>
        ///<value>An array of bytes that can be used to store all received data.</value>
        protected byte[] Bytes { get; set; }

        ///<summary>Starts the authentication process.</summary>
        ///<remarks>This abstract method must be implemented in the subclasses, according to the selected subprotocol.</remarks>
        ///<param name="Connection">The connection with the SOCKS client.</param>
        ///<param name="Callback">The method to call when the authentication is complete.</param>
        internal abstract void StartAuthentication(Socket Connection, AuthenticationCompleteDelegate Callback);

        ///<summary>Adds bytes to the array returned by the Bytes property.</summary>
        ///<param name="NewBytes">The bytes to add.</param>
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
    }
}