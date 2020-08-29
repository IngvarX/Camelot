using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Camelot.Views.Main.Directories
{
    public class FavouriteDirectoriesListView : UserControl
    {
        public FavouriteDirectoriesListView()
        {
            InitializeComponent();
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}