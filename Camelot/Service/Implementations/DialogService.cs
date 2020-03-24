using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ApplicationDispatcher.Interfaces;
using Avalonia;
using Avalonia.Controls;
using Camelot.Service.Interfaces;
using Camelot.ViewModels.Dialogs;
using Camelot.Views;
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

            return await ShowDialogAsync(window);
        }

        private async Task<T> ShowDialogAsync<T>(DialogWindowBase<T> window)
        {
            var mainWindow = (MainWindow)_mainWindowProvider.GetMainWindow();
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            mainWindow.ShowOverlay();
            var result = await window.ShowDialog<T>(mainWindow);
            mainWindow.HideOverlay();

            return result;
        }
    }
}