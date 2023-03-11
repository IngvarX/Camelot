using System;
using System.IO;
using System.Threading.Tasks;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services.Environment.Implementations;

public class EnvironmentFileServiceWindowsDecorator : IEnvironmentFileService
{
    private readonly IEnvironmentFileService _environmentFileService;

    public EnvironmentFileServiceWindowsDecorator(IEnvironmentFileService environmentFileService)
    {
        _environmentFileService = environmentFileService;
    }

    public FileInfo GetFile(string file) => _environmentFileService.GetFile(file);

    public string[] GetFiles(string directory) =>
        _environmentFileService.GetFiles(PreprocessPath(directory));

    public bool CheckIfExists(string filePath) => _environmentFileService.CheckIfExists(filePath);

    public void Move(string oldFilePath, string newFilePath) => _environmentFileService.Move(oldFilePath, newFilePath);

    public void Delete(string filePath) => _environmentFileService.Delete(filePath);

    public Task WriteTextAsync(string filePath, string text) => _environmentFileService.WriteTextAsync(filePath, text);

    public Task WriteBytesAsync(string filePath, byte[] bytes) => _environmentFileService.WriteBytesAsync(filePath, bytes);

    public void Create(string filePath) => _environmentFileService.Create(filePath);

    public Stream OpenRead(string filePath) => _environmentFileService.OpenRead(filePath);

    public Stream OpenWrite(string filePath) => _environmentFileService.OpenWrite(filePath);
    
    public FileAttributes GetAttributes(string filePath) => _environmentFileService.GetAttributes(filePath);

    public void SetAttributes(string filePath, FileAttributes attributes) => _environmentFileService.SetAttributes(filePath, attributes);

    public DateTime GetCreationTimeUtc(string filePath) => _environmentFileService.GetCreationTimeUtc(filePath);
    
    public void SetCreationTimeUtc(string filePath, DateTime creationDate) => _environmentFileService.SetCreationTimeUtc(filePath, creationDate);

    public DateTime GetLastWriteTimeUtc(string filePath) => _environmentFileService.GetLastWriteTimeUtc(filePath);

    public void SetLastWriteTimeUtc(string filePath, DateTime lastWriteDate) => _environmentFileService.SetLastWriteTimeUtc(filePath, lastWriteDate);

    private static string PreprocessPath(string directory) =>
        directory.EndsWith("\\") ? directory : directory + "\\";
}