using System;
using Camelot.Extensions;

namespace Camelot.ViewModels.Implementations.Dialogs
{
    public class DialogViewModelBase<TResult> : ViewModelBase
    {
        public event EventHandler<DialogResultEventArgs<TResult>> CloseRequested;

        protected void Close(TResult result = default)
        {
            var args = new DialogResultEventArgs<TResult>(result);

            CloseRequested.Raise(this, args);
        }
    }

    public class DialogViewModelBase : DialogViewModelBase<object>
    {
        
    }
}