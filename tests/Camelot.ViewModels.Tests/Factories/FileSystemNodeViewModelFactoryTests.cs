using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Factories.Implementations;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.Factories;

public class FileSystemNodeViewModelFactoryTests
{
    private const string Name = "Name";
    private const string FullPath = "fullPath";
    private const string Extension = "txt";

    private readonly AutoMocker _autoMocker;

    public FileSystemNodeViewModelFactoryTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Fact]
    public void TestCreateFile()
    {
        var fileModel = new FileModel
        {
            Name = Name,
            FullPath = FullPath
        };

        _autoMocker
            .Setup<IPathService, string>(m => m.GetFileName(fileModel.Name))
            .Returns(Name);
        _autoMocker
            .Setup<IPathService, string>(m => m.GetFileNameWithoutExtension(fileModel.Name))
            .Returns(Name);
        _autoMocker
            .Setup<IPathService, string>(m => m.GetExtension(fileModel.Name))
            .Returns(Extension);
        _autoMocker
            .Setup<IIconsSettingsService, IconsSettingsModel>(m => m.GetIconsSettings())
            .Returns(new IconsSettingsModel(IconsType.Builtin));

        var factory = _autoMocker.CreateInstance<FileSystemNodeViewModelFactory>();

        var node = factory.Create(fileModel);

        Assert.NotNull(node);
        Assert.Equal(node.Name, Name);
        Assert.Equal(node.FullPath, FullPath);
        Assert.False(node.IsEditing);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void TestCreateFileByPath(bool shouldReturnNull)
    {
        var fileModel = new FileModel
        {
            Name = Name,
            FullPath = FullPath
        };

        _autoMocker
            .Setup<IPathService, string>(m => m.GetFileName(fileModel.Name))
            .Returns(Name);
        _autoMocker
            .Setup<IPathService, string>(m => m.GetFileNameWithoutExtension(fileModel.Name))
            .Returns(Name);
        _autoMocker
            .Setup<IPathService, string>(m => m.GetExtension(fileModel.Name))
            .Returns(Extension);
        _autoMocker
            .Setup<IFileService, FileModel>(m => m.GetFile(FullPath))
            .Returns(shouldReturnNull ? null : fileModel);
        _autoMocker
            .Setup<IFileService, bool>(m => m.CheckIfExists(FullPath))
            .Returns(true);
        _autoMocker
            .Setup<IIconsSettingsService, IconsSettingsModel>(m => m.GetIconsSettings())
            .Returns(new IconsSettingsModel(IconsType.Builtin));

        var factory = _autoMocker.CreateInstance<FileSystemNodeViewModelFactory>();

        var node = factory.Create(FullPath);

        if (shouldReturnNull)
        {
            Assert.Null(node);

            return;
        }

        Assert.NotNull(node);
        Assert.Equal(node.Name, Name);
        Assert.Equal(node.FullPath, FullPath);
        Assert.False(node.IsEditing);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void TestCreateDirectory(bool isParentDirectory)
    {
        var directoryModel = new DirectoryModel
        {
            Name = Name,
            FullPath = FullPath
        };
        
        _autoMocker
            .Setup<IIconsSettingsService, IconsSettingsModel>(m => m.GetIconsSettings())
            .Returns(new IconsSettingsModel(IconsType.Builtin));

        var factory = _autoMocker.CreateInstance<FileSystemNodeViewModelFactory>();

        var node = factory.Create(directoryModel, isParentDirectory);

        Assert.NotNull(node);
        Assert.Equal(node.Name, Name);
        Assert.Equal(node.FullPath, FullPath);
        Assert.False(node.IsEditing);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void TestCreateDirectoryByPath(bool shouldReturnNull)
    {
        var directoryModel = new DirectoryModel
        {
            Name = Name,
            FullPath = FullPath
        };

        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(FullPath))
            .Returns(true);
        _autoMocker
            .Setup<IDirectoryService, DirectoryModel>(m => m.GetDirectory(FullPath))
            .Returns(shouldReturnNull ? null : directoryModel);
        _autoMocker
            .Setup<IIconsSettingsService, IconsSettingsModel>(m => m.GetIconsSettings())
            .Returns(new IconsSettingsModel(IconsType.Builtin));

        var factory = _autoMocker.CreateInstance<FileSystemNodeViewModelFactory>();

        var node = factory.Create(FullPath);

        if (shouldReturnNull)
        {
            Assert.Null(node);

            return;
        }

        Assert.NotNull(node);
        Assert.Equal(node.Name, Name);
        Assert.Equal(node.FullPath, FullPath);
        Assert.False(node.IsEditing);
    }

    [Fact]
    public void TestCreateNotFound()
    {
        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(FullPath))
            .Returns(false);
        _autoMocker
            .Setup<IFileService, bool>(m => m.CheckIfExists(FullPath))
            .Returns(false);
        _autoMocker
            .Setup<IIconsSettingsService, IconsSettingsModel>(m => m.GetIconsSettings())
            .Returns(new IconsSettingsModel(IconsType.Builtin));

        var factory = _autoMocker.CreateInstance<FileSystemNodeViewModelFactory>();

        var node = factory.Create(FullPath);

        Assert.Null(node);
    }
}