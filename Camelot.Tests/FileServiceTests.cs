using System;
using System.IO;
using System.Linq;
using Camelot.Services.Implementations;
using Camelot.Services.Interfaces;
using Camelot.Services.Models;
using Xunit;

namespace Camelot.Tests
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
            _fileService = new FileService();

            CreateFile();
        }

        [Fact]
        public void TestGetFiles()
        {
            var files = _fileService.GetFiles(CurrentDirectory);

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
            var files = _fileService.GetFiles(CurrentDirectory);

            Assert.NotNull(files);
            Assert.DoesNotContain(files, f => f.Name == FileName + FileExtension);
        }

        private static void CreateFile()
        {
            var file = File.Create(FilePath);
            file.Close();
        }

        public void Dispose()
        {
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
        }
    }
}