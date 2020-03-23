using System.Windows.Input;
using ApplicationDispatcher.Interfaces;
using ReactiveUI;

namespace Camelot.ViewModels.Menu
{
    public class MenuViewModel : ViewModelBase
    {
        public ICommand ExitCommand { get; }

        public MenuViewModel(IApplicationCloser applicationCloser)
        {
            ExitCommand = ReactiveCommand.Create(applicationCloser.CloseApp);
        }
    }
}