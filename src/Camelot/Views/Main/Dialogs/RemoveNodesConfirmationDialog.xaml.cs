using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Camelot.Views.Main.Dialogs
{
    public class RemoveNodesConfirmationDialog : DialogWindowBase<bool>
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

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}