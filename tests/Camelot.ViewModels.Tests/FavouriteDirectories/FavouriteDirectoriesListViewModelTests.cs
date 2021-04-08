using System.Collections.Generic;
using System.Linq;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.MainWindow.FavouriteDirectories;
using Camelot.ViewModels.Interfaces.MainWindow.Directories;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.FavouriteDirectories
{
    public class FavouriteDirectoriesListViewModelTests
    {
        private const string Dir = "Dir";

        private readonly AutoMocker _autoMocker;

        public FavouriteDirectoriesListViewModelTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Fact]
        public void TestLoad()
        {
            var favDirMock = new Mock<IFavouriteDirectoryViewModel>();
            _autoMocker
                .Setup<IFavouriteDirectoryViewModelFactory, IFavouriteDirectoryViewModel>(m => m.Create(Dir))
                .Returns(favDirMock.Object);
            _autoMocker
                .Setup<IFavouriteDirectoriesService, IReadOnlyCollection<string>>(m => m.FavouriteDirectories)
                .Returns(new[] {Dir});

            var viewModel = _autoMocker.CreateInstance<FavouriteDirectoriesListViewModel>();

            Assert.NotNull(viewModel.Directories);
            Assert.Single(viewModel.Directories);

            var dir = viewModel.Directories.Single();
            Assert.Equal(favDirMock.Object, dir);
        }

        [Fact]
        public void TestAdd()
        {
            var favDirMock = new Mock<IFavouriteDirectoryViewModel>();
            _autoMocker
                .Setup<IFavouriteDirectoryViewModelFactory, IFavouriteDirectoryViewModel>(m => m.Create(Dir))
                .Returns(favDirMock.Object);
            _autoMocker
                .Setup<IFavouriteDirectoriesService, IReadOnlyCollection<string>>(m => m.FavouriteDirectories)
                .Returns(new string[0]);

            var viewModel = _autoMocker.CreateInstance<FavouriteDirectoriesListViewModel>();

            Assert.NotNull(viewModel.Directories);
            Assert.Empty(viewModel.Directories);

            _autoMocker
                .GetMock<IFavouriteDirectoriesService>()
                .Raise(m => m.DirectoryAdded += null, CreateArgs());

            Assert.NotNull(viewModel.Directories);
            Assert.Single(viewModel.Directories);

            var dir = viewModel.Directories.Single();
            Assert.Equal(favDirMock.Object, dir);
        }

        [Fact]
        public void TestRemove()
        {
            var favDirMock = new Mock<IFavouriteDirectoryViewModel>();
            _autoMocker
                .Setup<IFavouriteDirectoryViewModelFactory, IFavouriteDirectoryViewModel>(m => m.Create(Dir))
                .Returns(favDirMock.Object);
            _autoMocker
                .Setup<IFavouriteDirectoriesService, IReadOnlyCollection<string>>(m => m.FavouriteDirectories)
                .Returns(new[] {Dir});

            var viewModel = _autoMocker.CreateInstance<FavouriteDirectoriesListViewModel>();

            Assert.NotNull(viewModel.Directories);
            Assert.Single(viewModel.Directories);

            _autoMocker
                .GetMock<IFavouriteDirectoriesService>()
                .Raise(m => m.DirectoryRemoved += null, CreateArgs());

            Assert.NotNull(viewModel.Directories);
            Assert.Empty(viewModel.Directories);
        }

        private FavouriteDirectoriesListChangedEventArgs CreateArgs() =>
            new FavouriteDirectoriesListChangedEventArgs(Dir);
    }
}