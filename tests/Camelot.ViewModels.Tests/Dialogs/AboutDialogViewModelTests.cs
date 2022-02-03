using System.Linq;
using Camelot.Avalonia.Interfaces;
using Camelot.Services.Abstractions;
using Camelot.ViewModels.Configuration;
using Camelot.ViewModels.Implementations.Dialogs;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.Dialogs;

public class AboutDialogViewModelTests
{
    private readonly AutoMocker _autoMocker;

    public AboutDialogViewModelTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Fact]
    public void TestInformation()
    {
        const string version = "1.0.0";

        _autoMocker
            .Setup<IApplicationVersionProvider, string>(m => m.Version)
            .Returns(version);
        var configuration = new AboutDialogConfiguration
        {
            Maintainers = new[] {"Maintainer1", "Maintainer2"}
        };
        _autoMocker.Use(configuration);

        var dialog = _autoMocker.CreateInstance<AboutDialogViewModel>();

        Assert.Equal(version, dialog.ApplicationVersion);
        Assert.True(configuration.Maintainers.All(dialog.Maintainers.Contains));
    }

    [Fact]
    public void TestOpenRepositoryCommand()
    {
        const string url = "url";

        _autoMocker
            .Setup<IResourceOpeningService>(m => m.Open(url))
            .Verifiable();
        var configuration = new AboutDialogConfiguration {RepositoryUrl = url};
        _autoMocker.Use(configuration);

        var dialog = _autoMocker.CreateInstance<AboutDialogViewModel>();

        Assert.True(dialog.OpenRepositoryCommand.CanExecute(null));

        dialog.OpenRepositoryCommand.Execute(null);

        _autoMocker.Verify<IResourceOpeningService>(m => m.Open(url), Times.Once);
    }
}