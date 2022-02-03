using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Models.Operations;
using Camelot.Services.Abstractions.Operations;
using Camelot.Services.Operations.Models;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Operations.Tests;

public class OperationsCancelTests
{
    private const string SourceName = "Source";
    private const string DestinationName = "Destination";

    private readonly AutoMocker _autoMocker;

    public OperationsCancelTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Fact]
    public async Task TestBlockedOperationCancel()
    {
        var copyOperation = new Mock<IInternalOperation>();
        copyOperation
            .SetupGet(m => m.State)
            .Returns(OperationState.Blocked);
        var blockedOperation = copyOperation.As<ISelfBlockingOperation>();
        blockedOperation
            .Setup(m => m.CurrentBlockedFile)
            .Returns((SourceName, DestinationName));
        copyOperation
            .Setup(m => m.RunAsync(It.IsAny<CancellationToken>()))
            .Callback(() =>
            {
                copyOperation.Raise(m => m.StateChanged += null,
                    new OperationStateChangedEventArgs(OperationState.Blocked));
            });
        IReadOnlyList<OperationGroup> operationGroups = new List<OperationGroup>
        {
            new OperationGroup(
                new[] {copyOperation.Object})
        };
        _autoMocker.Use(operationGroups);
        var settings = new BinaryFileSystemOperationSettings(
            new string[] { },
            new[] {SourceName},
            new string[] { },
            new[] {DestinationName},
            new Dictionary<string, string> {[SourceName] = DestinationName},
            new string[] { }
        );
        _autoMocker.Use(new OperationInfo(OperationType.Copy, settings));

        var operation = _autoMocker.CreateInstance<CompositeOperation>();
        var taskCompletionSource = new TaskCompletionSource<bool>();
        operation.Blocked += async (sender, args) =>
        {
            if (operation.CurrentBlockedFile == (SourceName, DestinationName))
            {
                await operation.CancelAsync();
                taskCompletionSource.SetResult(true);
            }
        };

        await operation.RunAsync();

        var task = await Task.WhenAny(Task.Delay(500), taskCompletionSource.Task);
        if (task != taskCompletionSource.Task)
        {
            taskCompletionSource.SetResult(false);
        }

        var result = await taskCompletionSource.Task;
        Assert.True(result);
    }
}