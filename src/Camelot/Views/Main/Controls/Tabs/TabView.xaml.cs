using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;

namespace Camelot.Views.Main.Controls.Tabs
{
    public class TabView : UserControl
    {
        public TabView()
        {
            InitializeComponent();
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

        private void ButtonOnPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            if (e.InitialPressMouseButton != MouseButton.Middle)
            {
                return;
            }

            e.Handled = true;

            if (DataContext is ITabViewModel viewModel)
            {
                viewModel.CloseTabCommand.Execute(null);
            }
        }
    }
}