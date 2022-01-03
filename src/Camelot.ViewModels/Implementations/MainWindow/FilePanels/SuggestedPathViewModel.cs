using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Enums;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels;

public class SuggestedPathViewModel : ViewModelBase, ISuggestedPathViewModel
{
    public string FullPath { get; }

    public SuggestedPathType Type { get; }

    public string Text { get; }

    public SuggestedPathViewModel(string fullPath, SuggestedPathType type, string text)
    {
        FullPath = fullPath;
        Type = type;
        Text = text;
    }
}