using Camelot.Services.Abstractions.Models.Operations;
using Camelot.ViewModels.Services;

namespace Camelot.ViewModels.Implementations.Dialogs.Results;

public class OverwriteOptionsDialogResult : DialogResultBase
{
    public OperationContinuationOptions Options { get; }

    public OverwriteOptionsDialogResult(OperationContinuationOptions options)
    {
        Options = options;
    }
}