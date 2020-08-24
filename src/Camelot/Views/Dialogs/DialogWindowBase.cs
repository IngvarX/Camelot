using System;
using Avalonia;
using Avalonia.Controls;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Services;

namespace Camelot.Views.Dialogs
{
    public class DialogWindowBase<TResult> : Window
        where TResult : DialogResultBase
    {
        private Window ParentWindow => (Window) Owner;

        protected DialogViewModelBase<TResult> ViewModel => (DialogViewModelBase<TResult>) DataContext;

        protected DialogWindowBase()
        {
            SubscribeToViewEvents();
        }

        protected virtual void OnOpened()
        {

        }

        private void OnOpened(object sender, EventArgs e)
        {
            LockSize();
            CenterDialog();

            OnOpened();
        }

        private void CenterDialog()
        {
            var x = ParentWindow.Position.X + (ParentWindow.Bounds.Width - Width) / 2;
            var y = ParentWindow.Position.Y + (ParentWindow.Bounds.Height - Height) / 2;

            Position = new PixelPoint((int) x, (int) y);
        }

        private void LockSize()
        {
            MaxWidth = MinWidth = Width;
            MaxHeight = MinHeight = Height;
        }

        private void SubscribeToViewModelEvents() => ViewModel.CloseRequested += ViewModelOnCloseRequested;

        private void UnsubscribeFromViewModelEvents() => ViewModel.CloseRequested -= ViewModelOnCloseRequested;

        private void SubscribeToViewEvents()
        {
            DataContextChanged += OnDataContextChanged;
            Opened += OnOpened;
        }

        private void UnsubscribeFromViewEvents()
        {
            DataContextChanged -= OnDataContextChanged;
            Opened -= OnOpened;
        }

        private void OnDataContextChanged(object sender, EventArgs e) => SubscribeToViewModelEvents();

        private void ViewModelOnCloseRequested(object sender, DialogResultEventArgs<TResult> args)
        {
            UnsubscribeFromViewModelEvents();
            UnsubscribeFromViewEvents();

            Close(args.Result);
        }
    }

    public class DialogWindowBase : DialogWindowBase<DialogResultBase>
    {

    }
}