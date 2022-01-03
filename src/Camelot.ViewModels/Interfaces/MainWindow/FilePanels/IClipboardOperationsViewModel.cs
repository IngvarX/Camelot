using System.Threading.Tasks;
using System.Windows.Input;

namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels;

public interface IClipboardOperationsViewModel
{
    public ICommand CopyToClipboardCommand { get; }

    public ICommand PasteFromClipboardCommand { get; }

    Task<bool> CanPasteAsync();
}