using System.Collections.Generic;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels.Comparers
{
    public abstract class FileSystemNodesComparerBase<T> : IComparer<T>
    {
        private readonly bool _isAscending;
        private readonly SortingColumn _sortingColumn;

        protected FileSystemNodesComparerBase(bool isAscending, SortingColumn sortingColumn)
        {
            _isAscending = isAscending;
            _sortingColumn = sortingColumn;
        }
        
        public int Compare(T x, T y)
        {
            var compareResult = Compare(x, y, _sortingColumn);

            return _isAscending ? compareResult : -compareResult;
        }

        protected abstract int Compare(T x, T y, SortingColumn sortingColumn);
        
        protected static string PreprocessFileName(string fileName) =>
            fileName.StartsWith(".") ? fileName.Substring(1) : fileName;
    }
}