using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
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
        private const string FileName = "file.txt";
        private const string MetaData = "metadata";
        private const string MetaDataPath = "/.local/share/Trash/info/file.txt.trashinfo";

        private readonly AutoMocker _autoMocker;

        public LinuxTrashCanServiceTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Fact]
        public async Task TestMoveToTrash()
        {
            var now = DateTime.UtcNow;
            _autoMocker
                .Setup<IDriveService, DriveModel>(m => m.GetFileDrive(It.IsAny<string>()))
                .Returns(new DriveModel {RootDirectory = "/"});
            _autoMocker
                .Setup<IOperationsService>(m => m.MoveAsync(It.IsAny<IReadOnlyDictionary<string, string>>()))
                .Verifiable();
            _autoMocker
                .Setup<IPathService, string>(m => m.GetFileName(FilePath))
                .Returns(FileName);
            _autoMocker
                .Setup<IPathService, string>(m => m.Combine(It.IsAny<string>(), It.IsAny<string>()))
                .Returns<string, string>((a, b) => $"{a}/{b}");
            _autoMocker
                .Setup<IFileService, bool>(m => m.CheckIfExists(FilePath))
                .Returns(true);
            _autoMocker
                .Setup<IFileService>(m => m.WriteTextAsync(MetaDataPath, MetaData))
                .Verifiable();
            _autoMocker
                .Setup<IEnvironmentService, string>(m => m.GetEnvironmentVariable("UID"))
                .Returns("42");
            _autoMocker
                .Setup<IEnvironmentService, string>(m => m.GetEnvironmentVariable("HOME"))
                .Returns(HomePath);
            _autoMocker
                .Setup<IDirectoryService, bool>(m => m.CheckIfExists(It.IsAny<string>()))
                .Returns(true);
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
                .Verify<IOperationsService>(m => m.MoveAsync(It.IsAny<IReadOnlyDictionary<string, string>>()), Times.Once);
            _autoMocker
                .Verify<IFileService>(m => m.WriteTextAsync(It.IsAny<string>(), MetaData), Times.Once);
            builderMock
                .Verify(m => m.WithFilePath(FilePath), Times.Once);
            builderMock
                .Verify(m => m.WithRemovingDateTime(now), Times.Once);
        }
    }
}