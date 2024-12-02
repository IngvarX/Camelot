using System;
using System.Windows.Input;
using Camelot.Services.Abstractions.Specifications;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.EventArgs;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Nodes;

namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels;

public interface IQuickSearchViewModel
{
    event EventHandler<QuickSearchFilterChangedEventArgs> QuickSearchFilterChanged;

    ICommand QuickSearchCommand { get; }

    ICommand ClearQuickSearchCommand { get; }

    ISpecification<IFileSystemNodeViewModel> GetSpecification();

    void ClearQuickSearch();
}