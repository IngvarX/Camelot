using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Behaviors;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Nodes;
using Camelot.ViewModels.Interfaces.Behaviors;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Nodes;
using Camelot.ViewModels.Services.Interfaces;

namespace Camelot.ViewModels.Factories.Implementations;

public class FileSystemNodeViewModelFactory : IFileSystemNodeViewModelFactory
{
    private readonly IFileSystemNodeOpeningBehavior _fileOpeningBehavior;
    private readonly IFileSystemNodeOpeningBehavior _directoryOpeningBehavior;
    private readonly IFileSizeFormatter _fileSizeFormatter;
    private readonly IPathService _pathService;
    private readonly IFilesOperationsMediator _filesOperationsMediator;
    private readonly IFileSystemNodePropertiesBehavior _filePropertiesBehavior;
    private readonly IFileSystemNodePropertiesBehavior _directoryPropertiesBehavior;
    private readonly IFileService _fileService;
    private readonly IDirectoryService _directoryService;
    private readonly IFileSystemNodeFacade _fileSystemNodeFacade;
    private readonly IFileTypeMapper _fileTypeMapper;

    public FileSystemNodeViewModelFactory(
        IFileSystemNodeOpeningBehavior fileOpeningBehavior,
        IFileSystemNodeOpeningBehavior directoryOpeningBehavior,
        IFileSizeFormatter fileSizeFormatter,
        IPathService pathService,
        IFilesOperationsMediator filesOperationsMediator,
        IFileSystemNodePropertiesBehavior filePropertiesBehavior,
        IFileSystemNodePropertiesBehavior directoryPropertiesBehavior,
        IFileService fileService,
        IDirectoryService directoryService,
        IFileSystemNodeFacade fileSystemNodeFacade,
        IFileTypeMapper fileTypeMapper)
    {
        _fileOpeningBehavior = fileOpeningBehavior;
        _directoryOpeningBehavior = directoryOpeningBehavior;
        _fileSizeFormatter = fileSizeFormatter;
        _pathService = pathService;
        _filesOperationsMediator = filesOperationsMediator;
        _filePropertiesBehavior = filePropertiesBehavior;
        _directoryPropertiesBehavior = directoryPropertiesBehavior;
        _fileService = fileService;
        _directoryService = directoryService;
        _fileSystemNodeFacade = fileSystemNodeFacade;
        _fileTypeMapper = fileTypeMapper;
    }

    public IFileSystemNodeViewModel Create(string path)
    {
        if (_fileService.CheckIfExists(path))
        {
            var fileModel = _fileService.GetFile(path);

            return fileModel is null ? null : Create(fileModel);
        }

        if (_directoryService.CheckIfExists(path))
        {
            var directoryModel = _directoryService.GetDirectory(path);

            return directoryModel is null ? null : Create(directoryModel, false);
        }

        return null;
    }

    public IFileSystemNodeViewModel Create(FileModel fileModel)
    {
        var fileViewModel = new FileViewModel(
            _fileOpeningBehavior,
            _filePropertiesBehavior,
            _fileSystemNodeFacade,
            false,
            _fileSizeFormatter,
            _fileTypeMapper)
        {
            FullPath = fileModel.FullPath,
            Size = fileModel.SizeBytes,
            LastModifiedDateTime = fileModel.LastModifiedDateTime,
            Name = _pathService.GetFileNameWithoutExtension(fileModel.Name),
            Extension = _pathService.GetExtension(fileModel.Name),
            FullName = _pathService.GetFileName(fileModel.Name)
        };

        return fileViewModel;
    }

    public IFileSystemNodeViewModel Create(DirectoryModel directoryModel, bool isParentDirectory)
    {
        var directoryViewModel = new DirectoryViewModel(
            _directoryOpeningBehavior,
            _directoryPropertiesBehavior,
            _fileSystemNodeFacade,
            true,
            _filesOperationsMediator)
        {
            FullPath = directoryModel.FullPath,
            Name = directoryModel.Name,
            LastModifiedDateTime = directoryModel.LastModifiedDateTime,
            FullName = directoryModel.Name,
            IsParentDirectory = isParentDirectory
        };

        return directoryViewModel;
    }
}