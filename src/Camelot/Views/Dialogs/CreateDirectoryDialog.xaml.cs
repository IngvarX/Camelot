using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Implementations.Dialogs.Results;

namespace Camelot.Views.Dialogs
{
    public class CreateDirectoryDialog : DialogWindowBase<CreateDirectoryDialogResult>
    {
        public CreateDirectoryDialog()
        {
            InitializeComponent();
        }

        protected override void OnOpened()
        {
            var textBox = this.FindControl<TextBox>("DirectoryNameTextBox");
            textBox.Focus();

            base.OnOpened();
        }

        private void OnDirectoryNameTextBoxKeyUp(object sender, KeyEventArgs args)
        {
            if (args.Key is Key.Enter)
            {
                args.Handled = true;

                var viewModel = (CreateDirectoryDialogViewModel) ViewModel;
                viewModel.CreateCommand.Execute(null);
            }
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}