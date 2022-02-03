using Avalonia.Markup.Xaml;
using Camelot.ViewModels.Implementations.Dialogs.Results;

namespace Camelot.Views.Dialogs;

public class RemoveNodesConfirmationDialog : DialogWindowBase<RemoveNodesConfirmationDialogResult>
{
    public RemoveNodesConfirmationDialog()
    {
        InitializeComponent();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}