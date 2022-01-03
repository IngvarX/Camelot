using System;
using System.Threading.Tasks;

namespace Camelot.Services.Abstractions.RecursiveSearch;

public interface IRecursiveSearchResultFactory
{
    IRecursiveSearchResult Create(Func<INodeFoundEventPublisher, Task> taskFactory);
}