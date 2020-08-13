using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Specifications;
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

        public ISpecification<NodeModelBase> GetSpecification() =>
            (IsSearchEnabled, IsRegexSearchEnabled) switch
            {
                (true, true) => new FileNameRegexSpecification(SearchText, IsSearchCaseSensitive),
                (true, false) => new FileNameTextSpecification(SearchText, IsSearchCaseSensitive),
                _ => new EmptySpecification()
            };
    }
}