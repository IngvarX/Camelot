using System.Windows.Input;
using Camelot.Avalonia.Interfaces;
using Camelot.Services.Abstractions;
using Camelot.ViewModels.Configuration;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.Dialogs
{
    public class AboutDialogViewModel : DialogViewModelBase
    {
        private readonly IResourceOpeningService _resourceOpeningService;
        private readonly AboutDialogConfiguration _aboutDialogConfiguration;

        public string ApplicationVersion { get; }

        public string Maintainers => string.Join(", ", _aboutDialogConfiguration.Maintainers);

        public ICommand OpenRepositoryCommand { get; }

        public AboutDialogViewModel(
            IApplicationVersionProvider applicationVersionProvider,
            IResourceOpeningService resourceOpeningService,
            AboutDialogConfiguration aboutDialogConfiguration)
        {
            _resourceOpeningService = resourceOpeningService;
            _aboutDialogConfiguration = aboutDialogConfiguration;

            ApplicationVersion = applicationVersionProvider.Version;
            OpenRepositoryCommand = ReactiveCommand.Create(OpenRepository);
        }

        private void OpenRepository() => _resourceOpeningService.Open(_aboutDialogConfiguration.RepositoryUrl);
    }
}