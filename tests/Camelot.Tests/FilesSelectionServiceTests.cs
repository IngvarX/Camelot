using System.Linq;
using Camelot.Services.Implementations;
using Camelot.Services.Interfaces;
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
            var files = Enumerable
                .Range(0, filesCount)
                .Select(i => i.ToString())
                .ToArray();

            _filesSelectionService.SelectFiles(files);

            var selectedFiles = _filesSelectionService.SelectedFiles;
            Assert.True(selectedFiles.Count == filesCount);

            Assert.True(files.All(fn => selectedFiles.Contains(fn)));

            _filesSelectionService.SelectFiles(selectedFiles);
            Assert.True(_filesSelectionService.SelectedFiles.Count == filesCount);
        }

        [Fact]
        public void TestFilesUnselection()
        {
            const int filesCount = 10;
            var files = Enumerable
                .Range(0, filesCount)
                .Select(i => i.ToString())
                .ToArray();

            _filesSelectionService.SelectFiles(files);
            const int filesToUnselectCount = 3;
            _filesSelectionService.UnselectFiles(files.Take(filesToUnselectCount));

            var selectedFiles = _filesSelectionService.SelectedFiles;
            Assert.True(selectedFiles.Count == filesCount - filesToUnselectCount);

            Assert.True(files.Skip(filesToUnselectCount).All(fn => selectedFiles.Contains(fn)));
        }
    }
}