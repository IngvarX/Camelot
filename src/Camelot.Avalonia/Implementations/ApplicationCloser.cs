using ApplicationDispatcher.Interfaces;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

namespace ApplicationDispatcher.Implementations
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