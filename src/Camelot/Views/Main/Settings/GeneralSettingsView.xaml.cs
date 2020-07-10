using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Camelot.Views.Main.Settings
{
    public class GeneralSettingsView : UserControl
    {
        public GeneralSettingsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
