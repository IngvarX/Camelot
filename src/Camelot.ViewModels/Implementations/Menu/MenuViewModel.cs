using System.Windows.Input;
using ApplicationDispatcher.Interfaces;
using Camelot.ViewModels.Interfaces.Menu;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.Menu
{
    public class MenuViewModel : ViewModelBase, IMenuViewModel
    {
        public ICommand ExitCommand { get; }

        public MenuViewModel(IApplicationCloser applicationCloser)
        {
            ExitCommand = ReactiveCommand.Create(applicationCloser.CloseApp);
        }
    }
}