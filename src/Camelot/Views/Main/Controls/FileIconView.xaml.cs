using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Camelot.Views.Main.Controls;

public class FileIconView : UserControl
{
    public FileIconView()
    {
        InitializeComponent();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}