using Avalonia.Markup.Xaml;

namespace Camelot.Views.Main.Dialogs
{
    public class SettingsDialog : DialogWindowBase
    {
        public SettingsDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}