using Camelot.Services.Environment.Interfaces;
using Moq;
using Xunit;

namespace Camelot.Services.Windows.Tests
{
    public class WindowsResourceOpeningServiceTests
    {
        [Fact]
        public void TestFileServiceOpeningWindows()
        {
            const string fileName = "File.txt";
            const string command = "explorer";
            var arguments = $"\"{fileName}\"";
            var processServiceMock = new Mock<IProcessService>();
            processServiceMock
                .Setup(m => m.Run(command, arguments))
                .Verifiable();

            var fileOpeningService = new WindowsResourceOpeningService(processServiceMock.Object);

            fileOpeningService.Open(fileName);

            processServiceMock.Verify(m => m.Run(command, arguments), Times.Once());
        }
    }
}