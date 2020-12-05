using Camelot.Services.Windows.Enums;

namespace Camelot.Services.Windows.Interfaces
{
    public interface IRegistryService
    {
        IRegistryKey GetRegistryKey(RootRegistryKey rootRegistryKey);
    }
}