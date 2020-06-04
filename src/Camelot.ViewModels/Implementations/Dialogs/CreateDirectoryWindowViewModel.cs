using System.Windows.Input;
using Camelot.ViewModels.Implementations.Dialogs.Results;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.Dialogs
{
    public class CreateDirectoryDialogViewModel : DialogViewModelBase<CreateDirectoryDialogResult>
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
            // TODO: validate if dir exists
            var canCreate = this.WhenAnyValue(x => x.DirectoryName,
                name => !string.IsNullOrWhiteSpace(name));

            CreateCommand = ReactiveCommand.Create(CreateDirectory, canCreate);
            CancelCommand = ReactiveCommand.Create(() => Close());
        }

        private void CreateDirectory()
        {
            var result = new CreateDirectoryDialogResult(_directoryName);

            Close(result);
        }
    }
}