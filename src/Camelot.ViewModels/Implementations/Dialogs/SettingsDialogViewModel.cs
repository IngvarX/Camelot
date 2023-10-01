using System.Linq;
using System.Windows.Input;
using Camelot.Extensions;
using Camelot.ViewModels.Interfaces.Settings;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.Dialogs;

public class SettingsDialogViewModel : DialogViewModelBase
{
    private readonly ISettingsViewModel[] _settingsViewModels;

    private int _selectedIndex;

    public ISettingsViewModel TerminalSettingsViewModel { get; set; }

    public ISettingsViewModel GeneralSettingsViewModel { get; set; }
    public ISettingsViewModel IconsSettingsViewModel { get; set; }

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

    public SettingsDialogViewModel(
        ISettingsViewModel generalSettingsViewModel,
        ISettingsViewModel terminalSettingsViewModel,
        ISettingsViewModel iconsSettingsViewModel)
    {
        TerminalSettingsViewModel = terminalSettingsViewModel;
        GeneralSettingsViewModel = generalSettingsViewModel;
        IconsSettingsViewModel = iconsSettingsViewModel;

        _settingsViewModels = new[]
        {
            generalSettingsViewModel,
            terminalSettingsViewModel,
            iconsSettingsViewModel
        };

        Activate(_settingsViewModels.First());

        SaveCommand = ReactiveCommand.Create(Save);
    }

    private void Save() =>
        _settingsViewModels
            .Where(vm => vm.IsChanged)
            .ForEach(vm => vm.SaveChanges());

    private static void Activate(ISettingsViewModel settingsViewModel) =>
        settingsViewModel.Activate();
}