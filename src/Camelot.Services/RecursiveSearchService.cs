using System;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Extensions;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Specifications;

namespace Camelot.Services
{
    public class RecursiveSearchService : IRecursiveSearchService
    {
        private readonly IDirectoryService _directoryService;
        private readonly IFileService _fileService;

        public RecursiveSearchService(
            IDirectoryService directoryService,
            IFileService fileService)
        {
            _directoryService = directoryService;
            _fileService = fileService;
        }

        public IRecursiveSearchResult Search(string directory, ISpecification<NodeModelBase> specification,
            CancellationToken cancellationToken)
        {
            Task TaskFactory(RecursiveSearchResult r) =>
                Task.Run(() =>
                {
                    foreach (var dir in _directoryService.GetDirectoriesRecursively(directory))
                    {
                        var model = _directoryService.GetDirectory(dir);
                        if (specification.IsSatisfiedBy(model))
                        {
                            r.RaiseNodeFoundEvent(model);
                        }
                    }

                    foreach (var file in _directoryService.GetFilesRecursively(directory))
                    {
                        var model = _fileService.GetFile(file);
                        if (specification.IsSatisfiedBy(model))
                        {
                            r.RaiseNodeFoundEvent(model);
                        }
                    }
                }, cancellationToken);

            return new RecursiveSearchResult(TaskFactory);
        }

        private class RecursiveSearchResult : IRecursiveSearchResult
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
}