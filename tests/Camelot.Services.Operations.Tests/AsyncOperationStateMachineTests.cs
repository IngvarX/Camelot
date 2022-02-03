using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Extensions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Models.Operations;
using Camelot.Services.Abstractions.Operations;
using Camelot.Tests.Shared.Extensions;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Operations.Tests;

public class AsyncOperationStateMachineTests
{
    private readonly AutoMocker _autoMocker;

    public AsyncOperationStateMachineTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Fact]
    public void TestProperties()
    {
        const double progress = 42;
        var operationInfo = new OperationInfo(OperationType.Copy, new BinaryFileSystemOperationSettings(
            new string[0], new string[0], new string[0],
            new string[0], new Dictionary<string, string>(0), new string[0]));

        _autoMocker
            .Setup<ICompositeOperation, OperationInfo>(m => m.Info)
            .Returns(operationInfo);
        _autoMocker
            .Setup<ICompositeOperation, double>(m => m.CurrentProgress)
            .Returns(progress);

        var machine = _autoMocker.CreateInstance<AsyncOperationStateMachine>();
        Assert.Equal(operationInfo, machine.Info);
        Assert.Equal(progress, machine.CurrentProgress);
    }

    [Fact]
    public void TestProgressEvents()
    {
        var machine = _autoMocker.CreateInstance<AsyncOperationStateMachine>();

        var callbackCallsCount = 0;

        void OnProgressChanged(object sender, OperationProgressChangedEventArgs e) => callbackCallsCount++;

        machine.ProgressChanged += OnProgressChanged;

        _autoMocker
            .GetMock<ICompositeOperation>()
            .Raise(m => m.ProgressChanged += null, new OperationProgressChangedEventArgs(42));

        Assert.Equal(1, callbackCallsCount);

        machine.ProgressChanged -= OnProgressChanged;

        _autoMocker
            .GetMock<ICompositeOperation>()
            .Raise(m => m.ProgressChanged += null, new OperationProgressChangedEventArgs(42));

        Assert.Equal(1, callbackCallsCount);
    }

    [Fact]
    public async Task TestException()
    {
        _autoMocker
            .Setup<ICompositeOperation, Task>(m => m.RunAsync())
            .Throws<InvalidOperationException>();
        _autoMocker.MockLogError();

        var machine = _autoMocker.CreateInstance<AsyncOperationStateMachine>();

        await machine.RunAsync();

        _autoMocker.VerifyLogError(Times.Once());
    }

    [Fact]
    public async Task TestWrongState()
    {
        var machine = _autoMocker.CreateInstance<AsyncOperationStateMachine>();

        Task PauseAsync() => machine.PauseAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(PauseAsync);
    }

    [Fact]
    public async Task TestPause()
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();

        _autoMocker
            .Setup<ICompositeOperation, Task>(m => m.RunAsync())
            .Returns(taskCompletionSource.Task);

        var machine = _autoMocker.CreateInstance<AsyncOperationStateMachine>();

        machine.RunAsync().Forget();

        await machine.PauseAsync();

        Assert.Equal(OperationState.Paused, machine.State);
        taskCompletionSource.SetResult(true);

        _autoMocker
            .Verify<ICompositeOperation, Task>(m => m.PauseAsync(),
                Times.Once);
    }

    [Fact]
    public async Task TestUnpause()
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();

        _autoMocker
            .Setup<ICompositeOperation, Task>(m => m.RunAsync())
            .Returns(taskCompletionSource.Task);

        var machine = _autoMocker.CreateInstance<AsyncOperationStateMachine>();

        machine.RunAsync().Forget();

        await machine.PauseAsync();
        await machine.UnpauseAsync();

        Assert.Equal(OperationState.InProgress, machine.State);
        taskCompletionSource.SetResult(true);

        _autoMocker
            .Verify<ICompositeOperation, Task>(m => m.UnpauseAsync(),
                Times.Once);
    }

    [Fact]
    public async Task TestCancel()
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();

        _autoMocker
            .Setup<ICompositeOperation, Task>(m => m.RunAsync())
            .Returns(taskCompletionSource.Task);

        var machine = _autoMocker.CreateInstance<AsyncOperationStateMachine>();

        machine.RunAsync().Forget();

        await machine.CancelAsync();

        Assert.Equal(OperationState.Cancelled, machine.State);
        taskCompletionSource.SetResult(true);

        _autoMocker
            .Verify<ICompositeOperation, Task>(m => m.CancelAsync(),
                Times.Once);
    }
}