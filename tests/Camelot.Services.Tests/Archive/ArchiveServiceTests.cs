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
        private const string FileWithTarPath = "File.tar";
        private const string OutputFilePath = "OutputFile";
        private const string OutputDirPath = "OutputDir";

        private readonly AutoMocker _autoMocker;

        public ArchiveServiceTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(ArchiveType.Tar, true)]
        [InlineData(ArchiveType.Zip, true)]
        [InlineData(ArchiveType.SevenZip, true)]
        [InlineData(ArchiveType.Gz, true)]
        [InlineData(ArchiveType.TarBz2, true)]
        [InlineData(ArchiveType.TarGz, true)]
        [InlineData(ArchiveType.Bz2, true)]
        [InlineData(ArchiveType.TarXz, true)]
        [InlineData(ArchiveType.Xz, true)]
        [InlineData(ArchiveType.TarLz, true)]
        [InlineData(ArchiveType.Lz, true)]
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
        [InlineData(ArchiveType.Tar)]
        [InlineData(ArchiveType.Zip)]
        [InlineData(ArchiveType.SevenZip)]
        [InlineData(ArchiveType.Gz)]
        [InlineData(ArchiveType.TarBz2)]
        [InlineData(ArchiveType.TarGz)]
        [InlineData(ArchiveType.Bz2)]
        [InlineData(ArchiveType.TarXz)]
        [InlineData(ArchiveType.Xz)]
        [InlineData(ArchiveType.TarLz)]
        [InlineData(ArchiveType.Lz)]
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
        [InlineData(ArchiveType.Tar, FileWithTarPath, OutputDirPath)]
        [InlineData(ArchiveType.Zip, FilePath, OutputDirPath)]
        [InlineData(ArchiveType.SevenZip, FilePath, OutputDirPath)]
        [InlineData(ArchiveType.Gz, FilePath, OutputDirPath)]
        [InlineData(ArchiveType.TarBz2, FileWithTarPath, OutputDirPath)]
        [InlineData(ArchiveType.TarGz, FileWithTarPath, OutputDirPath)]
        [InlineData(ArchiveType.TarLz, FileWithTarPath, OutputDirPath)]
        [InlineData(ArchiveType.TarXz, FileWithTarPath, OutputDirPath)]
        [InlineData(ArchiveType.Tar, FileWithTarPath, null)]
        [InlineData(ArchiveType.Zip, FilePath, null)]
        [InlineData(ArchiveType.SevenZip, FilePath, null)]
        [InlineData(ArchiveType.Gz, FilePath, null)]
        [InlineData(ArchiveType.TarBz2, FileWithTarPath, null)]
        [InlineData(ArchiveType.TarGz, FileWithTarPath, null)]
        [InlineData(ArchiveType.TarLz, FileWithTarPath, null)]
        [InlineData(ArchiveType.TarXz, FileWithTarPath, null)]
        public async Task ExtractToNewDirectoryAsync(ArchiveType archiveType, string filePath, string outputDirPath)
        {
            _autoMocker
                .Setup<IArchiveTypeMapper, ArchiveType?>(m => m.GetArchiveTypeFrom(filePath))
                .Returns(archiveType);
            _autoMocker
                .Setup<IFileNameGenerationService, string>(m => m.GenerateFullNameWithoutExtension(FilePath))
                .Returns(outputDirPath);
            if (outputDirPath is null)
            {
                _autoMocker
                    .Setup<IPathService, string>(m => m.GetParentDirectory(filePath))
                    .Returns(OutputDirPath);
            }

            _autoMocker
                .Setup<IOperationsService>(m => m.ExtractAsync(filePath, OutputDirPath, archiveType))
                .Verifiable();

            var service = _autoMocker.CreateInstance<ArchiveService>();
            await service.ExtractToNewDirectoryAsync(filePath);

            _autoMocker
                .Verify<IOperationsService>(m => m.ExtractAsync(filePath, OutputDirPath, archiveType), Times.Once);
        }

        [Theory]
        [InlineData(ArchiveType.Tar, OutputDirPath)]
        [InlineData(ArchiveType.Zip, OutputDirPath)]
        [InlineData(ArchiveType.SevenZip, OutputDirPath)]
        [InlineData(ArchiveType.Gz, OutputDirPath)]
        [InlineData(ArchiveType.TarBz2, OutputDirPath)]
        [InlineData(ArchiveType.TarGz, OutputDirPath)]
        [InlineData(ArchiveType.TarLz, OutputDirPath)]
        [InlineData(ArchiveType.TarXz, OutputDirPath)]
        [InlineData(ArchiveType.Tar, null)]
        [InlineData(ArchiveType.Zip, null)]
        [InlineData(ArchiveType.SevenZip, null)]
        [InlineData(ArchiveType.Gz, null)]
        [InlineData(ArchiveType.TarBz2, null)]
        [InlineData(ArchiveType.TarGz, null)]
        [InlineData(ArchiveType.TarLz, null)]
        [InlineData(ArchiveType.TarXz, null)]
        public async Task TestExtractAsync(ArchiveType archiveType, string outputDirPath)
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
        public async Task TestExtractAsyncFailed()
        {
            var service = _autoMocker.CreateInstance<ArchiveService>();
            Task ExtractAsync() => service.ExtractAsync(FilePath, OutputDirPath);

            await Assert.ThrowsAsync<InvalidOperationException>(ExtractAsync);
        }

        [Fact]
        public async Task TestExtractToNewDirectoryAsyncFailed()
        {
            var service = _autoMocker.CreateInstance<ArchiveService>();
            Task ExtractAsync() => service.ExtractToNewDirectoryAsync(FilePath);

            await Assert.ThrowsAsync<InvalidOperationException>(ExtractAsync);
        }
    }
}