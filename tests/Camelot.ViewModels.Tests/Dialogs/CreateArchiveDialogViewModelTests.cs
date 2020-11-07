using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.State;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Implementations.Dialogs.Archives;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Services.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.Dialogs
{
    public class CreateArchiveDialogViewModelTests
    {
        private const string ArchivePath = "Archive";
        private const string NewArchivePath = "NewArchive";
        private const string ArchiveTypeName = "Name";
        private const string ArchiveFullPath = "Archive.Name";
        private const string ArchiveZip = "zip";
        private const string ArchiveTar = "tar";

        private readonly AutoMocker _autoMocker;

        public CreateArchiveDialogViewModelTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void TestArchiveWithWhiteSpaceCreation(string archivePath)
        {
            SetupForType();

            var dialog = _autoMocker.CreateInstance<CreateArchiveDialogViewModel>();
            dialog.Activate(new CreateArchiveNavigationParameter(archivePath, true));

            Assert.True(dialog.CreateCommand.CanExecute(null));

            dialog.ArchivePath = archivePath;

            Assert.False(dialog.CreateCommand.CanExecute(null));
        }

        [Fact]
        public void TestSelectPathCommand()
        {
            SetupForType();

            _autoMocker
                .Setup<ISystemDialogService, Task<string>>(m => m.GetFileAsync(ArchiveFullPath))
                .ReturnsAsync(NewArchivePath);

            var dialog = _autoMocker.CreateInstance<CreateArchiveDialogViewModel>();
            dialog.Activate(new CreateArchiveNavigationParameter(ArchivePath, true));

            Assert.Equal(ArchiveFullPath, dialog.ArchivePath);

            Assert.True(dialog.SelectPathCommand.CanExecute(null));
            dialog.SelectPathCommand.Execute(null);

            Assert.Equal(NewArchivePath, dialog.ArchivePath);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void TestSelectPathCommandInvalidPath(string archivePath)
        {
            SetupForType();

            _autoMocker
                .Setup<ISystemDialogService, Task<string>>(m => m.GetFileAsync(ArchiveFullPath))
                .ReturnsAsync(archivePath);

            var dialog = _autoMocker.CreateInstance<CreateArchiveDialogViewModel>();
            dialog.Activate(new CreateArchiveNavigationParameter(ArchivePath, true));

            Assert.Equal(ArchiveFullPath, dialog.ArchivePath);

            Assert.True(dialog.SelectPathCommand.CanExecute(null));
            dialog.SelectPathCommand.Execute(null);

            Assert.Equal(ArchiveFullPath, dialog.ArchivePath);
        }

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public void TestArchiveWithExistingNodeNameCreation(bool fileExists, bool dirExists)
        {
            SetupForType();

            _autoMocker
                .Setup<IDirectoryService, bool>(m => m.CheckIfExists(NewArchivePath))
                .Returns(dirExists);
            _autoMocker
                .Setup<IFileService, bool>(m => m.CheckIfExists(NewArchivePath))
                .Returns(fileExists);

            var dialog = _autoMocker.CreateInstance<CreateArchiveDialogViewModel>();
            dialog.Activate(new CreateArchiveNavigationParameter(ArchivePath, true));

            dialog.ArchivePath = NewArchivePath;

            Assert.False(dialog.CreateCommand.CanExecute(null));
        }

        [Fact]
        public void TestProperties()
        {
            SetupForType();

            var dialog = _autoMocker.CreateInstance<CreateArchiveDialogViewModel>();
            dialog.Activate(new CreateArchiveNavigationParameter(ArchivePath, true));

            Assert.Equal($"{ArchivePath}.{ArchiveTypeName}", dialog.ArchivePath);
            Assert.Equal(ArchiveType.Zip, dialog.SelectedArchiveType.ArchiveType);
            Assert.Equal(ArchiveTypeName, dialog.SelectedArchiveType.Name);
            Assert.Single(dialog.AvailableArchiveTypes);
        }

        [Theory]
        [InlineData(ArchiveType.Zip)]
        [InlineData(ArchiveType.TarGz)]
        [InlineData(ArchiveType.Tar)]
        public void TestArchiveCreation(ArchiveType archiveType)
        {
            SetupForType(archiveType);

            _autoMocker
                .Setup<ICreateArchiveStateService>(m => m.SaveState(
                    It.Is<CreateArchiveStateModel>(sm => sm.ArchiveType == archiveType)))
                .Verifiable();

            var dialog = _autoMocker.CreateInstance<CreateArchiveDialogViewModel>();
            dialog.Activate(new CreateArchiveNavigationParameter(ArchivePath, true));

            var isCallbackCalled = false;
            dialog.CloseRequested += (sender, args) =>
            {
                if (args.Result.ArchivePath == NewArchivePath && args.Result.ArchiveType == archiveType)
                {
                    isCallbackCalled = true;
                }
            };

            dialog.ArchivePath = NewArchivePath;

            Assert.Equal(NewArchivePath, dialog.ArchivePath);
            Assert.True(dialog.CreateCommand.CanExecute(null));

            dialog.CreateCommand.Execute(null);

            Assert.True(isCallbackCalled);

            _autoMocker
                .Verify<ICreateArchiveStateService>(m => m.SaveState(
                    It.Is<CreateArchiveStateModel>(sm => sm.ArchiveType == archiveType)), Times.Once);
        }

        [Fact]
        public void TestArchiveSingleFile()
        {
            SetupForType();

            var dialog = _autoMocker.CreateInstance<CreateArchiveDialogViewModel>();
            dialog.Activate(new CreateArchiveNavigationParameter(ArchivePath, true));

            _autoMocker
                .Verify<IArchiveTypeViewModelFactory>(m => m.CreateForSingleFile(), Times.Once);
        }

        [Fact]
        public void TestArchiveMultipleFiles()
        {
            SetupForType();

            var dialog = _autoMocker.CreateInstance<CreateArchiveDialogViewModel>();
            dialog.Activate(new CreateArchiveNavigationParameter(ArchivePath, false));

            _autoMocker
                .Verify<IArchiveTypeViewModelFactory>(m => m.CreateForMultipleFiles(), Times.Once);
        }

        [Fact]
        public void TestArchiveChangeType()
        {
            var viewModels = new[]
            {
                new ArchiveTypeViewModel(ArchiveType.Zip, ArchiveZip),
                new ArchiveTypeViewModel(ArchiveType.Tar, ArchiveTar)
            };
            _autoMocker
                .Setup<IArchiveTypeViewModelFactory, IReadOnlyList<ArchiveTypeViewModel>>(m => m.CreateForSingleFile())
                .Returns(viewModels);
            _autoMocker
                .Setup<ICreateArchiveStateService, CreateArchiveStateModel>(m => m.GetState())
                .Returns(new CreateArchiveStateModel
                {
                    ArchiveType = ArchiveType.Zip
                });

            var dialog = _autoMocker.CreateInstance<CreateArchiveDialogViewModel>();
            dialog.Activate(new CreateArchiveNavigationParameter(ArchivePath, true));

            Assert.EndsWith(ArchiveZip, dialog.ArchivePath);

            dialog.SelectedArchiveType = dialog.AvailableArchiveTypes.Last();
            Assert.EndsWith(ArchiveTar, dialog.ArchivePath);

            dialog.SelectedArchiveType = dialog.AvailableArchiveTypes.First();
            Assert.EndsWith(ArchiveZip, dialog.ArchivePath);
        }

        [Fact]
        public void TestCancel()
        {
            SetupForType();

            var dialog = _autoMocker.CreateInstance<CreateArchiveDialogViewModel>();
            dialog.Activate(new CreateArchiveNavigationParameter(ArchivePath, true));

            var isCallbackCalled = false;
            dialog.CloseRequested += (sender, args) =>
            {
                if (args.Result is null)
                {
                    isCallbackCalled = true;
                }
            };

            Assert.True(dialog.CancelCommand.CanExecute(null));

            dialog.CancelCommand.Execute(null);

            Assert.True(isCallbackCalled);
        }

        private void SetupForType(ArchiveType archiveType = ArchiveType.Zip)
        {
            var viewModels = new[]
            {
                new ArchiveTypeViewModel(archiveType, ArchiveTypeName)
            };
            _autoMocker
                .Setup<IArchiveTypeViewModelFactory, IReadOnlyList<ArchiveTypeViewModel>>(m => m.CreateForSingleFile())
                .Returns(viewModels);
            _autoMocker
                .Setup<IArchiveTypeViewModelFactory, IReadOnlyList<ArchiveTypeViewModel>>(m => m.CreateForMultipleFiles())
                .Returns(viewModels);
            _autoMocker
                .Setup<ICreateArchiveStateService, CreateArchiveStateModel>(m => m.GetState())
                .Returns(new CreateArchiveStateModel
                {
                    ArchiveType = archiveType
                });
        }
    }
}