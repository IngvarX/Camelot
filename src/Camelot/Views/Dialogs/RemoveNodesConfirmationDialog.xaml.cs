using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Camelot.ViewModels.Implementations.Dialogs.Results;

namespace Camelot.Views.Dialogs
{
    public class RemoveNodesConfirmationDialog : DialogWindowBase<RemoveNodesConfirmationDialogResult>
    {
        public RemoveNodesConfirmationDialog()
        {
            InitializeComponent();
        }

        protected override void OnOpened()
        {
            var button = this.FindControl<Button>("RemoveButton");
            button.Focus();

            base.OnOpened();
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}