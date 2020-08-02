using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Operations;
using Camelot.Services.Environment.Interfaces;
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
        [InlineData("/", "/home/camelot/.Trash/")]
        [InlineData("/test", "/test/.Trashes/")]
        public async Task TestMoveToTrash(string volume, string newFilePath)
        {
            var now = DateTime.UtcNow;
            _autoMocker
                .Setup<IDriveService, DriveModel>(m => m.GetFileDrive(It.IsAny<string>()))
                .Returns(new DriveModel {RootDirectory = volume});
            var isCallbackCalled = false;
            _autoMocker
                .Setup<IOperationsService>(m => m.MoveAsync(
                    It.IsAny<IReadOnlyDictionary<string, string>>()))
                .Callback<IReadOnlyDictionary<string, string>>(d => 
                    isCallbackCalled = d.ContainsKey(FilePath) && d[FilePath] == newFilePath);
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
                .Setup<IDirectoryService, bool>(m => m.CheckIfExists(It.IsAny<string>()))
                .Returns(true);
            _autoMocker
                .Setup<IDateTimeProvider, DateTime>(m => m.Now)
                .Returns(now);
            _autoMocker
                .Setup<IEnvironmentService, string>(m => m.GetEnvironmentVariable("HOME"))
                .Returns(HomePath);

            var macTrashCanService = _autoMocker.CreateInstance<MacTrashCanService>();
            await macTrashCanService.MoveToTrashAsync(new[] {FilePath}, CancellationToken.None);

            Assert.True(isCallbackCalled);
        }
    }
}