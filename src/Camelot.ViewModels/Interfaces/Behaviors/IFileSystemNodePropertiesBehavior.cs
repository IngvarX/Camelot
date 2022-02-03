using System.Threading.Tasks;

namespace Camelot.ViewModels.Interfaces.Behaviors;

public interface IFileSystemNodePropertiesBehavior
{
    Task ShowPropertiesAsync(string directoryPath);
}