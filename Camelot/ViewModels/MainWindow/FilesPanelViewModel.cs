using System.Collections.ObjectModel;
using System.Linq;
using Camelot.Services.Interfaces;

namespace Camelot.ViewModels.MainWindow
{
    public class FilesPanelViewModel : ViewModelBase
    {
        public ObservableCollection<FileViewModel> Files { get; }

        public FilesPanelViewModel(IFileService fileService)
        {
            var files = fileService.GetFiles("/home");
            Files = new ObservableCollection<FileViewModel>(files.Select(f => new FileViewModel(f.Name)));
        }
    }
}