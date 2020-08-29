using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Camelot.Views.Main.Directories
{
    public class FavouriteDirectoryView : UserControl
    {
        public FavouriteDirectoryView()
        {
            InitializeComponent();
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}