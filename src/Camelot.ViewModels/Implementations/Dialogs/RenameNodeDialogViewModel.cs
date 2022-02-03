using System.Windows.Input;
using Camelot.Services.Abstractions;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Implementations.Dialogs.Results;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Camelot.ViewModels.Implementations.Dialogs;

public class RenameNodeDialogViewModel : ParameterizedDialogViewModelBase<RenameNodeDialogResult, RenameNodeNavigationParameter>
{
    private readonly INodeService _nodeService;
    private readonly IPathService _pathService;

    private string _directory;

    [Reactive]
    public string NodeName { get; set; }

    public ICommand RenameCommand { get; }

    public RenameNodeDialogViewModel(
        INodeService nodeService,
        IPathService pathService)
    {
        _nodeService = nodeService;
        _pathService = pathService;

        var canRename = this.WhenAnyValue(x => x.NodeName,
            CheckIfNameIsValid);

        RenameCommand = ReactiveCommand.Create(Rename, canRename);
    }

    public override void Activate(RenameNodeNavigationParameter parameter)
    {
        _directory = _pathService.GetParentDirectory(parameter.NodePath);
        NodeName = _pathService.GetFileName(parameter.NodePath);
    }

    private bool CheckIfNameIsValid(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        var fullPath = GetFullPath();

        return !_nodeService.CheckIfExists(fullPath);
    }

    private void Rename() => Close(new RenameNodeDialogResult(GetFullPath()));

    private string GetFullPath() => _pathService.Combine(_directory, NodeName);
}