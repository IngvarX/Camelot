using System;
using System.IO;
using System.Threading.Tasks;

namespace Camelot.Services.Environment.Interfaces;

public interface IEnvironmentFileService
{
    FileInfo GetFile(string file);

    string[] GetFiles(string directory);

    bool CheckIfExists(string filePath);

    void Move(string oldFilePath, string newFilePath);

    void Delete(string filePath);

    Task WriteTextAsync(string filePath, string text);

    Task WriteBytesAsync(string filePath, byte[] bytes);

    void Create(string filePath);

    Stream OpenRead(string filePath);

    Stream OpenWrite(string filePath);

    FileAttributes GetAttributes(string filePath);

    void SetAttributes(string filePath, FileAttributes attributes);

    DateTime GetCreationTimeUtc(string filePath);
    
    void SetCreationTimeUtc(string filePath, DateTime creationDate);
    
    DateTime GetLastWriteTimeUtc(string filePath);
    
    void SetLastWriteTimeUtc(string filePath, DateTime lastWriteDate);
}