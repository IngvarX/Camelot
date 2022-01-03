using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using ReactiveUI.Fody.Helpers;

namespace Camelot.ViewModels.Implementations.Dialogs;

public class AccessDeniedDialogViewModel : ParameterizedDialogViewModelBase<AccessDeniedNavigationParameter>
{
    [Reactive]
    public string Directory { get; set; }

    public override void Activate(AccessDeniedNavigationParameter parameter)
    {
        Directory = parameter.Directory;
    }
}