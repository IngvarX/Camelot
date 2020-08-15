using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Camelot.Views.Main.Controls
{
    public class DataGridHeaderArrowView :  UserControl
    {
        public DataGridHeaderArrowView()
        {
            InitializeComponent();
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}