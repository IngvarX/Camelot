using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Avalonia.Interfaces;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Implementations.Dialogs.Properties;
using Camelot.ViewModels.Interfaces.Properties;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.Dialogs;

public class DirectoryInformationDialogViewModelTests
{
    private const string Directory = "Dir";
    private const long Size = 42;
    private const int FilesCount = 13;
    private const int DirsCount = 31;

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
    public async Task TestActivation()
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();

        var directoryModel = new DirectoryModel {FullPath = Directory};
        _autoMocker
            .Setup<IDirectoryService, DirectoryModel>(m => m.GetDirectory(Directory))
            .Returns(directoryModel);
        _autoMocker
            .Setup<IDirectoryService, long>(m => m.CalculateSize(Directory))
            .Returns(Size);
        _autoMocker
            .Setup<IApplicationDispatcher>(m => m.Dispatch(It.IsAny<Action>()))
            .Callback<Action>(action =>
            {
                action();
                taskCompletionSource.SetResult(true);
            });
        _autoMocker
            .Setup<IDirectoryService, IReadOnlyList<DirectoryModel>>(m => m.GetChildDirectories(Directory, null))
            .Returns(Enumerable.Repeat(new DirectoryModel(), DirsCount).ToArray());
        _autoMocker
            .Setup<IFileService, IReadOnlyList<FileModel>>(m => m.GetFiles(Directory, null))
            .Returns(Enumerable.Repeat(new FileModel(), FilesCount).ToArray());
        _autoMocker
            .Setup<IMainNodeInfoTabViewModel>(m => m.SetSize(Size))
            .Verifiable();
        _autoMocker
            .Setup<IMainNodeInfoTabViewModel>(m => m.Activate(directoryModel, true, FilesCount, DirsCount))
            .Verifiable();

        var viewModel = _autoMocker.CreateInstance<DirectoryInformationDialogViewModel>();
        var parameter = new FileSystemNodeNavigationParameter(Directory);
        viewModel.Activate(parameter);

        await Task.WhenAny(taskCompletionSource.Task, Task.Delay(1000));

        _autoMocker
            .Verify<IMainNodeInfoTabViewModel>(m => m.SetSize(Size),
                Times.Once);
        _autoMocker
            .Verify<IMainNodeInfoTabViewModel>(m => m.Activate(directoryModel, true, FilesCount, DirsCount),
                Times.Once);
    }
}