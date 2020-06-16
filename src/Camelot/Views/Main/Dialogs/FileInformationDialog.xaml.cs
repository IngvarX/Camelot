using Avalonia.Markup.Xaml;

namespace Camelot.Views.Main.Dialogs
{
    public class FileInformationDialog : DialogWindowBase
    {
        public FileInformationDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}