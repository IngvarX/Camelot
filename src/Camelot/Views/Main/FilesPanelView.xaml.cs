using System;
using System.Linq;
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
        private FilesPanelViewModel ViewModel => (FilesPanelViewModel) DataContext;

        public FilesPanelView()
        {
            InitializeComponent();
            SubscribeToEvents();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void SubscribeToEvents()
        {
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, EventArgs e)
        {
            ViewModel.DeactivatedEvent += ViewModelOnDeactivatedEvent;
        }

        private void ViewModelOnDeactivatedEvent(object sender, EventArgs e)
        {
            var dataGrid = this.FindControl<DataGrid>("FilesDataGrid");

            dataGrid.SelectedItems.Clear();
        }

        private void OnDataGridSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            // TODO: swap RemovedItems and AddedItems after fixing bug in avalonia
            var addedItems = args
                .RemovedItems
                .Cast<IFileSystemNodeViewModel>()
                .ToArray();
            ViewModel.SelectedFileSystemNodes.AddRange(addedItems);
            addedItems.ForEach(vm => vm.IsSelected = true);

            var removedItems = args
                .AddedItems
                .Cast<IFileSystemNodeViewModel>()
                .ToArray();
            ViewModel.SelectedFileSystemNodes.RemoveMany(removedItems);

            addedItems.Concat(removedItems).ForEach(vm => { vm.IsEditing = false; });
        }

        private void OnDataGridCellPointerPressed(object sender, DataGridCellPointerPressedEventArgs args)
        {
            if (args.PointerPressedEventArgs.ClickCount == 2)
            {
                args.PointerPressedEventArgs.Handled = true;
                var fileViewModel = (IFileSystemNodeViewModel)args.Cell.DataContext;

                fileViewModel.OpenCommand.Execute(null);
            }
        }

        private void OnDataGridPointerReleased(object sender, PointerReleasedEventArgs args)
        {
            ViewModel.ActivateCommand.Execute(null);
        }

        private void OnNameTextBlockPointerPressed(object sender, PointerPressedEventArgs args)
        {
            if (args.MouseButton != MouseButton.Left || args.ClickCount == 2)
            {
                return;
            }
            
            var textBlock = (TextBlock) sender;
            var viewModel = (IFileSystemNodeViewModel) textBlock.DataContext;

            if (viewModel.IsSelected)
            {
                viewModel.IsEditing = true;
            }
        }

        private void OnFullNameTextBoxLostFocus(object sender, RoutedEventArgs args)
        {
            var textBox = (TextBox) sender;
            var viewModel = (IFileSystemNodeViewModel) textBox.DataContext;

            viewModel.IsEditing = false;
        }
    }
}