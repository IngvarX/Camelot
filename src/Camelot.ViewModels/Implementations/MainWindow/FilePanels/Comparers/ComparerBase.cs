using System.Collections.Generic;
using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels.Comparers
{
    public abstract class FileSystemNodesComparerBase<T> : IComparer<T>
    {
        private readonly bool _isAscending;
        private readonly SortingMode _sortingColumn;

        protected FileSystemNodesComparerBase(bool isAscending, SortingMode sortingColumn)
        {
            _isAscending = isAscending;
            _sortingColumn = sortingColumn;
        }

        public int Compare(T x, T y) => Compare(x, y, _sortingColumn, _isAscending);

        protected abstract int Compare(T x, T y, SortingMode sortingColumn, bool isAscending);

        protected static string PreprocessFileName(string fileName) =>
            fileName.StartsWith(".") ? fileName.Substring(1) : fileName;
    }
}