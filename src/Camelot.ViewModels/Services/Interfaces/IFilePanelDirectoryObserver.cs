using System;

namespace Camelot.ViewModels.Services.Interfaces;

public interface IFilePanelDirectoryObserver
{
    string CurrentDirectory { get; set; }

    event EventHandler<EventArgs> CurrentDirectoryChanged;
}