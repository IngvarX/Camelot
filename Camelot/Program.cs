using System;
using System.Threading;
using Avalonia;
using Avalonia.Logging.Serilog;
using Avalonia.ReactiveUI;
using Splat;

namespace Camelot
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var mutex = new Mutex(false, typeof(Program).FullName);

            try
            {
                if (!mutex.WaitOne(TimeSpan.FromSeconds(5), true))
                {
                    return;
                }

                Registry.Register(Locator.CurrentMutable, Locator.Current);

                BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        private static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug()
                .UseReactiveUI();
    }
}
