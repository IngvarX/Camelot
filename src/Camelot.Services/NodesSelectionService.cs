using System.Collections.Generic;
using System.Linq;
using Camelot.Extensions;
using Camelot.Services.Abstractions;

namespace Camelot.Services
{
    public class NodesSelectionService : INodesSelectionService
    {
        private readonly HashSet<string> _selectedNodes;

        public IReadOnlyList<string> SelectedNodes => _selectedNodes.ToArray();

        public NodesSelectionService()
        {
            _selectedNodes = new HashSet<string>();
        }

        public void SelectNodes(IEnumerable<string> nodes) => nodes.ForEach(f => _selectedNodes.Add(f));

        public void UnselectNodes(IEnumerable<string> nodes) => nodes.ForEach(f => _selectedNodes.Remove(f));
    }
}