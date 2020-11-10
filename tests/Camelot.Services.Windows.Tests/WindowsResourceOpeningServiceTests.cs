using Camelot.Services.Environment.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Windows.Tests
{
    public class WindowsResourceOpeningServiceTests
    {
        private const string Command = "explorer";

        private readonly AutoMocker _autoMocker;

        public WindowsResourceOpeningServiceTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Theory]
        [InlineData("File.txt", "File.txt")]
        [InlineData("Fi le.txt", "\"Fi le.txt\"")]
        public void TestFileServiceOpeningWindows(string fileName, string arguments)
        {
            _autoMocker
                .Setup<IProcessService>(m => m.Run(Command, arguments))
                .Verifiable();

            var fileOpeningService = _autoMocker.CreateInstance<WindowsResourceOpeningService>();

            fileOpeningService.Open(fileName);

            _autoMocker
                .Verify<IProcessService>(m => m.Run(Command, arguments), Times.Once);
        }
    }
}