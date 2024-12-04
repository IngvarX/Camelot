using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Camelot.Views.Dialogs.Settings;

public class AppearanceSettingsView : UserControl
{
    public AppearanceSettingsView()
    {
        InitializeComponent();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}