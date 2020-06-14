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

            var canSave = this.WhenAnyValue(x => x._settingsViewModels,
                vms => vms.Any(vm => vm.IsChanged));
            SaveCommand = ReactiveCommand.Create(Save, canSave);
            CloseCommand = ReactiveCommand.Create(Close);
            OpenTerminalSettingsCommand = ReactiveCommand.Create(() => CurrentSettingsViewModel = terminalSettingsViewModel);

            _settingsViewModels.First().Activate();
        }

        private void Save() =>
            _settingsViewModels
                .Where(vm => vm.IsChanged)
                .ForEach(vm => vm.SaveChanges());
    }
}