using Avalonia.Markup.Xaml;

namespace Camelot.Views.Dialogs
{
    public class AboutDialog : DialogWindowBase
    {
        public AboutDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}