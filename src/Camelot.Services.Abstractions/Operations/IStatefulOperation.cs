using System;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.EventArgs;

namespace Camelot.Services.Abstractions.Operations;

public interface IStatefulOperation
{
    OperationState State { get; }

    event EventHandler<OperationStateChangedEventArgs> StateChanged;
}