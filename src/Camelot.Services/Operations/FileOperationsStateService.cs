using System;
using System.Collections.Generic;
using Camelot.Extensions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Operations;

namespace Camelot.Services.Operations
{
    public class FileOperationsStateService : IFileOperationsStateService
    {
        private readonly List<IOperation> _activeOperations;

        public IReadOnlyList<IOperation> ActiveOperations => _activeOperations;

        public event EventHandler<OperationStartedEventArgs> OperationStarted;

        public event EventHandler<OperationFinishedEventArgs> OperationFinished;

        public FileOperationsStateService()
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
            operation.Cancelled += OnCancelled;
        }

        private void UnsubscribeFromOperationEvents(IOperation operation)
        {
            operation.StateChanged -= OperationOnStateChanged;
            operation.Cancelled -= OnCancelled;
        }

        private void OperationOnStateChanged(object sender, OperationStateChangedEventArgs e)
        {
            var operation = (IOperation) sender;
            if (e.OperationState == OperationState.Finished)
            {
                RemoveOperation(operation, OperationResult.Success);
            }
        }

        private void OnCancelled(object sender, EventArgs e)
        {
            var operation = (IOperation) sender;

            RemoveOperation(operation, OperationResult.Cancelled);
        }

        private void RemoveOperation(IOperation operation, OperationResult operationResult)
        {
            _activeOperations.Remove(operation);
            UnsubscribeFromOperationEvents(operation);

            var args = new OperationFinishedEventArgs(operation, operationResult);
            OperationFinished.Raise(this, args);
        }
    }
}