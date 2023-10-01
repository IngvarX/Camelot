using Avalonia.Media.Imaging;
using Camelot.Images;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Behaviors;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Interfaces.Behaviors;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Nodes;
using Camelot.ViewModels.Services.Interfaces;
using ReactiveUI;
using System;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels.Nodes;

public class FileViewModel : FileSystemNodeViewModelBase, IFileViewModel
{
    private readonly IFileSizeFormatter _fileSizeFormatter;
    private readonly IFileTypeMapper _fileTypeMapper;
    private readonly IShellIconsCacheService _shellIconsCacheService;
    private readonly IIconsSettingsService _iconsSettingsService;
    private long _size;
    private Bitmap _shellIcon = null;
    private bool? _useShellIcon = null;

    // Helper to load icon only on demand.
    // (Reason: Can't use icon member itself, since null is valid value,
    // in case file has no shell icon)
    private bool _loadedShellIcon = false;
    public string Extension { get; set; }

    public long Size
    {
        get => _size;
        set
        {
            this.RaiseAndSetIfChanged(ref _size, value);
            this.RaisePropertyChanged(nameof(FormattedSize));
        }
    }

    public string FormattedSize => _fileSizeFormatter.GetFormattedSize(Size);

    public FileContentType Type => _fileTypeMapper.GetFileType(Extension);

    public FileViewModel(
        IFileSystemNodeOpeningBehavior fileSystemNodeOpeningBehavior,
        IFileSystemNodePropertiesBehavior fileSystemNodePropertiesBehavior,
        IFileSystemNodeFacade fileSystemNodeFacade,
        bool shouldShowOpenSubmenu,
        IFileSizeFormatter fileSizeFormatter,
        IFileTypeMapper fileTypeMapper,
        IShellIconsCacheService shellIconsCacheService,
        IIconsSettingsService iconsSettingsService)
        : base(
            fileSystemNodeOpeningBehavior,
            fileSystemNodePropertiesBehavior,
            fileSystemNodeFacade,
            shouldShowOpenSubmenu)
    {
        _fileSizeFormatter = fileSizeFormatter;
        _fileTypeMapper = fileTypeMapper;
        _shellIconsCacheService = shellIconsCacheService;
        _iconsSettingsService = iconsSettingsService;
    }

    public Bitmap ShellIcon
    {
        get
        {
            if (!_loadedShellIcon)
            {
                var imageModel = _shellIconsCacheService.GetIcon(FullPath);
                _shellIcon = FromImageModel(imageModel);
                _loadedShellIcon = true;
            }
            return _shellIcon;
        }
    }

    private IconsType GetUserSelectedType()
    {
        var model = _iconsSettingsService.GetIconsSettings();
        return  model.SelectedIconsType;
    }
    public bool UseShellIcons
    {
        get
        {
            if (_useShellIcon == null)
                _useShellIcon = ComputeUseShellIcons();
            return (bool)_useShellIcon;
        }
    }
    private bool ComputeUseShellIcons()
    {
        var selected = GetUserSelectedType();
        if (selected == IconsType.Builtin)
            return false;

        // still need to some check, before can return true
        // if not first time, and already have value
        if (!_loadedShellIcon)
        {
            var imageModel = _shellIconsCacheService.GetIcon(FullPath);
            _shellIcon = FromImageModel(imageModel);
            _loadedShellIcon = true;
        }

        if (_shellIcon != null)
        {
            return true;
        }
        else
        {
            // file has no shell icon, so fallback to use builtin icons
            return false;
        }
    }

    private static Bitmap FromImageModel(ImageModel imageModel)
    {
        if (imageModel == null)
            throw new ArgumentNullException(nameof(imageModel));

        var concreteImage = imageModel as ConcreteImage;
        var result = concreteImage.Bitmap;
        return result;
    }
}