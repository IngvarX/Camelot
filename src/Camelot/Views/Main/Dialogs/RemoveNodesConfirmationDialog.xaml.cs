using Avalonia.Markup.Xaml;

namespace Camelot.Views.Main.Dialogs
{
    public class RemoveNodesConfirmationDialog : DialogWindowBase<bool>
    {
        public RemoveNodesConfirmationDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}