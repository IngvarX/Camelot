using Camelot.ViewModels.Implementations.Dialogs;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.Dialogs;

public class DialogViewModelBaseTests
{
    private readonly AutoMocker _autoMocker;

    public DialogViewModelBaseTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Fact]
    public void TestCloseCommand()
    {
        var dialog = _autoMocker.CreateInstance<TestDialogViewModel>();
        Assert.True(dialog.CloseCommand.CanExecute(null));

        var isCallbackCalled = false;
        dialog.CloseRequested += (sender, args) => isCallbackCalled = true;
        dialog.CloseCommand.Execute(null);

        Assert.True(isCallbackCalled);
    }

    private class TestDialogViewModel : DialogViewModelBase
    {

    }
}