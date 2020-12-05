using Camelot.Services.Environment.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Mac.Tests
{
    public class MacResourceOpeningServiceTests
    {
        private readonly AutoMocker _autoMocker;

        public MacResourceOpeningServiceTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Theory]
        [InlineData("File.txt", "open", "\"File.txt\"")]
        [InlineData("File.app", "open", "-a \"File.app\"")]
        public void TestFileOpeningMacOs(string fileName, string command, string arguments)
        {
            _autoMocker
                .Setup<IProcessService>(m => m.Run(command, arguments))
                .Verifiable();

            var fileOpeningService = _autoMocker.CreateInstance<MacResourceOpeningService>();

            fileOpeningService.Open(fileName);

            _autoMocker
                .Verify<IProcessService>(m => m.Run(command, arguments), Times.Once);
        }
        
        [Theory]
        [InlineData("File.txt", "Terminal", "open", "-a \"Terminal\" \"File.txt\"")]
        public void TestFileOpeningWithMacOs(string fileName, string app, string command, string arguments)
        {
            _autoMocker
                .Setup<IProcessService>(m => m.Run(command, arguments))
                .Verifiable();

            var fileOpeningService = _autoMocker.CreateInstance<MacResourceOpeningService>();

            fileOpeningService.OpenWith(app, "", fileName);

            _autoMocker
                .Verify<IProcessService>(m => m.Run(command, arguments), Times.Once);
        }
    }
}