using System.Linq;
using Camelot.Services.Implementations;
using Camelot.Services.Interfaces;
using Camelot.Services.Models;
using Xunit;

namespace Camelot.Tests
{
    public class FilesSelectionServiceTests
    {
        private readonly IFilesSelectionService _filesSelectionService;

        public FilesSelectionServiceTests()
        {
            _filesSelectionService = new FilesSelectionService();
        }

        [Fact]
        public void TestFilesSelection()
        {
            const int filesCount = 10;
            var filesNames = Enumerable
                .Range(0, filesCount)
                .Select(i => i.ToString())
                .ToArray();
            var files = filesNames.Select(n => new FileModel {Name = n});

            _filesSelectionService.SelectFiles(files);

            var selectedFiles = _filesSelectionService.SelectedFiles;
            Assert.True(selectedFiles.Count == filesCount);

            var selectedFilesNames = selectedFiles.Select(f => f.Name);
            Assert.True(filesNames.All(fn => selectedFilesNames.Contains(fn)));
        }

        [Fact]
        public void TestFilesUnselection()
        {
            const int filesCount = 10;
            var filesNames = Enumerable
                .Range(0, filesCount)
                .Select(i => i.ToString())
                .ToArray();
            var files = filesNames.Select(n => new FileModel {Name = n}).ToArray();

            _filesSelectionService.SelectFiles(files);
            const int filesToUnselectCount = 3;
            _filesSelectionService.UnselectFiles(files.Take(filesToUnselectCount));

            var selectedFiles = _filesSelectionService.SelectedFiles;
            Assert.True(selectedFiles.Count == filesCount - filesToUnselectCount);

            var selectedFilesNames = selectedFiles.Select(f => f.Name);
            Assert.True(filesNames.Skip(filesToUnselectCount).All(fn => selectedFilesNames.Contains(fn)));
        }
    }
}