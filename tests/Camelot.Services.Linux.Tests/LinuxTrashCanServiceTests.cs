using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Drives;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Operations;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Linux.Interfaces.Builders;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Linux.Tests
{
    public class LinuxTrashCanServiceTests
    {
        private const string FilePath = "/home/file.txt";
        private const string HomePath = "/home/camelot";
        private const string MetaData = "metadata";

        private readonly AutoMocker _autoMocker;

        public LinuxTrashCanServiceTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Theory]
        [InlineData("/", "/home/camelot/.local/share/Trash/info/file.txt.trashinfo", "/home/camelot/.local/share/Trash/files/file.txt", new[] {FilePath})]
        [InlineData("/", "/home/camelot/.local/share/Trash/info/file.txt (1).trashinfo", "/home/camelot/.local/share/Trash/files/file.txt (1)", new[] {FilePath, "/home/camelot/.local/share/Trash/files/file.txt"})]
        [InlineData("/test", "/test/.Trash-42/info/file.txt.trashinfo", "/test/.Trash-42/files/file.txt", new[] {FilePath})]
        [InlineData("/test", "/test/.Trash-42/info/file.txt (1).trashinfo", "/test/.Trash-42/files/file.txt (1)", new[] {FilePath, "/test/.Trash-42/files/file.txt"})]
        public async Task TestMoveToTrash(string volume, string metadataPath, string newFilePath, string[] existingFiles)
        {
            var now = DateTime.UtcNow;
            _autoMocker
                .Setup<IMountedDriveService, DriveModel>(m => m.GetFileDrive(It.IsAny<string>()))
                .Returns(new DriveModel {RootDirectory = volume});
            _autoMocker
                .Setup<IOperationsService>(m => m.MoveAsync(
                    It.Is<IReadOnlyDictionary<string, string>>(d => d.ContainsKey(FilePath) && d[FilePath] == newFilePath)))
                .Verifiable();
            _autoMocker
                .Setup<IPathService, string>(m => m.GetFileName(It.IsAny<string>()))
                .Returns<string>(p => p.Split("/")[^1]);
            _autoMocker
                .Setup<IPathService, string>(m => m.Combine(It.IsAny<string>(), It.IsAny<string>()))
                .Returns<string, string>((a, b) => $"{a}/{b}");
            _autoMocker
                .Setup<IFileService, bool>(m => m.CheckIfExists(It.IsAny<string>()))
                .Returns<string>(existingFiles.Contains);
            _autoMocker
                .Setup<IFileService>(m => m.WriteTextAsync(metadataPath, MetaData))
                .Verifiable();
            _autoMocker
                .Setup<IEnvironmentService, string>(m => m.GetEnvironmentVariable("UID"))
                .Returns("42");
            _autoMocker
                .Setup<IHomeDirectoryProvider, string>(m => m.HomeDirectoryPath)
                .Returns(HomePath);
            _autoMocker
                .Setup<IDateTimeProvider, DateTime>(m => m.Now)
                .Returns(now);
            var builderMock = new Mock<ILinuxRemovedFileMetadataBuilder>();
            builderMock
                .Setup(m => m.WithFilePath(FilePath))
                .Returns(builderMock.Object)
                .Verifiable();
            builderMock
                .Setup(m => m.WithRemovingDateTime(now))
                .Returns(builderMock.Object)
                .Verifiable();
            builderMock
                .Setup(m => m.Build())
                .Returns(MetaData);
            _autoMocker
                .Setup<ILinuxRemovedFileMetadataBuilderFactory, ILinuxRemovedFileMetadataBuilder>(m => m.Create())
                .Returns(builderMock.Object);

            var linuxTrashCanService = _autoMocker.CreateInstance<LinuxTrashCanService>();
            await linuxTrashCanService.MoveToTrashAsync(new[] {FilePath}, CancellationToken.None);

            _autoMocker
                .Verify<IOperationsService>(m => m.MoveAsync(
                    It.Is<IReadOnlyDictionary<string, string>>(d =>
                        d.ContainsKey(FilePath) && d[FilePath] == newFilePath)), Times.Once);
            _autoMocker
                .Verify<IFileService>(m => m.WriteTextAsync(metadataPath, MetaData), Times.Once);
            builderMock
                .Verify(m => m.WithFilePath(FilePath), Times.Once);
            builderMock
                .Verify(m => m.WithRemovingDateTime(now), Times.Once);
        }
    }
}