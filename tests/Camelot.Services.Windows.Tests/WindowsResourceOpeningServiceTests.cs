using Camelot.Services.Environment.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Windows.Tests
{
    public class WindowsResourceOpeningServiceTests
    {
        private readonly AutoMocker _autoMocker;

        public WindowsResourceOpeningServiceTests()
        {
            _autoMocker = new AutoMocker();
        }
        
        [Fact]
        public void TestFileServiceOpeningWindows()
        {
            const string fileName = "File.txt";
            const string command = "explorer";
            var arguments = $"\"{fileName}\"";

            _autoMocker
                .Setup<IProcessService>(m => m.Run(command, arguments))
                .Verifiable();

            var fileOpeningService = _autoMocker.CreateInstance<WindowsResourceOpeningService>();

            fileOpeningService.Open(fileName);

            _autoMocker
                .Verify<IProcessService>(m => m.Run(command, arguments), Times.Once);
        }
    }
}