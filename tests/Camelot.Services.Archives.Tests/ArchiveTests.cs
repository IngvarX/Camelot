using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Enums;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Archives.Tests
{
    public class ArchiveTests : IDisposable
    {
        private const string TestDirName = "Dir";
        private const string TestExtractDirName = "ExtractDir";
        private const string TestInnerDirName = "InnerDir";
        private const string TestInnerFileName = "InnerFile.txt";
        private const string TestInnerFileText = "TestInnerFileText";
        private const string TestInnerDirFileName = "InnerDirFile.txt";
        private const string TestInnerDirFileText = "TestInnerDirFileText";

        private readonly AutoMocker _autoMocker;
        private readonly string _testDirectory;
        private readonly string _testInnerDirectory;
        private readonly string _testInnerFile;
        private readonly string _testInnerDirectoryFile;
        private readonly string _testExtractDirectory;
        private readonly string _newInnerFile;
        private readonly string _newInnerDirFile;

        public ArchiveTests()
        {
            _autoMocker = new AutoMocker();
            _testDirectory = Path.Combine(Directory.GetCurrentDirectory(), TestDirName);
            _testInnerDirectory = Path.Combine(_testDirectory, TestInnerDirName);
            _testInnerFile = Path.Combine(_testDirectory, TestInnerFileName);
            _testInnerDirectoryFile = Path.Combine(_testInnerDirectory, TestInnerDirFileName);
            _testExtractDirectory = Path.Combine(_testInnerDirectory, TestExtractDirName);
            _newInnerFile = Path.Combine(_testExtractDirectory, TestInnerFileName);
            _newInnerDirFile = Path.Combine(_testExtractDirectory, TestInnerDirName, TestInnerDirFileName);

            CreateNodes();
            Setup();
        }

        [Theory]
        [InlineData("archive.tar", ArchiveType.Tar)]
        [InlineData("archive.zip", ArchiveType.Zip)]
        [InlineData("archive.tar.gz", ArchiveType.TarGz)]
        [InlineData("archive.tar.bz2", ArchiveType.TarBz2)]
        [InlineData("archive.tar.lz", ArchiveType.TarLz)]
        public async Task TestArchiveCreateExtractMultipleFiles(string outputFileName, ArchiveType archiveType)
        {
            var archivePath = Path.Combine(_testDirectory, outputFileName);

            var factory = _autoMocker.CreateInstance<ArchiveProcessorFactory>();
            var writer = factory.CreateWriter(archiveType);
            Assert.NotNull(writer);

            await writer.PackAsync(new[] {_testInnerFile}, new[] {_testInnerDirectory},
                _testDirectory, archivePath);

            var isArchiveExists = File.Exists(archivePath);
            Assert.True(isArchiveExists);

            var reader = factory.CreateReader(archiveType);
            Assert.NotNull(reader);

            await reader.ExtractAsync(archivePath, _testExtractDirectory);

            Assert.True(File.Exists(_newInnerFile));
            Assert.Equal(TestInnerFileText, await File.ReadAllTextAsync(_newInnerFile));

            Assert.True(File.Exists(_newInnerDirFile));
            Assert.Equal(TestInnerDirFileText, await File.ReadAllTextAsync(_newInnerDirFile));
        }

        [Theory]
        [InlineData("archive.tar", ArchiveType.Tar)]
        [InlineData("archive.zip", ArchiveType.Zip)]
        [InlineData("archive.tar.gz", ArchiveType.TarGz)]
        [InlineData("archive.tar.bz2", ArchiveType.TarBz2)]
        [InlineData("archive.tar.lz", ArchiveType.TarLz)]
        [InlineData("archive.gz", ArchiveType.Gz)]
        public async Task TestArchiveCreateExtractSingleFile(string outputFileName, ArchiveType archiveType)
        {
            var archivePath = Path.Combine(_testDirectory, outputFileName);

            var factory = _autoMocker.CreateInstance<ArchiveProcessorFactory>();
            var writer = factory.CreateWriter(archiveType);
            Assert.NotNull(writer);

            await writer.PackAsync(new[] {_testInnerFile}, new string[0],
                _testDirectory, archivePath);

            var isArchiveExists = File.Exists(archivePath);
            Assert.True(isArchiveExists);

            var reader = factory.CreateReader(archiveType);
            Assert.NotNull(reader);

            await reader.ExtractAsync(archivePath, _testExtractDirectory);

            Assert.True(File.Exists(_newInnerFile));
            Assert.Equal(TestInnerFileText, await File.ReadAllTextAsync(_newInnerFile));
        }

        public void Dispose() => Directory.Delete(_testDirectory, true);

        private void CreateNodes()
        {
            Directory.CreateDirectory(_testDirectory);
            Directory.CreateDirectory(_testInnerDirectory);
            File.WriteAllText(_testInnerFile, TestInnerFileText);
            File.WriteAllText(_testInnerDirectoryFile, TestInnerDirFileText);
        }

        private void Setup()
        {
            _autoMocker
                .Setup<IFileService, Stream>(m => m.OpenRead(It.IsAny<string>()))
                .Returns<string>(File.OpenRead);
            _autoMocker
                .Setup<IFileService, Stream>(m => m.OpenWrite(It.IsAny<string>()))
                .Returns<string>(File.OpenWrite);
            _autoMocker
                .Setup<IPathService, string>(m => m.GetRelativePath(It.IsAny<string>(), It.IsAny<string>()))
                .Returns<string, string>(Path.GetRelativePath);
            _autoMocker
                .Setup<IDirectoryService, IReadOnlyList<string>>(m => m.GetFilesRecursively(It.IsAny<string>()))
                .Returns<string>(d => Directory
                    .EnumerateFiles(d, "*.*", SearchOption.AllDirectories)
                    .ToArray());
        }
    }
}