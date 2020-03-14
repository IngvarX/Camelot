using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Camelot.ViewModels.MainWindow;
using DynamicData;

namespace Camelot.Views.Main
{
    public class FilesPanelView : UserControl
    {
        public FilesPanelView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnDataGridSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            // TODO: swap RemovedItems and AddedItems after fixing bug in avalonia
            var viewModel = (FilesPanelViewModel) DataContext;

            var addedItems = args.RemovedItems.Cast<FileViewModel>();
            viewModel.SelectedFiles.AddRange(addedItems);

            var removedItems = args.AddedItems.Cast<FileViewModel>();
            viewModel.SelectedFiles.RemoveMany(removedItems);
        }

        private void OnDataGridCellPointerPressed(object sender, DataGridCellPointerPressedEventArgs args)
        {
            if (args.PointerPressedEventArgs.ClickCount == 2)
            {
                var fileViewModel = (FileViewModel)args.Cell.DataContext;
                fileViewModel.Open();
            }
        }
    }
}