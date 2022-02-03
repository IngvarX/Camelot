using System;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.RecursiveSearch;

namespace Camelot.Services.RecursiveSearch;

public class RecursiveSearchResultFactory : IRecursiveSearchResultFactory
{
    public IRecursiveSearchResult Create(Func<INodeFoundEventPublisher, Task> taskFactory) =>
        new RecursiveSearchResult(taskFactory);
}