using System.Collections.Generic;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;

namespace Camelot.ViewModels.Factories.Interfaces
{
    public interface IFileSystemNodeViewModelComparerFactory
    {
        IComparer<IFileSystemNodeViewModel> Create(IFileSystemNodesSortingViewModel viewModel);
    }
}