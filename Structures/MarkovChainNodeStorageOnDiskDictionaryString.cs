using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Arachnode.Utilities;

namespace Arachnode.Structures
{
    [Serializable]
    public class MarkovChainNodeStorageOnDiskDictionaryString : AMarkovChainNodeStorageString
    {
        public MarkovChainNodeStorageOnDiskDictionaryString(string onDiskDirectoryBasePath)
        {
            _onDiskDirectoryBasePath = onDiskDirectoryBasePath;
        }

        internal MarkovChainNodeStorageOnDiskDictionaryString(IOrderedEnumerable<KeyValuePair<string, MarkovChainNodeString>> aMarkovChainNodeStorage, string onDiskDirectoryBasePath)
            : base(aMarkovChainNodeStorage, onDiskDirectoryBasePath)
        {
            _onDiskDirectoryBasePath = onDiskDirectoryBasePath;

            _dictionary = new Dictionary<string, MarkovChainNodeString>();

            IEnumerator<KeyValuePair<string, MarkovChainNodeString>> enumerator = aMarkovChainNodeStorage.GetEnumerator();

            while(enumerator.MoveNext())
            {
                _dictionary.Add(enumerator.Current.Key, enumerator.Current.Value);
            }
        }

        public override MarkovChainNodeString this[string key]
        {
            get
            {
                string filePath = GetActualOnDiskFilePath(key);

                MarkovChainNodeString markovChainNodeString = (MarkovChainNodeString)Serialization.DeserializeObject(filePath, typeof (MarkovChainNodeString));

                markovChainNodeString._onDiskDirectoryBasePath = _onDiskDirectoryBasePath;
                markovChainNodeString.Children = (MarkovChainNodeStorageOnDiskDictionaryString)Activator.CreateInstance(typeof(MarkovChainNodeStorageOnDiskDictionaryString), _onDiskDirectoryBasePath);

                return markovChainNodeString;
            }
            set
            {
                string filePath = GetActualOnDiskFilePath(key);

                Serialization.SerializeObject(filePath, value);
            }
        }

        public override int Count
        {
            get
            {
                string directoryPath = GetActualOnDiskDirectoryPath();

                int count = Directory.GetDirectories(directoryPath).Count();
                
                return count;
            }
        }

        public override void Add(string key, MarkovChainNodeString value)
        {
            string directoryPath = GetActualOnDiskDirectoryPath();

            directoryPath = System.IO.Path.Combine(directoryPath, key);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        public override bool ContainsKey(string key)
        {
            string directoryPath = GetActualOnDiskDirectoryPath();

            directoryPath = System.IO.Path.Combine(directoryPath, key);

            return Directory.Exists(directoryPath);
        }

        public override ICollection<string> Keys
        {
            get
            {
                string directoryPath = GetActualOnDiskDirectoryPath();

                string[] keys = Directory.GetDirectories(directoryPath).Select(_ => GetKeyPath(_)).ToArray();

                return keys;
            }
        }

        public override ICollection<MarkovChainNodeString> Values
        {
            get
            {
                ICollection<string> keys = Keys;

                MarkovChainNodeString[] values = new MarkovChainNodeString[keys.Count];

                int i = 0;
                foreach(string key in keys)
                {
                    values[i] = this[key];
                    i++;
                }

                return values;
            }
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
            if (Directory.Exists(_onDiskDirectoryBasePath))
            {
                Directory.Delete(_onDiskDirectoryBasePath, true);
            }

            while (Directory.Exists(_onDiskDirectoryBasePath))
            {
                Thread.Sleep(100);
            }

            if (!Directory.Exists(_onDiskDirectoryBasePath))
            {
                Directory.CreateDirectory(_onDiskDirectoryBasePath);
            }
        }
    }
}