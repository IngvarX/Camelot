using System.Collections.Generic;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.Operations;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Operations.Tests;

public class CompositeOperationTests
{
    private const string SourceName = "Source";
    private const string DestinationName = "Destination";

    private readonly AutoMocker _autoMocker;

    public CompositeOperationTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Fact]
    public void TestProperties()
    {
        var settings = new BinaryFileSystemOperationSettings(
            new string[] { },
            new[] {SourceName},
            new string[] { },
            new[] {DestinationName},
            new Dictionary<string, string>
            {
                [SourceName] = DestinationName
            },
            new string[] { }
        );
        var operationInfo = new OperationInfo(OperationType.Copy, settings);
        _autoMocker.Use(operationInfo);

        var operation = _autoMocker.CreateInstance<CompositeOperation>();

        Assert.Equal(operationInfo, operation.Info);
        Assert.Equal(default, operation.CurrentBlockedFile);
    }
}