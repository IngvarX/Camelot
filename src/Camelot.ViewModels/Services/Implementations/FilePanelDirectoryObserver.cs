using System;
using Camelot.Extensions;
using Camelot.ViewModels.Services.Interfaces;

namespace Camelot.ViewModels.Services.Implementations
{
    public class FilePanelDirectoryObserver : IFilePanelDirectoryObserver
    {
        private string _directory;

        public string CurrentDirectory
        {
            get => _directory;
            set
            {
                _directory = value;

                CurrentDirectoryChanged.Raise(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgs> CurrentDirectoryChanged;
    }
}