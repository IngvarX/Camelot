using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Camelot.Views.Main.Dialogs
{
    public class CreateDirectoryDialog : DialogWindowBase<string>
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

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}