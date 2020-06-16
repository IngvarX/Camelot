using Avalonia.Markup.Xaml;

namespace Camelot.Views.Main.Dialogs
{
    public class DirectoryInformationDialog : DialogWindowBase
    {
        public DirectoryInformationDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}