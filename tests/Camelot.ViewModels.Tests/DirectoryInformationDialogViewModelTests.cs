using System;
using Camelot.Avalonia.Interfaces;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Implementations.Dialogs.Properties;
using Camelot.ViewModels.Interfaces.Properties;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests
{
    public class DirectoryInformationDialogViewModelTests
    {
        private const string Directory = "Dir";
        private const long Size = 42;

        private readonly AutoMocker _autoMocker;

        public DirectoryInformationDialogViewModelTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Fact]
        public void TestMainNodeInfoTabViewModel()
        {
            var mainNodeInfoTabViewModelMock = new Mock<IMainNodeInfoTabViewModel>();
            _autoMocker.Use(mainNodeInfoTabViewModelMock.Object);
            var viewModel = _autoMocker.CreateInstance<DirectoryInformationDialogViewModel>();

            Assert.Equal(mainNodeInfoTabViewModelMock.Object, viewModel.MainNodeInfoTabViewModel);
        }

        [Fact]
        public void TestActivation()
        {
            var directoryModel = new DirectoryModel {FullPath = Directory};
            var directoryServiceMock = new Mock<IDirectoryService>();
            directoryServiceMock
                .Setup(m => m.GetDirectory(Directory))
                .Returns(directoryModel);
            directoryServiceMock
                .Setup(m => m.CalculateSize(Directory))
                .Returns(Size);
            var applicationDispatcherMock = new Mock<IApplicationDispatcher>();
            applicationDispatcherMock
                .Setup(m => m.Dispatch(It.IsAny<Action>()))
                .Callback<Action>(action => action());
            var mainNodeInfoTabViewModelMock = new Mock<IMainNodeInfoTabViewModel>();
            mainNodeInfoTabViewModelMock
                .Setup(m => m.SetSize(Size))
                .Verifiable();
            mainNodeInfoTabViewModelMock
                .Setup(m => m.Activate(directoryModel, true))
                .Verifiable();

            var viewModel = new DirectoryInformationDialogViewModel(directoryServiceMock.Object,
                applicationDispatcherMock.Object, mainNodeInfoTabViewModelMock.Object);
            var parameter = new FileSystemNodeNavigationParameter(Directory);
            viewModel.Activate(parameter);

            mainNodeInfoTabViewModelMock.Verify(m => m.SetSize(Size), Times.Once);
            mainNodeInfoTabViewModelMock.Verify(m => m.Activate(directoryModel, true), Times.Once);
        }
    }
}