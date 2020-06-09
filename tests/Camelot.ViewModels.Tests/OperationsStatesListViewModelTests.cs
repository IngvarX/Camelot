using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationDispatcher.Interfaces;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Models.Operations;
using Camelot.Services.Abstractions.Operations;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Implementations.Dialogs.Results;
using Camelot.ViewModels.Implementations.MainWindow.OperationsStates;
using Camelot.ViewModels.Services.Interfaces;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests
{
    public class OperationsStatesListViewModelTests
    {
        private const string FilePath = "Path";
        private const string Source = "Source";
        private const string Destination = "Destination";

        [Theory]
        [InlineData(false, 1)]
        [InlineData(true, 2)]
        public async Task TestBlockedOperationCancel(bool areMultipleFilesAvailable, int filesCount)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            var options = OperationContinuationOptions.CreateContinuationOptions(FilePath,
                true, OperationContinuationMode.Skip);
            var operationsStateServiceMock = new Mock<IOperationsStateService>();
            var operationStateViewModelFactoryMock = new Mock<IOperationStateViewModelFactory>();
            var applicationDispatcherMock = new Mock<IApplicationDispatcher>();
            applicationDispatcherMock
                .Setup(m => m.Dispatch(It.IsAny<Action>()))
                .Callback<Action>(action => action());
            applicationDispatcherMock
                .Setup(m => m.DispatchAsync(It.IsAny<Func<Task>>()))
                .Callback<Func<Task>>(async func =>
                {
                    await func();
                    taskCompletionSource.SetResult(true);
                });
            var dialogServiceMock = new Mock<IDialogService>();
            dialogServiceMock
                .Setup(m => m.ShowDialogAsync<OverwriteOptionsDialogResult, OverwriteOptionsNavigationParameter>(
                    nameof(OverwriteOptionsDialogViewModel),
                    It.IsAny<OverwriteOptionsNavigationParameter>()))
                .Callback<string, OverwriteOptionsNavigationParameter>((_, p) =>
                {
                    Assert.Equal(areMultipleFilesAvailable, p.AreMultipleFilesAvailable);
                    Assert.Equal(Source, p.SourceFilePath);
                    Assert.Equal(Destination, p.DestinationFilePath);
                })
                .Returns(Task.FromResult(new OverwriteOptionsDialogResult(options)));

            var viewModel = new OperationsStatesListViewModel(
                operationsStateServiceMock.Object,
                operationStateViewModelFactoryMock.Object,
                applicationDispatcherMock.Object,
                dialogServiceMock.Object);

            var operationMock = new Mock<IOperation>();
            operationMock
                .Setup(m => m.ContinueAsync(options))
                .Verifiable();
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
                array,array, dictionary);
            var operationInfo = new OperationInfo(OperationType.Copy, settings);
            operationMock
                .SetupGet(m => m.Info)
                .Returns(operationInfo);

            var operationStartedEventArgs = new OperationStartedEventArgs(operationMock.Object);
            operationsStateServiceMock.Raise(m => m.OperationStarted += null, operationStartedEventArgs);

            Assert.True(viewModel.AreAnyOperationsAvailable);

            var operationStateChangedEventArgs = new OperationStateChangedEventArgs(OperationState.Blocked);
            operationMock.Raise(m => m.StateChanged += null, operationStateChangedEventArgs);

            Assert.True(viewModel.AreAnyOperationsAvailable);

            var task = await Task.WhenAny(Task.Delay(1000), taskCompletionSource.Task);
            if (task != taskCompletionSource.Task)
            {
                taskCompletionSource.SetResult(false);
            }

            var result = await taskCompletionSource.Task;
            Assert.True(result);

            operationMock.Verify(m => m.ContinueAsync(options), Times.Once());
        }
    }
}