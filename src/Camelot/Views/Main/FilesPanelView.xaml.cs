using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Camelot.Extensions;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using DynamicData;

namespace Camelot.Views.Main
{
    public class FilesPanelView : UserControl
    {
        private DataGrid FilesDataGrid => this.FindControl<DataGrid>("FilesDataGrid");

        private TextBox DirectoryTextBox => this.FindControl<TextBox>("DirectoryTextBox");

        private FilesPanelViewModel ViewModel => (FilesPanelViewModel) DataContext;

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
        }

        private void ViewModelOnDeactivated(object sender, EventArgs e) => ClearSelection();

        private void ViewModelOnActivated(object sender, EventArgs e)
        {
            if (!DirectoryTextBox.IsFocused)
            {
                FilesDataGrid.Focus();
            }
        }

        private void OnDataGridSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            var addedItems = args
                .AddedItems
                .Cast<IFileSystemNodeViewModel>()
                .Where(i => !(i is IDirectoryViewModel directoryViewModel && directoryViewModel.IsParentDirectory))
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

        private void OnDataGridCellPointerPressed(object sender, DataGridCellPointerPressedEventArgs args) =>
            ActivateViewModel();

        private void OnDirectoryTextBoxGotFocus(object sender, GotFocusEventArgs args) => ActivateViewModel();

        private void ActivateViewModel() => ViewModel.ActivateCommand.Execute(null);

        private void OnNameTextBlockTapped(object sender, RoutedEventArgs args)
        {
            var textBlock = (TextBlock) sender;
            var viewModel = (IFileSystemNodeViewModel) textBlock.DataContext;

            if (viewModel.IsWaitingForEdit)
            {
                viewModel.IsEditing = true;

                // focus text box with file/dir name
                var textBox = textBlock.Parent.VisualChildren.OfType<TextBox>().Single();
                textBox.Focus();
            }
            else
            {
                viewModel.IsWaitingForEdit = true;
            }
        }

        private void OnFullNameTextBoxLostFocus(object sender, RoutedEventArgs args)
        {
            var textBox = (TextBox) sender;
            var viewModel = (IFileSystemNodeViewModel) textBox.DataContext;

            viewModel.IsEditing = false;
        }

        private static void StopEditing(IFileSystemNodeViewModel viewModel) =>
            viewModel.IsWaitingForEdit = viewModel.IsEditing = false;

        private void ClearSelection() => FilesDataGrid.SelectedItems.Clear();
    }
}