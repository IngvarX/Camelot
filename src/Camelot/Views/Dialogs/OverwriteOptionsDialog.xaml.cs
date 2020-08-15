using Avalonia.Markup.Xaml;
using Camelot.ViewModels.Implementations.Dialogs.Results;

namespace Camelot.Views.Dialogs
{
    public class OverwriteOptionsDialog :  DialogWindowBase<OverwriteOptionsDialogResult>
    {
        public OverwriteOptionsDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}