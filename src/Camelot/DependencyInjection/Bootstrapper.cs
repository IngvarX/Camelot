using System;
using System.IO;
using System.Reflection;
using Camelot.Avalonia.Implementations;
using Camelot.Avalonia.Interfaces;
using Camelot.Configuration;
using Camelot.DataAccess.Configuration;
using Camelot.DataAccess.LiteDb;
using Camelot.DataAccess.UnitOfWork;
using Camelot.FileSystemWatcher.Configuration;
using Camelot.FileSystemWatcher.Implementations;
using Camelot.FileSystemWatcher.Interfaces;
using Camelot.Operations;
using Camelot.Properties;
using Camelot.Services;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Operations;
using Camelot.Services.AllPlatforms;
using Camelot.Services.Behaviors;
using Camelot.Services.Configuration;
using Camelot.Services.Environment.Enums;
using Camelot.Services.Environment.Implementations;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Implementations;
using Camelot.Services.Linux;
using Camelot.Services.Linux.Builders;
using Camelot.Services.Linux.Configuration;
using Camelot.Services.Linux.Interfaces;
using Camelot.Services.Linux.Interfaces.Builders;
using Camelot.Services.Mac;
using Camelot.Services.Windows;
using Camelot.Services.Windows.Builders;
using Camelot.Services.Windows.Interfaces;
using Camelot.TaskPool.Interfaces;
using Camelot.ViewModels.Configuration;
using Camelot.ViewModels.Factories.Implementations;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations;
using Camelot.ViewModels.Implementations.Behaviors;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Implementations.Dialogs.Properties;
using Camelot.ViewModels.Implementations.MainWindow;
using Camelot.ViewModels.Implementations.MainWindow.Drives;
using Camelot.ViewModels.Implementations.MainWindow.FavouriteDirectories;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
using Camelot.ViewModels.Implementations.MainWindow.OperationsStates;
using Camelot.ViewModels.Implementations.Menu;
using Camelot.ViewModels.Implementations.Settings;
using Camelot.ViewModels.Implementations.Settings.General;
using Camelot.ViewModels.Interfaces.MainWindow;
using Camelot.ViewModels.Interfaces.MainWindow.Directories;
using Camelot.ViewModels.Interfaces.MainWindow.Drives;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Interfaces.MainWindow.OperationsStates;
using Camelot.ViewModels.Interfaces.Menu;
using Camelot.ViewModels.Services.Implementations;
using Camelot.ViewModels.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Extensions.Logging;
using Splat;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Camelot.DependencyInjection
{
    public static class Bootstrapper
    {
        public static void Register(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            RegisterConfiguration(services);
            RegisterLogging(services, resolver);
            RegisterEnvironmentServices(services);
            RegisterAvaloniaServices(services);
            RegisterFileSystemWatcherServices(services, resolver);
            RegisterTaskPool(services, resolver);
            RegisterDataAccess(services, resolver);
            RegisterServices(services, resolver);
            RegisterPlatformSpecificServices(services, resolver);
            RegisterViewModels(services, resolver);
        }

        private static void RegisterLogging(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
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

        private static void RegisterConfiguration(IMutableDependencyResolver services)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var aboutDialogConfiguration = new AboutDialogConfiguration();
            configuration.GetSection("About").Bind(aboutDialogConfiguration);
            services.RegisterConstant(aboutDialogConfiguration);

            var databaseName = configuration["DataAccess:DatabaseName"];
            var connectionString = configuration["DataAccess:ConnectionString"];
            var assemblyLocation = Assembly.GetEntryAssembly()?.Location;
            var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
            var databaseConfiguration = new DatabaseConfiguration
            {
                ConnectionString = string.Format(connectionString, Path.Combine(assemblyDirectory, databaseName))
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
        }

        private static void RegisterEnvironmentServices(IMutableDependencyResolver services)
        {
            services.RegisterLazySingleton<IEnvironmentService>(() => new EnvironmentService());
            services.RegisterLazySingleton<IProcessService>(() => new ProcessService());
            services.RegisterLazySingleton<IEnvironmentFileService>(() => new EnvironmentFileService());
            services.RegisterLazySingleton<IEnvironmentDirectoryService>(() => new EnvironmentDirectoryService());
            services.RegisterLazySingleton<IEnvironmentDriveService>(() => new EnvironmentDriveService());
            services.RegisterLazySingleton<IEnvironmentPathService>(() => new EnvironmentPathService());
            services.RegisterLazySingleton<IRegexService>(() => new RegexService());
            services.Register<IPlatformService>(() => new PlatformService());
        }

        private static void RegisterAvaloniaServices(IMutableDependencyResolver services)
        {
            services.RegisterLazySingleton<IApplicationCloser>(() => new ApplicationCloser());
            services.RegisterLazySingleton<IApplicationDispatcher>(() => new AvaloniaDispatcher());
            services.RegisterLazySingleton<IApplicationVersionProvider>(() => new ApplicationVersionProvider());
        }

        private static void RegisterFileSystemWatcherServices(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton<IFileSystemWatcherFactory>(() => new FileSystemWatcherFactory(
                resolver.GetRequiredService<IPathService>(),
                resolver.GetRequiredService<FileSystemWatcherConfiguration>()
            ));
        }

        private static void RegisterTaskPool(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton<ITaskPool>(() => new TaskPool.Implementations.TaskPool(
                resolver.GetRequiredService<IEnvironmentService>()
            ));
        }

        private static void RegisterDataAccess(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton<IUnitOfWorkFactory>(() => new LiteDbUnitOfWorkFactory(
                resolver.GetRequiredService<DatabaseConfiguration>()
            ));
            services.RegisterLazySingleton<IClipboardService>(() => new ClipboardService());
            services.RegisterLazySingleton<IMainWindowProvider>(() => new MainWindowProvider());
        }

        private static void RegisterServices(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton<IFileService>(() => new FileService(
                resolver.GetRequiredService<IPathService>(),
                resolver.GetRequiredService<IEnvironmentFileService>(),
                resolver.GetRequiredService<ILogger>()
            ));
            services.RegisterLazySingleton<IDateTimeProvider>(() => new DateTimeProvider());
            services.RegisterLazySingleton<IDriveService>(() => new DriveService(
                resolver.GetRequiredService<IEnvironmentDriveService>(),
                resolver.GetRequiredService<IUnmountedDriveService>(),
                resolver.GetRequiredService<DriveServiceConfiguration>()
            ));
            services.RegisterLazySingleton<IOperationsFactory>(() => new OperationsFactory(
                resolver.GetRequiredService<ITaskPool>(),
                resolver.GetRequiredService<IDirectoryService>(),
                resolver.GetRequiredService<IFileService>(),
                resolver.GetRequiredService<IPathService>(),
                resolver.GetRequiredService<IFileNameGenerationService>(),
                resolver.GetRequiredService<ILogger>()
            ));
            services.RegisterLazySingleton<INodesSelectionService>(() => new NodesSelectionService());
            services.RegisterLazySingleton<IOperationsService>(() => new OperationsService(
                resolver.GetRequiredService<IOperationsFactory>(),
                resolver.GetRequiredService<IDirectoryService>(),
                resolver.GetRequiredService<IResourceOpeningService>(),
                resolver.GetRequiredService<IFileService>(),
                resolver.GetRequiredService<IPathService>(),
                resolver.GetRequiredService<IOperationsStateService>()
            ));
            services.RegisterLazySingleton<IDirectoryService>(() => new DirectoryService(
                resolver.GetRequiredService<IPathService>(),
                resolver.GetRequiredService<IEnvironmentDirectoryService>(),
                resolver.GetRequiredService<IEnvironmentFileService>(),
                resolver.GetRequiredService<ILogger>()
            ));
            services.Register<IFileSystemWatchingService>(() => new FileSystemWatchingService(
                resolver.GetRequiredService<IFileSystemWatcherFactory>()
            ));
            services.RegisterLazySingleton(() => new FileOpeningBehavior(
                resolver.GetRequiredService<IResourceOpeningService>()
            ));
            services.RegisterLazySingleton(() => new DirectoryOpeningBehavior(
                resolver.GetRequiredService<IDirectoryService>()
            ));
            services.RegisterLazySingleton<ILocalizationService>(() => new LocalizationService(
                resolver.GetRequiredService<IUnitOfWorkFactory>()
            ));
            services.RegisterLazySingleton<IFileSizeFormatter>(() => new FileSizeFormatter());
            services.RegisterLazySingleton<IPathService>(() => new PathService(
                resolver.GetRequiredService<IEnvironmentPathService>()
            ));
            services.RegisterLazySingleton<IDialogService>(() => new DialogService(
                resolver.GetRequiredService<IMainWindowProvider>()
            ));
            services.RegisterLazySingleton<IResourceProvider>(() => new ResourceProvider());
            services.RegisterLazySingleton<ILanguageManager>(() => new LanguageManager());
            services.RegisterLazySingleton<IClipboardOperationsService>(() => new ClipboardOperationsService(
                resolver.GetRequiredService<IClipboardService>(),
                resolver.GetRequiredService<IOperationsService>(),
                resolver.GetRequiredService<IEnvironmentService>()
            ));
            services.RegisterLazySingleton<IOperationsStateService>(() => new OperationsStateService());
            services.RegisterLazySingleton<IFileNameGenerationService>(() => new FileNameGenerationService(
                resolver.GetRequiredService<IFileService>(),
                resolver.GetRequiredService<IDirectoryService>(),
                resolver.GetRequiredService<IPathService>()
            ));
        }

        private static void RegisterPlatformSpecificServices(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            var platformService = resolver.GetRequiredService<IPlatformService>();
            var platform = platformService.GetPlatform();

            switch (platform)
            {
                case Platform.Linux:
                    RegisterLinuxServices(services, resolver);
                    break;
                case Platform.MacOs:
                    RegisterMacServices(services, resolver);
                    break;
                case Platform.Windows:
                    RegisterWindowsServices(services, resolver);
                    break;
                case Platform.Unknown:
                    throw new InvalidOperationException("Unsupported platform");
                default:
                    throw new ArgumentOutOfRangeException(nameof(platform));
            }
        }

        private static void RegisterLinuxServices(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton<ILinuxRemovedFileMetadataBuilderFactory>(() => new LinuxRemovedFileMetadataBuilderFactory());
            services.RegisterLazySingleton<IUnmountedDriveService>(() => new LinuxUnmountedDriveService(
                resolver.GetRequiredService<IProcessService>(),
                resolver.GetRequiredService<IEnvironmentService>(),
                resolver.GetRequiredService<UnmountedDrivesConfiguration>()
            ));
            services.RegisterLazySingleton<ITrashCanService>(() => new LinuxTrashCanService(
                resolver.GetRequiredService<IDriveService>(),
                resolver.GetRequiredService<IOperationsService>(),
                resolver.GetRequiredService<IPathService>(),
                resolver.GetRequiredService<IFileService>(),
                resolver.GetRequiredService<IEnvironmentService>(),
                resolver.GetRequiredService<IDirectoryService>(),
                resolver.GetRequiredService<IDateTimeProvider>(),
                resolver.GetRequiredService<ILinuxRemovedFileMetadataBuilderFactory>(),
                resolver.GetRequiredService<IHomeDirectoryProvider>()
            ));
            services.RegisterLazySingleton<IHomeDirectoryProvider>(() => new UnixHomeDirectoryProvider(
                resolver.GetRequiredService<IEnvironmentService>()
            ));
            services.RegisterLazySingleton<IDesktopEnvironmentService>(() => new DesktopEnvironmentService(
                resolver.GetRequiredService<IEnvironmentService>()
            ));
            services.RegisterLazySingleton<IShellCommandWrappingService>(() => new ShellCommandWrappingService());
            services.RegisterLazySingleton<IResourceOpeningService>(() => new LinuxResourceOpeningService(
                resolver.GetRequiredService<IProcessService>(),
                resolver.GetRequiredService<IShellCommandWrappingService>(),
                resolver.GetRequiredService<IDesktopEnvironmentService>()
            ));
            services.RegisterLazySingleton<ITerminalService>(() => new LinuxTerminalService(
                resolver.GetRequiredService<IProcessService>(),
                resolver.GetRequiredService<IUnitOfWorkFactory>(),
                resolver.GetRequiredService<IDesktopEnvironmentService>(),
                resolver.GetRequiredService<IShellCommandWrappingService>()
            ));
            services.RegisterLazySingleton<ISoftwareService>(() => new LinuxSoftwareService());
        }

        private static void RegisterMacServices(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton<IUnmountedDriveService>(() => new MacUnmountedDriveService());
            services.RegisterLazySingleton<ITrashCanService>(() => new MacTrashCanService(
                resolver.GetRequiredService<IDriveService>(),
                resolver.GetRequiredService<IOperationsService>(),
                resolver.GetRequiredService<IPathService>(),
                resolver.GetRequiredService<IFileService>(),
                resolver.GetRequiredService<IEnvironmentService>(),
                resolver.GetRequiredService<IDirectoryService>(),
                resolver.GetRequiredService<IHomeDirectoryProvider>()
            ));
            services.RegisterLazySingleton<IResourceOpeningService>(() => new MacResourceOpeningService(
                resolver.GetRequiredService<IProcessService>()
            ));
            services.RegisterLazySingleton<IHomeDirectoryProvider>(() => new UnixHomeDirectoryProvider(
                resolver.GetRequiredService<IEnvironmentService>()
            ));
            services.RegisterLazySingleton<ITerminalService>(() => new MacTerminalService(
                resolver.GetRequiredService<IProcessService>(),
                resolver.GetRequiredService<IUnitOfWorkFactory>()
            ));
            services.RegisterLazySingleton<ISoftwareService>(() => new MacSoftwareService(
                resolver.GetRequiredService<IProcessService>(),
                resolver.GetRequiredService<ITerminalService>()
            ));
        }

        private static void RegisterWindowsServices(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton<IWindowsRemovedFileMetadataBuilderFactory>(() => new WindowsRemovedFileMetadataBuilderFactory());
            services.RegisterLazySingleton<IWindowsTrashCanNodeNameGenerator>(() => new WindowsTrashCanNodeNameGenerator());
            services.RegisterLazySingleton<IHomeDirectoryProvider>(() => new WindowsHomeDirectoryProvider(
                resolver.GetRequiredService<IEnvironmentService>()
            ));
            services.RegisterLazySingleton<IUnmountedDriveService>(() => new WindowsUnmountedDriveService());
            services.RegisterLazySingleton<ITrashCanService>(() => new WindowsTrashCanService(
                resolver.GetRequiredService<IDriveService>(),
                resolver.GetRequiredService<IOperationsService>(),
                resolver.GetRequiredService<IPathService>(),
                resolver.GetRequiredService<IFileService>(),
                resolver.GetRequiredService<IDateTimeProvider>(),
                resolver.GetRequiredService<IProcessService>(),
                resolver.GetRequiredService<IWindowsRemovedFileMetadataBuilderFactory>(),
                resolver.GetRequiredService<IWindowsTrashCanNodeNameGenerator>()
            ));
            services.RegisterLazySingleton<IResourceOpeningService>(() => new WindowsResourceOpeningService(
                resolver.GetRequiredService<IProcessService>()
            ));
            services.RegisterLazySingleton<ITerminalService>(() => new WindowsTerminalService(
                resolver.GetRequiredService<IProcessService>(),
                resolver.GetRequiredService<IUnitOfWorkFactory>()
            ));
            services.RegisterLazySingleton<ISoftwareService>(() => new WindowsSoftwareService());
        }

        private static void RegisterViewModels(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton<IFilesOperationsMediator>(() => new FilesOperationsMediator(
                resolver.GetRequiredService<IDirectoryService>()
            ));
            services.RegisterLazySingleton<IFileSystemNodeViewModelComparerFactory>(() => new FileSystemNodeViewModelComparerFactory());
            services.Register(() => new TerminalSettingsViewModel(
                resolver.GetRequiredService<ITerminalService>()
            ));
            services.Register(() => new GeneralSettingsViewModel(
                resolver.GetRequiredService<LanguageSettingsViewModel>()
            ));
            services.Register(() => new LanguageSettingsViewModel(
                resolver.GetRequiredService<ILocalizationService>(),
                resolver.GetRequiredService<ILanguageManager>()
            ));
            services.Register(() => new SettingsDialogViewModel(
                resolver.GetRequiredService<GeneralSettingsViewModel>(),
                resolver.GetRequiredService<TerminalSettingsViewModel>()
            ));
            services.RegisterLazySingleton<ITabViewModelFactory>(() => new TabViewModelFactory(
                resolver.GetRequiredService<IPathService>()
            ));
            services.RegisterLazySingleton(() => new FilePropertiesBehavior(
                resolver.GetRequiredService<IDialogService>()
            ));
            services.RegisterLazySingleton(() => new DirectoryPropertiesBehavior(
                resolver.GetRequiredService<IDialogService>()
            ));
            services.RegisterLazySingleton<IFileSystemNodeViewModelFactory>(() => new FileSystemNodeViewModelFactory(
                resolver.GetRequiredService<FileOpeningBehavior>(),
                resolver.GetRequiredService<DirectoryOpeningBehavior>(),
                resolver.GetRequiredService<IFileSizeFormatter>(),
                resolver.GetRequiredService<IPathService>(),
                resolver.GetRequiredService<IOperationsService>(),
                resolver.GetRequiredService<IClipboardOperationsService>(),
                resolver.GetRequiredService<IFilesOperationsMediator>(),
                resolver.GetRequiredService<FilePropertiesBehavior>(),
                resolver.GetRequiredService<DirectoryPropertiesBehavior>(),
                resolver.GetRequiredService<IDialogService>(),
                resolver.GetRequiredService<ITrashCanService>(),
                resolver.GetRequiredService<IFileService>(),
                resolver.GetRequiredService<IDirectoryService>()
            ));
            services.Register(() => new AboutDialogViewModel(
                resolver.GetRequiredService<IApplicationVersionProvider>(),
                resolver.GetRequiredService<IResourceOpeningService>(),
                resolver.GetRequiredService<AboutDialogConfiguration>()
            ));
            services.RegisterLazySingleton<IBitmapFactory>(() => new BitmapFactory());
            services.Register(() => new MainNodeInfoTabViewModel(
                resolver.GetRequiredService<IFileSizeFormatter>(),
                resolver.GetRequiredService<IPathService>(),
                resolver.GetRequiredService<IBitmapFactory>(),
                resolver.GetRequiredService<ImagePreviewConfiguration>()
            ));
            services.Register(() => new DirectoryInformationDialogViewModel(
                resolver.GetRequiredService<IDirectoryService>(),
                resolver.GetRequiredService<IApplicationDispatcher>(),
                resolver.GetRequiredService<MainNodeInfoTabViewModel>()
            ));
            services.Register(() => new FileInformationDialogViewModel(
                resolver.GetRequiredService<IFileService>(),
                resolver.GetRequiredService<MainNodeInfoTabViewModel>()
            ));
            services.Register(() => new OverwriteOptionsDialogViewModel(
                resolver.GetRequiredService<IFileService>(),
                resolver.GetRequiredService<IFileSystemNodeViewModelFactory>(),
                resolver.GetRequiredService<IFileNameGenerationService>(),
                resolver.GetRequiredService<IPathService>()
            ));
            services.Register(() => new CreateDirectoryDialogViewModel(
                resolver.GetRequiredService<IDirectoryService>(),
                resolver.GetRequiredService<IFileService>(),
                resolver.GetRequiredService<IPathService>()
            ));
            services.Register(() => new CreateFileDialogViewModel(
                resolver.GetRequiredService<IDirectoryService>(),
                resolver.GetRequiredService<IFileService>(),
                resolver.GetRequiredService<IPathService>()
            ));
            services.RegisterLazySingleton<IOperationsStateViewModel>(() => new OperationsStatesListViewModel(
                resolver.GetRequiredService<IOperationsStateService>(),
                resolver.GetRequiredService<IOperationStateViewModelFactory>(),
                resolver.GetRequiredService<IApplicationDispatcher>(),
                resolver.GetRequiredService<IDialogService>()
            ));
            services.Register(() => new RemoveNodesConfirmationDialogViewModel(
                resolver.GetRequiredService<IPathService>()
            ));
            services.RegisterLazySingleton<IOperationStateViewModelFactory>(() => new OperationStateViewModelFactory(
                resolver.GetRequiredService<IPathService>()
            ));
            services.Register<IOperationsViewModel>(() => new OperationsViewModel(
                resolver.GetRequiredService<IFilesOperationsMediator>(),
                resolver.GetRequiredService<IOperationsService>(),
                resolver.GetRequiredService<INodesSelectionService>(),
                resolver.GetRequiredService<IDialogService>(),
                resolver.GetRequiredService<IDirectoryService>(),
                resolver.GetRequiredService<ITrashCanService>()
            ));
            services.RegisterLazySingleton<IMenuViewModel>(() => new MenuViewModel(
                resolver.GetRequiredService<IApplicationCloser>(),
                resolver.GetRequiredService<IDialogService>()
            ));
            services.Register<ISearchViewModel>(() => new SearchViewModel(
                resolver.GetRequiredService<IRegexService>(),
                resolver.GetRequiredService<IResourceProvider>(),
                resolver.GetRequiredService<SearchViewModelConfiguration>()
            ));
            services.RegisterLazySingleton<IDriveViewModelFactory>(() => new DriveViewModelFactory(
                resolver.GetRequiredService<IFileSizeFormatter>(),
                resolver.GetRequiredService<IPathService>(),
                resolver.GetRequiredService<IFilesOperationsMediator>(),
                resolver.GetRequiredService<IUnmountedDriveService>()
            ));
            services.RegisterLazySingleton<IDrivesListViewModel>(() => new DrivesListViewModel(
                resolver.GetRequiredService<IDriveService>(),
                resolver.GetRequiredService<IDriveViewModelFactory>(),
                resolver.GetRequiredService<IApplicationDispatcher>()
            ));
            services.RegisterLazySingleton<ITopOperationsViewModel>(() => new TopOperationsViewModel(
                resolver.GetRequiredService<ITerminalService>(),
                resolver.GetRequiredService<IDirectoryService>(),
                resolver.GetRequiredService<IFilesOperationsMediator>()
            ));
            services.RegisterLazySingleton<IFavouriteDirectoryViewModelFactory>(() => new FavouriteDirectoryViewModelFactory(
                resolver.GetRequiredService<IFilesOperationsMediator>(),
                resolver.GetRequiredService<IDirectoryService>()
            ));
            services.RegisterLazySingleton<IFavouriteDirectoriesListViewModel>(() => new FavouriteDirectoriesListViewModel(
                resolver.GetRequiredService<IFavouriteDirectoryViewModelFactory>(),
                resolver.GetRequiredService<IHomeDirectoryProvider>()
            ));
            services.RegisterLazySingleton(() => new MainWindowViewModel(
                resolver.GetRequiredService<IFilesOperationsMediator>(),
                resolver.GetRequiredService<IOperationsViewModel>(),
                CreateFilesPanelViewModel(resolver, "Left"),
                CreateFilesPanelViewModel(resolver, "Right"),
                resolver.GetRequiredService<IMenuViewModel>(),
                resolver.GetRequiredService<IOperationsStateViewModel>(),
                resolver.GetRequiredService<ITopOperationsViewModel>(),
                resolver.GetRequiredService<IDrivesListViewModel>(),
                resolver.GetRequiredService<IFavouriteDirectoriesListViewModel>()
            ));
        }

        private static IFilesPanelViewModel CreateFilesPanelViewModel(
            IReadonlyDependencyResolver resolver,
            string panelKey)
        {
            var filesPanelStateService = new FilesPanelStateService(
                resolver.GetRequiredService<IUnitOfWorkFactory>(),
                panelKey
            );
            var tabsListViewModel = new TabsListViewModel(
                filesPanelStateService,
                resolver.GetRequiredService<IDirectoryService>(),
                resolver.GetRequiredService<ITabViewModelFactory>(),
                resolver.GetRequiredService<FilePanelConfiguration>()
            );
            var filesPanelViewModel = new FilesPanelViewModel(
                resolver.GetRequiredService<IFileService>(),
                resolver.GetRequiredService<IDirectoryService>(),
                resolver.GetRequiredService<INodesSelectionService>(),
                resolver.GetRequiredService<IFileSystemNodeViewModelFactory>(),
                resolver.GetRequiredService<IFileSystemWatchingService>(),
                resolver.GetRequiredService<IApplicationDispatcher>(),
                resolver.GetRequiredService<IFileSizeFormatter>(),
                resolver.GetRequiredService<IClipboardOperationsService>(),
                resolver.GetRequiredService<IFileSystemNodeViewModelComparerFactory>(),
                resolver.GetRequiredService<ISearchViewModel>(),
                tabsListViewModel,
                resolver.GetRequiredService<IOperationsViewModel>()
            );

            return filesPanelViewModel;
        }
    }
}