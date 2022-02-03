using System.Collections.Generic;
using System.IO;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services.Environment.Implementations;

public class EnvironmentDirectoryServiceWindowsDecorator : IEnvironmentDirectoryService
{
    private readonly IEnvironmentDirectoryService _environmentDirectoryService;

    public EnvironmentDirectoryServiceWindowsDecorator(IEnvironmentDirectoryService environmentDirectoryService)
    {
        _environmentDirectoryService = environmentDirectoryService;
    }

    public void CreateDirectory(string directory) => _environmentDirectoryService.CreateDirectory(directory);

    public IEnumerable<string> EnumerateFilesRecursively(string directory) =>
        _environmentDirectoryService.EnumerateFilesRecursively(PreprocessPath(directory));

    public IEnumerable<string> EnumerateDirectoriesRecursively(string directory) =>
        _environmentDirectoryService.EnumerateDirectoriesRecursively(PreprocessPath(directory));

    public IEnumerable<string> EnumerateFileSystemEntriesRecursively(string directory) =>
        _environmentDirectoryService.EnumerateFileSystemEntriesRecursively(PreprocessPath(directory));

    public DirectoryInfo GetDirectory(string directory) =>
        _environmentDirectoryService.GetDirectory(PreprocessPath(directory));

    public string[] GetDirectories(string directory) =>
        _environmentDirectoryService.GetDirectories(PreprocessPath(directory));

    public bool CheckIfExists(string directory) =>
        _environmentDirectoryService.CheckIfExists(PreprocessPath(directory));

    public string GetCurrentDirectory() => _environmentDirectoryService.GetCurrentDirectory();

    public void Move(string sourceDirectory, string destinationDirectory) =>
        _environmentDirectoryService.Move(sourceDirectory, destinationDirectory);

    public void Delete(string path, bool recursive) => _environmentDirectoryService.Delete(path, recursive);

    private static string PreprocessPath(string directory) =>
        directory.EndsWith("\\") ? directory : directory + "\\";
}