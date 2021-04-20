using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Avalonia.Interfaces;
using Camelot.Services.Abstractions.Operations;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.AllPlatforms.Tests
{
    public class FilesClipboardOperationsServiceTests
    {
        private const string Directory = "Dir";
        private const string File = "File";

        private readonly AutoMocker _autoMocker;

        public FilesClipboardOperationsServiceTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Fact]
        public async Task TestCopy()
        {
            _autoMocker
                .Setup<IClipboardService>(m => m.SetFilesAsync(It.Is<IReadOnlyList<string>>(
                    l => l.Count == 2 && l.Contains(Directory) && l.Contains(File))))
                .Verifiable();

            var clipboardOperationsService = _autoMocker.CreateInstance<FilesClipboardOperationsService>();

            await clipboardOperationsService.CopyFilesAsync(new[] {Directory, File});

            _autoMocker
                .Verify<IClipboardService>(m => m.SetFilesAsync(It.Is<IReadOnlyList<string>>(
                    l => l.Count == 2 && l.Contains(Directory) && l.Contains(File))),
                    Times.Once);
        }

        [Fact]
        public async Task TestPaste()
        {
            _autoMocker
                .Setup<IClipboardService, Task<IReadOnlyList<string>>>(m => m.GetFilesAsync())
                .ReturnsAsync(new[] {File});
            _autoMocker
                .Setup<IOperationsService>(m => m.CopyAsync(new[] {File}, Directory))
                .Verifiable();

            var clipboardOperationsService = _autoMocker.CreateInstance<FilesClipboardOperationsService>();

            await clipboardOperationsService.PasteFilesAsync(Directory);

            _autoMocker
                .Verify<IOperationsService>(m => m.CopyAsync(new[] {File}, Directory),
                    Times.Once);
        }

        [Theory]
        [InlineData(null, 0)]
        [InlineData(new string[0], 0)]
        public async Task TestPasteInvalidData(string[] files, int expectedCallsCount)
        {
            _autoMocker
                .Setup<IClipboardService, Task<IReadOnlyList<string>>>(m => m.GetFilesAsync())
                .ReturnsAsync(files);

            var clipboardOperationsService = _autoMocker.CreateInstance<FilesClipboardOperationsService>();

            await clipboardOperationsService.PasteFilesAsync(Directory);

            _autoMocker
                .Verify<IOperationsService>(m => m.CopyAsync(It.IsAny<IReadOnlyList<string>>(), Directory),
                    Times.Exactly(expectedCallsCount));
        }
    }
}