namespace Camelot.Services.EventArgs
{
    public class OperationProgressChangedEventArgs : System.EventArgs
    {
        public double CurrentProgress { get; }

        public OperationProgressChangedEventArgs(double currentProgress)
        {
            CurrentProgress = currentProgress;
        }
    }
}