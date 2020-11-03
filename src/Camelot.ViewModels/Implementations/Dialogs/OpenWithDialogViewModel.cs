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

        public IEnumerable<ApplicationModel> RecommendedApplications => _recommendedApplications;

        [Reactive]
        public string OpenFileExtension { get; set; }

        public OpenWithDialogViewModel(IApplicationService applicationService)
        {
            _applicationService = applicationService;
            _recommendedApplications = new ObservableCollection<ApplicationModel>();
        }

        public override async Task ActivateAsync(OpenWithNavigationParameter parameter)
        {
            OpenFileExtension = parameter.FileExtension;

            _recommendedApplications.AddRange(await _applicationService.GetAssociatedApplications(OpenFileExtension));
        }
    }
}
