using System.Collections.Generic;

namespace Camelot.Collections;

public class LimitedSizeHistory<T>
{
    private readonly int _capacity;
    private readonly LinkedList<T> _linkedList;

    private LinkedListNode<T> _current;

    public T Current => _current.Value;

    public int CurrentIndex { get; private set; }

    public IEnumerable<T> Items => _linkedList;

    public bool HasPrevious => _current.Previous is not null;

    public bool HasNext => _current.Next is not null;

    public LimitedSizeHistory(int capacity, IReadOnlyList<T> collection, int splitIndex)
    {
        _capacity = capacity;
        _linkedList = new LinkedList<T>();

        CurrentIndex = splitIndex;
        FillLinkedList(collection, splitIndex);
    }

    public T GoToPrevious()
    {
        if (HasPrevious)
        {
            _current = _current.Previous;
            CurrentIndex--;
        }

        return Current;
    }

    public T GoToNext()
    {
        if (HasNext)
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
        RemoveOldItems();

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

    private void RemoveOldItems()
    {
        while (_linkedList.Count > _capacity)
        {
            _linkedList.RemoveFirst();
            CurrentIndex--;
        }
    }
}