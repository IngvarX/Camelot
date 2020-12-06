using System.Collections.Generic;

namespace Camelot.Services.Abstractions
{
    public interface IPathService
    {
        string GetCommonRootDirectory(IReadOnlyList<string> paths);

        string Combine(string path1, string path2);

        string GetRelativePath(string relativeTo, string path);

        string GetParentDirectory(string path);

        string GetPathRoot(string path);

        string GetFileNameWithoutExtension(string path);

        string GetFileName(string path);

        string GetExtension(string path);
        
        string TrimPathSeparators(string path);
    }
}