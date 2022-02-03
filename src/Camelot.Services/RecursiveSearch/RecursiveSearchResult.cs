using System;
using System.Threading.Tasks;
using Camelot.Extensions;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.RecursiveSearch;

namespace Camelot.Services.RecursiveSearch;

public class RecursiveSearchResult : IRecursiveSearchResult, INodeFoundEventPublisher
{
    private readonly Func<INodeFoundEventPublisher, Task> _taskFactory;

    public Lazy<Task> Task => new(() => _taskFactory(this));

    public event EventHandler<NodeFoundEventArgs> NodeFoundEvent;

    public RecursiveSearchResult(Func<INodeFoundEventPublisher, Task> taskFactory)
    {
        _taskFactory = taskFactory;
    }

    public void RaiseNodeFoundEvent(string nodePath) =>
        NodeFoundEvent.Raise(this, new NodeFoundEventArgs(nodePath));
}