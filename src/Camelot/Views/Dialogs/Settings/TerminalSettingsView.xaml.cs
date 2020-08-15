using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Camelot.Views.Dialogs.Settings
{
    public class TerminalSettingsView : UserControl
    {
        public TerminalSettingsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}