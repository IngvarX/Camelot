using System;
using Camelot.Services.Abstractions.Models;

namespace Camelot.Services.Abstractions;

public interface IQuickSearchService
{
    bool IsEnabled { get; }

    event EventHandler<EventArgs> QuickSearchModeChanged;

    QuickSearchModel GetQuickSearchSettings();

    void SaveQuickSearchSettings(QuickSearchModel quickSearchModel);
}