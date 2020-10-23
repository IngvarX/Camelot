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
        [InlineData(ArchiveType.Tar, typeof(ArchiveWriter))]
        [InlineData(ArchiveType.Zip, typeof(ArchiveWriter))]
        [InlineData(ArchiveType.TarGz, typeof(ArchiveWriter))]
        [InlineData(ArchiveType.TarBz2, typeof(ArchiveWriter))]
        [InlineData(ArchiveType.GZip, typeof(ArchiveWriter))]
        [InlineData(ArchiveType.Bz2, typeof(ArchiveWriter))]
        [InlineData(ArchiveType.SevenZip, typeof(ArchiveWriter))]
        [InlineData(ArchiveType.Xz, typeof(ArchiveWriter))]
        [InlineData(ArchiveType.TarXz, typeof(ArchiveWriter))]
        [InlineData(ArchiveType.Lz, typeof(ArchiveWriter))]
        [InlineData(ArchiveType.TarLz, typeof(ArchiveWriter))]
        public void TestCreateWriter(ArchiveType archiveType, Type expectedType)
        {
            var factory = _autoMocker.CreateInstance<ArchiveProcessorFactory>();
            var processor = factory.CreateWriter(archiveType);

            Assert.NotNull(processor);
            Assert.IsType(expectedType, processor);
        }

        [Theory]
        [InlineData(ArchiveType.Tar, typeof(ArchiveReader))]
        [InlineData(ArchiveType.Zip, typeof(ArchiveReader))]
        [InlineData(ArchiveType.TarGz, typeof(ArchiveReader))]
        [InlineData(ArchiveType.TarBz2, typeof(ArchiveReader))]
        [InlineData(ArchiveType.GZip, typeof(ArchiveReader))]
        [InlineData(ArchiveType.Bz2, typeof(SingleFileZipArchiveReader))]
        [InlineData(ArchiveType.SevenZip, typeof(ArchiveReader))]
        [InlineData(ArchiveType.Xz, typeof(SingleFileZipArchiveReader))]
        [InlineData(ArchiveType.TarXz, typeof(ArchiveReader))]
        [InlineData(ArchiveType.Lz, typeof(SingleFileZipArchiveReader))]
        [InlineData(ArchiveType.TarLz, typeof(ArchiveReader))]
        public void TestCreateReader(ArchiveType archiveType, Type expectedType)
        {
            var factory = _autoMocker.CreateInstance<ArchiveProcessorFactory>();
            var processor = factory.CreateReader(archiveType);

            Assert.NotNull(processor);
            Assert.IsType(expectedType, processor);
        }

        [Fact]
        public void TestCreateReaderFailed()
        {
            const ArchiveType archiveType = (ArchiveType) 42;
            var factory = _autoMocker.CreateInstance<ArchiveProcessorFactory>();
            void Create() => factory.CreateReader(archiveType);

            Assert.Throws<ArgumentOutOfRangeException>(Create);
        }

        [Fact]
        public void TestCreateWriterFailed()
        {
            const ArchiveType archiveType = (ArchiveType) 42;
            var factory = _autoMocker.CreateInstance<ArchiveProcessorFactory>();
            void Create() => factory.CreateWriter(archiveType);

            Assert.Throws<ArgumentOutOfRangeException>(Create);
        }
    }
}