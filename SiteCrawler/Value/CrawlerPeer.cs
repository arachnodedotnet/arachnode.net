#region License : arachnode.net

// // Copyright (c) 2015 http://arachnode.net, arachnode.net, LLC
// //  
// // Permission is hereby granted, upon purchase, to any person
// // obtaining a copy of this software and associated documentation
// // files (the "Software"), to deal in the Software without
// // restriction, including without limitation the rights to use,
// // copy, merge and modify copies of the Software, and to permit persons
// // to whom the Software is furnished to do so, subject to the following
// // conditions:
// // 
// // LICENSE (ALL VERSIONS/EDITIONS): http://arachnode.net/r.ashx?3
// // 
// // The above copyright notice and this permission notice shall be
// // included in all copies or substantial portions of the Software.
// // 
// // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// // EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// // OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// // NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// // HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// // WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// // FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// // OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Arachnode.DataAccess.Value.Exceptions;
using Arachnode.SiteCrawler.Value.Enums;

#endregion

namespace Arachnode.SiteCrawler.Value
{
    public class CrawlerPeer
    {
        public CrawlerPeer()
        {
            
        }

        public CrawlerPeer(IPAddress ipAddress, int remotePortNumberToSendTo, bool isLocal, bool waitForInitialCheckin, int numberOfCrawlRequestsToSendPerThreadOnCrawlRequestRequest, bool stopEngineOnCrawlCompleted)
        {
            IPAddress = ipAddress;
            RemotePortNumberToSendTo = remotePortNumberToSendTo;
            IsLocal = isLocal;
            WaitForInitialCheckin = waitForInitialCheckin;
            NumberOfCrawlRequestsToSendPerThreadOnCrawlRequestRequest = numberOfCrawlRequestsToSendPerThreadOnCrawlRequestRequest;
            StopEngineOnCrawlCompleted = stopEngineOnCrawlCompleted;

            if (!isLocal && stopEngineOnCrawlCompleted)
            {
                throw new Exception("'StopEngineOnCrawlCompleted' has no effect on non-local CrawlerPeers.");
            }

            IPEndPoint = new IPEndPoint(IPAddress, RemotePortNumberToSendTo);

            TcpClientsForSending = new Dictionary<IPEndPoint, TcpClient>();
            TcpListenersForReceiving = new Dictionary<IPEndPoint, TcpListener>();
        }

        public CrawlerPeerMessageType CrawlerPeerMessageType { get; internal set; }
        public double CacheHit { get; internal set; }
        public double CacheMiss { get; internal set; }
        
        public IPAddress IPAddress { get; internal set; }
        public int RemotePortNumberToSendTo { get; internal set; }
        public IPEndPoint IPEndPoint { get; internal set; }

        public int NumberOfExceptions { get; internal set; }
        public EngineState EngineState { get; internal set; }

        public DateTime LastTcpClientConnectAttempt { get; internal set; }
        public int LastMessageID { get; internal set; }
        public int MessagesReceived { get; internal set; }
        public int MessagesSent { get; internal set; }
        public string LastCrawlRequestResponse { get; internal set; }
        public bool HasAdditionalMessages { get; internal set; }
        
        public bool IsLocal { get; internal set; }
        public bool WaitForInitialCheckin { get; internal set; }
        public int NumberOfCrawlRequestsToSendPerThreadOnCrawlRequestRequest { get; internal set; }
        public bool StopEngineOnCrawlCompleted { get; internal set; }

        public bool RequiresInitialCheckin { get; internal set; }

        public int MaximumNumberOfCrawlThreads { get; internal set; }
        public int UncrawledCrawlRequests { get; internal set; }
        public Uri DiscoveryUri { get; internal set; }

        public Dictionary<IPEndPoint, TcpClient> TcpClientsForSending { get; internal set; }
        public Dictionary<IPEndPoint, TcpListener> TcpListenersForReceiving { get; internal set; }

        public double GetCacheHitRatio()
        {
            return (CacheHit + 1)/(CacheMiss + 1);
        }

        public override string ToString()
        {
            try
            {
                return IPAddress.ToString() + ":" + RemotePortNumberToSendTo.ToString() + ", IsLocal: " + IsLocal + ", WaitForInitialCheckin: " + WaitForInitialCheckin + ", StopEngineOnCrawlCompleted: " + StopEngineOnCrawlCompleted;
            }
            catch
            {
                return null;
            }
        }
    }
}