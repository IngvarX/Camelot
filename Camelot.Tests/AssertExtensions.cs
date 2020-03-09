using Xunit;

namespace Camelot.Tests
{
    public static class AssertExtensions
    {
        public static void Fail()
        {
            Assert.True(false);
        }
    }
}