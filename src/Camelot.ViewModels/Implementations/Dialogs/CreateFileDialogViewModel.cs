using System.Windows.Input;
using Camelot.Services.Abstractions;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Implementations.Dialogs.Results;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Camelot.ViewModels.Implementations.Dialogs;

public class CreateFileDialogViewModel : ParameterizedDialogViewModelBase<CreateFileDialogResult, CreateNodeNavigationParameter>
{
    private readonly INodeService _nodeService;
    private readonly IPathService _pathService;

    private string _directoryPath;

    [Reactive]
    public string FileName { get; set; }

    public ICommand CreateCommand { get; }

    public CreateFileDialogViewModel(
        INodeService nodeService,
        IPathService pathService)
    {
        _nodeService = nodeService;
        _pathService = pathService;

        var canCreate = this.WhenAnyValue(x => x.FileName,
            CheckIfNameIsValid);

        CreateCommand = ReactiveCommand.Create(CreateFile, canCreate);
    }

    public override void Activate(CreateNodeNavigationParameter navigationParameter) =>
        _directoryPath = navigationParameter.DirectoryPath;

    private void CreateFile() => Close(new CreateFileDialogResult(FileName));

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