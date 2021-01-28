using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Camelot.ViewModels.Implementations.Dialogs.Results;

namespace Camelot.Views.Dialogs
{
    public class OpenWithDialog : DialogWindowBase<OpenWithDialogResult>
    {
        public OpenWithDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

        private void DefaultAppsListBoxOnGotFocus(object sender, GotFocusEventArgs e)
        {
            var otherAppsListBox = this.FindControl<ListBox>("OtherAppsListBox");

            otherAppsListBox.Selection.Clear();
        }

        private void OtherAppsListBoxOnGotFocus(object sender, GotFocusEventArgs e)
        {
            var defaultAppsListBox = this.FindControl<ListBox>("DefaultAppsListBox");

            defaultAppsListBox.Selection.Clear();
        }
    }
}
