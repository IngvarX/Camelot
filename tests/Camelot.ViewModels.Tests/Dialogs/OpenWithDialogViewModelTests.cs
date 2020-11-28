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
            Assert.Equal(parameter.FileExtension, dialog.OpenFileExtension);
            Assert.Null(dialog.SelectedApplication);
            Assert.Empty(dialog.RecommendedApplications);
            Assert.Empty(dialog.OtherApplications);
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

            var dialog = _autoMocker.CreateInstance<OpenWithDialogViewModel>();
            var parameter = new OpenWithNavigationParameter(FileExtension, null);
            await dialog.ActivateAsync(parameter);

            Assert.Equal(application, dialog.SelectedApplication);
            Assert.Equal(application, dialog.RecommendedApplications.Single());
            Assert.Empty(dialog.OtherApplications);
        }
    }
}