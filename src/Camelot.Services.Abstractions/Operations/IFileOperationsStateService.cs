using System;
using System.Collections.Generic;
using Camelot.Services.Abstractions.Models.EventArgs;

namespace Camelot.Services.Abstractions.Operations
{
    public interface IFileOperationsStateService
    {
        IReadOnlyList<IOperation> ActiveOperations { get; }

        event EventHandler<OperationStartedEventArgs> OperationStarted;

        event EventHandler<OperationFinishedEventArgs> OperationFinished;

        void AddOperation(IOperation operation);
    }
}