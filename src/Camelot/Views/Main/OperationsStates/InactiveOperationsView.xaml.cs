using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Camelot.Views.Main.OperationsStates
{
    public class InactiveOperationsView :  UserControl
    {
        public InactiveOperationsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}