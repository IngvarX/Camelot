using System;
using System.IO;
using System.Threading.Tasks;
using Camelot.Extensions;
using Camelot.Services.Abstractions;
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
        private const string NewFilePath = "NewFilePath";

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

        [Fact]
        public async Task TestCopyNoOverwrite()
        {
            _autoMocker
                .Setup<IEnvironmentFileService>(m => m.OpenRead(FileName))
                .Verifiable();
            _autoMocker
                .Setup<IEnvironmentFileService, bool>(m => m.CheckIfExists(NewFileName))
                .Returns(true);

            var fileService = _autoMocker.CreateInstance<FileService>();
            var result = await fileService.CopyAsync(FileName, NewFileName, false);

            Assert.False(result);

            _autoMocker
                .Verify<IEnvironmentFileService>(m => m.OpenRead(FileName),
                Times.Never);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public async Task TestCopy(bool throws, bool expected)
        {
            var inStream = new MemoryStream();
            var outStream = new MemoryStream();

            _autoMocker
                .Setup<IEnvironmentFileService, Stream>(m => m.OpenRead(FileName))
                .Returns(inStream)
                .Verifiable();
            _autoMocker
                .Setup<IEnvironmentFileService, Stream>(m => m.OpenWrite(NewFileName))
                .Returns(outStream)
                .Verifiable();
            if (throws)
            {
                _autoMocker
                    .Setup<IEnvironmentFileService>(m => m.OpenRead(FileName))
                    .Throws<InvalidOperationException>();
            }

            var fileService = _autoMocker.CreateInstance<FileService>();
            var result = await fileService.CopyAsync(FileName, NewFileName, false);

            Assert.Equal(expected, result);

            if (!throws)
            {
                _autoMocker
                    .Verify<IEnvironmentFileService, Stream>(m => m.OpenRead(FileName),
                        Times.Once);
                _autoMocker
                    .Verify<IEnvironmentFileService, Stream>(m => m.OpenWrite(NewFileName),
                        Times.Once);
            }
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
                .Verify<IEnvironmentFileService>(m => m.WriteTextAsync(FileName, text),
                    Times.Once);
        }

        [Fact]
        public async Task TestWriteBytes()
        {
            var bytes = new byte[] {1, 2, 5};
            _autoMocker
                .Setup<IEnvironmentFileService>(m => m.WriteBytesAsync(FileName, bytes))
                .Verifiable();
            var fileService = _autoMocker.CreateInstance<FileService>();
            await fileService.WriteBytesAsync(FileName, bytes);

            _autoMocker
                .Verify<IEnvironmentFileService>(m => m.WriteBytesAsync(FileName, bytes),
                    Times.Once);
        }

        [Fact]
        public void TestOpenRead()
        {
            _autoMocker
                .Setup<IEnvironmentFileService, Stream>(m => m.OpenRead(FileName))
                .Verifiable();
            var fileService = _autoMocker.CreateInstance<FileService>();
            fileService.OpenRead(FileName);

            _autoMocker
                .Verify<IEnvironmentFileService>(m => m.OpenRead(FileName),
                    Times.Once);
        }

        [Fact]
        public void TestRename()
        {
            _autoMocker
                .Setup<IEnvironmentFileService>(m => m.Move(FileName, NewFilePath))
                .Verifiable();
            _autoMocker
                .Setup<IPathService, string>(m => m.GetParentDirectory(FileName))
                .Returns(DirectoryName);
            _autoMocker
                .Setup<IPathService, string>(m => m.Combine(DirectoryName, NewFileName))
                .Returns(NewFilePath);
            _autoMocker.MockLogError();

            var fileService = _autoMocker.CreateInstance<FileService>();
            var result = fileService.Rename(FileName, NewFileName);

            Assert.True(result);

            _autoMocker
                .Verify<IEnvironmentFileService>(m => m.Move(FileName, NewFilePath),
                    Times.Once);
            _autoMocker.VerifyLogError(Times.Never());
        }

        [Fact]
        public void TestRenameFailed()
        {
            _autoMocker
                .Setup<IEnvironmentFileService>(m => m.Move(FileName, NewFilePath))
                .Throws<InvalidOperationException>()
                .Verifiable();
            _autoMocker
                .Setup<IPathService, string>(m => m.GetParentDirectory(FileName))
                .Returns(DirectoryName);
            _autoMocker
                .Setup<IPathService, string>(m => m.Combine(DirectoryName, NewFileName))
                .Returns(NewFilePath);
            _autoMocker.MockLogError();

            var fileService = _autoMocker.CreateInstance<FileService>();
            var result = fileService.Rename(FileName, NewFileName);

            Assert.False(result);

            _autoMocker
                .Verify<IEnvironmentFileService>(m => m.Move(FileName, NewFilePath),
                    Times.Once);
            _autoMocker.VerifyLogError(Times.Once());
        }

        [Fact]
        public void TestOpenWrite()
        {
            _autoMocker
                .Setup<IEnvironmentFileService>(m => m.OpenWrite(FileName))
                .Verifiable();
            var fileService = _autoMocker.CreateInstance<FileService>();
            fileService.OpenWrite(FileName);

            _autoMocker
                .Verify<IEnvironmentFileService>(m => m.OpenWrite(FileName),
                    Times.Once);
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