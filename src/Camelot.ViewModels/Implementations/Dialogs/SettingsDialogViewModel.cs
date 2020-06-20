using System.Linq;
using System.Windows.Input;
using Camelot.Extensions;
using Camelot.ViewModels.Interfaces.Settings;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.Dialogs
{
    public class SettingsDialogViewModel : DialogViewModelBase
    {
        private readonly ISettingsViewModel[] _settingsViewModels;

        private ISettingsViewModel _terminalSettingsViewModel;
        private int _selectedIndex;

        public ISettingsViewModel TerminalSettingsViewModel
        {
            get => _terminalSettingsViewModel;
            set => this.RaiseAndSetIfChanged(ref _terminalSettingsViewModel, value);
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedIndex, value);
                Activate(_settingsViewModels[_selectedIndex]);
            }
        }

        public ICommand SaveCommand { get; }

        public ICommand CloseCommand { get; }

        public SettingsDialogViewModel(
            ISettingsViewModel terminalSettingsViewModel)
        {
            TerminalSettingsViewModel = terminalSettingsViewModel;

            _settingsViewModels = new[]
            {
                terminalSettingsViewModel
            };
            Activate(_settingsViewModels.First());

            SaveCommand = ReactiveCommand.Create(Save);
            CloseCommand = ReactiveCommand.Create(Close);
        }

        private void Save() =>
            _settingsViewModels
                .Where(vm => vm.IsChanged)
                .ForEach(vm => vm.SaveChanges());

        private static void Activate(ISettingsViewModel settingsViewModel) =>
            settingsViewModel.Activate();
    }
}