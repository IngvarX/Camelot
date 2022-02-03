using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Camelot.ViewModels.Implementations.Dialogs.Results;

namespace Camelot.Views.Dialogs;

public class CreateDirectoryDialog : DialogWindowBase<CreateDirectoryDialogResult>
{
    public CreateDirectoryDialog()
    {
        InitializeComponent();
    }

    protected override void OnOpened()
    {
        var textBox = this.FindControl<TextBox>("NodeNameTextBox");
        textBox.Focus();

        base.OnOpened();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}