using System;
using Camelot.Extensions;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Operations;

namespace Camelot.Services.Operations;

public class OperationWithProgressBase : IOperationWithProgress
{
    private double _progress;

    public double CurrentProgress
    {
        get => _progress;
        protected set
        {
            if (Math.Abs(_progress - value) < 1e-5)
            {
                return;
            }

            _progress = value;
            var args = new OperationProgressChangedEventArgs(_progress);
            ProgressChanged.Raise(this, args);
        }
    }

    public event EventHandler<OperationProgressChangedEventArgs> ProgressChanged;

    protected void SetFinalProgress() => CurrentProgress = 1;
}