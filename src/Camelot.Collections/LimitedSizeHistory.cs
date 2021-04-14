using System.Collections.Generic;

namespace Camelot.Collections
{
    public class LimitedSizeHistory<T>
    {
        private readonly int _capacity;
        private readonly LinkedList<T> _linkedList;

        private LinkedListNode<T> _current;

        public T Current => _current.Value;

        public int CurrentIndex { get; private set; }

        public IEnumerable<T> Items => _linkedList;

        public LimitedSizeHistory(int capacity, IReadOnlyList<T> collection, int splitIndex)
        {
            _capacity = capacity;
            _linkedList = new LinkedList<T>();

            CurrentIndex = splitIndex;
            FillLinkedList(collection, splitIndex);
        }

        public T GoToPrevious()
        {
            if (_current.Previous != null)
            {
                _current = _current.Previous;
                CurrentIndex--;
            }

            return Current;
        }

        public T GoToNext()
        {
            if (_current.Next != null)
            {
                _current = _current.Next;
                CurrentIndex++;
            }

            return Current;
        }

        public T AddItem(T item)
        {
            RemoveAllItemsAfterCurrent();
            AppendItem(item);
            RemoveOldestItemIfNeeded();

            return Current;
        }

        private void FillLinkedList(IReadOnlyList<T> collection, int splitIndex)
        {
            for (var i = 0; i < collection.Count; i++)
            {
                _linkedList.AddLast(collection[i]);
                if (i == splitIndex)
                {
                    _current = _linkedList.Last;
                }
            }
        }

        private void RemoveAllItemsAfterCurrent()
        {
            while (_current != _linkedList.Last)
            {
                _linkedList.RemoveLast();
            }
        }

        private void AppendItem(T item)
        {
            _current = _linkedList.AddLast(item);
            CurrentIndex++;
        }

        private void RemoveOldestItemIfNeeded()
        {
            while (_linkedList.Count > _capacity)
            {
                _linkedList.RemoveFirst();
                CurrentIndex--;
            }
        }
    }
}