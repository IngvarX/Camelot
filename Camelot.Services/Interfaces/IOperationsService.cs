using System.Threading.Tasks;

namespace Camelot.Services.Interfaces
{
    public interface IOperationsService
    {
        void EditSelectedFiles();

        Task CopySelectedFilesAsync(string destinationDirectory);

        Task MoveSelectedFilesAsync(string destinationDirectory);

        void CreateDirectory(string directoryName);

        Task RemoveSelectedFilesAsync();
    }
}