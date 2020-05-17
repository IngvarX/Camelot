using System.Windows.Input;
using Camelot.ViewModels.Interfaces.MainWindow;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Interfaces.MainWindow.OperationsStates;
using Camelot.ViewModels.Interfaces.Menu;
using Camelot.ViewModels.Services.Interfaces;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IFilesOperationsMediator _filesOperationsMediator;

        public IOperationsViewModel OperationsViewModel { get; }

        public IFilesPanelViewModel LeftFilesPanelViewModel { get; }

        public IFilesPanelViewModel RightFilesPanelViewModel { get; }

        public IMenuViewModel MenuViewModel { get; }

        public IOperationsStateViewModel OperationsStateViewModel { get; }

        public ICommand CreateNewTabCommand { get; }

        public ICommand CloseCurrentTabCommand { get; }

        public MainWindowViewModel(
            IFilesOperationsMediator filesOperationsMediator,
            IOperationsViewModel operationsViewModel,
            IFilesPanelViewModel leftFilesPanelViewModel,
            IFilesPanelViewModel rightFilesPanelViewModel,
            IMenuViewModel menuViewModel,
            IOperationsStateViewModel operationsStateViewModel)
        {
            _filesOperationsMediator = filesOperationsMediator;

            OperationsViewModel = operationsViewModel;
            LeftFilesPanelViewModel = leftFilesPanelViewModel;
            RightFilesPanelViewModel = rightFilesPanelViewModel;
            MenuViewModel = menuViewModel;
            OperationsStateViewModel = operationsStateViewModel;

            CreateNewTabCommand = ReactiveCommand.Create(CreateNewTab);
            CloseCurrentTabCommand = ReactiveCommand.Create(CloseCurrentTab);

            // TODO: from settings
            filesOperationsMediator.Register(leftFilesPanelViewModel, rightFilesPanelViewModel);
        }

        private void CreateNewTab() => _filesOperationsMediator.ActiveFilesPanelViewModel.CreateNewTab();

        private void CloseCurrentTab() => _filesOperationsMediator.ActiveFilesPanelViewModel.CloseActiveTab();
    }
}
