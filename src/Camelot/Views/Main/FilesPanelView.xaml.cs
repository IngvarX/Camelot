using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Camelot.ViewModels.Implementations.MainWindow;
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
            var addedItems = args.RemovedItems.Cast<FileViewModel>();
            ViewModel.SelectedFiles.AddRange(addedItems);

            var removedItems = args.AddedItems.Cast<FileViewModel>();
            ViewModel.SelectedFiles.RemoveMany(removedItems);
        }

        private void OnDataGridCellPointerPressed(object sender, DataGridCellPointerPressedEventArgs args)
        {
            if (args.PointerPressedEventArgs.ClickCount == 2)
            {
                var fileViewModel = (FileViewModel)args.Cell.DataContext;

                fileViewModel.OpenCommand.Execute(null);
            }
        }

        private void OnDataGridPointerReleased(object sender, PointerReleasedEventArgs args)
        {
            ViewModel.ActivateCommand.Execute(null);
        }
    }
}