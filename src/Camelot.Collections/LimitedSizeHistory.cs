using System.Collections.Generic;
using System.Linq;

namespace Camelot.Collections
{
    public class LimitedSizeHistory<T>
    {
        private readonly LimitedSizeStack<T> _left;
        private readonly LimitedSizeStack<T> _right;

        public T Current { get; private set; }

        public IEnumerable<T> Items => _left
            .Items
            .Concat(Enumerable.Repeat(Current, 1))
            .Concat(_right.Items.Reverse());

        public LimitedSizeHistory(int capacity, IReadOnlyList<T> collection, int splitIndex)
        {
            _left = new LimitedSizeStack<T>(capacity);
            _right = new LimitedSizeStack<T>(capacity);

            FillStacks(collection, splitIndex);
        }

        public void GoToPrevious()
        {
            if (_left.IsEmpty)
            {
                return;
            }

            _right.Push(Current);
            Current = _left.Pop();
        }

        public void GoToNext()
        {
            if (_right.IsEmpty)
            {
                return;
            }

            _left.Push(Current);
            Current = _right.Pop();
        }

        public void AddItem(T item)
        {
            _left.Push(Current);
            _right.Clear();
            Current = item;
        }

        private void FillStacks(IReadOnlyList<T> collection, int splitIndex)
        {
            for (var i = 0; i < splitIndex; i++)
            {
                _left.Push(collection[i]);
            }

            Current = collection[splitIndex];

            for (var i = collection.Count - 1; i > splitIndex; i--)
            {
                _right.Push(collection[i]);
            }
        }
    }
}