using System;
using Camelot.Extensions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Operations;

namespace Camelot.Services.Operations;

public abstract class StatefulOperationWithProgressBase : OperationWithProgressBase, IStatefulOperation
{
    private readonly object _stateLocker;

    private OperationState _state;

    public OperationState State
    {
        get
        {
            lock (_stateLocker)
            {
                return _state;
            }
        }
        protected set
        {
            lock (_stateLocker)
            {
                if (_state == value)
                {
                    return;
                }

                _state = value;
            }

            var args = new OperationStateChangedEventArgs(value);

            StateChanged.Raise(this, args);
        }
    }

    public event EventHandler<OperationStateChangedEventArgs> StateChanged;

    protected StatefulOperationWithProgressBase()
    {
        _stateLocker = new object();
    }
}