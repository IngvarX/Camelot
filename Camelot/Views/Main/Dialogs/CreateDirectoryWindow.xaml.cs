using Avalonia.Markup.Xaml;

namespace Camelot.Views.Main.Dialogs
{
    public class CreateDirectoryWindow : DialogWindowBase<string>
    {
        public CreateDirectoryWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}