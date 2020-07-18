using System;
using System.Collections.Generic;
using Avalonia.Media.Imaging;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Configuration;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.Dialogs.Properties;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests
{
    public class MainNodeInfoTabViewModelTests
    {
        private const long Size = 42_000;
        private const string FormattedSize = "42 KB";
        private const string SizeAsNumber = "42 000";
        private const string ParentDirectory = "Dir";
        private const string FileName = "Name";

        [Theory]
        [InlineData("File", "", false, false)]
        [InlineData("File.png", "png", false, true)]
        [InlineData("File.jpg", "jpg", false, true)]
        [InlineData("File.gif", "gif", false, false)]
        [InlineData("Dir", null, true, false)]
        public void TestProperties(string filePath, string extension, bool isDirectory, bool isImage)
        {
            var configuration = new ImagePreviewConfiguration
            {
                SupportedFormats = new List<string>
                {
                    "png", "jpg"
                }
            };
            var fileSizeFormatterMock = new Mock<IFileSizeFormatter>();
            fileSizeFormatterMock
                .Setup(m => m.GetFormattedSize(Size))
                .Returns(FormattedSize);
            fileSizeFormatterMock
                .Setup(m => m.GetSizeAsNumber(Size))
                .Returns(SizeAsNumber);
            var pathServiceMock = new Mock<IPathService>();
            pathServiceMock
                .Setup(m => m.GetParentDirectory(filePath))
                .Returns(ParentDirectory);
            pathServiceMock
                .Setup(m => m.GetFileName(filePath))
                .Returns(FileName);
            pathServiceMock
                .Setup(m => m.GetExtension(filePath))
                .Returns(extension);
            var bitmapFactoryMock = new Mock<IBitmapFactory>();
            bitmapFactoryMock
                .Setup(m => m.Create(filePath))
                .Returns(new Mock<IBitmap>().Object);

            var viewModel = new MainNodeInfoTabViewModel(fileSizeFormatterMock.Object,
                pathServiceMock.Object, bitmapFactoryMock.Object, configuration);
            var nodeModel = new NodeModelBase
            {
                FullPath = filePath,
                CreatedDateTime = DateTime.Now,
                LastAccessDateTime = DateTime.Now.AddHours(1),
                LastModifiedDateTime = DateTime.Now.AddHours(2),
            };
            viewModel.Activate(nodeModel, isDirectory);
            viewModel.SetSize(Size);

            Assert.Equal(nodeModel.CreatedDateTime, viewModel.CreatedDateTime);
            Assert.Equal(nodeModel.LastAccessDateTime, viewModel.LastAccessDateTime);
            Assert.Equal(nodeModel.LastModifiedDateTime, viewModel.LastWriteDateTime);
            Assert.Equal(FormattedSize, viewModel.FormattedSize);
            Assert.Equal(SizeAsNumber, viewModel.FormattedSizeAsNumber);
            Assert.Equal(FileName, viewModel.Name);
            Assert.Equal(ParentDirectory, viewModel.Path);
            Assert.Equal(isDirectory, viewModel.IsDirectory);

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
}