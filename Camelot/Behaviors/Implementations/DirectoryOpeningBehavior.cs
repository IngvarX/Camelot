using Camelot.Behaviors.Interfaces;
using Camelot.Providers.Interfaces;

namespace Camelot.Behaviors.Implementations
{
    public class DirectoryOpeningBehavior : IFileOpeningBehavior
    {
        private readonly IMainWindowViewModelProvider _mainWindowViewModelProvider;

        public DirectoryOpeningBehavior(IMainWindowViewModelProvider mainWindowViewModelProvider)
        {
            _mainWindowViewModelProvider = mainWindowViewModelProvider;
        }

        public void Open(string file)
        {
            var mainWindowViewModel = _mainWindowViewModelProvider.Get();

            mainWindowViewModel.ActiveFilesPanelViewModel.CurrentDirectory = file;
        }
    }
}