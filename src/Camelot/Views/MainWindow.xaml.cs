using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Camelot.Views
{
    public class MainWindow : Window
    {
        private Grid OverlayGrid => this.FindControl<Grid>("OverlayGrid");

        public MainWindow()
        {
            InitializeComponent();
        }

        public void ShowOverlay()
        {
            OverlayGrid.ZIndex = 1000;
        }

        public void HideOverlay()
        {
            OverlayGrid.ZIndex = -1;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}