using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Camelot.ViewModels.Implementations.Dialogs
{
    public class OpenWithDialogViewModel : ParameterizedDialogViewModelBase<OpenWithNavigationParameter>
    {
        private readonly IApplicationService _applicationService;

        private readonly ObservableCollection<ApplicationModel> _recommendedApplications;

        private readonly ObservableCollection<ApplicationModel> _otherApplications;

        public IEnumerable<ApplicationModel> RecommendedApplications => _recommendedApplications;

        public IEnumerable<ApplicationModel> OtherApplications => _otherApplications;

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
            _otherApplications = new ObservableCollection<ApplicationModel>();

            CancelCommand = ReactiveCommand.Create(Close);
            SelectCommand = ReactiveCommand.CreateFromTask(SelectApplicationAsync);
        }

        public override async Task ActivateAsync(OpenWithNavigationParameter parameter)
        {
            OpenFileExtension = parameter.FileExtension;

            _recommendedApplications.AddRange(await _applicationService.GetAssociatedApplications(OpenFileExtension));
            _otherApplications.AddRange(await _applicationService.GetInstalledApplications());

            UsedApplication = _recommendedApplications.FirstOrDefault();
        }

        private async Task SelectApplicationAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            Close();
        }
    }
}
