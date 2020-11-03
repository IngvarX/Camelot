using Avalonia.Markup.Xaml;

namespace Camelot.Views.Dialogs
{
    public class OpenWithDialog : DialogWindowBase
    {
        public OpenWithDialog()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}
