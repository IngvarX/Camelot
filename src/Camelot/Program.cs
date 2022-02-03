using System;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Camelot.Configuration;
using Camelot.DependencyInjection;
using CommandLine;
using Microsoft.Extensions.Logging;
using Splat;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Camelot;

internal static class Program
{
    private const int TimeoutSeconds = 3;

    [STAThread]
    public static void Main(string[] args)
    {
        var parserResult = Parser
            .Default
            .ParseArguments<CommandLineOptions>(args);

        var shouldExit = parserResult.Tag == ParserResultType.NotParsed;
        if (shouldExit)
        {
            return;
        }

        var parsed = (Parsed<CommandLineOptions>) parserResult;
        var dataAccessConfig = new DataAccessConfiguration
        {
            UseInMemoryDatabase = parsed.Value.IsIncognitoModeEnabled
        };

        var mutex = new Mutex(false, typeof(Program).FullName);

        try
        {
            if (!mutex.WaitOne(TimeSpan.FromSeconds(TimeoutSeconds), true))
            {
                return;
            }

            SubscribeToDomainUnhandledEvents();
            RegisterDependencies(dataAccessConfig);
            RunBackgroundTasks();

            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args, ShutdownMode.OnMainWindowClose);
        }
        finally
        {
            mutex.ReleaseMutex();
        }
    }

    private static void SubscribeToDomainUnhandledEvents() =>
        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            var logger = Locator.Current.GetRequiredService<ILogger>();
            var ex = (Exception) args.ExceptionObject;

            logger.LogCritical($"Unhandled application error: {ex}");
        };

    private static void RegisterDependencies(DataAccessConfiguration dataAccessConfig) =>
        Bootstrapper.Register(Locator.CurrentMutable, Locator.Current, dataAccessConfig);

    private static void RunBackgroundTasks() =>
        BackgroundTasksRunner.Start(Locator.Current);

    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();
}