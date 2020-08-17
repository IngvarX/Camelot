using System;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Specifications;

namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels
{
    public interface ISearchViewModel
    {
        event EventHandler<EventArgs> SearchSettingsChanged;

        ISpecification<NodeModelBase> GetSpecification();

        void ToggleSearch();
    }
}