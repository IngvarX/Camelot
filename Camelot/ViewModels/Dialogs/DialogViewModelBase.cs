using System;
using Camelot.Extensions;

namespace Camelot.ViewModels.Dialogs
{
    public class DialogViewModelBase<T> : ViewModelBase
    {
        public event EventHandler<DialogResultEventArgs<T>> CloseRequested;

        protected void Close(T result = default)
        {
            var args = new DialogResultEventArgs<T>(result);

            CloseRequested.Raise(this, args);
        }
    }
}