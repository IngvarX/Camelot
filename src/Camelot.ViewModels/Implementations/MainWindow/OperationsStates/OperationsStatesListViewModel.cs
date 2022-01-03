using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Avalonia.Interfaces;
using Camelot.Collections;
using Camelot.Extensions;
using Camelot.Services.Abstractions.Extensions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Operations;
using Camelot.ViewModels.Configuration;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Implementations.Dialogs.Results;
using Camelot.ViewModels.Interfaces.MainWindow.OperationsStates;
using Camelot.ViewModels.Services.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Camelot.ViewModels.Implementations.MainWindow.OperationsStates;

public class OperationsStatesListViewModel : ViewModelBase, IOperationsStateViewModel
{
    private readonly IOperationsStateService _operationsStateService;
    private readonly IOperationStateViewModelFactory _operationStateViewModelFactory;
    private readonly IApplicationDispatcher _applicationDispatcher;
    private readonly IDialogService _dialogService;

    private readonly ObservableCollection<IOperationStateViewModel> _activeOperations;
    private readonly LimitedSizeStack<IOperationStateViewModel> _finishedOperationsQueue;
    private readonly IDictionary<IOperation, IOperationStateViewModel> _operationsViewModelsDictionary;

    [Reactive]
    public int TotalProgress  { get; set; }

    [Reactive]
    public bool AreAnyOperationsAvailable { get; set; }

    [Reactive]
    public bool IsLastOperationSuccessful { get; set; }

    public bool IsInProgress => ActiveOperations.Any();

    public IEnumerable<IOperationStateViewModel> ActiveOperations => _activeOperations;

    public IEnumerable<IOperationStateViewModel> InactiveOperations => _finishedOperationsQueue.Items.Reverse();

    public OperationsStatesListViewModel(
        IOperationsStateService operationsStateService,
        IOperationStateViewModelFactory operationStateViewModelFactory,
        IApplicationDispatcher applicationDispatcher,
        IDialogService dialogService,
        OperationsStatesConfiguration configuration)
    {
        _operationsStateService = operationsStateService;
        _operationStateViewModelFactory = operationStateViewModelFactory;
        _applicationDispatcher = applicationDispatcher;
        _dialogService = dialogService;

        _activeOperations = new ObservableCollection<IOperationStateViewModel>();
        _finishedOperationsQueue = new LimitedSizeStack<IOperationStateViewModel>(configuration.MaximumFinishedOperationsCount);
        _operationsViewModelsDictionary = new ConcurrentDictionary<IOperation, IOperationStateViewModel>();

        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        _activeOperations.CollectionChanged += (_, _) => this.RaisePropertyChanged(nameof(IsInProgress));
        _operationsStateService.OperationStarted += OperationsStateServiceOnOperationStarted;
    }

    private void OperationsStateServiceOnOperationStarted(object sender, OperationStartedEventArgs e) =>
        _applicationDispatcher.Dispatch(() => AddOperation(e.Operation));

    private void AddOperation(IOperation operation)
    {
        var viewModel = CreateFrom(operation);
        _activeOperations.Add(viewModel);
        _operationsViewModelsDictionary[operation] = viewModel;

        AreAnyOperationsAvailable = true;
        UpdateOperationStatus(operation);
        SubscribeToEvents(operation);
    }

    private void RemoveOperation(IOperation operation)
    {
        UnsubscribeFromEvents(operation);

        var isKeyAvailable = _operationsViewModelsDictionary.TryGetValue(operation, out var viewModel);
        if (!isKeyAvailable)
        {
            return;
        }

        _activeOperations.Remove(viewModel);
        _operationsViewModelsDictionary.Remove(operation);

        AddFinishedOperationViewModel(viewModel);
    }

    private void AddFinishedOperationViewModel(IOperationStateViewModel stateViewModel)
    {
        _finishedOperationsQueue.Push(stateViewModel);
        IsLastOperationSuccessful = !stateViewModel.State.IsFailedOrCancelled();
        this.RaisePropertyChanged(nameof(InactiveOperations));
    }

    private void SubscribeToEvents(IOperation operation)
    {
        operation.StateChanged += OperationOnStateChanged;
        operation.ProgressChanged += OperationOnProgressChanged;
    }

    private void UnsubscribeFromEvents(IOperation operation)
    {
        operation.StateChanged -= OperationOnStateChanged;
        operation.ProgressChanged -= OperationOnProgressChanged;
    }

    private void OperationOnStateChanged(object sender, OperationStateChangedEventArgs e) =>
        UpdateOperationStatus((IOperation) sender);

    private void UpdateOperationStatus(IOperation operation)
    {
        if (operation.State.IsCompleted())
        {
            _applicationDispatcher.Dispatch(() =>
            {
                RemoveOperation(operation);
                UpdateProgress();
            });
        }

        if (operation.State == OperationState.Blocked)
        {
            _applicationDispatcher.DispatchAsync(() => ProcessBlockedOperationAsync(operation)).Forget();
        }

        // TODO: change status
    }

    private void OperationOnProgressChanged(object sender, OperationProgressChangedEventArgs e) =>
        UpdateProgress();

    private void UpdateProgress()
    {
        var activeOperations = GetActiveOperations();
        if (!activeOperations.Any())
        {
            TotalProgress = default;

            return;
        }

        var averageProgress = activeOperations.Average(o => o.CurrentProgress);
        TotalProgress = (int) (averageProgress * 100);
    }

    private async Task ProcessBlockedOperationAsync(IOperation operation)
    {
        var (sourceFilePath, destinationFilePath) = operation.CurrentBlockedFile;
        var areMultipleFilesAvailable = operation.Info.TotalFilesCount > 1;
        var navigationParameter = new OverwriteOptionsNavigationParameter(
            sourceFilePath, destinationFilePath, areMultipleFilesAvailable);

        var dialogResult = await _dialogService.ShowDialogAsync<OverwriteOptionsDialogResult, OverwriteOptionsNavigationParameter>(
            nameof(OverwriteOptionsDialogViewModel), navigationParameter);

        if (dialogResult is null)
        {
            Task.Run(operation.CancelAsync).Forget();
        }
        else
        {
            Task.Run(() => operation.ContinueAsync(dialogResult.Options)).Forget();
        }
    }

    private IOperation[] GetActiveOperations() =>
        _operationsStateService.ActiveOperations.ToArray();

    private IOperationStateViewModel CreateFrom(IOperation operation) =>
        _operationStateViewModelFactory.Create(operation);
}