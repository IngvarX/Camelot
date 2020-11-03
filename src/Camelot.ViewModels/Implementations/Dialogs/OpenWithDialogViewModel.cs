using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using ReactiveUI.Fody.Helpers;

namespace Camelot.ViewModels.Implementations.Dialogs
{
    public class OpenWithDialogViewModel : ParameterizedDialogViewModelBase<OpenWithNavigationParameter>
    {
        private readonly IApplicationService _applicationService;

        public IEnumerable<ApplicationModel> Applications { get; private set; }

        [Reactive]
        public string OpenFileExtension { get; set; }

        public OpenWithDialogViewModel(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        public override async Task ActivateAsync(OpenWithNavigationParameter parameter)
        {
            OpenFileExtension = parameter.FileExtension;
            Applications = new ObservableCollection<ApplicationModel>(await _applicationService.GetAssociatedApplications(OpenFileExtension));
        }
    }
}
