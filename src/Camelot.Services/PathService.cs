using System;
using System.Collections.Generic;
using System.Linq;
using Camelot.Services.Abstractions;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services
{
    public class PathService : IPathService
    {
        private readonly IEnvironmentPathService _environmentPathService;

        public PathService(IEnvironmentPathService environmentPathService)
        {
            _environmentPathService = environmentPathService;
        }

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

        public string GetParentDirectory(string path) => _environmentPathService.GetDirectoryName(path);

        public string Combine(string path1, string path2) => _environmentPathService.Combine(path1, path2);

        public string GetRelativePath(string relativeTo, string path) =>
            _environmentPathService.GetRelativePath(relativeTo, path);

        public string GetPathRoot(string path) => _environmentPathService.GetPathRoot(path);

        public string GetFileNameWithoutExtension(string path)
        {
            if (!path.StartsWith("."))
            {
                return _environmentPathService.GetFileNameWithoutExtension(path);
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
            var fileName = _environmentPathService.GetFileName(path);

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

            var extension = _environmentPathService.GetExtension(path);

            return extension.StartsWith(".") ? extension.Substring(1) : extension;
        }
        
        public string TrimPathSeparators(string path) => path == "/" ? path : path.TrimEnd('/').TrimEnd('\\');
    }
}