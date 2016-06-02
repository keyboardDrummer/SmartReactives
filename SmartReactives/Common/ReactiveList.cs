using System;
using System.Collections.Generic;
using System.Linq;
using SmartReactives.Core;

namespace SmartReactives.Common
{
    /// <summary>
    /// Wraps an <see cref="IList{T}"/> so it becomes reactive.
    /// </summary>
    public class ReactiveList<T> : DefaultList<T>, IWeakListener
    {
        readonly IList<T> original;

        public ReactiveList(IList<T> original)
        {
            this.original = original;
        }

        public override IEnumerator<T> GetEnumerator()
        {
            return original.Select((element, index) =>
            {
                ReactiveManager.WasRead(new WeakStrongReactive(this, index));
                return element;
            }).GetEnumerator();
        }

        public override int Count
        {
            get
            {
                ReactiveManager.WasRead(this);
                return original.Count;
            }
        }

        public override void Insert(int index, T item)
        {
            original.Insert(index, item);
            UpdateFrom(index + 1, original.Count);
        }

        public override void RemoveAt(int index)
        {
            original.RemoveAt(index);
            UpdateFrom(index, original.Count);
        }

        public override void Clear()
        {
            int count = original.Count;
            original.Clear();;
            UpdateFrom(0, count);
        }

        void UpdateFrom(int start, int exclusiveTo)
        {
            for (int changedIndex = start; changedIndex < exclusiveTo; changedIndex++)
            {
                ReactiveManager.WasChanged(new WeakStrongReactive(this, changedIndex));
            }
            ReactiveManager.WasChanged(this);
        }

        public override T this[int index]
        {
            get
            {
                ReactiveManager.WasRead(new WeakStrongReactive(this, index));
                return original[index];
            }
            set
            {
                original[index] = value;
                ReactiveManager.WasChanged(new WeakStrongReactive(this, index));
            }
        }

        public void Notify(object strongKey)
        {
        }
    }
}
