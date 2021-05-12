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
        [InlineData(OperationState.Finished, OperationState.Finished, 1, 1)]
        [InlineData(OperationState.Finished, OperationState.Failed, 1, 0)]
        [InlineData(OperationState.Failed, OperationState.NotStarted, 0, 0)]
        public async Task TestCopyOperationCancel(OperationState firstState, OperationState secondState,
            int cancelFirstCount, int cancelSecondCount)
        {
            var copyOperation = new Mock<IInternalOperation>();
            copyOperation
                .SetupGet(m => m.State)
                .Returns(firstState);
            copyOperation
                .Setup(m => m.RunAsync(It.IsAny<CancellationToken>()))
                .Returns(() =>
                {
                    copyOperation.Raise(m => m.StateChanged += null,
                        new OperationStateChangedEventArgs(firstState));

                    return Task.CompletedTask;
                })
                .Verifiable();
            var secondCopyOperation = new Mock<IInternalOperation>();
            secondCopyOperation
                .SetupGet(m => m.State)
                .Returns(secondState);
            secondCopyOperation
                .Setup(m => m.RunAsync(It.IsAny<CancellationToken>()))
                .Returns(() =>
                {
                    secondCopyOperation.Raise(m => m.StateChanged += null,
                        new OperationStateChangedEventArgs(secondState));

                    return Task.CompletedTask;
                })
                .Verifiable();
            var cancelOperation = new Mock<IInternalOperation>();
            cancelOperation
                .SetupGet(m => m.State)
                .Returns(OperationState.Finished);
            cancelOperation
                .Setup(m => m.RunAsync(It.IsAny<CancellationToken>()))
                .Returns(() =>
                {
                    cancelOperation.Raise(m => m.StateChanged += null,
                        new OperationStateChangedEventArgs(OperationState.Finished));

                    return Task.CompletedTask;
                })
                .Verifiable();
            var secondCancelOperation = new Mock<IInternalOperation>();
            secondCancelOperation
                .Setup(m => m.RunAsync(It.IsAny<CancellationToken>()))
                .Returns(() =>
                {
                    secondCancelOperation.Raise(m => m.StateChanged += null,
                        new OperationStateChangedEventArgs(OperationState.Finished));

                    return Task.CompletedTask;
                })
                .Verifiable();
            secondCancelOperation
                .SetupGet(m => m.State)
                .Returns(OperationState.Finished);
            IReadOnlyList<OperationGroup> operationGroups = new List<OperationGroup>
            {
                new OperationGroup(
                    new[] {copyOperation.Object}, new[] {cancelOperation.Object}),
                new OperationGroup(
                    new[] {secondCopyOperation.Object}, new[] {secondCancelOperation.Object})
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

            try
            {
                await operation.RunAsync();
            }
            catch
            {
                // ignore
            }

            await operation.CancelAsync();

            cancelOperation
                .Verify(m => m.RunAsync(It.IsAny<CancellationToken>()),
                    Times.Exactly(cancelFirstCount));
            secondCancelOperation
                .Verify(m => m.RunAsync(It.IsAny<CancellationToken>()),
                    Times.Exactly(cancelSecondCount));
        }
    }
}