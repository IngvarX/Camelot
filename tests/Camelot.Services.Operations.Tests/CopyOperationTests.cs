using System;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.Operations;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Operations.Tests;

public class CopyOperationTests
{
    private const string SourceName = "Source";
    private const string DestinationName = "Destination";

    private readonly AutoMocker _autoMocker;

    public CopyOperationTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Fact]
    public async Task TestCopyCancel()
    {
        _autoMocker
            .Setup<IFileService, Task<bool>>(m => m.CopyAsync(SourceName, DestinationName,
                It.IsAny<CancellationToken>(),
                false))
            .Throws<TaskCanceledException>();

        var operation = new CopyOperation(
            _autoMocker.Get<IDirectoryService>(),
            _autoMocker.Get<IFileService>(),
            _autoMocker.Get<IPathService>(),
            SourceName,
            DestinationName
        );

        Task RunAsync() => operation.RunAsync(default);

        await Assert.ThrowsAsync<TaskCanceledException>(RunAsync);
        Assert.Equal(OperationState.Cancelled, operation.State);
    }

    [Fact]
    public async Task TestContinueThrows()
    {
        var operation = new CopyOperation(
            _autoMocker.Get<IDirectoryService>(),
            _autoMocker.Get<IFileService>(),
            _autoMocker.Get<IPathService>(),
            SourceName,
            DestinationName
        );
        const OperationContinuationMode mode = (OperationContinuationMode) 42;
        var options = OperationContinuationOptions.CreateContinuationOptions(SourceName, true, mode);

        Task ContinueAsync() => operation.ContinueAsync(options);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(ContinueAsync);
    }
}