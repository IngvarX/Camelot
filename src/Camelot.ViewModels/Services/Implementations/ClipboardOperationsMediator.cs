using System.Threading.Tasks;
using System.Windows.Input;
using Camelot.Services.Abstractions;
using Camelot.ViewModels.Implementations;
using Camelot.ViewModels.Services.Interfaces;
using ReactiveUI;

namespace Camelot.ViewModels.Services.Implementations;

public class ClipboardOperationsMediator : ViewModelBase, IClipboardOperationsMediator
{
    private readonly IClipboardOperationsService _clipboardOperationsService;
    private readonly INodesSelectionService _nodesSelectionService;
    private readonly IDirectoryService _directoryService;

    public ICommand CopyToClipboardCommand { get; }

    public ICommand PasteFromClipboardCommand { get; }

    public ClipboardOperationsMediator(
        IClipboardOperationsService clipboardOperationsService,
        INodesSelectionService nodesSelectionService,
        IDirectoryService directoryService)
    {
        _clipboardOperationsService = clipboardOperationsService;
        _nodesSelectionService = nodesSelectionService;
        _directoryService = directoryService;

        CopyToClipboardCommand = ReactiveCommand.CreateFromTask(CopyToClipboardAsync);
        PasteFromClipboardCommand = ReactiveCommand.CreateFromTask(PasteFromClipboardAsync);
    }

    public Task<bool> CanPasteAsync() => _clipboardOperationsService.CanPasteAsync();

    private Task CopyToClipboardAsync() =>
        _clipboardOperationsService.CopyFilesAsync(_nodesSelectionService.SelectedNodes);

    private Task PasteFromClipboardAsync() =>
        _clipboardOperationsService.PasteFilesAsync(_directoryService.SelectedDirectory);
}