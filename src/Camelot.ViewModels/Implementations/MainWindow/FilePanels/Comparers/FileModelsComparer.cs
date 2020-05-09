using System;
using Camelot.Services.Abstractions.Models;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels.Comparers
{
    public class FileModelsFileSystemNodesComparer : FileSystemNodesComparerBase<FileModel>
    {
        public FileModelsFileSystemNodesComparer(bool isAscending, SortingColumn sortingColumn) 
            : base(isAscending, sortingColumn)
        {
            
        }
        
        protected override int Compare(FileModel x, FileModel y, SortingColumn sortingColumn)
        {
            if (x is null)
            {
                throw new ArgumentNullException(nameof(x));
            }
            
            if (y is null)
            {
                throw new ArgumentNullException(nameof(y));
            }

            return sortingColumn switch
            {
                SortingColumn.Extension => string.Compare(x.Extension, y.Extension, StringComparison.InvariantCulture),
                SortingColumn.Size => x.SizeBytes.CompareTo(y.SizeBytes),
                SortingColumn.Name => string.Compare(PreprocessFileName(x.Name), PreprocessFileName(y.Name),
                    StringComparison.Ordinal),
                SortingColumn.Date => x.LastModifiedDateTime.CompareTo(y.LastModifiedDateTime),
                _ => throw new ArgumentOutOfRangeException(nameof(SortingColumn))
            };
        }
    }
}