using System.IO;
using Camelot.Configuration;
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
                var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), config.LogFileName);
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
    }
}