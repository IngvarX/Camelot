using System;
using Camelot.Avalonia.Interfaces;
using Camelot.DataAccess.UnitOfWork;
using Camelot.FileSystemWatcher.Interfaces;
using Camelot.Operations;
using Camelot.Services;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Abstractions.Drives;
using Camelot.Services.Abstractions.Operations;
using Camelot.Services.AllPlatforms;
using Camelot.Services.Archive;
using Camelot.Services.Archives;
using Camelot.Services.Behaviors;
using Camelot.Services.Configuration;
using Camelot.Services.Drives;
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
using Camelot.Services.Mac.Configuration;
using Camelot.Services.Mac.Interfaces;
using Camelot.Services.Windows;
using Camelot.Services.Windows.Builders;
using Camelot.Services.Windows.Interfaces;
using Camelot.Services.Windows.WinApi;
using Camelot.ViewModels.Services.Interfaces;
using Splat;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Camelot.DependencyInjection
{
    public static class ServicesBootstrapper
    {
        public static void RegisterServices(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            RegisterCommonServices(services, resolver);
            RegisterPlatformSpecificServices(services, resolver);
        }

        private static void RegisterCommonServices(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton<IArchiveProcessorFactory>(() => new ArchiveProcessorFactory(
                resolver.GetRequiredService<IFileService>(),
                resolver.GetRequiredService<IDirectoryService>(),
                resolver.GetRequiredService<IFileNameGenerationService>(),
                resolver.GetRequiredService<IPathService>()
            ));
            services.Register<ICreateArchiveStateService>(() => new CreateArchiveStateService(
                resolver.GetRequiredService<IUnitOfWorkFactory>()
            ));
            services.RegisterLazySingleton<IArchiveTypeMapper>(() => new ArchiveTypeMapper(
                resolver.GetRequiredService<IPathService>(),
                resolver.GetRequiredService<ArchiveTypeMapperConfiguration>()
            ));
            services.RegisterLazySingleton<IArchiveService>(() => new ArchiveService(
                resolver.GetRequiredService<IArchiveTypeMapper>(),
                resolver.GetRequiredService<IPathService>(),
                resolver.GetRequiredService<IOperationsService>(),
                resolver.GetRequiredService<IFileNameGenerationService>()
            ));
            services.RegisterLazySingleton<IFileService>(() => new FileService(
                resolver.GetRequiredService<IPathService>(),
                resolver.GetRequiredService<IEnvironmentFileService>(),
                resolver.GetRequiredService<ILogger>()
            ));
            services.RegisterLazySingleton<IDateTimeProvider>(() => new DateTimeProvider());
            services.RegisterLazySingleton<IMountedDriveService>(() => new MountedDriveService(
                resolver.GetRequiredService<IEnvironmentDriveService>()
            ));
            services.RegisterLazySingleton<IDrivesUpdateService>(() => new DrivesUpdateService(
                resolver.GetRequiredService<IMountedDriveService>(),
                resolver.GetRequiredService<IUnmountedDriveService>(),
                resolver.GetRequiredService<DriveServiceConfiguration>()
            ));
            services.RegisterLazySingleton<IOperationsFactory>(() => new OperationsFactory(
                resolver.GetRequiredService<IDirectoryService>(),
                resolver.GetRequiredService<IFileService>(),
                resolver.GetRequiredService<IPathService>(),
                resolver.GetRequiredService<IFileNameGenerationService>(),
                resolver.GetRequiredService<ILogger>(),
                resolver.GetRequiredService<IArchiveProcessorFactory>()
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
                resolver.GetRequiredService<IDirectoryService>(),
                resolver.GetRequiredService<IResourceOpeningService>()
            ));
            services.RegisterLazySingleton<ILocalizationService>(() => new LocalizationService(
                resolver.GetRequiredService<IUnitOfWorkFactory>()
            ));
            services.RegisterLazySingleton<IThemeService>(() => new ThemeService(
                resolver.GetRequiredService<IUnitOfWorkFactory>(),
                resolver.GetRequiredService<DefaultThemeConfiguration>()
            ));
            services.RegisterLazySingleton<IFileSizeFormatter>(() => new FileSizeFormatter());
            services.RegisterLazySingleton<IPathService>(() => new PathService(
                resolver.GetRequiredService<IEnvironmentPathService>()
            ));
            services.RegisterLazySingleton<IDialogService>(() => new DialogService(
                resolver.GetRequiredService<IMainWindowProvider>()
            ));
            services.RegisterLazySingleton<ISystemDialogService>(() => new SystemDialogService(
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
            services.RegisterLazySingleton<IOpenWithApplicationService>(() => new OpenWithApplicationService(
                resolver.GetRequiredService<IUnitOfWorkFactory>()
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
                resolver.GetRequiredService<IMountedDriveService>(),
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
            services.RegisterLazySingleton<IResourceOpeningService>(() => new ResourceOpeningServiceOpenWith(
                new LinuxResourceOpeningService(
                    resolver.GetRequiredService<IProcessService>(),
                    resolver.GetRequiredService<IShellCommandWrappingService>(),
                    resolver.GetRequiredService<IDesktopEnvironmentService>()),
                resolver.GetRequiredService<IOpenWithApplicationService>(),
                resolver.GetRequiredService<IPathService>()
            ));
            services.RegisterLazySingleton<ITerminalService>(() => new LinuxTerminalService(
                resolver.GetRequiredService<IProcessService>(),
                resolver.GetRequiredService<IUnitOfWorkFactory>(),
                resolver.GetRequiredService<IDesktopEnvironmentService>(),
                resolver.GetRequiredService<IShellCommandWrappingService>()
            ));
            services.RegisterLazySingleton<IMimeTypesReader>(() => new MimeTypesReader());
            services.RegisterLazySingleton<IIniReader>(() => new IniReader());
            services.RegisterLazySingleton<IApplicationService>(() => new LinuxApplicationService(
                resolver.GetRequiredService<IFileService>(),
                resolver.GetRequiredService<IIniReader>(),
                resolver.GetRequiredService<IMimeTypesReader>(),
                resolver.GetRequiredService<IPathService>()
            ));
        }

        private static void RegisterMacServices(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton<IUnmountedDriveService>(() => new MacUnmountedDriveService());
            services.RegisterLazySingleton<ITrashCanService>(() => new MacTrashCanService(
                resolver.GetRequiredService<IMountedDriveService>(),
                resolver.GetRequiredService<IOperationsService>(),
                resolver.GetRequiredService<IPathService>(),
                resolver.GetRequiredService<IFileService>(),
                resolver.GetRequiredService<IEnvironmentService>(),
                resolver.GetRequiredService<IDirectoryService>(),
                resolver.GetRequiredService<IHomeDirectoryProvider>()
            ));
            services.RegisterLazySingleton<IResourceOpeningService>(() => new ResourceOpeningServiceOpenWith(
                new MacResourceOpeningService(resolver.GetRequiredService<IProcessService>()),
                resolver.GetRequiredService<IOpenWithApplicationService>(),
                resolver.GetRequiredService<IPathService>()
            ));
            services.RegisterLazySingleton<IHomeDirectoryProvider>(() => new UnixHomeDirectoryProvider(
                resolver.GetRequiredService<IEnvironmentService>()
            ));
            services.RegisterLazySingleton<ITerminalService>(() => new MacTerminalService(
                resolver.GetRequiredService<IProcessService>(),
                resolver.GetRequiredService<IUnitOfWorkFactory>()
            ));
            services.Register<IApplicationsListLoader>(() => new MacApplicationsListLoader(
                resolver.GetRequiredService<IDirectoryService>()
            ));
            services.Register<IApplicationsAssociationsLoader>(() => new MacApplicationsAssociationsLoader(
                resolver.GetRequiredService<IProcessService>(),
                resolver.GetRequiredService<UtiToExtensionsMappingConfiguration>()
            ));
            services.RegisterLazySingleton<IApplicationService>(() => new MacApplicationService(
                resolver.GetRequiredService<IApplicationsListLoader>(),
                resolver.GetRequiredService<IApplicationsAssociationsLoader>()
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
                resolver.GetRequiredService<IMountedDriveService>(),
                resolver.GetRequiredService<IOperationsService>(),
                resolver.GetRequiredService<IPathService>(),
                resolver.GetRequiredService<IFileService>(),
                resolver.GetRequiredService<IDateTimeProvider>(),
                resolver.GetRequiredService<IProcessService>(),
                resolver.GetRequiredService<IWindowsRemovedFileMetadataBuilderFactory>(),
                resolver.GetRequiredService<IWindowsTrashCanNodeNameGenerator>()
            ));
            services.RegisterLazySingleton<IResourceOpeningService>(() => new ResourceOpeningServiceOpenWith(
                new WindowsResourceOpeningService(
                    resolver.GetRequiredService<IProcessService>()
                ),
                resolver.GetRequiredService<IOpenWithApplicationService>(),
                resolver.GetRequiredService<IPathService>()
            ));
            services.RegisterLazySingleton<ITerminalService>(() => new WindowsTerminalService(
                resolver.GetRequiredService<IProcessService>(),
                resolver.GetRequiredService<IUnitOfWorkFactory>()
            ));
            services.RegisterLazySingleton<IApplicationService>(() => new WindowsApplicationService(
                resolver.GetRequiredService<IEnvironmentService>(),
                resolver.GetRequiredService<IRegexService>(),
                resolver.GetRequiredService<IApplicationInfoProvider>(),
                resolver.GetRequiredService<IRegistryService>()
            ));
            services.RegisterLazySingleton<IApplicationInfoProvider>(() => new WindowsApplicationInfoProvider());
            services.RegisterLazySingleton<IRegistryService>(() => new WindowsRegistryService());
        }
    }
}