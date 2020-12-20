using System.IO;
using System.Reflection;
using Camelot.Configuration;
using Camelot.DataAccess.Configuration;
using Camelot.FileSystemWatcher.Configuration;
using Camelot.Properties;
using Camelot.Services.Configuration;
using Camelot.Services.Environment.Enums;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Linux.Configuration;
using Camelot.Services.Mac.Configuration;
using Camelot.ViewModels.Configuration;
using Microsoft.Extensions.Configuration;
using Splat;

namespace Camelot.DependencyInjection
{
    public static class ConfigurationBootstrapper
    {
        public static void RegisterConfiguration(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var aboutDialogConfiguration = new AboutDialogConfiguration();
            configuration.GetSection("About").Bind(aboutDialogConfiguration);
            services.RegisterConstant(aboutDialogConfiguration);

            var databaseConfiguration = new DatabaseConfiguration
            {
                ConnectionString = GetDatabaseConnectionString(configuration, resolver)
            };
            services.RegisterConstant(databaseConfiguration);

            var fileSystemWatcherConfiguration = new FileSystemWatcherConfiguration();
            configuration.GetSection("FileSystemWatcher").Bind(fileSystemWatcherConfiguration);
            services.RegisterConstant(fileSystemWatcherConfiguration);

            var imagePreviewConfiguration = new ImagePreviewConfiguration();
            configuration.GetSection("ImagePreview").Bind(imagePreviewConfiguration);
            services.RegisterConstant(imagePreviewConfiguration);

            var filePanelConfiguration = new FilePanelConfiguration();
            configuration.GetSection("FilePanel").Bind(filePanelConfiguration);
            services.RegisterConstant(filePanelConfiguration);

            var searchViewModelConfiguration = new SearchViewModelConfiguration();
            configuration.GetSection("SearchPanel").Bind(searchViewModelConfiguration);
            searchViewModelConfiguration.InvalidRegexResourceName = nameof(Resources.InvalidRegex);
            services.RegisterConstant(searchViewModelConfiguration);

            var driveServiceConfiguration = new DriveServiceConfiguration();
            configuration.GetSection("Drives").Bind(driveServiceConfiguration);
            services.RegisterConstant(driveServiceConfiguration);

            var unmountedDrivesConfiguration = new UnmountedDrivesConfiguration();
            configuration.GetSection("UnmountedDrives").Bind(unmountedDrivesConfiguration);
            services.RegisterConstant(unmountedDrivesConfiguration);

            var loggingConfiguration = new LoggingConfiguration();
            configuration.GetSection("Logging").Bind(loggingConfiguration);
            services.RegisterConstant(loggingConfiguration);

            var archiveTypeMapperConfiguration = new ArchiveTypeMapperConfiguration();
            configuration.GetSection("Archive").Bind(archiveTypeMapperConfiguration);
            services.RegisterConstant(archiveTypeMapperConfiguration);

            var archiveTypeViewModelFactoryConfiguration = new ArchiveTypeViewModelFactoryConfiguration();
            configuration.GetSection("ArchiveViewModelFactory").Bind(archiveTypeViewModelFactoryConfiguration);
            services.RegisterConstant(archiveTypeViewModelFactoryConfiguration);

            var operationsStatesConfiguration = new OperationsStatesConfiguration();
            configuration.GetSection("OperationsStates").Bind(operationsStatesConfiguration);
            services.RegisterConstant(operationsStatesConfiguration);

            var openWithDialogConfiguration = new OpenWithDialogConfiguration();
            configuration.GetSection("OpenWithDialog").Bind(openWithDialogConfiguration);
            services.RegisterConstant(openWithDialogConfiguration);

            var utiToExtensionsMappingConfiguration = new UtiToExtensionsMappingConfiguration();
            configuration.GetSection("UtiToExtensionsMapping").Bind(utiToExtensionsMappingConfiguration);
            services.RegisterConstant(utiToExtensionsMappingConfiguration);
        }

        private static string GetDatabaseConnectionString(IConfigurationRoot configuration,
            IReadonlyDependencyResolver resolver)
        {
            var platformService = resolver.GetRequiredService<IPlatformService>();
            var databaseName = configuration["DataAccess:DatabaseName"];
            var connectionString = configuration["DataAccess:ConnectionString"];

            string dbDirectory;
            if (platformService.GetPlatform() == Platform.Linux)
            {
                var environmentService = resolver.GetRequiredService<IEnvironmentService>();

                dbDirectory = $"{environmentService.GetEnvironmentVariable("HOME")}/.config/camelot";
            }
            else
            {
                var assemblyLocation = Assembly.GetEntryAssembly()?.Location;
                dbDirectory = Path.GetDirectoryName(assemblyLocation);
            }

            if (!Directory.Exists(dbDirectory))
            {
                Directory.CreateDirectory(dbDirectory);
            }

            return string.Format(connectionString, Path.Combine(dbDirectory, databaseName));
        }
    }
}