using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests.Dialogs
{
    public class OverwriteOptionsDialogViewModelTests
    {
        private const string SourceFilePath = "Source";
        private const string DestinationFilePath = "Destination";
        private const string NewFileName = "New";
        private const string ParentDirectory = "Parent";
        private const string ParentDirectoryName = "ParentName";
        private const string NewFilePath = ParentDirectory + NewFileName;

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public async Task TestProperties(bool shouldApplyToAll, bool areMultipleFilesAvailable)
        {
            var dialog = await Create(areMultipleFilesAvailable);
            dialog.ShouldApplyToAll = shouldApplyToAll;
            dialog.NewFileName = NewFileName;

            Assert.Equal(shouldApplyToAll, dialog.ShouldApplyToAll);
            Assert.Equal(areMultipleFilesAvailable, dialog.AreMultipleFilesAvailable);
            Assert.Equal(ParentDirectoryName, dialog.DestinationDirectoryName);
            Assert.Equal(NewFileName, dialog.NewFileName);
        }

        [Fact]
        public async Task TestCanceled()
        {
            var callbackCalled = false;
            var dialog = await Create();
            dialog.CloseRequested += (sender, args) =>
            {
                var result = args.Result;
                if (result is null)
                {
                    callbackCalled = true;
                }
            };

            dialog.CancelCommand.Execute(null);

            Assert.True(callbackCalled);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TestSkip(bool shouldApplyToAll)
        {
            var callbackCalled = false;
            var dialog = await Create();
            dialog.ShouldApplyToAll = shouldApplyToAll;
            dialog.CloseRequested += (sender, args) =>
            {
                var result = args.Result;
                Assert.NotNull(result);
                if (result.Options.Mode is OperationContinuationMode.Skip)
                {
                    callbackCalled = true;
                }

                Assert.Equal(shouldApplyToAll, result.Options.ApplyToAll);
                Assert.Equal(SourceFilePath, result.Options.FilePath);
            };

            dialog.SkipCommand.Execute(null);

            Assert.True(callbackCalled);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TestOverwrite(bool shouldApplyToAll)
        {
            var callbackCalled = false;
            var dialog = await Create();
            dialog.ShouldApplyToAll = shouldApplyToAll;
            dialog.CloseRequested += (sender, args) =>
            {
                var result = args.Result;
                Assert.NotNull(result);
                if (result.Options.Mode is OperationContinuationMode.Overwrite)
                {
                    callbackCalled = true;
                }

                Assert.Equal(shouldApplyToAll, result.Options.ApplyToAll);
                Assert.Equal(SourceFilePath, result.Options.FilePath);
            };

            dialog.ReplaceCommand.Execute(null);

            Assert.True(callbackCalled);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TestOverwriteIfOlder(bool shouldApplyToAll)
        {
            var callbackCalled = false;
            var dialog = await Create();
            dialog.ShouldApplyToAll = shouldApplyToAll;
            dialog.CloseRequested += (sender, args) =>
            {
                var result = args.Result;
                Assert.NotNull(result);
                if (result.Options.Mode is OperationContinuationMode.OverwriteIfOlder)
                {
                    callbackCalled = true;
                }

                Assert.Equal(shouldApplyToAll, result.Options.ApplyToAll);
                Assert.Equal(SourceFilePath, result.Options.FilePath);
            };

            dialog.ReplaceIfOlderCommand.Execute(null);

            Assert.True(callbackCalled);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TestRename(bool shouldApplyToAll)
        {
            var callbackCalled = false;
            var dialog = await Create();
            dialog.ShouldApplyToAll = shouldApplyToAll;
            dialog.NewFileName = NewFileName;
            dialog.CloseRequested += (sender, args) =>
            {
                var result = args.Result;
                Assert.NotNull(result);
                if (result.Options.Mode is OperationContinuationMode.Rename)
                {
                    callbackCalled = true;
                }

                Assert.Equal(shouldApplyToAll, result.Options.ApplyToAll);
                Assert.Equal(SourceFilePath, result.Options.FilePath);
                Assert.Equal(NewFilePath, result.Options.NewFilePath);
            };

            dialog.RenameCommand.Execute(null);

            Assert.True(callbackCalled);
        }

        private static async Task<OverwriteOptionsDialogViewModel> Create(bool areMultipleFilesAvailable = true)
        {
            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock
                .Setup(m => m.GetFile(It.IsAny<string>()))
                .Returns<string>(s => new FileModel {FullPath = s, Name = s});
            var fileSystemNodeViewModelFactory = new Mock<IFileSystemNodeViewModelFactory>();
            fileSystemNodeViewModelFactory
                .Setup(m => m.Create(It.IsAny<FileModel>()))
                .Returns<FileModel>(fm =>
                {
                    var mock = new Mock<IFileSystemNodeViewModel>();
                    mock
                        .SetupGet(m => m.FullPath)
                        .Returns(fm.FullPath);

                    return mock.Object;
                });
            var fileNameGenerationService = new Mock<IFileNameGenerationService>();
            var pathService = new Mock<IPathService>();
            pathService
                .Setup(m => m.GetParentDirectory(DestinationFilePath))
                .Returns(ParentDirectory);
            pathService
                .Setup(m => m.Combine(ParentDirectory, NewFileName))
                .Returns(NewFilePath);
            pathService
                .Setup(m => m.GetFileName(ParentDirectory))
                .Returns(ParentDirectoryName);

            var dialog = new OverwriteOptionsDialogViewModel(
                fileServiceMock.Object, fileSystemNodeViewModelFactory.Object,
                fileNameGenerationService.Object, pathService.Object);

            var parameter = new OverwriteOptionsNavigationParameter(SourceFilePath, DestinationFilePath, areMultipleFilesAvailable);
            await dialog.ActivateAsync(parameter);

            return dialog;
        }
    }
}