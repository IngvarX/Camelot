using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Camelot.Views.Main
{
    public class BottomButtonsView : UserControl
    {
        public BottomButtonsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}