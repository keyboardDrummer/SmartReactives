using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartReactives.Core;

namespace SmartReactives.Common
{
    /// <summary>
    /// Wraps an <see cref="ISet{T}"/> so it becomes reactive. 
    /// </summary>
    class ReactiveSet<T> : DefaultSet<T>
    {
        readonly ISet<T> original;

        public ReactiveSet(ISet<T> original)
        {
            this.original = original;
        }

        public override IEnumerator<T> GetEnumerator()
        {
            ReactiveManager.WasRead(this);
            return original.GetEnumerator(); //TODO lazyness bug?
        }

        public override bool Add(T item)
        {
            var result = original.Add(item);
            ReactiveManager.WasChanged(this);
            return result;
        }

        public override void Clear()
        {
            original.Clear();
            ReactiveManager.WasChanged(this);
        }

        public override bool Contains(T item)
        {
            ReactiveManager.WasRead(this);
            return original.Contains(item);
        }

        public override bool Remove(T item)
        {
            var result = original.Remove(item);
            ReactiveManager.WasChanged(this);
            return result;
        }

        public override int Count
        {
            get
            {
                ReactiveManager.WasRead(this);
                return original.Count;
            }
        }
    }
}
