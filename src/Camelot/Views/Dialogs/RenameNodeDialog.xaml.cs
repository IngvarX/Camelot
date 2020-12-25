using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Implementations.Dialogs.Results;

namespace Camelot.Views.Dialogs
{
    public class RenameNodeDialog : DialogWindowBase<RenameNodeDialogResult>
    {
        public RenameNodeDialog()
        {
            InitializeComponent();
        }

        protected override void OnOpened()
        {
            var textBox = this.FindControl<TextBox>("NodeNameTextBox");
            textBox.Focus();

            base.OnOpened();
        }

        private void OnNodeNameTextBoxKeyUp(object sender, KeyEventArgs args)
        {
            if (args.Key is Key.Enter)
            {
                args.Handled = true;

                var viewModel = (RenameNodeDialogViewModel) ViewModel;
                viewModel.RenameCommand.Execute(null);
            }
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}