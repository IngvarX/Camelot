using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Camelot.Services.Interfaces;

namespace Camelot.Services.Implementations
{
    public class PathService : IPathService
    {
        public string GetCommonRootDirectory(IList<string> paths)
        {
            if (paths.Count == 1)
            {
                return GetParentDirectory(paths[0]);
            }

            var length = paths[0].Length;
            for (var i = 1; i < paths.Count; i++)
            {
                length = Math.Min(length, paths[i].Length);
                for (var j = 0; j < length; j++)
                {
                    if (paths[i][j] != paths[0][j])
                    {
                        length = j;
                        break;
                    }
                }
            }

            return paths[0].Substring(0, length);
        }

        public string GetParentDirectory(string path) => Path.GetDirectoryName(path);

        public string Combine(string path1, string path2) => Path.Combine(path1, path2);

        public string GetRelativePath(string relativeTo, string path) => Path.GetRelativePath(relativeTo, path);

        public string GetPathRoot(string path) => Path.GetPathRoot(path);

        public string GetFileNameWithoutExtension(string path)
        {
            if (path.StartsWith("."))
            {
                if (path.Count(c => c == '.') == 1)
                {
                    return path;
                }

                var lastDot = path.LastIndexOf(".", StringComparison.InvariantCulture);

                return path.Substring(0, lastDot);
            }

            return Path.GetFileNameWithoutExtension(path);
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

        public string TrimPathSeparators(string path)
        {
            return path == "/" ? path : path.TrimEnd('/').TrimEnd('\\');
        }
    }
}