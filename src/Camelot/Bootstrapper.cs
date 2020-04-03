using System;
using ApplicationDispatcher.Implementations;
using ApplicationDispatcher.Interfaces;
using Camelot.DataAccess.LiteDb;
using Camelot.DataAccess.UnitOfWork;
using Camelot.FileSystemWatcherWrapper.Implementations;
using Camelot.FileSystemWatcherWrapper.Interfaces;
using Camelot.Services.Behaviors.Implementations;
using Camelot.Services.Implementations;
using Camelot.Services.Interfaces;
using Camelot.Services.Operations.Implementations;
using Camelot.Services.Operations.Interfaces;
using Camelot.TaskPool.Interfaces;
using Camelot.ViewModels.Factories.Implementations;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Implementations.MainWindow;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
using Camelot.ViewModels.Implementations.Menu;
using Camelot.ViewModels.Interfaces.MainWindow;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Services.Implementations;
using Camelot.ViewModels.Services.Interfaces;
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
                resolver.GetService<IResourceOpeningService>(),
                resolver.GetService<IFileService>(),
                resolver.GetService<IPathService>()
            ));
            services.RegisterLazySingleton<IDirectoryService>(() => new DirectoryService(
                resolver.GetService<IPathService>()
            ));
            services.RegisterLazySingleton<IProcessService>(() => new ProcessService());
            services.RegisterLazySingleton<IResourceOpeningService>(() => new ResourceOpeningService(
                resolver.GetService<IProcessService>(),
                resolver.GetService<IPlatformService>()
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
            services.RegisterLazySingleton<IApplicationDispatcher>(() => new AvaloniaDispatcher());
            services.RegisterLazySingleton<IFileSizeFormatter>(() => new FileSizeFormatter());
            services.RegisterLazySingleton<IPlatformService>(() => new PlatformService());
            services.RegisterLazySingleton<IApplicationCloser>(() => new ApplicationCloser());
            services.RegisterLazySingleton<IClipboardService>(() => new ClipboardService());
            services.RegisterLazySingleton<IPathService>(() => new PathService());
            services.RegisterLazySingleton<IMainWindowProvider>(() => new MainWindowProvider());
            services.RegisterLazySingleton<IDialogService>(() => new DialogService(
                resolver.GetService<IMainWindowProvider>()
            ));
            services.RegisterLazySingleton<IClipboardOperationsService>(() => new ClipboardOperationsService(
                resolver.GetService<IClipboardService>(),
                resolver.GetService<IOperationsService>()
            ));
            services.RegisterLazySingleton<IApplicationVersionProvider>(() => new ApplicationVersionProvider());
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
                resolver.GetService<IResourceOpeningService>()
            ));
            services.Register(() => new CreateDirectoryDialogViewModel());
            services.Register<IOperationsViewModel>(() => new OperationsViewModel(
                resolver.GetService<IFilesOperationsMediator>(),
                resolver.GetService<IOperationsService>(),
                resolver.GetService<IClipboardOperationsService>(),
                resolver.GetService<IFilesSelectionService>(),
                resolver.GetService<IDialogService>(),
                resolver.GetService<IDirectoryService>()
            ));
            services.Register(() => new MenuViewModel(
                resolver.GetService<IApplicationCloser>(),
                resolver.GetService<IDialogService>()
            ));
            services.RegisterLazySingleton(() => new MainWindowViewModel(
                resolver.GetService<IFilesOperationsMediator>(),
                resolver.GetService<IOperationsViewModel>(),
                CreateFilesPanelViewModel(resolver, "Left"),
                CreateFilesPanelViewModel(resolver, "Right"),
                resolver.GetService<MenuViewModel>()
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
                resolver.GetService<IFileSizeFormatter>()
            );

            return filesPanelViewModel;
        }
    }
}