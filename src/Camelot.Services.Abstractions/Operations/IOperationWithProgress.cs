using System;
using Camelot.Services.Abstractions.Models.EventArgs;

namespace Camelot.Services.Abstractions.Operations;

public interface IOperationWithProgress
{
    double CurrentProgress { get; }

    event EventHandler<OperationProgressChangedEventArgs> ProgressChanged;
}