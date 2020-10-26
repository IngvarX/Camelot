using System.Threading.Tasks;

namespace Camelot.ViewModels.Services.Interfaces
{
    public interface ISystemDialogService
    {
        Task<string> GetDirectoryAsync(string initialDirectory = null);

        Task<string> GetFileAsync(string initialFile = null);
    }
}