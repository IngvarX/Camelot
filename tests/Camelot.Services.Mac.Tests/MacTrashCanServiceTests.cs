using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Drives;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Operations;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Mac.Tests
{
    public class MacTrashCanServiceTests
    {
        private const string FilePath = "/home/file.txt";
        private const string HomePath = "/home/camelot";
        private const string FileName = "file.txt";

        private readonly AutoMocker _autoMocker;

        public MacTrashCanServiceTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Theory]
        [InlineData("/", "/home/camelot/.Trash/file.txt", new[] {FilePath})]
        [InlineData("/", "/home/camelot/.Trash/file.txt (1)", new[] {FilePath, "/home/camelot/.Trash/file.txt"})]
        [InlineData("/test", "/test/.Trashes/file.txt", new[] {FilePath})]
        [InlineData("/test", "/test/.Trashes/file.txt (1)", new[] {FilePath, "/test/.Trashes/file.txt"})]
        public async Task TestMoveToTrash(string volume, string newFilePath, string[] existingFiles)
        {
            _autoMocker
                .Setup<IMountedDriveService, DriveModel>(m => m.GetFileDrive(It.IsAny<string>()))
                .Returns(new DriveModel {RootDirectory = volume});
            _autoMocker
                .Setup<IOperationsService>(m => m.MoveAsync(
                    It.Is<IReadOnlyDictionary<string, string>>(d =>
                        d.ContainsKey(FilePath) && d[FilePath] == newFilePath)))
                .Verifiable();
            _autoMocker
                .Setup<IPathService, string>(m => m.GetFileName(FilePath))
                .Returns(FileName);
            _autoMocker
                .Setup<IPathService, string>(m => m.Combine(It.IsAny<string>(), It.IsAny<string>()))
                .Returns<string, string>((a, b) => $"{a}/{b}");
            _autoMocker
                .Setup<IFileService, bool>(m => m.CheckIfExists(It.IsAny<string>()))
                .Returns<string>(existingFiles.Contains);
            _autoMocker
                .Setup<IHomeDirectoryProvider, string>(m => m.HomeDirectoryPath)
                .Returns(HomePath);

            var macTrashCanService = _autoMocker.CreateInstance<MacTrashCanService>();
            await macTrashCanService.MoveToTrashAsync(new[] {FilePath}, CancellationToken.None);

            _autoMocker
                .Verify<IOperationsService>(m => m.MoveAsync(
                    It.Is<IReadOnlyDictionary<string, string>>(d =>
                        d.ContainsKey(FilePath) && d[FilePath] == newFilePath)), Times.Once);
        }
    }
}