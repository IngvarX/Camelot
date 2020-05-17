using System.Threading.Tasks;

namespace Camelot.ViewModels.Services.Interfaces
{
    public interface IDialogService
    {
        Task<TResult> ShowDialogAsync<TResult>(string viewModelName);
        
        Task ShowDialogAsync(string viewModelName);
        
        Task ShowDialogAsync<TParameter>(string viewModelName, TParameter parameter)
            where TParameter : NavigationParameterBase;
        
        Task<TResult> ShowDialogAsync<TResult, TParameter>(string viewModelName, TParameter parameter)
            where TParameter : NavigationParameterBase;
    }
}