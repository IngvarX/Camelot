using System.Collections.Generic;
using System.Linq;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.Operations;
using Camelot.Services.Abstractions.Operations;
using Camelot.ViewModels.Implementations.MainWindow.OperationsStates;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.OperationsStates
{
    public class OperationStateViewModelTests
    {
        private const string File = "File";
        private const string Directory = "Dir";

        private readonly AutoMocker _autoMocker;

        public OperationStateViewModelTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Theory]
        [InlineData(OperationType.Copy, 1, 13, false, OperationState.NotStarted)]
        [InlineData(OperationType.Move, 1, 0, true, OperationState.InProgress)]
        public void TestProperties(OperationType operationType, int filesCount,
            int dirsCount, bool isProcessingSingleFile, OperationState operationState)
        {
            var files = Enumerable
                .Repeat(File, filesCount)
                .ToArray();
            var dirs = Enumerable
                .Repeat(Directory, dirsCount)
                .ToArray();
            var settings = new BinaryFileSystemOperationSettings(dirs, files,
                dirs, files, new Dictionary<string, string>(0), new string[0]);
            var operationInfo = new OperationInfo(operationType, settings);

            _autoMocker
                .Setup<IOperation, OperationInfo>(m => m.Info)
                .Returns(operationInfo);
            _autoMocker
                .Setup<IOperation, OperationState>(m => m.State)
                .Returns(operationState);

            var viewModel = _autoMocker.CreateInstance<OperationStateViewModel>();

            Assert.Equal(0, viewModel.Progress);
            Assert.Equal(operationState, viewModel.State);
            Assert.Equal(filesCount, viewModel.SourceFilesCount);
            Assert.Equal(dirsCount, viewModel.SourceDirectoriesCount);
            Assert.Equal(isProcessingSingleFile, viewModel.IsProcessingSingleFile);
            Assert.Equal(operationType, viewModel.OperationType);
        }

        [Fact]
        public void TestCancelCommand()
        {
            var viewModel = _autoMocker.CreateInstance<OperationStateViewModel>();

            Assert.True(viewModel.CancelCommand.CanExecute(null));

            viewModel.CancelCommand.Execute(null);

            _autoMocker
                .Verify<IOperation>(m => m.CancelAsync(), Times.Once);
        }
    }
}