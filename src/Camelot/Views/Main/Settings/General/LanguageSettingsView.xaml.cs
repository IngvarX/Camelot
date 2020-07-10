using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Camelot.Views.Main.Settings.General
{
    public class LanguageSettingsView : UserControl
    {
        public LanguageSettingsView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
