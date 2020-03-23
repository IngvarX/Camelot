using System.Collections.Generic;

namespace Camelot.Services.Interfaces
{
    public interface IPathService
    {
        string GetCommonRootDirectory(IList<string> paths);

        string Combine(string path1, string path2);

        string GetRelativePath(string relativeTo, string path);
    }
}