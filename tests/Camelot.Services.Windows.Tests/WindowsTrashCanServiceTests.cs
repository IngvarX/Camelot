using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Drives;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Operations;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Windows.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Windows.Tests
{
    public class WindowsTrashCanServiceTests
    {
        private const string Sid = "Sid";
        private const string FilePath = "C:\\file.txt";
        private const string FileName = "file.txt";
        private const string GeneratedFileName = "GEN123";
        private const string MetadataFileName = "$IGEN123.txt";
        private const long FileSize = 42;
        private const string MetaData = "metadata";

        private readonly AutoMocker _autoMocker;

        public WindowsTrashCanServiceTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Theory]
        [InlineData("C:\\", "C:\\$Recycle.Bin\\Sid\\$IGEN123.txt", "C:\\$Recycle.Bin\\Sid\\$RGEN123.txt", new[] {FilePath})]
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
                .Returns<string>(p => p.Split("\\")[^1]);
            _autoMocker
                .Setup<IPathService, string>(m => m.Combine(It.IsAny<string>(), It.IsAny<string>()))
                .Returns<string, string>((a, b) => $"{a}\\{b}");
            _autoMocker
                .Setup<IPathService, string>(m => m.GetExtension(FileName))
                .Returns("txt");
            _autoMocker
                .Setup<IFileService, bool>(m => m.CheckIfExists(It.IsAny<string>()))
                .Returns<string>(existingFiles.Contains);
            _autoMocker
                .Setup<IFileService, IReadOnlyList<FileModel>>(m =>
                    m.GetFiles(It.Is<IReadOnlyList<string>>(l => l.Contains(FilePath))))
                .Returns<IReadOnlyList<string>>(l => l.Select(p => new FileModel
                {
                    FullPath = p,
                    SizeBytes = FileSize
                }).ToArray());
            _autoMocker
                .Setup<IFileService>(m => m.WriteBytesAsync(MetadataFileName, It.Is<byte[]>(a => Encoding.UTF8.GetString(a) == MetaData)))
                .Verifiable();
            _autoMocker
                .Setup<IProcessService, Task<string>>(m => m.ExecuteAndGetOutputAsync("whoami", "/user"))
                .Returns(Task.FromResult(Sid));
            _autoMocker
                .Setup<IDateTimeProvider, DateTime>(m => m.Now)
                .Returns(now);
            _autoMocker
                .Setup<IWindowsTrashCanNodeNameGenerator, string>(m => m.Generate())
                .Returns(GeneratedFileName);
            var builderMock = new Mock<IWindowsRemovedFileMetadataBuilder>();
            builderMock
                .Setup(m => m.WithFilePath(FilePath))
                .Returns(builderMock.Object)
                .Verifiable();
            builderMock
                .Setup(m => m.WithFileSize(FileSize))
                .Returns(builderMock.Object)
                .Verifiable();
            builderMock
                .Setup(m => m.WithRemovingDateTime(now))
                .Returns(builderMock.Object)
                .Verifiable();
            builderMock
                .Setup(m => m.Build())
                .Returns(Encoding.UTF8.GetBytes(MetaData));
            _autoMocker
                .Setup<IWindowsRemovedFileMetadataBuilderFactory, IWindowsRemovedFileMetadataBuilder>(m => m.Create())
                .Returns(builderMock.Object);

            var windowsTrashCanService = _autoMocker.CreateInstance<WindowsTrashCanService>();
            await windowsTrashCanService.MoveToTrashAsync(new[] {FilePath}, CancellationToken.None);

            _autoMocker
                .Verify<IOperationsService>(m => m.MoveAsync(
                    It.Is<IReadOnlyDictionary<string, string>>(d =>
                        d.ContainsKey(FilePath) && d[FilePath] == newFilePath)), Times.Once);
            _autoMocker
                .Verify<IFileService>(m => m.WriteBytesAsync(metadataPath,
                    It.Is<byte[]>(a => Encoding.UTF8.GetString(a) == MetaData)), Times.Once);
            builderMock
                .Verify(m => m.WithFilePath(FilePath), Times.Once);
            builderMock
                .Verify(m => m.WithFileSize(FileSize), Times.Once);
            builderMock
                .Verify(m => m.WithRemovingDateTime(now), Times.Once);
        }
    }
}