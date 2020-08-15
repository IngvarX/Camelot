using Avalonia.Markup.Xaml;

namespace Camelot.Views.Dialogs
{
    public class SettingsDialog : DialogWindowBase
    {
        public SettingsDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}