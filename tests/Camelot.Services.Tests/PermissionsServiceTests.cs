using System;
using Camelot.Services.Environment.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Tests;

public class PermissionsServiceTests
{
    private const string Directory = "Dir";
        
    private readonly AutoMocker _autoMocker;

    public PermissionsServiceTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Fact]
    public void TestCheckIfHasAccessSuccess()
    {
        var service = _autoMocker.CreateInstance<PermissionsService>();
        var result = service.CheckIfHasAccess(Directory);
            
        Assert.True(result);
            
        _autoMocker
            .Verify<IEnvironmentDirectoryService, string[]>(m => m.GetDirectories(Directory),
                Times.Once);
    }
        
    [Fact]
    public void TestCheckIfHasAccessFail()
    {
        _autoMocker
            .Setup<IEnvironmentDirectoryService, string[]>(m => m.GetDirectories(Directory))
            .Throws<UnauthorizedAccessException>();
            
        var service = _autoMocker.CreateInstance<PermissionsService>();
        var result = service.CheckIfHasAccess(Directory);
            
        Assert.False(result);
    }
}