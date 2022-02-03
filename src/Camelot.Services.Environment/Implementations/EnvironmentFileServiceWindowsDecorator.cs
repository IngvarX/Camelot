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

    private static string PreprocessPath(string directory) =>
        directory.EndsWith("\\") ? directory : directory + "\\";
}