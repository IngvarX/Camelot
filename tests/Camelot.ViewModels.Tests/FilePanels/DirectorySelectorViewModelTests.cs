using System.Collections.Generic;
using System.Linq;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.FilePanels
{
    public class DirectorySelectorViewModelTests
    {
        private const string Dir = "Dir";

        private readonly AutoMocker _autoMocker;

        public DirectorySelectorViewModelTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Theory]
        [InlineData(true, 1, 0)]
        [InlineData(false, 0, 1)]
        public void ToggleFavouriteStatusCommand(bool isFavourite, int addCallsCount, int removeCallsCount)
        {
            _autoMocker
                .Setup<IDirectoryService, bool>(m => m.CheckIfExists(Dir))
                .Returns(true);
            _autoMocker
                .Setup<IFavouriteDirectoriesService, bool>(m => m.ContainsDirectory(Dir))
                .Returns(isFavourite);

            var viewModel = _autoMocker.CreateInstance<DirectorySelectorViewModel>();

            viewModel.CurrentDirectory = Dir;

            Assert.Equal(isFavourite, viewModel.IsFavouriteDirectory);
            Assert.True(viewModel.ToggleFavouriteStatusCommand.CanExecute(null));
            viewModel.ToggleFavouriteStatusCommand.Execute(null);

            _autoMocker
                .Verify<IFavouriteDirectoriesService>(m => m.AddDirectory(Dir),
                    Times.Exactly(addCallsCount));
            _autoMocker
                .Verify<IFavouriteDirectoriesService>(m => m.RemoveDirectory(Dir),
                    Times.Exactly(removeCallsCount));
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void ToggleFavouriteDirectoryAdd(bool stateBefore, bool stateAfter)
        {
            _autoMocker
                .Setup<IDirectoryService, bool>(m => m.CheckIfExists(Dir))
                .Returns(true);
            _autoMocker
                .Setup<IFavouriteDirectoriesService, bool>(m => m.ContainsDirectory(Dir))
                .Returns(stateBefore);

            var viewModel = _autoMocker.CreateInstance<DirectorySelectorViewModel>();

            viewModel.CurrentDirectory = Dir;

            Assert.Equal(stateBefore, viewModel.IsFavouriteDirectory);

            _autoMocker
                .Setup<IFavouriteDirectoriesService, bool>(m => m.ContainsDirectory(Dir))
                .Returns(stateAfter);

            _autoMocker
                .GetMock<IFavouriteDirectoriesService>()
                .Raise(m => m.DirectoryAdded += null, new FavouriteDirectoriesListChangedEventArgs(Dir));

            Assert.Equal(stateAfter, viewModel.IsFavouriteDirectory);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void ToggleFavouriteDirectoryRemove(bool stateBefore, bool stateAfter)
        {
            _autoMocker
                .Setup<IDirectoryService, bool>(m => m.CheckIfExists(Dir))
                .Returns(true);
            _autoMocker
                .Setup<IFavouriteDirectoriesService, bool>(m => m.ContainsDirectory(Dir))
                .Returns(stateBefore);

            var viewModel = _autoMocker.CreateInstance<DirectorySelectorViewModel>();

            viewModel.CurrentDirectory = Dir;

            Assert.Equal(stateBefore, viewModel.IsFavouriteDirectory);

            _autoMocker
                .Setup<IFavouriteDirectoriesService, bool>(m => m.ContainsDirectory(Dir))
                .Returns(stateAfter);

            _autoMocker
                .GetMock<IFavouriteDirectoriesService>()
                .Raise(m => m.DirectoryRemoved += null, new FavouriteDirectoriesListChangedEventArgs(Dir));

            Assert.Equal(stateAfter, viewModel.IsFavouriteDirectory);
        }

        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void TestCurrentDirectoryEvent(bool dirExists, int callsCount)
        {
            _autoMocker
                .Setup<IDirectoryService, bool>(m => m.CheckIfExists(Dir))
                .Returns(dirExists);

            var viewModel = _autoMocker.CreateInstance<DirectorySelectorViewModel>();

            var callbackCalledTimes = 0;
            viewModel.CurrentDirectoryChanged += (sender, args) => callbackCalledTimes++;
            viewModel.CurrentDirectory = Dir;

            Assert.Equal(callsCount, callbackCalledTimes);
        }

        [Theory]
        [InlineData(true, 0, false, 0)]
        [InlineData(true, 1, false, 0)]
        [InlineData(false, 0, false, 0)]
        [InlineData(false, 1, true, 1)]
        public void TestSuggestions(bool dirExists, int suggestionsCount, bool shouldShowSuggestions,
            int expectedSuggestionsCount)
        {
            _autoMocker
                .Setup<IDirectoryService, bool>(m => m.CheckIfExists(Dir))
                .Returns(dirExists);
            _autoMocker
                .Setup<ISuggestionsService, IEnumerable<SuggestionModel>>(m => m.GetSuggestions(Dir))
                .Returns(Enumerable.Repeat(new SuggestionModel(Dir, SuggestionType.Directory), suggestionsCount));
            _autoMocker
                .Setup<ISuggestedPathViewModelFactory, ISuggestedPathViewModel>(m => m.Create(Dir,
                    It.Is<SuggestionModel>(m => m.Type == SuggestionType.Directory && m.FullPath == Dir)))
                .Returns(Mock.Of<ISuggestedPathViewModel>());

            var viewModel = _autoMocker.CreateInstance<DirectorySelectorViewModel>();
            viewModel.CurrentDirectory = Dir;

            Assert.Equal(shouldShowSuggestions, viewModel.ShouldShowSuggestions);
            Assert.Equal(expectedSuggestionsCount, viewModel.SuggestedPaths.Count());
        }
    }
}