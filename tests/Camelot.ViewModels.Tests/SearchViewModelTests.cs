using System;
using System.Threading.Tasks;
using Camelot.Avalonia.Interfaces;
using Camelot.Services.Environment.Interfaces;
using Camelot.ViewModels.Configuration;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Specifications;
using Camelot.ViewModels.Services.Interfaces;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests
{
    public class SearchViewModelTests
    {
        private const string SearchText = "text";
        private const string Error = "error";
        private const string ResourceName = "ResourceName";

        private readonly IResourceProvider _resourceProvider;
        private readonly IApplicationDispatcher _applicationDispatcher;

        public SearchViewModelTests()
        {
            var resourceProviderMock = new Mock<IResourceProvider>();
            resourceProviderMock
                .Setup(m => m.GetResourceByName(ResourceName))
                .Returns(Error);

            _resourceProvider = resourceProviderMock.Object;

            var applicationDispatcherMock = new Mock<IApplicationDispatcher>();
            applicationDispatcherMock
                .Setup(m => m.Dispatch(It.IsAny<Action>()))
                .Callback<Action>(action => action());

            _applicationDispatcher = applicationDispatcherMock.Object;
        }

        [Fact]
        public void TestDefaults()
        {
            var regexServiceMock = new Mock<IRegexService>();
            var configuration = GetConfiguration();
            var viewModel = new SearchViewModel(regexServiceMock.Object, _resourceProvider, _applicationDispatcher, configuration);

            Assert.False(viewModel.IsSearchEnabled);
            Assert.False(viewModel.IsRegexSearchEnabled);
            Assert.False(viewModel.IsSearchCaseSensitive);
            Assert.Equal(string.Empty, viewModel.SearchText);
        }

        [Fact]
        public void TestToggle()
        {
            var configuration = GetConfiguration();

            var regexServiceMock = new Mock<IRegexService>();

            var viewModel = new SearchViewModel(regexServiceMock.Object, _resourceProvider, _applicationDispatcher, configuration);
            Assert.False(viewModel.IsSearchEnabled);

            viewModel.SearchText = SearchText;
            viewModel.ToggleSearch();
            Assert.True(viewModel.IsSearchEnabled);
            Assert.Equal(string.Empty, viewModel.SearchText);

            viewModel.SearchText = SearchText;
            viewModel.ToggleSearch();
            Assert.False(viewModel.IsSearchEnabled);
            Assert.Equal(string.Empty, viewModel.SearchText);

            viewModel.SearchText = SearchText;
            viewModel.ToggleSearch();
            Assert.True(viewModel.IsSearchEnabled);
            Assert.Equal(string.Empty, viewModel.SearchText);
        }

        [Theory]
        [InlineData(false, false, typeof(EmptySpecification), false)]
        [InlineData(false, true, typeof(EmptySpecification), false)]
        [InlineData(true, true, typeof(EmptySpecification), false)]
        [InlineData(true, true, typeof(NodeNameRegexSpecification), true)]
        [InlineData(true, false, typeof(NodeNameTextSpecification), false)]
        public void TestSpecification(bool isSearchEnabled, bool isRegexSearchEnabled,
            Type specificationType, bool isRegexValid)
        {
            var regexServiceMock = new Mock<IRegexService>();
            regexServiceMock
                .Setup(m => m.ValidateRegex(It.IsAny<string>()))
                .Returns(isRegexValid);
            var configuration = GetConfiguration();
            var viewModel = new SearchViewModel(regexServiceMock.Object, _resourceProvider, _applicationDispatcher, configuration)
            {
                IsSearchEnabled = isSearchEnabled,
                IsRegexSearchEnabled = isRegexSearchEnabled
            };

            var specification = viewModel.GetSpecification();

            Assert.IsType(specificationType, specification);
        }

        [Fact]
        public async Task TestTextChangedChanged()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            var regexServiceMock = new Mock<IRegexService>();
            var configuration = GetConfiguration();
            var viewModel = new SearchViewModel(regexServiceMock.Object, _resourceProvider, _applicationDispatcher, configuration);
            viewModel.SearchSettingsChanged += (sender, args) => taskCompletionSource.SetResult(true);

            viewModel.SearchText = "test";

            var task = await Task.WhenAny(Task.Delay(1000), taskCompletionSource.Task);
            Assert.Equal(taskCompletionSource.Task, task);
        }

        [Fact]
        public async Task TestSearchCaseChanged()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            var regexServiceMock = new Mock<IRegexService>();
            var configuration = GetConfiguration();
            var viewModel = new SearchViewModel(regexServiceMock.Object, _resourceProvider, _applicationDispatcher, configuration);
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

            var regexServiceMock = new Mock<IRegexService>();
            regexServiceMock
                .Setup(m => m.ValidateRegex(SearchText))
                .Returns(isRegexValid);
            var configuration = GetConfiguration();
            var viewModel = new SearchViewModel(regexServiceMock.Object, _resourceProvider, _applicationDispatcher, configuration);
            viewModel.SearchSettingsChanged += (sender, args) => taskCompletionSource.SetResult(true);

            viewModel.SearchText = SearchText;
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
}