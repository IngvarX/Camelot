using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Implementations.MainWindow.FavouriteDirectories;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Services.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.FavouriteDirectories
{
    public class FavouriteDirectoryViewModelTests
    {
        private const string DirPath = "/home/test";
        private const string DirName = "Test";

        private readonly AutoMocker _autoMocker;

        public FavouriteDirectoryViewModelTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Fact]
        public void TestProperties()
        {
            var driveModel = new DirectoryModel
            {
                Name = DirName
            };
            _autoMocker.Use(driveModel);

            var viewModel = _autoMocker.CreateInstance<FavouriteDirectoryViewModel>();

            Assert.Equal(driveModel.Name, viewModel.DirectoryName);
        }

        [Fact]
        public void TestOpenCommand()
        {
            var directoryModel = new DirectoryModel
            {
                FullPath = DirPath
            };
            _autoMocker.Use(directoryModel);

            var filePanelViewModelMock = new Mock<IFilesPanelViewModel>();
            filePanelViewModelMock
                .SetupSet(m => m.CurrentDirectory = directoryModel.FullPath)
                .Verifiable();
            _autoMocker
                .Setup<IFilesOperationsMediator, IFilesPanelViewModel>(m => m.ActiveFilesPanelViewModel)
                .Returns(filePanelViewModelMock.Object);

            var viewModel = _autoMocker.CreateInstance<FavouriteDirectoryViewModel>();

            Assert.True(viewModel.OpenCommand.CanExecute(null));
            viewModel.OpenCommand.Execute(null);

            filePanelViewModelMock
                .VerifySet(m => m.CurrentDirectory = directoryModel.FullPath, Times.Once);
        }

        [Fact]
        public void TestRemoveCommand()
        {
            var directoryModel = new DirectoryModel
            {
                FullPath = DirPath
            };
            _autoMocker.Use(directoryModel);

            _autoMocker
                .Setup<IFavouriteDirectoriesService>(m => m.RemoveDirectory(DirPath))
                .Verifiable();

            var viewModel = _autoMocker.CreateInstance<FavouriteDirectoryViewModel>();

            Assert.True(viewModel.RemoveCommand.CanExecute(null));
            viewModel.RemoveCommand.Execute(null);

            _autoMocker
                .Verify<IFavouriteDirectoriesService>(m => m.RemoveDirectory(DirPath));
        }
    }
}