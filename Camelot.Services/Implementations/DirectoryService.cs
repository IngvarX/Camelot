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

        public bool CreateDirectory(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory))
            {
                return false;
            }

            try
            {
                var fullPath = Path.Combine(SelectedDirectory, directory);
                Directory.CreateDirectory(fullPath);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public IReadOnlyCollection<DirectoryModel> GetDirectories(string directory)
        {
            var directories = Directory
                .GetDirectories(directory)
                .Select(CreateFrom);

            return directories.ToArray();
        }

        public bool DirectoryExists(string directory)
        {
            return Directory.Exists(directory);
        }

        private static DirectoryModel CreateFrom(string directory)
        {
            var directoryInfo = new DirectoryInfo(directory);
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