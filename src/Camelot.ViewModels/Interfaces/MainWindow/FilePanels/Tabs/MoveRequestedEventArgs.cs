using System;

namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Tabs
{
    public class MoveRequestedEventArgs : EventArgs
    {
        public ITabViewModel Target { get; }

        public MoveRequestedEventArgs(ITabViewModel target)
        {
            Target = target;
        }
    }
}