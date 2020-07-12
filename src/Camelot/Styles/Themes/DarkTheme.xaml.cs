using Avalonia.Markup.Xaml;
using AvaloniaStyles = Avalonia.Styling.Styles;

namespace Camelot.Styles.Themes
{
    public class DarkTheme : AvaloniaStyles
    {
        public DarkTheme() => AvaloniaXamlLoader.Load(this);
    }
}