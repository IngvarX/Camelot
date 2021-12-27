using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Nodes;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.FilePanels
{
    public class FileViewModelTests
    {
        private readonly AutoMocker _autoMocker;

        public FileViewModelTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Theory]
        [InlineData(42, "42")]
        [InlineData(42_000, "42 KB")]
        [InlineData(42_000_000, "42 MB")]
        public void TestFormattedSize(long size, string formattedSize)
        {
            _autoMocker
                .Setup<IFileSizeFormatter, string>(m => m.GetFormattedSize(size))
                .Returns(formattedSize);
            _autoMocker.Use(false);

            var viewModel = _autoMocker.CreateInstance<FileViewModel>();
            viewModel.Size = size;

            var actualFormattedSize = viewModel.FormattedSize;
            Assert.Equal(formattedSize, actualFormattedSize);
        }
        
        [Theory]
        [InlineData("mp3", FileMimeType.Music)]
        [InlineData("txt", FileMimeType.Other)]
        public void TestFileType(string extension, FileMimeType fileType)
        {
            _autoMocker
                .Setup<IFileTypeMapper, FileMimeType>(m => m.GetFileType(extension))
                .Returns(fileType);
            _autoMocker.Use(false);

            var viewModel = _autoMocker.CreateInstance<FileViewModel>();
            viewModel.Extension = extension;

            var actualType = viewModel.Type;
            Assert.Equal(fileType, actualType);
        }
    }
}