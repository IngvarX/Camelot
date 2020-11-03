using System;
using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels.Comparers
{
    public class FileViewModelsComparer : FileSystemNodesComparerBase<FileViewModel>
    {
        public FileViewModelsComparer(bool isAscending, SortingMode sortingColumn)
            : base(isAscending, sortingColumn)
        {

        }

        protected override int Compare(FileViewModel x, FileViewModel y, SortingMode sortingColumn,
            bool isAscending)
        {
            var result =  sortingColumn switch
            {
                SortingMode.Extension => string.Compare(x.Extension, y.Extension, StringComparison.InvariantCulture),
                SortingMode.Size => x.Size.CompareTo(y.Size),
                SortingMode.Name => string.Compare(PreprocessFileName(x.Name), PreprocessFileName(y.Name),
                    StringComparison.Ordinal),
                SortingMode.Date => x.LastModifiedDateTime.CompareTo(y.LastModifiedDateTime),
                _ => throw new ArgumentOutOfRangeException(nameof(sortingColumn), sortingColumn, null)
            };

            return isAscending ? result : -result;
        }
    }
}