using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.Dialogs
{
    public class OpenWithDialogViewModelTests
    {
        private const string FileExtension = "pdf";

        private readonly AutoMocker _autoMocker;

        public OpenWithDialogViewModelTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Fact]
        public async Task TestDefaultProperties()
        {
            var dialog = _autoMocker.CreateInstance<OpenWithDialogViewModel>();
            var parameter = new OpenWithNavigationParameter(FileExtension, null);
            await dialog.ActivateAsync(parameter);

            Assert.Null(dialog.ApplicationName);
            Assert.False(dialog.IsDefaultApplication);
            Assert.Equal(parameter.FileExtension, dialog.OpenFileExtension);
            Assert.Null(dialog.SelectedDefaultApplication);
            Assert.Null(dialog.SelectedOtherApplication);
            Assert.Empty(dialog.RecommendedApplications);
            Assert.Empty(dialog.OtherApplications);
            Assert.False(dialog.IsDefaultApplication);
            Assert.False(dialog.SelectCommand.CanExecute(null));
        }

        [Fact]
        public async Task TestRecommendedApp()
        {
            var application = new ApplicationModel
            {
                DisplayName = "App"
            };
            _autoMocker
                .Setup<IApplicationService, Task<IEnumerable<ApplicationModel>>>(m => m.GetAssociatedApplicationsAsync(FileExtension))
                .ReturnsAsync(Enumerable.Repeat(application, 1));
            _autoMocker
                .Setup<IApplicationService, Task<IEnumerable<ApplicationModel>>>(m => m.GetInstalledApplicationsAsync())
                .ReturnsAsync(Enumerable.Repeat(application, 1));

            var dialog = _autoMocker.CreateInstance<OpenWithDialogViewModel>();
            var parameter = new OpenWithNavigationParameter(FileExtension, null);
            await dialog.ActivateAsync(parameter);

            Assert.Equal(application, dialog.SelectedDefaultApplication);
            Assert.Single(dialog.RecommendedApplications);
            Assert.Equal(application, dialog.RecommendedApplications.Single());
            Assert.Empty(dialog.OtherApplications);
        }

        [Fact]
        public async Task TestSelectedAndMultipleRecommendedApps()
        {
            var selectedApplication = new ApplicationModel
            {
                DisplayName = "App"
            };
            var recommendedApplication = new ApplicationModel
            {
                DisplayName = "AnotherApp"
            };
            var otherApplication = new ApplicationModel
            {
                DisplayName = "OneMoreApp"
            };
            _autoMocker
                .Setup<IApplicationService, Task<IEnumerable<ApplicationModel>>>(m => m.GetAssociatedApplicationsAsync(FileExtension))
                .ReturnsAsync(new[] {recommendedApplication, selectedApplication});
            _autoMocker
                .Setup<IApplicationService, Task<IEnumerable<ApplicationModel>>>(m => m.GetInstalledApplicationsAsync())
                .ReturnsAsync(Enumerable.Repeat(otherApplication, 1));

            var dialog = _autoMocker.CreateInstance<OpenWithDialogViewModel>();
            var parameter = new OpenWithNavigationParameter(FileExtension, selectedApplication);
            await dialog.ActivateAsync(parameter);

            Assert.Equal(selectedApplication, dialog.SelectedDefaultApplication);
            Assert.Equal(2, dialog.RecommendedApplications.Count());
            Assert.Equal(selectedApplication, dialog.RecommendedApplications.First());
            Assert.Equal(recommendedApplication, dialog.RecommendedApplications.Last());
            Assert.Single(dialog.OtherApplications);
            Assert.Equal(otherApplication, dialog.OtherApplications.Single());
        }

        [Fact]
        public async Task TestSelectedInOtherAppsList()
        {
            var selectedApplication = new ApplicationModel
            {
                DisplayName = "App"
            };
            var recommendedApplication = new ApplicationModel
            {
                DisplayName = "AnotherApp"
            };
            var otherApplication = new ApplicationModel
            {
                DisplayName = "OneMoreApp"
            };
            _autoMocker
                .Setup<IApplicationService, Task<IEnumerable<ApplicationModel>>>(m => m.GetAssociatedApplicationsAsync(FileExtension))
                .ReturnsAsync(new[] {recommendedApplication});
            _autoMocker
                .Setup<IApplicationService, Task<IEnumerable<ApplicationModel>>>(m => m.GetInstalledApplicationsAsync())
                .ReturnsAsync(new[] {recommendedApplication, selectedApplication, otherApplication});

            var dialog = _autoMocker.CreateInstance<OpenWithDialogViewModel>();
            var parameter = new OpenWithNavigationParameter(FileExtension, selectedApplication);
            await dialog.ActivateAsync(parameter);

            Assert.Equal(selectedApplication, dialog.SelectedDefaultApplication);
            Assert.Equal(2, dialog.RecommendedApplications.Count());
            Assert.Equal(selectedApplication, dialog.RecommendedApplications.First());
            Assert.Equal(recommendedApplication, dialog.RecommendedApplications.Last());
            Assert.Single(dialog.OtherApplications);
            Assert.Equal(otherApplication, dialog.OtherApplications.Single());
        }

        [Fact]
        public async Task TestCancelCommand()
        {
            var isCallbackCalled = false;
            var dialog = _autoMocker.CreateInstance<OpenWithDialogViewModel>();
            var parameter = new OpenWithNavigationParameter(FileExtension, null);
            await dialog.ActivateAsync(parameter);

            dialog.CloseRequested += (sender, args) =>
            {
                var result = args.Result;
                if (result is null)
                {
                    isCallbackCalled = true;
                }
            };

            dialog.CancelCommand.Execute(null);

            Assert.True(isCallbackCalled);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TestSelectCommand(bool isDefaultApplication)
        {
            var selectedApplication = new ApplicationModel
            {
                DisplayName = "App"
            };
            _autoMocker
                .Setup<IApplicationService, Task<IEnumerable<ApplicationModel>>>(m => m.GetAssociatedApplicationsAsync(FileExtension))
                .ReturnsAsync(new[] {selectedApplication});

            var isCallbackCalled = false;
            var dialog = _autoMocker.CreateInstance<OpenWithDialogViewModel>();
            var parameter = new OpenWithNavigationParameter(FileExtension, null);
            await dialog.ActivateAsync(parameter);

            dialog.CloseRequested += (sender, args) =>
            {
                var result = args.Result;

                isCallbackCalled = result != null
                                   && result.FileExtension == FileExtension
                                   && result.IsDefaultApplication == isDefaultApplication
                                   && result.Application == selectedApplication;
            };

            dialog.IsDefaultApplication = isDefaultApplication;
            dialog.SelectedDefaultApplication = dialog.RecommendedApplications.First();
            dialog.SelectCommand.Execute(null);

            Assert.True(isCallbackCalled);
        }

        [Theory]
        [InlineData("app", 3)]
        [InlineData("aPP", 3)]
        [InlineData("another", 1)]
        [InlineData("an1", 0)]
        [InlineData("O", 2)]
        [InlineData("o", 2)]
        [InlineData(null, 3)]
        [InlineData("", 3)]
        public async Task TestSearch(string searchString, int count)
        {
            var appsNames = new[] {"App", "AnotherApp", "OneMoreApp"};
            var apps = appsNames.Select(n => new ApplicationModel
            {
                DisplayName = n
            });
            _autoMocker
                .Setup<IApplicationService, Task<IEnumerable<ApplicationModel>>>(m => m.GetInstalledApplicationsAsync())
                .ReturnsAsync(apps);

            var dialog = _autoMocker.CreateInstance<OpenWithDialogViewModel>();
            var parameter = new OpenWithNavigationParameter(FileExtension, null);
            await dialog.ActivateAsync(parameter);

            Assert.Null(dialog.ApplicationName);
            dialog.ApplicationName = searchString;
            Assert.Equal(searchString, dialog.ApplicationName);

            await Task.Delay(100);
            Assert.Equal(count, dialog.OtherApplications.Count());
        }

        [Fact]
        public async Task TestSelectCommandCanBeExecuted()
        {
            var appsNames = new[] {"App", "AnotherApp", "OneMoreApp"};
            var apps = appsNames.Select(n => new ApplicationModel
            {
                DisplayName = n
            });
            _autoMocker
                .Setup<IApplicationService, Task<IEnumerable<ApplicationModel>>>(m => m.GetInstalledApplicationsAsync())
                .ReturnsAsync(apps);
            _autoMocker
                .Setup<IApplicationService, Task<IEnumerable<ApplicationModel>>>(m => m.GetAssociatedApplicationsAsync(FileExtension))
                .ReturnsAsync(new[] {apps.First()});

            var dialog = _autoMocker.CreateInstance<OpenWithDialogViewModel>();
            var parameter = new OpenWithNavigationParameter(FileExtension, null);
            await dialog.ActivateAsync(parameter);

            Assert.True(dialog.SelectCommand.CanExecute(null));
            dialog.SelectedDefaultApplication = null;
            Assert.False(dialog.SelectCommand.CanExecute(null));
            dialog.SelectedOtherApplication = dialog.OtherApplications.First();
            Assert.True(dialog.SelectCommand.CanExecute(null));
        }
    }
}