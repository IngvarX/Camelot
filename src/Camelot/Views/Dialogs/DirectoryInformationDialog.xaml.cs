using Avalonia.Markup.Xaml;

namespace Camelot.Views.Dialogs
{
    public class DirectoryInformationDialog : DialogWindowBase
    {
        public DirectoryInformationDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}