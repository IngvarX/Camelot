using System;
using System.Collections.Generic;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Nodes;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Nodes;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels.Comparers;

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
        (x, y) switch
        {
            (FileViewModel a, FileViewModel b) => _filesComparer.Compare(a, b),
            (DirectoryViewModel a, DirectoryViewModel b) => _directoriesComparer.Compare(a, b),
            (FileViewModel _, DirectoryViewModel _) => 1,
            (DirectoryViewModel _, FileViewModel _) => -1,
            _ => throw new InvalidOperationException()
        };
}