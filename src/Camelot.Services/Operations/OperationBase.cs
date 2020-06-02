using System;
using Camelot.Extensions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Operations;

namespace Camelot.Services.Operations
{
    public abstract class OperationBase : IOperationWithProgress, IStatefulOperation
    {
        private OperationState _state;
        private double _progress;

        public OperationState State
        {
            get => _state;
            protected set
            {
                if (_state == value)
                {
                    return;
                }

                _state = value;
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