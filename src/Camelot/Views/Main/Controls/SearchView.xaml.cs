using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;

namespace Camelot.Views.Main.Controls;

public class SearchView : UserControl
{
    private ISearchViewModel ViewModel => (ISearchViewModel) DataContext;

    public SearchView()
    {
        InitializeComponent();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        ViewModel.SearchSettingsChanged += ViewModelOnSearchSettingsChanged;
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void ViewModelOnSearchSettingsChanged(object sender, EventArgs e)
    {
        if (ViewModel.IsSearchEnabled)
        {
            var textBox = this.FindControl<TextBox>("SearchTextBox");

            textBox.Focus();
        }
    }
}