using Camelot.Services.Abstractions.Models;

namespace Camelot.ViewModels.Interfaces.Properties;

public interface IMainNodeInfoTabViewModel
{
    void Activate(NodeModelBase nodeModel, bool isDirectory = false, int innerFilesCount = 0,
        int innerDirectoriesCount = 0);

    void SetSize(long sizeBytes);
}