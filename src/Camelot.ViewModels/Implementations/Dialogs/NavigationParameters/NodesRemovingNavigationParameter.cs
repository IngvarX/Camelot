using System.Collections.Generic;
using Camelot.ViewModels.Services;

namespace Camelot.ViewModels.Implementations.Dialogs.NavigationParameters
{
    public class NodesRemovingNavigationParameter : NavigationParameterBase
    {
        public IReadOnlyList<string> Files { get; }

        public bool IsRemovingToTrash { get; }

        public NodesRemovingNavigationParameter(IReadOnlyList<string> files, bool isRemovingToTrash = true)
        {
            Files = files;
            IsRemovingToTrash = isRemovingToTrash;
        }
    }
}