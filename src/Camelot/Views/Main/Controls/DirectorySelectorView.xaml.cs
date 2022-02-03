using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;

namespace Camelot.Views.Main.Controls;

public class DirectorySelectorView : UserControl
{
    private ListBox SuggestionsListBox => this.FindControl<ListBox>("SuggestionsListBox");

    public TextBox DirectoryTextBox => this.FindControl<TextBox>("DirectoryTextBox");

    private IDirectorySelectorViewModel ViewModel => (IDirectorySelectorViewModel) DataContext;

    public DirectorySelectorView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void OnDataContextChanged(object sender, EventArgs e)
    {
        DataContextChanged -= OnDataContextChanged;
        ViewModel.ActivationRequested += ViewModelOnActivationRequested;
    }

    private void ViewModelOnActivationRequested(object sender, EventArgs e)
    {
        DirectoryTextBox.CaretIndex = DirectoryTextBox.Text.Length;
        DirectoryTextBox.Focus();
    }

    private void SuggestionViewOnTapped(object sender, RoutedEventArgs e) =>
        SelectDirectory(((IDataContextProvider) sender).DataContext);

    private void DirectoryTextBoxOnKeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Down && e.Key != Key.Escape)
        {
            return;
        }

        if (e.Key == Key.Down)
        {
            SuggestionsListBox.SelectedIndex = 0;
            var topItem = SuggestionsListBox
                .GetVisualDescendants()
                .OfType<SuggestionView>()
                .FirstOrDefault();
            topItem?.Focus();
        }
        else
        {
            HideSuggestionsPopup();
        }

        e.Handled = true;
    }

    private void SuggestionsListBoxOnKeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Enter:
                SelectDirectory(SuggestionsListBox.SelectedItem);
                break;
            case Key.Up when SuggestionsListBox.SelectedIndex == 0:
                SuggestionsListBox.SelectedItem = null;
                DirectoryTextBox.Focus();
                break;
            case Key.Escape:
                HideSuggestionsPopup();
                break;
        }
    }

    private void SelectDirectory(object sender)
    {
        var viewModel = (ISuggestedPathViewModel) sender;

        DirectoryTextBox.Text = viewModel.FullPath;
        DirectoryTextBox.CaretIndex = DirectoryTextBox.Text.Length;
        DirectoryTextBox.Focus();
    }

    private void HideSuggestionsPopup() => ViewModel.ShouldShowSuggestions = false;
}