using System;
using System.Windows.Input;

namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels;

public interface IDirectorySelectorViewModel
{
    string CurrentDirectory { get; set; }

    bool ShouldShowSuggestions { get; set; }

    event EventHandler<System.EventArgs> ActivationRequested;

    ICommand ToggleFavouriteStatusCommand { get; }

    void Activate();
}