using Avalonia.Markup.Xaml;

namespace Camelot.Views.Main.Dialogs
{
    public class CreateDirectoryDialog : DialogWindowBase<string>
    {
        public CreateDirectoryDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}