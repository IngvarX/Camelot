using System.Collections.Generic;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Comparers;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Nodes;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Tabs;

namespace Camelot.ViewModels.Factories.Implementations;

public class FileSystemNodeViewModelComparerFactory : IFileSystemNodeViewModelComparerFactory
{
    public IComparer<IFileSystemNodeViewModel> Create(IFileSystemNodesSortingViewModel viewModel) =>
        new FileSystemNodesComparer(viewModel.IsSortingByAscendingEnabled, viewModel.SortingColumn);
}