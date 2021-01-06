using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Implementations.Settings.General;

namespace Camelot.ViewModels.Factories.Interfaces
{
    public interface IThemeViewModelFactory
    {
        ThemeViewModel Create(Theme theme);
    }
}