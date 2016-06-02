using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SmartReactives.Common
{
    /// <summary>
    /// Provides default implementations for members of <see cref="ISet{T}"/>
    /// </summary>
    public abstract class DefaultSet<T> : DefaultCollection<T>, ISet<T>
    {
        public void UnionWith(IEnumerable<T> other)
        {
            foreach (var otherElement in other)
                Add(otherElement);
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            foreach (var element in other)
            {
                if (!Contains(element))
                    Remove(element);
            }
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            foreach (var element in other)
            {
                if (Contains(element))
                    Remove(element);
            }
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            foreach (var element in other)
            {
                if (Contains(element))
                    Remove(element);
                else
                    Add(element);
            }
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            var otherSet = new HashSet<T>(other);
            foreach (var element in this)
            {
                if (!otherSet.Contains(element))
                    return false;
            }
            return true;
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            foreach (var element in other)
            {
                if (!Contains(element))
                    return false;
            }
            return true;
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return IsSupersetOf(other) && Count != other.Count();
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return IsSubsetOf(other) && Count != other.Count();
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            foreach (var element in other)
            {
                if (Contains(element))
                    return true;
            }
            return false;
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            var otherCount = 0;
            foreach (var otherElement in other)
            {
                otherCount++;
                if (!Contains(otherElement))
                    return false;
            }
            return otherCount == Count;
        }

        public abstract bool Add(T item);

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}