using System;
using Avalonia;
using Avalonia.Controls;
using Camelot.ViewModels.Dialogs;

namespace Camelot.Views.Main.Dialogs
{
    public class DialogWindowBase<T> : Window
    {
        private Window ParentWindow => (Window) Owner;

        protected DialogViewModelBase<T> ViewModel => (DialogViewModelBase<T>) DataContext;


        protected DialogWindowBase()
        {
            SubscribeToViewEvents();
        }

        private void OnOpened(object sender, EventArgs e)
        {
            LockSize();
            CenterDialog();
        }

        private void CenterDialog()
        {
            var x = ParentWindow.Position.X + (ParentWindow.Bounds.Width - Width) / 2;
            var y = ParentWindow.Position.Y + (ParentWindow.Bounds.Height - Height) / 2;

            Position = new PixelPoint((int)x, (int)y);
        }

        private void LockSize()
        {
            MaxWidth = MinWidth = Width;
            MaxHeight = MinHeight = Height;
        }

        private void SubscribeToViewModelEvents()
        {
            ViewModel.CloseRequested += ViewModelOnCloseRequested;
        }

        private void UnsubscribeFromViewModelEvents()
        {
            ViewModel.CloseRequested -= ViewModelOnCloseRequested;
        }

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

        private void OnDataContextChanged(object sender, EventArgs e)
        {
            SubscribeToViewModelEvents();
        }

        private void ViewModelOnCloseRequested(object sender, DialogResultEventArgs<T> args)
        {
            UnsubscribeFromViewModelEvents();
            UnsubscribeFromViewEvents();
            Close(args.Result);
        }
    }
}