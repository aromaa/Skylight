using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Utilies
{
    public class ValueHolder
    {
        private Dictionary<string, object> Values;

        public ValueHolder()
        {
            this.Values = new Dictionary<string, object>();
        }

        public ValueHolder(string key, object value)
        {
            this.Values = new Dictionary<string, object>() { {key, value} };
        }

        public ValueHolder(string key, object value, params object[] extra)
        {
            this.Values = new Dictionary<string, object>() { { key, value } };

            if (extra != null)
            {
                if (extra.Length % 2 == 0)
                {
                    for (int i = 0; i < extra.Length; i += 2)
                    {
                        this.Values.Add((string)extra[i], extra[i + 1]);
                    }
                }
                else
                {
                    throw new Exception("Data lenght must be dividable with two");
                }
            }
        }

        public ValueHolder AddValue(string key, object value)
        {
            this.Values.Add(key, value);
            return this;
        }

        public T GetValue<T>(string key)
        {
            return (T)this.Values[key];
        }

        public T GetValueOrDefault<T>(string key)
        {
            object value;
            if (!this.Values.TryGetValue(key, out value))
            {
                value = default(T);
            }

            return (T)value;
        }

        public T GetValueOrDefault<T>(string key, T defaultValue)
        {
            object value;
            if (!this.Values.TryGetValue(key, out value))
            {
                value = defaultValue;
            }

            return (T)value;
        }
    }
}
