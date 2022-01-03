using System;
using System.Linq;
using Xunit;

namespace Camelot.Collections.Tests;

public class LimitedSizeStackTests
{
    private const int Size = 3;

    [Fact]
    public void TestPopPush()
    {
        var stack = new LimitedSizeStack<int>(Size);
        Assert.True(stack.IsEmpty);

        const int element = 1;
        stack.Push(element);
        Assert.False(stack.IsEmpty);

        Assert.Equal(element, stack.Pop());
    }

    [Fact]
    public void TestPopThrows()
    {
        var stack = new LimitedSizeStack<int>(Size);
        stack.Push(1);
        stack.Pop();
        Assert.True(stack.IsEmpty);

        Assert.Throws<InvalidOperationException>(() => stack.Pop());
    }

    [Fact]
    public void TestLimitedSize()
    {
        var stack = new LimitedSizeStack<int>(Size);
        Assert.True(stack.IsEmpty);

        for (var i = 0; i < 5; i++)
        {
            stack.Push(i);
        }

        Assert.NotNull(stack.Items);

        var items = stack.Items.ToArray();
        Assert.Equal(3, items.Length);
        Assert.Equal(2, items[0]);
        Assert.Equal(3, items[1]);
        Assert.Equal(4, items[2]);
    }

    [Fact]
    public void TestClear()
    {
        var stack = new LimitedSizeStack<int>(Size);

        for (var i = 0; i < 5; i++)
        {
            stack.Push(i);
        }

        Assert.False(stack.IsEmpty);

        stack.Clear();

        Assert.True(stack.IsEmpty);
        Assert.Empty(stack.Items);
    }
}