using System;
using System.IO;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.EventArgs;
using Xunit;

namespace Camelot.Services.Tests
{
    public class DirectoryServiceTests : IDisposable
    {
        private const string DirectoryName = nameof(DirectoryServiceTests);
        private const string NotExistingDirectoryName = "MissingDirectory";

        private readonly IDirectoryService _directoryService;

        private static string NewDirectory => Path.Combine(CurrentDirectory, DirectoryName);

        private static string CurrentDirectory => Directory.GetCurrentDirectory();

        public DirectoryServiceTests()
        {
            var pathService = new PathService();
            _directoryService = new DirectoryService(pathService);
        }

        [Fact]
        public void TestDirectoryCreationFailed()
        {
            Assert.False(_directoryService.Create(null));
            Assert.False(_directoryService.Create(string.Empty));
        }

        [Fact]
        public void TestCurrentDirectoryUpdateFailed()
        {
            _directoryService.SelectedDirectory = CurrentDirectory;
            var isCallbackCalled = false;
            _directoryService.SelectedDirectoryChanged += (sender, args) => isCallbackCalled = true;
            _directoryService.SelectedDirectory = CurrentDirectory;

            Assert.False(isCallbackCalled);
        }

        [Fact]
        public void TestDirectoryCreation()
        {
            _directoryService.SelectedDirectory = CurrentDirectory;
            Assert.True(_directoryService.SelectedDirectory == CurrentDirectory);

            Assert.True(_directoryService.Create(DirectoryName));
            Assert.True(Directory.Exists(NewDirectory));

            var directories = _directoryService.GetChildDirectories(CurrentDirectory);
            Assert.Contains(directories, d => d.Name == DirectoryName);
        }

        [Fact]
        public void TestGetParentDirectory()
        {
            var parentDirectory = _directoryService.GetParentDirectory(CurrentDirectory);

            Assert.NotNull(parentDirectory);

            var children = _directoryService.GetChildDirectories(parentDirectory.FullPath);
            Assert.Contains(children, dm => dm.FullPath == CurrentDirectory);
        }

        [Fact]
        public void TestGetRootParentDirectory()
        {
            var directory = _directoryService.GetAppRootDirectory();
            var parentDirectory = _directoryService.GetParentDirectory(directory);

            Assert.Null(parentDirectory);
        }

        [Fact]
        public void TestSelectedDirectoryEventChangedCreation()
        {
            var callbackCalled = false;
            void DirectoryServiceOnSelectedDirectoryChanged(object sender, SelectedDirectoryChangedEventArgs e)
            {
                callbackCalled = true;
                Assert.True(e.NewDirectory == CurrentDirectory);
            }

            _directoryService.SelectedDirectoryChanged += DirectoryServiceOnSelectedDirectoryChanged;
            _directoryService.SelectedDirectory = CurrentDirectory;

            Assert.True(callbackCalled);
        }

        [Fact]
        public void TestDirectoryExists()
        {
            Assert.True(_directoryService.CheckIfExists(CurrentDirectory));
            Assert.False(_directoryService.CheckIfExists(NotExistingDirectoryName));
            Assert.False(_directoryService.CheckIfExists(string.Empty));
            Assert.False(_directoryService.CheckIfExists(null));
        }

        public void Dispose()
        {
            if (Directory.Exists(NewDirectory))
            {
                Directory.Delete(NewDirectory);
            }
        }
    }
}