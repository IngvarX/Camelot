using Camelot.Services.Environment.Interfaces;
using Moq;
using Xunit;

namespace Camelot.Services.Windows.Tests
{
    public class WindowsResourceOpeningServiceTests
    {
        private const string FileName = "File.txt";

        [Fact]
        public void TestFileServiceOpeningWindows()
        {
            var processServiceMock = new Mock<IProcessService>();
            processServiceMock
                .Setup(m => m.Run(FileName))
                .Verifiable();

            var fileOpeningService = new WindowsResourceOpeningService(processServiceMock.Object);

            fileOpeningService.Open(FileName);

            processServiceMock.Verify(m => m.Run(FileName), Times.Once());
        }
    }
}