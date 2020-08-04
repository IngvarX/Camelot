using System.Collections.Generic;
using Xunit;

namespace Camelot.Services.Windows.Tests
{
    public class WindowsTrashCanNodeNameGeneratorTests
    {
        [Fact]
        public void TestFormat()
        {
            var generator = new WindowsTrashCanNodeNameGenerator();
            var name = generator.Generate();

            Assert.NotNull(name);
            Assert.Equal(6, name.Length);
            Assert.Equal(name.ToUpper(), name);
        }

        [Fact]
        public void TestRandom()
        {
            var generator = new WindowsTrashCanNodeNameGenerator();
            var names = new HashSet<string>();

            for (var i = 0; i < 1000; i++)
            {
                var name = generator.Generate();
                Assert.DoesNotContain(name, names);

                names.Add(name);
            }
        }
    }
}