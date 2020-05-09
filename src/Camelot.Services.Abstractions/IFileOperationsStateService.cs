using System;
using System.Collections.Generic;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Operations;

namespace Camelot.Services.Abstractions
{
    public interface IFileOperationsStateService
    {
        IReadOnlyCollection<IOperation> ActiveOperations { get; }

        event EventHandler<OperationStartedEventArgs> OperationStarted;

        event EventHandler<OperationFinishedEventArgs> OperationFinished;

        void AddOperation(IOperation operation);
    }
}