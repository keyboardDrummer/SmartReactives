using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SmartReactives.Common
{
    /// <summary>
    /// Provides default implementations for members of <see cref="IList{T}"/>
    /// </summary>
    public abstract class DefaultList<T> : DefaultCollection<T>, IList<T>
    {
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            Insert(Count, item);
        }

        public override bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public override bool Remove(T item)
        {
            var index = IndexOf(item);
            if (index == -1)
                return false;

            RemoveAt(index);
            return true;
        }

        public int IndexOf(T item)
        {
            int index = 0;
            while (index < Count && !Equals(item, this[index]))
            {
                index++;
            }
            if (index == Count)
                index = -1;
            return index;
        }

        public abstract void Insert(int index, T item);

        public abstract void RemoveAt(int index);

        public abstract T this[int index] { get; set; }
    }
}