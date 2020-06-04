using Camelot.Services.Abstractions.Models.Operations;

namespace Camelot.ViewModels.Implementations.Dialogs.Results
{
    public class OverwriteOptionsDialogResult
    {
        public OperationContinuationOptions Options { get; }

        public OverwriteOptionsDialogResult(OperationContinuationOptions options)
        {
            Options = options;
        }
    }
}