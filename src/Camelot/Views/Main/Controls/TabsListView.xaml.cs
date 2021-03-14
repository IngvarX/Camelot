using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Camelot.Extensions;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;

namespace Camelot.Views.Main.Controls
{
    public class TabsListView : UserControl
    {
        private ITabsListViewModel ViewModel => (ITabsListViewModel) DataContext;

        private ScrollViewer ScrollViewer => this.FindControl<ScrollViewer>("TabsScrollViewer");

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

        private void LeftButtonOnClick(object sender, RoutedEventArgs e) =>
            Enumerable.Repeat(0, 5).ForEach(_ => ScrollViewer.LineLeft());

        private void RightButtonOnClick(object sender, RoutedEventArgs e) =>
            Enumerable.Repeat(0, 5).ForEach(_ => ScrollViewer.LineRight());
    }
}