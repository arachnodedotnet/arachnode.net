using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Arachnode.Structures
{
    [Serializable]
    public abstract class AMarkovChainNodeStorageString : AMarkovChainStorage
    {
        protected Dictionary<string, MarkovChainNodeString> _dictionary = new Dictionary<string, MarkovChainNodeString>();

        public AMarkovChainNodeStorageString()
        {
        }

        public AMarkovChainNodeStorageString(string onDiskDirectoryBasePath)
        {
            
        }

        internal AMarkovChainNodeStorageString(IOrderedEnumerable<KeyValuePair<string, MarkovChainNodeString>> aMarkovChainNodeStorage, string onDiskDirectoryBasePath)
        {
        }

        public abstract MarkovChainNodeString this[string key] { get; set; }

        public abstract int Count { get; }

        public abstract void Add(string key, MarkovChainNodeString value);

        public abstract bool ContainsKey(string key);

        public abstract ICollection<string> Keys { get; }

        public abstract ICollection<MarkovChainNodeString> Values { get; }

        public abstract IEnumerator GetEnumerator();

        public abstract KeyValuePair<string, MarkovChainNodeString> First();

        public abstract IOrderedEnumerable<KeyValuePair<string, MarkovChainNodeString>> OrderByDescendingChainCount();

        public abstract void Clear();
    }
}