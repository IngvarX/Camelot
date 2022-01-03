using System;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Factories.Implementations;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Enums;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.Factories;

public class SuggestedPathViewModelFactoryTests
{
    private readonly AutoMocker _autoMocker;

    public SuggestedPathViewModelFactoryTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Theory]
    [InlineData("/home/camel", "/home/camelot", SuggestedPathType.Directory, "camelot", "/home", SuggestionType.Directory)]
    [InlineData("/home/camel", "/home/camelot", SuggestedPathType.FavouriteDirectory, "camelot", "/home", SuggestionType.FavouriteDirectory)]
    [InlineData("/home/camel", "/home/camelot", SuggestedPathType.FavouriteDirectory, null, "/home", SuggestionType.FavouriteDirectory)]
    public void TestCreate(string searchText, string fullPath, SuggestedPathType suggestedPathType,
        string relativePath, string parentDir, SuggestionType suggestionType)
    {
        _autoMocker
            .Setup<IPathService, string>(m => m.GetParentDirectory(searchText))
            .Returns(parentDir);
        _autoMocker
            .Setup<IPathService, string>(m => m.GetRelativePath(parentDir, fullPath))
            .Returns(relativePath);
        _autoMocker
            .Setup<IPathService, string>(m => m.LeftTrimPathSeparators(relativePath))
            .Returns(relativePath);

        var factory = _autoMocker.CreateInstance<SuggestedPathViewModelFactory>();
        var model = new SuggestionModel(fullPath, suggestionType);
        var viewModel = factory.Create(searchText, model);

        Assert.NotNull(viewModel);
        Assert.IsType<SuggestedPathViewModel>(viewModel);

        var suggestedPathViewModel = (SuggestedPathViewModel) viewModel;
        Assert.Equal(suggestedPathType, suggestedPathViewModel.Type);
        Assert.Equal(relativePath, suggestedPathViewModel.Text);
        Assert.Equal(fullPath, suggestedPathViewModel.FullPath);
    }

    [Theory]
    [InlineData("/home/camel", "/home/camelot", "camelot", "/home", 42)]
    public void TestCreateThrows(string searchText, string fullPath, string relativePath,
        string parentDir, byte suggestionType)
    {
        _autoMocker
            .Setup<IPathService, string>(m => m.GetParentDirectory(searchText))
            .Returns(parentDir);
        _autoMocker
            .Setup<IPathService, string>(m => m.GetRelativePath(parentDir, fullPath))
            .Returns(relativePath);
        _autoMocker
            .Setup<IPathService, string>(m => m.LeftTrimPathSeparators(relativePath))
            .Returns(relativePath);

        var factory = _autoMocker.CreateInstance<SuggestedPathViewModelFactory>();
        var model = new SuggestionModel(fullPath, (SuggestionType) suggestionType);

        void Create() => factory.Create(searchText, model);

        Assert.Throws<ArgumentOutOfRangeException>(Create);
    }
}