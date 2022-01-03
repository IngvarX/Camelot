using System;
using System.Collections.Generic;
using Camelot.Services.Abstractions.Models.EventArgs;

namespace Camelot.Services.Abstractions.Operations;

public interface IOperationsStateService
{
    IReadOnlyList<IOperation> ActiveOperations { get; }

    event EventHandler<OperationStartedEventArgs> OperationStarted;

    void AddOperation(IOperation operation);
}