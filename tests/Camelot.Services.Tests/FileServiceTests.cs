using System;
using System.IO;
using System.Threading.Tasks;
using Camelot.Extensions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Specifications;
using Camelot.Services.Environment.Interfaces;
using Camelot.Tests.Common.Extensions;
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

            _files.ForEach(Create);
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
        [InlineData(true, true, false)]
        [InlineData(true, false, true)]
        [InlineData(false, true, false)]
        [InlineData(false, false, true)]
        public async Task TestCopy(bool overwrite, bool throws, bool expected)
        {
            _autoMocker
                .Setup<IEnvironmentFileService>(m => m.Copy(FileName, NewFileName, overwrite))
                .Verifiable();
            if (throws)
            {
                _autoMocker
                    .Setup<IEnvironmentFileService>(m => m.Copy(FileName, NewFileName, overwrite))
                    .Throws<InvalidOperationException>();
            }

            var fileService = _autoMocker.CreateInstance<FileService>();
            var result = await fileService.CopyAsync(FileName, NewFileName, overwrite);

            Assert.Equal(expected, result);
            _autoMocker
                .Verify<IEnvironmentFileService>(m => m.Copy(FileName, NewFileName, overwrite));
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void TestRemove(bool throws, bool expected)
        {
            _autoMocker
                .Setup<IEnvironmentFileService>(m => m.Delete(FileName))
                .Verifiable();
            if (throws)
            {
                _autoMocker
                    .Setup<IEnvironmentFileService>(m => m.Delete(FileName))
                    .Throws<InvalidOperationException>();
            }

            var fileService = _autoMocker.CreateInstance<FileService>();
            var result = fileService.Remove(FileName);

            Assert.Equal(expected, result);
            _autoMocker
                .Verify<IEnvironmentFileService>(m => m.Delete(FileName));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestCreateFile(bool throws)
        {
            _autoMocker
                .Setup<IEnvironmentFileService>(m => m.Create(FileName))
                .Verifiable();
            if (throws)
            {
                _autoMocker
                    .Setup<IEnvironmentFileService>(m => m.Create(FileName))
                    .Throws<InvalidOperationException>();
            }

            _autoMocker.MockLogError();

            var fileService = _autoMocker.CreateInstance<FileService>();
            fileService.CreateFile(FileName);

            _autoMocker
                .Verify<IEnvironmentFileService>(m => m.Create(FileName), Times.Once);
            _autoMocker.VerifyLogError(throws ? Times.Once() : Times.Never());
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

        private static void Create(string file) => File.Create(file).Dispose();

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