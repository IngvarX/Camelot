using System.Windows.Input;
using Camelot.Services.Abstractions;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Implementations.Dialogs.Results;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Camelot.ViewModels.Implementations.Dialogs;

public class CreateDirectoryDialogViewModel : ParameterizedDialogViewModelBase<CreateDirectoryDialogResult, CreateNodeNavigationParameter>
{
    private readonly INodeService _nodeService;
    private readonly IPathService _pathService;

    private string _directoryPath;

    [Reactive]
    public string DirectoryName { get; set; }

    public ICommand CreateCommand { get; }

    public CreateDirectoryDialogViewModel(
        INodeService nodeService,
        IPathService pathService)
    {
        _nodeService = nodeService;
        _pathService = pathService;

        var canCreate = this.WhenAnyValue(x => x.DirectoryName,
            CheckIfNameIsValid);

        CreateCommand = ReactiveCommand.Create(CreateDirectory, canCreate);
    }

    public override void Activate(CreateNodeNavigationParameter navigationParameter) =>
        _directoryPath = navigationParameter.DirectoryPath;

    private void CreateDirectory() => Close(new CreateDirectoryDialogResult(DirectoryName));

    private bool CheckIfNameIsValid(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        var newFullPath = _pathService.Combine(_directoryPath, name);

        return !_nodeService.CheckIfExists(newFullPath);
    }
}