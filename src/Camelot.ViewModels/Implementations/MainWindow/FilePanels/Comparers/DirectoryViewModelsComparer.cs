using System;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels.Comparers
{
    public class DirectoryViewModelsComparer : FileSystemNodesComparerBase<DirectoryViewModel>
    {
        public DirectoryViewModelsComparer(bool isAscending, SortingColumn sortingColumn)
            : base(isAscending, sortingColumn)
        {

        }

        protected override int Compare(DirectoryViewModel x, DirectoryViewModel y, SortingColumn sortingColumn)
        {
            if (x.IsParentDirectory)
            {
                return -1;
            }

            if (y.IsParentDirectory)
            {
                return 1;
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