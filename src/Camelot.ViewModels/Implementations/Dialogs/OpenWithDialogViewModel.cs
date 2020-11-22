﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Implementations.Dialogs.Comparers;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Implementations.Dialogs.Results;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Camelot.ViewModels.Implementations.Dialogs
{
    public class OpenWithDialogViewModel : ParameterizedDialogViewModelBaseAsync<OpenWithDialogResult, OpenWithNavigationParameter>
    {
        private readonly IApplicationService _applicationService;

        private readonly ObservableCollection<ApplicationModel> _recommendedApplications;
        private readonly ObservableCollection<ApplicationModel> _otherApplications;

        public IEnumerable<ApplicationModel> RecommendedApplications => _recommendedApplications;

        public IEnumerable<ApplicationModel> OtherApplications => _otherApplications.Where(Filter);

        [Reactive]
        public ApplicationModel UsedApplication { get; set; }

        [Reactive]
        public string OpenFileExtension { get; private set; }

        [Reactive]
        public string ApplicationName { get; set; }

        [Reactive]
        public bool IsDefaultApplication { get; set; }

        public ICommand CancelCommand { get; }

        public ICommand SelectCommand { get; }

        public OpenWithDialogViewModel(IApplicationService applicationService)
        {
            _applicationService = applicationService;

            _recommendedApplications = new ObservableCollection<ApplicationModel>();
            _otherApplications = new ObservableCollection<ApplicationModel>();

            CancelCommand = ReactiveCommand.Create(Close);
            SelectCommand = ReactiveCommand.Create(SelectApplication);

            this
                .WhenAnyValue(vm => vm.ApplicationName)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(OtherApplications)));
        }

        public override async Task ActivateAsync(OpenWithNavigationParameter parameter,
            CancellationToken cancellationToken = default)
        {
            OpenFileExtension = parameter.FileExtension;

            var associatedApps = await _applicationService.GetAssociatedApplications(OpenFileExtension);
            _recommendedApplications.AddRange(associatedApps);

            var installedApps = await _applicationService.GetInstalledApplications();
            var comparer = GetAppsComparer();
            var otherApps = installedApps
                .Except(_recommendedApplications, comparer)
                .OrderBy(a => a.DisplayName);
            _otherApplications.AddRange(otherApps);

            ApplicationModel selectedApplication;

            if (parameter.Application == null)
            {
                selectedApplication = _recommendedApplications.FirstOrDefault();
            }
            else
            {
                selectedApplication = FindApplication(_recommendedApplications, parameter.Application);
                if (selectedApplication == null)
                {
                    selectedApplication = FindApplication(_otherApplications, parameter.Application);
                    if (selectedApplication != null)
                    {
                        _otherApplications.Remove(selectedApplication);
                    }
                }
                else
                {
                    _recommendedApplications.Remove(selectedApplication);
                }

                _recommendedApplications.Insert(0, selectedApplication);
            }

            UsedApplication = selectedApplication;

            static ApplicationModel FindApplication(IEnumerable<ApplicationModel> applications, ApplicationModel application) =>
                applications.FirstOrDefault(m => m.DisplayName == application.DisplayName);
        }

        private bool Filter(ApplicationModel model) =>
            string.IsNullOrEmpty(ApplicationName)
            || model.DisplayName.Contains(ApplicationName, StringComparison.InvariantCultureIgnoreCase);

        private static IEqualityComparer<ApplicationModel> GetAppsComparer() =>
            new ApplicationModelComparer();

        private void SelectApplication() =>
            Close(new OpenWithDialogResult(OpenFileExtension, UsedApplication, IsDefaultApplication));
    }
}