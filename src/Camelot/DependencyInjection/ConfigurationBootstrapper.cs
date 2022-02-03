using System.IO;
using System.Reflection;
using Camelot.Configuration;
using Camelot.DataAccess.Configuration;
using Camelot.Properties;
using Camelot.Services.Configuration;
using Camelot.Services.Environment.Enums;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.FileSystemWatcher.Configuration;
using Camelot.Services.Mac.Configuration;
using Camelot.ViewModels.Configuration;
using Microsoft.Extensions.Configuration;
using Splat;
using LinuxUnmountedDrivesConfiguration = Camelot.Services.Linux.Configuration.UnmountedDrivesConfiguration;
using MacUnmountedDrivesConfiguration = Camelot.Services.Mac.Configuration.UnmountedDrivesConfiguration;

namespace Camelot.DependencyInjection;

public static class ConfigurationBootstrapper
{
    public static void RegisterConfiguration(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver,
        DataAccessConfiguration dataAccessConfig)
    {
        var configuration = BuildConfiguration();

        RegisterAboutDialogConfiguration(services, configuration);
        RegisterDatabaseConfiguration(services, resolver, configuration, dataAccessConfig);
        RegisterFileSystemWatcherConfiguration(services, configuration);
        RegisterImagePreviewConfiguration(services, configuration);
        RegisterTabConfiguration(services, configuration);
        RegisterTabsListConfiguration(services, configuration);
        RegisterSearchViewModelConfiguration(services, configuration);
        RegisterDriveServiceConfiguration(services, configuration);
        RegisterUnmountedDrivesConfiguration(services, configuration);
        RegisterLoggingConfiguration(services, configuration);
        RegisterArchiveTypeMapperConfiguration(services, configuration);
        RegisterArchiveTypeViewModelFactoryConfiguration(services, configuration);
        RegisterOperationsStatesConfiguration(services, configuration);
        RegisterOpenWithDialogConfiguration(services, configuration);
        RegisterUtiToExtensionsMappingConfiguration(services, configuration);
        RegisterDefaultThemeConfiguration(services, configuration);
        RegisterThemesNamesConfiguration(services, configuration);
        RegisterLanguagesConfiguration(services, configuration);
        RegisterSuggestionsConfiguration(services, configuration);
        RegisterFileTypeMapperConfiguration(services, configuration);
    }

    private static IConfiguration BuildConfiguration() =>
        new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

    private static void RegisterAboutDialogConfiguration(IMutableDependencyResolver services,
        IConfiguration configuration)
    {
        var config = new AboutDialogConfiguration();
        configuration.GetSection("About").Bind(config);
        services.RegisterConstant(config);
    }

    private static void RegisterDatabaseConfiguration(IMutableDependencyResolver services,
        IReadonlyDependencyResolver resolver, IConfiguration configuration, DataAccessConfiguration dataAccessConfig)
    {
        var config = new DatabaseConfiguration
        {
            ConnectionString = GetDatabaseConnectionString(configuration, resolver),
            UseInMemoryDatabase = dataAccessConfig.UseInMemoryDatabase
        };
        services.RegisterConstant(config);
    }

    private static string GetDatabaseConnectionString(IConfiguration configuration,
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

    private static void RegisterFileSystemWatcherConfiguration(IMutableDependencyResolver services,
        IConfiguration configuration)
    {
        var config = new FileSystemWatcherConfiguration();
        configuration.GetSection("FileSystemWatcher").Bind(config);
        services.RegisterConstant(config);
    }

    private static void RegisterImagePreviewConfiguration(IMutableDependencyResolver services,
        IConfiguration configuration)
    {
        var config = new ImagePreviewConfiguration();
        configuration.GetSection("ImagePreview").Bind(config);
        services.RegisterConstant(config);
    }

    private static void RegisterTabConfiguration(IMutableDependencyResolver services,
        IConfiguration configuration)
    {
        var config = new TabConfiguration();
        configuration.GetSection("Tab").Bind(config);
        services.RegisterConstant(config);
    }

    private static void RegisterTabsListConfiguration(IMutableDependencyResolver services,
        IConfiguration configuration)
    {
        var config = new TabsListConfiguration();
        configuration.GetSection("TabsList").Bind(config);
        services.RegisterConstant(config);
    }

    private static void RegisterSearchViewModelConfiguration(IMutableDependencyResolver services,
        IConfiguration configuration)
    {
        var config = new SearchViewModelConfiguration();
        configuration.GetSection("SearchPanel").Bind(config);
        config.InvalidRegexResourceName = nameof(Resources.InvalidRegex);
        services.RegisterConstant(config);
    }

    private static void RegisterDriveServiceConfiguration(IMutableDependencyResolver services,
        IConfiguration configuration)
    {
        var config = new DriveServiceConfiguration();
        configuration.GetSection("Drives").Bind(config);
        services.RegisterConstant(config);
    }

    private static void RegisterUnmountedDrivesConfiguration(IMutableDependencyResolver services,
        IConfiguration configuration)
    {
        RegisterLinuxUnmountedDrivesConfiguration(services, configuration);
        RegisterMacUnmountedDrivesConfiguration(services, configuration);
    }

    private static void RegisterLinuxUnmountedDrivesConfiguration(IMutableDependencyResolver services,
        IConfiguration configuration)
    {
        var config = new LinuxUnmountedDrivesConfiguration();
        configuration.GetSection("UnmountedDrives:Linux").Bind(config);
        services.RegisterConstant(config);
    }

    private static void RegisterMacUnmountedDrivesConfiguration(IMutableDependencyResolver services,
        IConfiguration configuration)
    {
        var config = new MacUnmountedDrivesConfiguration();
        configuration.GetSection("UnmountedDrives:Mac").Bind(config);
        services.RegisterConstant(config);
    }

    private static void RegisterLoggingConfiguration(IMutableDependencyResolver services,
        IConfiguration configuration)
    {
        var config = new LoggingConfiguration();
        configuration.GetSection("Logging").Bind(config);
        services.RegisterConstant(config);
    }

    private static void RegisterArchiveTypeMapperConfiguration(IMutableDependencyResolver services,
        IConfiguration configuration)
    {
        var config = new ArchiveTypeMapperConfiguration();
        configuration.GetSection("Archive").Bind(config);
        services.RegisterConstant(config);
    }

    private static void RegisterArchiveTypeViewModelFactoryConfiguration(IMutableDependencyResolver services,
        IConfiguration configuration)
    {
        var config = new ArchiveTypeViewModelFactoryConfiguration();
        configuration.GetSection("ArchiveViewModelFactory").Bind(config);
        services.RegisterConstant(config);
    }

    private static void RegisterOperationsStatesConfiguration(IMutableDependencyResolver services,
        IConfiguration configuration)
    {
        var config = new OperationsStatesConfiguration();
        configuration.GetSection("OperationsStates").Bind(config);
        services.RegisterConstant(config);
    }

    private static void RegisterOpenWithDialogConfiguration(IMutableDependencyResolver services,
        IConfiguration configuration)
    {
        var config = new OpenWithDialogConfiguration();
        configuration.GetSection("OpenWithDialog").Bind(config);
        services.RegisterConstant(config);
    }

    private static void RegisterUtiToExtensionsMappingConfiguration(IMutableDependencyResolver services,
        IConfiguration configuration)
    {
        var config = new UtiToExtensionsMappingConfiguration();
        configuration.GetSection("UtiToExtensionsMapping").Bind(config);
        services.RegisterConstant(config);
    }

    private static void RegisterDefaultThemeConfiguration(IMutableDependencyResolver services,
        IConfiguration configuration)
    {
        var config = new DefaultThemeConfiguration();
        configuration.GetSection("Themes").Bind(config);
        services.RegisterConstant(config);
    }

    private static void RegisterThemesNamesConfiguration(IMutableDependencyResolver services,
        IConfiguration configuration)
    {
        var config = new ThemesNamesConfiguration();
        configuration.GetSection("Themes").Bind(config);
        services.RegisterConstant(config);
    }

    private static void RegisterLanguagesConfiguration(IMutableDependencyResolver services,
        IConfiguration configuration)
    {
        var config = new LanguagesConfiguration();
        configuration.GetSection("Languages").Bind(config);
        services.RegisterConstant(config);
    }

    private static void RegisterSuggestionsConfiguration(IMutableDependencyResolver services,
        IConfiguration configuration)
    {
        var config = new SuggestionsConfiguration();
        configuration.GetSection("Suggestions").Bind(config);
        services.RegisterConstant(config);
    }
        
    private static void RegisterFileTypeMapperConfiguration(IMutableDependencyResolver services,
        IConfiguration configuration)
    {
        var config = new FileTypeMapperConfiguration();
        configuration.GetSection("FileTypeMappings").Bind(config);
        services.RegisterConstant(config);
    }
}