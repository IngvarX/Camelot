using System;
using System.Collections.Generic;
using Camelot.Extensions;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Operations;

namespace Camelot.Services
{
    public class FileOperationsStateService : IFileOperationsStateService
    {
        private readonly List<IOperation> _activeOperations;

        public IReadOnlyCollection<IOperation> ActiveOperations => _activeOperations;

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
            operation.OperationFinished += OperationOnOperationFinished;
            operation.OperationCancelled += OperationOnOperationCancelled;
        }

        private void UnsubscribeFromOperationEvents(IOperation operation)
        {
            operation.OperationFinished -= OperationOnOperationFinished;
            operation.OperationCancelled -= OperationOnOperationCancelled;
        }

        private void OperationOnOperationFinished(object sender, EventArgs e)
        {
            var operation = (IOperation) sender;

            RemoveOperation(operation, OperationResult.Success);
        }

        private void OperationOnOperationCancelled(object sender, EventArgs e)
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