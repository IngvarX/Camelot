using Camelot.ViewModels.Implementations.MainWindow;
using Camelot.ViewModels.Implementations.Menu;
using Camelot.ViewModels.Interfaces;
using Camelot.ViewModels.Interfaces.MainWindow;
using Camelot.ViewModels.Interfaces.Menu;
using Camelot.ViewModels.Services.Interfaces;

namespace Camelot.ViewModels.Implementations
{
    public class MainWindowViewModel : ViewModelBase
    {
        public IOperationsViewModel OperationsViewModel { get; }

        public IFilesPanelViewModel LeftFilesPanelViewModel { get; }

        public IFilesPanelViewModel RightFilesPanelViewModel { get; }

        public IMenuViewModel MenuViewModel { get; }

        public MainWindowViewModel(
            IFilesOperationsMediator filesOperationsMediator,
            IOperationsViewModel operationsViewModel,
            IFilesPanelViewModel leftFilesPanelViewModel,
            IFilesPanelViewModel rightFilesPanelViewModel,
            IMenuViewModel menuViewModel)
        {
            OperationsViewModel = operationsViewModel;
            LeftFilesPanelViewModel = leftFilesPanelViewModel;
            RightFilesPanelViewModel = rightFilesPanelViewModel;
            MenuViewModel = menuViewModel;

            // TODO: from settings
            filesOperationsMediator.Register(leftFilesPanelViewModel, rightFilesPanelViewModel);
        }
    }
}
