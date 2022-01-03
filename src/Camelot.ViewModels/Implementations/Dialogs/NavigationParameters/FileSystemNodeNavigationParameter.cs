using Camelot.ViewModels.Services;

namespace Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;

public class FileSystemNodeNavigationParameter : NavigationParameterBase
{
    public string NodePath { get; }

    public FileSystemNodeNavigationParameter(string nodePath)
    {
        NodePath = nodePath;
    }
}