using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Extensions;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Models.Operations;
using Camelot.Services.Abstractions.Operations;
using Camelot.Services.Operations.Models;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Operations.Tests
{
    public class OperationsCancelTests
    {
        private const string SourceName = "Source";
        private const string SecondSourceName = "SecondSource";
        private const string DestinationName = "Destination";
        private const string SecondDestinationName = "SecondDestination";

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
            var cancelOperation = new Mock<IInternalOperation>();
            IReadOnlyList<OperationGroup> operationGroups = new List<OperationGroup>
            {
                new OperationGroup(
                    new[] {copyOperation.Object}, new[] {cancelOperation.Object})
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

            cancelOperation
                .Verify(m => m.RunAsync(It.IsAny<CancellationToken>()),
                    Times.Never);
        }

        [Theory]
        [InlineData(true, 1, 1, false)]
        [InlineData(false, 0, 1, false)]
        [InlineData(false, 1, 1, true)]
        public async Task TestCopyOperationCancel(bool isSuccessFirst, int removeFirstCalled,
            int removeSecondCalled, bool shouldThrow)
        {
            var firstTaskCompletionSource = new TaskCompletionSource<bool>();
            var secondTaskCompletionSource = new TaskCompletionSource<bool>();
            var cancelTaskCompletionSource = new TaskCompletionSource<bool>();

            _autoMocker
                .Setup<IFileService, Task<bool>>(m =>
                    m.CopyAsync(SourceName, DestinationName, It.IsAny<CancellationToken>(), false))
                .Returns(async () =>
                {
                    await firstTaskCompletionSource.Task;
                    secondTaskCompletionSource.SetResult(true);
                    await cancelTaskCompletionSource.Task;
                    await Task.Delay(100);
                    if (shouldThrow)
                    {
                        throw new TaskCanceledException();
                    }

                    return isSuccessFirst;
                });
            _autoMocker
                .Setup<IFileService, Task<bool>>(m =>
                    m.CopyAsync(SecondSourceName, SecondDestinationName, It.IsAny<CancellationToken>(), false))
                .ReturnsAsync(true)
                .Callback(() => firstTaskCompletionSource.SetResult(true));
            _autoMocker
                .Setup<IFileService, bool>(m => m.Remove(DestinationName))
                .Returns(true)
                .Verifiable();
            _autoMocker
                .Setup<IFileService, bool>(m => m.Remove(SecondDestinationName))
                .Returns(true)
                .Verifiable();

            var operationsFactory = _autoMocker.CreateInstance<OperationsFactory>();
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
            var copyOperation = operationsFactory.CreateCopyOperation(settings);

            Assert.Equal(OperationState.NotStarted, copyOperation.State);

            Task.Run(copyOperation.RunAsync).Forget();

            await firstTaskCompletionSource.Task;
            await secondTaskCompletionSource.Task;
            var task = copyOperation.CancelAsync();
            cancelTaskCompletionSource.SetResult(true);
            await task;

            Assert.Equal(OperationState.Cancelled, copyOperation.State);

            _autoMocker
                .Verify<IFileService, bool>(m => m.Remove(DestinationName),
                    Times.Exactly(removeFirstCalled));
            _autoMocker
                .Verify<IFileService, bool>(m => m.Remove(SecondDestinationName),
                    Times.Exactly(removeSecondCalled));
        }
    }
}