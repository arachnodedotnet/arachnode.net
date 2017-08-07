using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Arachnode.Structures
{
    [Serializable]
    public class MarkovChainNodeStorageInMemoryDictionaryString : AMarkovChainNodeStorageString
    {
        public MarkovChainNodeStorageInMemoryDictionaryString(string onDiskDirectoryBasePath)
        {
            
        }

        internal MarkovChainNodeStorageInMemoryDictionaryString(IOrderedEnumerable<KeyValuePair<string, MarkovChainNodeString>> aMarkovChainNodeStorage, string onDiskDirectoryBasePath)
            : base(aMarkovChainNodeStorage, onDiskDirectoryBasePath)
        {
            _dictionary = new Dictionary<string, MarkovChainNodeString>();

            IEnumerator<KeyValuePair<string, MarkovChainNodeString>> enumerator = aMarkovChainNodeStorage.GetEnumerator();

            while(enumerator.MoveNext())
            {
                _dictionary.Add(enumerator.Current.Key, enumerator.Current.Value);
            }
        }

        public override MarkovChainNodeString this[string key]
        {
            get { return _dictionary[key]; }
            set { _dictionary[key] = value; }
        }

        public override int Count
        {
            get { return _dictionary.Count; }
        }

        public override void Add(string key, MarkovChainNodeString value)
        {
            _dictionary.Add(key, value);
        }

        public override bool ContainsKey(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public override ICollection<string> Keys
        {
            get { return _dictionary.Keys; }
        }

        public override ICollection<MarkovChainNodeString> Values
        {
            get { return _dictionary.Values; }
        }

        public override IEnumerator GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public override KeyValuePair<string, MarkovChainNodeString> First()
        {
            return _dictionary.First();
        }

        public override IOrderedEnumerable<KeyValuePair<string, MarkovChainNodeString>> OrderByDescendingChainCount()
        {
            return _dictionary.OrderByDescending(_ => _.Value.ChainCount);
        }

        public override void Clear()
        {
            _dictionary.Clear();
        }
    }
}