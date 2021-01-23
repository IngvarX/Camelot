using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia;
using Camelot.Avalonia.Interfaces;
using Camelot.DependencyInjection;
using Camelot.ViewModels.Implementations;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Services;
using Camelot.ViewModels.Services.Interfaces;
using Camelot.Views;
using Camelot.Views.Dialogs;
using Splat;

namespace Camelot.Services.Implementations
{
    public class DialogService : IDialogService
    {
        private readonly IMainWindowProvider _mainWindowProvider;

        public DialogService(IMainWindowProvider mainWindowProvider)
        {
            _mainWindowProvider = mainWindowProvider;
        }

        public async Task<TResult> ShowDialogAsync<TResult>(string viewModelName)
            where TResult : DialogResultBase
        {
            var window = CreateView<TResult>(viewModelName);
            var viewModel = CreateViewModel<TResult>(viewModelName);
            Bind(window, viewModel);

            return await ShowDialogAsync(window);
        }

        public async Task<TResult> ShowDialogAsync<TResult, TParameter>(string viewModelName, TParameter parameter)
            where TResult : DialogResultBase
            where TParameter : NavigationParameterBase
        {
            var window = CreateView<TResult>(viewModelName);
            var viewModel = CreateViewModel<TResult>(viewModelName);
            Bind(window, viewModel);

            switch (viewModel)
            {
                case ParameterizedDialogViewModelBase<TResult, TParameter> parameterizedDialogViewModelBase:
                    parameterizedDialogViewModelBase.Activate(parameter);
                    break;
                case ParameterizedDialogViewModelBaseAsync<TResult, TParameter> parameterizedDialogViewModelBaseAsync:
                    await parameterizedDialogViewModelBaseAsync.ActivateAsync(parameter);
                    break;
                default:
                    throw new InvalidOperationException(
                        $"{viewModel.GetType().FullName} doesn't support passing parameters!");
            }

            return await ShowDialogAsync(window);
        }

        public Task ShowDialogAsync(string viewModelName) => ShowDialogAsync<DialogResultBase>(viewModelName);

        public Task ShowDialogAsync<TParameter>(string viewModelName, TParameter parameter)
            where TParameter : NavigationParameterBase =>
                ShowDialogAsync<DialogResultBase, TParameter>(viewModelName, parameter);

        private static void Bind(IDataContextProvider window, object viewModel) => window.DataContext = viewModel;

        private static DialogWindowBase<TResult> CreateView<TResult>(string viewModelName)
            where TResult : DialogResultBase
        {
            var viewType = GetViewType(viewModelName);
            if (viewType is null)
            {
                throw new InvalidOperationException($"View for {viewModelName} was not found!");
            }

            return (DialogWindowBase<TResult>) GetView(viewType);
        }

        private static DialogViewModelBase<TResult> CreateViewModel<TResult>(string viewModelName)
            where TResult : DialogResultBase
        {
            var viewModelType = GetViewModelType(viewModelName);
            if (viewModelType is null)
            {
                throw new InvalidOperationException($"View model {viewModelName} was not found!");
            }

            return (DialogViewModelBase<TResult>) GetViewModel(viewModelType);
        }

        private static Type GetViewModelType(string viewModelName)
        {
            var viewModelsAssembly = Assembly.GetAssembly(typeof(ViewModelBase));
            if (viewModelsAssembly is null)
            {
                throw new InvalidOperationException("Broken installation!");
            }

            var viewModelTypes = viewModelsAssembly.GetTypes();

            return viewModelTypes.SingleOrDefault(t => t.Name == viewModelName);
        }

        private static object GetView(Type type) => Activator.CreateInstance(type);

        private static object GetViewModel(Type type) => Locator.Current.GetRequiredService(type);

        private static Type GetViewType(string viewModelName)
        {
            var viewsAssembly = Assembly.GetExecutingAssembly();
            var viewTypes = viewsAssembly.GetTypes();
            var viewName = viewModelName.Replace("ViewModel", string.Empty);

            return viewTypes.SingleOrDefault(t => t.Name == viewName);
        }

        private async Task<TResult> ShowDialogAsync<TResult>(DialogWindowBase<TResult> window)
            where TResult : DialogResultBase
        {
            var mainWindow = (MainWindow) _mainWindowProvider.GetMainWindow();

            mainWindow.ShowOverlay();
            var result = await window.ShowDialog<TResult>(mainWindow);
            mainWindow.HideOverlay();
            if (window is IDisposable disposable)
            {
                disposable.Dispose();
            }

            return result;
        }
    }
}