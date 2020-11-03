using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Environment.Interfaces;
using Camelot.Tests.Common.Extensions;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Tests
{
    public class DirectoryServiceTests
    {
        private const string DirectoryName = "Directory";
        private const string ParentDirectoryName = "Parent";
        private const string NewDirectoryName = "New";
        private const string NotExistingDirectoryName = "MissingDirectory";
        private const string File = "File";

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
        public void TestCurrentDirectoryUpdateSucceeded()
        {
            var directoryService = _autoMocker.CreateInstance<DirectoryService>();
            directoryService.SelectedDirectory = DirectoryName;
            var isCallbackCalled = false;
            directoryService.SelectedDirectoryChanged += (sender, args) => isCallbackCalled = true;
            directoryService.SelectedDirectory = ParentDirectoryName;

            Assert.True(isCallbackCalled);
            Assert.Equal(ParentDirectoryName, directoryService.SelectedDirectory);
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
        public void TestGetFilesRecursively()
        {
            _autoMocker
                .Setup<IEnvironmentDirectoryService, IEnumerable<string>>(m => m.EnumerateFilesRecursively(DirectoryName))
                 .Returns(new[] {File})
                .Verifiable();
            var directoryService = _autoMocker.CreateInstance<DirectoryService>();
            var files = directoryService.GetFilesRecursively(DirectoryName);

            Assert.NotNull(files);
            Assert.Single(files);
            Assert.Equal(File, files.Single());
        }

        [Fact]
        public void TestGetDirectoriesRecursively()
        {
            _autoMocker
                .Setup<IEnvironmentDirectoryService, IEnumerable<string>>(m => m.EnumerateDirectoriesRecursively(DirectoryName))
                .Returns(new[] {NewDirectoryName})
                .Verifiable();
            var directoryService = _autoMocker.CreateInstance<DirectoryService>();
            var directories = directoryService.GetDirectoriesRecursively(DirectoryName);

            Assert.NotNull(directories);
            Assert.Single(directories);
            Assert.Equal(NewDirectoryName, directories.Single());
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

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void TestDirectoryRemove(bool throws, bool expected)
        {
            _autoMocker
                .Setup<IEnvironmentDirectoryService>(m => m.Delete(DirectoryName, true))
                .Verifiable();
            if (throws)
            {
                _autoMocker
                    .Setup<IEnvironmentDirectoryService>(m => m.Delete(DirectoryName, true))
                    .Throws<InvalidOperationException>();
            }

            _autoMocker.MockLogError();

            var directoryService = _autoMocker.CreateInstance<DirectoryService>();

            var actual = directoryService.RemoveRecursively(DirectoryName);

            Assert.Equal(expected, actual);
            _autoMocker
                .Verify<IEnvironmentDirectoryService>(m => m.Delete(DirectoryName, true));
            _autoMocker.VerifyLogError(throws ? Times.Once() : Times.Never());
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void TestDirectoryRename(bool throws, bool expected)
        {
            _autoMocker
                .Setup<IPathService, string>(m => m.GetParentDirectory(DirectoryName))
                .Returns(ParentDirectoryName);
            _autoMocker
                .Setup<IPathService, string>(m => m.Combine(ParentDirectoryName, NewDirectoryName))
                .Returns(NotExistingDirectoryName);
            _autoMocker
                .Setup<IEnvironmentDirectoryService>(m => m.Move(DirectoryName, NotExistingDirectoryName))
                .Verifiable();
            if (throws)
            {
                _autoMocker
                    .Setup<IEnvironmentDirectoryService>(m => m.Move(DirectoryName, NotExistingDirectoryName))
                    .Throws<InvalidOperationException>();
            }

            _autoMocker.MockLogError();

            var directoryService = _autoMocker.CreateInstance<DirectoryService>();

            var actual = directoryService.Rename(DirectoryName, NewDirectoryName);

            Assert.Equal(expected, actual);
            _autoMocker
                .Verify<IEnvironmentDirectoryService>(m => m.Move(DirectoryName, NotExistingDirectoryName));
            _autoMocker.VerifyLogError(throws ? Times.Once() : Times.Never());
        }
    }
}