using System;
using System.Windows.Input;
using Camelot.Extensions;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Specifications;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Specifications.QuickSearch;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.EventArgs;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Nodes;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels;

public class QuickSearchViewModel : ViewModelBase, IQuickSearchViewModel
{
    private readonly IQuickSearchService _quickSearchService;
    private readonly IPathService _pathService;

    private string _searchTerm;

    private QuickSearchMode QuickSearchMode => _quickSearchService.GetQuickSearchSettings().SelectedMode;

    public event EventHandler<QuickSearchFilterChangedEventArgs> QuickSearchFilterChanged;

    public ICommand QuickSearchCommand { get; }

    public ICommand ClearQuickSearchCommand { get; }

    public QuickSearchViewModel(
        IQuickSearchService quickSearchService,
        IPathService pathService)
    {
        _quickSearchService = quickSearchService;
        _pathService = pathService;

        _searchTerm = String.Empty;

        QuickSearchCommand = ReactiveCommand.Create<QuickSearchCommandModel>(QuickSearch);
        ClearQuickSearchCommand = ReactiveCommand.Create(ClearQuickSearch);

        SubscribeToEvents();
    }

    public ISpecification<IFileSystemNodeViewModel> GetSpecification()
    {
        switch (QuickSearchMode)
        {
            case QuickSearchMode.Disabled:
                return new QuickSearchEmptySpecification();
            case QuickSearchMode.Letter:
            case QuickSearchMode.Word:
                return new QuickSearchSpecification(_pathService, _searchTerm);
            default:
                throw new ArgumentOutOfRangeException(nameof(QuickSearchMode), QuickSearchMode, null);
        }
    }

    public void ClearQuickSearch()
    {
        _searchTerm = string.Empty;
        RaiseQuickSearchFilterChangedEvent();
    }

    private void SubscribeToEvents()
    {
        _quickSearchService.QuickSearchModeChanged += (_, _) => ClearQuickSearch();
    }

    private void QuickSearch(QuickSearchCommandModel parameter)
    {
        switch (QuickSearchMode)
        {
            case QuickSearchMode.Disabled:
                return;
            case QuickSearchMode.Letter:
                var newSearchTerm = parameter.Symbol.ToString();
                if (_searchTerm == newSearchTerm)
                {
                    RaiseQuickSearchFilterChangedEvent(parameter.IsBackwardsDirectionEnabled);
                }
                else
                {
                    _searchTerm = newSearchTerm;
                    RaiseQuickSearchFilterChangedEvent(parameter.IsBackwardsDirectionEnabled);
                }

                break;
            case QuickSearchMode.Word:
                _searchTerm += parameter.Symbol;
                RaiseQuickSearchFilterChangedEvent(parameter.IsBackwardsDirectionEnabled);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(QuickSearchMode), QuickSearchMode, null);
        }
    }

    private void RaiseQuickSearchFilterChangedEvent(bool isBackwardsDirectionEnabled) =>
        RaiseQuickSearchFilterChangedEvent(isBackwardsDirectionEnabled
            ? SelectionChangeDirection.Backward
            : SelectionChangeDirection.Forward);

    private void RaiseQuickSearchFilterChangedEvent(
        SelectionChangeDirection direction = SelectionChangeDirection.Keep) =>
        QuickSearchFilterChanged.Raise(this, new QuickSearchFilterChangedEventArgs(direction));
}