using System;
using System.Linq;
using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels.Comparers
{
    public class DirectoryViewModelsComparer : FileSystemNodesComparerBase<DirectoryViewModel>
    {
        public DirectoryViewModelsComparer(bool isAscending, SortingMode sortingColumn)
            : base(isAscending, sortingColumn)
        {

        }

        protected override int Compare(DirectoryViewModel x, DirectoryViewModel y, SortingMode sortingColumn,
            bool isAscending)
        {
            if (x.IsParentDirectory)
            {
                return -1;
            }

            if (y.IsParentDirectory)
            {
                return 1;
            }

            var sortingByNameColumns = new[] {SortingMode.Extension, SortingMode.Size, SortingMode.Name};
            var result = sortingColumn switch
            {
                _ when sortingByNameColumns.Contains(sortingColumn) =>
                    string.Compare(PreprocessFileName(x.Name), PreprocessFileName(y.Name), StringComparison.Ordinal),
                SortingMode.Date => x.LastModifiedDateTime.CompareTo(y.LastModifiedDateTime),
                _ => throw new ArgumentOutOfRangeException(nameof(sortingColumn), sortingColumn, null)
            };

            return isAscending ? result : -result;
        }
    }
}