using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Camelot.Avalonia.Interfaces;
using Camelot.DependencyInjection;
using Camelot.Extensions;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Nodes;
using Camelot.Views.Main.Controls;
using DynamicData;
using Splat;

namespace Camelot.Views.Main;

public class FilesPanelView : UserControl
{
    private const string PasteFromClipboardMenuItemName = "PasteFromClipboard";
    private const int DragAndDropDelay = 300;

    private readonly Timer _timer;

    private bool _isCellPressed;
    private PointerEventArgs _pointerEventArgs;
    private IDataContextProvider _dataContextProvider;

    private DataGrid FilesDataGrid => this.FindControl<DataGrid>("FilesDataGrid");

    private DirectorySelectorView DirectorySelectorView => this.FindControl<DirectorySelectorView>("DirectorySelectorView");

    private IFilesPanelViewModel ViewModel => (IFilesPanelViewModel) DataContext;

    public FilesPanelView()
    {
        InitializeComponent();
        SubscribeToEvents();

        _timer = new Timer {Interval = DragAndDropDelay};
        _timer.Elapsed += TimerOnElapsed;
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void SubscribeToEvents() => DataContextChanged += OnDataContextChanged;

    private void OnDataContextChanged(object sender, EventArgs e)
    {
        ViewModel.Deactivated += ViewModelOnDeactivated;
        ViewModel.Activated += ViewModelOnActivated;
        ViewModel.SelectionAdded += ViewModelOnSelectionAdded;
        ViewModel.SelectionRemoved += ViewModelOnSelectionRemoved;
        DirectorySelectorView.DirectoryTextBox.GotFocus += OnDirectoryTextBoxGotFocus;

        FilesDataGrid.AddHandler(DragDrop.DropEvent, OnDrop);
    }

    private void ViewModelOnDeactivated(object sender, EventArgs e) => ClearSelection();

    private void ViewModelOnActivated(object sender, EventArgs e)
    {
        if (!DirectorySelectorView.DirectoryTextBox.IsFocused)
        {
            FilesDataGrid.Focus();
        }
    }

    private void ViewModelOnSelectionAdded(object sender, SelectionAddedEventArgs e)
    {
        var item = GetNode(e.NodePath);
        if (item is not null)
        {
            FilesDataGrid.SelectedItems.Add(item);
        }
    }

    private void ViewModelOnSelectionRemoved(object sender, SelectionRemovedEventArgs e)
    {
        var item = GetNode(e.NodePath);
        if (item is null)
        {
            return;
        }

        if (FilesDataGrid.SelectedItems.Count == 1)
        {
            FilesDataGrid.SelectedIndex = -1;
            FilesDataGrid.SelectedItems.Clear();
        }
        else
        {
            FilesDataGrid.SelectedItems.Remove(item);
        }
    }

    private IFileSystemNodeViewModel GetNode(string nodePath) => FilesDataGrid
        .Items
        .Cast<IFileSystemNodeViewModel>()
        .SingleOrDefault(m => m.FullPath == nodePath);

    private void OnDataGridSelectionChanged(object sender, SelectionChangedEventArgs args)
    {
        var addedItems = args
            .AddedItems
            .Cast<IFileSystemNodeViewModel>()
            .Where(i => i is not IDirectoryViewModel {IsParentDirectory: true})
            .ToArray();

        if (!ViewModel.IsActive)
        {
            // data grid inserted items in inactive file panel
            // remove them if any
            if (addedItems.Any())
            {
                ClearSelection();
            }

            return;
        }

        ViewModel.SelectedFileSystemNodes.AddRange(addedItems);

        var removedItems = args
            .RemovedItems
            .Cast<IFileSystemNodeViewModel>()
            .ToArray();
        ViewModel.SelectedFileSystemNodes.RemoveMany(removedItems);

        addedItems
            .Concat(removedItems)
            .ForEach(StopEditing);
    }

    private void OnDataGridDoubleTapped(object sender, RoutedEventArgs args)
    {
        if (args.Source is not IDataContextProvider dataContextProvider)
        {
            return;
        }

        if (dataContextProvider.DataContext is not IFileSystemNodeViewModel nodeViewModel)
        {
            return;
        }

        args.Handled = true;

        StopEditing(nodeViewModel);
        nodeViewModel.OpenCommand.Execute(null);
    }

    private void OnDataGridKeyDown(object sender, KeyEventArgs args)
    {
        if (args.Key != Key.Delete && args.Key != Key.Back)
        {
            return;
        }

        args.Handled = true;

        ViewModel.OperationsViewModel.MoveToTrashCommand.Execute(null);
    }

    private void OnDataGridCellPointerPressed(object sender, DataGridCellPointerPressedEventArgs args)
    {
        ActivateViewModel();
        ProcessPointerClickInCell(args.PointerPressedEventArgs, args.Cell);
        PrepareDrag(args);
    }

    private void OnDirectoryTextBoxGotFocus(object sender, GotFocusEventArgs args) => ActivateViewModel();

    private void ActivateViewModel() => ViewModel.ActivateCommand.Execute(null);

    private void ProcessPointerClickInCell(PointerEventArgs args, IDataContextProvider cell)
    {
        var point = args.GetCurrentPoint(this);
        if (point.Properties.IsMiddleButtonPressed 
            && cell.DataContext is IDirectoryViewModel directoryViewModel)
        {
            args.Handled = true;

            directoryViewModel.OpenInNewTabCommand.Execute(null);
        }
        else if (point.Properties.IsLeftButtonPressed
                 && args.Source is TextBlock {Name: "NameTextBlock"} textBlock)
        {
            if (textBlock.DataContext is not IFileSystemNodeViewModel viewModel)
            {
                return;
            }

            args.Handled = true;

            if (ViewModel.SelectedFileSystemNodes.Contains(viewModel))
            {
                viewModel.IsEditing = true;

                // focus text box with file/dir name
                var textBox = textBlock.Parent.VisualChildren.OfType<TextBox>().Single();
                textBox.Focus();
            }
        }
    }

    private void OnFullNameTextBoxLostFocus(object sender, RoutedEventArgs args)
    {
        var textBox = (TextBox) sender;
        if (textBox.DataContext is IFileSystemNodeViewModel viewModel)
        {
            viewModel.IsEditing = false;
        }
    }

    private static void StopEditing(IFileSystemNodeViewModel viewModel) => viewModel.IsEditing = false;

    private void ClearSelection() => FilesDataGrid.SelectedItems.Clear();

    private void PrepareDrag(DataGridCellPointerPressedEventArgs e)
    {
        _timer.Stop();

        if (!e.PointerPressedEventArgs.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            return;
        }

        if (e.Cell.DataContext is not IFileSystemNodeViewModel {IsEditing: false})
        {
            return;
        }

        if (e.Cell.DataContext is IDirectoryViewModel {IsParentDirectory: true})
        {
            return;
        }

        _isCellPressed = true;
        e.Cell.PointerReleased += CellOnPointerReleased;

        _pointerEventArgs = e.PointerPressedEventArgs;
        _dataContextProvider = e.Cell;
        _timer.Start();
    }

    private void CellOnPointerReleased(object sender, PointerReleasedEventArgs e)
    {
        var cell = (DataGridCell) sender;
        cell.PointerReleased -= CellOnPointerReleased;
        _isCellPressed = false;
    }

    private async Task DoDragAsync(IDataContextProvider sender, PointerEventArgs e)
    {
        if (!_isCellPressed)
        {
            return;
        }

        if (sender.DataContext is not IFileSystemNodeViewModel viewModel)
        {
            return;
        }

        var dragData = new DataObject();
        var fileNames = ViewModel
            .SelectedFileSystemNodes
            .Select(f => f.FullPath)
            .Concat(new[] {viewModel.FullPath})
            .ToHashSet();
        dragData.Set(DataFormats.FileNames, fileNames);

        try
        {
            await DragDrop.DoDragDrop(e, dragData,
                DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link);
        }
        catch
        {
            // ignore
        }
    }

    private async void OnDrop(object sender, DragEventArgs e)
    {
        if (!e.Data.Contains(DataFormats.FileNames))
        {
            return;
        }

        var fileNames = e.Data.GetFileNames()?.ToArray();
        if (fileNames is null || !fileNames.Any())
        {
            return;
        }

        if (e.Source is not IDataContextProvider dataContextProvider)
        {
            return;
        }

        var fullPath = ViewModel.CurrentDirectory;
        if (dataContextProvider.DataContext is IFileSystemNodeViewModel viewModel)
        {
            if (ViewModel.SelectedFileSystemNodes.Contains(viewModel))
            {
                return;
            }

            fullPath = viewModel.FullPath;
        }

        var mediator = ViewModel.DragAndDropOperationsMediator;
        if ((e.KeyModifiers & KeyModifiers.Shift) > 0)
        {
            await mediator.MoveFilesAsync(fileNames, fullPath);
        }
        else
        {
            await mediator.CopyFilesAsync(fileNames, fullPath);
        }

        e.Handled = true;
    }

    private async void TimerOnElapsed(object sender, ElapsedEventArgs e)
    {
        _timer.Stop();

        await DoDragInUiThreadAsync();
    }

    private async Task DoDragInUiThreadAsync()
    {
        var dispatcher = Locator.Current.GetRequiredService<IApplicationDispatcher>();

        await dispatcher.DispatchAsync(() => DoDragAsync(_dataContextProvider, _pointerEventArgs));
    }

    private async void DataGridOnContextMenuOpening(object sender, CancelEventArgs e)
    {
        var menu = (ContextMenu) sender;
        var item = menu
            .Items
            .Cast<MenuItem>()
            .SingleOrDefault(i => i.Name == PasteFromClipboardMenuItemName);
        if (item is not null)
        {
            item.IsVisible = await ViewModel.ClipboardOperationsViewModel.CanPasteAsync();
        }
    }
}