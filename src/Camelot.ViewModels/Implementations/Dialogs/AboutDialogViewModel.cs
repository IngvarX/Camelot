using System.Windows.Input;
using ApplicationDispatcher.Interfaces;
using Camelot.Services.Interfaces;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.Dialogs
{
    public class AboutDialogViewModel : DialogViewModelBase
    {
        // TODO: to appsettings.json
        private const string RepositoryUrl = "https://github.com/ingvar1995/Camelot";
        
        private readonly IResourceOpeningService _resourceOpeningService;

        public string ApplicationVersion { get; }
        
        public ICommand OpenRepositoryCommand { get; }

        public AboutDialogViewModel(
            IApplicationVersionProvider applicationVersionProvider,
            IResourceOpeningService resourceOpeningService)
        {
            _resourceOpeningService = resourceOpeningService;

            ApplicationVersion = applicationVersionProvider.Version;
            OpenRepositoryCommand = ReactiveCommand.Create(OpenRepository);
        }

        private void OpenRepository()
        {
            _resourceOpeningService.Open(RepositoryUrl);
        }
    }
}