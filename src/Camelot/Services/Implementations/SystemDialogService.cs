using System.Threading.Tasks;
using Avalonia.Controls;
using Camelot.Avalonia.Interfaces;
using Camelot.ViewModels.Services.Interfaces;

namespace Camelot.Services.Implementations
{
    public class SystemDialogService : ISystemDialogService
    {
        private readonly IMainWindowProvider _mainWindowProvider;

        public SystemDialogService(IMainWindowProvider mainWindowProvider)
        {
            _mainWindowProvider = mainWindowProvider;
        }

        public async Task<string> GetDirectoryAsync(string initialDirectory = null)
        {
            var dialog = new OpenFolderDialog {Directory = initialDirectory};
            var window = _mainWindowProvider.GetMainWindow();

            return await dialog.ShowAsync(window);
        }

        public async Task<string> GetFileAsync(string initialFile = null)
        {
            var dialog = new SaveFileDialog {InitialFileName = initialFile};
            var window = _mainWindowProvider.GetMainWindow();

            return await dialog.ShowAsync(window);
        }
    }
}