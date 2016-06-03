using System.Collections.Generic;
using System.Linq;
using SmartReactives.Core;

namespace SmartReactives.Common
{
    /// <summary>
    /// Wraps an <see cref="IList{T}"/> so it becomes reactive. 
    /// The reactive list is precise in the sense that it remembers which indices you access and only throws notifications if those particular indices change.
    /// </summary>
    public class ReactiveList<T> : DefaultList<T>
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
                ReactiveManager.WasRead(CountReactiveObject);
                return original.Count;
            }
        }

        /// <summary>
        /// We're simply using 'this' to track the property Count.
        /// </summary>
        ReactiveList<T> CountReactiveObject => this;

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
            ReactiveManager.WasChanged(CountReactiveObject);
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
    }
}
