using System.Linq;
using Camelot.Services.Abstractions;
using Xunit;

namespace Camelot.Services.Tests
{
    public class NodesSelectionServiceTests
    {
        private readonly INodesSelectionService _nodesSelectionService;

        public NodesSelectionServiceTests()
        {
            _nodesSelectionService = new NodesSelectionService();
        }

        [Fact]
        public void TestFilesSelection()
        {
            const int filesCount = 10;
            var files = Enumerable
                .Range(0, filesCount)
                .Select(i => i.ToString())
                .ToArray();

            _nodesSelectionService.SelectNodes(files);

            var selectedFiles = _nodesSelectionService.SelectedNodes;
            Assert.True(selectedFiles.Count == filesCount);

            Assert.True(files.All(fn => selectedFiles.Contains(fn)));

            _nodesSelectionService.SelectNodes(selectedFiles);
            Assert.True(_nodesSelectionService.SelectedNodes.Count == filesCount);
        }

        [Fact]
        public void TestFilesUnselection()
        {
            const int filesCount = 10;
            var files = Enumerable
                .Range(0, filesCount)
                .Select(i => i.ToString())
                .ToArray();

            _nodesSelectionService.SelectNodes(files);
            const int filesToUnselectCount = 3;
            _nodesSelectionService.UnselectNodes(files.Take(filesToUnselectCount));

            var selectedFiles = _nodesSelectionService.SelectedNodes;
            Assert.True(selectedFiles.Count == filesCount - filesToUnselectCount);

            Assert.True(files.Skip(filesToUnselectCount).All(fn => selectedFiles.Contains(fn)));
        }
    }
}