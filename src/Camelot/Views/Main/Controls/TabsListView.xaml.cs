using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;

namespace Camelot.Views.Main.Controls
{
    public class TabsListView : UserControl
    {
        private ITabsListViewModel ViewModel => (ITabsListViewModel) DataContext;

        public TabsListView()
        {
            InitializeComponent();
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

        private void TabsListOnPointerWheelChanged(object sender, PointerWheelEventArgs e)
        {
            var command = e.Delta.Y > 0
                ? ViewModel.SelectTabToTheLeftCommand
                : ViewModel.SelectTabToTheRightCommand;

            command.Execute(null);
        }
    }
}