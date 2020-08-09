using System.Linq;
using Camelot.Avalonia.Interfaces;
using Camelot.Services.Abstractions;
using Camelot.ViewModels.Configuration;
using Camelot.ViewModels.Implementations.Dialogs;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests.Dialogs
{
    public class AboutDialogViewModelTests
    {
        [Fact]
        public void TestInformation()
        {
            const string version = "1.0.0";

            var applicationVersionProviderMock = new Mock<IApplicationVersionProvider>();
            applicationVersionProviderMock
                .SetupGet(m => m.Version)
                .Returns(version);
            var resourceOpeningServiceMock = new Mock<IResourceOpeningService>();
            var configuration = new AboutDialogConfiguration
            {
                Maintainers = new[] {"Maintainer1", "Maintainer2"}
            };
            var dialog = new AboutDialogViewModel(applicationVersionProviderMock.Object,
                resourceOpeningServiceMock.Object, configuration);

            Assert.Equal(version, dialog.ApplicationVersion);
            Assert.True(configuration.Maintainers.All(dialog.Maintainers.Contains));
        }

        [Fact]
        public void TestOpenRepositoryCommand()
        {
            const string url = "url";

            var applicationVersionProviderMock = new Mock<IApplicationVersionProvider>();
            var resourceOpeningServiceMock = new Mock<IResourceOpeningService>();
            resourceOpeningServiceMock
                .Setup(m => m.Open(url))
                .Verifiable();
            var configuration = new AboutDialogConfiguration {RepositoryUrl = url};
            var dialog = new AboutDialogViewModel(applicationVersionProviderMock.Object,
                resourceOpeningServiceMock.Object, configuration);

            Assert.True(dialog.OpenRepositoryCommand.CanExecute(null));

            dialog.OpenRepositoryCommand.Execute(null);

            resourceOpeningServiceMock.Verify(m => m.Open(url), Times.Once);
        }
    }
}