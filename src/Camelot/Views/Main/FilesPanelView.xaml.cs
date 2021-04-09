using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Camelot.Extensions;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Nodes;
using Camelot.Views.Main.Controls;
using DynamicData;

namespace Camelot.Views.Main
{
    public class FilesPanelView : UserControl
    {
        private const string DataFormat = "Node";

        private DataGrid FilesDataGrid => this.FindControl<DataGrid>("FilesDataGrid");

        private TextBox DirectoryTextBox => this.FindControl<TextBox>("DirectoryTextBox");

        private ListBox SuggestionsListBox => this.FindControl<ListBox>("SuggestionsListBox");

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

            FilesDataGrid.AddHandler(DragDrop.DropEvent, OnDrop);
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
            DoDrag(args);
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
        }

        private void OnNameTextBlockTapped(object sender, RoutedEventArgs args)
        {
            var textBlock = (TextBlock) sender;
            if (!(textBlock.DataContext is IFileSystemNodeViewModel viewModel))
            {
                return;
            }

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
            if (textBox.DataContext is IFileSystemNodeViewModel viewModel)
            {
                viewModel.IsEditing = false;
            }
        }

        private static void StopEditing(IFileSystemNodeViewModel viewModel) =>
            viewModel.IsWaitingForEdit = viewModel.IsEditing = false;

        private void ClearSelection() => FilesDataGrid.SelectedItems.Clear();

        private void SuggestionViewOnTapped(object sender, RoutedEventArgs e) =>
            SelectDirectory(((IDataContextProvider) sender).DataContext);

        private void DirectoryTextBoxOnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Down && e.Key != Key.Escape)
            {
                return;
            }

            if (e.Key == Key.Down)
            {
                SuggestionsListBox.SelectedIndex = 0;
                var topItem = SuggestionsListBox
                    .GetVisualDescendants()
                    .OfType<SuggestionView>()
                    .FirstOrDefault();
                topItem?.Focus();
            }
            else
            {
                HideSuggestionsPopup();
            }

            e.Handled = true;
        }

        private void SuggestionsListBoxOnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    SelectDirectory(SuggestionsListBox.SelectedItem);
                    break;
                case Key.Up when SuggestionsListBox.SelectedIndex == 0:
                    SuggestionsListBox.SelectedItem = null;
                    DirectoryTextBox.Focus();
                    break;
                case Key.Escape:
                    HideSuggestionsPopup();
                    break;
            }
        }

        private void SelectDirectory(object sender)
        {
            var viewModel = (ISuggestedPathViewModel) sender;

            DirectoryTextBox.Text = viewModel.FullPath;
            DirectoryTextBox.CaretIndex = DirectoryTextBox.Text.Length;
            DirectoryTextBox.Focus();
        }

        private void HideSuggestionsPopup() => ViewModel.ShouldShowSuggestions = false;

        private async void DoDrag(DataGridCellPointerPressedEventArgs e)
        {
            if (!e.PointerPressedEventArgs.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                return;
            }

            if (!(e.Cell.DataContext is IFileSystemNodeViewModel viewModel))
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

            await DragDrop.DoDragDrop(e.PointerPressedEventArgs, dragData, DragDropEffects.Copy);
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

            switch (e.DragEffects)
            {
                case DragDropEffects.Copy:
                    await ViewModel.DragAndDropOperationsViewModel.CopyFilesAsync(fileNames, fullPath);
                    break;
                case DragDropEffects.Move:
                    await ViewModel.DragAndDropOperationsViewModel.MoveFilesAsync(fileNames, fullPath);
                    break;
            }
        }
    }
}