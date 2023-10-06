using System;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Comparers;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Nodes;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.FilePanels.Comparers;

public class FileViewModelsComparerTests
{
    private readonly AutoMocker _autoMocker;

    public FileViewModelsComparerTests()
    {
        _autoMocker = new AutoMocker();
        _autoMocker.Use(true);
        _autoMocker.Use(IconsType.Builtin);
    }

    [Theory]
    [InlineData(true, SortingMode.Date, false)]
    [InlineData(false, SortingMode.Date, true)]
    [InlineData(false, SortingMode.Name, false)]
    [InlineData(true, SortingMode.Name, true)]
    [InlineData(true, SortingMode.Extension, false)]
    [InlineData(false, SortingMode.Extension, true)]
    [InlineData(false, SortingMode.Size, true)]
    [InlineData(true, SortingMode.Size, false)]
    public void TestSorting(bool isAscending, SortingMode sortingColumn, bool expected)
    {
        var dateTime = DateTime.UtcNow;

        var firstFileViewModel = _autoMocker.CreateInstance<FileViewModel>();
        firstFileViewModel.Name = "Program";
        firstFileViewModel.Extension = "cs";
        firstFileViewModel.Size = 1024;
        firstFileViewModel.LastModifiedDateTime = dateTime;

        var secondFileViewModel = _autoMocker.CreateInstance<FileViewModel>();
        secondFileViewModel.Name = "module";
        secondFileViewModel.Extension = "js";
        secondFileViewModel.Size = 2048;
        secondFileViewModel.LastModifiedDateTime = dateTime.AddDays(1);

        var comparer = new FileViewModelsComparer(isAscending, sortingColumn);

        var result = comparer.Compare(firstFileViewModel, secondFileViewModel);
        Assert.Equal(expected, result > 0);

        result = comparer.Compare(secondFileViewModel, firstFileViewModel);
        Assert.Equal(expected, result < 0);
    }

    [Fact]
    public void TestThrows()
    {
        var comparer = new FileViewModelsComparer(false, (SortingMode) 42);
        var firstFileViewModel = _autoMocker.CreateInstance<FileViewModel>();
        var secondFileViewModel = _autoMocker.CreateInstance<FileViewModel>();

        void Compare() => comparer.Compare(firstFileViewModel, secondFileViewModel);

        Assert.Throws<ArgumentOutOfRangeException>(Compare);
    }
}