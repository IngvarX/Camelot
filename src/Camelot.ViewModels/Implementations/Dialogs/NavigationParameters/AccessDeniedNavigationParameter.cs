using Camelot.ViewModels.Services;

namespace Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;

public class AccessDeniedNavigationParameter : NavigationParameterBase
{
    public string Directory { get; }

    public AccessDeniedNavigationParameter(string directory)
    {
        Directory = directory;
    }
}