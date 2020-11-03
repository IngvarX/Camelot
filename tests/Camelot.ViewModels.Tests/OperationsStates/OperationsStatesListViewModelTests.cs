using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Avalonia.Interfaces;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Models.Operations;
using Camelot.Services.Abstractions.Operations;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Implementations.Dialogs.Results;
using Camelot.ViewModels.Implementations.MainWindow.OperationsStates;
using Camelot.ViewModels.Services.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.OperationsStates
{
    public class OperationsStatesListViewModelTests
    {
        private const string FilePath = "Path";
        private const string Source = "Source";
        private const string Destination = "Destination";

        private readonly AutoMocker _autoMocker;

        public OperationsStatesListViewModelTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Theory]
        [InlineData(false, 1)]
        [InlineData(true, 2)]
        public async Task TestBlockedOperationCancel(bool areMultipleFilesAvailable, int filesCount)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            var options = OperationContinuationOptions.CreateContinuationOptions(FilePath,
                true, OperationContinuationMode.Skip);
            _autoMocker
                .Setup<IApplicationDispatcher>(m => m.Dispatch(It.IsAny<Action>()))
                .Callback<Action>(action => action());
            _autoMocker
                .Setup<IApplicationDispatcher>(m => m.DispatchAsync(It.IsAny<Func<Task>>()))
                .Callback<Func<Task>>(async func =>
                {
                    await func();
                    taskCompletionSource.SetResult(true);
                });
            _autoMocker
                .Setup<IDialogService, Task<OverwriteOptionsDialogResult>>(m => m.ShowDialogAsync<OverwriteOptionsDialogResult, OverwriteOptionsNavigationParameter>(
                    nameof(OverwriteOptionsDialogViewModel),
                    It.IsAny<OverwriteOptionsNavigationParameter>()))
                .Callback<string, OverwriteOptionsNavigationParameter>((_, p) =>
                {
                    Assert.Equal(areMultipleFilesAvailable, p.AreMultipleFilesAvailable);
                    Assert.Equal(Source, p.SourceFilePath);
                    Assert.Equal(Destination, p.DestinationFilePath);
                })
                .ReturnsAsync(new OverwriteOptionsDialogResult(options));

            var viewModel = _autoMocker.CreateInstance<OperationsStatesListViewModel>();

            var state = OperationState.InProgress;
            var operationMock = new Mock<IOperation>();
            operationMock
                .Setup(m => m.ContinueAsync(options))
                .Verifiable();
            operationMock
                .SetupGet(m => m.State)
                .Returns(() => state);
            operationMock
                .SetupGet(m => m.CurrentBlockedFile)
                .Returns((Source, Destination));
            var array = new string[0];
            var dictionary = new Dictionary<string, string>();
            for (var i = 0; i < filesCount; i++)
            {
                dictionary[i.ToString()] = i.ToString();
            }
            var settings = new BinaryFileSystemOperationSettings(
                array, array,
                array,array, dictionary, array);
            var operationInfo = new OperationInfo(OperationType.Copy, settings);
            operationMock
                .SetupGet(m => m.Info)
                .Returns(operationInfo);

            var operationStartedEventArgs = new OperationStartedEventArgs(operationMock.Object);
            _autoMocker
                .GetMock<IOperationsStateService>()
                .Raise(m => m.OperationStarted += null, operationStartedEventArgs);

            Assert.True(viewModel.AreAnyOperationsAvailable);

            state = OperationState.Blocked;
            var operationStateChangedEventArgs = new OperationStateChangedEventArgs(state);
            operationMock.Raise(m => m.StateChanged += null, operationStateChangedEventArgs);

            Assert.True(viewModel.AreAnyOperationsAvailable);

            var task = await Task.WhenAny(Task.Delay(1000), taskCompletionSource.Task);
            if (task != taskCompletionSource.Task)
            {
                taskCompletionSource.SetResult(false);
            }

            var result = await taskCompletionSource.Task;
            Assert.True(result);

            await Task.Delay(500);

            operationMock.Verify(m => m.ContinueAsync(options), Times.Once);
        }
    }
}