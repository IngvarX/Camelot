using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Factories.Implementations;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.Factories
{
    public class FileSystemNodeViewModelFactoryTests
    {
        private readonly AutoMocker _autoMocker;

        public FileSystemNodeViewModelFactoryTests()
        {
            _autoMocker = new AutoMocker();
        }

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

            _autoMocker
                .Setup<IPathService, string>(m => m.GetFileName(fileModel.Name))
                .Returns(name);
            _autoMocker
                .Setup<IPathService, string>(m => m.GetFileNameWithoutExtension(fileModel.Name))
                .Returns(name);
            _autoMocker
                .Setup<IPathService, string>(m => m.GetExtension(fileModel.Name))
                .Returns(extension);

            var factory = _autoMocker.CreateInstance<FileSystemNodeViewModelFactory>();

            var node = factory.Create(fileModel);

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

            var factory = _autoMocker.CreateInstance<FileSystemNodeViewModelFactory>();

            var node = factory.Create(directoryModel, isParentDirectory);

            Assert.Equal(node.Name, name);
            Assert.Equal(node.FullPath, fullPath);
            Assert.False(node.IsEditing);
            Assert.False(node.IsWaitingForEdit);
        }
    }
}