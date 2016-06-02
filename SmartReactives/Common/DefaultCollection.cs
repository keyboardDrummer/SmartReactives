using System.Collections;
using System.Collections.Generic;

namespace SmartReactives.Common
{
    /// <summary>
    /// Provides default implementations for members of <see cref="ICollection{T}"/>
    /// </summary>
    public abstract class DefaultCollection<T>
    {
        public abstract IEnumerator<T> GetEnumerator();

        public abstract void Clear();
        public abstract bool Contains(T item);

        public void CopyTo(T[] array, int arrayIndex)
        {
            int index = arrayIndex;
            foreach (var entry in this)
            {
                array[index] = entry;
                index++;
            }
        }

        public abstract bool Remove(T item);

        public abstract int Count { get; }
        public virtual bool IsReadOnly => false;
    }
}