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

    public ISettingsViewModel AppearanceSettingsViewModel { get; set; }
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
        ISettingsViewModel appearanceSettingsViewModel,
        ISettingsViewModel terminalSettingsViewModel,
        ISettingsViewModel iconsSettingsViewModel)
    {
        TerminalSettingsViewModel = terminalSettingsViewModel;
        GeneralSettingsViewModel = generalSettingsViewModel;
        IconsSettingsViewModel = iconsSettingsViewModel;
        AppearanceSettingsViewModel = appearanceSettingsViewModel;
        // Items in next array should be in same order as 'tabs' in xaml,
        // Otherwise, Activate will called for wrong model.
        // TODO: need to make it more dynamic, and not rely on order in view.
        _settingsViewModels = new[]
        {
            generalSettingsViewModel,
            appearanceSettingsViewModel,
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