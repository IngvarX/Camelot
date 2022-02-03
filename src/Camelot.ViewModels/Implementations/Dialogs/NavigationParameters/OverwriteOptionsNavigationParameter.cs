using Camelot.ViewModels.Services;

namespace Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;

public class OverwriteOptionsNavigationParameter : NavigationParameterBase
{
    public string SourceFilePath { get; }

    public string DestinationFilePath { get; }

    public bool AreMultipleFilesAvailable { get; }

    public OverwriteOptionsNavigationParameter(
        string sourceFilePath,
        string destinationFilePath,
        bool areMultipleFilesAvailable)
    {
        SourceFilePath = sourceFilePath;
        DestinationFilePath = destinationFilePath;
        AreMultipleFilesAvailable = areMultipleFilesAvailable;
    }
}