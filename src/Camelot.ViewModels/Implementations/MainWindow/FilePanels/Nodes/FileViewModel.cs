using Avalonia.Media.Imaging;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Behaviors;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Interfaces.Behaviors;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Nodes;
using Camelot.ViewModels.Services.Interfaces;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels.Nodes;

public class FileViewModel : FileSystemNodeViewModelBase, IFileViewModel
{
    private readonly IFileSizeFormatter _fileSizeFormatter;
    private readonly IFileTypeMapper _fileTypeMapper;
    private readonly IShellIconsCacheService _shellIconsCacheService;
    private readonly IconsType _iconsType;
    
    private long _size;
    private IBitmap _shellIcon;
    private bool? _useShellIcon;

    // Helper to load icon only on demand.
    // (Reason: Can't use icon member itself, since null is valid value,
    // in case file has no shell icon)
    private bool _loadedShellIcon;
    
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
        IconsType iconsType)
        : base(
            fileSystemNodeOpeningBehavior,
            fileSystemNodePropertiesBehavior,
            fileSystemNodeFacade,
            shouldShowOpenSubmenu)
    {
        _fileSizeFormatter = fileSizeFormatter;
        _fileTypeMapper = fileTypeMapper;
        _shellIconsCacheService = shellIconsCacheService;
        _iconsType = iconsType;
    }

    public IBitmap ShellIcon
    {
        get
        {
            TryLoadIcon();
            
            return _shellIcon;
        }
    }

    public bool UseShellIcons
    {
        get
        {
            _useShellIcon ??= ComputeUseShellIcons();
            
            return (bool)_useShellIcon;
        }
    }
    
    private bool ComputeUseShellIcons()
    {
        if (_iconsType == IconsType.Builtin)
            return false;

        // still need to some check, before can return true
        // if not first time, and already have value
        TryLoadIcon();

        // file has no shell icon, so fallback to use builtin icons
        return _shellIcon != null;
    }

    private void TryLoadIcon()
    {
        if (_loadedShellIcon)
        {
            return;
        }
        
        _shellIcon = _shellIconsCacheService.GetIcon(FullPath).Bitmap;
        _loadedShellIcon = true;
    }
}