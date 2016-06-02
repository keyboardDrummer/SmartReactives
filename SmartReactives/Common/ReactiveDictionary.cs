using System.Collections.Generic;
using System.Linq;
using SmartReactives.Core;

namespace SmartReactives.Common
{
    /// <summary>
    /// Wrap an <see cref="IDictionary{TKey,TValue}"/> into a reactive dictionary.
    /// The reactive dictionary is precise in the sense that it remembers which keys you access and only throws notifications if those particular keys change.
    /// </summary>
    public class ReactiveDictionary<TKey, TValue> : DefaultDictionary<TKey, TValue>, IWeakListener
    {
        readonly IDictionary<TKey, TValue> original;

        public ReactiveDictionary(IDictionary<TKey, TValue> original)
        {
            this.original = original;
        }

        public override void Clear()
        {
            var keys = Keys;
            original.Clear();
            foreach (var key in keys)
            {
                ReactiveManager.WasChanged(GetKeyReactive(key));
            }
            ReactiveManager.WasChanged(CountReactiveObject);
        }

        public override int Count
        {
            get
            {
                ReactiveManager.WasRead(CountReactiveObject);
                return original.Count;
            } 
        }

        /// <summary>
        /// We're simply using 'this' to track the property Count.
        /// </summary>
        ReactiveDictionary<TKey, TValue> CountReactiveObject => this;

        public override bool ContainsKey(TKey key)
        {
            ReactiveManager.WasRead(GetKeyReactive(key));
            return original.ContainsKey(key);
        }

        WeakStrongReactive GetKeyReactive(TKey key)
        {
            return new WeakStrongReactive(this, key);
        }

        public override void Add(TKey key, TValue value)
        {
            original.Add(key, value);
            ReactiveManager.WasChanged(CountReactiveObject);
        }

        public override bool Remove(TKey key)
        {
            var result = original.Remove(key);
            ReactiveManager.WasChanged(GetKeyReactive(key));
            ReactiveManager.WasChanged(CountReactiveObject);
            return result;
        }

        public override bool TryGetValue(TKey key, out TValue value)
        {
            ReactiveManager.WasRead(GetKeyReactive(key));
            return original.TryGetValue(key, out value);
        }

        public override TValue this[TKey key]
        {
            get
            {
                ReactiveManager.WasRead(GetKeyReactive(key));
                return original[key];
            }
            set
            {
                original[key] = value;
                ReactiveManager.WasChanged(GetKeyReactive(key));
            }
        }

        public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return original.Select(kv =>
            {
                ReactiveManager.WasRead(GetKeyReactive(kv.Key));
                return kv;
            }).GetEnumerator();
        }

        public void Notify(object strongKey)
        {
        }
    }
}