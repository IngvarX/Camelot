using System;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.RecursiveSearch;
using Camelot.Services.Models;

namespace Camelot.Services
{
    public class RecursiveSearchResultFactory : IRecursiveSearchResultFactory
    {
        public IRecursiveSearchResult Create(Func<INodeFoundEventPublisher, Task> taskFactory) =>
            new RecursiveSearchResult(taskFactory);
    }
}