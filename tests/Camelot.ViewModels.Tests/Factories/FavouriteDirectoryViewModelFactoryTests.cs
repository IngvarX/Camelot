using Camelot.Services.Abstractions;
using Camelot.ViewModels.Factories.Implementations;
using Camelot.ViewModels.Implementations.MainWindow.FavouriteDirectories;
using Camelot.ViewModels.Services.Interfaces;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests.Factories
{
    public class FavouriteDirectoryViewModelFactoryTests
    {
        private const string DirPath = "Dir";

        [Fact]
        public void TestCreate()
        {
            var mediatorMock = new Mock<IFilesOperationsMediator>();
            var directoryServiceMock = new Mock<IDirectoryService>();
            directoryServiceMock
                .Setup(m => m.GetDirectory(DirPath))
                .Verifiable();

            var factory = new FavouriteDirectoryViewModelFactory(mediatorMock.Object, directoryServiceMock.Object);

            var viewModel = factory.Create(DirPath);
            Assert.IsType<FavouriteDirectoryViewModel>(viewModel);

            directoryServiceMock
                .Verify(m => m.GetDirectory(DirPath), Times.Once);
        }
    }
}