using Camelot.Avalonia.Interfaces;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Services;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Abstractions.Drives;
using Camelot.Services.Abstractions.Operations;
using Camelot.Services.Behaviors;
using Camelot.Services.Environment.Enums;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Mac;
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
using Splat;

namespace Camelot.DependencyInjection
{
    public static class ViewModelsBootstrapper
    {
        public static void RegisterViewModels(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            RegisterFactories(services, resolver);
            RegisterCommonViewModels(services, resolver);
            RegisterPlatformSpecificViewModels(services, resolver);
        }

        private static void RegisterFactories(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton<IFileSystemNodeViewModelComparerFactory>(() => new FileSystemNodeViewModelComparerFactory());
            services.RegisterLazySingleton<ITabViewModelFactory>(() => new TabViewModelFactory(
                resolver.GetRequiredService<IPathService>()
            ));
            services.RegisterLazySingleton<IThemeViewModelFactory>(() => new ThemeViewModelFactory(
                resolver.GetRequiredService<IResourceProvider>(),
                resolver.GetRequiredService<ThemesNamesConfiguration>()
            ));
            services.Register<IArchiveTypeViewModelFactory>(() => new ArchiveTypeViewModelFactory(
                resolver.GetRequiredService<ArchiveTypeViewModelFactoryConfiguration>()
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
                resolver.GetRequiredService<IDirectoryService>(),
                resolver.GetRequiredService<IArchiveService>(),
                resolver.GetRequiredService<ISystemDialogService>(),
                resolver.GetRequiredService<IOpenWithApplicationService>()
            ));
            services.RegisterLazySingleton<IBitmapFactory>(() => new BitmapFactory());
            services.Register(() => new MainNodeInfoTabViewModel(
                resolver.GetRequiredService<IFileSizeFormatter>(),
                resolver.GetRequiredService<IPathService>(),
                resolver.GetRequiredService<IBitmapFactory>(),
                resolver.GetRequiredService<ImagePreviewConfiguration>()
            ));
            services.RegisterLazySingleton<IDriveViewModelFactory>(() => new DriveViewModelFactory(
                resolver.GetRequiredService<IFileSizeFormatter>(),
                resolver.GetRequiredService<IPathService>(),
                resolver.GetRequiredService<IFilesOperationsMediator>(),
                resolver.GetRequiredService<IUnmountedDriveService>()
            ));
            services.RegisterLazySingleton<IFavouriteDirectoryViewModelFactory>(() => new FavouriteDirectoryViewModelFactory(
                resolver.GetRequiredService<IFilesOperationsMediator>(),
                resolver.GetRequiredService<IDirectoryService>()
            ));
            services.RegisterLazySingleton<IFavouriteDirectoriesListViewModel>(() => new FavouriteDirectoriesListViewModel(
                resolver.GetRequiredService<IFavouriteDirectoryViewModelFactory>(),
                resolver.GetRequiredService<IHomeDirectoryProvider>()
            ));
        }

        private static void RegisterCommonViewModels(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton<IFilesOperationsMediator>(() => new FilesOperationsMediator(
                resolver.GetRequiredService<IDirectoryService>()
            ));
            services.Register(() => new TerminalSettingsViewModel(
                resolver.GetRequiredService<ITerminalService>()
            ));
            services.Register(() => new GeneralSettingsViewModel(
                resolver.GetRequiredService<LanguageSettingsViewModel>(),
                resolver.GetRequiredService<ThemeSettingsViewModel>()
            ));
            services.Register(() => new LanguageSettingsViewModel(
                resolver.GetRequiredService<ILocalizationService>(),
                resolver.GetRequiredService<ILanguageManager>()
            ));
            services.Register(() => new ThemeSettingsViewModel(
                resolver.GetRequiredService<IThemeService>(),
                resolver.GetRequiredService<IThemeViewModelFactory>()
            ));
            services.Register(() => new SettingsDialogViewModel(
                resolver.GetRequiredService<GeneralSettingsViewModel>(),
                resolver.GetRequiredService<TerminalSettingsViewModel>()
            ));
            services.RegisterLazySingleton(() => new FilePropertiesBehavior(
                resolver.GetRequiredService<IDialogService>()
            ));
            services.RegisterLazySingleton(() => new DirectoryPropertiesBehavior(
                resolver.GetRequiredService<IDialogService>()
            ));
            services.Register(() => new AboutDialogViewModel(
                resolver.GetRequiredService<IApplicationVersionProvider>(),
                resolver.GetRequiredService<IResourceOpeningService>(),
                resolver.GetRequiredService<AboutDialogConfiguration>()
            ));
            services.Register(() => new CreateArchiveDialogViewModel(
                resolver.GetRequiredService<IDirectoryService>(),
                resolver.GetRequiredService<IFileService>(),
                resolver.GetRequiredService<IArchiveTypeViewModelFactory>(),
                resolver.GetRequiredService<ISystemDialogService>(),
                resolver.GetRequiredService<ICreateArchiveStateService>()
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
            services.Register(() => new RenameNodeDialogViewModel(
                resolver.GetRequiredService<IDirectoryService>(),
                resolver.GetRequiredService<IFileService>(),
                resolver.GetRequiredService<IPathService>()
            ));
            services.RegisterLazySingleton<IOperationsStateViewModel>(() => new OperationsStatesListViewModel(
                resolver.GetRequiredService<IOperationsStateService>(),
                resolver.GetRequiredService<IOperationStateViewModelFactory>(),
                resolver.GetRequiredService<IApplicationDispatcher>(),
                resolver.GetRequiredService<IDialogService>(),
                resolver.GetRequiredService<OperationsStatesConfiguration>()
            ));
            services.Register(() => new RemoveNodesConfirmationDialogViewModel(
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
            services.Register(() => new OpenWithDialogViewModel(
                resolver.GetRequiredService<IApplicationService>(),
                resolver.GetRequiredService<OpenWithDialogConfiguration>()
            ));
            services.Register<ISearchViewModel>(() => new SearchViewModel(
                resolver.GetRequiredService<IRegexService>(),
                resolver.GetRequiredService<IResourceProvider>(),
                resolver.GetRequiredService<IApplicationDispatcher>(),
                resolver.GetRequiredService<SearchViewModelConfiguration>()
            ));
            services.RegisterLazySingleton<IDrivesListViewModel>(() => new DrivesListViewModel(
                resolver.GetRequiredService<IMountedDriveService>(),
                resolver.GetRequiredService<IUnmountedDriveService>(),
                resolver.GetRequiredService<IDrivesUpdateService>(),
                resolver.GetRequiredService<IDriveViewModelFactory>(),
                resolver.GetRequiredService<IApplicationDispatcher>()
            ));
            services.RegisterLazySingleton<ITopOperationsViewModel>(() => new TopOperationsViewModel(
                resolver.GetRequiredService<ITerminalService>(),
                resolver.GetRequiredService<IDirectoryService>(),
                resolver.GetRequiredService<IFilesOperationsMediator>(),
                resolver.GetRequiredService<IDialogService>(),
                resolver.GetRequiredService<IPathService>(),
                resolver.GetRequiredService<IArchiveService>(),
                resolver.GetRequiredService<INodesSelectionService>(),
                resolver.GetRequiredService<ISystemDialogService>()
            ));
            services.RegisterLazySingleton<IOperationStateViewModelFactory>(() => new OperationStateViewModelFactory(
                resolver.GetRequiredService<IPathService>()
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

        private static void RegisterPlatformSpecificViewModels(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            var platformService = resolver.GetRequiredService<IPlatformService>();
            var platform = platformService.GetPlatform();
            if (platform is Platform.MacOs)
            {
                RegisterMacViewModels(services, resolver);
            }
        }

        private static void RegisterMacViewModels(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton(() => new MacDirectoryOpeningBehavior(
                resolver.GetRequiredService<FileOpeningBehavior>(),
                resolver.GetRequiredService<DirectoryOpeningBehavior>()
            ));
            services.RegisterLazySingleton<IFileSystemNodeViewModelFactory>(() => new FileSystemNodeViewModelFactory(
                resolver.GetRequiredService<FileOpeningBehavior>(),
                resolver.GetRequiredService<MacDirectoryOpeningBehavior>(),
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
                resolver.GetRequiredService<IDirectoryService>(),
                resolver.GetRequiredService<IArchiveService>(),
                resolver.GetRequiredService<ISystemDialogService>(),
                resolver.GetRequiredService<IOpenWithApplicationService>()
            ));
        }
    }
}