using System;
using Camelot.Extensions;
using Camelot.Services.Abstractions;
using Camelot.ViewModels.Services.Interfaces;

namespace Camelot.ViewModels.Services.Implementations;

public class FilePanelDirectoryObserver : IFilePanelDirectoryObserver
{
    private readonly IPathService _pathService;
    private string _directory;

    public string CurrentDirectory
    {
        get => _directory;
        set
        {
            var newDirectory = PreprocessPath(value);
            if (_directory != newDirectory)
            {
                _directory = newDirectory;

                CurrentDirectoryChanged.Raise(this, EventArgs.Empty);
            }
        }
    }

    public event EventHandler<EventArgs> CurrentDirectoryChanged;

    public FilePanelDirectoryObserver(IPathService pathService)
    {
        _pathService = pathService;
    }

    private string PreprocessPath(string path) => _pathService.RightTrimPathSeparators(path);
}