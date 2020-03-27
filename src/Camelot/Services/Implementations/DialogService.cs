using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ApplicationDispatcher.Interfaces;
using Camelot.ViewModels;
using Camelot.ViewModels.Implementations;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Services.Interfaces;
using Camelot.Views;
using Camelot.Views.Main.Dialogs;

namespace Camelot.Services.Implementations
{
    public class DialogService : IDialogService
    {
        private readonly IMainWindowProvider _mainWindowProvider;

        public DialogService(IMainWindowProvider mainWindowProvider)
        {
            _mainWindowProvider = mainWindowProvider;
        }

        public async Task<T> ShowDialogAsync<T>(string viewModelName)
        {
            var viewModelType = GetViewModelType(viewModelName);
            if (viewModelType is null)
            {
                throw new InvalidOperationException($"View model {viewModelName} was not found!");
            }

            var viewType = GetViewType(viewModelName);
            if (viewType is null)
            {
                throw new InvalidOperationException($"View for {viewModelName} was not found!");
            }

            var window = (DialogWindowBase<T>)Activator.CreateInstance(viewType);
            var viewModel = (DialogViewModelBase<T>)Activator.CreateInstance(viewModelType);
            window.DataContext = viewModel;

            return await ShowDialogAsync(window);
        }

        private static Type GetViewModelType(string viewModelName)
        {
            var viewModelsAssembly = Assembly.GetAssembly(typeof(ViewModelBase));
            var viewModelTypes = viewModelsAssembly.GetTypes();
            
            return viewModelTypes.SingleOrDefault(t => t.Name == viewModelName);
        }
        
        private static Type GetViewType(string viewModelName)
        {
            var viewsAssembly = Assembly.GetExecutingAssembly();
            var viewTypes = viewsAssembly.GetTypes();
            var viewName = viewModelName.Replace("ViewModel", string.Empty);
            
            return viewTypes.SingleOrDefault(t => t.Name == viewName);
        }

        private async Task<T> ShowDialogAsync<T>(DialogWindowBase<T> window)
        {
            var mainWindow = (MainWindow)_mainWindowProvider.GetMainWindow();
            window.Owner = mainWindow;

            mainWindow.ShowOverlay();
            var result = await window.ShowDialog<T>(mainWindow);
            mainWindow.HideOverlay();

            return result;
        }
    }
}