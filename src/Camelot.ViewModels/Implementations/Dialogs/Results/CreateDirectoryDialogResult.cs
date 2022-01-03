using Camelot.ViewModels.Services;

namespace Camelot.ViewModels.Implementations.Dialogs.Results;

public class CreateDirectoryDialogResult : DialogResultBase
{
    public string DirectoryName { get; }

    public CreateDirectoryDialogResult(string directoryName)
    {
        DirectoryName = directoryName;
    }
}