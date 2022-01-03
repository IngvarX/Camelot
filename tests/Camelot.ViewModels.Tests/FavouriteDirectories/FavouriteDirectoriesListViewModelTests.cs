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

namespace Camelot.ViewModels.Tests.FavouriteDirectories;

public class FavouriteDirectoriesListViewModelTests
{
    private const string Dir = "Dir";
    private const string SecondDir = "SecondDir";
    private const string ThirdDir = "ThirdDir";

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

    [Theory]
    [InlineData(0, 2, 1, 2, 0)]
    [InlineData(1, 1, 0, 1, 2)]
    [InlineData(1, 2, 0, 2, 1)]
    [InlineData(2, 1, 0, 2, 1)]
    [InlineData(0, 1, 1, 0, 2)]
    [InlineData(2, 2, 0, 1, 2)]
    [InlineData(0, 0, 0, 1, 2)]
    public void TestMoveRequested(int sourceIndex, int targetIndex, int firstIndex, int secondIndex, int thirdIndex)
    {
        var favDirMock = new Mock<IFavouriteDirectoryViewModel>();
        _autoMocker
            .Setup<IFavouriteDirectoryViewModelFactory, IFavouriteDirectoryViewModel>(m => m.Create(Dir))
            .Returns(favDirMock.Object);
        var favSecondDirMock = new Mock<IFavouriteDirectoryViewModel>();
        _autoMocker
            .Setup<IFavouriteDirectoryViewModelFactory, IFavouriteDirectoryViewModel>(m => m.Create(SecondDir))
            .Returns(favSecondDirMock.Object);
        var favThirdDirMock = new Mock<IFavouriteDirectoryViewModel>();
        _autoMocker
            .Setup<IFavouriteDirectoryViewModelFactory, IFavouriteDirectoryViewModel>(m => m.Create(ThirdDir))
            .Returns(favThirdDirMock.Object);
        _autoMocker
            .Setup<IFavouriteDirectoriesService, IReadOnlyCollection<string>>(m => m.FavouriteDirectories)
            .Returns(new[] {Dir, SecondDir, ThirdDir});

        var viewModel = _autoMocker.CreateInstance<FavouriteDirectoriesListViewModel>();

        Assert.NotNull(viewModel.Directories);
        Assert.Equal(3, viewModel.Directories.Count());

        var mocks = new[] { favDirMock, favSecondDirMock, favThirdDirMock };
        mocks[sourceIndex]
            .Raise(m => m.MoveRequested += null,
                CreateMoveArgs(mocks[targetIndex].Object));

        _autoMocker
            .Verify<IFavouriteDirectoriesService>(m => m.MoveDirectory(sourceIndex, targetIndex),
                Times.Once);

        Assert.Equal(3, viewModel.Directories.Count());

        var models = viewModel.Directories.ToArray();
        Assert.Equal(mocks[firstIndex].Object, models[0]);
        Assert.Equal(mocks[secondIndex].Object, models[1]);
        Assert.Equal(mocks[thirdIndex].Object, models[2]);
    }

    private static FavouriteDirectoriesListChangedEventArgs CreateArgs() => new(Dir);

    private static FavouriteDirectoryMoveRequestedEventArgs CreateMoveArgs(IFavouriteDirectoryViewModel model)
        => new(model);
}