using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Camelot.Views.Dialogs.Settings;
public class IconsSettingsView : UserControl
{
    public IconsSettingsView()
    {
        InitializeComponent();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}