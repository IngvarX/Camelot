using System.Threading.Tasks;
using System.Timers;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Camelot.Avalonia.Interfaces;
using Camelot.DependencyInjection;
using Camelot.ViewModels.Interfaces.MainWindow.Directories;
using Splat;

namespace Camelot.Views.Main.FavouriteDirectories;

public class FavouriteDirectoryView : UserControl
{
    private const int DragAndDropDelay = 200;
    private const string DataFormat = "FavDir";

    private readonly Timer _timer;

    private PointerEventArgs _pointerEventArgs;

    private Grid Grid => this.Find<Grid>("FavDirGrid");

    private IFavouriteDirectoryViewModel ViewModel => (IFavouriteDirectoryViewModel) DataContext;

    private bool _isGridPressed;

    public FavouriteDirectoryView()
    {
        InitializeComponent();
        SetupDragAndDrop();

        _timer = new Timer {Interval = DragAndDropDelay};
        _timer.Elapsed += TimerOnElapsed;
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void SetupDragAndDrop()
    {
        Grid.PointerPressed += PrepareDrag;

        AddHandler(DragDrop.DropEvent, OnDrop);
    }

    private void PrepareDrag(object sender, PointerPressedEventArgs e)
    {
        _timer.Stop();

        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            return;
        }

        _isGridPressed = true;
        Grid.PointerReleased += GridOnPointerReleased;

        _pointerEventArgs = e;
        _timer.Start();
    }

    private async Task DoDragAsync(PointerEventArgs e)
    {
        if (!_isGridPressed)
        {
            return;
        }

        var dragData = new DataObject();
        dragData.Set(DataFormat, ViewModel);

        try
        {
            await DragDrop.DoDragDrop(e, dragData, DragDropEffects.Move);
        }
        catch
        {
            // ignore
        }
    }

    private void GridOnPointerReleased(object sender, PointerReleasedEventArgs e)
    {
        Grid.PointerReleased -= GridOnPointerReleased;
        _isGridPressed = false;
    }

    private void OnDrop(object sender, DragEventArgs e)
    {
        if (e.Data.Contains(DataFormat)
            && e.Data.Get(DataFormat) is IFavouriteDirectoryViewModel favouriteDirectoryViewModel
            && favouriteDirectoryViewModel != ViewModel)
        {
            favouriteDirectoryViewModel.RequestMoveCommand.Execute(ViewModel);
        }
    }

    private async void TimerOnElapsed(object sender, ElapsedEventArgs e)
    {
        _timer.Stop();

        await DoDragInUiThreadAsync();
    }

    private async Task DoDragInUiThreadAsync()
    {
        var dispatcher = Locator.Current.GetRequiredService<IApplicationDispatcher>();

        await dispatcher.DispatchAsync(() => DoDragAsync(_pointerEventArgs));
    }
}