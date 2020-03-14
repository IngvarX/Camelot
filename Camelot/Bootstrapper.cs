using System;
using Camelot.FileSystemWatcherWrapper.Implementations;
using Camelot.FileSystemWatcherWrapper.Interfaces;
using Camelot.Services.Implementations;
using Camelot.Services.Interfaces;
using Camelot.Services.Operations.Implementations;
using Camelot.Services.Operations.Interfaces;
using Camelot.TaskPool.Interfaces;
using Camelot.ViewModels;
using Camelot.ViewModels.MainWindow;
using Splat;

namespace Camelot
{
    public static class Bootstrapper
    {
        public static void Register(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            RegisterServices(services, resolver);
            RegisterViewModels(services, resolver);
        }

        private static void RegisterServices(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.Register<IFileService>(() => new FileService());
            services.Register<IFileSystemWatcherWrapperFactory>(() => new FileSystemWatcherWrapperFactory());
            services.Register<ITaskPool>(() => new TaskPool.Implementations.TaskPool(Environment.ProcessorCount));
            services.Register<IOperationsFactory>(() => new OperationsFactory(
                resolver.GetService<ITaskPool>()));
            services.Register<IFileSystemWatchingService>(() => new FileSystemWatchingService(
                resolver.GetService<IFileSystemWatcherWrapperFactory>()));

        }

        private static void RegisterViewModels(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.Register(() => new OperationsViewModel());
            services.Register(() => new FilesPanelViewModel(
                resolver.GetService<IFileService>()));

            services.Register(() => new MainWindowViewModel(
                resolver.GetService<OperationsViewModel>(),
                resolver.GetService<FilesPanelViewModel>(),
                resolver.GetService<FilesPanelViewModel>()));
        }
    }
}