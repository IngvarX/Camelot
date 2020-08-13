using System;
using System.IO;
using System.Linq;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Specifications;
using Moq;
using Xunit;

namespace Camelot.Services.Tests
{
    public class FileServiceTests : IDisposable
    {
        private const string FileName = "File.txt";
        private const string FileExtension = ".txt";

        private readonly IFileService _fileService;

        private static string CurrentDirectory => Directory.GetCurrentDirectory();

        private static string FilePath => Path.Combine(CurrentDirectory, FileName);

        public FileServiceTests()
        {
            var pathServiceMock = new Mock<IPathService>();
            pathServiceMock
                .Setup(m => m.GetExtension(FileName))
                .Returns(FileExtension);

            _fileService = new FileService(pathServiceMock.Object);

            CreateFile();
        }

        [Fact]
        public void TestGetFiles()
        {
            var files = _fileService.GetFiles(CurrentDirectory, GetSpecification());

            Assert.NotNull(files);
            Assert.True(files.Any());

            var file = files.Single(f => f.Name == FileName);
            Assert.True(file.Type == FileType.RegularFile);
            Assert.True(file.Extension == FileExtension);
            Assert.True(file.SizeBytes == 0);
            Assert.True(file.FullPath == FilePath);
        }

        [Fact]
        public void TestGetFilesNoResult()
        {
            var files = _fileService.GetFiles(CurrentDirectory, GetSpecification());

            Assert.NotNull(files);
            Assert.DoesNotContain(files, f => f.Name == FileName + FileExtension);
        }

        public void Dispose()
        {
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
        }

        private static void CreateFile()
        {
            var file = File.Create(FilePath);
            file.Close();
        }

        private static ISpecification<NodeModelBase> GetSpecification() =>
            new TestSpecification();

        private class TestSpecification : ISpecification<NodeModelBase>
        {
            public bool IsSatisfiedBy(NodeModelBase nodeModel) => true;
        }
    }
}