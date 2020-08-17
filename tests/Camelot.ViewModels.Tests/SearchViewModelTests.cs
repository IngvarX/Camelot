using System;
using System.Threading.Tasks;
using Camelot.ViewModels.Configuration;
using Camelot.ViewModels.Implementations.MainWindow;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Specifications;
using Xunit;

namespace Camelot.ViewModels.Tests
{
    public class SearchViewModelTests
    {
        [Fact]
        public void TestDefaults()
        {
            var configuration = new SearchViewModelConfiguration();
            var viewModel = new SearchViewModel(configuration);

            Assert.False(viewModel.IsSearchEnabled);
            Assert.False(viewModel.IsRegexSearchEnabled);
            Assert.False(viewModel.IsSearchCaseSensitive);
            Assert.Equal(string.Empty, viewModel.SearchText);
        }

        [Fact]
        public void TestToggle()
        {
            const string searchText = "text";
            var configuration = new SearchViewModelConfiguration();
            
            var viewModel = new SearchViewModel(configuration);
            Assert.False(viewModel.IsSearchEnabled);

            viewModel.SearchText = searchText;
            viewModel.ToggleSearch();
            Assert.True(viewModel.IsSearchEnabled);
            Assert.Equal(string.Empty, viewModel.SearchText);

            viewModel.SearchText = searchText;
            viewModel.ToggleSearch();
            Assert.False(viewModel.IsSearchEnabled);
            Assert.Equal(string.Empty, viewModel.SearchText);

            viewModel.SearchText = searchText;
            viewModel.ToggleSearch();
            Assert.True(viewModel.IsSearchEnabled);
            Assert.Equal(string.Empty, viewModel.SearchText);
        }

        [Theory]
        [InlineData(false, false, typeof(EmptySpecification))]
        [InlineData(false, true, typeof(EmptySpecification))]
        [InlineData(true, true, typeof(NodeNameRegexSpecification))]
        [InlineData(true, false, typeof(NodeNameTextSpecification))]
        public void TestSpecification(bool isSearchEnabled, bool isRegexSearchEnabled,
            Type specificationType)
        {
            var configuration = new SearchViewModelConfiguration();
            var viewModel = new SearchViewModel(configuration)
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
            var configuration = new SearchViewModelConfiguration
            {
                TimeoutMs = 10
            };
            var viewModel = new SearchViewModel(configuration);
            viewModel.SearchSettingsChanged += (sender, args) => taskCompletionSource.SetResult(true);

            viewModel.SearchText = "test";

            var task = await Task.WhenAny(Task.Delay(1000), taskCompletionSource.Task);
            Assert.Equal(taskCompletionSource.Task, task);
        }

        [Fact]
        public async Task TestSearchCaseChanged()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            var configuration = new SearchViewModelConfiguration
            {
                TimeoutMs = 10
            };
            var viewModel = new SearchViewModel(configuration);
            viewModel.SearchSettingsChanged += (sender, args) => taskCompletionSource.SetResult(true);

            viewModel.IsSearchCaseSensitive = true;

            var task = await Task.WhenAny(Task.Delay(1000), taskCompletionSource.Task);
            Assert.Equal(taskCompletionSource.Task, task);
        }

        [Fact]
        public async Task TestRegexChanged()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            var configuration = new SearchViewModelConfiguration
            {
                TimeoutMs = 10
            };
            var viewModel = new SearchViewModel(configuration);
            viewModel.SearchSettingsChanged += (sender, args) => taskCompletionSource.SetResult(true);

            viewModel.IsRegexSearchEnabled = true;

            var task = await Task.WhenAny(Task.Delay(1000), taskCompletionSource.Task);
            Assert.Equal(taskCompletionSource.Task, task);
        }
    }
}