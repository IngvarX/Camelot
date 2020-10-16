using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels;

namespace Camelot.Views.Main.Controls
{
    public class SearchView : UserControl
    {
        private SearchViewModel ViewModel => (SearchViewModel) DataContext;

        public SearchView()
        {
            InitializeComponent();
            Initialized += OnInitialized;
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

        private void OnInitialized(object sender, EventArgs e) =>
            ViewModel.SearchSettingsChanged += ViewModelOnSearchSettingsChanged;

        private void ViewModelOnSearchSettingsChanged(object sender, EventArgs e)
        {
            if (ViewModel.IsSearchEnabled)
            {
                var textBox = this.FindControl<TextBox>("SearchTextBox");

                textBox.Focus();
            }
        }
    }
}