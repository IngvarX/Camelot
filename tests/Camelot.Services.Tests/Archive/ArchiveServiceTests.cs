using System;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Operations;
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
            var actual = service.CheckIfNodeIsArchive(FilePath);

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

            _autoMocker
                .Setup<IOperationsService>(m => m.PackAsync(nodes, OutputFilePath, archiveType))
                .Verifiable();

            var service = _autoMocker.CreateInstance<ArchiveService>();
            await service.PackAsync(nodes, OutputFilePath, archiveType);

            _autoMocker
                .Verify<IOperationsService>(m => m.PackAsync(nodes, OutputFilePath, archiveType), Times.Once);
        }

        [Theory]
        [InlineData(ArchiveType.Rar, OutputDirPath)]
        [InlineData(ArchiveType.Tar, OutputDirPath)]
        [InlineData(ArchiveType.Zip, OutputDirPath)]
        [InlineData(ArchiveType.SevenZip, OutputDirPath)]
        [InlineData(ArchiveType.GZip, OutputDirPath)]
        [InlineData(ArchiveType.TarBz, OutputDirPath)]
        [InlineData(ArchiveType.TarGz, OutputDirPath)]
        [InlineData(ArchiveType.TarLz, OutputDirPath)]
        [InlineData(ArchiveType.TarXz, OutputDirPath)]
        [InlineData(ArchiveType.Rar, null)]
        [InlineData(ArchiveType.Tar, null)]
        [InlineData(ArchiveType.Zip, null)]
        [InlineData(ArchiveType.SevenZip, null)]
        [InlineData(ArchiveType.GZip, null)]
        [InlineData(ArchiveType.TarBz, null)]
        [InlineData(ArchiveType.TarGz, null)]
        [InlineData(ArchiveType.TarLz, null)]
        [InlineData(ArchiveType.TarXz, null)]
        public async Task TestUnpackAsync(ArchiveType archiveType, string outputDirPath)
        {
            _autoMocker
                .Setup<IArchiveTypeMapper, ArchiveType?>(m => m.GetArchiveTypeFrom(FilePath))
                .Returns(archiveType);
            if (outputDirPath is null)
            {
                _autoMocker
                    .Setup<IPathService, string>(m => m.GetParentDirectory(FilePath))
                    .Returns(OutputDirPath);
            }

            _autoMocker
                .Setup<IOperationsService>(m => m.ExtractAsync(FilePath, OutputDirPath, archiveType))
                .Verifiable();

            var service = _autoMocker.CreateInstance<ArchiveService>();
            await service.ExtractAsync(FilePath, outputDirPath);

            _autoMocker
                .Verify<IOperationsService>(m => m.ExtractAsync(FilePath, OutputDirPath, archiveType), Times.Once);
        }

        [Fact]
        public async Task TestUnpackAsyncFailed()
        {
            var service = _autoMocker.CreateInstance<ArchiveService>();
            Task UnpackAsync() => service.ExtractAsync(FilePath, OutputDirPath);

            await Assert.ThrowsAsync<InvalidOperationException>(UnpackAsync);
        }
    }
}