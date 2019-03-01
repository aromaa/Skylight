using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Collections
{
    public class ConcurrentListDictionary<K, S, V> : ConcurrentDictionary<K, V>
    {
        private readonly List<V> EmptyList = new List<V>();
        private ConcurrentDictionary<S, List<V>> Sub;

        public ConcurrentListDictionary()
        {
            this.Sub = new ConcurrentDictionary<S, List<V>>();
        }

        public bool Add(K key, S sub, V value)
        {
            if (this.TryAdd(key, value))
            {
                this.Sub.AddOrUpdate(sub, new List<V>() { value }, delegate (S s, List<V> l) { l.Add(value); return l; });

                return true;
            }

            return false;
        }

        public bool Remove(K key, S sub)
        {
            V trash;
            if (this.TryRemove(key, out trash))
            {
                List<V> list;
                if (this.Sub.TryGetValue(sub, out list))
                {
                    list.Remove(trash);

                    if (list.Count <= 0)
                    {
                        this.Sub.TryRemove(sub, out list);
                    }

                    return true;
                }
            }

            return false;
        }

        public V Get(K key)
        {
            V value;
            this.TryGetValue(key, out value);
            return value;
        }

        public List<V> Get(S sub)
        {
            List<V> list;
            this.Sub.TryGetValue(sub, out list);
            return list ?? this.EmptyList;
        }
    }
}
