using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Implementations.Dialogs.Properties;
using Camelot.ViewModels.Interfaces.Properties;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.Dialogs;

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
        _autoMocker
            .Setup<IFileService, FileModel>(m => m.GetFile(File))
            .Returns(fileModel);
        _autoMocker
            .Setup<IMainNodeInfoTabViewModel>(m => m.SetSize(Size))
            .Verifiable();
        _autoMocker
            .Setup<IMainNodeInfoTabViewModel>(m => m.Activate(fileModel, false, 0, 0))
            .Verifiable();

        var viewModel = _autoMocker.CreateInstance<FileInformationDialogViewModel>();
        var parameter = new FileSystemNodeNavigationParameter(File);
        viewModel.Activate(parameter);

        _autoMocker
            .Verify<IMainNodeInfoTabViewModel>(m => m.SetSize(Size),
                Times.Once);
        _autoMocker
            .Verify<IMainNodeInfoTabViewModel>(m => m.Activate(fileModel, false, 0, 0),
                Times.Once);
    }
}