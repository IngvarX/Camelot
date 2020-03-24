using System;
using Avalonia.Controls;
using Camelot.ViewModels.Dialogs;

namespace Camelot.Views.Main.Dialogs
{
    public class DialogWindowBase<T> : Window
    {
        protected DialogViewModelBase<T> ViewModel => (DialogViewModelBase<T>) DataContext;

        protected DialogWindowBase()
        {
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, EventArgs e)
        {
            SubscribeToEvents();
        }

        private void ViewModelOnCloseRequested(object sender, DialogResultEventArgs<T> args)
        {
            UnsubscribeFromEvents();
            Close(args.Result);
        }

        private void SubscribeToEvents()
        {
            ViewModel.CloseRequested += ViewModelOnCloseRequested;
        }

        private void UnsubscribeFromEvents()
        {
            ViewModel.CloseRequested -= ViewModelOnCloseRequested;
        }
    }
}