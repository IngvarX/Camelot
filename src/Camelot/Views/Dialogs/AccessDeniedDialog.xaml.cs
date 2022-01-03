using Avalonia.Markup.Xaml;

namespace Camelot.Views.Dialogs;

public class AccessDeniedDialog : DialogWindowBase
{
    public AccessDeniedDialog()
    {
        InitializeComponent();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}