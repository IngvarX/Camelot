using System;
using System.Linq;
using Xunit;

namespace Camelot.Collections.Tests
{
    public class LimitedSizeHistoryTests
    {
        private const int ElementsSize = 10;
        private const int SplitIndex = 5;
        private const int Item = 42;

        [Fact]
        public void TestInitialization()
        {
            var history = CreateHistory();

            Assert.Equal(SplitIndex, history.Current);
            Assert.NotNull(history.Items);
            Assert.NotEmpty(history.Items);

            var items = history.Items.ToArray();
            Assert.Equal(ElementsSize, items.Length);

            for (var i = 0; i < ElementsSize; i++)
            {
                Assert.Equal(i, items[i]);
            }
        }

        [Fact]
        public void TestGoToPrevious()
        {
            var history = CreateHistory();

            for (var i = 0; i < ElementsSize; i++)
            {
                var c = history.GoToPrevious();
                Assert.Equal(history.Current, c);
                Assert.Equal(history.Current, Math.Max(0, SplitIndex - i - 1));
            }
        }

        [Fact]
        public void TestGoToNext()
        {
            var history = CreateHistory();

            for (var i = 0; i < ElementsSize; i++)
            {
                var c = history.GoToNext();
                Assert.Equal(history.Current, c);
                Assert.Equal(history.Current, Math.Min(ElementsSize - 1, SplitIndex + i + 1));
            }
        }

        [Fact]
        public void TestAddItem()
        {
            var history = CreateHistory();

            var added = history.AddItem(Item);
            Assert.Equal(Item, history.Current);
            Assert.Equal(Item, added);

            var items = history.Items.ToArray();
            Assert.Equal(SplitIndex + 2, items.Length);

            for (var i = 0; i < SplitIndex; i++)
            {
                Assert.Equal(i, items[i]);
            }
        }

        private static LimitedSizeHistory<int> CreateHistory()
        {
            var array = Enumerable.Range(0, ElementsSize).ToArray();

            return new LimitedSizeHistory<int>(100, array, SplitIndex);
        }
    }
}