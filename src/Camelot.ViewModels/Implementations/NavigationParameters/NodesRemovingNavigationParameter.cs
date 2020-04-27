using System.Collections.Generic;
using Camelot.ViewModels.Services;

namespace Camelot.ViewModels.Implementations.NavigationParameters
{
    public class NodesRemovingNavigationParameter : NavigationParameter
    {
        public IReadOnlyCollection<string> Files { get; }
        
        public bool IsRemovingToTrash { get; }

        public NodesRemovingNavigationParameter(IReadOnlyCollection<string> files, bool isRemovingToTrash)
        {
            Files = files;
            IsRemovingToTrash = isRemovingToTrash;
        }
    }
}