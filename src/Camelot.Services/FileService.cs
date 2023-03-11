using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Extensions;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Specifications;
using Camelot.Services.Environment.Interfaces;
using Microsoft.Extensions.Logging;

namespace Camelot.Services;

public class FileService : IFileService
{
    private readonly IPathService _pathService;
    private readonly IEnvironmentFileService _environmentFileService;
    private readonly ILogger _logger;

    public FileService(
        IPathService pathService,
        IEnvironmentFileService environmentFileService,
        ILogger logger)
    {
        _pathService = pathService;
        _environmentFileService = environmentFileService;
        _logger = logger;
    }

    public IReadOnlyList<FileModel> GetFiles(string directory, ISpecification<FileModel> specification = null) =>
        _environmentFileService
            .GetFiles(directory)
            .Select(CreateFrom)
            .WhereNotNull()
            .Where(f => specification?.IsSatisfiedBy(f) ?? true)
            .ToArray();

    public IReadOnlyList<FileModel> GetFiles(IEnumerable<string> files) =>
        files.Select(CreateFrom).WhereNotNull().ToArray();

    public FileModel GetFile(string file) => CreateFrom(file);

    public bool CheckIfExists(string file) => _environmentFileService.CheckIfExists(file);

    public async Task<bool> CopyAsync(string source, string destination, CancellationToken cancellationToken,
        bool overwrite)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (CheckIfExists(destination) && !overwrite)
        {
            return false;
        }

        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            await using var readStream = _environmentFileService.OpenRead(source);
            await using var writeStream = _environmentFileService.OpenWrite(destination);
            await readStream.CopyToAsync(writeStream, cancellationToken);

            CopyMetadata(source, destination, cancellationToken);
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation(
                $"Cancelled file copy {source} to {destination} (overwrite: {overwrite})");

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                $"Failed to copy file {source} to {destination} (overwrite: {overwrite}) with error {ex}");

            return false;
        }

        return true;
    }

    public bool Remove(string file)
    {
        try
        {
            _environmentFileService.Delete(file);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to remove file {file} with error {ex}");

            return false;
        }

        return true;
    }

    public bool Rename(string filePath, string newName)
    {
        var parentDirectory = _pathService.GetParentDirectory(filePath);
        var newFilePath = _pathService.Combine(parentDirectory, newName);

        try
        {
            _environmentFileService.Move(filePath, newFilePath);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to rename file {filePath} to {newName} with error {ex}");

            return false;
        }

        return true;
    }

    public Task WriteTextAsync(string filePath, string text) =>
        _environmentFileService.WriteTextAsync(filePath, text);

    public Task WriteBytesAsync(string filePath, byte[] bytes) =>
        _environmentFileService.WriteBytesAsync(filePath, bytes);

    public void CreateFile(string filePath)
    {
        try
        {
            _environmentFileService.Create(filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to create file {filePath} with error {ex}");
        }
    }

    public Stream OpenRead(string filePath) => _environmentFileService.OpenRead(filePath);

    public Stream OpenWrite(string filePath) => _environmentFileService.OpenWrite(filePath);

    private FileModel CreateFrom(string file)
    {
        try
        {
            var fileInfo = _environmentFileService.GetFile(file);
            var fileModel = new FileModel
            {
                Name = fileInfo.Name,
                FullPath = _pathService.RightTrimPathSeparators(fileInfo.FullName),
                LastModifiedDateTime = fileInfo.LastWriteTime,
                Type = GetFileType(fileInfo),
                SizeBytes = fileInfo.Length,
                Extension = _pathService.GetExtension(fileInfo.Name),
                LastAccessDateTime = fileInfo.LastAccessTime,
                CreatedDateTime = fileInfo.CreationTime
            };

            return fileModel;
        }
        catch
        {
            return null;
        }
    }
    
    private void CopyMetadata(string source, string destination, CancellationToken cancellationToken)
    {
        CopyAttributes(source, destination, cancellationToken);
        CopyDates(source, destination, cancellationToken);
    }

    private void CopyAttributes(string source, string destination, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var sourceAttributes = _environmentFileService.GetAttributes(source);
        _environmentFileService.SetAttributes(destination, sourceAttributes);
    }
    
    private void CopyDates(string source, string destination, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var sourceCreateDate = _environmentFileService.GetCreationTimeUtc(source);
        _environmentFileService.SetCreationTimeUtc(destination, sourceCreateDate);
        
        var sourceLastWriteDate = _environmentFileService.GetLastWriteTimeUtc(source);
        _environmentFileService.SetLastWriteTimeUtc(destination, sourceLastWriteDate);
    }

    private static FileType GetFileType(FileSystemInfo fileInfo) =>
        fileInfo.Attributes.HasFlag(FileAttributes.ReparsePoint) ? FileType.Link : FileType.RegularFile;
}