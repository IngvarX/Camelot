using System;
using System.Collections.Generic;
using Avalonia.Media.Imaging;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Configuration;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.Dialogs.Properties;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests;

public class MainNodeInfoTabViewModelTests
{
    private const long Size = 42_000;
    private const string FormattedSize = "42 KB";
    private const string SizeAsNumber = "42 000";
    private const string ParentDirectory = "Dir";
    private const string FileName = "Name";
    private const int InnerFilesCount = 5;
    private const int InnerDirectoriesCount = 13;

    private readonly AutoMocker _autoMocker;

    public MainNodeInfoTabViewModelTests()
    {
        _autoMocker = new AutoMocker();
        var configuration = new ImagePreviewConfiguration
        {
            SupportedFormats = new List<string>
            {
                "png", "jpg"
            }
        };
        _autoMocker.Use(configuration);
    }

    [Theory]
    [InlineData("File", "", false, false)]
    [InlineData("File.png", "png", false, true)]
    [InlineData("File.jpg", "jpg", false, true)]
    [InlineData("File.gif", "gif", false, false)]
    [InlineData("Dir", null, true, false)]
    public void TestProperties(string filePath, string extension, bool isDirectory, bool isImage)
    {
        _autoMocker
            .Setup<IFileSizeFormatter, string>(m => m.GetFormattedSize(Size))
            .Returns(FormattedSize);
        _autoMocker
            .Setup<IFileSizeFormatter, string>(m => m.GetSizeAsNumber(Size))
            .Returns(SizeAsNumber);
        _autoMocker
            .Setup<IPathService, string>(m => m.GetParentDirectory(filePath))
            .Returns(ParentDirectory);
        _autoMocker
            .Setup<IPathService, string>(m => m.GetFileName(filePath))
            .Returns(FileName);
        _autoMocker
            .Setup<IPathService, string>(m => m.GetExtension(filePath))
            .Returns(extension);
        _autoMocker
            .Setup<IBitmapFactory, IBitmap>(m => m.Create(filePath))
            .Returns(new Mock<IBitmap>().Object);

        var viewModel = _autoMocker.CreateInstance<MainNodeInfoTabViewModel>();
        var nodeModel = new NodeModelBase
        {
            FullPath = filePath,
            CreatedDateTime = DateTime.Now,
            LastAccessDateTime = DateTime.Now.AddHours(1),
            LastModifiedDateTime = DateTime.Now.AddHours(2),
        };
        viewModel.Activate(nodeModel, isDirectory, InnerFilesCount, InnerDirectoriesCount);
        viewModel.SetSize(Size);

        Assert.Equal(nodeModel.CreatedDateTime, viewModel.CreatedDateTime);
        Assert.Equal(nodeModel.LastAccessDateTime, viewModel.LastAccessDateTime);
        Assert.Equal(nodeModel.LastModifiedDateTime, viewModel.LastWriteDateTime);
        Assert.Equal(FormattedSize, viewModel.FormattedSize);
        Assert.Equal(SizeAsNumber, viewModel.FormattedSizeAsNumber);
        Assert.Equal(FileName, viewModel.Name);
        Assert.Equal(ParentDirectory, viewModel.Path);
        Assert.Equal(isDirectory, viewModel.IsDirectory);
        Assert.Equal(InnerFilesCount, viewModel.InnerFilesCount);
        Assert.Equal(InnerDirectoriesCount, viewModel.InnerDirectoriesCount);

        if (isImage)
        {
            Assert.NotNull(viewModel.ImageBitmap);
        }
        else
        {
            Assert.Null(viewModel.ImageBitmap);
        }
    }
}