using System;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Comparers;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Nodes;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.FilePanels.Comparers;

public class DirectoryViewModelsComparerTests
{
    private readonly AutoMocker _autoMocker;

    public DirectoryViewModelsComparerTests()
    {
        _autoMocker = new AutoMocker();
        _autoMocker.Use(true);
    }

    [Theory]
    [InlineData(true, SortingMode.Date)]
    [InlineData(false, SortingMode.Name)]
    [InlineData(true, SortingMode.Extension)]
    [InlineData(false, SortingMode.Size)]
    public void TestSortingParentDirectory(bool isAscending, SortingMode sortingColumn)
    {
        var parentDirectoryViewModel = _autoMocker.CreateInstance<DirectoryViewModel>();
        parentDirectoryViewModel.IsParentDirectory = true;
        var directoryViewModel = _autoMocker.CreateInstance<DirectoryViewModel>();

        var comparer = new DirectoryViewModelsComparer(isAscending, sortingColumn);

        var result = comparer.Compare(parentDirectoryViewModel, directoryViewModel);
        Assert.True(result < 0);

        result = comparer.Compare(directoryViewModel, parentDirectoryViewModel);
        Assert.True(result > 0);
    }

    [Theory]
    [InlineData(true, SortingMode.Date, false)]
    [InlineData(false, SortingMode.Date, true)]
    [InlineData(false, SortingMode.Name, true)]
    [InlineData(true, SortingMode.Name, false)]
    [InlineData(true, SortingMode.Extension, false)]
    [InlineData(false, SortingMode.Extension, true)]
    [InlineData(false, SortingMode.Size, true)]
    [InlineData(true, SortingMode.Size, false)]
    public void TestSorting(bool isAscending, SortingMode sortingColumn, bool expected)
    {
        var dateTime = DateTime.UtcNow;

        var firstViewModel = _autoMocker.CreateInstance<DirectoryViewModel>();
        firstViewModel.Name = "Camelot";
        firstViewModel.LastModifiedDateTime = dateTime;

        var secondViewModel = _autoMocker.CreateInstance<DirectoryViewModel>();
        secondViewModel.Name = "Code";
        secondViewModel.LastModifiedDateTime = dateTime.AddDays(1);

        var comparer = new DirectoryViewModelsComparer(isAscending, sortingColumn);

        var result = comparer.Compare(firstViewModel, secondViewModel);
        Assert.Equal(expected, result > 0);

        result = comparer.Compare(secondViewModel, firstViewModel);
        Assert.Equal(expected, result < 0);
    }

    [Fact]
    public void TestThrows()
    {
        var comparer = new DirectoryViewModelsComparer(false, (SortingMode) 42);
        var firstViewModel = _autoMocker.CreateInstance<DirectoryViewModel>();
        var secondViewModel = _autoMocker.CreateInstance<DirectoryViewModel>();

        void Compare() => comparer.Compare(firstViewModel, secondViewModel);

        Assert.Throws<ArgumentOutOfRangeException>(Compare);
    }
}