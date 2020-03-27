using System.Threading.Tasks;

namespace Camelot.ViewModels.Services.Interfaces
{
    public interface IDialogService
    {
        Task<T> ShowDialogAsync<T>(string viewModelName);
    }
}