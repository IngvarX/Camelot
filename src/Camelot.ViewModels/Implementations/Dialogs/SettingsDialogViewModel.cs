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

        private ISettingsViewModel _currentSettingsViewModel;

        public ISettingsViewModel CurrentSettingsViewModel
        {
            get => _currentSettingsViewModel;
            set
            {
                this.RaiseAndSetIfChanged(ref _currentSettingsViewModel, value);
                value.Activate();
            }
        }

        public ICommand OpenTerminalSettingsCommand { get; }

        public ICommand SaveCommand { get; }

        public ICommand CloseCommand { get; }

        public SettingsDialogViewModel(
            ISettingsViewModel terminalSettingsViewModel)
        {
            _settingsViewModels = new[]
            {
                terminalSettingsViewModel
            };

            CurrentSettingsViewModel = _settingsViewModels.First();

            OpenTerminalSettingsCommand = ReactiveCommand.Create(() => CurrentSettingsViewModel = terminalSettingsViewModel);
            SaveCommand = ReactiveCommand.Create(Save);
            CloseCommand = ReactiveCommand.Create(Close);
        }

        private void Save() =>
            _settingsViewModels
                .Where(vm => vm.IsChanged)
                .ForEach(vm => vm.SaveChanges());
    }
}