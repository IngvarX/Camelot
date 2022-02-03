using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Operations;
using Camelot.ViewModels.Services.Implementations;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.Services;

public class DragAndDropOperationsMediatorTests
{
    private const string Directory = "Dir";
    private const string File = "File";
    private const string ParentDirectory = "Dir";
    private const string FileToProcess = "FileToProcess";

    private readonly AutoMocker _autoMocker;

    public DragAndDropOperationsMediatorTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Theory]
    [InlineData(Directory, true, Directory)]
    [InlineData(File, false, ParentDirectory)]
    public async Task TestCopy(string destination, bool dirExists, string destinationDirectory)
    {
        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(Directory))
            .Returns(dirExists);
        _autoMocker
            .Setup<IPathService, string>(m => m.GetParentDirectory(File))
            .Returns(ParentDirectory);
        _autoMocker
            .Setup<IOperationsService>(m => m.CopyAsync(
                It.Is<IReadOnlyList<string>>(m => m.Single() == FileToProcess), destinationDirectory))
            .Verifiable();

        var viewModel = _autoMocker.CreateInstance<DragAndDropOperationsMediator>();
        var files = new[] {FileToProcess};

        await viewModel.CopyFilesAsync(files, destination);

        _autoMocker
            .Verify<IOperationsService>(m => m.CopyAsync(
                    It.Is<IReadOnlyList<string>>(l => l.Single() == FileToProcess), destinationDirectory),
                Times.Once);
    }

    [Fact]
    public async Task TestCopyNotCalled()
    {
        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(Directory))
            .Returns(true);
        _autoMocker
            .Setup<IPathService, string>(m => m.GetParentDirectory(File))
            .Returns(ParentDirectory);
        _autoMocker
            .Setup<IOperationsService>(m => m.CopyAsync(
                It.IsAny<IReadOnlyList<string>>(), It.IsAny<string>()))
            .Verifiable();

        var viewModel = _autoMocker.CreateInstance<DragAndDropOperationsMediator>();
        var files = new[] {FileToProcess};

        await viewModel.CopyFilesAsync(files, ParentDirectory);

        _autoMocker
            .Verify<IOperationsService>(m => m.CopyAsync(
                    It.IsAny<IReadOnlyList<string>>(), It.IsAny<string>()),
                Times.Once);
    }

    [Theory]
    [InlineData(Directory, true, Directory)]
    [InlineData(File, false, ParentDirectory)]
    public async Task TestMove(string destination, bool dirExists, string destinationDirectory)
    {
        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(Directory))
            .Returns(dirExists);
        _autoMocker
            .Setup<IPathService, string>(m => m.GetParentDirectory(File))
            .Returns(ParentDirectory);
        _autoMocker
            .Setup<IOperationsService>(m => m.MoveAsync(
                It.Is<IReadOnlyList<string>>(l => l.Single() == FileToProcess), destinationDirectory))
            .Verifiable();

        var viewModel = _autoMocker.CreateInstance<DragAndDropOperationsMediator>();
        var files = new[] {FileToProcess};

        await viewModel.MoveFilesAsync(files, destination);

        _autoMocker
            .Verify<IOperationsService>(m => m.MoveAsync(
                    It.Is<IReadOnlyList<string>>(m => m.Single() == FileToProcess), destinationDirectory),
                Times.Once);
    }

    [Fact]
    public async Task TestMoveNotCalled()
    {
        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(Directory))
            .Returns(true);
        _autoMocker
            .Setup<IPathService, string>(m => m.GetParentDirectory(File))
            .Returns(ParentDirectory);
        _autoMocker
            .Setup<IOperationsService>(m => m.CopyAsync(
                It.IsAny<IReadOnlyList<string>>(), It.IsAny<string>()))
            .Verifiable();

        var viewModel = _autoMocker.CreateInstance<DragAndDropOperationsMediator>();
        var files = new[] {FileToProcess};

        await viewModel.MoveFilesAsync(files, ParentDirectory);

        _autoMocker
            .Verify<IOperationsService>(m => m.MoveAsync(
                    It.IsAny<IReadOnlyList<string>>(), It.IsAny<string>()),
                Times.Once);
    }
}