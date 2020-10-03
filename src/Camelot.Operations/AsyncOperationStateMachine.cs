using System;
using System.Threading.Tasks;
using Camelot.Extensions;
using Camelot.Services.Abstractions.Exceptions;
using Camelot.Services.Abstractions.Extensions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Models.Operations;
using Camelot.Services.Abstractions.Operations;
using Microsoft.Extensions.Logging;

namespace Camelot.Operations
{
    public class AsyncOperationStateMachine : IOperation
    {
        private readonly ICompositeOperation _compositeOperation;
        private readonly ILogger _logger;

        private OperationState _operationState;

        public OperationState State
        {
            get => _operationState;
            private set
            {
                _operationState = value;

                var args = new OperationStateChangedEventArgs(State);
                StateChanged.Raise(this, args);
            }
        }

        public OperationInfo Info  => _compositeOperation.Info;

        public (string SourceFilePath, string DestinationFilePath) CurrentBlockedFile =>
            _compositeOperation.CurrentBlockedFile;

        public double CurrentProgress => _compositeOperation.CurrentProgress;

        public event EventHandler<OperationStateChangedEventArgs> StateChanged;

        public event EventHandler<OperationProgressChangedEventArgs> ProgressChanged
        {
            add => _compositeOperation.ProgressChanged += value;
            remove => _compositeOperation.ProgressChanged -= value;
        }

        public AsyncOperationStateMachine(
            ICompositeOperation compositeOperation,
            ILogger logger)
        {
            _compositeOperation = compositeOperation;
            _logger = logger;

            SubscribeToEvents();
        }

        public Task RunAsync() =>
            ChangeStateAsync(OperationState.NotStarted, OperationState.InProgress);

        public Task ContinueAsync(OperationContinuationOptions options) =>
            ChangeStateAsync(OperationState.Blocked, OperationState.InProgress, options);

        public Task PauseAsync() =>
            ChangeStateAsync(OperationState.InProgress, OperationState.Pausing);

        public Task UnpauseAsync() =>
            ChangeStateAsync(OperationState.Paused, OperationState.Unpausing);

        public Task CancelAsync() =>
            ChangeStateAsync(State, OperationState.Cancelling);

        private async Task ChangeStateAsync(
            OperationState expectedState,
            OperationState requestedState,
            OperationContinuationOptions options = null)
        {
            var taskFactory = (State, requestedState) switch
            {
                _ when State == requestedState => GetCompletedTask,

                _ when State != expectedState =>
                    throw new InvalidOperationException($"Inner state {State} is not {expectedState}"),

                _ when State == requestedState =>
                    throw new InvalidOperationException($"Inner state {State} is the same as requested state"),

                (OperationState.NotStarted, OperationState.InProgress) =>
                    WrapAsync(_compositeOperation.RunAsync, OperationState.InProgress, OperationState.Finished),

                _ when (State.IsCancellationAvailable() || State is OperationState.Blocked) && requestedState is OperationState.Cancelling =>
                    WrapAsync(_compositeOperation.CancelAsync, OperationState.Cancelling, OperationState.Cancelled),

                (OperationState.InProgress, OperationState.Failed) => GetCompletedTask, // TODO: cleanup?

                (OperationState.InProgress, OperationState.Pausing) =>
                    WrapAsync(_compositeOperation.PauseAsync, OperationState.Pausing, OperationState.Paused),

                (OperationState.Blocked, OperationState.InProgress) when options is null =>
                    throw new ArgumentNullException(nameof(options)),

                (OperationState.Blocked, OperationState.InProgress) => () => _compositeOperation.ContinueAsync(options),

                (OperationState.Paused, OperationState.InProgress) =>
                    WrapAsync(_compositeOperation.UnpauseAsync, OperationState.InProgress, OperationState.Finished),

                (OperationState.Cancelling, OperationState.Cancelled) => GetCompletedTask,
                (OperationState.InProgress, OperationState.Finished) => GetCompletedTask,
                (OperationState.InProgress, OperationState.Blocked) => GetCompletedTask,
                (OperationState.Unpausing, OperationState.InProgress) => GetCompletedTask,
                (OperationState.Pausing, OperationState.Paused) => GetCompletedTask,

                _ => throw new InvalidOperationException($"{State} has no transition to {requestedState}")
            };

            State = requestedState;

            await taskFactory();
        }

        private Func<Task> WrapAsync(Func<Task> taskFactory,
            OperationState expectedState, OperationState requestedState) =>
            async () =>
            {
                try
                {
                    await taskFactory();
                    if (State == expectedState)
                    {
                        await ChangeStateAsync(expectedState, requestedState);
                    }
                }
                catch (OperationFailedException ex)
                {
                    _logger.LogError(
                        $"{nameof(AsyncOperationStateMachine)} {nameof(OperationFailedException)} occurred: {ex}");

                    await ChangeStateAsync(State, OperationState.Failed);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        $"{nameof(AsyncOperationStateMachine)} {nameof(Exception)} occurred: {ex}");
                }
            };

        private static Task GetCompletedTask() => Task.CompletedTask;

        private void SubscribeToEvents() => _compositeOperation.Blocked += CompositeOperationOnBlocked;

        private async void CompositeOperationOnBlocked(object sender, EventArgs e) =>
            await ChangeStateAsync(OperationState.InProgress, OperationState.Blocked);
    }
}