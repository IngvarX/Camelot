using System.Collections.Generic;
using System.IO;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services.Environment.Implementations
{
    public class EnvironmentDirectoryService : IEnvironmentDirectoryService
    {
        public void CreateDirectory(string directory) =>
            Directory.CreateDirectory(directory);

        public IEnumerable<string> EnumerateFilesRecursively(string directory) =>
            Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories);

        public IEnumerable<string> EnumerateDirectoriesRecursively(string directory) =>
            Directory.EnumerateDirectories(directory, "*.*", SearchOption.AllDirectories);

        public IEnumerable<string> EnumerateFileSystemEntriesRecursively(string directory) =>
            Directory.EnumerateFileSystemEntries(directory, "*.*", SearchOption.AllDirectories);

        public DirectoryInfo GetDirectory(string directory) =>
            new DirectoryInfo(directory);

        public string[] GetDirectories(string directory) =>
            Directory.GetDirectories(directory);

        public bool CheckIfExists(string directory) =>
            Directory.Exists(directory);

        public string GetCurrentDirectory() =>
            Directory.GetCurrentDirectory();

        public void Move(string sourceDirectory, string destinationDirectory) =>
            Directory.Move(sourceDirectory, destinationDirectory);

        public void Delete(string path, bool recursive) =>
            Directory.Delete(path, recursive);
    }
}