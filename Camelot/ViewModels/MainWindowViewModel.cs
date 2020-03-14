using Camelot.ViewModels.MainWindow;

namespace Camelot.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public OperationsViewModel OperationsViewModel { get; }

        public FilesPanelViewModel LeftFilesPanelViewModel { get; }

        public FilesPanelViewModel RightFilesPanelViewModel { get; }

        public MainWindowViewModel(
            OperationsViewModel operationsViewModel,
            FilesPanelViewModel leftFilesPanelViewModel,
            FilesPanelViewModel rightFilesPanelViewModel)
        {
            OperationsViewModel = operationsViewModel;
            LeftFilesPanelViewModel = leftFilesPanelViewModel;
            RightFilesPanelViewModel = rightFilesPanelViewModel;
        }
    }
}
