namespace Camelot.Services.Environment.Interfaces
{
    public interface IEnvironmentPathService
    {
        string GetDirectoryName(string path);

        string Combine(string path1, string path2);

        string GetRelativePath(string relativeTo, string path);

        string GetPathRoot(string path);

        string GetFileNameWithoutExtension(string path);

        string GetFileName(string path);

        string GetExtension(string path);
    }
}