using System;
using Camelot.Services.Abstractions.Behaviors;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Comparers;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Nodes;
using Camelot.ViewModels.Interfaces.Behaviors;
using Camelot.ViewModels.Services.Interfaces;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.FilePanels.Comparers;

public class FileSystemNodesComparerTests
{
    private readonly AutoMocker _autoMocker;

    public FileSystemNodesComparerTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Theory]
    [InlineData(true, SortingMode.Date)]
    [InlineData(false, SortingMode.Name)]
    [InlineData(true, SortingMode.Extension)]
    [InlineData(false, SortingMode.Size)]
    public void TestSortingFileAndDirectory(bool isAscending, SortingMode sortingColumn)
    {
        _autoMocker.Use(true);
        _autoMocker.Use(IconsType.Builtin);

        var directoryViewModel =  _autoMocker.CreateInstance<DirectoryViewModel>();
        var fileViewModel =  _autoMocker.CreateInstance<FileViewModel>();

        var comparer = new FileSystemNodesComparer(isAscending, sortingColumn);

        var result = comparer.Compare(directoryViewModel, fileViewModel);
        Assert.True(result < 0);

        result = comparer.Compare(fileViewModel, directoryViewModel);
        Assert.True(result > 0);
    }

    [Theory]
    [InlineData(true, SortingMode.Date)]
    [InlineData(false, SortingMode.Name)]
    [InlineData(true, SortingMode.Extension)]
    [InlineData(false, SortingMode.Size)]
    public void TestThrows(bool isAscending, SortingMode sortingColumn)
    {
        _autoMocker.Use(true);
        _autoMocker.Use(IconsType.Builtin);

        var directoryViewModel = _autoMocker.CreateInstance<DirectoryViewModel>();
        var nodeViewModel = _autoMocker.CreateInstance<NodeViewModel>();

        var comparer = new FileSystemNodesComparer(isAscending, sortingColumn);

        void Compare() => comparer.Compare(nodeViewModel, directoryViewModel);

        Assert.Throws<InvalidOperationException>(Compare);

        void CompareReversed() => comparer.Compare(directoryViewModel, nodeViewModel);

        Assert.Throws<InvalidOperationException>(CompareReversed);
    }

    private class NodeViewModel : FileSystemNodeViewModelBase
    {
        public NodeViewModel(
            IFileSystemNodeOpeningBehavior fileSystemNodeOpeningBehavior,
            IFileSystemNodePropertiesBehavior fileSystemNodePropertiesBehavior,
            IFileSystemNodeFacade fileSystemNodeFacade,
            bool shouldShowOpenSubmenu)
            : base(
                fileSystemNodeOpeningBehavior,
                fileSystemNodePropertiesBehavior,
                fileSystemNodeFacade,
                shouldShowOpenSubmenu)
        {

        }
    }
}