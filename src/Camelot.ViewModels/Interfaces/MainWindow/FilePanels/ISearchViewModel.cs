using System;

namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels;

public interface ISearchViewModel
{
    bool IsSearchEnabled { get; }

    event EventHandler<EventArgs> SearchSettingsChanged;

    INodeSpecification GetSpecification();

    void ToggleSearch();
}