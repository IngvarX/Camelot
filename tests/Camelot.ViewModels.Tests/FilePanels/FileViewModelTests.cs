using Camelot.Services.Abstractions;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
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

            var viewModel = _autoMocker.CreateInstance<FileViewModel>();
            viewModel.Size = size;

            var actualFormattedSize = viewModel.FormattedSize;
            Assert.Equal(formattedSize, actualFormattedSize);
        }
    }
}