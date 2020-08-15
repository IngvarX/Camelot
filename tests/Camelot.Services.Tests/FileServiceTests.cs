using System;
using System.IO;
using System.Threading.Tasks;
using Camelot.Extensions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Specifications;
using Camelot.Services.Environment.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Tests
{
    public class FileServiceTests : IDisposable
    {
        private const string DirectoryName = "Dir";
        private const string FileName = "File";
        private const string NewFileName = "NewFile";

        private readonly AutoMocker _autoMocker;
        private readonly string[] _files;

        public FileServiceTests()
        {
            _autoMocker = new AutoMocker();
            _files = new[] {"1", "2", "3"};

            _files.ForEach(f => File.Create(f));
        }

        [Theory]
        [InlineData("", 3)]
        [InlineData("1", 1)]
        [InlineData("4", 0)]
        public void TestGetFiles(string patternToSearch, int resultsCount)
        {
            _autoMocker
                .Setup<IEnvironmentFileService, string[]>(m => m.GetFiles(DirectoryName))
                .Returns(_files);
            _autoMocker
                .Setup<IEnvironmentFileService, FileInfo>(m => m.GetFile(It.IsAny<string>()))
                .Returns<string>(f => new FileInfo(f));
            var fileService = _autoMocker.CreateInstance<FileService>();
            var fileModels = fileService.GetFiles(DirectoryName, GetSpecification(patternToSearch));

            Assert.NotNull(fileModels);
            Assert.Equal(resultsCount, fileModels.Count);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestCheckIfExists(bool exists)
        {
            _autoMocker
                .Setup<IEnvironmentFileService, bool>(m => m.CheckIfExists(FileName))
                .Returns(exists);
            var fileService = _autoMocker.CreateInstance<FileService>();
            var result = fileService.CheckIfExists(FileName);

            Assert.Equal(exists, result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TestCopy(bool overwrite)
        {
            _autoMocker
                .Setup<IEnvironmentFileService>(m => m.Copy(FileName, NewFileName, overwrite))
                .Verifiable();
            var fileService = _autoMocker.CreateInstance<FileService>();
            await fileService.CopyAsync(FileName, NewFileName, overwrite);

            _autoMocker
                .Verify<IEnvironmentFileService>(m => m.Copy(FileName, NewFileName, overwrite));
        }

        [Fact]
        public void TestRemove()
        {
            _autoMocker
                .Setup<IEnvironmentFileService>(m => m.Delete(FileName))
                .Verifiable();
            var fileService = _autoMocker.CreateInstance<FileService>();
            fileService.Remove(FileName);

            _autoMocker
                .Verify<IEnvironmentFileService>(m => m.Delete(FileName));
        }

        [Fact]
        public async Task TestWriteText()
        {
            const string text = "text";
            _autoMocker
                .Setup<IEnvironmentFileService>(m => m.WriteTextAsync(FileName, text))
                .Verifiable();
            var fileService = _autoMocker.CreateInstance<FileService>();
            await fileService.WriteTextAsync(FileName, text);

            _autoMocker
                .Verify<IEnvironmentFileService>(m => m.WriteTextAsync(FileName, text));
        }

        [Fact]
        public async Task TestWriteBites()
        {
            var bytes = new byte[] {1, 2, 5};
            _autoMocker
                .Setup<IEnvironmentFileService>(m => m.WriteBytesAsync(FileName, bytes))
                .Verifiable();
            var fileService = _autoMocker.CreateInstance<FileService>();
            await fileService.WriteBytesAsync(FileName, bytes);

            _autoMocker
                .Verify<IEnvironmentFileService>(m => m.WriteBytesAsync(FileName, bytes));
        }

        public void Dispose()
        {
            _files.ForEach(File.Delete);
        }

        private static ISpecification<NodeModelBase> GetSpecification(string pattern) =>
            new TestSpecification(pattern);

        private class TestSpecification : ISpecification<NodeModelBase>
        {
            private readonly string _text;

            public TestSpecification(string text)
            {
                _text = text;
            }

            public bool IsSatisfiedBy(NodeModelBase nodeModel) =>
                _text == string.Empty || nodeModel.Name == _text;
        }
    }
}