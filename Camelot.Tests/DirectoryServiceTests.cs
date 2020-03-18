using System;
using System.IO;
using Camelot.Services.EventArgs;
using Camelot.Services.Implementations;
using Camelot.Services.Interfaces;
using Xunit;

namespace Camelot.Tests
{
    public class DirectoryServiceTests : IDisposable
    {
        private const string DirectoryName = nameof(DirectoryServiceTests);

        private readonly IDirectoryService _directoryService;

        private static string NewDirectory => Path.Combine(CurrentDirectory, DirectoryName);

        private static string CurrentDirectory => Directory.GetCurrentDirectory();

        public DirectoryServiceTests()
        {
            _directoryService = new DirectoryService();
        }

        [Fact]
        public void TestDirectoryCreationFailed()
        {
            Assert.False(_directoryService.CreateDirectory(null));
            Assert.False(_directoryService.CreateDirectory(string.Empty));
            Assert.False(_directoryService.CreateDirectory(" "));
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

            Assert.True(_directoryService.CreateDirectory(DirectoryName));
            Assert.True(Directory.Exists(NewDirectory));
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

        public void Dispose()
        {
            if (Directory.Exists(NewDirectory))
            {
                Directory.Delete(NewDirectory);
            }
        }
    }
}