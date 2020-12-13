using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Avalonia.Interfaces;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Models.Operations;
using Camelot.Services.Abstractions.Operations;
using Camelot.ViewModels.Configuration;
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
        public async Task TestBlockedOperationContinue(bool areMultipleFilesAvailable, int filesCount)
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
                .Setup<IDialogService, Task<OverwriteOptionsDialogResult>>(m =>
                    m.ShowDialogAsync<OverwriteOptionsDialogResult, OverwriteOptionsNavigationParameter>(
                        nameof(OverwriteOptionsDialogViewModel),
                        It.Is<OverwriteOptionsNavigationParameter>(p =>
                            p.AreMultipleFilesAvailable == areMultipleFilesAvailable && p.SourceFilePath == Source &&
                            p.DestinationFilePath == Destination)))
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

        [Fact]
        public async Task TestBlockedOperationCancel()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

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

            var viewModel = _autoMocker.CreateInstance<OperationsStatesListViewModel>();

            var state = OperationState.InProgress;
            var operationMock = new Mock<IOperation>();
            operationMock
                .Setup(m => m.CancelAsync())
                .Verifiable();
            operationMock
                .SetupGet(m => m.State)
                .Returns(() => state);
            operationMock
                .SetupGet(m => m.CurrentBlockedFile)
                .Returns((Source, Destination));
            var array = new string[0];
            var dictionary = new Dictionary<string, string>();
            for (var i = 0; i < 5; i++)
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

            operationMock.Verify(m => m.CancelAsync(), Times.Once);
        }

        [Fact]
        public async Task TestInProgressOperation()
        {
            _autoMocker
                .Setup<IApplicationDispatcher>(m => m.Dispatch(It.IsAny<Action>()))
                .Callback<Action>(action => action());

            var configuration = new OperationsStatesConfiguration
            {
                MaximumFinishedOperationsCount = 1
            };
            _autoMocker.Use(configuration);

            var viewModel = _autoMocker.CreateInstance<OperationsStatesListViewModel>();

            var state = OperationState.InProgress;
            var operationMock = new Mock<IOperation>();
            operationMock
                .SetupGet(m => m.State)
                .Returns(() => state);
            var array = new string[0];
            var dictionary = new Dictionary<string, string>();
            for (var i = 0; i < 5; i++)
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

            Assert.False(viewModel.IsInProgress);
            Assert.Equal(0, viewModel.TotalProgress);

            var operationStartedEventArgs = new OperationStartedEventArgs(operationMock.Object);
            _autoMocker
                .GetMock<IOperationsStateService>()
                .Raise(m => m.OperationStarted += null, operationStartedEventArgs);

           Assert.NotEmpty(viewModel.ActiveOperations);
           Assert.Single(viewModel.ActiveOperations);
           Assert.Empty(viewModel.InactiveOperations);

           _autoMocker
               .Setup<IOperationsStateService, IReadOnlyList<IOperation>>(m => m.ActiveOperations)
               .Returns(new[] {operationMock.Object});

           const double progress = 0.5;
           var args = new OperationProgressChangedEventArgs(progress);
           operationMock
               .Setup(m => m.CurrentProgress)
               .Returns(progress);
           operationMock
               .Raise(m => m.ProgressChanged += null, args);

           Assert.NotEmpty(viewModel.ActiveOperations);
           Assert.Single(viewModel.ActiveOperations);
           Assert.Empty(viewModel.InactiveOperations);

           Assert.True(viewModel.IsInProgress);
           Assert.Equal(50, viewModel.TotalProgress);

           var taskCompletionSource = new TaskCompletionSource<bool>();

           _autoMocker
               .Setup<IApplicationDispatcher>(m => m.Dispatch(It.IsAny<Action>()))
               .Callback<Action>(action =>
               {
                   action();
                   taskCompletionSource.SetResult(true);
               });

           state = OperationState.Finished;
           var finishedArgs = new OperationStateChangedEventArgs(OperationState.Finished);
           operationMock
               .Raise(m => m.StateChanged += null, finishedArgs);

           var task = await Task.WhenAny(Task.Delay(1000), taskCompletionSource.Task);
           if (task != taskCompletionSource.Task)
           {
               taskCompletionSource.SetResult(false);
           }

           var result = await taskCompletionSource.Task;
           Assert.True(result);

           Assert.NotEmpty(viewModel.InactiveOperations);
           Assert.Single(viewModel.InactiveOperations);
           Assert.Empty(viewModel.ActiveOperations);
        }
    }
}