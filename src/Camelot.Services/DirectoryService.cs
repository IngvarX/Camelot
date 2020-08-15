using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Camelot.Extensions;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Specifications;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services
{
    public class DirectoryService : IDirectoryService
    {
        private readonly IPathService _pathService;
        private readonly IEnvironmentDirectoryService _environmentDirectoryService;
        private readonly IEnvironmentFileService _environmentFileService;

        private string _directory;

        public string SelectedDirectory
        {
            get => _directory;
            set
            {
                if (_directory == value)
                {
                    return;
                }

                _directory = value;

                var args = new SelectedDirectoryChangedEventArgs(_directory);
                SelectedDirectoryChanged.Raise(this, args);
            }
        }

        public event EventHandler<SelectedDirectoryChangedEventArgs> SelectedDirectoryChanged;

        public DirectoryService(
            IPathService pathService,
            IEnvironmentDirectoryService environmentDirectoryService,
            IEnvironmentFileService environmentFileService)
        {
            _pathService = pathService;
            _environmentDirectoryService = environmentDirectoryService;
            _environmentFileService = environmentFileService;
        }

        public bool Create(string directory)
        {
            try
            {
                _environmentDirectoryService.CreateDirectory(directory);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public long CalculateSize(string directory) =>
            _environmentDirectoryService
                .EnumerateFilesRecursively(directory)
                .Sum(GetFileSize);

        public DirectoryModel GetDirectory(string directory) => CreateFrom(directory);

        public DirectoryModel GetParentDirectory(string directory)
        {
            var parentDirectory = _environmentDirectoryService.GetDirectory(directory).Parent;

            return parentDirectory is null ? null : CreateFrom(parentDirectory);
        }

        public IReadOnlyList<DirectoryModel> GetChildDirectories(string directory, ISpecification<DirectoryModel> specification) =>
            _environmentDirectoryService
                .GetDirectories(directory)
                .Select(CreateFrom)
                .Where(specification.IsSatisfiedBy)
                .ToArray();

        public IReadOnlyList<string> GetEmptyDirectoriesRecursively(string directory)
        {
            if (CheckIfEmpty(directory))
            {
                return new[] {directory};
            }

            var directories = GetDirectoriesRecursively(directory);

            return directories.Where(CheckIfEmpty).ToArray();
        }

        public bool CheckIfExists(string directory) =>
            _environmentDirectoryService.CheckIfExists(directory);

        public string GetAppRootDirectory() =>
            _pathService.GetPathRoot(_environmentDirectoryService.GetCurrentDirectory());

        public IEnumerable<string> GetFilesRecursively(string directory) =>
            _environmentDirectoryService
                .EnumerateFilesRecursively(directory)
                .ToArray();

        public IEnumerable<string> GetDirectoriesRecursively(string directory) =>
            _environmentDirectoryService
                .EnumerateDirectoriesRecursively(directory)
                .ToArray();

        public void RemoveRecursively(string directory) =>
            _environmentDirectoryService.Delete(directory, true);

        public bool Rename(string directoryPath, string newName)
        {
            var parentDirectory = _pathService.GetParentDirectory(directoryPath);
            var newDirectoryPath = _pathService.Combine(parentDirectory, newName);

            try
            {
                _environmentDirectoryService.Move(directoryPath, newDirectoryPath);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private bool CheckIfEmpty(string directory) =>
            !_environmentDirectoryService.EnumerateFileSystemEntriesRecursively(directory).Any();

        private DirectoryModel CreateFrom(string directory)
        {
            var directoryInfo = _environmentDirectoryService.GetDirectory(directory);

            return CreateFrom(directoryInfo);
        }

        private long GetFileSize(string file) =>
            _environmentFileService.GetFile(file).Length;

        private static DirectoryModel CreateFrom(FileSystemInfo directoryInfo) =>
            new DirectoryModel
            {
                Name = directoryInfo.Name,
                FullPath = directoryInfo.FullName,
                LastModifiedDateTime = directoryInfo.LastWriteTime,
                LastAccessDateTime = directoryInfo.LastAccessTime,
                CreatedDateTime = directoryInfo.CreationTime
            };
    }
}