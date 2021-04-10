using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Camelot.Views.Main.OperationsStates
{
    public class OperationsStatesListView : UserControl
    {
        public OperationsStatesListView()
        {
            InitializeComponent();
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}