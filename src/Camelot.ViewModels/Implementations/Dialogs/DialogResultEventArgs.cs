using System;

namespace Camelot.ViewModels.Implementations.Dialogs
{
    public class DialogResultEventArgs<T> : EventArgs
    {
        public T Result { get; }

        public DialogResultEventArgs(T result)
        {
            Result = result;
        }
    }
}