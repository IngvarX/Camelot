using Camelot.Mediator.Interfaces;
using Camelot.ViewModels.MainWindow;
using Camelot.ViewModels.Menu;

namespace Camelot.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public OperationsViewModel OperationsViewModel { get; }

        public FilesPanelViewModel LeftFilesPanelViewModel { get; }

        public FilesPanelViewModel RightFilesPanelViewModel { get; }

        public MenuViewModel MenuViewModel { get; }

        public MainWindowViewModel(
            IFilesOperationsMediator filesOperationsMediator,
            OperationsViewModel operationsViewModel,
            FilesPanelViewModel leftFilesPanelViewModel,
            FilesPanelViewModel rightFilesPanelViewModel,
            MenuViewModel menuViewModel)
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
