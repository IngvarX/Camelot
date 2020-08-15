using System;
using System.Reactive.Linq;
using Camelot.Extensions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Specifications;
using Camelot.ViewModels.Configuration;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Specifications;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow
{
    public class SearchViewModel : ViewModelBase, ISearchViewModel
    {
        private string _searchText;
        private bool _isSearchCaseSensitive;
        private bool _isRegexSearchEnabled;
        private bool _isSearchEnabled;

        public string SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        public bool IsSearchCaseSensitive
        {
            get => _isSearchCaseSensitive;
            set => this.RaiseAndSetIfChanged(ref _isSearchCaseSensitive, value);
        }

        public bool IsRegexSearchEnabled
        {
            get => _isRegexSearchEnabled;
            set => this.RaiseAndSetIfChanged(ref _isRegexSearchEnabled, value);
        }

        public bool IsSearchEnabled
        {
            get => _isSearchEnabled;
            set => this.RaiseAndSetIfChanged(ref _isSearchEnabled, value);
        }

        public event EventHandler<EventArgs> SearchSettingsChanged;

        public SearchViewModel(SearchViewModelConfiguration searchViewModelConfiguration)
        {
            SearchText = string.Empty;

            this.WhenAnyValue(x => x.SearchText, x => x.IsSearchEnabled,
                    x => x.IsRegexSearchEnabled, x => x.IsSearchCaseSensitive)
                .Throttle(TimeSpan.FromMilliseconds(searchViewModelConfiguration.TimeoutMs))
                .Subscribe(_ => FireSettingsChangedEvent());
        }

        public ISpecification<NodeModelBase> GetSpecification() =>
            (IsSearchEnabled, IsRegexSearchEnabled) switch
            {
                (true, true) => new NodeNameRegexSpecification(SearchText, IsSearchCaseSensitive),
                (true, false) => new NodeNameTextSpecification(SearchText, IsSearchCaseSensitive),
                _ => new EmptySpecification()
            };

        public void ToggleVisibility() => IsSearchEnabled = !IsSearchEnabled;

        private void FireSettingsChangedEvent() => SearchSettingsChanged.Raise(this, EventArgs.Empty);
    }
}