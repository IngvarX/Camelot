using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using DynamicData;
using ReactiveUI.Fody.Helpers;

namespace Camelot.ViewModels.Implementations.Dialogs
{
    public class OpenWithDialogViewModel : ParameterizedDialogViewModelBase<OpenWithNavigationParameter>
    {
        private readonly IApplicationService _applicationService;

        private readonly ObservableCollection<ApplicationModel> _recommendedApplications;

        private readonly ObservableCollection<ApplicationModel> _installedApplications;

        public IEnumerable<ApplicationModel> RecommendedApplications => _recommendedApplications;

        public IEnumerable<ApplicationModel> InstalledApplications => _installedApplications;

        [Reactive]
        public ApplicationModel UsedApplication { get; set; }

        [Reactive]
        public string OpenFileExtension { get; set; }

        public ICommand CancelCommand { get; }

        public ICommand SelectCommand { get; }

        public OpenWithDialogViewModel(IApplicationService applicationService)
        {
            _applicationService = applicationService;

            _recommendedApplications = new ObservableCollection<ApplicationModel>();
            _installedApplications = new ObservableCollection<ApplicationModel>();

            CancelCommand = ReactiveCommand.Create(Close);
            SelectCommand = ReactiveCommand.CreateFromTask(SelectApplicationAsync);
        }

        public override async Task ActivateAsync(OpenWithNavigationParameter parameter)
        {
            OpenFileExtension = parameter.FileExtension;

            _installedApplications.AddRange(await _applicationService.GetInstalledApplications());
            _recommendedApplications.AddRange(await _applicationService.GetAssociatedApplications(OpenFileExtension));

            UsedApplication = _recommendedApplications.FirstOrDefault();
        }

        private async Task SelectApplicationAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            Close();
        }
    }
}
