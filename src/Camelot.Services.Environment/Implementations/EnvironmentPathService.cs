using System.IO;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services.Environment.Implementations
{
    public class EnvironmentPathService : IEnvironmentPathService
    {
        public string GetDirectoryName(string path) => Path.GetDirectoryName(path);

        public string Combine(string path1, string path2) => Path.Combine(path1, path2);

        public string GetRelativePath(string relativeTo, string path) => Path.GetRelativePath(relativeTo, path);

        public string GetPathRoot(string path) => Path.GetPathRoot(path);

        public string GetFileNameWithoutExtension(string path) => Path.GetFileNameWithoutExtension(path);

        public string GetFileName(string path) => Path.GetFileName(path);

        public string GetExtension(string path) => Path.GetExtension(path);
    }
}