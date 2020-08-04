using Camelot.Services.Windows.Builders;
using Xunit;

namespace Camelot.Services.Windows.Tests
{
    public class WindowsRemovedFileMetadataBuilderFactoryTests
    {
        [Fact]
        public void TestCreation()
        {
            var factory = new WindowsRemovedFileMetadataBuilderFactory();
            var builder = factory.Create();

            Assert.NotNull(builder);
            Assert.IsType<WindowsRemovedFileMetadataBuilder>(builder);
        }
    }
}