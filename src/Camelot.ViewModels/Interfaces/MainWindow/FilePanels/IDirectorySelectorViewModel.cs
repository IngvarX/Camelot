using System;

namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels
{
    public interface IDirectorySelectorViewModel
    {
        string CurrentDirectory { get; set; }

        bool ShouldShowSuggestions { get; set; }

        event EventHandler<EventArgs> CurrentDirectoryChanged;
    }
}