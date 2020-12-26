using System.IO;
using Camelot.Configuration;
using Camelot.Services.Environment.Enums;
using Camelot.Services.Environment.Interfaces;
using Serilog;
using Serilog.Extensions.Logging;
using Splat;

namespace Camelot.DependencyInjection
{
    public static class LoggingBootstrapper
    {
        public static void RegisterLogging(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton(() =>
            {
                var config = resolver.GetRequiredService<LoggingConfiguration>();
                var logFilePath = GetLogFileName(config, resolver);
                var logger = new LoggerConfiguration()
                    .MinimumLevel.Override("Default", config.DefaultLogLevel)
                    .MinimumLevel.Override("Microsoft", config.MicrosoftLogLevel)
                    .WriteTo.Console()
                    .WriteTo.RollingFile(logFilePath, fileSizeLimitBytes: config.LimitBytes)
                    .CreateLogger();
                var factory = new SerilogLoggerFactory(logger);

                return factory.CreateLogger("Default");
            });
        }

        private static string GetLogFileName(LoggingConfiguration config,
            IReadonlyDependencyResolver resolver)
        {
            var platformService = resolver.GetRequiredService<IPlatformService>();

            string logDirectory;
            if (platformService.GetPlatform() == Platform.Linux)
            {
                var environmentService = resolver.GetRequiredService<IEnvironmentService>();

                logDirectory = $"{environmentService.GetEnvironmentVariable("HOME")}/.config/camelot/logs";
            }
            else
            {
                logDirectory = Directory.GetCurrentDirectory();
            }

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            return Path.Combine(logDirectory, config.LogFileName);
        }
    }
}