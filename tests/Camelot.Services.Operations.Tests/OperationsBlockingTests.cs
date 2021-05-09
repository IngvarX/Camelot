using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.Operations;
using Camelot.Services.Abstractions.Operations;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Operations.Tests
{
    public class OperationsBlockingTests
    {
        private const string SourceName = "Source";
        private const string SecondSourceName = "SecondSource";
        private const string DestinationName = "Destination";
        private const string SecondDestinationName = "SecondDestination";
        private const string ThirdDestinationName = "ThirdDestination";

        private readonly AutoMocker _autoMocker;

        public OperationsBlockingTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Theory]
        [InlineData(true, 1, OperationContinuationMode.Overwrite, 1, 1, 0)]
        [InlineData(false, 2, OperationContinuationMode.Overwrite, 1, 1, 0)]
        [InlineData(true, 1, OperationContinuationMode.Skip, 0, 0, 0)]
        [InlineData(false, 2, OperationContinuationMode.Skip, 0, 0, 0)]
        [InlineData(true, 1, OperationContinuationMode.OverwriteIfOlder, 1, 0, 0)]
        [InlineData(false, 2, OperationContinuationMode.OverwriteIfOlder, 1, 0, 0)]
        [InlineData(true, 1, OperationContinuationMode.Rename, 0, 0, 1)]
        [InlineData(false, 2, OperationContinuationMode.Rename, 0, 0, 1)]
        public async Task TestBlockedCopyOperation(bool applyToAll, int expectedCallbackCallsCount,
            OperationContinuationMode mode, int expectedWriteCallsCountFirstFile, int expectedWriteCallsCountSecondFile,
            int expectedWriteCallsCountThirdFile)
        {
            var now = DateTime.UtcNow;
            var hourBeforeNow = now.AddHours(-1);

            _autoMocker
                .Setup<IFileService, FileModel>(m => m.GetFile(SourceName))
                .Returns(new FileModel {LastModifiedDateTime = now});
            _autoMocker
                .Setup<IFileService, FileModel>(m => m.GetFile(DestinationName))
                .Returns(new FileModel {LastModifiedDateTime = hourBeforeNow});
            _autoMocker
                .Setup<IFileService, FileModel>(m => m.GetFile(SecondSourceName))
                .Returns(new FileModel {LastModifiedDateTime = hourBeforeNow});
            _autoMocker
                .Setup<IFileService, FileModel>(m => m.GetFile(SecondDestinationName))
                .Returns(new FileModel {LastModifiedDateTime = now});
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
                .Setup<IFileService>(m =>
                    m.CopyAsync(SecondSourceName, SecondDestinationName, It.IsAny<CancellationToken>(), false))
                .Verifiable();
            _autoMocker
                .Setup<IFileService, Task<bool>>(m =>
                    m.CopyAsync(SecondSourceName, SecondDestinationName, It.IsAny<CancellationToken>(), true))
                .ReturnsAsync(true)
                .Verifiable();
            _autoMocker
                .Setup<IFileService, Task<bool>>(m =>
                    m.CopyAsync(SourceName, ThirdDestinationName, It.IsAny<CancellationToken>(), false))
                .ReturnsAsync(true)
                .Verifiable();
            _autoMocker
                .Setup<IFileService, Task<bool>>(m =>
                    m.CopyAsync(SecondSourceName, ThirdDestinationName, It.IsAny<CancellationToken>(), false))
                .ReturnsAsync(true)
                .Verifiable();
            _autoMocker
                .Setup<IFileService, bool>(m => m.CheckIfExists(It.IsAny<string>()))
                .Returns(true);
            _autoMocker
                .Setup<IFileNameGenerationService, string>(m => m.GenerateFullName(It.IsAny<string>()))
                .Returns(ThirdDestinationName);
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

                Interlocked.Increment(ref callbackCallsCount);

                var (sourceFilePath, _) = operation.CurrentBlockedFile;
                var options = mode is OperationContinuationMode.Rename
                    ? OperationContinuationOptions.CreateRenamingContinuationOptions(sourceFilePath, applyToAll,
                        ThirdDestinationName)
                    : OperationContinuationOptions.CreateContinuationOptions(sourceFilePath, applyToAll, mode);

                await copyOperation.ContinueAsync(options);
            };

            await copyOperation.RunAsync();

            Assert.Equal(expectedCallbackCallsCount, callbackCallsCount);

            Assert.Equal(OperationState.Finished, copyOperation.State);
            _autoMocker
                .Verify<IFileService>(
                    m => m.CopyAsync(SourceName, DestinationName, It.IsAny<CancellationToken>(), true),
                    Times.Exactly(expectedWriteCallsCountFirstFile));
            _autoMocker
                .Verify<IFileService>(
                    m => m.CopyAsync(SecondSourceName, SecondDestinationName, It.IsAny<CancellationToken>(), true),
                    Times.Exactly(expectedWriteCallsCountSecondFile));
            _autoMocker
                .Verify<IFileService>(
                    m => m.CopyAsync(SourceName, DestinationName, It.IsAny<CancellationToken>(), false),
                    Times.Never);
            _autoMocker
                .Verify<IFileService>(
                    m => m.CopyAsync(SecondSourceName, SecondDestinationName, It.IsAny<CancellationToken>(), false),
                    Times.Never);
            _autoMocker
                .Verify<IFileService>(
                    m => m.CopyAsync(SourceName, ThirdDestinationName, It.IsAny<CancellationToken>(), false),
                    Times.Exactly(expectedWriteCallsCountThirdFile));
            _autoMocker
                .Verify<IFileService>(
                    m => m.CopyAsync(SecondSourceName, ThirdDestinationName, It.IsAny<CancellationToken>(), false),
                    Times.Exactly(expectedWriteCallsCountThirdFile));
        }
    }
}