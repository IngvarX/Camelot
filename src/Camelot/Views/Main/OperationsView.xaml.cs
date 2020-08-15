using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Camelot.Views.Main
{
    public class OperationsView : UserControl
    {
        public OperationsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}