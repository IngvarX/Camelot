using System;
using Camelot.Extensions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Operations;

namespace Camelot.Services.Operations
{
    public abstract class OperationBase : IOperationBase
    {
        private OperationState _operationState;
        private double _progress;

        public OperationState OperationState
        {
            get => _operationState;
            protected set
            {
                if (_operationState == value)
                {
                    return;
                }

                _operationState = value;
                var args = new OperationStateChangedEventArgs(value);
                StateChanged.Raise(this, args);
            }
        }

        public double CurrentProgress
        {
            get => _progress;
            protected set
            {
                _progress = value;
                var args = new OperationProgressChangedEventArgs(_progress);
                ProgressChanged.Raise(this, args);
            }
        }

        public event EventHandler<OperationProgressChangedEventArgs> ProgressChanged;

        public event EventHandler<OperationStateChangedEventArgs> StateChanged;
    }
}