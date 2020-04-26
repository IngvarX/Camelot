using Camelot.ViewModels.Services;

namespace Camelot.ViewModels.Implementations.NavigationParameters
{
    public class NodesRemovingNavigationParameter : NavigationParameter
    {
        public string[] Files { get; }

        public NodesRemovingNavigationParameter(string[] files)
        {
            Files = files;
        }
    }
}