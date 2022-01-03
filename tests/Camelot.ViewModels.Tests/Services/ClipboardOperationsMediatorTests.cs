using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.ViewModels.Services.Implementations;
using Camelot.ViewModels.Services.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.Services
{
    public class ClipboardOperationsMediatorTests
    {
        private const string Directory = "Dir";
        private const string File = "File";

        private readonly AutoMocker _autoMocker;

        public ClipboardOperationsMediatorTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Fact]
        public void TestCopyToClipboardCommand()
        {
            _autoMocker
                .Setup<INodesSelectionService, IReadOnlyList<string>>(m => m.SelectedNodes)
                .Returns(new[] {File});
            _autoMocker
                .Setup<IClipboardOperationsService>(m => m.CopyFilesAsync(
                    It.Is<IReadOnlyList<string>>(l => l.Single() == File)))
                .Verifiable();

           var mediator = _autoMocker.CreateInstance<ClipboardOperationsMediator>();

           Assert.True(mediator.CopyToClipboardCommand.CanExecute(null));

           mediator.CopyToClipboardCommand.Execute(null);

            _autoMocker
                .Verify<IClipboardOperationsService>(m => m.CopyFilesAsync(
                    It.Is<IReadOnlyList<string>>(l => l.Single() == File)),
                    Times.Once);
        }

        [Fact]
        public void TestPasteFromClipboardCommand()
        {
            _autoMocker
                .Setup<IDirectoryService, string>(m => m.SelectedDirectory)
                .Returns(Directory);

            var mediator = _autoMocker.CreateInstance<ClipboardOperationsMediator>();

            Assert.True(mediator.CopyToClipboardCommand.CanExecute(null));

            _autoMocker
                .GetMock<IFilePanelDirectoryObserver>()
                .Raise(m => m.CurrentDirectoryChanged += null, EventArgs.Empty);

            Assert.True(mediator.PasteFromClipboardCommand.CanExecute(null));

            mediator.PasteFromClipboardCommand.Execute(null);

            _autoMocker
                .Verify<IClipboardOperationsService>(m => m.PasteFilesAsync(Directory),
                    Times.Once);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, true)]
        public async Task TestCanPaste(bool canPaste, bool expected)
        {
            _autoMocker
                .Setup<IClipboardOperationsService, Task<bool>>(m => m.CanPasteAsync())
                .ReturnsAsync(canPaste);

            var mediator = _autoMocker.CreateInstance<ClipboardOperationsMediator>();

            var actual = await mediator.CanPasteAsync();
            Assert.Equal(expected, actual);

            _autoMocker
                .Verify<IClipboardOperationsService, Task<bool>>(m => m.CanPasteAsync(),
                    Times.Once);
        }
    }
}