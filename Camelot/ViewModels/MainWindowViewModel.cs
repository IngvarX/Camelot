using Camelot.Services.Interfaces;
using Camelot.ViewModels.MainWindow;

namespace Camelot.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public OperationsViewModel OperationsViewModel { get; }

        public FilesPanelViewModel LeftFilesPanelViewModel { get; }

        public FilesPanelViewModel RightFilesPanelViewModel { get; }

        public MainWindowViewModel(IFileService fileService)
        {
            LeftFilesPanelViewModel = new FilesPanelViewModel(fileService);
            RightFilesPanelViewModel = new FilesPanelViewModel(fileService);
        }
    }
}
