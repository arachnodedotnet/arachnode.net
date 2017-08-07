using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Caching;
using Arachnode.Utilities;

namespace Arachnode.Structures
{
    [Serializable]
    public class MarkovChainNodeStorageOnDiskDictionary : AMarkovChainNodeStorageString
    {
        private readonly CacheItemRemovedCallback _cacheItemRemovedCallback;
        private static bool _isCheckedForSearchIndexer;
        private static bool _rootChecked;
        private const double _slidingExpirationInSeconds = 60000;

        public MarkovChainNodeStorageOnDiskDictionary(string onDiskDirectoryBasePath)
        {
            _onDiskDirectoryBasePath = onDiskDirectoryBasePath;

            if (!_rootChecked)
            {
                if (!Delimon.Win32.IO.Directory.Exists(_onDiskDirectoryBasePath))
                {
                    Delimon.Win32.IO.Directory.CreateDirectory(_onDiskDirectoryBasePath);
                }

                _rootChecked = true;
            }

            if (Path == ROOT && !_isCheckedForSearchIndexer)
            {
                CheckForSearchIndexer();
            }

            _cacheItemRemovedCallback = CacheItemRemoved;
        }

        internal MarkovChainNodeStorageOnDiskDictionary(IOrderedEnumerable<KeyValuePair<string, MarkovChainNodeString>> aMarkovChainNodeStorage, string onDiskDirectoryBasePath)
            : base(aMarkovChainNodeStorage, onDiskDirectoryBasePath)
        {
            _onDiskDirectoryBasePath = onDiskDirectoryBasePath;

            _dictionary = new Dictionary<string, MarkovChainNodeString>();

            IEnumerator<KeyValuePair<string, MarkovChainNodeString>> enumerator = aMarkovChainNodeStorage.GetEnumerator();

            while(enumerator.MoveNext())
            {
                _dictionary.Add(enumerator.Current.Key, enumerator.Current.Value);
            }

            CheckForSearchIndexer();
        }

        public void CheckForSearchIndexer()
        {
            foreach (Process process in Process.GetProcesses())
            {
                if (process.ToString().Contains("SearchIndexer"))
                {
                    //throw new Exception("SearchIndexer.exe is running.  Please terminate SearchIndexer.exe.");
                }
            }

            _isCheckedForSearchIndexer = true;
        }

        public override MarkovChainNodeString this[string key]
        {
            get
            {
                string filePath = GetActualOnDiskFilePath(key);

                object o = HttpRuntime.Cache.Get(filePath);

                if(o != null)
                {
                    return (MarkovChainNodeString) o;
                }

                MarkovChainNodeString markovChainNodeString = (MarkovChainNodeString) Serialization.DeserializeObject(filePath, typeof (MarkovChainNodeString));

                markovChainNodeString._onDiskDirectoryBasePath = _onDiskDirectoryBasePath;
                markovChainNodeString.Children = (MarkovChainNodeStorageOnDiskDictionary) Activator.CreateInstance(typeof (MarkovChainNodeStorageOnDiskDictionary), _onDiskDirectoryBasePath);
                markovChainNodeString.Children.Path = markovChainNodeString.Path;

                HttpRuntime.Cache.Add(filePath, markovChainNodeString, null, DateTime.MaxValue, TimeSpan.FromSeconds(_slidingExpirationInSeconds), CacheItemPriority.Normal, _cacheItemRemovedCallback);

                return markovChainNodeString;
            }
            set
            {
                string filePath = GetActualOnDiskFilePath(key);

                HttpRuntime.Cache.Add(filePath, value, null, Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(_slidingExpirationInSeconds), CacheItemPriority.Normal, _cacheItemRemovedCallback);

                Serialization.SerializeObject(filePath, value);

                ManageCachedValues(Keys, value);
            }
        }

        public override int Count
        {
            get
            {
                string directoryPath = GetActualOnDiskDirectoryPath();

                HashSet<string> directories = ManageCachedKeys(directoryPath, false);

                return directories.Count;
            }
        }

        public override void Add(string key, MarkovChainNodeString value)
        {
            string directoryPath = GetActualOnDiskDirectoryPath();

            directoryPath = System.IO.Path.Combine(directoryPath, key);

            if (HttpRuntime.Cache.Get(directoryPath) == null || !Directory.Exists(directoryPath))
            {
                Delimon.Win32.IO.Directory.CreateDirectory(directoryPath);

                value.Update();

                ManageCachedKeys(directoryPath, true);
                ManageCachedValues(Keys, value);

                HttpRuntime.Cache.Add(directoryPath, true, null, Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(_slidingExpirationInSeconds), CacheItemPriority.Normal, _cacheItemRemovedCallback);                
            }
        }

        public override bool ContainsKey(string key)
        {
            string directoryPath = GetActualOnDiskDirectoryPath();

            directoryPath = Delimon.Win32.IO.Path.Combine(directoryPath, key);

            object o = HttpRuntime.Cache.Get(directoryPath);

            if(o != null)
            {
                return (bool)o;
            }

            bool containsKey = Delimon.Win32.IO.Directory.Exists(directoryPath);

            if (containsKey)
            {
                HttpRuntime.Cache.Add(directoryPath, true, null, Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(_slidingExpirationInSeconds), CacheItemPriority.Normal, _cacheItemRemovedCallback);
            }

            return containsKey;
        }

        public override ICollection<string> Keys
        {
            get
            {
                string directoryPath = GetActualOnDiskDirectoryPath();

                HashSet<string> directories = ManageCachedKeys(directoryPath, false);

                string[] keys = directories.Select(_ => GetKeyPath(_)).ToArray();

                return keys;
            }
        }

        private HashSet<string> ManageCachedKeys(string directoryPath, bool addDirectoryPath)
        {
            object o = HttpRuntime.Cache.Get("Keys: " + Path);

            HashSet<string> directories = null;

            if (o == null)
            {
                directories = new HashSet<string>();

                foreach (string directory in Delimon.Win32.IO.Directory.GetDirectories(directoryPath))
                {
                    directories.Add(directory);
                }

                HttpRuntime.Cache.Add("Keys: " + Path, directories, null, Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(_slidingExpirationInSeconds), CacheItemPriority.Normal, _cacheItemRemovedCallback);
            }
            else
            {
                directories = (HashSet<string>)o;
            }

            if (addDirectoryPath)
            {
                directories.Add(directoryPath);
            }

            return directories;
        }

        public override ICollection<MarkovChainNodeString> Values
        {
            get
            {
                MarkovChainNodeString[] markovChainNodeStrings = ManageCachedValues(Keys, null);

                return markovChainNodeStrings;
            }
        }

        private MarkovChainNodeString[] ManageCachedValues(ICollection<string> keys, MarkovChainNodeString markovChainNodeString)
        {
            object o = HttpRuntime.Cache.Get("Values: " + Path);

            HashSet<MarkovChainNodeString> markovChainNodeStrings = null;

            if (o == null)
            {
                markovChainNodeStrings = new HashSet<MarkovChainNodeString>();

                int i = 0;
                foreach (string key in keys)
                {
                    markovChainNodeStrings.Add(this[key]);
                    i++;
                }

                HttpRuntime.Cache.Add("Values: " + Path, markovChainNodeStrings, null, Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(_slidingExpirationInSeconds), CacheItemPriority.Normal, _cacheItemRemovedCallback);
            }
            else
            {
                markovChainNodeStrings = (HashSet<MarkovChainNodeString>)o;
            }

            if (markovChainNodeString != null)
            {
                markovChainNodeStrings.Add(markovChainNodeString);
            }

            return markovChainNodeStrings.ToArray();
        }

        public override IEnumerator GetEnumerator()
        {
            return Keys.GetEnumerator();
        }

        public override KeyValuePair<string, MarkovChainNodeString> First()
        {
            string first = Keys.First();

            return new KeyValuePair<string, MarkovChainNodeString>(first, this[first]);
        }
    
        public override IOrderedEnumerable<KeyValuePair<string, MarkovChainNodeString>> OrderByDescendingChainCount()
        {
            Dictionary<string, MarkovChainNodeString> orderByDescendingChainCount = new Dictionary<string, MarkovChainNodeString>();

            List<string> keys = new List<string>(Keys);
            List<MarkovChainNodeString> values = new List<MarkovChainNodeString>(Values);

            for (int i = 0; i < keys.Count; i++)
            {
                orderByDescendingChainCount.Add(keys[i], values[i]);
            }

            return orderByDescendingChainCount.OrderByDescending(_ => _.Value.ChainCount);
        }

        public override void Clear()
        {
            if (Delimon.Win32.IO.Directory.Exists(_onDiskDirectoryBasePath))
            {
                Delimon.Win32.IO.Directory.Delete(_onDiskDirectoryBasePath, true);
            }

            while (Delimon.Win32.IO.Directory.Exists(_onDiskDirectoryBasePath))
            {
                Thread.Sleep(100);
            }

            if (!Delimon.Win32.IO.Directory.Exists(_onDiskDirectoryBasePath))
            {
                Delimon.Win32.IO.Directory.CreateDirectory(_onDiskDirectoryBasePath);
            }

            /**/

            List<string> keys = new List<string>();

            IDictionaryEnumerator enumerator = HttpRuntime.Cache.GetEnumerator();

            while (enumerator.MoveNext())
            {
                keys.Add(enumerator.Key.ToString());
            }

            for (int i = 0; i < keys.Count; i++)
            {
                HttpRuntime.Cache.Remove(keys[i]);
            }
        }

        private void CacheItemRemoved(string cacheKey, object o, CacheItemRemovedReason cacheItemRemovedReason)
        {
            //Console.WriteLine(cacheKey);
        }
    }
}