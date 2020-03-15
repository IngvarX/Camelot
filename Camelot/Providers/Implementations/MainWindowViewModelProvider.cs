using Camelot.Providers.Interfaces;
using Camelot.ViewModels;
using Splat;

namespace Camelot.Providers.Implementations
{
    public class MainWindowViewModelProvider : IMainWindowViewModelProvider
    {
        public MainWindowViewModel Get()
        {
            return Locator.Current.GetService<MainWindowViewModel>();
        }
    }
}