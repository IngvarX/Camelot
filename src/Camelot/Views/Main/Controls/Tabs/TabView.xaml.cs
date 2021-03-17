using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Tabs;

namespace Camelot.Views.Main.Controls.Tabs
{
    public class TabView : UserControl
    {
        private const string DataFormat = "Tab";

        private ITabViewModel ViewModel => (ITabViewModel) DataContext;

        public TabView()
        {
            InitializeComponent();

            SetupDragAndDrop();
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

        private void ButtonOnPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            if (e.InitialPressMouseButton != MouseButton.Middle)
            {
                return;
            }

            e.Handled = true;
            ViewModel.CloseTabCommand.Execute(null);
        }

        private void SetupDragAndDrop()
        {
            var grid = this.Find<Grid>("TabGrid");
            grid.PointerPressed += DoDrag;

            AddHandler(DragDrop.DropEvent, Drop);
            AddHandler(DragDrop.DragOverEvent, DragOver);
        }

        private async void DoDrag(object sender, PointerPressedEventArgs e)
        {
            var dragData = new DataObject();
            dragData.Set(DataFormat, ViewModel);

            await DragDrop.DoDragDrop(e, dragData, DragDropEffects.Copy);
            ViewModel.ActivateCommand.Execute(null);
        }

        private void DragOver(object sender, DragEventArgs e)
        {

        }

        private void Drop(object sender, DragEventArgs e)
        {
            if (e.Data.Contains(DataFormat)
                && e.Data.Get(DataFormat) is ITabViewModel tabViewModel
                && tabViewModel != ViewModel)
            {
                tabViewModel.RequestMoveCommand.Execute(ViewModel);
            }
        }
    }
}