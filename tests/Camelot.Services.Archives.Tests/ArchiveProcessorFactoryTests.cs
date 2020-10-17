using System;
using Camelot.Services.Abstractions.Models.Enums;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Archives.Tests
{
    public class ArchiveProcessorFactoryTests
    {
        private readonly AutoMocker _autoMocker;

        public ArchiveProcessorFactoryTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Theory]
        [InlineData(ArchiveType.Tar, typeof(TarArchiveProcessor))]
        public void TestCreate(ArchiveType archiveType, Type expectedType)
        {
            var factory = _autoMocker.CreateInstance<ArchiveProcessorFactory>();
            var processor = factory.Create(archiveType);

            Assert.NotNull(processor);
            Assert.IsType(expectedType, processor);
        }

        [Fact]
        public void TestCreateFailed()
        {
            const ArchiveType archiveType = (ArchiveType) 42;
            var factory = _autoMocker.CreateInstance<ArchiveProcessorFactory>();
            void Create() => factory.Create(archiveType);

            Assert.Throws<ArgumentOutOfRangeException>(Create);
        }
    }
}