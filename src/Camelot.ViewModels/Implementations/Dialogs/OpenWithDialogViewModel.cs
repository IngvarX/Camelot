using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        private readonly ObservableCollection<ApplicationModel> _applications;

        public IEnumerable<ApplicationModel> RecommendedApplications => _recommendedApplications;

        public IEnumerable<ApplicationModel> Applications => _applications;

        [Reactive]
        public string OpenFileExtension { get; set; }

        public OpenWithDialogViewModel(IApplicationService applicationService)
        {
            _applicationService = applicationService;

            _recommendedApplications = new ObservableCollection<ApplicationModel>();
            _applications = new ObservableCollection<ApplicationModel>();
        }

        public override async Task ActivateAsync(OpenWithNavigationParameter parameter)
        {
            OpenFileExtension = parameter.FileExtension;

            _applications.AddRange(await _applicationService.GetAllInstalledApplications());
            _recommendedApplications.AddRange(await _applicationService.GetAssociatedApplications(OpenFileExtension));
        }
    }
}
