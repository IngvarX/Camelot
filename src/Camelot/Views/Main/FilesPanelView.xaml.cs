using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Camelot.Avalonia.Interfaces;
using Camelot.DependencyInjection;
using Camelot.Extensions;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Nodes;
using Camelot.Views.Main.Controls;
using DynamicData;
using Splat;

namespace Camelot.Views.Main
{
    public class FilesPanelView : UserControl
    {
        private const int DragAndDropDelay = 200;

        private bool _isCellPressed;

        private DataGrid FilesDataGrid => this.FindControl<DataGrid>("FilesDataGrid");

        private DirectorySelectorView DirectorySelectorView => this.FindControl<DirectorySelectorView>("DirectorySelectorView");

        private IFilesPanelViewModel ViewModel => (IFilesPanelViewModel) DataContext;

        public FilesPanelView()
        {
            InitializeComponent();
            SubscribeToEvents();
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

        private void SubscribeToEvents() => DataContextChanged += OnDataContextChanged;

        private void OnDataContextChanged(object sender, EventArgs e)
        {
            ViewModel.Deactivated += ViewModelOnDeactivated;
            ViewModel.Activated += ViewModelOnActivated;
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

        private void OnDataGridSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            var addedItems = args
                .AddedItems
                .Cast<IFileSystemNodeViewModel>()
                .Where(i => !(i is IDirectoryViewModel {IsParentDirectory: true}))
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
            if (!(args.Source is IDataContextProvider dataContextProvider))
            {
                return;
            }

            if (!(dataContextProvider.DataContext is IFileSystemNodeViewModel nodeViewModel))
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
            if (point.Properties.IsMiddleButtonPressed &&
                cell.DataContext is IDirectoryViewModel directoryViewModel)
            {
                args.Handled = true;

                directoryViewModel.OpenInNewTabCommand.Execute(null);
            }
            else if (point.Properties.IsLeftButtonPressed
                     && args.Source is TextBlock {Name: "NameTextBlock"} textBlock)
            {
                if (!(textBlock.DataContext is IFileSystemNodeViewModel viewModel))
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
            if (!e.PointerPressedEventArgs.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                return;
            }

            if (!(e.Cell.DataContext is IFileSystemNodeViewModel {IsEditing: false}))
            {
                return;
            }

            _isCellPressed = true;
            e.Cell.PointerReleased += CellOnPointerReleased;

            Task.Delay(DragAndDropDelay).ContinueWith(_ =>
            {
                var dispatcher = Locator.Current.GetRequiredService<IApplicationDispatcher>();
                dispatcher.DispatchAsync(() => DoDragAsync(e.Cell, e.PointerPressedEventArgs));
            });
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

            if (!(sender.DataContext is IFileSystemNodeViewModel viewModel))
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

            await DragDrop.DoDragDrop(e, dragData,
                DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link);
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

            if (!(e.Source is IDataContextProvider dataContextProvider))
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
    }
}