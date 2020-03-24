using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ApplicationDispatcher.Interfaces;
using Camelot.Service.Interfaces;
using Camelot.ViewModels.Dialogs;
using Camelot.Views.Main.Dialogs;

namespace Camelot.Service.Implementations
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
            var currentAssembly = Assembly.GetExecutingAssembly();

            var viewModelType = currentAssembly.GetTypes().SingleOrDefault(t => t.Name == viewModelName);
            if (viewModelType is null)
            {
                throw new InvalidOperationException($"View model {viewModelName} was not found!");
            }

            var viewName = viewModelName.Replace("ViewModel", string.Empty);
            var viewType = currentAssembly.GetTypes().SingleOrDefault(t => t.Name == viewName);
            if (viewType is null)
            {
                throw new InvalidOperationException($"View {viewName} was not found!");
            }

            var window = (DialogWindowBase<T>)Activator.CreateInstance(viewType);
            var viewModel = (DialogViewModelBase<T>)Activator.CreateInstance(viewModelType);
            window.DataContext = viewModel;

            var mainWindow = _mainWindowProvider.GetMainWindow();

            return await window.ShowDialog<T>(mainWindow);
        }
    }
}