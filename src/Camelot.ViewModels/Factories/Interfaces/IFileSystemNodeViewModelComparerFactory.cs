using System.Collections.Generic;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Nodes;

namespace Camelot.ViewModels.Factories.Interfaces
{
    public interface IFileSystemNodeViewModelComparerFactory
    {
        IComparer<IFileSystemNodeViewModel> Create(IFileSystemNodesSortingViewModel viewModel);
    }
}