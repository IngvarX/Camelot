using System;
using System.Reactive.Linq;
using System.Windows.Input;
using Camelot.Avalonia.Interfaces;
using Camelot.Extensions;
using Camelot.Services.Environment.Interfaces;
using Camelot.ViewModels.Configuration;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Specifications;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Services.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels;

public class SearchViewModel : ValidatableViewModelBase, ISearchViewModel
{
    private readonly IRegexService _regexService;
    private readonly IApplicationDispatcher _applicationDispatcher;

    private bool HasText => !string.IsNullOrEmpty(SearchText);

    private bool IsValid => !IsSearchEnabled || !IsRegexSearchEnabled || _regexService.ValidateRegex(SearchText);

    [Reactive]
    public string SearchText { get; set; }

    [Reactive]
    public bool IsSearchCaseSensitive { get; set; }

    [Reactive]
    public bool IsRegexSearchEnabled { get; set; }

    [Reactive]
    public bool IsSearchEnabled { get; set; }

    [Reactive]
    public bool IsRecursiveSearchEnabled { get; set; }

    public event EventHandler<EventArgs> SearchSettingsChanged;

    public ICommand ToggleSearchCommand { get; }

    public SearchViewModel(
        IRegexService regexService,
        IResourceProvider resourceProvider,
        IApplicationDispatcher applicationDispatcher,
        SearchViewModelConfiguration searchViewModelConfiguration)
    {
        _regexService = regexService;
        _applicationDispatcher = applicationDispatcher;

        ToggleSearchCommand = ReactiveCommand.Create(ToggleSearch);

        Reset();

        this.ValidationRule(
            vm => vm.SearchText,
            this.WhenAnyValue(x => x.IsRegexSearchEnabled, x => x.SearchText),
            v => !HasText || IsValid,
            _ => resourceProvider.GetResourceByName(searchViewModelConfiguration.InvalidRegexResourceName)
        );
        this.WhenAnyValue(
                x => x.SearchText,
                x => x.IsSearchEnabled,
                x => x.IsRegexSearchEnabled,
                x => x.IsSearchCaseSensitive,
                x => x.IsRecursiveSearchEnabled)
            .Throttle(TimeSpan.FromMilliseconds(searchViewModelConfiguration.TimeoutMs))
            .Subscribe(_ => FireSettingsChangedEvent());
    }

    public INodeSpecification GetSpecification() =>
        (IsSearchEnabled, IsRegexSearchEnabled) switch
        {
            (true, true) when HasText && IsValid => new NodeNameRegexSpecification(_regexService, SearchText, IsSearchCaseSensitive, IsRecursiveSearchEnabled),
            (true, false) when HasText => new NodeNameTextSpecification(SearchText, IsSearchCaseSensitive, IsRecursiveSearchEnabled),
            _ => new EmptySpecification(false)
        };

    public void ToggleSearch()
    {
        Reset();
        IsSearchEnabled = !IsSearchEnabled;
    }

    private void Reset() => SearchText = string.Empty;

    private void FireSettingsChangedEvent()
    {
        if (IsValid)
        {
            _applicationDispatcher.Dispatch(() => SearchSettingsChanged.Raise(this, EventArgs.Empty));
        }
    }
}