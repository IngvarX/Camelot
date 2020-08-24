using Camelot.ViewModels.Services;

namespace Camelot.ViewModels.Implementations.Dialogs.Results
{
    public class CreateFileDialogResult : DialogResultBase
    {
        public string FileName { get; }

        public CreateFileDialogResult(string fileName)
        {
            FileName = fileName;
        }
    }
}