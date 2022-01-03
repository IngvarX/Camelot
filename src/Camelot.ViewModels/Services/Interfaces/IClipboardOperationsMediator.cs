using System.Threading.Tasks;
using System.Windows.Input;

namespace Camelot.ViewModels.Services.Interfaces;

public interface IClipboardOperationsMediator
{
    public ICommand CopyToClipboardCommand { get; }

    public ICommand PasteFromClipboardCommand { get; }

    Task<bool> CanPasteAsync();
}