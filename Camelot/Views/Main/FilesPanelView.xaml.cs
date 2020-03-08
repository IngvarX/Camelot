using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Camelot.Views.Main
{
    public class FilesPanelView : UserControl
    {
        public FilesPanelView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}