using System.Collections.Generic;

namespace Camelot.Services.Abstractions.Models.Operations;

public class UnaryFileSystemOperationSettings
{
    public IReadOnlyList<string> TopLevelDirectories { get; }

    public IReadOnlyList<string> TopLevelFiles { get; }

    public string SourceDirectory { get; }

    public UnaryFileSystemOperationSettings(
        IReadOnlyList<string> topLevelDirectories,
        IReadOnlyList<string> topLevelFiles,
        string sourceDirectory)
    {
        TopLevelDirectories = topLevelDirectories;
        TopLevelFiles = topLevelFiles;
        SourceDirectory = sourceDirectory;
    }
}