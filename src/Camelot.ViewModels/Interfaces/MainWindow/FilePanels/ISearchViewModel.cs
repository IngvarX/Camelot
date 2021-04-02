using System;

namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels
{
    public interface ISearchViewModel
    {
        event EventHandler<EventArgs> SearchSettingsChanged;

        INodeSpecification GetSpecification();

        void ToggleSearch();
    }
}