using Camelot.Services.Abstractions;
using Camelot.ViewModels.Services.Implementations;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.Services;

public class FilePanelDirectoryObserverTests
{
    private const string Directory = "Dir";
    private const string DirectoryWithSlash = "Dir/";
    private const string NewDirectory = "NewDir";

    private readonly AutoMocker _autoMocker;

    public FilePanelDirectoryObserverTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Fact]
    public void TestProperty()
    {
        _autoMocker
            .Setup<IPathService, string>(m => m.RightTrimPathSeparators(It.IsAny<string>()))
            .Returns<string>(s => s);

        var observer = _autoMocker.CreateInstance<FilePanelDirectoryObserver>();

        observer.CurrentDirectory = Directory;

        Assert.Equal(Directory, observer.CurrentDirectory);
    }

    [Theory]
    [InlineData(Directory, Directory)]
    [InlineData(DirectoryWithSlash, Directory)]
    public void TestEventNotFired(string newDirectory, string trimmedPath)
    {
        _autoMocker
            .Setup<IPathService, string>(m => m.RightTrimPathSeparators(newDirectory))
            .Returns(trimmedPath);
        _autoMocker
            .Setup<IPathService, string>(m => m.RightTrimPathSeparators(Directory))
            .Returns(trimmedPath);

        var observer = _autoMocker.CreateInstance<FilePanelDirectoryObserver>();

        var isCallbackCalled = false;
        observer.CurrentDirectory = Directory;
        observer.CurrentDirectoryChanged += (sender, args) => isCallbackCalled = true;

        observer.CurrentDirectory = newDirectory;
        Assert.False(isCallbackCalled);
    }

    [Fact]
    public void TestEventFired()
    {
        _autoMocker
            .Setup<IPathService, string>(m => m.RightTrimPathSeparators(It.IsAny<string>()))
            .Returns<string>(s => s);

        var observer = _autoMocker.CreateInstance<FilePanelDirectoryObserver>();

        var isCallbackCalled = false;
        observer.CurrentDirectory = Directory;
        observer.CurrentDirectoryChanged += (sender, args) => isCallbackCalled = true;

        observer.CurrentDirectory = NewDirectory;
        Assert.True(isCallbackCalled);
    }
}