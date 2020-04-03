using System.Threading.Tasks;
using System.Windows.Input;
using ApplicationDispatcher.Interfaces;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Interfaces.Menu;
using Camelot.ViewModels.Services.Interfaces;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.Menu
{
    public class MenuViewModel : ViewModelBase, IMenuViewModel
    {
        private readonly IDialogService _dialogService;
        public ICommand ExitCommand { get; }
        
        public ICommand AboutCommand { get; }

        public MenuViewModel(
            IApplicationCloser applicationCloser,
            IDialogService dialogService)
        {
            _dialogService = dialogService;
            
            ExitCommand = ReactiveCommand.Create(applicationCloser.CloseApp);
            AboutCommand = ReactiveCommand.CreateFromTask(ShowAboutDialogAsync);
        }

        private async Task ShowAboutDialogAsync()
        {
            await _dialogService.ShowDialogAsync(nameof(AboutDialogViewModel));
        }
    }
}