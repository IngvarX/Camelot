using System;
using Camelot.Services.Abstractions.Models;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels.Comparers
{
    public class DirectoryModelsFileSystemNodesComparer : FileSystemNodesComparerBase<DirectoryModel>
    {
        public DirectoryModelsFileSystemNodesComparer(bool isAscending, SortingColumn sortingColumn)
        : base(isAscending, sortingColumn)
        {
            
        }
        
        protected override int Compare(DirectoryModel x, DirectoryModel y, SortingColumn sortingColumn)
        {
            if (x is null)
            {
                throw new ArgumentNullException(nameof(x));
            }
            
            if (y is null)
            {
                throw new ArgumentNullException(nameof(y));
            }
            
            switch (sortingColumn)
            {
                case SortingColumn.Extension:
                case SortingColumn.Size:
                case SortingColumn.Name:
                    return string.Compare(PreprocessFileName(x.Name), PreprocessFileName(y.Name),
                        StringComparison.Ordinal);
                case SortingColumn.Date:
                    return x.LastModifiedDateTime.CompareTo(y.LastModifiedDateTime);
                default:
                    throw new ArgumentOutOfRangeException(nameof(SortingColumn));
            }
        }
    }
}