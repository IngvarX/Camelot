using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Camelot.Extensions;
using Camelot.Services.EventArgs;
using Camelot.Services.Interfaces;
using Camelot.Services.Models;

namespace Camelot.Services.Implementations
{
    public class DirectoryService : IDirectoryService
    {
        private readonly IPathService _pathService;

        private string _directory;

        public string SelectedDirectory
        {
            get => _directory;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _directory = value;

                var args = new SelectedDirectoryChangedEventArgs(_directory);
                SelectedDirectoryChanged.Raise(this, args);
            }
        }

        public event EventHandler<SelectedDirectoryChangedEventArgs> SelectedDirectoryChanged;

        public DirectoryService(IPathService pathService)
        {
            _pathService = pathService;
        }

        public bool Create(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory))
            {
                return false;
            }

            try
            {
                Directory.CreateDirectory(directory);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public DirectoryModel GetParentDirectory(string directory)
        {
            var parentDirectory = new DirectoryInfo(directory).Parent;

            return parentDirectory is null ? null : CreateFrom(parentDirectory);
        }

        public IReadOnlyCollection<DirectoryModel> GetDirectories(string directory)
        {
            var directories = Directory
                .GetDirectories(directory)
                .Select(CreateFrom);

            return directories.ToArray();
        }

        public bool CheckIfExists(string directory)
        {
            return Directory.Exists(directory);
        }

        public string GetAppRootDirectory()
        {
            return _pathService.GetPathRoot(Directory.GetCurrentDirectory());
        }

        public IReadOnlyCollection<string> GetFilesRecursively(string directory)
        {
            return Directory
                .EnumerateFiles(directory, "*.*", SearchOption.AllDirectories)
                .ToArray();
        }

        public void RemoveRecursively(string directory)
        {
            Directory.Delete(directory, true);
        }

        public void Rename(string directoryPath, string newName)
        {
            var parentDirectory = _pathService.GetParentDirectory(directoryPath);
            var newDirectoryPath = _pathService.Combine(parentDirectory, newName);
            if (directoryPath == newDirectoryPath)
            {
                return;
            }
            
            Directory.Move(directoryPath, newDirectoryPath);
        }

        private static DirectoryModel CreateFrom(string directory)
        {
            var directoryInfo = new DirectoryInfo(directory);

            return CreateFrom(directoryInfo);
        }

        private static DirectoryModel CreateFrom(FileSystemInfo directoryInfo)
        {
            var directoryModel = new DirectoryModel
            {
                Name = directoryInfo.Name,
                FullPath = directoryInfo.FullName,
                LastModifiedDateTime = directoryInfo.LastWriteTime
            };

            return directoryModel;
        }
    }
}