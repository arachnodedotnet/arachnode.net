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
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Caching;

#endregion

namespace Arachnode.Cache
{
    public class DistributedCache
    {
        private static readonly object BinaryFormatterForReadLock = new object();
        private static readonly object BinaryFormatterForWriteLock = new object();
        private static readonly object FileNamesLock = new object();

        private readonly BinaryFormatter _binaryFormatterForRead = new BinaryFormatter();
        private readonly BinaryFormatter _binaryFormatterForWrite = new BinaryFormatter();
        private readonly CacheItemRemovedCallback _cacheItemRemovedCallback;

        private readonly string _diskCacheDirectoryRoot;

        private readonly List<string> _diskCachePeerDirectoryRoots = new List<string>();
        private readonly Dictionary<string, string> _fileNamesByCacheKey = new Dictionary<string, string>();

        public DistributedCache(string diskCacheDirectoryRoot)
        {
            _diskCacheDirectoryRoot = Path.Combine(diskCacheDirectoryRoot, "arachnode.cache\\");

            CacheItemPriority = CacheItemPriority.Normal;
            CacheItemSlidingExpiration = TimeSpan.FromMinutes(1);

            _cacheItemRemovedCallback = CacheItemRemoved;
        }

        public DistributedCache(string diskCacheDirectoryRoot, string[] diskCachePeerDirectoryRoots)
            : this(diskCacheDirectoryRoot)
        {
            foreach (string diskCachePeerDirectoryRoot in diskCachePeerDirectoryRoots)
            {
                if (diskCacheDirectoryRoot == diskCachePeerDirectoryRoot)
                {
                    throw new ArgumentException("DiskCacheDirectoryRoot may not equal any of the DiskCachePeerDirectoryRoots.");
                }

                _diskCachePeerDirectoryRoots.Add(Path.Combine(diskCachePeerDirectoryRoot, "arachnode.cache\\"));
            }
        }

        public bool UseSlidingWindowCache { get; set; }
        public CacheItemPriority CacheItemPriority { get; set; }
        public TimeSpan CacheItemSlidingExpiration { get; set; }
        public bool WriteCacheItemsWhenReadFromCachePeers { get; set; }

        public void Clear()
        {
            List<string> diskCacheDirectoryRoots = new List<string>();

            diskCacheDirectoryRoots.Add(_diskCacheDirectoryRoot);
            diskCacheDirectoryRoots.AddRange(_diskCachePeerDirectoryRoots);

            foreach (string diskCacheDirectoryRoot in diskCacheDirectoryRoots)
            {
                ClearDirectoryRoot(diskCacheDirectoryRoot);
            }
        }

        private void ClearDirectoryRoot(string directoryRoot)
        {
            if (!Directory.Exists(directoryRoot))
            {
                return;
            }

            foreach (string directory in Directory.GetDirectories(directoryRoot))
            {
                ClearDirectoryRoot(directory);
            }

            foreach (string file in Directory.GetFiles(directoryRoot))
            {
                File.Delete(file);
            }
        }

        public void Write(byte[] cacheKey, object cacheObject, SHA1 sha1, bool writeObjectToDisk)
        {
            Write(Encoding.UTF8.GetString(sha1.ComputeHash(cacheKey)), cacheObject, sha1, writeObjectToDisk);
        }

        public void Write(string cacheKey, object cacheObject, SHA1 sha1, bool writeObjectToDisk)
        {
            if (cacheKey == null || cacheObject == null || sha1 == null)
            {
                return;
            }

            /**/

            if (UseSlidingWindowCache)
            {
                HttpRuntime.Cache.Add(cacheKey, cacheObject, null, DateTime.MaxValue, CacheItemSlidingExpiration, CacheItemPriority, _cacheItemRemovedCallback);
            }

            /**/

            byte[] sha1Hash = GetSHA1Hash(cacheKey, sha1);

            string fileName = GetFileName(_diskCacheDirectoryRoot, cacheKey, sha1Hash, writeObjectToDisk, writeObjectToDisk);

            /**/

            if (writeObjectToDisk || !UseSlidingWindowCache)
            {
                WriteObjectToDisk(cacheKey, cacheObject, fileName);
            }
        }

        private void WriteObjectToDisk(string cacheKey, object cacheObject, string fileName)
        {
            Debug.Print("Arachnode.Cache is in development and does not accept disk writes.");

            //lock (_fileNamesByCacheKey[cacheKey])
            //{
            //    using (FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None, 256, FileOptions.Asynchronous))
            //    {
            //        lock (BinaryFormatterForWriteLock)
            //        {
            //            _binaryFormatterForWrite.Serialize(fileStream, cacheObject);
            //        }
            //    }
            //}
        }

        private void CacheItemRemoved(string cacheKey, object o, CacheItemRemovedReason cacheItemRemovedReason)
        {
            string fileName = GetFileNameWithDirectory(cacheKey, SHA1.Create());

            if (_fileNamesByCacheKey.ContainsKey(cacheKey))
            {
                WriteObjectToDisk(cacheKey, o, fileName);
            }

            try
            {
                _fileNamesByCacheKey.Remove(cacheKey);
            }
            catch (Exception)
            {
                //throw;
            }
        }

        public object Read(byte[] cacheKey, SHA1 sha1)
        {
            return Read(Encoding.UTF8.GetString(sha1.ComputeHash(cacheKey)), sha1);
        }

        public object Read(string cacheKey, SHA1 sha1)
        {
            if (cacheKey == null || sha1 == null)
            {
                return null;
            }

            /**/

            object o = HttpRuntime.Cache.Get(cacheKey);

            if (o != null)
            {
                return o;
            }

            /**/

            byte[] sha1Hash = GetSHA1Hash(cacheKey, sha1);

            /**/

            string fileName = GetFileName(_diskCacheDirectoryRoot, cacheKey, sha1Hash, false, false);

            /**/

            if (File.Exists(fileName))
            {
                if (!WriteCacheItemsWhenReadFromCachePeers)
                {
                    GetFileName(_diskCacheDirectoryRoot, cacheKey, sha1Hash, true, false);

                    return ReadObjectFromDisk(cacheKey, fileName);
                }

                //check the cache peers.
                if (ReadObjectFromCachePeerDisk(cacheKey, sha1Hash, fileName) == null)
                {
                    fileName = GetFileName(_diskCacheDirectoryRoot, cacheKey, sha1Hash, true, false);

                    return ReadObjectFromDisk(cacheKey, fileName);
                }
            }

            //check the cache peers.
            return ReadObjectFromCachePeerDisk(cacheKey, sha1Hash, fileName);
        }

        /// <summary>
        /// 	Only UTF8.
        /// </summary>
        /// <param name = "cacheKey"></param>
        /// <param name = "text"></param>
        /// <param name = "sha1"></param>
        public bool ContainsText(string cacheKey, string text, SHA1 sha1)
        {
            byte[] sha1Hash = GetSHA1Hash(cacheKey, sha1);

            /**/

            string fileName = GetFileName(_diskCacheDirectoryRoot, cacheKey, sha1Hash, true, true);

            /**/

            int textOffset = 0;

            lock (_fileNamesByCacheKey[cacheKey])
            {
                using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, 256, FileOptions.Asynchronous))
                {
                    byte[] buffer = new byte[1];

                    while (fileStream.Position != fileStream.Length - 1)
                    {
                        fileStream.Read(buffer, 0, 1);

                        if (buffer[0] == text[textOffset])
                        {
                            textOffset++;

                            if (textOffset == text.Length)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            textOffset = 0;
                        }
                    }
                }
            }

            return false;
        }

        private object ReadObjectFromDisk(string cacheKey, string fileName)
        {
            lock (_fileNamesByCacheKey[cacheKey])
            {
                using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, 256, FileOptions.Asynchronous))
                {
                    object cacheObject = null;

                    lock (BinaryFormatterForReadLock)
                    {
                        try
                        {
                            if (fileStream.Length != 0)
                            {
                                cacheObject = _binaryFormatterForRead.Deserialize(fileStream);
                            }
                        }
                        catch
                        {
                            //TODO:!!!
                            throw;
                        }
                    }

                    /**/

                    if (cacheObject != null && UseSlidingWindowCache)
                    {
                        HttpRuntime.Cache.Add(cacheKey, cacheObject, null, DateTime.MaxValue, CacheItemSlidingExpiration, CacheItemPriority, _cacheItemRemovedCallback);
                    }

                    /**/

                    return cacheObject;
                }
            }
        }

        private object ReadObjectFromCachePeerDisk(string cacheKey, byte[] sha1Hash, string fileName)
        {
            DateTime cacheObjectDateTime = DateTime.MinValue;

            /**/

            FileInfo cacheObjectFileInfo = new FileInfo(fileName);

            if (cacheObjectFileInfo.Exists)
            {
                cacheObjectDateTime = cacheObjectFileInfo.LastWriteTime;
            }

            /**/

            FileInfo cachePeerObjectFileInfo;

            string lastWrittenDiskCachePeerDirectoryRoot = null;

            foreach (string diskCachePeerDirectoryRoot in _diskCachePeerDirectoryRoots)
            {
                string diskCachePeerFileName = GetFileName(diskCachePeerDirectoryRoot, cacheKey, sha1Hash, false, false);

                cachePeerObjectFileInfo = new FileInfo(diskCachePeerFileName);

                if (cachePeerObjectFileInfo.Exists && cachePeerObjectFileInfo.LastWriteTime > cacheObjectDateTime)
                {
                    lastWrittenDiskCachePeerDirectoryRoot = diskCachePeerDirectoryRoot;
                }
            }

            /**/

            if (lastWrittenDiskCachePeerDirectoryRoot != null)
            {
                string cachePeerFileName = GetFileName(lastWrittenDiskCachePeerDirectoryRoot, cacheKey, sha1Hash, false, false);

                string cacheFileName = null;

                if (!WriteCacheItemsWhenReadFromCachePeers)
                {
                    GetFileName(lastWrittenDiskCachePeerDirectoryRoot, cacheKey, sha1Hash, true, false);
                }
                else
                {
                    cacheFileName = GetFileName(_diskCacheDirectoryRoot, cacheKey, sha1Hash, true, true);
                }

                object o = ReadObjectFromDisk(cacheKey, cachePeerFileName);

                if (WriteCacheItemsWhenReadFromCachePeers)
                {
                    WriteObjectToDisk(cacheKey, o, cacheFileName);
                }

                return o;
            }

            /**/

            return null;
        }

        public string GetFileNameWithDirectory(byte[] cacheKey, SHA1 sha1)
        {
            return GetFileName(_diskCacheDirectoryRoot, Encoding.UTF8.GetString(sha1.ComputeHash(cacheKey)), GetSHA1Hash(Encoding.UTF8.GetString(sha1.ComputeHash(cacheKey)), sha1), false, false);
        }

        public string GetFileNameWithDirectory(string cacheKey, SHA1 sha1)
        {
            return GetFileName(_diskCacheDirectoryRoot, cacheKey, GetSHA1Hash(cacheKey, sha1), false, false);
        }

        public string GetFileNameWithoutDirectory(string cacheKey, SHA1 sha1)
        {
            byte[] sha1Hash = GetSHA1Hash(cacheKey, sha1);

            /**/

            StringBuilder fileNameStringBuilder = new StringBuilder();

            foreach (byte b in sha1Hash)
            {
                fileNameStringBuilder.Append((int.Parse(b.ToString())) + ".");
            }

            return fileNameStringBuilder.ToString();
        }

        public string GenerateFullTextUniqueDirectory(string word, bool createDirectory, SHA1 sha1)
        {
            StringBuilder directoryStringBuilder = new StringBuilder();

            foreach (char c in word)
            {
                directoryStringBuilder.Append("\\" + c);
            }

            string directory = _diskCacheDirectoryRoot + directoryStringBuilder;

            if (createDirectory)
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }

            return directory;
        }

        public string GenerateShortUniqueDirectory(string cacheKey, bool createDirectory, SHA1 sha1)
        {
            byte[] sha1Hash = GetSHA1Hash(cacheKey, sha1);

            /**/

            StringBuilder directoryStringBuilder = new StringBuilder();

            foreach (byte b in sha1Hash)
            {
                if (directoryStringBuilder.Length%6 == 0)
                {
                    directoryStringBuilder.Append("\\");
                }

                if (b%2 == 0)
                {
                    directoryStringBuilder.Append("0");
                }
                else
                {
                    directoryStringBuilder.Append("1");
                }
            }

            string directory = _diskCacheDirectoryRoot + directoryStringBuilder;

            if (createDirectory)
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }

            return directory;
        }

        public string GenerateLongUniqueDirectory(string cacheKey, bool createDirectory, SHA1 sha1)
        {
            byte[] sha1Hash = GetSHA1Hash(cacheKey, sha1);

            /**/

            StringBuilder directoryStringBuilder = new StringBuilder();

            foreach (byte b in sha1Hash)
            {
                directoryStringBuilder.Append("\\" + b);
            }

            string directory = _diskCacheDirectoryRoot + directoryStringBuilder;

            if (createDirectory)
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }

            return directory;
        }

        private string GetFileName(string diskCacheDirectoryRoot, string cacheKey, byte[] sha1Hash, bool createKey, bool createDirectory)
        {
            string fileName;

            if (!_fileNamesByCacheKey.TryGetValue(cacheKey, out fileName))
            {
                StringBuilder directoryStringBuilder = new StringBuilder();
                StringBuilder fileNameStringBuilder = new StringBuilder();

                foreach (byte b in sha1Hash)
                {
                    if (directoryStringBuilder.Length%6 == 0)
                    {
                        directoryStringBuilder.Append("\\");
                    }

                    if (b%2 == 0)
                    {
                        directoryStringBuilder.Append("0");
                    }
                    else
                    {
                        directoryStringBuilder.Append("1");
                    }

                    fileNameStringBuilder.Append(b);
                }

                string directory = diskCacheDirectoryRoot + directoryStringBuilder;

                //TODO: Correct the pathing, too many backslashes.

                fileName = directory + "\\" + fileNameStringBuilder;

                if (createKey)
                {
                    try
                    {
                        _fileNamesByCacheKey.Add(cacheKey, fileName);
                    }
                    catch (Exception)
                    {
                        //throw;
                    }
                }

                if (createDirectory)
                {
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                }
            }

            return fileName;
        }

        private static byte[] GetSHA1Hash(string cacheKey, SHA1 sha1)
        {
            return sha1.ComputeHash(Encoding.Default.GetBytes(cacheKey));
        }

        //ANODET: Evaluate this?  Necessary?

        #region Nested type: DirectoryAndFileName

        internal class DirectoryAndFileName
        {
            internal string Directory { get; set; }
            internal string FileName { get; set; }
        }

        #endregion
    }
}