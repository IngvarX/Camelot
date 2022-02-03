using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;

namespace Camelot.ViewModels.Factories.Interfaces;

public interface ISuggestedPathViewModelFactory
{
    ISuggestedPathViewModel Create(string searchText, SuggestionModel model);
}