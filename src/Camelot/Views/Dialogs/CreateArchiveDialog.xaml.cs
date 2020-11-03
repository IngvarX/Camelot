using Avalonia.Markup.Xaml;
using Camelot.ViewModels.Implementations.Dialogs.Results;

namespace Camelot.Views.Dialogs
{
    public class CreateArchiveDialog : DialogWindowBase<CreateArchiveDialogResult>
    {
        public CreateArchiveDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}