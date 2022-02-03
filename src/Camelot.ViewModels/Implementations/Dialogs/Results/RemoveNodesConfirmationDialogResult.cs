using Camelot.ViewModels.Services;

namespace Camelot.ViewModels.Implementations.Dialogs.Results;

public class RemoveNodesConfirmationDialogResult : DialogResultBase
{
    public bool IsConfirmed { get; }

    public RemoveNodesConfirmationDialogResult(bool isConfirmed)
    {
        IsConfirmed = isConfirmed;
    }
}