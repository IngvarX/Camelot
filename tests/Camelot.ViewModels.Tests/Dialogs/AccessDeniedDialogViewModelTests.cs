using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.Dialogs;

public class AccessDeniedDialogViewModelTests
{
    private const string Directory = "Dir";

    private readonly AutoMocker _autoMocker;

    public AccessDeniedDialogViewModelTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Fact]
    public void TestDirectory()
    {
        var dialog = _autoMocker.CreateInstance<AccessDeniedDialogViewModel>();
        Assert.Null(dialog.Directory);

        var parameter = new AccessDeniedNavigationParameter(Directory);
        dialog.Activate(parameter);
        Assert.Equal(Directory, dialog.Directory);
    }
}