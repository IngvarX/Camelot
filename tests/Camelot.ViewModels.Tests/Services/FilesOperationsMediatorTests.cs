using System;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Services.Implementations;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests.Services
{
    public class FilesOperationsMediatorTests
    {
        private const string Directory = "Dir";
        private const string NewDirectory = "NewDir";

        [Fact]
        public void TestRegister()
        {
            var activeFilesPanelViewModelMock = new Mock<IFilesPanelViewModel>();
            activeFilesPanelViewModelMock
                .SetupGet(m => m.CurrentDirectory)
                .Returns(Directory);
            var inactiveFilesPanelViewModelMock = new Mock<IFilesPanelViewModel>();
            inactiveFilesPanelViewModelMock
                .SetupGet(m => m.CurrentDirectory)
                .Returns(NewDirectory);
            var directoryServiceMock = new Mock<IDirectoryService>();
            directoryServiceMock
                .SetupSet(m => m.SelectedDirectory = Directory)
                .Verifiable();
            var mediator = new FilesOperationsMediator(directoryServiceMock.Object);
            mediator.Register(activeFilesPanelViewModelMock.Object, inactiveFilesPanelViewModelMock.Object);

            Assert.Equal(activeFilesPanelViewModelMock.Object, mediator.ActiveFilesPanelViewModel);
            Assert.Equal(inactiveFilesPanelViewModelMock.Object, mediator.InactiveFilesPanelViewModel);
            Assert.Equal(NewDirectory, mediator.OutputDirectory);
            directoryServiceMock
                .VerifySet(m => m.SelectedDirectory = Directory, Times.Once);
        }

        [Fact]
        public void TestDirectoryServiceDirectoryChangedNoModels()
        {
            var directoryServiceMock = new Mock<IDirectoryService>();
            var mediator = new FilesOperationsMediator(directoryServiceMock.Object);
            var args = new SelectedDirectoryChangedEventArgs(Directory);
            directoryServiceMock
                .Raise(m => m.SelectedDirectoryChanged += null, args);

            Assert.Null(mediator.ActiveFilesPanelViewModel);
            Assert.Null(mediator.InactiveFilesPanelViewModel);
        }

        [Fact]
        public void TestDirectoryServiceDirectoryChanged()
        {
            var activeFilesPanelViewModelMock = new Mock<IFilesPanelViewModel>();
            activeFilesPanelViewModelMock
                .SetupSet(m => m.CurrentDirectory = Directory)
                .Verifiable();
            var inactiveFilesPanelViewModelMock = new Mock<IFilesPanelViewModel>();
            var directoryServiceMock = new Mock<IDirectoryService>();
            var mediator = new FilesOperationsMediator(directoryServiceMock.Object);
            mediator.Register(activeFilesPanelViewModelMock.Object, inactiveFilesPanelViewModelMock.Object);

            var args = new SelectedDirectoryChangedEventArgs(Directory);
            directoryServiceMock
                .Raise(m => m.SelectedDirectoryChanged += null, args);

            activeFilesPanelViewModelMock
                .VerifySet(m => m.CurrentDirectory = Directory, Times.Once);
        }

        [Fact]
        public void TestFilesPanelViewModelDirectoryChanged()
        {
            var activeFilesPanelViewModelMock = new Mock<IFilesPanelViewModel>();
            activeFilesPanelViewModelMock
                .SetupGet(m => m.CurrentDirectory)
                .Returns(Directory);
            var inactiveFilesPanelViewModelMock = new Mock<IFilesPanelViewModel>();
            var directoryServiceMock = new Mock<IDirectoryService>();
            directoryServiceMock
                .SetupSet(m => m.SelectedDirectory = NewDirectory)
                .Verifiable();
            var mediator = new FilesOperationsMediator(directoryServiceMock.Object);
            mediator.Register(activeFilesPanelViewModelMock.Object, inactiveFilesPanelViewModelMock.Object);

            activeFilesPanelViewModelMock
                .SetupGet(m => m.CurrentDirectory)
                .Returns(NewDirectory);
           activeFilesPanelViewModelMock.Raise(m => m.CurrentDirectoryChanged += null, EventArgs.Empty);

           directoryServiceMock
               .VerifySet(m => m.SelectedDirectory = NewDirectory, Times.Once);
        }

        [Fact]
        public void TestFilesPanelViewModelActivation()
        {
            var activeFilesPanelViewModelMock = new Mock<IFilesPanelViewModel>();
            activeFilesPanelViewModelMock
                .SetupGet(m => m.CurrentDirectory)
                .Returns(Directory);
            var inactiveFilesPanelViewModelMock = new Mock<IFilesPanelViewModel>();
            inactiveFilesPanelViewModelMock
                .SetupGet(m => m.CurrentDirectory)
                .Returns(NewDirectory);
            var directoryServiceMock = new Mock<IDirectoryService>();
            directoryServiceMock
                .SetupSet(m => m.SelectedDirectory = NewDirectory)
                .Verifiable();
            var mediator = new FilesOperationsMediator(directoryServiceMock.Object);
            mediator.Register(activeFilesPanelViewModelMock.Object, inactiveFilesPanelViewModelMock.Object);

            inactiveFilesPanelViewModelMock
                .Raise(m => m.Activated += null, EventArgs.Empty);
            Assert.Equal(inactiveFilesPanelViewModelMock.Object, mediator.ActiveFilesPanelViewModel);
            Assert.Equal(activeFilesPanelViewModelMock.Object, mediator.InactiveFilesPanelViewModel);
            directoryServiceMock
                .VerifySet(m => m.SelectedDirectory = NewDirectory, Times.Once);
        }

        [Fact]
        public void TestToggleSearchVisibility()
        {
            var searchViewModelMock = new Mock<ISearchViewModel>();
            searchViewModelMock
                .Setup(m => m.ToggleSearch())
                .Verifiable();
            var activeFilesPanelViewModelMock = new Mock<IFilesPanelViewModel>();
            activeFilesPanelViewModelMock
                .SetupGet(m => m.SearchViewModel)
                .Returns(searchViewModelMock.Object);
            var inactiveFilesPanelViewModelMock = new Mock<IFilesPanelViewModel>();
            var directoryServiceMock = new Mock<IDirectoryService>();

            var mediator = new FilesOperationsMediator(directoryServiceMock.Object);
            mediator.Register(activeFilesPanelViewModelMock.Object, inactiveFilesPanelViewModelMock.Object);

            mediator.ToggleSearchPanelVisibility();

            searchViewModelMock
                .Verify(m => m.ToggleSearch(), Times.Once);
        }
    }
}