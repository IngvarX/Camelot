using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Camelot.Views.Main
{
    public class TabView : UserControl
    {
        public TabView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}