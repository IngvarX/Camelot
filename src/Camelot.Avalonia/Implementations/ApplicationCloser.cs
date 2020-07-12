using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Camelot.Avalonia.Interfaces;

namespace Camelot.Avalonia.Implementations
{
    public class ApplicationCloser : IApplicationCloser
    {
        public void CloseApp()
        {
            var lifetime = (IClassicDesktopStyleApplicationLifetime) Application.Current.ApplicationLifetime;

            lifetime.Shutdown();
        }
    }
}