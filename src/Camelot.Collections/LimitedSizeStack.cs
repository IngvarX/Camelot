using System;
using System.Collections.Generic;

namespace Camelot.Collections;

public class LimitedSizeStack<T>
{
    private readonly int _capacity;
    private readonly LinkedList<T> _linkedList;

    private int _currentLength;

    public bool IsEmpty => _currentLength == 0;

    public IEnumerable<T> Items => _linkedList;

    public LimitedSizeStack(int capacity)
    {
        _capacity = capacity;
        _linkedList = new LinkedList<T>();
        _currentLength = 0;
    }

    public void Push(T item)
    {
        if (_currentLength == _capacity)
        {
            CleanupLinkedList();
        }

        Add(item);
    }

    public T Pop() => IsEmpty ? throw new InvalidOperationException("Stack is empty!") : Remove();

    public void Clear()
    {
        _linkedList.Clear();
        _currentLength = 0;
    }

    private void CleanupLinkedList()
    {
        _linkedList.RemoveFirst();
        _currentLength--;
    }

    private void Add(T item)
    {
        _linkedList.AddLast(item);
        _currentLength++;
    }

    private T Remove()
    {
        // ReSharper disable once PossibleNullReferenceException
        var last = _linkedList.Last.Value;
        _linkedList.RemoveLast();
        _currentLength--;

        return last;
    }
}