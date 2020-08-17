using System;
using System.IO;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Environment.Interfaces;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Tests
{
    public class DirectoryServiceTests
    {
        private const string DirectoryName = "Directory";
        private const string ParentDirectoryName = "Parent";
        private const string NotExistingDirectoryName = "MissingDirectory";

        private readonly AutoMocker _autoMocker;

        public DirectoryServiceTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Fact]
        public void TestDirectoryCreationSuccess()
        {
            _autoMocker
                .Setup<IEnvironmentDirectoryService>(m => m.CreateDirectory(DirectoryName))
                .Verifiable();
            var directoryService = _autoMocker.CreateInstance<DirectoryService>();

            var result = directoryService.Create(DirectoryName);
            Assert.True(result);
            _autoMocker
                .Verify<IEnvironmentDirectoryService>(m => m.CreateDirectory(DirectoryName));
        }

        [Fact]
        public void TestDirectoryCreationFail()
        {
            _autoMocker
                .Setup<IEnvironmentDirectoryService>(m => m.CreateDirectory(DirectoryName))
                .Throws<InvalidOperationException>()
                .Verifiable();
            var directoryService = _autoMocker.CreateInstance<DirectoryService>();

            var result = directoryService.Create(DirectoryName);
            Assert.False(result);
            _autoMocker
                .Verify<IEnvironmentDirectoryService>(m => m.CreateDirectory(DirectoryName));
        }

        [Fact]
        public void TestCurrentDirectoryUpdateFailed()
        {
            var directoryService = _autoMocker.CreateInstance<DirectoryService>();
            directoryService.SelectedDirectory = DirectoryName;
            var isCallbackCalled = false;
            directoryService.SelectedDirectoryChanged += (sender, args) => isCallbackCalled = true;
            directoryService.SelectedDirectory = DirectoryName;

            Assert.False(isCallbackCalled);
        }

        [Fact]
        public void TestGetParentDirectory()
        {
            var directoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            _autoMocker
                .Setup<IEnvironmentDirectoryService, DirectoryInfo>(m => m.GetDirectory(DirectoryName))
                .Returns(directoryInfo);
            var directoryService = _autoMocker.CreateInstance<DirectoryService>();
            var parentDirectory = directoryService.GetParentDirectory(DirectoryName);

            Assert.NotNull(parentDirectory);
            Assert.Equal(directoryInfo.Parent.FullName, parentDirectory.FullPath);
        }

        [Fact]
        public void TestGetRootParentDirectory()
        {
            _autoMocker
                .Setup<IEnvironmentDirectoryService, string>(m => m.GetCurrentDirectory())
                .Returns(DirectoryName);
            _autoMocker
                .Setup<IPathService, string>(m => m.GetPathRoot(DirectoryName))
                .Returns(ParentDirectoryName);
            var directoryService = _autoMocker.CreateInstance<DirectoryService>();
            var directory = directoryService.GetAppRootDirectory();

            Assert.Equal(ParentDirectoryName, directory);
        }

        [Fact]
        public void TestSelectedDirectoryEventChangedCreation()
        {
            var callbackCalled = false;
            void DirectoryServiceOnSelectedDirectoryChanged(object sender, SelectedDirectoryChangedEventArgs e) =>
                callbackCalled = e.NewDirectory == DirectoryName;

            var directoryService = _autoMocker.CreateInstance<DirectoryService>();
            directoryService.SelectedDirectoryChanged += DirectoryServiceOnSelectedDirectoryChanged;
            directoryService.SelectedDirectory = DirectoryName;

            Assert.True(callbackCalled);
        }

        [Theory]
        [InlineData(DirectoryName, true)]
        [InlineData(NotExistingDirectoryName, false)]
        public void TestDirectoryExists(string directory, bool isExist)
        {
            _autoMocker
                .Setup<IEnvironmentDirectoryService, bool>(m => m.CheckIfExists(directory))
                .Returns(isExist);
            var directoryService = _autoMocker.CreateInstance<DirectoryService>();
            var result = directoryService.CheckIfExists(directory);

            Assert.Equal(isExist, result);
        }

        [Fact]
        public void TestDirectoryRemove()
        {
            _autoMocker
                .Setup<IEnvironmentDirectoryService>(m => m.Delete(DirectoryName, true))
                .Verifiable();
            var directoryService = _autoMocker.CreateInstance<DirectoryService>();

            directoryService.RemoveRecursively(DirectoryName);

            _autoMocker
                .Verify<IEnvironmentDirectoryService>(m => m.Delete(DirectoryName, true));
        }
    }
}