using ReactiveUI;

namespace Camelot.ViewModels.MainWindow
{
    public class FileViewModel : ViewModelBase
    {
        private string _fileName;

        public string FileName
        {
            get => _fileName;
            set => this.RaiseAndSetIfChanged(ref _fileName, value);
        }

        public FileViewModel(string fileName)
        {
            _fileName = fileName;
        }
    }
}