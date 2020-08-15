using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Camelot.Views.Main.OperationsStates
{
    public class ActiveOperationsView :  UserControl
    {
        public ActiveOperationsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}