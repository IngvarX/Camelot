using System;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Archive;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Tests.Archive
{
    public class ArchiveServiceTests
    {
        private const string FilePath = "File";
        private const string OutputFilePath = "OutputFile";
        private const string OutputDirPath = "OutputDir";

        private readonly AutoMocker _autoMocker;

        public ArchiveServiceTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(ArchiveType.Rar, true)]
        [InlineData(ArchiveType.Tar, true)]
        [InlineData(ArchiveType.Zip, true)]
        [InlineData(ArchiveType.SevenZip, true)]
        [InlineData(ArchiveType.GZip, true)]
        [InlineData(ArchiveType.TarBz, true)]
        [InlineData(ArchiveType.TarGz, true)]
        [InlineData(ArchiveType.TarLz, true)]
        [InlineData(ArchiveType.TarXz, true)]
        public void TestCheckIfFileIsArchive(ArchiveType? archiveType, bool isArchive)
        {
            _autoMocker
                .Setup<IArchiveTypeMapper, ArchiveType?>(m => m.GetArchiveTypeFrom(FilePath))
                .Returns(archiveType);

            var service = _autoMocker.CreateInstance<ArchiveService>();
            var actual = service.CheckIfFileIsArchive(FilePath);

            Assert.Equal(isArchive, actual);
        }

        [Theory]
        [InlineData(ArchiveType.Rar)]
        [InlineData(ArchiveType.Tar)]
        [InlineData(ArchiveType.Zip)]
        [InlineData(ArchiveType.SevenZip)]
        [InlineData(ArchiveType.GZip)]
        [InlineData(ArchiveType.TarBz)]
        [InlineData(ArchiveType.TarGz)]
        [InlineData(ArchiveType.TarLz)]
        [InlineData(ArchiveType.TarXz)]
        public async Task TestPackAsync(ArchiveType archiveType)
        {
            var nodes = new[] {FilePath};

            var processorMock = new Mock<IArchiveProcessor>();
            processorMock
                .Setup(m => m.PackAsync(nodes, OutputFilePath))
                .Verifiable();
            _autoMocker
                .Setup<IArchiveProcessorFactory, IArchiveProcessor>(m => m.Create(archiveType))
                .Returns(processorMock.Object);

            var service = _autoMocker.CreateInstance<ArchiveService>();
            await service.PackAsync(nodes, OutputFilePath, archiveType);

            processorMock
                .Verify(m => m.PackAsync(nodes, OutputFilePath), Times.Once);
        }

        [Theory]
        [InlineData(ArchiveType.Rar)]
        [InlineData(ArchiveType.Tar)]
        [InlineData(ArchiveType.Zip)]
        [InlineData(ArchiveType.SevenZip)]
        [InlineData(ArchiveType.GZip)]
        [InlineData(ArchiveType.TarBz)]
        [InlineData(ArchiveType.TarGz)]
        [InlineData(ArchiveType.TarLz)]
        [InlineData(ArchiveType.TarXz)]
        public async Task TestUnpackAsync(ArchiveType archiveType)
        {
            _autoMocker
                .Setup<IArchiveTypeMapper, ArchiveType?>(m => m.GetArchiveTypeFrom(FilePath))
                .Returns(archiveType);

            var processorMock = new Mock<IArchiveProcessor>();
            processorMock
                .Setup(m => m.UnpackAsync(FilePath, OutputDirPath))
                .Verifiable();
            _autoMocker
                .Setup<IArchiveProcessorFactory, IArchiveProcessor>(m => m.Create(archiveType))
                .Returns(processorMock.Object);

            var service = _autoMocker.CreateInstance<ArchiveService>();
            await service.UnpackAsync(FilePath, OutputDirPath);

            processorMock
                .Verify(m => m.UnpackAsync(FilePath, OutputDirPath), Times.Once);
        }

        [Fact]
        public async Task TestUnpackAsyncFailed()
        {
            var service = _autoMocker.CreateInstance<ArchiveService>();
            Task UnpackAsync() => service.UnpackAsync(FilePath, OutputDirPath);

            await Assert.ThrowsAsync<InvalidOperationException>(UnpackAsync);
        }
    }
}