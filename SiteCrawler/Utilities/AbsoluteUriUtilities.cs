using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Components;
using Arachnode.SiteCrawler.Managers;

namespace Arachnode.SiteCrawler.Utilities
{
    public static class AbsoluteUriUtilities<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        public static List<string> GetOnlineAbsoluteUris(List<string> absoluteUrisToCheck, int numberOfThreads)
        {
            object absoluteUrisToCheck2Lock = new object();
            Queue<string> absoluteUrisToCheck2 = new Queue<string>(absoluteUrisToCheck);

            object onlineAbsoluteUrisLock = new object();
            List<string> onlineAbsoluteUris = new List<string>(absoluteUrisToCheck.Count);

            int numberOfCompletedThreads = 0;
            int numberOfVerifiedAbsoluteUris = 0;
            int numberOfOnlineAbsoluteUris = 0;
            int numberOfOfflineAbsoluteUris = 0;

            for(int i = 0; i < numberOfThreads; i++)
            {
                Thread thread = new Thread(() =>
                                    {
                                        TArachnodeDAO arachnodeDAO = (TArachnodeDAO)Activator.CreateInstance(typeof(TArachnodeDAO));

                                        ConsoleManager<TArachnodeDAO> consoleManager = new ConsoleManager<TArachnodeDAO>(arachnodeDAO.ApplicationSettings, arachnodeDAO.WebSettings);
                                        CookieManager cookieManager = new CookieManager();
                                        ProxyManager<TArachnodeDAO> proxyManager = new ProxyManager<TArachnodeDAO>(arachnodeDAO.ApplicationSettings, arachnodeDAO.WebSettings, consoleManager);

                                        WebClient<TArachnodeDAO> webClient = new WebClient<TArachnodeDAO>(arachnodeDAO.ApplicationSettings, arachnodeDAO.WebSettings, consoleManager, cookieManager, proxyManager);
                                        

                                        while(true)
                                        {
                                            string absoluteUri = null;

                                            lock(absoluteUrisToCheck2Lock)
                                            {
                                                if(absoluteUrisToCheck2.Count != 0)
                                                {
                                                    absoluteUri = absoluteUrisToCheck2.Dequeue();

                                                    Interlocked.Increment(ref numberOfVerifiedAbsoluteUris);
                                                    Console.WriteLine("NumberOfVerifiedAbsoluteUris: " + numberOfVerifiedAbsoluteUris);
                                                }
                                                else
                                                {
                                                    Interlocked.Increment(ref numberOfCompletedThreads);

                                                    break;
                                                }
                                            }

                                            if(!string.IsNullOrEmpty(absoluteUri))
                                            {
                                                try
                                                {
                                                    if (arachnodeDAO.ApplicationSettings.SetRefererToParentAbsoluteUri)
                                                    {
                                                        webClient.GetHttpWebResponse(absoluteUri, "GET", absoluteUri, null, null, null);
                                                    }
                                                    else
                                                    {
                                                        webClient.GetHttpWebResponse(absoluteUri, "GET", null, null, null, null);
                                                    }

                                                    lock(onlineAbsoluteUrisLock)
                                                    {
                                                        Interlocked.Increment(ref numberOfOnlineAbsoluteUris);
                                                        Console.WriteLine("NumberOfOnlineAbsoluteUris: " + numberOfOnlineAbsoluteUris);

                                                        onlineAbsoluteUris.Add(absoluteUri);
                                                    }
                                                }
                                                catch (Exception exception)
                                                {
                                                    Interlocked.Increment(ref numberOfOfflineAbsoluteUris);
                                                    Console.WriteLine("NumberOfOfflineAbsoluteUris: " + numberOfOfflineAbsoluteUris);

                                                    arachnodeDAO.InsertException(absoluteUri, absoluteUri, exception, false);
                                                }
                                            }
                                        }
                                    });

                thread.Start();
            }

            while(numberOfThreads != numberOfCompletedThreads)
            {
                Thread.Sleep(100);
            }

            return onlineAbsoluteUris;
        }
    }
}
