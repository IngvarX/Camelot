using System.Linq;
using Camelot.Services.Abstractions;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.MainWindow.FavouriteDirectories;
using Camelot.ViewModels.Interfaces.MainWindow.Directories;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests.FavouriteDirectories
{
    public class FavouriteDirectoriesListViewModelTests
    {
        private const string HomeDir = "HomeDir";

        [Fact]
        public void TestHomeDir()
        {
            var favDirMock = new Mock<IFavouriteDirectoryViewModel>();
            var factoryMock = new Mock<IFavouriteDirectoryViewModelFactory>();
            factoryMock
                .Setup(m => m.Create(HomeDir))
                .Returns(favDirMock.Object)
                .Verifiable();
            var homeDirProviderMock = new Mock<IHomeDirectoryProvider>();
            homeDirProviderMock
                .SetupGet(m => m.HomeDirectoryPath)
                .Returns(HomeDir);

            var viewModel = new FavouriteDirectoriesListViewModel(factoryMock.Object, homeDirProviderMock.Object);

            Assert.NotNull(viewModel.Directories);
            Assert.Single(viewModel.Directories);

            var homeDir = viewModel.Directories.Single();
            Assert.Equal(favDirMock.Object, homeDir);
        }
    }
}