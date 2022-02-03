namespace Camelot.Services.Abstractions.RecursiveSearch;

public interface INodeFoundEventPublisher
{
    void RaiseNodeFoundEvent(string nodePath);
}