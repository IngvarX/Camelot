using System;
using Avalonia.Media.Imaging;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Configuration;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Interfaces.Properties;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.Dialogs.Properties;

public class MainNodeInfoTabViewModel : ViewModelBase, IMainNodeInfoTabViewModel
{
    private readonly IFileSizeFormatter _fileSizeFormatter;
    private readonly IPathService _pathService;
    private readonly IBitmapFactory _bitmapFactory;
    private readonly IFileTypeMapper _fileTypeMapper;
    private readonly ImagePreviewConfiguration _configuration;
    private string _fullPath;
    private long _size;

    private long Size
    {
        get => _size;
        set
        {
            this.RaiseAndSetIfChanged(ref _size, value);
            this.RaisePropertyChanged(nameof(FormattedSize));
            this.RaisePropertyChanged(nameof(FormattedSizeAsNumber));
        }
    }

    public string Name => _pathService.GetFileName(_fullPath);

    public string Path => _pathService.GetParentDirectory(_fullPath);

    public bool IsDirectory { get; set; }

    public IBitmap ImageBitmap => CheckIfImage() ? _bitmapFactory.Create(_fullPath) : null;

    public string FormattedSize => _fileSizeFormatter.GetFormattedSize(Size);

    public string FormattedSizeAsNumber => _fileSizeFormatter.GetSizeAsNumber(Size);
        
    public FileContentType FileType => _fileTypeMapper.GetFileType(Extension);

    public DateTime CreatedDateTime { get; set; }

    public DateTime LastWriteDateTime { get; set; }

    public DateTime LastAccessDateTime { get; set; }

    public int InnerFilesCount { get; set; }

    public int InnerDirectoriesCount { get; set; }

    private string Extension => _pathService.GetExtension(_fullPath);

    public MainNodeInfoTabViewModel(
        IFileSizeFormatter fileSizeFormatter,
        IPathService pathService,
        IBitmapFactory bitmapFactory,
        IFileTypeMapper fileTypeMapper,
        ImagePreviewConfiguration configuration)
    {
        _fileSizeFormatter = fileSizeFormatter;
        _pathService = pathService;
        _bitmapFactory = bitmapFactory;
        _fileTypeMapper = fileTypeMapper;
        _configuration = configuration;
    }

    public void Activate(NodeModelBase nodeModel, bool isDirectory, int innerFilesCount,
        int innerDirectoriesCount)
    {
        _fullPath = nodeModel.FullPath;
        CreatedDateTime = nodeModel.CreatedDateTime;
        LastWriteDateTime = nodeModel.LastModifiedDateTime;
        LastAccessDateTime = nodeModel.LastAccessDateTime;
        IsDirectory = isDirectory;
        InnerFilesCount = innerFilesCount;
        InnerDirectoriesCount = innerDirectoriesCount;
    }

    public void SetSize(long sizeBytes) => Size = sizeBytes;

    private bool CheckIfImage() => !IsDirectory && CheckIfImageFormatIsSupported();

    private bool CheckIfImageFormatIsSupported() => _configuration.SupportedFormats.Contains(Extension);
}