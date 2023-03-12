using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Drives;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Operations;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.AllPlatforms.Tests;

public class TrashCanServiceBaseTests
{
    private const string FilePath = "/home/file.txt";
    
    private readonly AutoMocker _autoMocker;

    public TrashCanServiceBaseTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Theory]
    [InlineData(true, 0)]
    [InlineData(true, 1)]
    [InlineData(false, 2)]
    public async Task TestMoveToTrashFirstFails(bool expected, int failuresCount)
    {
        var moveResults = new bool[2];
        for (var i = 0; i < moveResults.Length - failuresCount; i++)
        {
            moveResults[moveResults.Length - 1 - i] = true;
        }

        _autoMocker
            .SetupSequence<IOperationsService, Task<bool>>(m => m.MoveAsync(
                It.Is<IReadOnlyDictionary<string, string>>(d => true)))
            .ReturnsAsync(moveResults[0])
            .ReturnsAsync(moveResults[1]);
        _autoMocker
            .Setup<IMountedDriveService, DriveModel>(m => m.GetFileDrive(It.IsAny<string>()))
            .Returns(new DriveModel {RootDirectory = "/"});
        
        var trashCanService = _autoMocker.CreateInstance<MockTrashCanService>();
        
        var actual = await trashCanService.MoveToTrashAsync(new[] { FilePath }, CancellationToken.None);
        Assert.Equal(actual, expected);
    }

    private class MockTrashCanService : TrashCanServiceBase
    {
        public MockTrashCanService(
            IMountedDriveService mountedDriveService,
            IOperationsService operationsService,
            IPathService pathService) 
            : base(mountedDriveService, operationsService, pathService)
        {
            
        }

        protected override IReadOnlyList<string> GetTrashCanLocations(string volume) =>
            new[] { "1", "2" };
        
        protected override Task WriteMetaDataAsync(
            IReadOnlyDictionary<string, string> filePathsDictionary, string trashCanLocation) =>
            Task.CompletedTask;

        protected override string GetUniqueFilePath(string fileName, HashSet<string> filesNamesSet, string directory) =>
            fileName;
    }
}