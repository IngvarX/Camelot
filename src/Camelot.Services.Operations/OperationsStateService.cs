using System;
using System.Collections.Generic;
using Camelot.Extensions;
using Camelot.Services.Abstractions.Extensions;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Operations;

namespace Camelot.Services.Operations;

public class OperationsStateService : IOperationsStateService
{
    private readonly List<IOperation> _activeOperations;

    public IReadOnlyList<IOperation> ActiveOperations => _activeOperations;

    public event EventHandler<OperationStartedEventArgs> OperationStarted;

    public OperationsStateService()
    {
        _activeOperations = new List<IOperation>();
    }

    public void AddOperation(IOperation operation)
    {
        SubscribeToOperationEvents(operation);

        _activeOperations.Add(operation);

        OperationStarted.Raise(this, new OperationStartedEventArgs(operation));
    }

    private void SubscribeToOperationEvents(IOperation operation)
    {
        operation.StateChanged += OperationOnStateChanged;
    }

    private void UnsubscribeFromOperationEvents(IOperation operation)
    {
        operation.StateChanged -= OperationOnStateChanged;
    }

    private void OperationOnStateChanged(object sender, OperationStateChangedEventArgs e)
    {
        var operation = (IOperation) sender;
        if (e.OperationState.IsCompleted())
        {
            RemoveOperation(operation);
        }
    }

    private void RemoveOperation(IOperation operation)
    {
        _activeOperations.Remove(operation);
        UnsubscribeFromOperationEvents(operation);
    }
}