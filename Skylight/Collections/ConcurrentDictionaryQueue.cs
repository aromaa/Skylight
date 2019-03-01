using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Collections
{
    public class ConcurrentDictionaryQueue<K, V> : ConcurrentDictionary<K, V>
    {
        public bool TryDequeueValue(out V value)
        {
            while (this.Count > 0) //This cant make dead lock, right?
            {
                foreach (K key in this.Keys)
                {
                    if (this.TryRemove(key, out value))
                    {
                        return true;
                    }
                }
            }

            value = default(V);
            return false;
        }

        public bool TryRemove(K key)
        {
            V value;
            return this.TryRemove(key, out value);
        }
    }
}
