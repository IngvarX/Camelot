using System;
using System.Linq;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels.Comparers
{
    public class DirectoryViewModelsComparer : FileSystemNodesComparerBase<DirectoryViewModel>
    {
        public DirectoryViewModelsComparer(bool isAscending, SortingColumn sortingColumn)
            : base(isAscending, sortingColumn)
        {

        }

        protected override int Compare(DirectoryViewModel x, DirectoryViewModel y, SortingColumn sortingColumn,
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

            var sortingByNameColumns = new[] {SortingColumn.Extension, SortingColumn.Size, SortingColumn.Name};
            var result = sortingColumn switch
            {
                _ when sortingByNameColumns.Contains(sortingColumn) =>
                    string.Compare(PreprocessFileName(x.Name), PreprocessFileName(y.Name), StringComparison.Ordinal),
                SortingColumn.Date => x.LastModifiedDateTime.CompareTo(y.LastModifiedDateTime),
                _ => throw new ArgumentOutOfRangeException(nameof(SortingColumn))
            };

            return isAscending ? result : -result;
        }
    }
}