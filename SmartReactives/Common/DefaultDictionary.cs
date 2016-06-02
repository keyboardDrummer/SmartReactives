using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SmartReactives.Common
{
    /// <summary>
    /// Provides default implementations for members of <see cref="IDictionary{TKey, TValue}"/>
    /// </summary>
    public abstract class DefaultDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public virtual bool IsReadOnly => false;
        public abstract void Clear();
        public abstract int Count { get; }

        public abstract bool ContainsKey(TKey key);
        public abstract void Add(TKey key, TValue value);
        public abstract bool Remove(TKey key);

        public abstract bool TryGetValue(TKey key, out TValue value);

        public abstract TValue this[TKey key] { get; set; }
        public ICollection<TKey> Keys => this.Select(kv => kv.Key).ToList();
        public ICollection<TValue> Values => this.Select(kv => kv.Value).ToList();

        public abstract IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}