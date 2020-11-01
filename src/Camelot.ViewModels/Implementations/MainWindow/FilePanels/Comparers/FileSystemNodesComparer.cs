using System;
using System.Collections.Generic;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels.Comparers
{
    public class FileSystemNodesComparer : IComparer<IFileSystemNodeViewModel>
    {
        private readonly DirectoryViewModelsComparer _directoriesComparer;
        private readonly FileViewModelsComparer _filesComparer;

        public FileSystemNodesComparer(bool isAscending, SortingMode sortingColumn)
        {
            _directoriesComparer = new DirectoryViewModelsComparer(isAscending, sortingColumn);
            _filesComparer = new FileViewModelsComparer(isAscending, sortingColumn);
        }

        public int Compare(IFileSystemNodeViewModel x, IFileSystemNodeViewModel y) =>
            x switch
            {
                FileViewModel fileViewModel => y is DirectoryViewModel
                    ? 1
                    : _filesComparer.Compare(fileViewModel, (FileViewModel) y),
                DirectoryViewModel directoryViewModel => y is FileViewModel
                    ? -1
                    : _directoriesComparer.Compare(directoryViewModel, (DirectoryViewModel) y),
                _ => throw new InvalidOperationException()
            };
    }
}