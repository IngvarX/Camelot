using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Camelot.Avalonia.Interfaces;
using Camelot.DependencyInjection;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Tabs;
using Splat;

namespace Camelot.Views.Main.Controls.Tabs
{
    public class TabView : UserControl
    {
        private const int DragAndDropDelay = 200;
        private const string DataFormat = "Tab";

        private Grid Grid => this.Find<Grid>("TabGrid");

        private ITabViewModel ViewModel => (ITabViewModel) DataContext;

        private bool _isGridPressed;

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
            Grid.PointerPressed += PrepareDrag;

            AddHandler(DragDrop.DropEvent, OnDrop);
        }

        private void PrepareDrag(object sender, PointerPressedEventArgs e)
        {
            if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                return;
            }

            _isGridPressed = true;
            Grid.PointerReleased += GridOnPointerReleased;

            Task.Delay(DragAndDropDelay).ContinueWith(_ =>
            {
                var dispatcher = Locator.Current.GetRequiredService<IApplicationDispatcher>();
                dispatcher.DispatchAsync(() => DoDragAsync(e));
            });
        }

        private async Task DoDragAsync(PointerEventArgs e)
        {
            var button = this.Find<Button>("TabButton");
            button.Classes.Set(":pressed", false);

            if (!_isGridPressed)
            {
                return;
            }

            var dragData = new DataObject();
            dragData.Set(DataFormat, ViewModel);

            await DragDrop.DoDragDrop(e, dragData, DragDropEffects.Move);

            ViewModel.ActivateCommand.Execute(null);
        }

        private void GridOnPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            Grid.PointerReleased -= GridOnPointerReleased;
            _isGridPressed = false;
        }

        private void OnDrop(object sender, DragEventArgs e)
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