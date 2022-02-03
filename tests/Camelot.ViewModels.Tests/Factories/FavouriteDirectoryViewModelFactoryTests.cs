using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Factories.Implementations;
using Camelot.ViewModels.Implementations.MainWindow.FavouriteDirectories;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.Factories;

public class FavouriteDirectoryViewModelFactoryTests
{
    private const string DirPath = "Dir";
    private const string DirName = "Name";

    private readonly AutoMocker _autoMocker;

    public FavouriteDirectoryViewModelFactoryTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Fact]
    public void TestCreate()
    {
        _autoMocker
            .Setup<IDirectoryService, DirectoryModel>(m => m.GetDirectory(DirPath))
            .Returns(new DirectoryModel
            {
                Name = DirName
            });

        var factory = _autoMocker.CreateInstance<FavouriteDirectoryViewModelFactory>();

        var viewModel = factory.Create(DirPath);
        Assert.IsType<FavouriteDirectoryViewModel>(viewModel);

        var favDirViewModel = (FavouriteDirectoryViewModel) viewModel;

        Assert.Equal(DirName, favDirViewModel.DirectoryName);
    }
}