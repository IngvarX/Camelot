using Xunit;

namespace Camelot.Services.Tests
{
    public static class AssertExtensions
    {
        public static void Fail()
        {
            Assert.True(false);
        }
    }
}