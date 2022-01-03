using Camelot.Services.Abstractions.Models;

namespace Camelot.Services.Abstractions;

public interface INodeService
{
    bool CheckIfExists(string nodePath);

    NodeModelBase GetNode(string nodePath);
}