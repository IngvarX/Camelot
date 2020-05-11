using System;
using System.Collections.Generic;
using Camelot.Services.Abstractions.Models.EventArgs;

namespace Camelot.Services.Abstractions.Operations
{
    public interface IFileOperationsStateService
    {
        IReadOnlyCollection<IOperation> ActiveOperations { get; }

        event EventHandler<OperationStartedEventArgs> OperationStarted;

        event EventHandler<OperationFinishedEventArgs> OperationFinished;

        void AddOperation(IOperation operation);
    }
}