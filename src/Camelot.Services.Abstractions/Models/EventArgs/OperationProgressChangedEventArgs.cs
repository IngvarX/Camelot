namespace Camelot.Services.Abstractions.Models.EventArgs;

public class OperationProgressChangedEventArgs : System.EventArgs
{
    public double CurrentProgress { get; }

    public OperationProgressChangedEventArgs(double currentProgress)
    {
        CurrentProgress = currentProgress;
    }
}