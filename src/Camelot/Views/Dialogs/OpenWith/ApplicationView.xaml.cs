using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Camelot.Views.Dialogs.OpenWith
{
    public class ApplicationView : UserControl
    {
        public ApplicationView()
        {
            InitializeComponent();
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}
