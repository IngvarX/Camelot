using System.Windows.Input;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.Dialogs
{
    public class CreateDirectoryDialogViewModel : DialogViewModelBase<string>
    {
        private string _directoryName;

        public string DirectoryName
        {
            get => _directoryName;
            set => this.RaiseAndSetIfChanged(ref _directoryName, value);
        }

        public ICommand CreateCommand { get; }

        public ICommand CancelCommand { get; }

        public CreateDirectoryDialogViewModel()
        {
            var canCreate = this.WhenAnyValue(x => x.DirectoryName,
                name => !string.IsNullOrWhiteSpace(name));

            CreateCommand = ReactiveCommand.Create(CreateDirectory, canCreate);
            CancelCommand = ReactiveCommand.Create(Cancel);
        }

        private void Cancel()
        {
            Close();
        }

        private void CreateDirectory()
        {
            Close(_directoryName);
        }
    }
}