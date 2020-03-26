using System.Threading.Tasks;

namespace Camelot.Service.Interfaces
{
    public interface IDialogService
    {
        Task<T> ShowDialogAsync<T>(string viewModelName);
    }
}