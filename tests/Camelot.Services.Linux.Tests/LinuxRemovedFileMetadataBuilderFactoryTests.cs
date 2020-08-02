using Camelot.Services.Linux.Builders;
using Xunit;

namespace Camelot.Services.Linux.Tests
{
    public class LinuxRemovedFileMetadataBuilderFactoryTests
    {
        [Fact]
        public void TestCreation()
        {
            var factory = new LinuxRemovedFileMetadataBuilderFactory();
            var builder = factory.Create();

            Assert.NotNull(builder);
            Assert.IsType<LinuxRemovedFileMetadataBuilder>(builder);
        }
    }
}