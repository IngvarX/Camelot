using System;
using System.IO;
using System.Reflection;
using ApplicationDispatcher.Implementations;
using ApplicationDispatcher.Interfaces;
using Camelot.DataAccess.Configuration;
using Camelot.DataAccess.LiteDb;
using Camelot.DataAccess.UnitOfWork;
using Camelot.FileSystemWatcherWrapper.Implementations;
using Camelot.FileSystemWatcherWrapper.Interfaces;
using Camelot.Services;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Operations;
using Camelot.Services.Behaviors;
using Camelot.Services.Environment.Enums;
using Camelot.Services.Environment.Implementations;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Implementations;
using Camelot.Services.Linux;
using Camelot.Services.Mac;
using Camelot.Services.Operations;
using Camelot.Services.Windows;
using Camelot.TaskPool.Interfaces;
using Camelot.ViewModels.Configuration;
using Camelot.ViewModels.Factories.Implementations;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Implementations.MainWindow;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
using Camelot.ViewModels.Implementations.MainWindow.OperationsStates;
using Camelot.ViewModels.Implementations.Menu;
using Camelot.ViewModels.Interfaces.MainWindow;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Interfaces.MainWindow.OperationsStates;
using Camelot.ViewModels.Interfaces.Menu;
using Camelot.ViewModels.Services.Implementations;
using Camelot.ViewModels.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Splat;

namespace Camelot
{
    public static class Bootstrapper
    {
        public static void Register(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            RegisterConfiguration(services);
            RegisterEnvironmentServices(services);
            RegisterAvaloniaServices(services);
            RegisterFileSystemWatcherServices(services);
            RegisterTaskPool(services, resolver);
            RegisterDataAccess(services, resolver);
            RegisterServices(services, resolver);
            RegisterPlatformSpecificServices(services, resolver);
            RegisterViewModels(services, resolver);
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
            var assemblyLocation = Assembly.GetEntryAssembly()?.Location;
            var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
            var databaseConfiguration = new DatabaseConfiguration
            {
                ConnectionString = Path.Combine(assemblyDirectory, databaseName)
            };

            services.RegisterConstant(databaseConfiguration);
        }

        private static void RegisterEnvironmentServices(IMutableDependencyResolver services)
        {
            services.RegisterLazySingleton<IEnvironmentService>(() => new EnvironmentService());
            services.RegisterLazySingleton<IProcessService>(() => new ProcessService());
            services.Register<IPlatformService>(() => new PlatformService());
        }

        private static void RegisterAvaloniaServices(IMutableDependencyResolver services)
        {
            services.RegisterLazySingleton<IApplicationCloser>(() => new ApplicationCloser());
            services.RegisterLazySingleton<IApplicationDispatcher>(() => new AvaloniaDispatcher());
            services.RegisterLazySingleton<IApplicationVersionProvider>(() => new ApplicationVersionProvider());
        }

        private static void RegisterFileSystemWatcherServices(IMutableDependencyResolver services)
        {
            services.RegisterLazySingleton<IFileSystemWatcherWrapperFactory>(() => new FileSystemWatcherWrapperFactory());
        }

        private static void RegisterTaskPool(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton<ITaskPool>(() => new TaskPool.Implementations.TaskPool(
                resolver.GetService<IEnvironmentService>()
            ));
        }

        private static void RegisterDataAccess(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton<IUnitOfWorkFactory>(() => new LiteDbUnitOfWorkFactory(
                resolver.GetService<DatabaseConfiguration>()
            ));
            services.RegisterLazySingleton<IClipboardService>(() => new ClipboardService());
            services.RegisterLazySingleton<IMainWindowProvider>(() => new MainWindowProvider());
        }

        private static void RegisterServices(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton<IFileService>(() => new FileService(
                resolver.GetService<IPathService>()
            ));
            services.RegisterLazySingleton<IDriveService>(() => new DriveService());
            services.Register<IOperationsFactory>(() => new OperationsFactory(
                resolver.GetService<ITaskPool>(),
                resolver.GetService<IDirectoryService>(),
                resolver.GetService<IFileService>(),
                resolver.GetService<IPathService>()
            ));
            services.RegisterLazySingleton<IFileSystemWatchingService>(() => new FileSystemWatchingService(
                resolver.GetService<IFileSystemWatcherWrapperFactory>()
            ));
            services.RegisterLazySingleton<IFilesSelectionService>(() => new FilesSelectionService());
            services.RegisterLazySingleton<IOperationsService>(() => new OperationsService(
                resolver.GetService<IOperationsFactory>(),
                resolver.GetService<IDirectoryService>(),
                resolver.GetService<IResourceOpeningService>(),
                resolver.GetService<IFileService>(),
                resolver.GetService<IPathService>(),
                resolver.GetService<IOperationsStateService>()
            ));
            services.RegisterLazySingleton<IDirectoryService>(() => new DirectoryService(
                resolver.GetService<IPathService>()
            ));
            services.RegisterLazySingleton<IFileSystemWatcherWrapperFactory>(() => new FileSystemWatcherWrapperFactory());
            services.Register<IFileSystemWatchingService>(() => new FileSystemWatchingService(
                resolver.GetService<IFileSystemWatcherWrapperFactory>()
            ));
            services.RegisterLazySingleton(() => new FileOpeningBehavior(
                resolver.GetService<IResourceOpeningService>()
            ));
            services.RegisterLazySingleton(() => new DirectoryOpeningBehavior(
                resolver.GetService<IDirectoryService>()
            ));
            services.RegisterLazySingleton<IFileSizeFormatter>(() => new FileSizeFormatter());
            services.RegisterLazySingleton<IPathService>(() => new PathService());
            services.RegisterLazySingleton<IDialogService>(() => new DialogService(
                resolver.GetService<IMainWindowProvider>()
            ));
            services.RegisterLazySingleton<IClipboardOperationsService>(() => new ClipboardOperationsService(
                resolver.GetService<IClipboardService>(),
                resolver.GetService<IOperationsService>(),
                resolver.GetService<IEnvironmentService>()
            ));
            services.RegisterLazySingleton<IOperationsStateService>(() => new OperationsStateService());
        }

        private static void RegisterPlatformSpecificServices(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            var platformService = resolver.GetService<IPlatformService>();
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
            services.RegisterLazySingleton<ITrashCanService>(() => new LinuxTrashCanService(
                resolver.GetService<IDriveService>(),
                resolver.GetService<IOperationsService>(),
                resolver.GetService<IPathService>(),
                resolver.GetService<IFileService>(),
                resolver.GetService<IEnvironmentService>(),
                resolver.GetService<IDirectoryService>()
            ));
            services.RegisterLazySingleton<IResourceOpeningService>(() => new LinuxResourceOpeningService(
                resolver.GetService<IProcessService>(),
                resolver.GetService<IEnvironmentService>()
            ));
        }

        private static void RegisterMacServices(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton<ITrashCanService>(() => new MacTrashCanService(
                resolver.GetService<IDriveService>(),
                resolver.GetService<IOperationsService>(),
                resolver.GetService<IPathService>(),
                resolver.GetService<IFileService>(),
                resolver.GetService<IEnvironmentService>()
            ));
            services.RegisterLazySingleton<IResourceOpeningService>(() => new MacResourceOpeningService(
                resolver.GetService<IProcessService>()
            ));
        }

        private static void RegisterWindowsServices(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton<ITrashCanService>(() => new WindowsTrashCanService(
                resolver.GetService<IDriveService>(),
                resolver.GetService<IOperationsService>(),
                resolver.GetService<IPathService>(),
                resolver.GetService<IFileService>(),
                resolver.GetService<IEnvironmentService>(),
                resolver.GetService<IProcessService>()
            ));
            services.RegisterLazySingleton<IResourceOpeningService>(() => new WindowsResourceOpeningService(
                resolver.GetService<IProcessService>()
            ));
        }

        private static void RegisterViewModels(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton<IFilesOperationsMediator>(() => new FilesOperationsMediator(
                resolver.GetService<IDirectoryService>()
            ));
            services.RegisterLazySingleton<ITabViewModelFactory>(() => new TabViewModelFactory(
                resolver.GetService<IPathService>()
            ));
            services.RegisterLazySingleton<IFileSystemNodeViewModelFactory>(() => new FileSystemNodeViewModelFactory(
                resolver.GetService<FileOpeningBehavior>(),
                resolver.GetService<DirectoryOpeningBehavior>(),
                resolver.GetService<IFileSizeFormatter>(),
                resolver.GetService<IPathService>(),
                resolver.GetService<IOperationsService>(),
                resolver.GetService<IClipboardOperationsService>(),
                resolver.GetService<IFilesOperationsMediator>()

            ));
            services.Register(() => new AboutDialogViewModel(
                resolver.GetService<IApplicationVersionProvider>(),
                resolver.GetService<IResourceOpeningService>(),
                resolver.GetService<AboutDialogConfiguration>()
            ));
            services.Register(() => new CreateDirectoryDialogViewModel());
            services.Register<IOperationsStateViewModel>(() => new OperationsStateViewModel(
                resolver.GetService<IOperationsStateService>()
            ));
            services.Register(() => new RemoveNodesConfirmationDialogViewModel(
                resolver.GetService<IPathService>()
            ));
            services.Register<IOperationsViewModel>(() => new OperationsViewModel(
                resolver.GetService<IFilesOperationsMediator>(),
                resolver.GetService<IOperationsService>(),
                resolver.GetService<IFilesSelectionService>(),
                resolver.GetService<IDialogService>(),
                resolver.GetService<IDirectoryService>(),
                resolver.GetService<ITrashCanService>()
            ));
            services.Register<IMenuViewModel>(() => new MenuViewModel(
                resolver.GetService<IApplicationCloser>(),
                resolver.GetService<IDialogService>()
            ));
            services.RegisterLazySingleton(() => new MainWindowViewModel(
                resolver.GetService<IFilesOperationsMediator>(),
                resolver.GetService<IOperationsViewModel>(),
                CreateFilesPanelViewModel(resolver, "Left"),
                CreateFilesPanelViewModel(resolver, "Right"),
                resolver.GetService<IMenuViewModel>(),
                resolver.GetService<IOperationsStateViewModel>()
            ));
        }

        private static IFilesPanelViewModel CreateFilesPanelViewModel(
            IReadonlyDependencyResolver resolver,
            string panelKey)
        {
            var filesPanelStateService = new FilesPanelStateService(
                resolver.GetService<IUnitOfWorkFactory>(),
                panelKey
            );
            var filesPanelViewModel = new FilesPanelViewModel(
                resolver.GetService<IFileService>(),
                resolver.GetService<IDirectoryService>(),
                resolver.GetService<IFilesSelectionService>(),
                resolver.GetService<IFileSystemNodeViewModelFactory>(),
                resolver.GetService<IFileSystemWatchingService>(),
                resolver.GetService<IApplicationDispatcher>(),
                filesPanelStateService,
                resolver.GetService<ITabViewModelFactory>(),
                resolver.GetService<IFileSizeFormatter>(),
                resolver.GetService<IClipboardOperationsService>()
            );

            return filesPanelViewModel;
        }
    }
}