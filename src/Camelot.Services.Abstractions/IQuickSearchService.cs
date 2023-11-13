using Camelot.Services.Abstractions.Models;
using System.Collections.Generic;

namespace Camelot.Services.Abstractions;

public interface IQuickSearchService
{
    bool IsEnabled { get; }

    QuickSearchModel GetQuickSearchSettings();

    void SaveQuickSearchSettings(QuickSearchModel quickSearchModel);

    /// <param name="symbol">
    /// This arg is is of type 'char' and not 'Key', since translation from Key to char
    ///  is language/keyboard dependent, and should be done in caller level by Avalonia.</param>
    /// <param name="isBackwardsDirectionEnabled"></param>
    /// <param name="nodes"></param>
    IReadOnlyList<QuickSearchNodeModel> FilterNodes(char symbol, bool isBackwardsDirectionEnabled, IReadOnlyList<string> nodes);

    void ClearSearch();
}