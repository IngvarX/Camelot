using Avalonia.Markup.Xaml;
using AvaloniaStyles = Avalonia.Styling.Styles;

namespace Camelot.Styles.Themes
{
    public class LightTheme : AvaloniaStyles
    {
        public LightTheme() => AvaloniaXamlLoader.Load(this);
    }
}