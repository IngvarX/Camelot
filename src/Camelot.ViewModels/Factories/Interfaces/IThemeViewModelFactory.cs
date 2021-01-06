using System.Collections.Generic;
using Camelot.ViewModels.Implementations.Settings.General;

namespace Camelot.ViewModels.Factories.Interfaces
{
    public interface IThemeViewModelFactory
    {
        IReadOnlyList<ThemeViewModel> CreateAll();
    }
}