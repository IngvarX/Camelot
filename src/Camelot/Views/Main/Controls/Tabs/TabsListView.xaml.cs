using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Camelot.Avalonia.Interfaces;
using Camelot.DependencyInjection;
using Camelot.Extensions;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Tabs;
using Splat;

namespace Camelot.Views.Main.Controls.Tabs;

public class TabsListView : UserControl
{
    private const int ScrollsCount = 5;

    private ITabsListViewModel ViewModel => (ITabsListViewModel) DataContext;

    private ScrollViewer ScrollViewer => this.FindControl<ScrollViewer>("TabsScrollViewer");

    private ItemsControl ItemsControl => this.FindControl<ItemsControl>("TabsItemsControl");

    public TabsListView()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        ViewModel.SelectedTabChanged += (_, _) => ScrollToSelectedTab();
        var dispatcher = Locator.Current.GetRequiredService<IApplicationDispatcher>();
        dispatcher.Dispatch(ScrollToSelectedTab);

        base.OnDataContextChanged(e);
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void ScrollToSelectedTab()
    {
        var selectedTab = ItemsControl
            .GetLogicalDescendants()
            .OfType<TabView>()
            .SingleOrDefault(t => t.DataContext is ITabViewModel {IsActive: true});

        selectedTab?.BringIntoView();
    }

    private void TabsListOnPointerWheelChanged(object sender, PointerWheelEventArgs e)
    {
        var command = e.Delta.Y > 0
            ? ViewModel.SelectTabToTheLeftCommand
            : ViewModel.SelectTabToTheRightCommand;

        command.Execute(null);
    }

    private void LeftButtonOnClick(object sender, RoutedEventArgs e) => Scroll(ScrollViewer.LineLeft);

    private void RightButtonOnClick(object sender, RoutedEventArgs e) => Scroll(ScrollViewer.LineRight);

    private static void Scroll(Action scrollAction) =>
        Enumerable.Repeat(0, ScrollsCount).ForEach(_ => scrollAction());

    private void ScrollViewerOnScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        var leftButton = this.FindControl<Button>("LeftArrowButton");
        var rightButton = this.FindControl<Button>("RightArrowButton");

        leftButton.IsVisible = ScrollViewer.Offset.X > 0;
        rightButton.IsVisible = ScrollViewer.Offset.X < ScrollViewer.Extent.Width - ScrollViewer.Viewport.Width;
    }
}