using System.Collections.Generic;

namespace Camelot.Services.Abstractions
{
    public interface IFilesSelectionService
    {
        IReadOnlyCollection<string> SelectedFiles { get; }

        void SelectFiles(IEnumerable<string> files);

        void UnselectFiles(IEnumerable<string> files);
    }
}