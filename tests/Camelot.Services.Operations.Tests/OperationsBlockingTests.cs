using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Models.Operations;
using Camelot.Services.Abstractions.Operations;
using Camelot.Services.Operations.Models;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Operations.Tests;

public class OperationsBlockingTests
{
    private const string SourceName = "Source";
    private const string SecondSourceName = "SecondSource";
    private const string DestinationName = "Destination";
    private const string SecondDestinationName = "SecondDestination";
    private const string ThirdDestinationName = "ThirdDestination";
    private const string FourthDestinationName = "FourthDestination";

    private readonly AutoMocker _autoMocker;

    public OperationsBlockingTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Theory]
    [InlineData(true, 1, OperationContinuationMode.Overwrite, 1, 1, 0, 0)]
    [InlineData(false, 2, OperationContinuationMode.Overwrite, 1, 1, 0, 0)]
    [InlineData(true, 1, OperationContinuationMode.Skip, 1, 1, 0, 0)]
    [InlineData(false, 2, OperationContinuationMode.Skip, 1, 1, 0, 0)]
    [InlineData(true, 1, OperationContinuationMode.OverwriteIfOlder, 1, 1, 0, 0)]
    [InlineData(false, 2, OperationContinuationMode.OverwriteIfOlder, 1, 1, 0, 0)]
    [InlineData(true, 1, OperationContinuationMode.Rename, 0, 0, 1, 1)]
    [InlineData(false, 2, OperationContinuationMode.Rename, 0, 0, 1, 1)]
    public async Task TestBlockedCopyOperationMultiple(bool applyToAll, int expectedCallbackCallsCount,
        OperationContinuationMode mode, int expectedWriteCallsCountFirstFile, int expectedWriteCallsCountSecondFile,
        int expectedWriteCallsCountThirdFile, int expectedWriteCallsCountFourthFile)
    {
        _autoMocker
            .Setup<IFileNameGenerationService, string>(m => m.GenerateFullName(It.IsAny<string>()))
            .Returns<string>(GetNewFileName);

        var taskCompletionSource = new TaskCompletionSource<bool>();
        var secondTaskCompletionSource = new TaskCompletionSource<bool>();
        var copyOperation = new Mock<IInternalOperation>();
        copyOperation
            .SetupGet(m => m.State)
            .Returns(OperationState.Blocked);
        var blockedOperation = copyOperation.As<ISelfBlockingOperation>();
        blockedOperation
            .Setup(m => m.CurrentBlockedFile)
            .Returns((SourceName, DestinationName));
        blockedOperation
            .Setup(m => m.ContinueAsync(It.Is<OperationContinuationOptions>(
                o => o.ApplyToAll == applyToAll
                     && o.Mode == mode)))
            .Returns(() =>
            {
                copyOperation
                    .SetupGet(m => m.State)
                    .Returns(OperationState.Finished);
                copyOperation.Raise(m => m.StateChanged += null,
                    new OperationStateChangedEventArgs(OperationState.Finished));
                taskCompletionSource.SetResult(true);

                return Task.CompletedTask;
            })
            .Verifiable();
        copyOperation
            .Setup(m => m.RunAsync(It.IsAny<CancellationToken>()))
            .Returns(async () =>
            {
                copyOperation.Raise(m => m.StateChanged += null,
                    new OperationStateChangedEventArgs(OperationState.Blocked));

                await taskCompletionSource.Task;
            });
        var secondCopyOperation = new Mock<IInternalOperation>();
        secondCopyOperation
            .SetupGet(m => m.State)
            .Returns(OperationState.Blocked);
        var secondBlockedOperation = secondCopyOperation.As<ISelfBlockingOperation>();
        secondBlockedOperation
            .Setup(m => m.CurrentBlockedFile)
            .Returns((SecondSourceName, SecondDestinationName));
        secondBlockedOperation
            .Setup(m => m.ContinueAsync(It.Is<OperationContinuationOptions>(
                o => o.ApplyToAll == applyToAll
                     && o.Mode == mode)))
            .Returns(() =>
            {
                secondCopyOperation
                    .SetupGet(m => m.State)
                    .Returns(OperationState.Finished);
                secondCopyOperation.Raise(m => m.StateChanged += null,
                    new OperationStateChangedEventArgs(OperationState.Finished));
                secondTaskCompletionSource.SetResult(true);

                return Task.CompletedTask;
            })
            .Verifiable();
        secondCopyOperation
            .Setup(m => m.RunAsync(It.IsAny<CancellationToken>()))
            .Returns(async () =>
            {
                secondCopyOperation.Raise(m => m.StateChanged += null,
                    new OperationStateChangedEventArgs(OperationState.Blocked));

                await secondTaskCompletionSource.Task;
            });
        IReadOnlyList<OperationGroup> operationGroups = new List<OperationGroup>
        {
            new OperationGroup(
                new[] {copyOperation.Object}),
            new OperationGroup(
                new[] {secondCopyOperation.Object})
        };
        _autoMocker.Use(operationGroups);
        var settings = new BinaryFileSystemOperationSettings(
            new string[] { },
            new[] {SourceName, SecondSourceName},
            new string[] { },
            new[] {DestinationName, SecondDestinationName},
            new Dictionary<string, string>
            {
                [SourceName] = DestinationName,
                [SecondSourceName] = SecondDestinationName
            },
            new string[] { }
        );
        _autoMocker.Use(new OperationInfo(OperationType.Copy, settings));

        var operation = _autoMocker.CreateInstance<CompositeOperation>();

        var callbackCallsCount = 0;
        operation.Blocked += async (sender, args) =>
        {
            callbackCallsCount++;

            var (sourceFilePath, _) = operation.CurrentBlockedFile;
            var options = mode is OperationContinuationMode.Rename
                ? OperationContinuationOptions.CreateRenamingContinuationOptions(sourceFilePath, applyToAll,
                    GetNewFileName(sourceFilePath))
                : OperationContinuationOptions.CreateContinuationOptions(sourceFilePath, applyToAll, mode);

            await operation.ContinueAsync(options);
        };

        var task = operation.RunAsync();
        var firstTask = await Task.WhenAny(task, Task.Delay(1000));
        Assert.Equal(task, firstTask);

        Assert.Equal(expectedCallbackCallsCount, callbackCallsCount);

        blockedOperation
            .Verify(m => m.ContinueAsync(It.Is<OperationContinuationOptions>(
                    o => o.ApplyToAll == applyToAll
                         && o.Mode == mode
                         && o.FilePath == SourceName
                         && o.NewFilePath == null)),
                Times.Exactly(expectedWriteCallsCountFirstFile));
        blockedOperation
            .Verify(m => m.ContinueAsync(It.Is<OperationContinuationOptions>(
                    o => o.ApplyToAll == applyToAll
                         && o.Mode == mode
                         && o.FilePath == SourceName
                         && o.NewFilePath == ThirdDestinationName)),
                Times.Exactly(expectedWriteCallsCountThirdFile));
        secondBlockedOperation
            .Verify(m => m.ContinueAsync(It.Is<OperationContinuationOptions>(
                    o => o.ApplyToAll == applyToAll
                         && o.Mode == mode
                         && o.FilePath == SecondSourceName
                         && o.NewFilePath == null)),
                Times.Exactly(expectedWriteCallsCountSecondFile));
        secondBlockedOperation
            .Verify(m => m.ContinueAsync(It.Is<OperationContinuationOptions>(
                    o => o.ApplyToAll == applyToAll
                         && o.Mode == mode
                         && o.FilePath == SecondSourceName
                         && o.NewFilePath == FourthDestinationName)),
                Times.Exactly(expectedWriteCallsCountFourthFile));
    }

    [Theory]
    [InlineData(true, OperationContinuationMode.Overwrite, 1, 0)]
    [InlineData(false, OperationContinuationMode.Overwrite, 1, 0)]
    [InlineData(true, OperationContinuationMode.Skip, 0, 0)]
    [InlineData(false, OperationContinuationMode.Skip, 0, 0)]
    [InlineData(true, OperationContinuationMode.OverwriteIfOlder, 1, 0)]
    [InlineData(false, OperationContinuationMode.OverwriteIfOlder, 1, 0)]
    [InlineData(true, OperationContinuationMode.OverwriteIfOlder, 0, 0, 1)]
    [InlineData(false, OperationContinuationMode.OverwriteIfOlder, 0, 0, 1)]
    [InlineData(true, OperationContinuationMode.Rename, 0, 0, 1)]
    [InlineData(false, OperationContinuationMode.Rename, 0, 0, 1)]
    public async Task TestBlockedCopyOperationSingle(bool applyToAll, OperationContinuationMode mode,
        int expectedWriteCalls, int expectedWriteCallsSecondFile, int offsetHours = -1)
    {
        var now = DateTime.UtcNow;
        var nowWithOffset = now.AddHours(offsetHours);

        _autoMocker
            .Setup<IFileService, FileModel>(m => m.GetFile(SourceName))
            .Returns(new FileModel {LastModifiedDateTime = now});
        _autoMocker
            .Setup<IFileService, FileModel>(m => m.GetFile(DestinationName))
            .Returns(new FileModel {LastModifiedDateTime = nowWithOffset});
        _autoMocker
            .Setup<IFileService>(
                m => m.CopyAsync(SourceName, DestinationName, It.IsAny<CancellationToken>(), false))
            .Verifiable();
        _autoMocker
            .Setup<IFileService, Task<bool>>(m =>
                m.CopyAsync(SourceName, DestinationName, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync(true)
            .Verifiable();
        _autoMocker
            .Setup<IFileService, Task<bool>>(m =>
                m.CopyAsync(SourceName, SecondDestinationName, It.IsAny<CancellationToken>(), false))
            .ReturnsAsync(true)
            .Verifiable();
        _autoMocker
            .Setup<IFileService, bool>(m => m.CheckIfExists(DestinationName))
            .Returns(true);
        var operationsFactory = _autoMocker.CreateInstance<OperationsFactory>();
        var settings = new BinaryFileSystemOperationSettings(
            new string[] { },
            new[] {SourceName},
            new string[] { },
            new[] {DestinationName},
            new Dictionary<string, string>
            {
                [SourceName] = DestinationName
            },
            new string[] { }
        );
        var copyOperation = operationsFactory.CreateCopyOperation(settings);

        var callbackCallsCount = 0;
        copyOperation.StateChanged += async (sender, args) =>
        {
            if (args.OperationState != OperationState.Blocked)
            {
                return;
            }

            var operation = (IOperation) sender;
            if (operation is null)
            {
                return;
            }

            callbackCallsCount++;

            var (sourceFilePath, _) = operation.CurrentBlockedFile;
            var options = mode is OperationContinuationMode.Rename
                ? OperationContinuationOptions.CreateRenamingContinuationOptions(sourceFilePath, applyToAll,
                    SecondDestinationName)
                : OperationContinuationOptions.CreateContinuationOptions(sourceFilePath, applyToAll, mode);

            await copyOperation.ContinueAsync(options);
        };

        await copyOperation.RunAsync();

        Assert.Equal(1, callbackCallsCount);

        Assert.Equal(OperationState.Finished, copyOperation.State);
        _autoMocker
            .Verify<IFileService>(
                m => m.CopyAsync(SourceName, DestinationName, It.IsAny<CancellationToken>(), true),
                Times.Exactly(expectedWriteCalls));
        _autoMocker
            .Verify<IFileService>(
                m => m.CopyAsync(SourceName, SecondDestinationName, It.IsAny<CancellationToken>(), true),
                Times.Exactly(expectedWriteCallsSecondFile));
    }

    private static string GetNewFileName(string name) =>
        name == SourceName ? ThirdDestinationName : FourthDestinationName;
}