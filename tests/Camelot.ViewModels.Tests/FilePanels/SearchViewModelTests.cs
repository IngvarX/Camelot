using System;
using System.Threading.Tasks;
using Camelot.Avalonia.Interfaces;
using Camelot.Services.Environment.Interfaces;
using Camelot.ViewModels.Configuration;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Specifications;
using Camelot.ViewModels.Services.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.FilePanels;

public class SearchViewModelTests
{
    private const string SearchText = "text";
    private const string Error = "error";
    private const string ResourceName = "ResourceName";

    private readonly AutoMocker _autoMocker;

    public SearchViewModelTests()
    {
        _autoMocker = new AutoMocker();

        _autoMocker
            .Setup<IResourceProvider, string>(m => m.GetResourceByName(ResourceName))
            .Returns(Error);

        _autoMocker
            .Setup<IApplicationDispatcher>(m => m.Dispatch(It.IsAny<Action>()))
            .Callback<Action>(action => action());

        _autoMocker.Use(GetConfiguration());
    }

    [Fact]
    public void TestDefaults()
    {
        var viewModel = _autoMocker.CreateInstance<SearchViewModel>();

        Assert.False(viewModel.IsSearchEnabled);
        Assert.False(viewModel.IsRegexSearchEnabled);
        Assert.False(viewModel.IsSearchCaseSensitive);
        Assert.False(viewModel.IsRecursiveSearchEnabled);
        Assert.Equal(string.Empty, viewModel.SearchText);
    }

    [Fact]
    public void TestToggleCommand()
    {
        var viewModel = _autoMocker.CreateInstance<SearchViewModel>();

        viewModel.ToggleSearch();
        viewModel.SearchText = SearchText;
        viewModel.IsRecursiveSearchEnabled = true;

        Assert.True(viewModel.ToggleSearchCommand.CanExecute(null));

        viewModel.ToggleSearchCommand.Execute(null);

        Assert.False(viewModel.IsSearchEnabled);
        Assert.Equal(string.Empty, viewModel.SearchText);
        Assert.True(viewModel.IsRecursiveSearchEnabled);
    }

    [Fact]
    public void TestToggle()
    {
        var viewModel = _autoMocker.CreateInstance<SearchViewModel>();
        Assert.False(viewModel.IsSearchEnabled);

        viewModel.SearchText = SearchText;
        viewModel.IsRecursiveSearchEnabled = true;
        viewModel.ToggleSearch();
        Assert.True(viewModel.IsSearchEnabled);
        Assert.Equal(string.Empty, viewModel.SearchText);
        Assert.True(viewModel.IsRecursiveSearchEnabled);

        viewModel.SearchText = SearchText;
        viewModel.IsRecursiveSearchEnabled = true;
        viewModel.ToggleSearch();
        Assert.False(viewModel.IsSearchEnabled);
        Assert.Equal(string.Empty, viewModel.SearchText);
        Assert.True(viewModel.IsRecursiveSearchEnabled);

        viewModel.SearchText = SearchText;
        viewModel.IsRecursiveSearchEnabled = true;
        viewModel.ToggleSearch();
        Assert.True(viewModel.IsSearchEnabled);
        Assert.Equal(string.Empty, viewModel.SearchText);
        Assert.True(viewModel.IsRecursiveSearchEnabled);
    }

    [Theory]
    [InlineData("Text", false, false, typeof(EmptySpecification), false)]
    [InlineData("Text", false, true, typeof(EmptySpecification), false)]
    [InlineData("Text", true, true, typeof(EmptySpecification), false)]
    [InlineData("", true, true, typeof(EmptySpecification), false)]
    [InlineData(null, true, true, typeof(EmptySpecification), false)]
    [InlineData("", true, false, typeof(EmptySpecification), false)]
    [InlineData(null, true, false, typeof(EmptySpecification), false)]
    [InlineData("Text", true, true, typeof(NodeNameRegexSpecification), true)]
    [InlineData("Text", true, false, typeof(NodeNameTextSpecification), false)]
    public void TestSpecification(string searchText, bool isSearchEnabled, bool isRegexSearchEnabled,
        Type specificationType, bool isRegexValid)
    {
        _autoMocker
            .Setup<IRegexService, bool>(m => m.ValidateRegex(It.IsAny<string>()))
            .Returns(isRegexValid);

        var viewModel = _autoMocker.CreateInstance<SearchViewModel>();

        viewModel.IsSearchEnabled = isSearchEnabled;
        viewModel.IsRegexSearchEnabled = isRegexSearchEnabled;
        viewModel.SearchText = searchText;

        var specification = viewModel.GetSpecification();

        Assert.IsType(specificationType, specification);
    }

    [Fact]
    public async Task TestTextChangedChanged()
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();

        var viewModel = _autoMocker.CreateInstance<SearchViewModel>();
        viewModel.SearchSettingsChanged += (sender, args) => taskCompletionSource.SetResult(true);

        viewModel.SearchText = "test";

        var task = await Task.WhenAny(Task.Delay(1000), taskCompletionSource.Task);
        Assert.Equal(taskCompletionSource.Task, task);
    }

    [Fact]
    public async Task TestSearchCaseChanged()
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();

        var viewModel = _autoMocker.CreateInstance<SearchViewModel>();
        viewModel.SearchSettingsChanged += (sender, args) => taskCompletionSource.SetResult(true);

        viewModel.IsSearchCaseSensitive = true;

        var task = await Task.WhenAny(Task.Delay(1000), taskCompletionSource.Task);
        Assert.Equal(taskCompletionSource.Task, task);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task TestRegexChanged(bool isRegexValid)
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();

        _autoMocker
            .Setup<IRegexService, bool>(m => m.ValidateRegex(SearchText))
            .Returns(isRegexValid);

        var viewModel = _autoMocker.CreateInstance<SearchViewModel>();

        viewModel.SearchText = SearchText;
        viewModel.IsSearchEnabled = true;

        viewModel.SearchSettingsChanged += (sender, args) => taskCompletionSource.SetResult(true);
        viewModel.IsRegexSearchEnabled = true;

        var task = await Task.WhenAny(Task.Delay(1000), taskCompletionSource.Task);
        Assert.Equal(isRegexValid, taskCompletionSource.Task == task);
    }

    private static SearchViewModelConfiguration GetConfiguration() =>
        new SearchViewModelConfiguration
        {
            InvalidRegexResourceName = ResourceName,
            TimeoutMs = 10
        };
}