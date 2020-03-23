using System;
using System.Collections.Generic;
using System.IO;
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

        private static string GetParentDirectory(string path)
        {
            return Path.GetDirectoryName(path);
        }

        public string Combine(string path1, string path2)
        {
            return Path.Combine(path1, path2);
        }

        public string GetRelativePath(string relativeTo, string path)
        {
            return Path.GetRelativePath(relativeTo, path);
        }
    }
}