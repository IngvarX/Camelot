using System;
using ApplicationDispatcher.Implementations;
using ApplicationDispatcher.Interfaces;
using Camelot.DataAccess.LiteDb;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Factories.Implementations;
using Camelot.Factories.Interfaces;
using Camelot.FileSystemWatcherWrapper.Implementations;
using Camelot.FileSystemWatcherWrapper.Interfaces;
using Camelot.Mediator.Implementations;
using Camelot.Mediator.Interfaces;
using Camelot.Services.Behaviors.Implementations;
using Camelot.Services.Implementations;
using Camelot.Services.Interfaces;
using Camelot.Services.Operations.Implementations;
using Camelot.Services.Operations.Interfaces;
using Camelot.TaskPool.Interfaces;
using Camelot.ViewModels;
using Camelot.ViewModels.MainWindow;
using Camelot.ViewModels.Menu;
using Splat;

namespace Camelot
{
    public static class Bootstrapper
    {
        public static void Register(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            RegisterDataAccess(services, resolver);
            RegisterServices(services, resolver);
            RegisterViewModels(services, resolver);
        }

        private static void RegisterDataAccess(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton<IUnitOfWorkFactory>(() => new LiteDbUnitOfWorkFactory());
        }

        private static void RegisterServices(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton<IFileService>(() => new FileService(
                resolver.GetService<IPathService>()
            ));
            services.RegisterLazySingleton<IFileSystemWatcherWrapperFactory>(() => new FileSystemWatcherWrapperFactory());
            services.RegisterLazySingleton<ITaskPool>(() => new TaskPool.Implementations.TaskPool(Environment.ProcessorCount));
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
                resolver.GetService<IFileOpeningService>(),
                resolver.GetService<IFileService>(),
                resolver.GetService<IPathService>()
            ));
            services.RegisterLazySingleton<IDirectoryService>(() => new DirectoryService(
                resolver.GetService<IPathService>()
            ));
            services.RegisterLazySingleton<IProcessService>(() => new ProcessService());
            services.RegisterLazySingleton<IFileOpeningService>(() => new FileOpeningService(
                resolver.GetService<IProcessService>(),
                resolver.GetService<IPlatformService>()
            ));
            services.RegisterLazySingleton<IFileSystemWatcherWrapperFactory>(() => new FileSystemWatcherWrapperFactory());
            services.Register<IFileSystemWatchingService>(() => new FileSystemWatchingService(
                resolver.GetService<IFileSystemWatcherWrapperFactory>()
            ));
            services.RegisterLazySingleton(() => new FileOpeningBehavior(
                resolver.GetService<IFileOpeningService>()
            ));
            services.RegisterLazySingleton(() => new DirectoryOpeningBehavior(
                resolver.GetService<IDirectoryService>()
            ));
            services.RegisterLazySingleton<IApplicationDispatcher>(() => new AvaloniaDispatcher());
            services.RegisterLazySingleton<IFileSizeFormatter>(() => new FileSizeFormatter());
            services.RegisterLazySingleton<IPlatformService>(() => new PlatformService());
            services.RegisterLazySingleton<IApplicationCloser>(() => new ApplicationCloser());
            services.RegisterLazySingleton<IClipboardService>(() => new ClipboardService());
            services.RegisterLazySingleton<IPathService>(() => new PathService());
            services.RegisterLazySingleton<IClipboardOperationsService>(() => new ClipboardOperationsService(
                resolver.GetService<IClipboardService>(),
                resolver.GetService<IOperationsService>()
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
            services.RegisterLazySingleton<IFileViewModelFactory>(() => new FileViewModelFactory(
                resolver.GetService<FileOpeningBehavior>(),
                resolver.GetService<DirectoryOpeningBehavior>(),
                resolver.GetService<IFileSizeFormatter>(),
                resolver.GetService<IPathService>()

            ));
            services.Register(() => new OperationsViewModel(
                resolver.GetService<IFilesOperationsMediator>(),
                resolver.GetService<IOperationsService>(),
                resolver.GetService<IClipboardOperationsService>(),
                resolver.GetService<IFilesSelectionService>()
            ));
            services.Register(() => new MenuViewModel(
                resolver.GetService<IApplicationCloser>()
            ));
            services.RegisterLazySingleton(() => new MainWindowViewModel(
                resolver.GetService<IFilesOperationsMediator>(),
                resolver.GetService<OperationsViewModel>(),
                CreateFilesPanelViewModel(resolver, "Left"),
                CreateFilesPanelViewModel(resolver, "Right"),
                resolver.GetService<MenuViewModel>()
            ));
        }

        private static FilesPanelViewModel CreateFilesPanelViewModel(
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
                resolver.GetService<IFileViewModelFactory>(),
                resolver.GetService<IFileSystemWatchingService>(),
                resolver.GetService<IApplicationDispatcher>(),
                filesPanelStateService,
                resolver.GetService<ITabViewModelFactory>()
            );

            return filesPanelViewModel;
        }
    }
}