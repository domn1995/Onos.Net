using System.Collections.Generic;

namespace Onos.Net.Utils.Misc.OnLab.Helpers
{
    public static class DictionaryExtensions
    {
        public static void AddOrSet<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }
        }

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default)
        {
            if (dict.TryGetValue(key, out TValue value))
            {
                return value;
            }
            return defaultValue;
        }

        public static TValue GetOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default)
        {
            if (dict.TryGetValue(key, out TValue value))
            {
                return value;
            }
            return defaultValue;
        }
    }
}