using System.Threading.Tasks;
using ApplicationDispatcher.Interfaces;
using Camelot.Services.Abstractions.Operations;
using Camelot.Services.Environment.Interfaces;
using Moq;
using Xunit;

namespace Camelot.Services.Tests
{
    public class ClipboardOperationsServiceTests
    {
        [Theory]
        [InlineData("file.txt", "file://file.txt")]
        [InlineData("file42.txt", "file://file42.txt")]
        public async Task TestCopy(string sourceFilePath, string clipboardString)
        {
            var clipboardServiceMock = new Mock<IClipboardService>();
            clipboardServiceMock
                .Setup(m => m.SetTextAsync(clipboardString))
                .Verifiable();
            var operationsServiceMock = new Mock<IOperationsService>();
            var environmentServiceMock = new Mock<IEnvironmentService>();
            environmentServiceMock
                .SetupGet(m => m.NewLine)
                .Returns(System.Environment.NewLine);

            var clipboardOperationsService = new ClipboardOperationsService(
                clipboardServiceMock.Object,
                operationsServiceMock.Object,
                environmentServiceMock.Object
            );

            await clipboardOperationsService.CopyFilesAsync(new[] {sourceFilePath});

            clipboardServiceMock.Verify(m => m.SetTextAsync(clipboardString), Times.Once());
        }

        [Theory]
        [InlineData("file.txt", "file://file.txt")]
        [InlineData("file42.txt", "file://file42.txt")]
        public async Task TestPaste(string sourceFilePath, string clipboardString)
        {
            const string directory = "Dir";

            var clipboardServiceMock = new Mock<IClipboardService>();
            clipboardServiceMock
                .Setup(m => m.GetTextAsync())
                .ReturnsAsync(clipboardString);
            var operationsServiceMock = new Mock<IOperationsService>();
            operationsServiceMock
                .Setup(m => m.CopyAsync(new[] {sourceFilePath}, directory))
                .Verifiable();
            var environmentServiceMock = new Mock<IEnvironmentService>();

            var clipboardOperationsService = new ClipboardOperationsService(
                clipboardServiceMock.Object,
                operationsServiceMock.Object,
                environmentServiceMock.Object
            );

            await clipboardOperationsService.PasteFilesAsync(directory);

            operationsServiceMock.Verify(m => m.CopyAsync(new[] {sourceFilePath}, directory), Times.Once());
        }
    }
}