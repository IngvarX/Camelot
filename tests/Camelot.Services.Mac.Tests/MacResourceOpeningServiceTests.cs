using Camelot.Services.Environment.Interfaces;
using Moq;
using Xunit;

namespace Camelot.Services.Mac.Tests
{
    public class MacResourceOpeningServiceTests
    {
        private const string FileName = "File.txt";

        [Fact]
        public void TestFileServiceOpeningMacOs()
        {
            const string command = "open";
            var arguments = $"\"{FileName}\"";

            var processServiceMock = new Mock<IProcessService>();
            processServiceMock
                .Setup(m => m.Run(command, arguments))
                .Verifiable();

            var fileOpeningService = new MacResourceOpeningService(processServiceMock.Object);

            fileOpeningService.Open(FileName);

            processServiceMock.Verify(m => m.Run(command, arguments), Times.Once());
        }
    }
}