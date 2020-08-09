using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Implementations.Dialogs.Properties;
using Camelot.ViewModels.Interfaces.Properties;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.Dialogs
{
    public class FileInformationDialogViewModelTests
    {
        private const string File = "File";
        private const long Size = 42;

        private readonly AutoMocker _autoMocker;

        public FileInformationDialogViewModelTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Fact]
        public void TestMainNodeInfoTabViewModel()
        {
            var mainNodeInfoTabViewModelMock = new Mock<IMainNodeInfoTabViewModel>();
            _autoMocker.Use(mainNodeInfoTabViewModelMock.Object);
            var viewModel = _autoMocker.CreateInstance<FileInformationDialogViewModel>();

            Assert.Equal(mainNodeInfoTabViewModelMock.Object, viewModel.MainNodeInfoTabViewModel);
        }

        [Fact]
        public void TestActivation()
        {
            var fileModel = new FileModel {FullPath = File, SizeBytes = Size};
            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock
                .Setup(m => m.GetFile(File))
                .Returns(fileModel);
            var mainNodeInfoTabViewModelMock = new Mock<IMainNodeInfoTabViewModel>();
            mainNodeInfoTabViewModelMock
                .Setup(m => m.SetSize(Size))
                .Verifiable();
            mainNodeInfoTabViewModelMock
                .Setup(m => m.Activate(fileModel, false))
                .Verifiable();

            var viewModel = new FileInformationDialogViewModel(fileServiceMock.Object,
                mainNodeInfoTabViewModelMock.Object);
            var parameter = new FileSystemNodeNavigationParameter(File);
            viewModel.Activate(parameter);

            mainNodeInfoTabViewModelMock.Verify(m => m.SetSize(Size), Times.Once);
            mainNodeInfoTabViewModelMock.Verify(m => m.Activate(fileModel, false), Times.Once);
        }
    }
}