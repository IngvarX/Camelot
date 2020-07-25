using System.Collections.Generic;

namespace Camelot.Services.Abstractions
{
    public interface INodesSelectionService
    {
        IReadOnlyList<string> SelectedNodes { get; }

        void SelectNodes(IEnumerable<string> nodes);

        void UnselectNodes(IEnumerable<string> nodes);
    }
}