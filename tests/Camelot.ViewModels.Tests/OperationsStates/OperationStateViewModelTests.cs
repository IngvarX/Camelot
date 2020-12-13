using System.Collections.Generic;
using System.Linq;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.EventArgs;
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
        private const string SourceDirectory = "SourceDir";
        private const string TargetDirectory = "TargetDir";

        private readonly AutoMocker _autoMocker;

        public OperationStateViewModelTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Theory]
        [InlineData(OperationType.Copy, 1, 13, false, OperationState.NotStarted, null)]
        [InlineData(OperationType.Move, 1, 0, true, OperationState.InProgress, File)]
        [InlineData(OperationType.Move, 0, 1, true, OperationState.InProgress, Directory)]
        public void TestProperties(OperationType operationType, int filesCount,
            int dirsCount, bool isProcessingSingleFile, OperationState operationState,
            string sourceFile)
        {
            var files = Enumerable
                .Repeat(File, filesCount)
                .ToArray();
            var dirs = Enumerable
                .Repeat(Directory, dirsCount)
                .ToArray();
            var settings = new BinaryFileSystemOperationSettings(dirs, files,
                dirs, files, new Dictionary<string, string>(0),
                new string[0], SourceDirectory, TargetDirectory);
            var operationInfo = new OperationInfo(operationType, settings);

            _autoMocker
                .Setup<IOperation, OperationInfo>(m => m.Info)
                .Returns(operationInfo);
            _autoMocker
                .Setup<IOperation, OperationState>(m => m.State)
                .Returns(operationState);
            _autoMocker
                .Setup<IPathService, string>(m => m.GetFileName(It.IsAny<string>()))
                .Returns<string>(s => s);

            var viewModel = _autoMocker.CreateInstance<OperationStateViewModel>();

            Assert.Equal(0, viewModel.Progress);
            Assert.Equal(operationState, viewModel.State);
            Assert.Equal(filesCount, viewModel.SourceFilesCount);
            Assert.Equal(dirsCount, viewModel.SourceDirectoriesCount);
            Assert.Equal(isProcessingSingleFile, viewModel.IsProcessingSingleFile);
            Assert.Equal(operationType, viewModel.OperationType);
            Assert.Equal(sourceFile, viewModel.SourceFile);
            Assert.Equal(SourceDirectory, viewModel.SourceDirectory);
            Assert.Equal(TargetDirectory, viewModel.TargetDirectory);
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

        [Fact]
        public void TestProgress()
        {
            var viewModel = _autoMocker.CreateInstance<OperationStateViewModel>();

            Assert.Equal(0, viewModel.Progress);

            _autoMocker
                .GetMock<IOperation>()
                .Raise(o => o.ProgressChanged += null, new OperationProgressChangedEventArgs(0.1));

            Assert.Equal(10, viewModel.Progress);

            _autoMocker
                .GetMock<IOperation>()
                .Raise(o => o.ProgressChanged += null, new OperationProgressChangedEventArgs(0.5));

            Assert.Equal(50, viewModel.Progress);
        }

        [Theory]
        [InlineData(OperationState.Failed)]
        [InlineData(OperationState.Finished)]
        [InlineData(OperationState.Cancelled)]
        public void TestUnsubscribe(OperationState operationState)
        {
            var viewModel = _autoMocker.CreateInstance<OperationStateViewModel>();

            Assert.Equal(0, viewModel.Progress);

            _autoMocker
                .GetMock<IOperation>()
                .Raise(o => o.StateChanged += null, new OperationStateChangedEventArgs(operationState));

            Assert.Equal(0, viewModel.Progress);

            _autoMocker
                .GetMock<IOperation>()
                .Raise(o => o.ProgressChanged += null, new OperationProgressChangedEventArgs(0.5));

            Assert.Equal(0, viewModel.Progress);
        }
    }
}