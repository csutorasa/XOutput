using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Tools
{
    public static class GarbageFreeDictionary
    {
        public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            var enumerator = dictionary.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    TKey currentKey = enumerator.Current.Key;
                    if (key.Equals(currentKey))
                    {
                        return enumerator.Current.Value;
                    }
                }
                throw new KeyNotFoundException();
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue))
        {
            var enumerator = dictionary.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    TKey currentKey = enumerator.Current.Key;
                    if (key.Equals(currentKey))
                    {
                        return enumerator.Current.Value;
                    }
                }
                return defaultValue;
            }
            finally
            {
                enumerator.Dispose();
            }
        }
    }
}
