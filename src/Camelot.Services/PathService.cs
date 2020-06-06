using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Camelot.Services.Abstractions;

namespace Camelot.Services
{
    public class PathService : IPathService
    {
        public string GetCommonRootDirectory(IReadOnlyList<string> paths)
        {
            if (!paths.Any())
            {
                return null;
            }

            var commonPrefix = new string(
                paths
                    .First()
                    .Substring(0, paths.Min(s => s.Length))
                    .TakeWhile((c, i) => paths.All(s => s[i] == c)).ToArray()
            );

            return new[] {'/', '\\'}.Contains(commonPrefix[^1])
                ? TrimPathSeparators(commonPrefix)
                : GetParentDirectory(commonPrefix);
        }

        public string GetParentDirectory(string path) => Path.GetDirectoryName(path);

        public string Combine(string path1, string path2) => Path.Combine(path1, path2);

        public string GetRelativePath(string relativeTo, string path) => Path.GetRelativePath(relativeTo, path);

        public string GetPathRoot(string path) => Path.GetPathRoot(path);

        public string GetFileNameWithoutExtension(string path)
        {
            if (!path.StartsWith("."))
            {
                return Path.GetFileNameWithoutExtension(path);
            }

            if (path.Count(c => c == '.') == 1)
            {
                return path;
            }

            var lastDot = path.LastIndexOf(".", StringComparison.InvariantCulture);

            return path.Substring(0, lastDot);
        }

        public string GetFileName(string path)
        {
            var fileName = Path.GetFileName(path);

            return string.IsNullOrEmpty(fileName) ? path : fileName;
        }

        public string GetExtension(string path)
        {
            if (path.StartsWith("."))
            {
                if (path.Count(c => c == '.') == 1)
                {
                    return string.Empty;
                }

                var lastDot = path.LastIndexOf(".", StringComparison.InvariantCulture);

                return path.Substring(lastDot + 1);
            }

            var extension = Path.GetExtension(path);

            return extension.StartsWith(".") ? extension.Substring(1) : extension;
        }

        public string TrimPathSeparators(string path) => path == "/" ? path : path.TrimEnd('/').TrimEnd('\\');
    }
}