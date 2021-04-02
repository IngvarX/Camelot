using System;
using System.Threading.Tasks;
using Camelot.Extensions;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.EventArgs;

namespace Camelot.Services.Models
{
    public class RecursiveSearchResult : IRecursiveSearchResult
    {
        private readonly Func<RecursiveSearchResult, Task> _taskFactory;

        public Lazy<Task> Task => new Lazy<Task>(() => _taskFactory(this));

        public event EventHandler<NodeFoundEventArgs> NodeFoundEvent;

        public RecursiveSearchResult(Func<RecursiveSearchResult, Task> taskFactory)
        {
            _taskFactory = taskFactory;
        }

        public void RaiseNodeFoundEvent(NodeModelBase nodeModel) =>
            NodeFoundEvent.Raise(this, new NodeFoundEventArgs(nodeModel.FullPath));
    }
}