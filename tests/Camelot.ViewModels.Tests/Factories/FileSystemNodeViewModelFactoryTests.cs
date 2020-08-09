using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Behaviors;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Operations;
using Camelot.ViewModels.Factories.Implementations;
using Camelot.ViewModels.Interfaces.Behaviors;
using Camelot.ViewModels.Services.Interfaces;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests.Factories
{
    public class FileSystemNodeViewModelFactoryTests
    {
        [Fact]
        public void TestCreateFile()
        {
            const string name = "Name";
            const string extension = "txt";
            const string fullPath = "fullPath";
            var fileModel = new FileModel
            {
                Name = name,
                FullPath = fullPath
            };

            var fileSystemNodeOpeningBehaviorMock = new Mock<IFileSystemNodeOpeningBehavior>();
            var fileSizeFormatterMock = new Mock<IFileSizeFormatter>();
            var pathServiceMock = new Mock<IPathService>();
            pathServiceMock
                .Setup(m => m.GetFileName(fileModel.Name))
                .Returns(name);
            pathServiceMock
                .Setup(m => m.GetFileNameWithoutExtension(fileModel.Name))
                .Returns(name);
            pathServiceMock
                .Setup(m => m.GetExtension(fileModel.Name))
                .Returns(extension);
            var operationsServiceMock = new Mock<IOperationsService>();
            var clipboardOperationsServiceMock = new Mock<IClipboardOperationsService>();
            var filesOperationsMediatorMock = new Mock<IFilesOperationsMediator>();
            var filesPropertiesBehaviorMock = new Mock<IFileSystemNodePropertiesBehavior>();
            var directoriesPropertiesBehaviorMock = new Mock<IFileSystemNodePropertiesBehavior>();
            var dialogServiceMock = new Mock<IDialogService>();
            var trashCanServiceMock = new Mock<ITrashCanService>();
            var fileSystemNodeViewModelFactory = new FileSystemNodeViewModelFactory(
                fileSystemNodeOpeningBehaviorMock.Object,
                fileSystemNodeOpeningBehaviorMock.Object,
                fileSizeFormatterMock.Object,
                pathServiceMock.Object,
                operationsServiceMock.Object,
                clipboardOperationsServiceMock.Object,
                filesOperationsMediatorMock.Object,
                filesPropertiesBehaviorMock.Object,
                directoriesPropertiesBehaviorMock.Object,
                dialogServiceMock.Object,
                trashCanServiceMock.Object
            );

            var node = fileSystemNodeViewModelFactory.Create(fileModel);

            Assert.Equal(node.Name, name);
            Assert.Equal(node.FullPath, fullPath);
            Assert.False(node.IsEditing);
            Assert.False(node.IsWaitingForEdit);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestCreateDirectory(bool isParentDirectory)
        {
            const string name = "Name";
            const string fullPath = "fullPath";
            var directoryModel = new DirectoryModel
            {
                Name = name,
                FullPath = fullPath
            };

            var fileSystemNodeOpeningBehaviorMock = new Mock<IFileSystemNodeOpeningBehavior>();
            var fileSizeFormatterMock = new Mock<IFileSizeFormatter>();
            var pathServiceMock = new Mock<IPathService>();
            var operationsServiceMock = new Mock<IOperationsService>();
            var clipboardOperationsServiceMock = new Mock<IClipboardOperationsService>();
            var filesOperationsMediatorMock = new Mock<IFilesOperationsMediator>();
            var filesPropertiesBehaviorMock = new Mock<IFileSystemNodePropertiesBehavior>();
            var directoriesPropertiesBehaviorMock = new Mock<IFileSystemNodePropertiesBehavior>();
            var dialogServiceMock = new Mock<IDialogService>();
            var trashCanServiceMock = new Mock<ITrashCanService>();
            var fileSystemNodeViewModelFactory = new FileSystemNodeViewModelFactory(
                fileSystemNodeOpeningBehaviorMock.Object,
                fileSystemNodeOpeningBehaviorMock.Object,
                fileSizeFormatterMock.Object,
                pathServiceMock.Object,
                operationsServiceMock.Object,
                clipboardOperationsServiceMock.Object,
                filesOperationsMediatorMock.Object,
                filesPropertiesBehaviorMock.Object,
                directoriesPropertiesBehaviorMock.Object,
                dialogServiceMock.Object,
                trashCanServiceMock.Object
            );

            var node = fileSystemNodeViewModelFactory.Create(directoryModel, isParentDirectory);

            Assert.Equal(node.Name, name);
            Assert.Equal(node.FullPath, fullPath);
            Assert.False(node.IsEditing);
            Assert.False(node.IsWaitingForEdit);
        }
    }
}