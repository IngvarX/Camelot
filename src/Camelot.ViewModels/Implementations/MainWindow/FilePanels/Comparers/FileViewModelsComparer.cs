using System;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels.Comparers
{
    public class FileViewModelsComparer : FileSystemNodesComparerBase<FileViewModel>
    {
        public FileViewModelsComparer(bool isAscending, SortingColumn sortingColumn)
            : base(isAscending, sortingColumn)
        {

        }

        protected override int Compare(FileViewModel x, FileViewModel y, SortingColumn sortingColumn,
            bool isAscending)
        {
            var result =  sortingColumn switch
            {
                SortingColumn.Extension => string.Compare(x.Extension, y.Extension, StringComparison.InvariantCulture),
                SortingColumn.Size => x.Size.CompareTo(y.Size),
                SortingColumn.Name => string.Compare(PreprocessFileName(x.Name), PreprocessFileName(y.Name),
                    StringComparison.Ordinal),
                SortingColumn.Date => x.LastModifiedDateTime.CompareTo(y.LastModifiedDateTime),
                _ => throw new ArgumentOutOfRangeException(nameof(SortingColumn))
            };

            return isAscending ? result : -result;
        }
    }
}