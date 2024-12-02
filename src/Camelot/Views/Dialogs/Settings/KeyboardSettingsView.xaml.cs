using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Camelot.Views.Dialogs.Settings;
public class KeyboardSettingsView : UserControl
{
    public KeyboardSettingsView()
    {
        InitializeComponent();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}