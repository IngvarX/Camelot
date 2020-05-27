using System;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Models.EventArgs;

namespace Camelot.Services.Abstractions.Operations
{
    public interface IInternalOperation : IOperationBase
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}