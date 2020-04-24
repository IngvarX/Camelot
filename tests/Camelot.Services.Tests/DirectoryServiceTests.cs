using System;
using System.IO;
using Camelot.Services.EventArgs;
using Camelot.Services.Implementations;
using Camelot.Services.Interfaces;
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
            Assert.False(_directoryService.Create(" "));
        }

        [Fact]
        public void TestCurrentDirectoryUpdateFailed()
        {
            Assert.Throws<ArgumentNullException>(() => _directoryService.SelectedDirectory = null);
            Assert.Throws<ArgumentNullException>(() => _directoryService.SelectedDirectory = string.Empty);
            Assert.Throws<ArgumentNullException>(() => _directoryService.SelectedDirectory = " ");
        }

        [Fact]
        public void TestDirectoryCreation()
        {
            _directoryService.SelectedDirectory = CurrentDirectory;
            Assert.True(_directoryService.SelectedDirectory == CurrentDirectory);

            Assert.True(_directoryService.Create(DirectoryName));
            Assert.True(Directory.Exists(NewDirectory));

            var directories = _directoryService.GetDirectories(CurrentDirectory);
            Assert.Contains(directories, d => d.Name == DirectoryName);
        }

        [Fact]
        public void TestGetParentDirectory()
        {
            var parentDirectory = _directoryService.GetParentDirectory(CurrentDirectory);

            Assert.NotNull(parentDirectory);

            var children = _directoryService.GetDirectories(parentDirectory.FullPath);
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