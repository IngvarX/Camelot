using System;
using Camelot.ViewModels.MainWindow;
using ReactiveUI;

namespace Camelot.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private FilesPanelViewModel _activeFilesPanelViewModel;

        public OperationsViewModel OperationsViewModel { get; }

        public FilesPanelViewModel LeftFilesPanelViewModel { get; }

        public FilesPanelViewModel RightFilesPanelViewModel { get; }

        public FilesPanelViewModel ActiveFilesPanelViewModel
        {
            get => _activeFilesPanelViewModel;
            set => this.RaiseAndSetIfChanged(ref _activeFilesPanelViewModel, value);
        }

        public MainWindowViewModel(
            OperationsViewModel operationsViewModel,
            FilesPanelViewModel leftFilesPanelViewModel,
            FilesPanelViewModel rightFilesPanelViewModel)
        {
            OperationsViewModel = operationsViewModel;
            LeftFilesPanelViewModel = leftFilesPanelViewModel;
            // TODO: change
            ActiveFilesPanelViewModel = RightFilesPanelViewModel = rightFilesPanelViewModel;

            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            LeftFilesPanelViewModel.ActivatedEvent += FilesPanelViewModelOnActivatedEvent;
            RightFilesPanelViewModel.ActivatedEvent += FilesPanelViewModelOnActivatedEvent;
        }

        private void FilesPanelViewModelOnActivatedEvent(object sender, EventArgs e)
        {
            ActiveFilesPanelViewModel = (FilesPanelViewModel) sender;
        }
    }
}
