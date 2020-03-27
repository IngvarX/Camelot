using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ApplicationDispatcher.Interfaces;
using Camelot.ViewModels;
using Camelot.ViewModels.Dialogs;
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
            var viewModelsAssembly = Assembly.GetAssembly(typeof(ViewModelBase));
            var viewModelTypes = viewModelsAssembly.GetTypes();
            var viewModelType = viewModelTypes.SingleOrDefault(t => t.Name == viewModelName);
            if (viewModelType is null)
            {
                throw new InvalidOperationException($"View model {viewModelName} was not found!");
            }

            var viewsAssembly = Assembly.GetExecutingAssembly();
            var viewTypes = viewsAssembly.GetTypes();
            var viewName = viewModelName.Replace("ViewModel", string.Empty);
            var viewType = viewTypes.SingleOrDefault(t => t.Name == viewName);
            if (viewType is null)
            {
                throw new InvalidOperationException($"View {viewName} was not found!");
            }

            var window = (DialogWindowBase<T>)Activator.CreateInstance(viewType);
            var viewModel = (DialogViewModelBase<T>)Activator.CreateInstance(viewModelType);
            window.DataContext = viewModel;

            return await ShowDialogAsync(window);
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