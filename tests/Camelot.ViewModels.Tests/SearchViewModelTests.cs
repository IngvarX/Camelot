using System;
using System.Threading.Tasks;
using Camelot.Services.Environment.Interfaces;
using Camelot.ViewModels.Configuration;
using Camelot.ViewModels.Implementations.MainWindow;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Specifications;
using Camelot.ViewModels.Services.Interfaces;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests
{
    public class SearchViewModelTests
    {
        private const string SearchText = "text";

        [Fact]
        public void TestDefaults()
        {
            var regexServiceMock = new Mock<IRegexService>();
            var resourceProviderMock = new Mock<IResourceProvider>();
            var configuration = new SearchViewModelConfiguration();
            var viewModel = new SearchViewModel(regexServiceMock.Object, resourceProviderMock.Object, configuration);

            Assert.False(viewModel.IsSearchEnabled);
            Assert.False(viewModel.IsRegexSearchEnabled);
            Assert.False(viewModel.IsSearchCaseSensitive);
            Assert.Equal(string.Empty, viewModel.SearchText);
        }

        [Fact]
        public void TestToggle()
        {
            var configuration = new SearchViewModelConfiguration();

            var regexServiceMock = new Mock<IRegexService>();
            var resourceProviderMock = new Mock<IResourceProvider>();

            var viewModel = new SearchViewModel(regexServiceMock.Object, resourceProviderMock.Object, configuration);
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
            var resourceProviderMock = new Mock<IResourceProvider>();
            var configuration = new SearchViewModelConfiguration();
            var viewModel = new SearchViewModel(regexServiceMock.Object, resourceProviderMock.Object, configuration)
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
            var resourceProviderMock = new Mock<IResourceProvider>();
            var configuration = new SearchViewModelConfiguration
            {
                TimeoutMs = 10
            };
            var viewModel = new SearchViewModel(regexServiceMock.Object, resourceProviderMock.Object, configuration);
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
            var resourceProviderMock = new Mock<IResourceProvider>();
            var configuration = new SearchViewModelConfiguration
            {
                TimeoutMs = 10
            };
            var viewModel = new SearchViewModel(regexServiceMock.Object, resourceProviderMock.Object, configuration);
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
            var resourceProviderMock = new Mock<IResourceProvider>();
            var configuration = new SearchViewModelConfiguration
            {
                TimeoutMs = 10
            };
            var viewModel = new SearchViewModel(regexServiceMock.Object, resourceProviderMock.Object, configuration);
            viewModel.SearchSettingsChanged += (sender, args) => taskCompletionSource.SetResult(true);

            viewModel.SearchText = SearchText;
            viewModel.IsRegexSearchEnabled = true;

            var task = await Task.WhenAny(Task.Delay(1000), taskCompletionSource.Task);
            Assert.Equal(isRegexValid, taskCompletionSource.Task == task);
        }
    }
}