using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
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
            var addedItems = args.RemovedItems.Cast<IFileSystemNodeViewModel>();
            ViewModel.SelectedFileSystemNodes.AddRange(addedItems);

            var removedItems = args.AddedItems.Cast<IFileSystemNodeViewModel>();
            ViewModel.SelectedFileSystemNodes.RemoveMany(removedItems);
        }

        private void OnDataGridCellPointerPressed(object sender, DataGridCellPointerPressedEventArgs args)
        {
            if (args.PointerPressedEventArgs.ClickCount == 2)
            {
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
            var textBlock = (TextBlock) sender;
            // TODO: check if selected?
            var viewModel = (IFileSystemNodeViewModel) textBlock.DataContext;

            viewModel.IsEditing = true;
        }

        private void OnFullNameTextBoxLostFocus(object sender, RoutedEventArgs args)
        {
            var textBlock = (TextBox) sender;
            var viewModel = (IFileSystemNodeViewModel) textBlock.DataContext;

            viewModel.IsEditing = false;
        }
    }
}