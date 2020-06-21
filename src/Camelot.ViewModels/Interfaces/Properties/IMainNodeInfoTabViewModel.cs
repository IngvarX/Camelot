using Camelot.Services.Abstractions.Models;

namespace Camelot.ViewModels.Interfaces.Properties
{
    public interface IMainNodeInfoTabViewModel
    {
        void Activate(NodeModelBase nodeModel, bool isDirectory = false);

        void SetSize(long sizeBytes);
    }
}