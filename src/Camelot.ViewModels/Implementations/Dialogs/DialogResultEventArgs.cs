using System;

namespace Camelot.ViewModels.Implementations.Dialogs;

public class DialogResultEventArgs<TResult> : EventArgs
{
    public TResult Result { get; }

    public DialogResultEventArgs(TResult result)
    {
        Result = result;
    }
}