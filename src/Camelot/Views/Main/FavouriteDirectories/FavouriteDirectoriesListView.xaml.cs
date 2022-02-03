using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Camelot.Extensions;

namespace Camelot.Views.Main.FavouriteDirectories;

public class FavouriteDirectoriesListView : UserControl
{
    private const int ScrollsCount = 6;

    private ScrollViewer ScrollViewer => this.FindControl<ScrollViewer>("FavDirsScrollViewer");

    public FavouriteDirectoriesListView()
    {
        InitializeComponent();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

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