using System;
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
            var viewModel = new SearchViewModel();

            Assert.False(viewModel.IsSearchEnabled);
            Assert.False(viewModel.IsRegexSearchEnabled);
            Assert.False(viewModel.IsSearchCaseSensitive);
            Assert.Null(viewModel.SearchText);
        }

        [Theory]
        [InlineData(false, false, typeof(EmptySpecification))]
        [InlineData(false, true, typeof(EmptySpecification))]
        [InlineData(true, true, typeof(NodeNameRegexSpecification))]
        [InlineData(true, false, typeof(NodeNameTextSpecification))]
        public void TestSpecification(bool isSearchEnabled, bool isRegexSearchEnabled,
            Type specificationType)
        {
            var viewModel = new SearchViewModel
            {
                IsSearchEnabled = isSearchEnabled,
                IsRegexSearchEnabled = isRegexSearchEnabled
            };

            var specification = viewModel.GetSpecification();

            Assert.IsType(specificationType, specification);
        }
    }
}