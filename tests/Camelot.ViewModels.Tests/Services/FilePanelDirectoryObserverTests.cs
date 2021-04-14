using Camelot.ViewModels.Services.Implementations;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.Services
{
    public class FilePanelDirectoryObserverTests
    {
        private const string Directory = "Dir";
        private const string NewDirectory = "NewDir";

        private readonly AutoMocker _autoMocker;

        public FilePanelDirectoryObserverTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Fact]
        public void TestProperty()
        {
            var observer = _autoMocker.CreateInstance<FilePanelDirectoryObserver>();

            observer.CurrentDirectory = Directory;

            Assert.Equal(Directory, observer.CurrentDirectory);
        }

        [Fact]
        public void TestEventNotFired()
        {
            var observer = _autoMocker.CreateInstance<FilePanelDirectoryObserver>();

            var isCallbackCalled = false;
            observer.CurrentDirectory = Directory;
            observer.CurrentDirectoryChanged += (sender, args) => isCallbackCalled = true;

            observer.CurrentDirectory = Directory;
            Assert.False(isCallbackCalled);
        }

        [Fact]
        public void TestEventFired()
        {
            var observer = _autoMocker.CreateInstance<FilePanelDirectoryObserver>();

            var isCallbackCalled = false;
            observer.CurrentDirectory = Directory;
            observer.CurrentDirectoryChanged += (sender, args) => isCallbackCalled = true;

            observer.CurrentDirectory = NewDirectory;
            Assert.True(isCallbackCalled);
        }
    }
}