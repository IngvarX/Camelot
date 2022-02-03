using System.Collections.Generic;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Nodes;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Tabs;

namespace Camelot.ViewModels.Factories.Interfaces;

public interface IFileSystemNodeViewModelComparerFactory
{
    IComparer<IFileSystemNodeViewModel> Create(IFileSystemNodesSortingViewModel viewModel);
}