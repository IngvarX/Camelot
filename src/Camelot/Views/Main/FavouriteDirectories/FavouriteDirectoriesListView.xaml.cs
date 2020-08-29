using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Camelot.Views.Main.FavouriteDirectories
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